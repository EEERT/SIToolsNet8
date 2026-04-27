using SITools.BLL.Services;

namespace SITools.UI.Forms
{
    /// <summary>
    /// 主窗体（应用程序的导航入口）。
    /// MainForm 是打开程序后看到的第一个窗口，通过菜单栏提供导航，
    /// 用户可以从这里进入各个功能模块：
    ///   - 企业养老补缴计算（EntPensionForm）
    ///   - 企业破产清算计算（EntBankruptcyForm）
    ///   - 利率参数设置（SettingForm）
    ///   - 关于（AboutForm）
    ///
    /// 主窗体持有所有服务的引用，在打开子窗体时将其传递过去（构造函数注入）。
    /// </summary>
    public partial class MainForm : Form
    {
        // 各业务服务的引用（由程序入口 Program.Main 创建并传入）
        private readonly IPensionCalculationService _pensionSvc;
        private readonly IInterestCalculationService _interestSvc;
        private readonly ILateFeeCalculationService _lateFeeSvc;
        private readonly EntPensionCalculationFacade _calcFacade;
        private readonly ExcelService _excelSvc;

        /// <summary>
        /// 构造函数：接收所有服务实例并保存，然后初始化窗体控件（由设计器生成的 InitializeComponent）。
        /// </summary>
        public MainForm(
            IPensionCalculationService pensionSvc,
            IInterestCalculationService interestSvc,
            ILateFeeCalculationService lateFeeSvc,
            EntPensionCalculationFacade calcFacade,
            ExcelService excelSvc)
        {
            _pensionSvc = pensionSvc;
            _interestSvc = interestSvc;
            _lateFeeSvc = lateFeeSvc;
            _calcFacade = calcFacade;
            _excelSvc = excelSvc;

            InitializeComponent();  // 初始化窗体上的所有控件（由设计器自动生成）
        }

        /// <summary>
        /// 菜单"设置"点击事件：打开利率参数设置窗体。
        /// form.Show() 以非模态方式打开（用户可以同时操作主窗体和设置窗体）。
        /// </summary>
        private void menuItemSettings_Click(object sender, EventArgs e)
        {
            var form = new SettingForm(_interestSvc);
            form.Show();
        }

        /// <summary>
        /// 菜单"企业养老补缴"点击事件：打开企业养老保险补缴计算窗体。
        /// 将所有必要的服务传递给子窗体。
        /// </summary>
        private void menuItemEntPension_Click(object sender, EventArgs e)
        {
            var form = new EntPensionForm(_pensionSvc, _interestSvc, _lateFeeSvc, _calcFacade, _excelSvc);
            form.Show();
        }

        /// <summary>
        /// 菜单"企业破产清算"点击事件：打开企业破产清算计算窗体。
        /// 破产清算场景不需要本金计算服务，因为本金从Excel导入。
        /// </summary>
        private void menuItemEntBankruptcy_Click(object sender, EventArgs e)
        {
            var form = new EntBankruptcyForm(_interestSvc, _lateFeeSvc, _excelSvc);
            form.Show();
        }

        /// <summary>
        /// 菜单"机关转企业"点击事件：该功能尚未开发，显示提示消息。
        /// </summary>
        private void menuItemCivilToEnt_Click(object sender, EventArgs e)
        {
            MessageBox.Show("功能正在开发中……", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 菜单"个人账户"点击事件：该功能尚未开发，显示提示消息。
        /// </summary>
        private void menuItemPersonalAccount_Click(object sender, EventArgs e)
        {
            MessageBox.Show("功能正在开发中……", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 菜单"关于"点击事件：以模态方式打开关于窗体。
        /// ShowDialog() 打开模态窗体（必须关闭该窗体才能操作主窗体，与 Show() 不同）。
        /// </summary>
        private void menuItemAbout_Click(object sender, EventArgs e)
        {
            var form = new AboutForm();
            form.ShowDialog();
        }
    }
}
