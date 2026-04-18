namespace SITools.BLL.Services
{
    /// <summary>
    /// 滞纳金计算服务实现
    /// 计算规则：日滞纳金率 0.05%（0.0005），最高不超过本金
    /// </summary>
    public class LateFeeCalculationService : ILateFeeCalculationService
    {
        private const double DailyLateFeeRate = 0.0005;

        /// <summary>
        /// 计算滞纳金
        /// </summary>
        /// <param name="principal">本金（单位+个人之和）</param>
        /// <param name="paymentDate">实际缴款日期</param>
        /// <param name="dueDate">费款所属月份对应的日期（次月起算）</param>
        /// <param name="contributionTypeName">补缴类型名称</param>
        public double CalculateLateFee(double principal, DateTime paymentDate, DateTime dueDate, string contributionTypeName)
        {
            if (paymentDate <= dueDate)
                return 0.0;

            int days;
            if (contributionTypeName == "职工158号文补缴")
            {
                // 158号文：从2011-07-01起算
                TimeSpan ts = paymentDate - new DateTime(2011, 7, 1);
                days = ts.Days + 1;
            }
            else
            {
                // 一般情形：从费款所属期次月起算
                TimeSpan ts = paymentDate - dueDate.AddMonths(1);
                days = ts.Days + 1;
            }

            if (days <= 0)
                return 0.0;

            double lateFee = principal * days * DailyLateFeeRate;

            // 滞纳金最高不超过本金
            return Math.Min(lateFee, principal);
        }
    }
}
