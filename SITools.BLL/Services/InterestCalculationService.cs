using SITools.Models.Entities;
using SITools.DAL.Repositories;

namespace SITools.BLL.Services
{
    /// <summary>
    /// 利息计算服务实现（按年度社保记账利率，三段式复利计算）。
    ///
    /// ★ 计算逻辑说明（以本金P、起始月份S、截止日期E为例）：
    ///
    ///   假设 S = 2010年3月，E = 2023年5月，各年度利率已知。
    ///
    ///   1. 第一年（S所在年，即2010年）的利息 interest0：
    ///      interest0 = P × rate[2010] × (12 - 3 + 1) / 12
    ///      含义：本金P在2010年只计了10个月的利息（3月到12月，共10个月）。
    ///
    ///   2. 中间完整年（2011年至2022年）的利息 interestMid（逐年复利叠加）：
    ///      每年新增利息 = (P + interest0 + 前几年累计利息) × rate[当年]
    ///      interestSum 是中间各年利息的累加总和。
    ///
    ///   3. 最后一年（E所在年，即2023年）的利息 interestLast：
    ///      interestLast = (P + interest0 + interestSum) × rate[2023] × (5 - 1) / 12
    ///      含义：2023年只计了4个月的利息（到5月，但5月本身不参与，所以 endDate.Month - 1 = 4）。
    ///
    ///   最终利息 = interest0 + interestSum + interestLast
    /// </summary>
    public class InterestCalculationService : IInterestCalculationService
    {
        // 利率仓储，通过构造函数注入
        private readonly IInterestRateRepository _rateRepo;

        /// <summary>
        /// 构造函数：接收利率仓储对象（依赖注入）。
        /// </summary>
        public InterestCalculationService(IInterestRateRepository rateRepo)
        {
            _rateRepo = rateRepo;
        }

        // 以下三个方法直接委托给仓储层，不做额外处理
        public Dictionary<int, double> GetAllRates() => _rateRepo.GetAllRates();
        public void UpdateRates(Dictionary<int, double> rates) => _rateRepo.UpdateRates(rates);
        public List<RateData> GetRateDataList() => _rateRepo.GetRateDataList();

        /// <summary>
        /// 按三段式复利计算利息。
        /// 如果截止日期不晚于起始日期，利息为0（不可能为负值）。
        /// </summary>
        public double CalculateInterest(double principal, DateTime startDate, DateTime endDate)
        {
            // 如果截止日早于或等于起始日，没有欠费天数，利息为0
            if (endDate <= startDate)
                return 0.0;

            // 获取全部年度利率字典
            var rates = _rateRepo.GetAllRates();

            // ----------------------------------------------------------------
            // 第一段：起始年（startDate.Year）的利息
            // 计算方式：本金 × 当年利率 × 该年剩余月数 / 12
            // 例：起始月为3月，则该年剩余10个月（3、4、5…12月），即 (12 - 3 + 1) / 12 = 10/12
            // ----------------------------------------------------------------
            double interest0 = principal * rates[startDate.Year] * (12 - startDate.Month + 1) / 12.0;

            double interestSum = 0.0;   // 中间各年利息的累加总和
            double interestMid = 0.0;   // 当前循环年的利息（临时变量）
            double interestLast = 0.0;  // 最后一年（截止年）的利息

            // ----------------------------------------------------------------
            // 逐年循环：从 startDate.Year + 1 到 endDate.Year
            // ----------------------------------------------------------------
            for (int year = startDate.Year + 1; year <= endDate.Year; year++)
            {
                // 将上一次循环计算的中间年利息加入累计总和
                interestSum += interestMid;

                if (year < endDate.Year)
                {
                    // 中间完整年：整年复利
                    // 当年利息 = （本金 + 第一年利息 + 历年累计利息）× 当年利率
                    interestMid = (principal + interest0 + interestSum) * rates[year];
                }
                else
                {
                    // 最后一年（截止年）：只计到 endDate.Month 的前一个月
                    // 例：截止日期为5月，则只计4个月的利息（(5 - 1) / 12 = 4/12）
                    interestLast = (principal + interest0 + interestSum) * rates[year] * (endDate.Month - 1) / 12.0;
                }
            }

            // 三段利息求和（若 startDate 和 endDate 在同一年，interestSum 和 interestLast 均为0）
            return interest0 + interestSum + interestLast;
        }
    }
}
