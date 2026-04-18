using SITools.BLL.Services;
using SITools.Models.Entities;
using System.Data;

namespace SITools.UI.Forms
{
    public partial class EntPensionForm : Form
    {
        private readonly IPensionCalculationService _pensionSvc;
        private readonly IInterestCalculationService _interestSvc;
        private readonly ILateFeeCalculationService _lateFeeSvc;
        private readonly EntPensionCalculationFacade _calcFacade;
        private readonly ExcelService _excelSvc;

        // 明细数据
        private List<CalculationResultDetail> _details = new();
        // 汇总数据
        private List<CalculationResultSummary> _summaries = new();

        public EntPensionForm(
            IPensionCalculationService pensionSvc,
            IInterestCalculationService interestSvc,
            ILateFeeCalculationService lateFeeSvc,
            EntPensionCalculationFacade calcFacade,
            ExcelService excelSvc)
        {
            _pensionSvc = pensionSvc;
            _interestSvc = interestSvc;
            _lateFeeSvc = lateFeeSvc;
            _calcFacade = calcFacade;
            _excelSvc = excelSvc;

            InitializeComponent();
            InitComboBoxes();
        }

        private void InitComboBoxes()
        {
            var typeNames = _pensionSvc.GetContributionTypeNames();
            cmbContributionType.Items.Clear();
            cmbContributionType.Items.AddRange(typeNames.ToArray<object>());

            cmbApplyLimit.Items.AddRange(new object[] { "是", "否" });
            cmbCalcInterest.Items.AddRange(new object[] { "是", "否" });
            cmbCalcLateFee.Items.AddRange(new object[] { "是", "否" });
        }

