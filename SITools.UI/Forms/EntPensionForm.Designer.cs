namespace SITools.UI.Forms
{
    partial class EntPensionForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            tabControl = new TabControl();
            tabInput = new TabPage();
            tabDetail = new TabPage();
            tabSummary = new TabPage();

            // ===== 输入 Tab =====
            panelInputTop = new Panel();
            grpInputFields = new GroupBox();
            lblName = new Label();
            txtName = new TextBox();
            lblIdCard = new Label();
            txtIdCard = new TextBox();
            lblBegin = new Label();
            txtBegin = new TextBox();
            lblEnd = new Label();
            txtEnd = new TextBox();
            lblBase = new Label();
            txtBase = new TextBox();
            lblType = new Label();
            cmbContributionType = new ComboBox();
            lblLimit = new Label();
            cmbApplyLimit = new ComboBox();
            btnAdd = new Button();

            grpCalcOptions = new GroupBox();
            lblCalcInterest = new Label();
            cmbCalcInterest = new ComboBox();
            lblInterestEnd = new Label();
            dtpInterestEnd = new DateTimePicker();
            lblCalcLateFee = new Label();
            cmbCalcLateFee = new ComboBox();
            lblLateFeeEnd = new Label();
            dtpLateFeeEnd = new DateTimePicker();

            panelInputButtons = new Panel();
            btnCalc = new Button();
            btnReset = new Button();
            btnImportExcel = new Button();
            btnRemoveRow = new Button();

            dgvInput = new DataGridView();

            // ===== 明细 Tab =====
            panelDetailButtons = new Panel();
            btnExportDetail = new Button();
            dgvDetail = new DataGridView();

            // ===== 汇总 Tab =====
            panelSummaryButtons = new Panel();
            btnExportSummary = new Button();
            dgvSummary = new DataGridView();

            tabControl.SuspendLayout();
            tabInput.SuspendLayout();
            tabDetail.SuspendLayout();
            tabSummary.SuspendLayout();
            SuspendLayout();

            // ===== TabControl =====
            tabControl.Controls.Add(tabInput);
            tabControl.Controls.Add(tabDetail);
            tabControl.Controls.Add(tabSummary);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;

            // ===== tabInput =====
            tabInput.Text = "补缴总览";
            tabInput.Controls.Add(dgvInput);
            tabInput.Controls.Add(panelInputButtons);
            tabInput.Controls.Add(panelInputTop);
            tabInput.Padding = new Padding(5);

            // panelInputTop（上半部：输入字段 + 计算选项）
            panelInputTop.Dock = DockStyle.Top;
            panelInputTop.Height = 160;
            panelInputTop.Controls.Add(grpCalcOptions);
            panelInputTop.Controls.Add(grpInputFields);

            // grpInputFields
            grpInputFields.Text = "输入信息";
            grpInputFields.Location = new Point(5, 5);
            grpInputFields.Size = new Size(700, 148);

            int col1X = 10, col2X = 130, col3X = 280, col4X = 420, col5X = 570;
            int row1Y = 20, row2Y = 65, row3Y = 110;
            int lblW = 110, ctlW = 130, ctlH = 28;

            AddLabelAndControl(grpInputFields, lblName, "姓名：", col1X, row1Y, lblW, txtName, col2X, row1Y, ctlW, ctlH);
            AddLabelAndControl(grpInputFields, lblIdCard, "身份证号码：", col1X, row2Y, lblW, txtIdCard, col2X, row2Y, ctlW * 2, ctlH);
            AddLabelAndControl(grpInputFields, lblBegin, "补缴开始时间：", col3X, row1Y, lblW, txtBegin, col4X, row1Y, ctlW, ctlH);
            AddLabelAndControl(grpInputFields, lblEnd, "补缴结束时间：", col3X, row2Y, lblW, txtEnd, col4X, row2Y, ctlW, ctlH);
            AddLabelAndControl(grpInputFields, lblBase, "月缴费基数：", col5X, row1Y, 90, txtBase, col5X + 90, row1Y, 90, ctlH);
            AddLabelAndControl(grpInputFields, lblType, "补缴类型：", col1X, row3Y, lblW, cmbContributionType, col2X, row3Y, 200, ctlH);
            AddLabelAndControl(grpInputFields, lblLimit, "是否保底：", col3X, row3Y, lblW, cmbApplyLimit, col4X, row3Y, 90, ctlH);

            cmbContributionType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbApplyLimit.DropDownStyle = ComboBoxStyle.DropDownList;

            btnAdd.Text = "添加";
            btnAdd.Location = new Point(610, row3Y);
            btnAdd.Size = new Size(75, ctlH);
            btnAdd.Click += btnAdd_Click;
            grpInputFields.Controls.Add(btnAdd);

            // grpCalcOptions
            grpCalcOptions.Text = "计算选项";
            grpCalcOptions.Location = new Point(715, 5);
            grpCalcOptions.Size = new Size(360, 148);

            int oRow1 = 22, oRow2 = 58;
            AddLabelAndControl(grpCalcOptions, new Label(), "计算利息：", 8, oRow1, 80, cmbCalcInterest, 92, oRow1, 60, ctlH);
            AddLabelAndControl(grpCalcOptions, new Label(), "利息截止：", 165, oRow1, 80, dtpInterestEnd, 248, oRow1, 100, ctlH);
            AddLabelAndControl(grpCalcOptions, new Label(), "计算滞纳金：", 8, oRow2, 80, cmbCalcLateFee, 92, oRow2, 60, ctlH);
            AddLabelAndControl(grpCalcOptions, new Label(), "滞纳金截止：", 165, oRow2, 80, dtpLateFeeEnd, 248, oRow2, 100, ctlH);

            cmbCalcInterest.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCalcLateFee.DropDownStyle = ComboBoxStyle.DropDownList;
            dtpInterestEnd.Format = DateTimePickerFormat.Short;
            dtpLateFeeEnd.Format = DateTimePickerFormat.Short;

            // ===== panelInputButtons =====
            panelInputButtons.Dock = DockStyle.Bottom;
            panelInputButtons.Height = 44;
            btnCalc.Text = "开始计算";
            btnCalc.Size = new Size(100, 34);
            btnCalc.Location = new Point(8, 5);
            btnCalc.BackColor = Color.SteelBlue;
            btnCalc.ForeColor = Color.White;
            btnCalc.Click += btnCalc_Click;
            btnReset.Text = "重置清空";
            btnReset.Size = new Size(100, 34);
            btnReset.Location = new Point(120, 5);
            btnReset.Click += btnReset_Click;
            btnImportExcel.Text = "从Excel导入";
            btnImportExcel.Size = new Size(110, 34);
            btnImportExcel.Location = new Point(232, 5);
            btnImportExcel.Click += btnImportExcel_Click;
            btnRemoveRow.Text = "删除选中行";
            btnRemoveRow.Size = new Size(110, 34);
            btnRemoveRow.Location = new Point(354, 5);
            btnRemoveRow.Click += btnRemoveRow_Click;
            panelInputButtons.Controls.AddRange(new Control[] { btnCalc, btnReset, btnImportExcel, btnRemoveRow });

            // dgvInput
            dgvInput.Dock = DockStyle.Fill;
            dgvInput.AllowUserToAddRows = true;
            dgvInput.AllowUserToDeleteRows = true;
            dgvInput.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvInput.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            SetupInputGridColumns();

            // ===== tabDetail =====
            tabDetail.Text = "计算明细";
            tabDetail.Controls.Add(dgvDetail);
            tabDetail.Controls.Add(panelDetailButtons);

            panelDetailButtons.Dock = DockStyle.Bottom;
            panelDetailButtons.Height = 44;
            btnExportDetail.Text = "导出明细到Excel";
            btnExportDetail.Size = new Size(140, 34);
            btnExportDetail.Location = new Point(8, 5);
            btnExportDetail.Click += btnExportDetail_Click;
            panelDetailButtons.Controls.Add(btnExportDetail);

            dgvDetail.Dock = DockStyle.Fill;
            dgvDetail.ReadOnly = true;
            dgvDetail.AllowUserToAddRows = false;
            dgvDetail.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            SetupDetailGridColumns();

            // ===== tabSummary =====
            tabSummary.Text = "汇总结果";
            tabSummary.Controls.Add(dgvSummary);
            tabSummary.Controls.Add(panelSummaryButtons);

            panelSummaryButtons.Dock = DockStyle.Bottom;
            panelSummaryButtons.Height = 44;
            btnExportSummary.Text = "导出汇总到Excel";
            btnExportSummary.Size = new Size(140, 34);
            btnExportSummary.Location = new Point(8, 5);
            btnExportSummary.Click += btnExportSummary_Click;
            panelSummaryButtons.Controls.Add(btnExportSummary);

            dgvSummary.Dock = DockStyle.Fill;
            dgvSummary.ReadOnly = true;
            dgvSummary.AllowUserToAddRows = false;
            dgvSummary.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // ===== EntPensionForm =====
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1100, 680);
            Controls.Add(tabControl);
            MinimumSize = new Size(1000, 600);
            Name = "EntPensionForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "企业社会保险费补缴测算";

            tabControl.ResumeLayout(false);
            tabInput.ResumeLayout(false);
            tabDetail.ResumeLayout(false);
            tabSummary.ResumeLayout(false);
            ResumeLayout(false);

            // 绑定输入限制
            txtBegin.KeyPress += txtBegin_KeyPress;
            txtEnd.KeyPress += txtEnd_KeyPress;
            txtBase.KeyPress += txtBase_KeyPress;
            txtIdCard.KeyPress += txtIdCard_KeyPress;
        }

        private void SetupInputGridColumns()
        {
            dgvInput.Columns.Clear();
            AddDgvColumn(dgvInput, "colName", "姓名", 10);
            AddDgvColumn(dgvInput, "colIdCard", "身份证号码", 20);
            AddDgvColumn(dgvInput, "colBegin", "补缴开始时间", 15);
            AddDgvColumn(dgvInput, "colEnd", "补缴结束时间", 15);
            AddDgvColumn(dgvInput, "colBase", "月缴费基数", 15);
            AddDgvColumn(dgvInput, "colType", "补缴类型", 25);
            AddDgvColumn(dgvInput, "colLimit", "是否保底", 10);
        }

        private void SetupDetailGridColumns()
        {
            dgvDetail.Columns.Clear();
            AddDgvColumn(dgvDetail, "colDName", "姓名", 8);
            AddDgvColumn(dgvDetail, "colDIdCard", "身份证号码", 14);
            AddDgvColumn(dgvDetail, "colDPeriod", "费款所属期", 10);
            AddDgvColumn(dgvDetail, "colDBase", "月缴费基数", 10);
            AddDgvColumn(dgvDetail, "colDType", "补缴类型", 15);
            AddDgvColumn(dgvDetail, "colDUnitP", "统筹部分本金", 10);
            AddDgvColumn(dgvDetail, "colDUnitI", "统筹部分利息", 10);
            AddDgvColumn(dgvDetail, "colDPersP", "个人部分本金", 10);
            AddDgvColumn(dgvDetail, "colDPersI", "个人部分利息", 10);
            AddDgvColumn(dgvDetail, "colDLate", "滞纳金", 8);
            AddDgvColumn(dgvDetail, "colDTotal", "合计", 8);
        }

        private static void AddDgvColumn(DataGridView dgv, string name, string header, float fillWeight)
        {
            var col = new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = header,
                FillWeight = fillWeight
            };
            dgv.Columns.Add(col);
        }

        private static void AddLabelAndControl(Control parent, Label lbl, string text, int lx, int ly, int lw, Control ctl, int cx, int cy, int cw, int ch)
        {
            lbl.AutoSize = false;
            lbl.Text = text;
            lbl.Location = new Point(lx, ly + 4);
            lbl.Size = new Size(lw, ch);
            lbl.TextAlign = ContentAlignment.MiddleRight;
            ctl.Location = new Point(cx, cy);
            ctl.Size = new Size(cw, ch);
            parent.Controls.Add(lbl);
            parent.Controls.Add(ctl);
        }

        private TabControl tabControl;
        private TabPage tabInput, tabDetail, tabSummary;
        private Panel panelInputTop, panelInputButtons, panelDetailButtons, panelSummaryButtons;
        private GroupBox grpInputFields, grpCalcOptions;
        private Label lblName, lblIdCard, lblBegin, lblEnd, lblBase, lblType, lblLimit;
        private Label lblCalcInterest, lblInterestEnd, lblCalcLateFee, lblLateFeeEnd;
        private TextBox txtName, txtIdCard, txtBegin, txtEnd, txtBase;
        private ComboBox cmbContributionType, cmbApplyLimit, cmbCalcInterest, cmbCalcLateFee;
        private DateTimePicker dtpInterestEnd, dtpLateFeeEnd;
        private Button btnAdd, btnCalc, btnReset, btnImportExcel, btnRemoveRow;
        private Button btnExportDetail, btnExportSummary;
        private DataGridView dgvInput, dgvDetail, dgvSummary;
    }
}
