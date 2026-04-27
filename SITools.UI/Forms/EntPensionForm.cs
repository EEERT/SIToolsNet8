using SITools.BLL.Services;
using SITools.Models.Entities;
using System.Data;

namespace SITools.UI.Forms
{
    /// <summary>
    /// 企业养老保险补缴计算窗体。
    ///
    /// 该窗体是本系统的核心功能界面，分为三个标签页（TabControl）：
    ///   1. 输入页（tabInput）：
    ///      - 用户填写姓名、身份证、补缴起止期、月缴费基数、补缴类型等信息
    ///      - 点击"添加"后，数据显示在输入明细表格（dgvInput）中
    ///      - 也可以从 Excel 文件批量导入
    ///   2. 明细页（tabDetail）：
    ///      - 显示逐月计算结果（dgvDetail），每月一行
    ///   3. 汇总页（tabSummary）：
    ///      - 显示按人员汇总的计算结果（dgvSummary），每人一行
    ///
    /// 操作流程：填写/导入数据 → 选择计息/滞纳金选项 → 点击"计算" → 查看/导出结果
    /// </summary>
    public partial class EntPensionForm : Form
    {
        // 依赖的三个业务服务和一个门面服务
        private readonly IPensionCalculationService _pensionSvc;
        private readonly IInterestCalculationService _interestSvc;
        private readonly ILateFeeCalculationService _lateFeeSvc;
        private readonly EntPensionCalculationFacade _calcFacade;
        private readonly ExcelService _excelSvc;

        // 缓存计算结果（供导出使用）
        private List<CalculationResultDetail> _details = new();   // 逐月明细
        private List<CalculationResultSummary> _summaries = new(); // 人员汇总

        /// <summary>
        /// 构造函数：接收所有服务，初始化窗体控件，并填充下拉框选项。
        /// </summary>
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
            InitComboBoxes();  // 初始化所有下拉框的可选项
        }

        /// <summary>
        /// 初始化下拉框选项：
        ///   - cmbContributionType：补缴类型（从服务层动态获取）
        ///   - cmbApplyLimit：是否保底（是/否）
        ///   - cmbCalcInterest：是否计算利息（是/否）
        ///   - cmbCalcLateFee：是否计算滞纳金（是/否）
        /// </summary>
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

