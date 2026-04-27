using SITools.Models.Entities;
using SITools.DAL.Repositories;

namespace SITools.BLL.Services
{
    /// <summary>
    /// 养老保险补缴计算服务实现。
    /// 本类负责根据不同的补缴类型和费款所属期，
    /// 查找对应的单位和个人缴费比例，并计算应缴本金。
    /// 缴费比例依据历年政策文件内置于代码中。
    /// </summary>
    public class PensionCalculationService : IPensionCalculationService
    {
        // 依赖的缴费基数上下限仓储（通过构造函数注入，遵循依赖倒置原则）
        private readonly IContributionBaseRepository _baseRepo;

        // 补缴类型中文名称与枚举值的映射字典，供下拉框展示和反向解析使用
        private static readonly Dictionary<string, ContributionType> _nameToType = new()
        {
            { "职工158号文补缴",       ContributionType.Doc158 },
            { "职工补中断",             ContributionType.SupplementInterruption },
            { "历史陈欠清算",           ContributionType.HistoricalDebt },
            { "个体缴费",               ContributionType.Individual },
            { "62号文件补缴（合同工）", ContributionType.Doc62ContractWorker },
            { "62号文件补缴（固定工）", ContributionType.Doc62RegularWorker },
        };

        /// <summary>
        /// 构造函数：通过参数接收缴费基数仓储，实现依赖注入。
        /// 依赖注入的好处是：测试时可以替换为模拟数据，便于单元测试。
        /// </summary>
        public PensionCalculationService(IContributionBaseRepository baseRepo)
        {
            _baseRepo = baseRepo;
        }

        /// <summary>
        /// 返回所有补缴类型的中文名称列表，供界面下拉框绑定。
        /// </summary>
        public List<string> GetContributionTypeNames()
        {
            return new List<string>(_nameToType.Keys);
        }

        /// <summary>
        /// 将中文名称（如"职工158号文补缴"）解析为对应的枚举值。
        /// 如果名称不在字典中，抛出 ArgumentException 异常。
        /// </summary>
        public ContributionType ParseContributionType(string name)
        {
            if (_nameToType.TryGetValue(name, out var type))
                return type;
            throw new ArgumentException($"未知的补缴类型：{name}");
        }

        /// <summary>
        /// 对月缴费基数应用政策规定的上下限约束。
        /// 步骤：
        ///   1. 从 period（如202001）解析年份和月份
        ///   2. 检查该年份是否有基数数据，没有则直接返回原值
        ///   3. 获取该年月的下限和上限
        ///   4. 如果基数低于下限，返回下限；高于上限，返回上限；否则返回原值
        /// </summary>
        public double ApplyBaseLimit(double baseAmount, int period)
        {
            int year = period / 100;   // 取 yyyyMM 的前4位：年份（如 202001 / 100 = 2020）
            int month = period % 100;  // 取 yyyyMM 的后2位：月份（如 202001 % 100 = 1）

            // 如果该年份没有基数数据，直接使用用户输入的原始值
            if (!_baseRepo.ContainsYear(year))
                return baseAmount;

            double min = _baseRepo.GetMinBase(year, month);
            double max = _baseRepo.GetMaxBase(year, month);

            // 低于下限则取下限（保底），高于上限则取上限（封顶）
            if (baseAmount < min) return min;
            if (baseAmount > max) return max;
            return baseAmount;
        }

        /// <summary>
        /// 计算单位部分本金：月缴费基数 × 单位缴费比例。
        /// 不同补缴类型、不同时间段的单位缴费比例不同，由 GetUnitContributionRate 方法查表返回。
        /// </summary>
        public double CalculateUnitPrincipal(double baseAmount, ContributionType contributionType, int period)
        {
            double rate = GetUnitContributionRate(contributionType, period);
            return baseAmount * rate;
        }

        /// <summary>
        /// 计算个人部分本金：月缴费基数 × 个人缴费比例。
        /// 特殊情况：1992年4月至12月（199204-199212），个人部分固定为2.5元，
        /// 不按比例计算（历史政策特殊规定）。
        /// </summary>
        public double CalculatePersonalPrincipal(double baseAmount, ContributionType contributionType, int period)
        {
            double rate = GetPersonalContributionRate(contributionType, period);

            // 特殊处理：199204-199212期间，个人部分固定为2.5元
            if (period >= 199204 && period <= 199212)
                return 2.5;

            return baseAmount * rate;
        }

