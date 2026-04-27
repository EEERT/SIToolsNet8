namespace SITools.BLL.Services
{
    /// <summary>
    /// 滞纳金计算服务接口。
    /// 滞纳金是对逾期未缴社保费的一种经济处罚，
    /// 按日0.05%（即万分之五）对欠缴本金总额（单位+个人）计算，
    /// 最高不超过本金总额。
    /// </summary>
    public interface ILateFeeCalculationService
    {
        /// <summary>
        /// 计算滞纳金。
        /// 计算公式：滞纳金 = 本金 × 逾期天数 × 0.05%
        /// 其中逾期天数 = 实际缴款日期 - 应缴款截止日期（含当天），最终结果上限为本金。
        /// 特殊规则：158号文补缴从2011年7月1日起算，而非按正常逾期次月。
        /// </summary>
        /// <param name="principal">本金（单位缴费金额 + 个人缴费金额，元）</param>
        /// <param name="paymentDate">实际缴纳日期（即用户指定的缴款截止日）</param>
        /// <param name="dueDate">费款所属月份对应的日期（逾期从次月开始算，见实现类说明）</param>
        /// <param name="contributionTypeName">补缴类型名称（用于判断是否适用158号文特殊规则）</param>
        /// <returns>应收滞纳金金额（元）</returns>
        double CalculateLateFee(double principal, DateTime paymentDate, DateTime dueDate, string contributionTypeName);
    }
}
