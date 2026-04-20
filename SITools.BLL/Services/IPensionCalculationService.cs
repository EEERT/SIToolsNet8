using SITools.Models.Entities;

namespace SITools.BLL.Services
{
    /// <summary>
    /// 养老保险补缴计算服务接口
    /// </summary>
    public interface IPensionCalculationService
    {
        /// <summary>
        /// 计算单位部分本金
        /// </summary>
        double CalculateUnitPrincipal(double baseAmount, ContributionType contributionType, int period);

        /// <summary>
        /// 计算个人部分本金
        /// </summary>
        double CalculatePersonalPrincipal(double baseAmount, ContributionType contributionType, int period);

        /// <summary>
        /// 对基数应用上下限约束（period 格式为 yyyyMM，用于处理同年不同月存在不同基数标准的情况）
        /// </summary>
        double ApplyBaseLimit(double baseAmount, int period);

        /// <summary>
        /// 将补缴类型名称转换为枚举
        /// </summary>
        ContributionType ParseContributionType(string name);

        /// <summary>
        /// 获取补缴类型显示名称列表
        /// </summary>
        List<string> GetContributionTypeNames();
    }
}
