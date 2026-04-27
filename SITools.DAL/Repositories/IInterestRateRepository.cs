using SITools.Models.Entities;

namespace SITools.DAL.Repositories
{
    /// <summary>
    /// 社保记账利率仓储接口。
    /// 定义了读取、查询和更新历年社保记账利率所需的所有方法。
    /// 利率数据用于计算历史欠费的利息：欠费年份越早，累积利息越多。
    /// </summary>
    public interface IInterestRateRepository
    {
        /// <summary>
        /// 获取所有年度的利率，以字典形式返回（键=年份，值=利率小数）。
        /// 例如：{2023, 0.0297} 表示2023年度的利率为2.97%。
        /// </summary>
        Dictionary<int, double> GetAllRates();

        /// <summary>
        /// 获取指定年度的利率（小数形式，如 0.0297 表示 2.97%）。
        /// 如果该年份没有数据，会抛出 KeyNotFoundException 异常。
        /// </summary>
        /// <param name="year">要查询的年份</param>
        double GetRate(int year);

        /// <summary>
        /// 更新（替换）所有年度的利率数据。
        /// 在"设置"界面修改利率参数后，通过此方法将新数据写入内存。
        /// </summary>
        /// <param name="rates">新的利率字典（键=年份，值=利率小数）</param>
        void UpdateRates(Dictionary<int, double> rates);

        /// <summary>
        /// 获取利率数据列表（用于界面DataGridView绑定展示）。
        /// 将字典转换为按年份升序排列的 RateData 对象列表，便于界面显示和编辑。
        /// </summary>
        List<RateData> GetRateDataList();
    }
}
