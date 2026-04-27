using SITools.Models.Entities;

namespace SITools.BLL.Services
{
    /// <summary>
    /// 养老保险补缴计算服务接口。
    /// 定义了计算补缴本金所需的所有方法，包括：
    ///   1. 计算单位（用人单位）应缴部分本金
    ///   2. 计算个人应缴部分本金
    ///   3. 对月缴费基数应用上下限约束
    ///   4. 补缴类型名称和枚举之间的转换
    /// </summary>
    public interface IPensionCalculationService
    {
        /// <summary>
        /// 计算单位部分本金（统筹部分）。
        /// 计算公式：月缴费基数 × 单位缴费比例
        /// 单位缴费比例因补缴类型和所属期不同而不同（详见 PensionCalculationService 实现）。
        /// </summary>
        /// <param name="baseAmount">月缴费基数（元）</param>
        /// <param name="contributionType">补缴类型枚举</param>
        /// <param name="period">费款所属期，格式 yyyyMM（如 202001）</param>
        /// <returns>单位部分本金（元）</returns>
        double CalculateUnitPrincipal(double baseAmount, ContributionType contributionType, int period);

        /// <summary>
        /// 计算个人部分本金（个人账户部分）。
        /// 计算公式：月缴费基数 × 个人缴费比例（特殊月份有固定金额，见实现类说明）。
        /// </summary>
        /// <param name="baseAmount">月缴费基数（元）</param>
        /// <param name="contributionType">补缴类型枚举</param>
        /// <param name="period">费款所属期，格式 yyyyMM</param>
        /// <returns>个人部分本金（元）</returns>
        double CalculatePersonalPrincipal(double baseAmount, ContributionType contributionType, int period);

        /// <summary>
        /// 对月缴费基数应用政策规定的上下限约束。
        /// 如果基数低于当年下限，则取下限；如果高于当年上限，则取上限；
        /// 如果在范围内，则原值返回。
        /// period 格式为 yyyyMM，这样可以正确处理2019年等年度内有多个标准的情况。
        /// </summary>
        /// <param name="baseAmount">用户输入的原始月缴费基数（元）</param>
        /// <param name="period">费款所属期，格式 yyyyMM</param>
        /// <returns>经上下限约束后的月缴费基数（元）</returns>
        double ApplyBaseLimit(double baseAmount, int period);

        /// <summary>
        /// 将补缴类型的中文名称（如"职工158号文补缴"）转换为对应的枚举值。
        /// 如果名称无法识别，抛出 ArgumentException 异常。
        /// </summary>
        /// <param name="name">补缴类型中文名称</param>
        /// <returns>对应的 ContributionType 枚举值</returns>
        ContributionType ParseContributionType(string name);

        /// <summary>
        /// 获取所有补缴类型的中文名称列表，用于在下拉框（ComboBox）中显示供用户选择。
        /// </summary>
        List<string> GetContributionTypeNames();
    }
}
