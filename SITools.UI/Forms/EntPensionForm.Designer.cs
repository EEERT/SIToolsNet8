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
            tlpInputTop = new TableLayoutPanel();
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
            tabControl.Font = new Font("Microsoft YaHei", 9F);

            // ===== tabInput =====
            tabInput.Text = "补缴总览";
            tabInput.Controls.Add(dgvInput);
            tabInput.Controls.Add(panelInputButtons);
            tabInput.Controls.Add(tlpInputTop);
            tabInput.Padding = new Padding(4);

            // tlpInputTop：两列 TableLayoutPanel，左侧输入字段自动填充，右侧计算选项固定宽度
            tlpInputTop.Dock = DockStyle.Top;
            tlpInputTop.Height = 165;
            tlpInputTop.ColumnCount = 2;
            tlpInputTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpInputTop.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 385F));
            tlpInputTop.RowCount = 1;
            tlpInputTop.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpInputTop.Padding = new Padding(0, 0, 0, 5);

            // grpInputFields
            grpInputFields.Text = "输入信息";
            grpInputFields.Dock = DockStyle.Fill;
            grpInputFields.Margin = new Padding(0, 0, 4, 0);
            grpInputFields.Font = new Font("Microsoft YaHei", 9F);

            int ctlH = 28;
            int row1Y = 22, row2Y = 67, row3Y = 112;
            // 三列布局：col1(左), col2(中), col3(右)
            int c1L = 8, c1C = 108;   // 姓名/身份证/补缴类型
            int c2L = 260, c2C = 355; // 开始/结束时间
            int c3L = 462, c3C = 542; // 月缴费基数 / 是否保底 / 按钮

            AddLabelAndControl(grpInputFields, lblName, "姓名：", c1L, row1Y, 96, txtName, c1C, row1Y, 140, ctlH);
            AddLabelAndControl(grpInputFields, lblIdCard, "身份证号码：", c1L, row2Y, 96, txtIdCard, c1C, row2Y, 170, ctlH);
            AddLabelAndControl(grpInputFields, lblBegin, "补缴开始：", c2L, row1Y, 90, txtBegin, c2C, row1Y, 115, ctlH);
            AddLabelAndControl(grpInputFields, lblEnd, "补缴结束：", c2L, row2Y, 90, txtEnd, c2C, row2Y, 115, ctlH);
            AddLabelAndControl(grpInputFields, lblBase, "月缴费基数：", c3L, row1Y, 76, txtBase, c3C, row1Y, 95, ctlH);
            AddLabelAndControl(grpInputFields, lblType, "补缴类型：", c1L, row3Y, 96, cmbContributionType, c1C, row3Y, 200, ctlH);
            AddLabelAndControl(grpInputFields, lblLimit, "是否保底：", c2L, row3Y, 90, cmbApplyLimit, c2C, row3Y, 80, ctlH);

            cmbContributionType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbApplyLimit.DropDownStyle = ComboBoxStyle.DropDownList;

            btnAdd.Text = "添  加";
            btnAdd.Location = new Point(c3L, row3Y);
            btnAdd.Size = new Size(80, ctlH);
            btnAdd.BackColor = Color.SteelBlue;
            btnAdd.ForeColor = Color.White;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += btnAdd_Click;
            grpInputFields.Controls.Add(btnAdd);

            // grpCalcOptions（右侧固定宽度列）
            grpCalcOptions.Text = "计算选项";
            grpCalcOptions.Dock = DockStyle.Fill;
            grpCalcOptions.Margin = new Padding(0);
            grpCalcOptions.Font = new Font("Microsoft YaHei", 9F);

            // 使用 TableLayoutPanel 布局计算选项内的控件
            var tlpOpts = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 2,
                Padding = new Padding(6, 4, 6, 4)
            };
            tlpOpts.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tlpOpts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpOpts.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tlpOpts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpOpts.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlpOpts.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            lblCalcInterest = new Label { Text = "计算利息：", AutoSize = true, Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight };
            lblInterestEnd = new Label { Text = "利息截止：", AutoSize = true, Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight };
            lblCalcLateFee = new Label { Text = "计算滞纳金：", AutoSize = true, Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight };
            lblLateFeeEnd = new Label { Text = "滞纳金截止：", AutoSize = true, Anchor = AnchorStyles.Right, TextAlign = ContentAlignment.MiddleRight };

            cmbCalcInterest.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCalcInterest.Dock = DockStyle.Fill;
            cmbCalcInterest.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            cmbCalcLateFee.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCalcLateFee.Dock = DockStyle.Fill;
            cmbCalcLateFee.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            dtpInterestEnd.Format = DateTimePickerFormat.Short;
            dtpInterestEnd.Dock = DockStyle.Fill;

            dtpLateFeeEnd.Format = DateTimePickerFormat.Short;
            dtpLateFeeEnd.Dock = DockStyle.Fill;

            tlpOpts.Controls.Add(lblCalcInterest, 0, 0);
            tlpOpts.Controls.Add(cmbCalcInterest, 1, 0);
            tlpOpts.Controls.Add(lblInterestEnd, 2, 0);
            tlpOpts.Controls.Add(dtpInterestEnd, 3, 0);
            tlpOpts.Controls.Add(lblCalcLateFee, 0, 1);
            tlpOpts.Controls.Add(cmbCalcLateFee, 1, 1);
            tlpOpts.Controls.Add(lblLateFeeEnd, 2, 1);
            tlpOpts.Controls.Add(dtpLateFeeEnd, 3, 1);
            grpCalcOptions.Controls.Add(tlpOpts);

            tlpInputTop.Controls.Add(grpInputFields, 0, 0);
            tlpInputTop.Controls.Add(grpCalcOptions, 1, 0);

            // ===== panelInputButtons =====
            panelInputButtons.Dock = DockStyle.Bottom;
            panelInputButtons.Height = 50;
            panelInputButtons.BackColor = Color.FromArgb(245, 246, 250);
            SetStyledButton(btnCalc, "开始计算", 8, 8, 100, 34, true);
            SetStyledButton(btnReset, "重置清空", 120, 8, 100, 34, false);
            SetStyledButton(btnImportExcel, "从Excel导入", 232, 8, 110, 34, false);
            SetStyledButton(btnRemoveRow, "删除选中行", 354, 8, 110, 34, false);
            btnCalc.Click += btnCalc_Click;
            btnReset.Click += btnReset_Click;
            btnImportExcel.Click += btnImportExcel_Click;
            btnRemoveRow.Click += btnRemoveRow_Click;
            panelInputButtons.Controls.AddRange(new Control[] { btnCalc, btnReset, btnImportExcel, btnRemoveRow });

            // dgvInput
            dgvInput.Dock = DockStyle.Fill;
            dgvInput.AllowUserToAddRows = true;
            dgvInput.AllowUserToDeleteRows = true;
            dgvInput.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvInput.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvInput.BorderStyle = BorderStyle.None;
            ApplyDgvStyle(dgvInput);
            SetupInputGridColumns();

            // ===== tabDetail =====
            tabDetail.Text = "计算明细";
            tabDetail.Controls.Add(dgvDetail);
            tabDetail.Controls.Add(panelDetailButtons);

            panelDetailButtons.Dock = DockStyle.Bottom;
            panelDetailButtons.Height = 50;
            panelDetailButtons.BackColor = Color.FromArgb(245, 246, 250);
            SetStyledButton(btnExportDetail, "导出明细到Excel", 8, 8, 140, 34, true);
            btnExportDetail.Click += btnExportDetail_Click;
            panelDetailButtons.Controls.Add(btnExportDetail);

            dgvDetail.Dock = DockStyle.Fill;
            dgvDetail.ReadOnly = true;
            dgvDetail.AllowUserToAddRows = false;
            dgvDetail.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDetail.BorderStyle = BorderStyle.None;
            ApplyDgvStyle(dgvDetail);
            SetupDetailGridColumns();

            // ===== tabSummary =====
            tabSummary.Text = "汇总结果";
            tabSummary.Controls.Add(dgvSummary);
            tabSummary.Controls.Add(panelSummaryButtons);

            panelSummaryButtons.Dock = DockStyle.Bottom;
            panelSummaryButtons.Height = 50;
            panelSummaryButtons.BackColor = Color.FromArgb(245, 246, 250);
            SetStyledButton(btnExportSummary, "导出汇总到Excel", 8, 8, 140, 34, true);
            btnExportSummary.Click += btnExportSummary_Click;
            panelSummaryButtons.Controls.Add(btnExportSummary);

            dgvSummary.Dock = DockStyle.Fill;
            dgvSummary.ReadOnly = true;
            dgvSummary.AllowUserToAddRows = false;
            dgvSummary.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSummary.BorderStyle = BorderStyle.None;
            ApplyDgvStyle(dgvSummary);

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

        private static void ApplyDgvStyle(DataGridView dgv)
        {
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 128, 185);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft YaHei", 9F, FontStyle.Regular);
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);
            dgv.DefaultCellStyle.Font = new Font("Microsoft YaHei", 9F);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(189, 215, 238);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(235, 244, 251);
            dgv.GridColor = Color.FromArgb(210, 225, 240);
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private static void SetStyledButton(Button btn, string text, int x, int y, int w, int h, bool primary)
        {
            btn.Text = text;
            btn.Location = new Point(x, y);
            btn.Size = new Size(w, h);
            btn.FlatStyle = FlatStyle.Flat;
            btn.Font = new Font("Microsoft YaHei", 9F);
            if (primary)
            {
                btn.BackColor = Color.SteelBlue;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderSize = 0;
            }
            else
            {
                btn.BackColor = Color.White;
                btn.ForeColor = Color.FromArgb(60, 60, 60);
                btn.FlatAppearance.BorderColor = Color.FromArgb(180, 200, 220);
                btn.FlatAppearance.BorderSize = 1;
            }
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

        private TableLayoutPanel tlpInputTop;
        private TabControl tabControl;
        private TabPage tabInput, tabDetail, tabSummary;
        private Panel panelInputButtons, panelDetailButtons, panelSummaryButtons;
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
