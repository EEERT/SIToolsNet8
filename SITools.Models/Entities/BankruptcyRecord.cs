namespace SITools.Models.Entities
{
    /// <summary>
    /// 企业破产清算中的数据行（从Excel导入）
    /// </summary>
    public class BankruptcyRecord
    {
        /// <summary>姓名</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>身份证号码</summary>
        public string IdCardNo { get; set; } = string.Empty;
        /// <summary>险种类型</summary>
        public string InsuranceTypeName { get; set; } = string.Empty;
        /// <summary>费款所属期（yyyyMM）</summary>
        public string Period { get; set; } = string.Empty;
        /// <summary>月缴费基数</summary>
        public double MonthlyBase { get; set; }
        /// <summary>单位应缴金额</summary>
        public double UnitAmount { get; set; }
        /// <summary>个人应缴金额</summary>
        public double PersonalAmount { get; set; }
    }

    /// <summary>
    /// 企业破产清算按险种汇总结果
    /// </summary>
    public class BankruptcySummary
    {
        /// <summary>姓名</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>身份证号码</summary>
        public string IdCardNo { get; set; } = string.Empty;
        /// <summary>险种类型</summary>
        public string InsuranceTypeName { get; set; } = string.Empty;
        /// <summary>费款起止期号</summary>
        public string PeriodRange { get; set; } = string.Empty;
        /// <summary>基数总额</summary>
        public double TotalBase { get; set; }
        /// <summary>统筹部分本金</summary>
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
}
