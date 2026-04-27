namespace SITools.Models.Entities
{
    /// <summary>
    /// 补缴记录（输入数据）。
    /// 这是用户在界面上填写后、系统读取进行计算的数据模型。
    /// 每一条补缴记录代表一个人在某段时间内、以某种缴费类型进行补缴的情况。
    /// </summary>
    public class ContributionRecord
    {
        /// <summary>
        /// 姓名。
        /// 仅用于输出报表显示，不参与计算。
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 身份证号码（18位）。
        /// 用于标识唯一的参保人员，也是汇总计算时的分组依据。
        /// </summary>
        public string IdCardNo { get; set; } = string.Empty;

        /// <summary>
        /// 补缴开始时间，格式为 yyyyMM（如 198601 表示1986年1月）。
        /// 与 EndPeriod 一起确定本条记录需要补缴的月份范围。
        /// </summary>
        public int BeginPeriod { get; set; }

        /// <summary>
        /// 补缴结束时间，格式为 yyyyMM（如 202412 表示2024年12月）。
        /// 系统会从 BeginPeriod 逐月循环到 EndPeriod，每月分别计算本金、利息和滞纳金。
        /// </summary>
        public int EndPeriod { get; set; }

        /// <summary>
        /// 月缴费基数（单位：元）。
        /// 这是计算每月应缴本金的基础金额。如果 ApplyBaseLimit 为 true，
        /// 系统会将此值约束在该年度政策规定的上下限范围内。
        /// </summary>
        public double MonthlyBase { get; set; }

        /// <summary>
        /// 补缴类型（见 ContributionType 枚举）。
        /// 不同的补缴类型对应不同的单位和个人缴费比例。
        /// </summary>
        public ContributionType ContributionType { get; set; }

        /// <summary>
        /// 是否按上下限校正基数。
        /// 如果为 true，系统会自动将月缴费基数调整到该年月政策规定的最低或最高上下限范围内；
        /// 如果为 false，则直接使用用户输入的原始基数进行计算。
        /// </summary>
        public bool ApplyBaseLimit { get; set; }
    }
}