        /// <summary>
        /// "添加"按钮点击事件：将当前输入框中的数据验证后添加到输入明细表格（dgvInput）。
        ///
        /// 操作步骤：
        ///   1. 验证所有输入项的格式（ValidateInputs）
        ///   2. 验证月缴费基数是否为正数
        ///   3. 验证补缴类型与补缴时间段的业务匹配性（ValidatePeriodAndType）
        ///   4. 通过验证后，将数据行追加到输入表格，并清空输入框（ClearInputFields）
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            // 验证基本输入格式（期号、基数是否为空、类型是否已选等）
            if (!ValidateInputs(out string errorMsg))
            {
                MessageBox.Show(errorMsg, "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int beginPeriod = int.Parse(txtBegin.Text.Trim());
            int endPeriod = int.Parse(txtEnd.Text.Trim());

            // 验证基数是否为正数
            if (!ValidationService.IsPositiveNumber(txtBase.Text.Trim(), out double baseAmount))
            {
                MessageBox.Show("月缴费基数必须为正数！", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string typeName = cmbContributionType.SelectedItem!.ToString()!;
            bool applyLimit = cmbApplyLimit.SelectedItem?.ToString() == "是";

            // 验证补缴类型与时间段的匹配性（不同时间段只允许特定类型）
            if (!ValidatePeriodAndType(beginPeriod, endPeriod, typeName, out string typeError))
            {
                MessageBox.Show(typeError, "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 所有验证通过，将数据添加到输入表格
            dgvInput.Rows.Add(
                txtName.Text.Trim(),
                txtIdCard.Text.Trim().ToUpper(),  // 身份证号统一转大写（处理末位X）
                txtBegin.Text.Trim(),
                txtEnd.Text.Trim(),
                txtBase.Text.Trim(),
                typeName,
                applyLimit ? "是" : "否");

            ClearInputFields();  // 清空输入框，便于继续录入下一条
        }

        /// <summary>
        /// "删除所选行"按钮点击事件：删除用户在输入表格中选中的行。
        /// DataGridView 中 IsNewRow 表示最后一行空白占位行，不可删除。
        /// </summary>
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

        /// <summary>
        /// "从Excel导入"按钮点击事件：弹出文件选择对话框，读取Excel文件并批量填充输入表格。
        ///
        /// 说明：Excel 文件格式要求列顺序为：
        ///   姓名、身份证、起始期、结束期、月缴费基数、补缴类型、是否保底
        /// </summary>
        private void btnImportExcel_Click(object sender, EventArgs e)
        {
            // 弹出文件选择对话框，限制只能选择 Excel 文件
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
                    // 要求 Excel 至少有7列（与输入表格列数对应）
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

        /// <summary>
        /// "计算"按钮点击事件：读取输入表格中的所有数据，调用门面服务计算，展示结果。
        ///
        /// 计算流程：
        ///   1. 前置验证：输入表格不为空、利息/滞纳金选项已选
        ///   2. 将输入表格数据解析为 ContributionRecord 列表
        ///   3. 调用 _calcFacade.Calculate 生成逐月明细
        ///   4. 调用 _calcFacade.Summarize 生成人员汇总
        ///   5. 将结果绑定到明细表格和汇总表格，切换到明细标签页
        ///   6. 计算完成后禁用"计算"按钮，防止重复计算
        /// </summary>
        private void btnCalc_Click(object sender, EventArgs e)
        {
            // 前置检查：输入表格不能为空
            if (dgvInput.Rows.Count == 0)
            {
                MessageBox.Show("补缴总览表信息为空，请确认是否添加了数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 前置检查：必须选择是否计算利息
            if (cmbCalcInterest.SelectedItem == null)
            {
                MessageBox.Show("请选择是否计算利息！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 前置检查：必须选择是否计算滞纳金
            if (cmbCalcLateFee.SelectedItem == null)
            {
                MessageBox.Show("请选择是否计算滞纳金！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool calcInterest = cmbCalcInterest.SelectedItem.ToString() == "是";
            bool calcLateFee = cmbCalcLateFee.SelectedItem.ToString() == "是";
            DateTime interestEndDate = dtpInterestEnd.Value;   // 利息截止日期
            DateTime lateFeeEndDate = dtpLateFeeEnd.Value;     // 滞纳金截止日期（实际缴款日）

            // 从输入表格逐行读取，构建 ContributionRecord 列表
            var records = new List<ContributionRecord>();
            foreach (DataGridViewRow row in dgvInput.Rows)
            {
                if (row.IsNewRow) continue;  // 跳过表格末尾的空占位行
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
                // 调用门面服务执行计算，获取逐月明细
                _details = _calcFacade.Calculate(records, calcInterest, interestEndDate, calcLateFee, lateFeeEndDate);
                // 对明细按身份证汇总
                _summaries = _calcFacade.Summarize(_details);

                // 将结果绑定到两个表格
                BindDetailGrid();
                BindSummaryGrid();

                // 切换到明细标签页，方便用户查看结果
                tabControl.SelectedTab = tabDetail;
                // 禁用计算按钮，提示用户如需重新计算需点"重置"
                btnCalc.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"计算失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 导出按钮

        /// <summary>
        /// "导出明细"按钮：将明细表格数据导出为 Excel 文件。
        /// </summary>
        private void btnExportDetail_Click(object sender, EventArgs e)
        {
            ExportGrid(dgvDetail, "明细数据");
        }

        /// <summary>
        /// "导出汇总"按钮：将汇总表格数据导出为 Excel 文件。
        /// </summary>
        private void btnExportSummary_Click(object sender, EventArgs e)
        {
            ExportGrid(dgvSummary, "汇总数据");
        }

        /// <summary>
        /// 通用导出方法：将指定 DataGridView 中的数据导出为 Excel 文件。
        /// 弹出保存对话框让用户选择文件路径和文件名。
        /// </summary>
        /// <param name="dgv">要导出数据的 DataGridView 控件</param>
        /// <param name="sheetName">Excel 工作表名称（同时作为默认文件名）</param>
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
                FileName = sheetName  // 默认文件名
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                // 将 DataGridView 数据转换为 DataTable，再通过 ExcelService 写入文件
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

        /// <summary>
        /// "重置"按钮：清空所有表格和输入框，恢复初始状态，允许重新输入和计算。
        /// </summary>
        private void btnReset_Click(object sender, EventArgs e)
        {
            dgvInput.Rows.Clear();    // 清空输入表格
            dgvDetail.Rows.Clear();   // 清空明细表格
            dgvSummary.DataSource = null;  // 解除汇总表格的数据绑定（清空）
            _details.Clear();
            _summaries.Clear();
            ClearInputFields();
            cmbCalcInterest.SelectedIndex = -1;  // 取消利息选择
            cmbCalcLateFee.SelectedIndex = -1;   // 取消滞纳金选择
            btnCalc.Enabled = true;              // 重新启用计算按钮
            tabControl.SelectedTab = tabInput;   // 切回输入标签页
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 验证输入框中的各字段格式是否合法。
        /// 按顺序检查：起始期格式→结束期格式→起止期大小关系→基数非空→类型已选→保底已选。
        /// 一旦发现问题，通过 errorMsg 输出错误说明并返回 false。
        /// </summary>
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

        /// <summary>
        /// 验证补缴时间段与补缴类型的业务匹配性。
        /// 依据政策规定，不同历史时期只能使用特定的补缴类型：
        ///   - 1986-1995年12月：62号文补缴或历史陈欠清算
        ///   - 1996-2011年6月：158号文补缴、历史陈欠清算或个体缴费
        ///   - 2011年7月-12月：职工补中断、历史陈欠清算或个体缴费
        ///   - 2012年1月至今：职工补中断或个体缴费
        ///
        /// 注意：此校验仅对时间段完全落在某个区间内的情况生效，
        ///       跨区间的情况（begin 和 end 不在同一区间）不做限制。
        /// </summary>
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

        /// <summary>
        /// 清空输入区的所有输入框和下拉框选择，便于继续录入下一条数据。
        /// </summary>
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

        /// <summary>
        /// 将逐月明细数据（_details）绑定到明细表格（dgvDetail）。
        /// 先清空表格，然后逐行添加。
        /// </summary>
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

        /// <summary>
        /// 将人员汇总数据（_summaries）绑定到汇总表格（dgvSummary）。
        /// 通过创建 DataTable 并设置为 DataSource 的方式绑定，
        /// 这样可以利用 DataGridView 的自动列生成和排序功能。
        /// </summary>
        private void BindSummaryGrid()
        {
            // 创建 DataTable 并定义列结构（double 类型的列会在表格中右对齐）
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
            dgvSummary.DataSource = dt;  // 将 DataTable 绑定到表格控件
        }

        /// <summary>
        /// 将 DataGridView 中的可见数据转换为 DataTable，用于导出 Excel。
        /// 只处理可见列（col.Visible == true），隐藏列不导出。
        /// </summary>
        private static DataTable GridViewToDataTable(DataGridView dgv)
        {
            var dt = new DataTable();
            // 遍历可见列，创建对应的 DataTable 列（以列标题作为列名）
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (col.Visible)
                    dt.Columns.Add(col.HeaderText);
            }
            // 遍历每行数据（跳过末尾占位行），逐格读取值
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
        // 以下事件处理器限制特定文本框只能输入特定字符，
        // 通过将 e.Handled = true 来阻止不合法的按键输入。

        /// <summary>
        /// 起始期输入框：只允许输入数字（不允许字母、符号等）。
        /// Keys.Back 是退格键，需要放行。
        /// </summary>
        private void txtBegin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;  // 阻止非数字/退格键的输入
        }

        /// <summary>
        /// 结束期输入框：只允许输入数字（不允许字母、符号等）。
        /// </summary>
        private void txtEnd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }

        /// <summary>
        /// 月缴费基数输入框：只允许输入数字、小数点（且小数点只允许一个）和退格键。
        /// </summary>
        private void txtBase_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
            // 如果已包含小数点，则不允许再输入小数点
            if (e.KeyChar == '.' && ((TextBox)sender).Text.Contains('.'))
                e.Handled = true;
        }

        /// <summary>
        /// 身份证号码输入框：只允许输入数字、大写X、小写x和退格键。
        /// （身份证末位可能是 X，代表校验码10）
        /// </summary>
        private void txtIdCard_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 'X' && e.KeyChar != 'x' && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }

        #endregion
    }
}
