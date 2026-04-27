using SITools.Models.Entities;

namespace SITools.BLL.Services
{
    /// <summary>
    /// 企业养老保险补缴计算门面服务（Facade 模式）。
    ///
    /// "门面"（Facade）是一种设计模式：它对外提供一个简单的接口，
    /// 内部统一协调多个子服务（本金计算、利息计算、滞纳金计算），
    /// 使调用方不需要直接与每个子服务交互。
    ///
    /// 主要职责：
    ///   1. Calculate：将用户输入的补缴记录展开为逐月明细，调用三个子服务计算每月的本金、利息和滞纳金。
    ///   2. Summarize：将逐月明细按身份证号汇总，生成人员汇总报表。
    /// </summary>
    public class EntPensionCalculationFacade
    {
        // 三个子服务，通过构造函数注入
        private readonly IPensionCalculationService _pensionSvc;     // 负责计算本金和基数约束
        private readonly IInterestCalculationService _interestSvc;   // 负责计算利息
        private readonly ILateFeeCalculationService _lateFeeSvc;     // 负责计算滞纳金

        /// <summary>
        /// 构造函数：接收并保存三个子服务的实例（依赖注入）。
        /// </summary>
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
        /// 执行补缴计算，将输入的补缴记录列表展开为逐月明细列表。
        ///
        /// 计算步骤（对每条补缴记录的每个月份）：
        ///   1. 解析起止期，逐月循环（从 BeginPeriod 到 EndPeriod）
        ///   2. 如果开启基数上下限约束，则先对基数进行约束
        ///   3. 计算单位部分本金和个人部分本金（保留2位小数）
        ///   4. 如果需要计算利息，调用利息服务计算单位和个人利息
        ///   5. 如果需要计算滞纳金，调用滞纳金服务计算
        ///   6. 汇总合计，生成一条明细记录
        /// </summary>
        /// <param name="records">用户输入的补缴记录列表（每条记录含姓名、身份证、起止期、基数、补缴类型等）</param>
        /// <param name="calculateInterest">是否计算利息（true=计算，false=不计算，利息为0）</param>
        /// <param name="interestEndDate">利息计算截止日期</param>
        /// <param name="calculateLateFee">是否计算滞纳金（true=计算，false=不计算）</param>
        /// <param name="lateFeeEndDate">滞纳金计算截止日期（即实际缴款日）</param>
        /// <returns>逐月明细结果列表（每月一行）</returns>
        public List<CalculationResultDetail> Calculate(
            List<ContributionRecord> records,
            bool calculateInterest,
            DateTime interestEndDate,
            bool calculateLateFee,
            DateTime lateFeeEndDate)
        {
            var results = new List<CalculationResultDetail>();

            // 遍历每条补缴记录
            foreach (var rec in records)
            {
                // 将 yyyyMM 整数格式的起止期解析为 DateTime 对象（取各月的1日）
                // 例：198601 → 1986年1月1日；202412 → 2024年12月1日
                var beginDate = DateTime.ParseExact(rec.BeginPeriod.ToString(), "yyyyMM", null);
                var endDate = DateTime.ParseExact(rec.EndPeriod.ToString(), "yyyyMM", null);

                // 获取补缴类型的中文名称（用于滞纳金特殊规则判断和结果展示）
                string typeName = GetContributionTypeName(rec.ContributionType);

                // 逐月循环：从 beginDate 到 endDate，每次加一个月
                for (var date = beginDate; date <= endDate; date = date.AddMonths(1))
                {
                    // 将日期转回 yyyyMM 整数格式，用于查找费率等
                    int period = int.Parse(date.ToString("yyyyMM"));
                    double baseAmount = rec.MonthlyBase;

                    // 如果勾选了"保底"（ApplyBaseLimit），则对基数进行上下限约束
                    if (rec.ApplyBaseLimit)
                        baseAmount = _pensionSvc.ApplyBaseLimit(baseAmount, period);

                    // 计算单位和个人部分本金（Math.Round 保留2位小数，避免浮点数精度问题）
                    double unitPrincipal = Math.Round(
                        _pensionSvc.CalculateUnitPrincipal(baseAmount, rec.ContributionType, period), 2);
                    double personalPrincipal = Math.Round(
                        _pensionSvc.CalculatePersonalPrincipal(baseAmount, rec.ContributionType, period), 2);

                    // 计算利息（如果不需要，则利息为0）
                    double unitInterest = 0.0, personalInterest = 0.0;
                    if (calculateInterest)
                    {
                        unitInterest = Math.Round(_interestSvc.CalculateInterest(unitPrincipal, date, interestEndDate), 2);
                        personalInterest = Math.Round(_interestSvc.CalculateInterest(personalPrincipal, date, interestEndDate), 2);
                    }

                    // 计算滞纳金（如果不需要，则滞纳金为0）
                    double lateFee = 0.0;
                    if (calculateLateFee)
                    {
                        // 滞纳金以（单位本金+个人本金）的总和为基数计算
                        double totalPrincipal = unitPrincipal + personalPrincipal;
                        lateFee = Math.Round(_lateFeeSvc.CalculateLateFee(totalPrincipal, lateFeeEndDate, date, typeName), 2);
                    }

                    // 合计 = 单位本金 + 单位利息 + 个人本金 + 个人利息 + 滞纳金
                    double total = Math.Round(unitPrincipal + unitInterest + personalPrincipal + personalInterest + lateFee, 2);

                    // 将本月计算结果添加到结果列表
                    results.Add(new CalculationResultDetail
                    {
                        Name = rec.Name,
                        IdCardNo = rec.IdCardNo,
                        Period = date.ToString("yyyyMM"),
                        MonthlyBase = Math.Round(baseAmount, 0),  // 基数取整到元
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
        /// 对逐月明细列表按身份证号分组汇总，生成人员汇总报表。
        ///
        /// 使用 LINQ（Language Integrated Query，语言集成查询）进行数据聚合：
        ///   - GroupBy：按身份证号分组（同一人的所有月份归为一组）
        ///   - Select：对每组计算汇总值（姓名、期号范围、各项金额之和）
        ///   - ToList：将结果收集为列表
        /// </summary>
        /// <param name="details">逐月明细列表（由 Calculate 方法返回）</param>
        /// <returns>按人员汇总的结果列表（每人一行）</returns>
        public List<CalculationResultSummary> Summarize(List<CalculationResultDetail> details)
        {
            return details
                .GroupBy(d => d.IdCardNo)  // 按身份证号分组
                .Select(grp => new CalculationResultSummary
                {
                    Name = grp.First().Name,              // 取该组第一条记录的姓名
                    IdCardNo = grp.Key,                   // 分组键就是身份证号
                    PeriodRange = $"{grp.Min(d => d.Period)}-{grp.Max(d => d.Period)}",  // 起止期范围
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

        /// <summary>
        /// 将补缴类型枚举转换为对应的中文名称（用于结果展示）。
        /// </summary>
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