        /// <summary>
        /// 根据补缴类型和费款所属期，查找单位缴费比例。
        /// 使用 switch 表达式（C# 8.0以上的新语法）实现多分支查表，逻辑清晰简洁。
        /// 说明：
        ///   - Doc158：固定20%，不随时间变化
        ///   - SupplementInterruption：随政策阶段变化（201107-201604: 20%，201605-201904: 19%，201905+: 16%）
        ///   - HistoricalDebt：按历史阶段分段（最早16%，最高23%，近年降至16%）
        ///   - Individual：固定12%（个体户单位和个人合并，只有一个12%的"单位"比例）
        ///   - Doc62ContractWorker/Doc62RegularWorker：仅适用于1986-1995年
        /// 若所属期不在任何已定义范围内，返回0.0（不计单位费用）。
        /// </summary>
        private static double GetUnitContributionRate(ContributionType type, int period)
        {
            return type switch
            {
                ContributionType.Doc158 => 0.2,  // 158号文单位固定20%

                ContributionType.SupplementInterruption => period switch
                {
                    >= 201107 and <= 201604 => 0.20,  // 2011年7月起，单位20%
                    >= 201605 and <= 201904 => 0.19,  // 2016年5月起，单位19%
                    >= 201905 => 0.16,                 // 2019年5月起，单位16%
                    _ => 0.0
                },

                ContributionType.HistoricalDebt => period switch
                {
                    >= 198601 and <= 199112 => 0.16,  // 1986-1991年，单位16%
                    >= 199201 and <= 199312 => 0.19,  // 1992-1993年，单位19%
                    >= 199401 and <= 199712 => 0.23,  // 1994-1997年，单位23%
                    >= 199801 and <= 200012 => 0.22,  // 1998-2000年，单位22%
                    >= 200101 and <= 200212 => 0.21,  // 2001-2002年，单位21%
                    >= 200301 and <= 201604 => 0.20,  // 2003-2016年4月，单位20%
                    >= 201605 and <= 201904 => 0.19,  // 2016年5月-2019年4月，单位19%
                    >= 201905 => 0.16,                 // 2019年5月起，单位16%
                    _ => 0.0
                },

                ContributionType.Individual => 0.12,  // 个体缴费单位比例固定12%

                ContributionType.Doc62ContractWorker => period switch
                {
                    >= 198601 and <= 199112 => 0.16,  // 1986-1991年，单位16%
                    >= 199201 and <= 199312 => 0.19,  // 1992-1993年，单位19%
                    >= 199401 and <= 199512 => 0.23,  // 1994-1995年，单位23%
                    _ => 0.0
                },

                ContributionType.Doc62RegularWorker => period switch
                {
                    >= 198601 and <= 199112 => 0.16,  // 1986-1991年，单位16%
                    >= 199201 and <= 199312 => 0.19,  // 1992-1993年，单位19%
                    >= 199401 and <= 199512 => 0.23,  // 1994-1995年，单位23%
                    _ => 0.0
                },

                _ => 0.0  // 未知类型返回0
            };
        }

        /// <summary>
        /// 根据补缴类型和费款所属期，查找个人缴费比例。
        /// 说明：
        ///   - Doc158/SupplementInterruption/Individual：个人固定8%
        ///   - HistoricalDebt：个人比例随历史阶段逐步提高（3%→8%）
        ///   - Doc62ContractWorker：个人固定3%
        ///   - Doc62RegularWorker：固定工个人比例较低（1986-1992: 3%，1993-1995: 2%）
        /// </summary>
        private static double GetPersonalContributionRate(ContributionType type, int period)
        {
            return type switch
            {
                ContributionType.Doc158 => 0.08,               // 158号文个人固定8%
                ContributionType.SupplementInterruption => 0.08, // 补中断个人固定8%

                ContributionType.HistoricalDebt => period switch
                {
                    >= 198601 and <= 199312 => 0.03,  // 1986-1993年，个人3%
                    >= 199401 and <= 199712 => 0.03,  // 1994-1997年，个人3%
                    >= 199801 and <= 199906 => 0.04,  // 1998年1月-1999年6月，个人4%
                    >= 199907 and <= 200012 => 0.05,  // 1999年7月-2000年12月，个人5%
                    >= 200101 and <= 200212 => 0.06,  // 2001-2002年，个人6%
                    >= 200301 => 0.08,                 // 2003年起，个人8%
                    _ => 0.0
                },

                ContributionType.Individual => 0.08,            // 个体缴费个人固定8%
                ContributionType.Doc62ContractWorker => 0.03,   // 合同工个人固定3%

                ContributionType.Doc62RegularWorker => period switch
                {
                    >= 198601 and <= 199212 => 0.03,  // 1986-1992年，固定工个人3%
                    >= 199301 and <= 199512 => 0.02,  // 1993-1995年，固定工个人2%
                    _ => 0.0
                },

                _ => 0.0  // 未知类型返回0
            };
        }
    }
}
