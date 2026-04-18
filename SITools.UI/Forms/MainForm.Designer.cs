namespace SITools.UI.Forms
{
    partial class MainForm
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
            menuStrip = new MenuStrip();
            menuItemTools = new ToolStripMenuItem();
            menuItemSettings = new ToolStripMenuItem();
            menuItemCalc = new ToolStripMenuItem();
            menuItemEntPension = new ToolStripMenuItem();
            menuItemEntBankruptcy = new ToolStripMenuItem();
            menuItemCivilToEnt = new ToolStripMenuItem();
            menuItemPersonalAccount = new ToolStripMenuItem();
            menuItemHelp = new ToolStripMenuItem();
            menuItemAbout = new ToolStripMenuItem();
            panelContent = new Panel();
            labelTitle = new Label();
            menuStrip.SuspendLayout();
            panelContent.SuspendLayout();
            SuspendLayout();

            // menuStrip
            menuStrip.Items.AddRange(new ToolStripItem[]
            {
                menuItemTools,
                menuItemCalc,
                menuItemHelp
            });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(1024, 24);
            menuStrip.TabIndex = 0;

            // menuItemTools
            menuItemTools.DropDownItems.AddRange(new ToolStripItem[] { menuItemSettings });
            menuItemTools.Name = "menuItemTools";
            menuItemTools.Text = "征缴参数(&P)";

            // menuItemSettings
            menuItemSettings.Name = "menuItemSettings";
            menuItemSettings.Text = "征缴参数查询(&Q)";
            menuItemSettings.Click += menuItemSettings_Click;

            // menuItemCalc
            menuItemCalc.DropDownItems.AddRange(new ToolStripItem[]
            {
                menuItemEntPension,
                menuItemEntBankruptcy,
                menuItemCivilToEnt,
                menuItemPersonalAccount
            });
            menuItemCalc.Name = "menuItemCalc";
            menuItemCalc.Text = "补缴测算(&C)";

            // menuItemEntPension
            menuItemEntPension.Name = "menuItemEntPension";
            menuItemEntPension.Text = "企业社会保险费补缴测算(&E)";
            menuItemEntPension.Click += menuItemEntPension_Click;

            // menuItemEntBankruptcy
            menuItemEntBankruptcy.Name = "menuItemEntBankruptcy";
            menuItemEntBankruptcy.Text = "企业破产社保费清算(&B)";
            menuItemEntBankruptcy.Click += menuItemEntBankruptcy_Click;

            // menuItemCivilToEnt
            menuItemCivilToEnt.Name = "menuItemCivilToEnt";
            menuItemCivilToEnt.Text = "老机保转企保补缴测算(&M)";
            menuItemCivilToEnt.Click += menuItemCivilToEnt_Click;

            // menuItemPersonalAccount
            menuItemPersonalAccount.Name = "menuItemPersonalAccount";
            menuItemPersonalAccount.Text = "城镇职工基本养老保险个人账户测算(&A)";
            menuItemPersonalAccount.Click += menuItemPersonalAccount_Click;

            // menuItemHelp
            menuItemHelp.DropDownItems.AddRange(new ToolStripItem[] { menuItemAbout });
            menuItemHelp.Name = "menuItemHelp";
            menuItemHelp.Text = "帮助(&H)";

            // menuItemAbout
            menuItemAbout.Name = "menuItemAbout";
            menuItemAbout.Text = "关于(&A)";
            menuItemAbout.Click += menuItemAbout_Click;

            // panelContent
            panelContent.BackColor = Color.WhiteSmoke;
            panelContent.Controls.Add(labelTitle);
            panelContent.Dock = DockStyle.Fill;
            panelContent.Location = new Point(0, 24);
            panelContent.Name = "panelContent";

            // labelTitle
            labelTitle.AutoSize = false;
            labelTitle.Dock = DockStyle.Fill;
            labelTitle.Font = new Font("Microsoft YaHei", 22F, FontStyle.Regular);
            labelTitle.ForeColor = Color.SteelBlue;
            labelTitle.Name = "labelTitle";
            labelTitle.Text = "社会保险费征缴管理工具";
            labelTitle.TextAlign = ContentAlignment.MiddleCenter;

            // MainForm
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1024, 640);
            Controls.Add(panelContent);
            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;
            MinimumSize = new Size(900, 600);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "社会保险费征缴管理工具 v2.0";

            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            panelContent.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private MenuStrip menuStrip;
        private ToolStripMenuItem menuItemTools;
        private ToolStripMenuItem menuItemSettings;
        private ToolStripMenuItem menuItemCalc;
        private ToolStripMenuItem menuItemEntPension;
        private ToolStripMenuItem menuItemEntBankruptcy;
        private ToolStripMenuItem menuItemCivilToEnt;
        private ToolStripMenuItem menuItemPersonalAccount;
        private ToolStripMenuItem menuItemHelp;
        private ToolStripMenuItem menuItemAbout;
        private Panel panelContent;
        private Label labelTitle;
    }
}
