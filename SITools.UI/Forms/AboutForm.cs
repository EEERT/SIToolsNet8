namespace SITools.UI.Forms
{
    /// <summary>
    /// 关于窗体（AboutForm）。
    /// 显示软件版本、版权、作者等信息，是一个只读展示的简单窗体。
    /// 窗体内容由设计器（Designer）在 AboutForm.Designer.cs 中定义。
    /// </summary>
    public partial class AboutForm : Form
    {
        /// <summary>
        /// 构造函数：初始化窗体控件（由设计器生成的布局代码）。
        /// </summary>
        public AboutForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// "关闭"按钮点击事件：调用 Close() 关闭本窗体。
        /// </summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
