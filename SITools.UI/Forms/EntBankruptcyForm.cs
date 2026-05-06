using SITools.BLL.Services;
using SITools.Models.Entities;
using SITools.UI.Services;
using System.Data;

namespace SITools.UI.Forms
{
    /// <summary>
    /// 企业破产清算计算窗体。
    ///
    /// 直接从企业提供的 Excel 清算报表中导入（已包含各月各险种的应缴金额），
    /// 系统只需在此基础上计算利息和滞纳金。
    ///
    /// 窗体分为三个标签页：
    ///   1. 输入页（tabInput）：从 Excel 导入欠费明细数据
    ///   2. 明细页（tabDetail）：展示逐条计算结果（每行对应一条欠费记录）
    ///   3. 汇总页（tabSummary）：按身份证+险种分组汇总展示
    ///
    /// 注意：利息只对"企业职工基本养老保险"险种计算；其他险种不计利息。
    /// </summary>
    public partial class EntBankruptcyForm : Form
    {
        // 利息计算服务和滞纳金计算服务（本金来自Excel，不需要本金计算服务）
        private readonly IInterestCalculationService _interestSvc;
        private readonly ILateFeeCalculationService _lateFeeSvc;
        private readonly ExcelService _excelSvc;

        /// <summary>
        /// 构造函数：接收服务实例，初始化窗体控件，填充下拉框选项。
        /// </summary>
        public EntBankruptcyForm(
            IInterestCalculationService interestSvc,
            ILateFeeCalculationService lateFeeSvc,
            ExcelService excelSvc)
        {
            _interestSvc = interestSvc;
            _lateFeeSvc = lateFeeSvc;
            _excelSvc = excelSvc;

            InitializeComponent();

            // 填充是否计算利息/滞纳金的下拉框
            cmbCalcInterest.Items.AddRange(new object[] { "是", "否" });
            cmbCalcLateFee.Items.AddRange(new object[] { "是", "否" });
        }

        /// <summary>
        /// "从Excel导入"按钮：读取破产清算欠费明细 Excel 文件，填充输入表格。
        /// Excel 文件列顺序要求：
        ///   姓名、身份证、险种类型、费款所属期、月缴费基数、单位应缴金额、个人应缴金额
        /// </summary>
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
                dgvInput.Rows.Clear();  // 清空已有数据，重新导入
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

