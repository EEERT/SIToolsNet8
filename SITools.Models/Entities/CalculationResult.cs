namespace SITools.Models.Entities
{
    /// <summary>
    /// 逐月计算结果明细行。
    /// 这是系统计算完成后，每一个月份生成的一行输出数据。
    /// 例如补缴 2020年1月至2020年3月，就会产生3条明细行。
    /// </summary>
    public class CalculationResultDetail
    {
        /// <summary>姓名（来自输入数据，直接显示）</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>身份证号码（来自输入数据）</summary>
        public string IdCardNo { get; set; } = string.Empty;

        /// <summary>
        /// 费款所属期，格式为 yyyyMM（如 202001 表示2020年1月）。
        /// 表示这一行数据对应的补缴月份。
        /// </summary>
        public string Period { get; set; } = string.Empty;

        /// <summary>
        /// 月缴费基数（元）。
        /// 经过上下限约束后的实际基数（四舍五入到元）。
        /// </summary>
        public double MonthlyBase { get; set; }

        /// <summary>补缴类型名称（如"职工158号文补缴"）</summary>
        public string ContributionTypeName { get; set; } = string.Empty;

        /// <summary>
        /// 统筹部分本金（单位缴费金额，元，保留2位小数）。
        /// 统筹部分是指由单位（用人单位）承担的那部分社保费，用于社会统筹基金。
        /// 计算方式：月缴费基数 × 单位缴费比例。
        /// </summary>
        public double UnitPrincipal { get; set; }

        /// <summary>
        /// 统筹部分利息（元，保留2位小数）。
        /// 对单位部分本金按历年社保记账利率计算的复利利息。
        /// </summary>
        public double UnitInterest { get; set; }

        /// <summary>
        /// 个人部分本金（元，保留2位小数）。
        /// 个人部分是指由参保人个人承担的那部分社保费，记入个人账户。
        /// 计算方式：月缴费基数 × 个人缴费比例。
        /// </summary>
        public double PersonalPrincipal { get; set; }

        /// <summary>
        /// 个人部分利息（元，保留2位小数）。
        /// 对个人部分本金按历年社保记账利率计算的复利利息。
        /// </summary>
        public double PersonalInterest { get; set; }

        /// <summary>
        /// 滞纳金（元，保留2位小数）。
        /// 按日0.05%对（单位+个人）本金计算，从逾期次月起算，上限不超过本金总额。
        /// </summary>
        public double LateFee { get; set; }

        /// <summary>
        /// 合计（元，保留2位小数）。
        /// = 单位本金 + 单位利息 + 个人本金 + 个人利息 + 滞纳金。
        /// </summary>
        public double Total { get; set; }
    }

    /// <summary>
    /// 按人员汇总的计算结果。
    /// 将同一身份证号的所有逐月明细行加总，生成一行汇总数据。
    /// 通常用于最终报表的汇总展示。
    /// </summary>
    public class CalculationResultSummary
    {
        /// <summary>姓名</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>身份证号码（汇总分组的依据）</summary>
        public string IdCardNo { get; set; } = string.Empty;

        /// <summary>
        /// 费款起止期号（如 198601-202412），表示此汇总行涵盖的补缴月份范围。
        /// </summary>
        public string PeriodRange { get; set; } = string.Empty;

        /// <summary>基数总额（各月月缴费基数之和）</summary>
        public double TotalBase { get; set; }

        /// <summary>统筹部分本金合计</summary>
        public double UnitPrincipal { get; set; }

        /// <summary>统筹部分利息合计</summary>
        public double UnitInterest { get; set; }

        /// <summary>个人部分本金合计</summary>
        public double PersonalPrincipal { get; set; }

        /// <summary>个人部分利息合计</summary>
        public double PersonalInterest { get; set; }

        /// <summary>滞纳金合计</summary>
        public double LateFee { get; set; }

        /// <summary>合计（所有项目之和）</summary>
        public double Total { get; set; }
    }
}
