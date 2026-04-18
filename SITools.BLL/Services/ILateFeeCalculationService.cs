namespace SITools.BLL.Services
{
    /// <summary>
    /// 滞纳金计算服务接口
    /// </summary>
    public interface ILateFeeCalculationService
    {
        /// <summary>
        /// 计算滞纳金
        /// </summary>
        /// <param name="principal">本金（单位+个人）</param>
        /// <param name="paymentDate">实际缴纳日期</param>
        /// <param name="dueDate">费款所属期（yyyyMM -> 次月起算）</param>
        /// <param name="contributionTypeName">补缴类型名称</param>
        double CalculateLateFee(double principal, DateTime paymentDate, DateTime dueDate, string contributionTypeName);
    }
}
