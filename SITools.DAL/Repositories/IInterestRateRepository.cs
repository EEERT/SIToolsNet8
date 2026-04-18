using SITools.Models.Entities;

namespace SITools.DAL.Repositories
{
    /// <summary>
    /// 社保记账利率仓储接口
    /// </summary>
    public interface IInterestRateRepository
    {
        /// <summary>获取所有年度利率</summary>
        Dictionary<int, double> GetAllRates();
        /// <summary>获取指定年度利率</summary>
        double GetRate(int year);
        /// <summary>更新利率</summary>
        void UpdateRates(Dictionary<int, double> rates);
        /// <summary>获取利率数据列表（用于界面显示）</summary>
        List<RateData> GetRateDataList();
    }
}
