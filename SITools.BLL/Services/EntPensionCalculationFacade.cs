using SITools.Models.Entities;

namespace SITools.BLL.Services
{
    /// <summary>
    /// 企业养老保险补缴计算门面服务（协调各子服务）
    /// </summary>
    public class EntPensionCalculationFacade
    {
        private readonly IPensionCalculationService _pensionSvc;
        private readonly IInterestCalculationService _interestSvc;
        private readonly ILateFeeCalculationService _lateFeeSvc;

        public EntPensionCalculationFacade(
            IPensionCalculationService pensionSvc,
            IInterestCalculationService interestSvc,
            ILateFeeCalculationService lateFeeSvc)
        {
            _pensionSvc = pensionSvc;
            _interestSvc = interestSvc;
            _lateFeeSvc = lateFeeSvc;
        }

        /// <summary>
        /// 执行补缴计算，返回逐月明细列表
        /// </summary>
        public List<CalculationResultDetail> Calculate(
            List<ContributionRecord> records,
            bool calculateInterest,
            DateTime interestEndDate,
            bool calculateLateFee,
            DateTime lateFeeEndDate)
        {
            var results = new List<CalculationResultDetail>();

            foreach (var rec in records)
            {
                var beginDate = DateTime.ParseExact(rec.BeginPeriod.ToString(), "yyyyMM", null);
                var endDate = DateTime.ParseExact(rec.EndPeriod.ToString(), "yyyyMM", null);
                string typeName = GetContributionTypeName(rec.ContributionType);

                for (var date = beginDate; date <= endDate; date = date.AddMonths(1))
                {
                    int period = int.Parse(date.ToString("yyyyMM"));
                    double baseAmount = rec.MonthlyBase;

                    if (rec.ApplyBaseLimit)
                        baseAmount = _pensionSvc.ApplyBaseLimit(baseAmount, date.Year);

                    double unitPrincipal = Math.Round(
                        _pensionSvc.CalculateUnitPrincipal(baseAmount, rec.ContributionType, period), 2);
                    double personalPrincipal = Math.Round(
                        _pensionSvc.CalculatePersonalPrincipal(baseAmount, rec.ContributionType, period), 2);

                    double unitInterest = 0.0, personalInterest = 0.0;
                    if (calculateInterest)
                    {
                        unitInterest = Math.Round(_interestSvc.CalculateInterest(unitPrincipal, date, interestEndDate), 2);
                        personalInterest = Math.Round(_interestSvc.CalculateInterest(personalPrincipal, date, interestEndDate), 2);
                    }

                    double lateFee = 0.0;
                    if (calculateLateFee)
                    {
                        double totalPrincipal = unitPrincipal + personalPrincipal;
                        lateFee = Math.Round(_lateFeeSvc.CalculateLateFee(totalPrincipal, lateFeeEndDate, date, typeName), 2);
                    }

                    double total = Math.Round(unitPrincipal + unitInterest + personalPrincipal + personalInterest + lateFee, 2);

                    results.Add(new CalculationResultDetail
                    {
                        Name = rec.Name,
                        IdCardNo = rec.IdCardNo,
                        Period = date.ToString("yyyyMM"),
                        MonthlyBase = Math.Round(baseAmount, 0),
                        ContributionTypeName = typeName,
                        UnitPrincipal = unitPrincipal,
                        UnitInterest = unitInterest,
                        PersonalPrincipal = personalPrincipal,
                        PersonalInterest = personalInterest,
                        LateFee = lateFee,
                        Total = total,
                    });
                }
            }

            return results;
        }

        /// <summary>
        /// 对明细列表按身份证分组汇总
        /// </summary>
        public List<CalculationResultSummary> Summarize(List<CalculationResultDetail> details)
        {
            return details
                .GroupBy(d => d.IdCardNo)
                .Select(grp => new CalculationResultSummary
                {
                    Name = grp.First().Name,
                    IdCardNo = grp.Key,
                    PeriodRange = $"{grp.Min(d => d.Period)}-{grp.Max(d => d.Period)}",
                    TotalBase = Math.Round(grp.Sum(d => d.MonthlyBase), 2),
                    UnitPrincipal = Math.Round(grp.Sum(d => d.UnitPrincipal), 2),
                    UnitInterest = Math.Round(grp.Sum(d => d.UnitInterest), 2),
                    PersonalPrincipal = Math.Round(grp.Sum(d => d.PersonalPrincipal), 2),
                    PersonalInterest = Math.Round(grp.Sum(d => d.PersonalInterest), 2),
                    LateFee = Math.Round(grp.Sum(d => d.LateFee), 2),
                    Total = Math.Round(grp.Sum(d => d.Total), 2),
                })
                .ToList();
        }

        private static string GetContributionTypeName(ContributionType type)
        {
            return type switch
            {
                ContributionType.Doc158 => "职工158号文补缴",
                ContributionType.SupplementInterruption => "职工补中断",
                ContributionType.HistoricalDebt => "历史陈欠清算",
                ContributionType.Individual => "个体缴费",
                ContributionType.Doc62ContractWorker => "62号文件补缴（合同工）",
                ContributionType.Doc62RegularWorker => "62号文件补缴（固定工）",
                _ => "未知类型"
            };
        }
    }
}
