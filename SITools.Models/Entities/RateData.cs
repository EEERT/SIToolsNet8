namespace SITools.Models.Entities
{
    /// <summary>
    /// 年度利率数据。
    /// 该类用于在界面（DataGridView）中显示和编辑每一年度的社保记账利率。
    /// 社保记账利率由国家每年公布，用于计算历史欠费产生的利息。
    /// </summary>
    public class RateData
    {
        /// <summary>年度（如 2023 表示2023年度）</summary>
        public int Year { get; set; }

        /// <summary>
        /// 社保记账利率（小数形式，如 0.0297 表示 2.97%）。
        /// 这是国家公布的个人账户记账利率，不同于银行利率。
        /// </summary>
        public double Rate { get; set; }

        /// <summary>
        /// 构造函数：在创建 RateData 对象时，同时初始化年度和利率两个属性。
        /// 构造函数名与类名相同，在用 new RateData(year, rate) 创建对象时自动调用。
        /// </summary>
        /// <param name="year">年度</param>
        /// <param name="rate">利率（小数形式）</param>
        public RateData(int year, double rate)
        {
            Year = year;
            Rate = rate;
        }
    }
}
