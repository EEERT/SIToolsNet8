namespace SITools.BLL.Services
{
    /// <summary>
    /// 滞纳金计算服务实现。
    ///
    /// ★ 计算规则：
    ///   滞纳金 = 本金（单位+个人之和） × 逾期天数 × 0.05%（日滞纳金率）
    ///   最终结果上限：不超过本金总额。
    ///
    /// ★ 逾期天数的起算规则：
    ///   一般情形：从费款所属期的次月（第一天）起算到实际缴款日（含当天）。
    ///   例：2020年1月的欠费，应从2020年2月1日起算逾期天数。
    ///
    /// ★ 158号文特殊规则：
    ///   依据政策规定，158号文补缴的滞纳金一律从2011年7月1日起算，
    ///   与费款所属期无关。
    /// </summary>
    public class LateFeeCalculationService : ILateFeeCalculationService
    {
        // 日滞纳金率：0.05%（即万分之五）
        private const double DailyLateFeeRate = 0.0005;

        /// <summary>
        /// 计算滞纳金。
        /// </summary>
        /// <param name="principal">本金（单位缴费金额 + 个人缴费金额，元）</param>
        /// <param name="paymentDate">实际缴款日期（用户指定的截止日）</param>
        /// <param name="dueDate">费款所属月份的日期（即该月的1日；逾期从次月起算）</param>
        /// <param name="contributionTypeName">补缴类型名称（"职工158号文补缴"有特殊起算规则）</param>
        /// <returns>应收滞纳金（元）；若未逾期则返回0</returns>
        public double CalculateLateFee(double principal, DateTime paymentDate, DateTime dueDate, string contributionTypeName)
        {
            // 如果实际缴款日期不晚于应缴期，说明没有逾期，无需收取滞纳金
            if (paymentDate <= dueDate)
                return 0.0;

            int days;  // 逾期天数

            if (contributionTypeName == "职工158号文补缴")
            {
                // 158号文特殊规则：统一从2011年7月1日起算逾期
                // TimeSpan 是一种表示时间差的类型，.Days 属性给出整天数
                TimeSpan ts = paymentDate - new DateTime(2011, 7, 1);
                days = ts.Days + 1;  // +1 是因为起算当天也算一天逾期
            }
            else
            {
                // 一般情形：从费款所属期的次月起算
                // dueDate.AddMonths(1) 将日期向后推一个月，得到逾期起算日
                TimeSpan ts = paymentDate - dueDate.AddMonths(1);
                days = ts.Days + 1;  // +1 包含起算当天
            }

            // 如果逾期天数不为正（例如缴款日恰好在起算日当天或之前），不收滞纳金
            if (days <= 0)
                return 0.0;

            // 计算滞纳金：本金 × 天数 × 日滞纳金率
            double lateFee = principal * days * DailyLateFeeRate;

            // 滞纳金上限：不超过本金总额（政策规定）
            return Math.Min(lateFee, principal);
        }
    }
}
