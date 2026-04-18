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

            // tabInput
            tabInput.Text = "清算总览";
            tabInput.Controls.Add(dgvInput);
            tabInput.Controls.Add(panelButtons);
            tabInput.Controls.Add(panelOptions);

            // panelOptions
            panelOptions.Dock = DockStyle.Top;
            panelOptions.Height = 80;
            panelOptions.Controls.Add(grpOptions);

            // grpOptions
            grpOptions.Text = "计算选项";
            grpOptions.Dock = DockStyle.Fill;
            grpOptions.Margin = new Padding(5);
            grpOptions.Padding = new Padding(5);

            int ctlH = 28;
            AddLabelControl(grpOptions, lblCalcInterest, "计算利息：", 10, 22, 70, cmbCalcInterest, 84, 22, 60, ctlH);
            AddLabelControl(grpOptions, lblInterestEnd, "利息截止：", 155, 22, 70, dtpInterestEnd, 229, 22, 120, ctlH);
            AddLabelControl(grpOptions, lblCalcLateFee, "计算滞纳金：", 360, 22, 80, cmbCalcLateFee, 444, 22, 60, ctlH);
            AddLabelControl(grpOptions, lblLateFeeEnd, "滞纳金截止：", 515, 22, 80, dtpLateFeeEnd, 599, 22, 120, ctlH);

            cmbCalcInterest.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCalcLateFee.DropDownStyle = ComboBoxStyle.DropDownList;
            dtpInterestEnd.Format = DateTimePickerFormat.Short;
            dtpLateFeeEnd.Format = DateTimePickerFormat.Short;

            // panelButtons
            panelButtons.Dock = DockStyle.Bottom;
            panelButtons.Height = 44;
            SetBtn(btnImportExcel, "从Excel导入", 8, 5, 110, 34);
            SetBtn(btnCalc, "开始计算", 130, 5, 100, 34);
            SetBtn(btnReset, "重置清空", 242, 5, 100, 34);
            btnCalc.BackColor = Color.SteelBlue;
            btnCalc.ForeColor = Color.White;
            btnImportExcel.Click += btnImportExcel_Click;
            btnCalc.Click += btnCalc_Click;
            btnReset.Click += btnReset_Click;
            panelButtons.Controls.AddRange(new Control[] { btnImportExcel, btnCalc, btnReset });

            // dgvInput
            dgvInput.Dock = DockStyle.Fill;
            dgvInput.AllowUserToAddRows = true;
            dgvInput.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            SetupInputColumns();

            // tabDetail
            tabDetail.Text = "计算明细";
            tabDetail.Controls.Add(dgvDetail);
            tabDetail.Controls.Add(panelDetailButtons);

            panelDetailButtons.Dock = DockStyle.Bottom;
            panelDetailButtons.Height = 44;
            SetBtn(btnExportDetail, "导出明细到Excel", 8, 5, 140, 34);
            btnExportDetail.Click += btnExportDetail_Click;
            panelDetailButtons.Controls.Add(btnExportDetail);

            dgvDetail.Dock = DockStyle.Fill;
            dgvDetail.ReadOnly = true;
            dgvDetail.AllowUserToAddRows = false;
            dgvDetail.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            SetupDetailColumns();

            // tabSummary
            tabSummary.Text = "汇总结果";
            tabSummary.Controls.Add(dgvSummary);
            tabSummary.Controls.Add(panelSummaryButtons);

            panelSummaryButtons.Dock = DockStyle.Bottom;
            panelSummaryButtons.Height = 44;
            SetBtn(btnExportSummary, "导出汇总到Excel", 8, 5, 140, 34);
            btnExportSummary.Click += btnExportSummary_Click;
            panelSummaryButtons.Controls.Add(btnExportSummary);

            dgvSummary.Dock = DockStyle.Fill;
            dgvSummary.ReadOnly = true;
            dgvSummary.AllowUserToAddRows = false;
            dgvSummary.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

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

        private static void AddLabelControl(Control parent, Label lbl, string text, int lx, int ly, int lw, Control ctl, int cx, int cy, int cw, int ch)
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
        private Panel panelOptions, panelButtons, panelDetailButtons, panelSummaryButtons;
        private GroupBox grpOptions;
        private Label lblCalcInterest, lblInterestEnd, lblCalcLateFee, lblLateFeeEnd;
        private ComboBox cmbCalcInterest, cmbCalcLateFee;
        private DateTimePicker dtpInterestEnd, dtpLateFeeEnd;
        private Button btnImportExcel, btnCalc, btnReset, btnExportDetail, btnExportSummary;
        private DataGridView dgvInput, dgvDetail, dgvSummary;
    }
}
