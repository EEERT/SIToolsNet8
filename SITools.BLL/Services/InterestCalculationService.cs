using SITools.Models.Entities;
using SITools.DAL.Repositories;

namespace SITools.BLL.Services
{
    /// <summary>
    /// 利息计算服务实现（按年度社保记账利率，复利计算）
    /// </summary>
    public class InterestCalculationService : IInterestCalculationService
    {
        private readonly IInterestRateRepository _rateRepo;

        public InterestCalculationService(IInterestRateRepository rateRepo)
        {
            _rateRepo = rateRepo;
        }

        public Dictionary<int, double> GetAllRates() => _rateRepo.GetAllRates();

        public void UpdateRates(Dictionary<int, double> rates) => _rateRepo.UpdateRates(rates);

        public List<RateData> GetRateDataList() => _rateRepo.GetRateDataList();

        /// <summary>
        /// 利息计算逻辑：
        /// 1. 第一年（startDate所在年）：本金 × 年利率 × (12 - 起始月 + 1) / 12
        /// 2. 中间各年：（本金 + 第一年利息 + 历年累计利息）× 年利率
        /// 3. 最后一年（endDate所在年）：（本金 + 第一年利息 + 历年累计利息）× 年利率 × (endDate月 - 1) / 12
        /// </summary>
        public double CalculateInterest(double principal, DateTime startDate, DateTime endDate)
        {
            if (endDate <= startDate)
                return 0.0;

            var rates = _rateRepo.GetAllRates();

            double interest0 = principal * rates[startDate.Year] * (12 - startDate.Month + 1) / 12.0;

            double interestSum = 0.0;
            double interestMid = 0.0;
            double interestLast = 0.0;

            for (int year = startDate.Year + 1; year <= endDate.Year; year++)
            {
                interestSum += interestMid;
                if (year < endDate.Year)
                {
                    interestMid = (principal + interest0 + interestSum) * rates[year];
                }
                else
                {
                    interestLast = (principal + interest0 + interestSum) * rates[year] * (endDate.Month - 1) / 12.0;
                }
            }

            return interest0 + interestSum + interestLast;
        }
    }
}
