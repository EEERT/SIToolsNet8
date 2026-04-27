namespace SITools.Models.Entities
{
    /// <summary>
    /// 企业破产清算中的数据行（从Excel导入）。
    /// 本类用于存储从外部Excel表格中读取的每一行欠费记录。
    /// 破产清算场景下，企业欠缴的每一笔社保费都作为一条记录导入，
    /// 系统根据这些记录计算本金、利息和滞纳金。
    /// </summary>
    public class BankruptcyRecord
    {
        /// <summary>参保人姓名</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>参保人身份证号码（18位）</summary>
        public string IdCardNo { get; set; } = string.Empty;

        /// <summary>
        /// 险种类型名称（如"企业职工基本养老保险"）。
        /// 破产清算时可能涉及多个险种，利息只对养老保险计算。
        /// </summary>
        public string InsuranceTypeName { get; set; } = string.Empty;

        /// <summary>
        /// 费款所属期，格式为 yyyyMM（如 202001 表示2020年1月）。
        /// 表示这一行欠费对应的月份。
        /// </summary>
        public string Period { get; set; } = string.Empty;

        /// <summary>该月月缴费基数（元）</summary>
        public double MonthlyBase { get; set; }

        /// <summary>
        /// 单位应缴金额（本金，元）。
        /// 直接来自导入的Excel数据，不需要系统重新计算。
        /// </summary>
        public double UnitAmount { get; set; }

        /// <summary>
        /// 个人应缴金额（本金，元）。
        /// 直接来自导入的Excel数据，不需要系统重新计算。
        /// </summary>
        public double PersonalAmount { get; set; }
    }

    /// <summary>
    /// 企业破产清算按险种汇总结果。
    /// 将同一身份证号、同一险种的所有逐月明细加总，生成一行汇总数据，
    /// 方便查看每个人每个险种的欠缴总额。
    /// </summary>
    public class BankruptcySummary
    {
        /// <summary>参保人姓名</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>参保人身份证号码（汇总分组依据之一）</summary>
        public string IdCardNo { get; set; } = string.Empty;

        /// <summary>险种类型名称（汇总分组依据之二）</summary>
        public string InsuranceTypeName { get; set; } = string.Empty;

        /// <summary>费款起止期号（如 201901-202012，表示欠费时间跨度）</summary>
        public string PeriodRange { get; set; } = string.Empty;

        /// <summary>各月缴费基数之和（元）</summary>
        public double TotalBase { get; set; }

        /// <summary>单位部分本金合计（元）</summary>
        public double UnitPrincipal { get; set; }

        /// <summary>单位部分利息合计（元）</summary>
        public double UnitInterest { get; set; }

        /// <summary>个人部分本金合计（元）</summary>
        public double PersonalPrincipal { get; set; }

        /// <summary>个人部分利息合计（元）</summary>
        public double PersonalInterest { get; set; }

        /// <summary>滞纳金合计（元）</summary>
        public double LateFee { get; set; }

        /// <summary>合计（所有项目之和，元）</summary>
        public double Total { get; set; }
    }
}