        #region 输入区事件

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs(out string errorMsg))
            {
                MessageBox.Show(errorMsg, "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int beginPeriod = int.Parse(txtBegin.Text.Trim());
            int endPeriod = int.Parse(txtEnd.Text.Trim());

            if (!ValidationService.IsPositiveNumber(txtBase.Text.Trim(), out double baseAmount))
            {
                MessageBox.Show("月缴费基数必须为正数！", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string typeName = cmbContributionType.SelectedItem!.ToString()!;
            bool applyLimit = cmbApplyLimit.SelectedItem?.ToString() == "是";

            // 校验补缴类型与时间段的匹配性
            if (!ValidatePeriodAndType(beginPeriod, endPeriod, typeName, out string typeError))
            {
                MessageBox.Show(typeError, "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 添加到输入明细表
            dgvInput.Rows.Add(
                txtName.Text.Trim(),
                txtIdCard.Text.Trim().ToUpper(),
                txtBegin.Text.Trim(),
                txtEnd.Text.Trim(),
                txtBase.Text.Trim(),
                typeName,
                applyLimit ? "是" : "否");

            ClearInputFields();
        }

        private void btnRemoveRow_Click(object sender, EventArgs e)
        {
            if (dgvInput.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvInput.SelectedRows)
                {
                    if (!row.IsNewRow)
                        dgvInput.Rows.Remove(row);
                }
            }
        }

        private void btnImportExcel_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Title = "选择Excel文件",
                Filter = "Excel文件|*.xls;*.xlsx;*.xlsm"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                var dt = _excelSvc.ImportFromExcel(dlg.FileName);
                foreach (DataRow row in dt.Rows)
                {
                    if (dt.Columns.Count >= 7)
                        dgvInput.Rows.Add(row[0], row[1], row[2], row[3], row[4], row[5], row[6]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导入失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 计算按钮

        private void btnCalc_Click(object sender, EventArgs e)
        {
            if (dgvInput.Rows.Count == 0)
            {
                MessageBox.Show("补缴总览表信息为空，请确认是否添加了数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbCalcInterest.SelectedItem == null)
            {
                MessageBox.Show("请选择是否计算利息！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbCalcLateFee.SelectedItem == null)
            {
                MessageBox.Show("请选择是否计算滞纳金！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool calcInterest = cmbCalcInterest.SelectedItem.ToString() == "是";
            bool calcLateFee = cmbCalcLateFee.SelectedItem.ToString() == "是";
            DateTime interestEndDate = dtpInterestEnd.Value;
            DateTime lateFeeEndDate = dtpLateFeeEnd.Value;

            // 构建 ContributionRecord 列表
            var records = new List<ContributionRecord>();
            foreach (DataGridViewRow row in dgvInput.Rows)
            {
                if (row.IsNewRow) continue;
                try
                {
                    string typeName = row.Cells["colType"].Value?.ToString() ?? "";
                    records.Add(new ContributionRecord
                    {
                        Name = row.Cells["colName"].Value?.ToString() ?? "",
                        IdCardNo = row.Cells["colIdCard"].Value?.ToString() ?? "",
                        BeginPeriod = int.Parse(row.Cells["colBegin"].Value?.ToString() ?? "0"),
                        EndPeriod = int.Parse(row.Cells["colEnd"].Value?.ToString() ?? "0"),
                        MonthlyBase = double.Parse(row.Cells["colBase"].Value?.ToString() ?? "0"),
                        ContributionType = _pensionSvc.ParseContributionType(typeName),
                        ApplyBaseLimit = row.Cells["colLimit"].Value?.ToString() == "是",
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"数据行解析失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            try
            {
                _details = _calcFacade.Calculate(records, calcInterest, interestEndDate, calcLateFee, lateFeeEndDate);
                _summaries = _calcFacade.Summarize(_details);

                BindDetailGrid();
                BindSummaryGrid();

                tabControl.SelectedTab = tabDetail;
                btnCalc.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"计算失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 导出按钮

        private void btnExportDetail_Click(object sender, EventArgs e)
        {
            ExportGrid(dgvDetail, "明细数据");
        }

        private void btnExportSummary_Click(object sender, EventArgs e)
        {
            ExportGrid(dgvSummary, "汇总数据");
        }

        private void ExportGrid(DataGridView dgv, string sheetName)
        {
            if (dgv.Rows.Count == 0)
            {
                MessageBox.Show("无数据可导出！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var dlg = new SaveFileDialog
            {
                Title = "保存Excel文件",
                Filter = "Excel文件(*.xlsx)|*.xlsx",
                FileName = sheetName
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                var dt = GridViewToDataTable(dgv);
                _excelSvc.ExportToExcel(dt, dlg.FileName, sheetName);
                MessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导出失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 重置按钮

        private void btnReset_Click(object sender, EventArgs e)
        {
            dgvInput.Rows.Clear();
            dgvDetail.Rows.Clear();
            dgvSummary.DataSource = null;
            _details.Clear();
            _summaries.Clear();
            ClearInputFields();
            cmbCalcInterest.SelectedIndex = -1;
            cmbCalcLateFee.SelectedIndex = -1;
            btnCalc.Enabled = true;
            tabControl.SelectedTab = tabInput;
        }

        #endregion

        #region 辅助方法

        private bool ValidateInputs(out string errorMsg)
        {
            errorMsg = "";

            if (!ValidationService.IsValidPeriod(txtBegin.Text.Trim()))
            {
                errorMsg = "补缴开始时间格式不正确，请输入 yyyyMM 格式（如 202001）！";
                return false;
            }
            if (!ValidationService.IsValidPeriod(txtEnd.Text.Trim()))
            {
                errorMsg = "补缴结束时间格式不正确，请输入 yyyyMM 格式（如 202012）！";
                return false;
            }
            int begin = int.Parse(txtBegin.Text.Trim());
            int end = int.Parse(txtEnd.Text.Trim());
            if (begin > end)
            {
                errorMsg = "补缴开始时间不能大于补缴结束时间！";
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtBase.Text))
            {
                errorMsg = "月缴费基数不能为空！";
                return false;
            }
            if (cmbContributionType.SelectedItem == null)
            {
                errorMsg = "请选择补缴类型！";
                return false;
            }
            if (cmbApplyLimit.SelectedItem == null)
            {
                errorMsg = "请选择是否保底！";
                return false;
            }
            return true;
        }

        private static bool ValidatePeriodAndType(int begin, int end, string type, out string error)
        {
            error = "";
            if (begin >= 198601 && end <= 199512)
            {
                if (type != "62号文件补缴（合同工）" && type != "62号文件补缴（固定工）" && type != "历史陈欠清算")
                {
                    error = "198601到199512期间，只能选择62号文补缴或历史陈欠清算！";
                    return false;
                }
            }
            else if (begin >= 199601 && end <= 201106)
            {
                if (type != "职工158号文补缴" && type != "历史陈欠清算" && type != "个体缴费")
                {
                    error = "199601到201106期间，只能选择158号文补缴、历史陈欠清算或个体缴费！";
                    return false;
                }
            }
            else if (begin >= 201107 && end <= 201112)
            {
                if (type != "职工补中断" && type != "历史陈欠清算" && type != "个体缴费")
                {
                    error = "201107到201112期间，只能选择职工补中断、历史陈欠清算或个体缴费！";
                    return false;
                }
            }
            else if (begin >= 201201)
            {
                if (type != "职工补中断" && type != "个体缴费")
                {
                    error = "201201至今，只能选择职工补中断或个体缴费！";
                    return false;
                }
            }
            return true;
        }

        private void ClearInputFields()
        {
            txtName.Clear();
            txtIdCard.Clear();
            txtBegin.Clear();
            txtEnd.Clear();
            txtBase.Clear();
            cmbContributionType.SelectedIndex = -1;
            cmbApplyLimit.SelectedIndex = -1;
        }

        private void BindDetailGrid()
        {
            dgvDetail.Rows.Clear();
            foreach (var d in _details)
            {
                dgvDetail.Rows.Add(
                    d.Name, d.IdCardNo, d.Period, d.MonthlyBase,
                    d.ContributionTypeName, d.UnitPrincipal, d.UnitInterest,
                    d.PersonalPrincipal, d.PersonalInterest, d.LateFee, d.Total);
            }
        }

        private void BindSummaryGrid()
        {
            var dt = new DataTable();
            dt.Columns.Add("姓名");
            dt.Columns.Add("身份证号码");
            dt.Columns.Add("费款起止期号");
            dt.Columns.Add("基数总额", typeof(double));
            dt.Columns.Add("统筹部分本金", typeof(double));
            dt.Columns.Add("统筹部分利息", typeof(double));
            dt.Columns.Add("个人部分本金", typeof(double));
            dt.Columns.Add("个人部分利息", typeof(double));
            dt.Columns.Add("滞纳金", typeof(double));
            dt.Columns.Add("合计", typeof(double));

            foreach (var s in _summaries)
            {
                dt.Rows.Add(s.Name, s.IdCardNo, s.PeriodRange, s.TotalBase,
                    s.UnitPrincipal, s.UnitInterest, s.PersonalPrincipal,
                    s.PersonalInterest, s.LateFee, s.Total);
            }
            dgvSummary.DataSource = dt;
        }

        private static DataTable GridViewToDataTable(DataGridView dgv)
        {
            var dt = new DataTable();
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (col.Visible)
                    dt.Columns.Add(col.HeaderText);
            }
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;
                var dr = dt.NewRow();
                int idx = 0;
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    if (col.Visible)
                        dr[idx++] = row.Cells[col.Index].Value ?? DBNull.Value;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        #endregion

        #region 输入限制事件

        private void txtBegin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }

        private void txtEnd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }

        private void txtBase_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
            if (e.KeyChar == '.' && ((TextBox)sender).Text.Contains('.'))
                e.Handled = true;
        }

        private void txtIdCard_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 'X' && e.KeyChar != 'x' && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }

        #endregion
    }
}
