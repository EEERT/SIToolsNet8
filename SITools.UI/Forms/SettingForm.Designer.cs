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
            lblTitle.Height = 38;
            lblTitle.Text = "历年社保记账利率参数（可直接在表格中修改，点击保存生效）";
            lblTitle.Font = new Font("Microsoft YaHei", 9.5F);
            lblTitle.ForeColor = Color.FromArgb(60, 60, 60);
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            lblTitle.Padding = new Padding(10, 0, 0, 0);
            lblTitle.BackColor = Color.FromArgb(245, 246, 250);

            // dgvRates
            dgvRates.Dock = DockStyle.Fill;
            dgvRates.AutoGenerateColumns = false;
            dgvRates.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRates.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRates.AllowUserToAddRows = false;
            dgvRates.BorderStyle = BorderStyle.None;

            // DataGridView 样式
            dgvRates.EnableHeadersVisualStyles = false;
            dgvRates.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(41, 128, 185);
            dgvRates.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvRates.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft YaHei", 9.5F, FontStyle.Regular);
            dgvRates.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(41, 128, 185);
            dgvRates.DefaultCellStyle.Font = new Font("Microsoft YaHei", 9.5F);
            dgvRates.DefaultCellStyle.SelectionBackColor = Color.FromArgb(189, 215, 238);
            dgvRates.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvRates.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(235, 244, 251);
            dgvRates.GridColor = Color.FromArgb(210, 225, 240);
            dgvRates.RowHeadersVisible = false;
            dgvRates.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

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
            panelButtons.Height = 52;
            panelButtons.BackColor = Color.FromArgb(245, 246, 250);

            btnSave.Text = "保存修改";
            btnSave.Size = new Size(100, 34);
            btnSave.Location = new Point(10, 9);
            btnSave.BackColor = Color.SteelBlue;
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Font = new Font("Microsoft YaHei", 9.5F);
            btnSave.Click += btnSave_Click;

            btnReset.Text = "还  原";
            btnReset.Size = new Size(86, 34);
            btnReset.Location = new Point(122, 9);
            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.FlatAppearance.BorderColor = Color.FromArgb(180, 200, 220);
            btnReset.Font = new Font("Microsoft YaHei", 9.5F);
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
