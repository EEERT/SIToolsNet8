namespace SITools.Models.Entities
{
    /// <summary>
    /// 补缴记录（输入数据）
    /// </summary>
    public class ContributionRecord
    {
        /// <summary>姓名</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>身份证号码</summary>
        public string IdCardNo { get; set; } = string.Empty;
        /// <summary>补缴开始时间（yyyyMM格式，如 198601）</summary>
        public int BeginPeriod { get; set; }
        /// <summary>补缴结束时间（yyyyMM格式，如 202412）</summary>
        public int EndPeriod { get; set; }
        /// <summary>月缴费基数</summary>
        public double MonthlyBase { get; set; }
        /// <summary>补缴类型</summary>
        public ContributionType ContributionType { get; set; }
        /// <summary>是否保底（按上下限校正基数）</summary>
        public bool ApplyBaseLimit { get; set; }
    }
}
