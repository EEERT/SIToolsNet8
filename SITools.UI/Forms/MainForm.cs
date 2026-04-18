using SITools.BLL.Services;

namespace SITools.UI.Forms
{
    public partial class MainForm : Form
    {
        private readonly IPensionCalculationService _pensionSvc;
        private readonly IInterestCalculationService _interestSvc;
        private readonly ILateFeeCalculationService _lateFeeSvc;
        private readonly EntPensionCalculationFacade _calcFacade;
        private readonly ExcelService _excelSvc;

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

            InitializeComponent();
        }

        private void menuItemSettings_Click(object sender, EventArgs e)
        {
            var form = new SettingForm(_interestSvc);
            form.Show();
        }

        private void menuItemEntPension_Click(object sender, EventArgs e)
        {
            var form = new EntPensionForm(_pensionSvc, _interestSvc, _lateFeeSvc, _calcFacade, _excelSvc);
            form.Show();
        }

        private void menuItemEntBankruptcy_Click(object sender, EventArgs e)
        {
            var form = new EntBankruptcyForm(_interestSvc, _lateFeeSvc, _excelSvc);
            form.Show();
        }

        private void menuItemCivilToEnt_Click(object sender, EventArgs e)
        {
            MessageBox.Show("功能正在开发中……", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void menuItemPersonalAccount_Click(object sender, EventArgs e)
        {
            MessageBox.Show("功能正在开发中……", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void menuItemAbout_Click(object sender, EventArgs e)
        {
            var form = new AboutForm();
            form.ShowDialog();
        }
    }
}
