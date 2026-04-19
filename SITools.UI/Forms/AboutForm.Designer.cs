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
            panelBottom = new Panel();
            panelContent = new Panel();
            lblTitle = new Label();
            lblVersion = new Label();
            lblDesc = new Label();
            lblContact = new Label();
            btnClose = new Button();

            panelBottom.SuspendLayout();
            panelContent.SuspendLayout();
            SuspendLayout();

            // panelBottom（底部按钮区）
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Height = 56;
            panelBottom.BackColor = Color.FromArgb(245, 246, 250);

            // btnClose
            btnClose.Text = "关  闭";
            btnClose.Size = new Size(100, 34);
            btnClose.BackColor = Color.SteelBlue;
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Font = new Font("Microsoft YaHei", 9.5F);
            btnClose.Click += btnClose_Click;
            // 居中放置
            btnClose.Anchor = AnchorStyles.None;
            btnClose.Location = new Point((400 - 100) / 2, (56 - 34) / 2);
            panelBottom.Controls.Add(btnClose);

            // panelContent（内容区）
            panelContent.Dock = DockStyle.Fill;
            panelContent.Padding = new Padding(20, 20, 20, 10);
            panelContent.BackColor = Color.White;

            // lblTitle
            lblTitle.AutoSize = false;
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 64;
            lblTitle.Font = new Font("Microsoft YaHei", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.SteelBlue;
            lblTitle.Text = "社会保险费征缴管理工具";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // lblVersion
            lblVersion.AutoSize = false;
            lblVersion.Dock = DockStyle.Top;
            lblVersion.Height = 36;
            lblVersion.Font = new Font("Microsoft YaHei", 10.5F);
            lblVersion.ForeColor = Color.FromArgb(80, 80, 80);
            lblVersion.Text = "版本：v2.0（.NET 8.0 WinForms 重构版）";
            lblVersion.TextAlign = ContentAlignment.MiddleCenter;

            // lblDesc
            lblDesc.AutoSize = false;
            lblDesc.Dock = DockStyle.Top;
            lblDesc.Height = 84;
            lblDesc.Font = new Font("Microsoft YaHei", 9.5F);
            lblDesc.ForeColor = Color.FromArgb(60, 60, 60);
            lblDesc.Text = "本软件用于社会保险费征缴管理工作，\r\n包含企业养老保险补缴测算、企业破产清算等功能。\r\n计算结果仅供参考，请以实际政策文件为准。";
            lblDesc.TextAlign = ContentAlignment.MiddleCenter;

            // lblContact
            lblContact.AutoSize = false;
            lblContact.Dock = DockStyle.Top;
            lblContact.Height = 36;
            lblContact.Font = new Font("Microsoft YaHei", 9F);
            lblContact.ForeColor = Color.Gray;
            lblContact.Text = "采用分层架构设计（Models / DAL / BLL / UI）";
            lblContact.TextAlign = ContentAlignment.MiddleCenter;

            // Controls 添加顺序决定 Dock 堆叠顺序（后加的在顶部）
            panelContent.Controls.Add(lblContact);
            panelContent.Controls.Add(lblDesc);
            panelContent.Controls.Add(lblVersion);
            panelContent.Controls.Add(lblTitle);

            // AboutForm
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(400, 300);
            BackColor = Color.White;
            Controls.Add(panelContent);
            Controls.Add(panelBottom);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "关于本软件";

            panelBottom.ResumeLayout(false);
            panelContent.ResumeLayout(false);
            ResumeLayout(false);
        }

        private Panel panelBottom, panelContent;
        private Label lblTitle, lblVersion, lblDesc, lblContact;
        private Button btnClose;
    }
}
