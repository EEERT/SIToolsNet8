using SITools.Models.Entities;

namespace SITools.BLL.Services
{
    /// <summary>
    /// 利息计算服务接口
    /// </summary>
    public interface IInterestCalculationService
    {
        /// <summary>
        /// 计算利息（复利模式，按年度利率）
        /// </summary>
        /// <param name="principal">本金</param>
        /// <param name="startDate">费款所属期开始月份</param>
        /// <param name="endDate">计息截止日期</param>
        double CalculateInterest(double principal, DateTime startDate, DateTime endDate);

        /// <summary>获取所有年度利率</summary>
        Dictionary<int, double> GetAllRates();

        /// <summary>更新年度利率</summary>
        void UpdateRates(Dictionary<int, double> rates);

        /// <summary>获取利率数据列表</summary>
        List<RateData> GetRateDataList();
    }
}
