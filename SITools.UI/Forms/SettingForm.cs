using SITools.BLL.Services;
using SITools.Models.Entities;

namespace SITools.UI.Forms
{
    /// <summary>
    /// 利率参数设置窗体。
    /// 允许用户查看和修改历年社保记账利率数据。
    ///
    /// 界面说明：
    ///   - dgvRates：展示所有年度利率的 DataGridView 表格（每行：年度 + 利率）
    ///   - btnSave：将表格中修改后的利率保存到内存（当前运行期间生效）
    ///   - btnReset：放弃修改，从内存重新加载原始利率数据
    ///
    /// 注意：本窗体的修改仅在程序运行期间生效，重启程序后恢复为代码内置的默认值。
    /// 如需永久保存，需要修改 InterestRateRepository 中的数据或添加文件/数据库持久化功能。
    /// </summary>
    public partial class SettingForm : Form
    {
        // 利息计算服务（通过它读写利率数据）
        private readonly IInterestCalculationService _interestSvc;

        /// <summary>
        /// 构造函数：接收利息服务实例，初始化窗体控件。
        /// </summary>
        public SettingForm(IInterestCalculationService interestSvc)
        {
            _interestSvc = interestSvc;
            InitializeComponent();
        }

        /// <summary>
        /// 窗体加载事件：窗体显示时自动加载利率数据到表格。
        /// </summary>
        private void SettingForm_Load(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        /// <summary>
        /// 从服务层读取利率数据，重新绑定到表格（用于初始加载和重置操作）。
        /// DataSource 先设 null 再重新赋值，确保表格完全刷新。
        /// </summary>
        private void RefreshGrid()
        {
            var list = _interestSvc.GetRateDataList();
            dgvRates.DataSource = null;   // 先断开绑定，触发表格刷新
            dgvRates.DataSource = list;   // 重新绑定新数据
        }

        /// <summary>
        /// "保存"按钮：将表格中用户修改的利率数据收集后更新到服务层内存中。
        ///
        /// 操作步骤：
        ///   1. 遍历表格所有行（跳过空行和无效值）
        ///   2. 解析年份（int）和利率（double）
        ///   3. 调用服务层的 UpdateRates 方法更新内存中的利率字典
        ///   4. 弹出提示确认保存成功
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            var updatedRates = new Dictionary<int, double>();
            foreach (DataGridViewRow row in dgvRates.Rows)
            {
                if (row.IsNewRow) continue;  // 跳过末尾占位行
                if (row.Cells["colYear"].Value == null || row.Cells["colRate"].Value == null) continue;  // 跳过空值行

                // 尝试解析年份和利率，解析失败的行直接跳过（容错处理）
                if (int.TryParse(row.Cells["colYear"].Value.ToString(), out int year) &&
                    double.TryParse(row.Cells["colRate"].Value.ToString(), out double rate))
                {
                    updatedRates[year] = rate;
                }
            }
            _interestSvc.UpdateRates(updatedRates);
            MessageBox.Show("利率参数已更新！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// "重置"按钮：丢弃当前表格中的修改，重新从服务层加载利率数据。
        /// </summary>
        private void btnReset_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }
    }
}
