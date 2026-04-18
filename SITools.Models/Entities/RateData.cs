namespace SITools.Models.Entities
{
    /// <summary>
    /// 年度利率数据
    /// </summary>
    public class RateData
    {
        /// <summary>年度</summary>
        public int Year { get; set; }
        /// <summary>社保记账利率</summary>
        public double Rate { get; set; }

        public RateData(int year, double rate)
        {
            Year = year;
            Rate = rate;
        }
    }
}
