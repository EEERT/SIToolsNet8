using SITools.Models.Entities;
using SITools.DAL.Repositories;

namespace SITools.BLL.Services
{
    /// <summary>
    /// 养老保险补缴计算服务实现
    /// </summary>
    public class PensionCalculationService : IPensionCalculationService
    {
        private readonly IContributionBaseRepository _baseRepo;

        private static readonly Dictionary<string, ContributionType> _nameToType = new()
        {
            { "职工158号文补缴",       ContributionType.Doc158 },
            { "职工补中断",             ContributionType.SupplementInterruption },
            { "历史陈欠清算",           ContributionType.HistoricalDebt },
            { "个体缴费",               ContributionType.Individual },
            { "62号文件补缴（合同工）", ContributionType.Doc62ContractWorker },
            { "62号文件补缴（固定工）", ContributionType.Doc62RegularWorker },
        };

        public PensionCalculationService(IContributionBaseRepository baseRepo)
        {
            _baseRepo = baseRepo;
        }

        public List<string> GetContributionTypeNames()
        {
            return new List<string>(_nameToType.Keys);
        }

        public ContributionType ParseContributionType(string name)
        {
            if (_nameToType.TryGetValue(name, out var type))
                return type;
            throw new ArgumentException($"未知的补缴类型：{name}");
        }

        public double ApplyBaseLimit(double baseAmount, int period)
        {
            int year = period / 100;
            int month = period % 100;
            if (!_baseRepo.ContainsYear(year))
                return baseAmount;
            double min = _baseRepo.GetMinBase(year, month);
            double max = _baseRepo.GetMaxBase(year, month);
            if (baseAmount < min) return min;
            if (baseAmount > max) return max;
            return baseAmount;
        }

        /// <summary>计算单位部分本金</summary>
        public double CalculateUnitPrincipal(double baseAmount, ContributionType contributionType, int period)
        {
            double rate = GetUnitContributionRate(contributionType, period);
            return baseAmount * rate;
        }

        /// <summary>计算个人部分本金</summary>
        public double CalculatePersonalPrincipal(double baseAmount, ContributionType contributionType, int period)
        {
            double rate = GetPersonalContributionRate(contributionType, period);

            // 特殊处理：199204-199212期间，个人部分固定为2.5元
            if (period >= 199204 && period <= 199212)
                return 2.5;

            return baseAmount * rate;
        }

        private static double GetUnitContributionRate(ContributionType type, int period)
        {
            return type switch
            {
                ContributionType.Doc158 => 0.2,
                ContributionType.SupplementInterruption => period switch
                {
                    >= 201107 and <= 201604 => 0.20,
                    >= 201605 and <= 201904 => 0.19,
                    >= 201905 => 0.16,
                    _ => 0.0
                },
                ContributionType.HistoricalDebt => period switch
                {
                    >= 198601 and <= 199112 => 0.16,
                    >= 199201 and <= 199312 => 0.19,
                    >= 199401 and <= 199712 => 0.23,
                    >= 199801 and <= 200012 => 0.22,
                    >= 200101 and <= 200212 => 0.21,
                    >= 200301 and <= 201604 => 0.20,
                    >= 201605 and <= 201904 => 0.19,
                    >= 201905 => 0.16,
                    _ => 0.0
                },
                ContributionType.Individual => 0.12,
                ContributionType.Doc62ContractWorker => period switch
                {
                    >= 198601 and <= 199112 => 0.16,
                    >= 199201 and <= 199312 => 0.19,
                    >= 199401 and <= 199512 => 0.23,
                    _ => 0.0
                },
                ContributionType.Doc62RegularWorker => period switch
                {
                    >= 198601 and <= 199112 => 0.16,
                    >= 199201 and <= 199312 => 0.19,
                    >= 199401 and <= 199512 => 0.23,
                    _ => 0.0
                },
                _ => 0.0
            };
        }

        private static double GetPersonalContributionRate(ContributionType type, int period)
        {
            return type switch
            {
                ContributionType.Doc158 => 0.08,
                ContributionType.SupplementInterruption => 0.08,
                ContributionType.HistoricalDebt => period switch
                {
                    >= 198601 and <= 199312 => 0.03,
                    >= 199401 and <= 199712 => 0.03,
                    >= 199801 and <= 199906 => 0.04,
                    >= 199907 and <= 200012 => 0.05,
                    >= 200101 and <= 200212 => 0.06,
                    >= 200301 => 0.08,
                    _ => 0.0
                },
                ContributionType.Individual => 0.08,
                ContributionType.Doc62ContractWorker => 0.03,
                ContributionType.Doc62RegularWorker => period switch
                {
                    >= 198601 and <= 199212 => 0.03,
                    >= 199301 and <= 199512 => 0.02,
                    _ => 0.0
                },
                _ => 0.0
            };
        }
    }
}