        /// <summary>
        /// "计算"按钮：读取输入表格数据，计算每条欠费记录的利息和滞纳金，
        /// 生成明细和汇总结果。
        ///
        /// 计算逻辑：
        ///   1. 前置验证（数据不为空、利息/滞纳金选项已选）
        ///   2. 从输入表格解析 BankruptcyRecord 列表
        ///   3. 对每条记录：
        ///      a. 仅"企业职工基本养老保险"的单位和个人本金才计算利息
        ///      b. 所有险种均按"欠费清算"规则计算滞纳金
        ///      c. 汇总合计
        ///   4. 按身份证+险种分组汇总（从已生成的明细表格中读取汇总）
        /// </summary>
        private void btnCalc_Click(object sender, EventArgs e)
        {
            // 前置检查
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

            // 从输入表格解析每行数据为 BankruptcyRecord 对象
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

            dgvDetail.Rows.Clear();  // 清空明细表格，准备填入新结果

            // 对每条欠费记录逐一计算
            foreach (var rec in details)
            {
                // 将 yyyyMM 格式的费款所属期解析为 DateTime（取该月1日）
                DateTime dueDate = DateTime.ParseExact(rec.Period, "yyyyMM", null);
                double unitPrincipal = Math.Round(rec.UnitAmount, 2);
                double personalPrincipal = Math.Round(rec.PersonalAmount, 2);

                // 利息只对"企业职工基本养老保险"险种计算，其他险种利息为0
                double unitInterest = 0.0, personalInterest = 0.0;
                if (calcInterest && rec.InsuranceTypeName == "企业职工基本养老保险")
                {
                    unitInterest = Math.Round(_interestSvc.CalculateInterest(unitPrincipal, dueDate, interestEndDate), 2);
                    personalInterest = Math.Round(_interestSvc.CalculateInterest(personalPrincipal, dueDate, interestEndDate), 2);
                }

                // 滞纳金对所有险种计算（使用"欠费清算"规则，从次月起算）
                double lateFee = 0.0;
                if (calcLateFee)
                {
                    double totalPrincipal = unitPrincipal + personalPrincipal;
                    lateFee = Math.Round(_lateFeeSvc.CalculateLateFee(totalPrincipal, lateFeeEndDate, dueDate, "欠费清算"), 2);
                }

                double total = Math.Round(unitPrincipal + unitInterest + personalPrincipal + personalInterest + lateFee, 2);

                // 将本条计算结果追加到明细表格
                dgvDetail.Rows.Add(
                    rec.Name, rec.IdCardNo, rec.Period, rec.MonthlyBase, rec.InsuranceTypeName,
                    unitPrincipal, unitInterest, personalPrincipal, personalInterest, lateFee, total);
            }

            // ---------------------------------------------------------------
            // 生成汇总：按"身份证+险种"分组，对明细表格中的金额求和
            // ---------------------------------------------------------------
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

            // 使用 LINQ 对 details 按"身份证+险种"分组，并从明细表格读取对应行的利息、滞纳金等
            var query = details
                .GroupBy(d => new { d.IdCardNo, d.InsuranceTypeName })
                .Select(grp =>
                {
                    // 本金直接从原始记录汇总（不受计算是否选择影响，因为本金来自Excel）
                    double uPrinc = Math.Round(grp.Sum(x => x.UnitAmount), 2);
                    double pPrinc = Math.Round(grp.Sum(x => x.PersonalAmount), 2);

                    // 利息、滞纳金、基数合计需要从明细表格（dgvDetail）中读取对应行求和
                    // （因为利息/滞纳金在 dgvDetail 填充时才计算，此处汇总读取已计算的结果）
                    double uInt = 0, pInt = 0, late = 0, tot = 0, baseSum = 0;
                    foreach (DataGridViewRow row in dgvDetail.Rows)
                    {
                        if (row.IsNewRow) continue;
                        // 筛选出同一身份证+险种的明细行
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

            // 将汇总结果填入 DataTable，并绑定到汇总表格
            foreach (var item in query)
            {
                summaryDt.Rows.Add(item.Name, item.IdCardNo, item.InsuranceTypeName, item.PeriodRange,
                    item.TotalBase, item.UnitPrincipal, item.UnitInterest, item.PersonalPrincipal,
                    item.PersonalInterest, item.LateFee, item.Total);
            }

            dgvSummary.DataSource = summaryDt;
            dgvSummary.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;  // 列宽自动填充

            tabControl.SelectedTab = tabDetail;  // 切换到明细标签页
            btnCalc.Enabled = false;             // 禁用计算按钮，防止重复计算
        }

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
        /// 通用导出方法：弹出保存对话框，将指定表格数据导出为 Excel 文件。
        /// </summary>
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

        /// <summary>
        /// "重置"按钮：清空所有表格和选择，恢复初始状态。
        /// </summary>
        private void btnReset_Click(object sender, EventArgs e)
        {
            dgvInput.Rows.Clear();
            dgvDetail.Rows.Clear();
            // 清空汇总表格（先获取绑定的 DataTable，再清除其行）
            (dgvSummary.DataSource as DataTable)?.Rows.Clear();
            cmbCalcInterest.SelectedIndex = -1;
            cmbCalcLateFee.SelectedIndex = -1;
            btnCalc.Enabled = true;
            tabControl.SelectedTab = tabInput;
        }

        /// <summary>
        /// 将 DataGridView 中的可见数据转换为 DataTable，用于导出 Excel。
        /// </summary>
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
