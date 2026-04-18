namespace SITools.Models.Entities
{
    /// <summary>
    /// 逐月计算结果明细行
    /// </summary>
    public class CalculationResultDetail
    {
        /// <summary>姓名</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>身份证号码</summary>
        public string IdCardNo { get; set; } = string.Empty;
        /// <summary>费款所属期（yyyyMM）</summary>
        public string Period { get; set; } = string.Empty;
        /// <summary>月缴费基数</summary>
        public double MonthlyBase { get; set; }
        /// <summary>补缴类型名称</summary>
        public string ContributionTypeName { get; set; } = string.Empty;
        /// <summary>统筹部分本金（单位）</summary>
        public double UnitPrincipal { get; set; }
        /// <summary>统筹部分利息</summary>
        public double UnitInterest { get; set; }
        /// <summary>个人部分本金</summary>
        public double PersonalPrincipal { get; set; }
        /// <summary>个人部分利息</summary>
        public double PersonalInterest { get; set; }
        /// <summary>滞纳金</summary>
        public double LateFee { get; set; }
        /// <summary>合计</summary>
        public double Total { get; set; }
    }

    /// <summary>
    /// 按人员汇总的计算结果
    /// </summary>
    public class CalculationResultSummary
    {
        /// <summary>姓名</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>身份证号码</summary>
        public string IdCardNo { get; set; } = string.Empty;
        /// <summary>费款起止期号（如 198601-202412）</summary>
        public string PeriodRange { get; set; } = string.Empty;
        /// <summary>基数总额</summary>
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
        /// <summary>合计</summary>
        public double Total { get; set; }
    }
}
