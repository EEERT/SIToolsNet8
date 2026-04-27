using SITools.Models.Entities;

namespace SITools.BLL.Services
{
    /// <summary>
    /// 利息计算服务接口。
    /// 定义了按年度社保记账利率（复利模式）计算利息的方法，
    /// 以及管理利率数据的读写方法。
    /// </summary>
    public interface IInterestCalculationService
    {
        /// <summary>
        /// 计算利息（复利模式，按年度利率分段计算）。
        /// 计算规则（三段式复利）：
        ///   第一段（起始年）：本金 × 当年利率 × (12 - 起始月 + 1) / 12
        ///   中间各完整年：（本金 + 历年累计利息）× 当年利率
        ///   最后一段（截止年）：（本金 + 历年累计利息）× 当年利率 × (截止月 - 1) / 12
        /// </summary>
        /// <param name="principal">本金（元）</param>
        /// <param name="startDate">费款所属期的开始月份（对应缴费月份的第一天）</param>
        /// <param name="endDate">计息截止日期（通常为用户指定的截止日）</param>
        /// <returns>计算得到的利息金额（元）</returns>
        double CalculateInterest(double principal, DateTime startDate, DateTime endDate);

        /// <summary>
        /// 获取所有年度的利率数据（字典形式，键=年份，值=利率小数）。
        /// </summary>
        Dictionary<int, double> GetAllRates();

        /// <summary>
        /// 更新年度利率数据（用于在设置界面修改利率后生效）。
        /// </summary>
        void UpdateRates(Dictionary<int, double> rates);

        /// <summary>
        /// 获取利率数据列表（RateData 对象列表，按年份升序），供界面绑定展示。
        /// </summary>
        List<RateData> GetRateDataList();
    }
}
