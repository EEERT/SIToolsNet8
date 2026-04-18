using SITools.BLL.Services;
using SITools.Models.Entities;

namespace SITools.UI.Forms
{
    public partial class SettingForm : Form
    {
        private readonly IInterestCalculationService _interestSvc;

        public SettingForm(IInterestCalculationService interestSvc)
        {
            _interestSvc = interestSvc;
            InitializeComponent();
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            var list = _interestSvc.GetRateDataList();
            dgvRates.DataSource = null;
            dgvRates.DataSource = list;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var updatedRates = new Dictionary<int, double>();
            foreach (DataGridViewRow row in dgvRates.Rows)
            {
                if (row.IsNewRow) continue;
                if (row.Cells["colYear"].Value == null || row.Cells["colRate"].Value == null) continue;
                if (int.TryParse(row.Cells["colYear"].Value.ToString(), out int year) &&
                    double.TryParse(row.Cells["colRate"].Value.ToString(), out double rate))
                {
                    updatedRates[year] = rate;
                }
            }
            _interestSvc.UpdateRates(updatedRates);
            MessageBox.Show("利率参数已更新！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }
    }
}
