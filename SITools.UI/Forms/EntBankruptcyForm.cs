using SITools.BLL.Services;
using SITools.Models.Entities;
using System.Data;

namespace SITools.UI.Forms
{
    public partial class EntBankruptcyForm : Form
    {
        private readonly IInterestCalculationService _interestSvc;
        private readonly ILateFeeCalculationService _lateFeeSvc;
        private readonly ExcelService _excelSvc;

        public EntBankruptcyForm(
            IInterestCalculationService interestSvc,
            ILateFeeCalculationService lateFeeSvc,
            ExcelService excelSvc)
        {
            _interestSvc = interestSvc;
            _lateFeeSvc = lateFeeSvc;
            _excelSvc = excelSvc;

            InitializeComponent();

            cmbCalcInterest.Items.AddRange(new object[] { "是", "否" });
            cmbCalcLateFee.Items.AddRange(new object[] { "是", "否" });
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
                dgvInput.Rows.Clear();
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

        private void btnCalc_Click(object sender, EventArgs e)
        {
            if (dgvInput.Rows.Count == 0)
            {
                MessageBox.Show("清算总览表信息为空，请确认是否添加了数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            var details = new List<BankruptcyRecord>();

            foreach (DataGridViewRow row in dgvInput.Rows)
            {
                if (row.IsNewRow) continue;
                try
                {
                    details.Add(new BankruptcyRecord
                    {
                        Name = row.Cells["colName"].Value?.ToString() ?? "",
                        IdCardNo = row.Cells["colIdCard"].Value?.ToString() ?? "",
                        InsuranceTypeName = row.Cells["colInsType"].Value?.ToString() ?? "",
                        Period = row.Cells["colPeriod"].Value?.ToString() ?? "",
                        MonthlyBase = double.Parse(row.Cells["colBase"].Value?.ToString() ?? "0"),
                        UnitAmount = double.Parse(row.Cells["colUnit"].Value?.ToString() ?? "0"),
                        PersonalAmount = double.Parse(row.Cells["colPersonal"].Value?.ToString() ?? "0"),
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"数据行解析失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            dgvDetail.Rows.Clear();

            foreach (var rec in details)
            {
                DateTime dueDate = DateTime.ParseExact(rec.Period, "yyyyMM", null);
                double unitPrincipal = Math.Round(rec.UnitAmount, 2);
                double personalPrincipal = Math.Round(rec.PersonalAmount, 2);

                double unitInterest = 0.0, personalInterest = 0.0;
                if (calcInterest && rec.InsuranceTypeName == "企业职工基本养老保险")
                {
                    unitInterest = Math.Round(_interestSvc.CalculateInterest(unitPrincipal, dueDate, interestEndDate), 2);
                    personalInterest = Math.Round(_interestSvc.CalculateInterest(personalPrincipal, dueDate, interestEndDate), 2);
                }

                double lateFee = 0.0;
                if (calcLateFee)
                {
                    double totalPrincipal = unitPrincipal + personalPrincipal;
                    lateFee = Math.Round(_lateFeeSvc.CalculateLateFee(totalPrincipal, lateFeeEndDate, dueDate, "欠费清算"), 2);
                }

                double total = Math.Round(unitPrincipal + unitInterest + personalPrincipal + personalInterest + lateFee, 2);

                dgvDetail.Rows.Add(
                    rec.Name, rec.IdCardNo, rec.Period, rec.MonthlyBase, rec.InsuranceTypeName,
                    unitPrincipal, unitInterest, personalPrincipal, personalInterest, lateFee, total);
            }

            // 生成汇总
            var summaryDt = new DataTable();
            summaryDt.Columns.Add("姓名");
            summaryDt.Columns.Add("身份证号码");
            summaryDt.Columns.Add("险种类型");
            summaryDt.Columns.Add("费款起止期号");
            summaryDt.Columns.Add("基数总额", typeof(double));
            summaryDt.Columns.Add("统筹部分本金", typeof(double));
            summaryDt.Columns.Add("统筹部分利息", typeof(double));
            summaryDt.Columns.Add("个人部分本金", typeof(double));
            summaryDt.Columns.Add("个人部分利息", typeof(double));
            summaryDt.Columns.Add("滞纳金", typeof(double));
            summaryDt.Columns.Add("合计", typeof(double));

            var query = details
                .GroupBy(d => new { d.IdCardNo, d.InsuranceTypeName })
                .Select(grp =>
                {
                    double uPrinc = Math.Round(grp.Sum(x => x.UnitAmount), 2);
                    double pPrinc = Math.Round(grp.Sum(x => x.PersonalAmount), 2);
                    // Recalculate interest/latefee per group from dgvDetail rows is complex, 
                    // so we sum from detail grid
                    double uInt = 0, pInt = 0, late = 0, tot = 0, baseSum = 0;
                    foreach (DataGridViewRow row in dgvDetail.Rows)
                    {
                        if (row.IsNewRow) continue;
                        if (row.Cells["colDIdCard"].Value?.ToString() == grp.Key.IdCardNo &&
                            row.Cells["colDInsType"].Value?.ToString() == grp.Key.InsuranceTypeName)
                        {
                            baseSum += Convert.ToDouble(row.Cells["colDBase"].Value ?? 0);
                            uInt += Convert.ToDouble(row.Cells["colDUnitI"].Value ?? 0);
                            pInt += Convert.ToDouble(row.Cells["colDPersI"].Value ?? 0);
                            late += Convert.ToDouble(row.Cells["colDLate"].Value ?? 0);
                            tot += Convert.ToDouble(row.Cells["colDTotal"].Value ?? 0);
                        }
                    }
                    return new
                    {
                        grp.First().Name,
                        grp.Key.IdCardNo,
                        grp.Key.InsuranceTypeName,
                        PeriodRange = $"{grp.Min(x => x.Period)}-{grp.Max(x => x.Period)}",
                        TotalBase = Math.Round(baseSum, 2),
                        UnitPrincipal = uPrinc,
                        UnitInterest = Math.Round(uInt, 2),
                        PersonalPrincipal = pPrinc,
                        PersonalInterest = Math.Round(pInt, 2),
                        LateFee = Math.Round(late, 2),
                        Total = Math.Round(tot, 2),
                    };
                });

            foreach (var item in query)
            {
                summaryDt.Rows.Add(item.Name, item.IdCardNo, item.InsuranceTypeName, item.PeriodRange,
                    item.TotalBase, item.UnitPrincipal, item.UnitInterest, item.PersonalPrincipal,
                    item.PersonalInterest, item.LateFee, item.Total);
            }

            dgvSummary.DataSource = summaryDt;
            dgvSummary.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            tabControl.SelectedTab = tabDetail;
            btnCalc.Enabled = false;
        }

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

        private void btnReset_Click(object sender, EventArgs e)
        {
            dgvInput.Rows.Clear();
            dgvDetail.Rows.Clear();
            (dgvSummary.DataSource as DataTable)?.Rows.Clear();
            cmbCalcInterest.SelectedIndex = -1;
            cmbCalcLateFee.SelectedIndex = -1;
            btnCalc.Enabled = true;
            tabControl.SelectedTab = tabInput;
        }

        private static DataTable GridViewToDataTable(DataGridView dgv)
        {
            var dt = new DataTable();
            foreach (DataGridViewColumn col in dgv.Columns)
                if (col.Visible) dt.Columns.Add(col.HeaderText);
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;
                var dr = dt.NewRow();
                int idx = 0;
                foreach (DataGridViewColumn col in dgv.Columns)
                    if (col.Visible) dr[idx++] = row.Cells[col.Index].Value ?? DBNull.Value;
                dt.Rows.Add(dr);
            }
            return dt;
        }
    }
}
