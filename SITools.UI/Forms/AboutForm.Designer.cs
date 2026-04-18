namespace SITools.UI.Forms
{
    partial class AboutForm
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
            panelMain = new Panel();
            lblTitle = new Label();
            lblVersion = new Label();
            lblDesc = new Label();
            lblContact = new Label();
            btnClose = new Button();

            panelMain.SuspendLayout();
            SuspendLayout();

            // panelMain
            panelMain.Dock = DockStyle.Fill;
            panelMain.Padding = new Padding(20);

            // lblTitle
            lblTitle.AutoSize = false;
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 60;
            lblTitle.Font = new Font("Microsoft YaHei", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.SteelBlue;
            lblTitle.Text = "社会保险费征缴管理工具";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // lblVersion
            lblVersion.AutoSize = false;
            lblVersion.Dock = DockStyle.Top;
            lblVersion.Height = 36;
            lblVersion.Font = new Font("Microsoft YaHei", 11F);
            lblVersion.Text = "版本：v2.0（.NET 8.0 WinForms 重构版）";
            lblVersion.TextAlign = ContentAlignment.MiddleCenter;

            // lblDesc
            lblDesc.AutoSize = false;
            lblDesc.Dock = DockStyle.Top;
            lblDesc.Height = 80;
            lblDesc.Font = new Font("Microsoft YaHei", 10F);
            lblDesc.Text = "本软件用于社会保险费征缴管理工作，\r\n包含企业养老保险补缴测算、企业破产清算等功能。\r\n计算结果仅供参考，请以实际政策文件为准。";
            lblDesc.TextAlign = ContentAlignment.MiddleCenter;

            // lblContact
            lblContact.AutoSize = false;
            lblContact.Dock = DockStyle.Top;
            lblContact.Height = 36;
            lblContact.Font = new Font("Microsoft YaHei", 10F);
            lblContact.ForeColor = Color.Gray;
            lblContact.Text = "采用分层架构设计（Models / DAL / BLL / UI）";
            lblContact.TextAlign = ContentAlignment.MiddleCenter;

            panelMain.Controls.Add(lblContact);
            panelMain.Controls.Add(lblDesc);
            panelMain.Controls.Add(lblVersion);
            panelMain.Controls.Add(lblTitle);

            // btnClose
            btnClose.Anchor = AnchorStyles.Bottom;
            btnClose.Size = new Size(100, 34);
            btnClose.Location = new Point(150, 250);
            btnClose.Text = "关闭";
            btnClose.Click += btnClose_Click;

            // AboutForm
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(400, 300);
            Controls.Add(panelMain);
            Controls.Add(btnClose);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "关于";

            panelMain.ResumeLayout(false);
            ResumeLayout(false);
        }

        private Panel panelMain;
        private Label lblTitle, lblVersion, lblDesc, lblContact;
        private Button btnClose;
    }
}
