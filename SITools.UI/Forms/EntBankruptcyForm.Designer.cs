namespace SITools.UI.Forms
{
    partial class EntBankruptcyForm
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

            panelOptions = new Panel();
            grpOptions = new GroupBox();
            lblCalcInterest = new Label();
            cmbCalcInterest = new ComboBox();
            lblInterestEnd = new Label();
            dtpInterestEnd = new DateTimePicker();
            lblCalcLateFee = new Label();
            cmbCalcLateFee = new ComboBox();
            lblLateFeeEnd = new Label();
            dtpLateFeeEnd = new DateTimePicker();

            panelButtons = new Panel();
            btnImportExcel = new Button();
            btnCalc = new Button();
            btnReset = new Button();

            dgvInput = new DataGridView();
            dgvDetail = new DataGridView();
            dgvSummary = new DataGridView();

            panelDetailButtons = new Panel();
            btnExportDetail = new Button();
            panelSummaryButtons = new Panel();
            btnExportSummary = new Button();

            tabControl.SuspendLayout();
            SuspendLayout();

            // tabControl
            tabControl.Dock = DockStyle.Fill;
            tabControl.Controls.Add(tabInput);
            tabControl.Controls.Add(tabDetail);
            tabControl.Controls.Add(tabSummary);
            tabControl.Name = "tabControl";
            tabControl.Font = new Font("Microsoft YaHei", 9F);

            // tabInput
            tabInput.Text = "清算总览";
            tabInput.Controls.Add(dgvInput);
            tabInput.Controls.Add(panelButtons);
            tabInput.Controls.Add(panelOptions);

            // panelOptions
            panelOptions.Dock = DockStyle.Top;
            panelOptions.Height = 82;
            panelOptions.Controls.Add(grpOptions);

            // grpOptions：使用 TableLayoutPanel 内部布局，随窗口宽度自适应
            grpOptions.Text = "计算选项";
            grpOptions.Dock = DockStyle.Fill;
            grpOptions.Font = new Font("Microsoft YaHei", 9F);
            grpOptions.Padding = new Padding(4, 2, 4, 2);

            var tlpOpts = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 8,
                RowCount = 1,
                Padding = new Padding(4, 4, 4, 4)
            };
            tlpOpts.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));          // 计算利息 label
            tlpOpts.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));     // cmbCalcInterest
            tlpOpts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));      // spacer
            tlpOpts.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));          // 利息截止 label
            tlpOpts.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));    // dtpInterestEnd
            tlpOpts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));      // spacer
            tlpOpts.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));          // 计算滞纳金 label
            tlpOpts.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));     // cmbCalcLateFee

            // 增加第二行放截止日期（考虑到宽度可能不够时）
            // 改为：8列1行，两个截止日期直接放在同行中（spacer帮助对齐）
            // 实际上对于 minWidth=1000 的窗体，水平空间足够
            // 调整为 4列2行更清晰
            tlpOpts.ColumnCount = 4;
            tlpOpts.ColumnStyles.Clear();
            tlpOpts.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));          // label
            tlpOpts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));      // control
            tlpOpts.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));          // label
            tlpOpts.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));      // control
            tlpOpts.RowCount = 2;
            tlpOpts.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlpOpts.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            lblCalcInterest.Text = "计算利息：";
            lblCalcInterest.AutoSize = true;
            lblCalcInterest.Anchor = AnchorStyles.Right;
            lblCalcInterest.TextAlign = ContentAlignment.MiddleRight;

            lblInterestEnd.Text = "利息截止：";
            lblInterestEnd.AutoSize = true;
            lblInterestEnd.Anchor = AnchorStyles.Right;
            lblInterestEnd.TextAlign = ContentAlignment.MiddleRight;

            lblCalcLateFee.Text = "计算滞纳金：";
            lblCalcLateFee.AutoSize = true;
            lblCalcLateFee.Anchor = AnchorStyles.Right;
            lblCalcLateFee.TextAlign = ContentAlignment.MiddleRight;

            lblLateFeeEnd.Text = "滞纳金截止：";
            lblLateFeeEnd.AutoSize = true;
            lblLateFeeEnd.Anchor = AnchorStyles.Right;
            lblLateFeeEnd.TextAlign = ContentAlignment.MiddleRight;

            cmbCalcInterest.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCalcInterest.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            dtpInterestEnd.Format = DateTimePickerFormat.Short;
            dtpInterestEnd.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            cmbCalcLateFee.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCalcLateFee.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            dtpLateFeeEnd.Format = DateTimePickerFormat.Short;
            dtpLateFeeEnd.Anchor = AnchorStyles.Left | AnchorStyles.Right;

            tlpOpts.Controls.Add(lblCalcInterest, 0, 0);
            tlpOpts.Controls.Add(cmbCalcInterest, 1, 0);
            tlpOpts.Controls.Add(lblInterestEnd, 2, 0);
            tlpOpts.Controls.Add(dtpInterestEnd, 3, 0);
            tlpOpts.Controls.Add(lblCalcLateFee, 0, 1);
            tlpOpts.Controls.Add(cmbCalcLateFee, 1, 1);
            tlpOpts.Controls.Add(lblLateFeeEnd, 2, 1);
            tlpOpts.Controls.Add(dtpLateFeeEnd, 3, 1);
            grpOptions.Controls.Add(tlpOpts);

            // panelButtons
            panelButtons.Dock = DockStyle.Bottom;
            panelButtons.Height = 50;
            panelButtons.BackColor = Color.FromArgb(245, 246, 250);
            SetBtn(btnImportExcel, "从Excel导入", 8, 8, 110, 34);
            SetBtn(btnCalc, "开始计算", 130, 8, 100, 34);
            SetBtn(btnReset, "重置清空", 242, 8, 100, 34);

            btnImportExcel.FlatStyle = FlatStyle.Flat;
            btnImportExcel.FlatAppearance.BorderColor = Color.FromArgb(180, 200, 220);
            btnImportExcel.Font = new Font("Microsoft YaHei", 9F);

            btnCalc.BackColor = Color.SteelBlue;
            btnCalc.ForeColor = Color.White;
            btnCalc.FlatStyle = FlatStyle.Flat;
            btnCalc.FlatAppearance.BorderSize = 0;
            btnCalc.Font = new Font("Microsoft YaHei", 9F);

            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.FlatAppearance.BorderColor = Color.FromArgb(180, 200, 220);
            btnReset.Font = new Font("Microsoft YaHei", 9F);

            btnImportExcel.Click += btnImportExcel_Click;
            btnCalc.Click += btnCalc_Click;
            btnReset.Click += btnReset_Click;
            panelButtons.Controls.AddRange(new Control[] { btnImportExcel, btnCalc, btnReset });

            // dgvInput
            dgvInput.Dock = DockStyle.Fill;
            dgvInput.AllowUserToAddRows = true;
            dgvInput.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvInput.BorderStyle = BorderStyle.None;
            ApplyDgvStyle(dgvInput);
            SetupInputColumns();

            // tabDetail
            tabDetail.Text = "计算明细";
            tabDetail.Controls.Add(dgvDetail);
            tabDetail.Controls.Add(panelDetailButtons);

            panelDetailButtons.Dock = DockStyle.Bottom;
            panelDetailButtons.Height = 50;
            panelDetailButtons.BackColor = Color.FromArgb(245, 246, 250);
            SetBtn(btnExportDetail, "导出明细到Excel", 8, 8, 140, 34);
            btnExportDetail.BackColor = Color.SteelBlue;
            btnExportDetail.ForeColor = Color.White;
            btnExportDetail.FlatStyle = FlatStyle.Flat;
            btnExportDetail.FlatAppearance.BorderSize = 0;
            btnExportDetail.Font = new Font("Microsoft YaHei", 9F);
            btnExportDetail.Click += btnExportDetail_Click;
            panelDetailButtons.Controls.Add(btnExportDetail);

            dgvDetail.Dock = DockStyle.Fill;
            dgvDetail.ReadOnly = true;
            dgvDetail.AllowUserToAddRows = false;
            dgvDetail.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDetail.BorderStyle = BorderStyle.None;
            ApplyDgvStyle(dgvDetail);
            SetupDetailColumns();

            // tabSummary
            tabSummary.Text = "汇总结果";
            tabSummary.Controls.Add(dgvSummary);
            tabSummary.Controls.Add(panelSummaryButtons);

            panelSummaryButtons.Dock = DockStyle.Bottom;
            panelSummaryButtons.Height = 50;
            panelSummaryButtons.BackColor = Color.FromArgb(245, 246, 250);
            SetBtn(btnExportSummary, "导出汇总到Excel", 8, 8, 140, 34);
            btnExportSummary.BackColor = Color.SteelBlue;
            btnExportSummary.ForeColor = Color.White;
            btnExportSummary.FlatStyle = FlatStyle.Flat;
            btnExportSummary.FlatAppearance.BorderSize = 0;
            btnExportSummary.Font = new Font("Microsoft YaHei", 9F);
            btnExportSummary.Click += btnExportSummary_Click;
            panelSummaryButtons.Controls.Add(btnExportSummary);

            dgvSummary.Dock = DockStyle.Fill;
            dgvSummary.ReadOnly = true;
            dgvSummary.AllowUserToAddRows = false;
            dgvSummary.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSummary.BorderStyle = BorderStyle.None;
            ApplyDgvStyle(dgvSummary);

            // Form
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1100, 680);
            Controls.Add(tabControl);
            MinimumSize = new Size(1000, 600);
            Name = "EntBankruptcyForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "企业破产社保费清算";

            tabControl.ResumeLayout(false);
            ResumeLayout(false);
        }

        private void SetupInputColumns()
        {
            dgvInput.Columns.Clear();
            AddColumn(dgvInput, "colName", "姓名", 10);
            AddColumn(dgvInput, "colIdCard", "身份证号码", 18);
            AddColumn(dgvInput, "colInsType", "险种类型", 15);
            AddColumn(dgvInput, "colPeriod", "费款所属期", 12);
            AddColumn(dgvInput, "colBase", "月缴费基数", 12);
            AddColumn(dgvInput, "colUnit", "单位应缴金额", 12);
            AddColumn(dgvInput, "colPersonal", "个人应缴金额", 12);
        }

        private void SetupDetailColumns()
        {
            dgvDetail.Columns.Clear();
            AddColumn(dgvDetail, "colDName", "姓名", 8);
            AddColumn(dgvDetail, "colDIdCard", "身份证号码", 14);
            AddColumn(dgvDetail, "colDPeriod", "费款所属期", 9);
            AddColumn(dgvDetail, "colDBase", "月缴费基数", 9);
            AddColumn(dgvDetail, "colDInsType", "险种类型", 12);
            AddColumn(dgvDetail, "colDUnitP", "统筹部分本金", 9);
            AddColumn(dgvDetail, "colDUnitI", "统筹部分利息", 9);
            AddColumn(dgvDetail, "colDPersP", "个人部分本金", 9);
            AddColumn(dgvDetail, "colDPersI", "个人部分利息", 9);
            AddColumn(dgvDetail, "colDLate", "滞纳金", 7);
            AddColumn(dgvDetail, "colDTotal", "合计", 7);
        }

        private static void AddColumn(DataGridView dgv, string name, string header, float fill)
        {
            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = header,
                FillWeight = fill
            });
        }

        private static void SetBtn(Button btn, string text, int x, int y, int w, int h)
        {
            btn.Text = text;
            btn.Location = new Point(x, y);
            btn.Size = new Size(w, h);
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

        private TabControl tabControl;
        private TabPage tabInput, tabDetail, tabSummary;
        private Panel panelOptions, panelButtons, panelDetailButtons, panelSummaryButtons;
        private GroupBox grpOptions;
        private Label lblCalcInterest, lblInterestEnd, lblCalcLateFee, lblLateFeeEnd;
        private ComboBox cmbCalcInterest, cmbCalcLateFee;
        private DateTimePicker dtpInterestEnd, dtpLateFeeEnd;
        private Button btnImportExcel, btnCalc, btnReset, btnExportDetail, btnExportSummary;
        private DataGridView dgvInput, dgvDetail, dgvSummary;
    }
}
