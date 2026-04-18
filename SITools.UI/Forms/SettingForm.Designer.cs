namespace SITools.UI.Forms
{
    partial class SettingForm
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
            dgvRates = new DataGridView();
            panelButtons = new Panel();
            btnSave = new Button();
            btnReset = new Button();
            lblTitle = new Label();

            SuspendLayout();

            // lblTitle
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 36;
            lblTitle.Text = "历年社保记账利率参数（可直接在表格中修改，点击保存生效）";
            lblTitle.Font = new Font("Microsoft YaHei", 10F);
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblTitle.Padding = new Padding(8, 0, 0, 0);

            // dgvRates
            dgvRates.Dock = DockStyle.Fill;
            dgvRates.AutoGenerateColumns = false;
            dgvRates.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRates.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRates.AllowUserToAddRows = false;

            var yearCol = new DataGridViewTextBoxColumn
            {
                Name = "colYear",
                HeaderText = "年度",
                DataPropertyName = "Year",
                ReadOnly = true,
                FillWeight = 40
            };
            var rateCol = new DataGridViewTextBoxColumn
            {
                Name = "colRate",
                HeaderText = "社保记账利率",
                DataPropertyName = "Rate",
                FillWeight = 60
            };
            dgvRates.Columns.Add(yearCol);
            dgvRates.Columns.Add(rateCol);

            // panelButtons
            panelButtons.Dock = DockStyle.Bottom;
            panelButtons.Height = 48;

            btnSave.Text = "保存修改";
            btnSave.Size = new Size(100, 34);
            btnSave.Location = new Point(10, 7);
            btnSave.BackColor = Color.SteelBlue;
            btnSave.ForeColor = Color.White;
            btnSave.Click += btnSave_Click;

            btnReset.Text = "还原";
            btnReset.Size = new Size(80, 34);
            btnReset.Location = new Point(122, 7);
            btnReset.Click += btnReset_Click;

            panelButtons.Controls.AddRange(new Control[] { btnSave, btnReset });

            // SettingForm
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(400, 620);
            Controls.Add(dgvRates);
            Controls.Add(lblTitle);
            Controls.Add(panelButtons);
            MinimumSize = new Size(350, 400);
            Name = "SettingForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "征缴参数查询";
            Load += SettingForm_Load;

            ResumeLayout(false);
        }

        private DataGridView dgvRates;
        private Panel panelButtons;
        private Button btnSave, btnReset;
        private Label lblTitle;
    }
}
