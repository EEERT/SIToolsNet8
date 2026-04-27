using SITools.Models.Entities;

namespace SITools.DAL.Repositories
{
    /// <summary>
    /// 社保记账利率仓储实现（数据存储在内存中，来自国家历年政策文件）。
    /// 利率字典存储了1986年至今每个年度的社保个人账户记账利率。
    /// 这是国家人社部每年公布的利率，与银行存款利率不同，
    /// 专门用于计算养老保险个人账户及补缴欠费的利息。
    /// </summary>
    public class InterestRateRepository : IInterestRateRepository
    {
        // ======================================================================
        // 历年社保记账利率表（1986年至2026年）
        // 键（Key）= 年份，值（Value）= 当年利率（小数形式，如 0.0297 = 2.97%）
        // 数据来源：历年国家社会保险个人账户记账利率政策文件。
        // ======================================================================
        private Dictionary<int, double> _interestRates = new Dictionary<int, double>
        {
            {1986, 0.0120}, {1987, 0.0220}, {1988, 0.0364}, {1989, 0.1443},
            {1990, 0.1108}, {1991, 0.0256}, {1992, 0.0356}, {1993, 0.1498},
            {1994, 0.1198}, {1995, 0.1298}, {1996, 0.0347}, {1997, 0.0467},
            {1998, 0.0177}, {1999, 0.0200}, {2000, 0.0325}, {2001, 0.0425},
            {2002, 0.0298}, {2003, 0.0398}, {2004, 0.0425}, {2005, 0.0525},
            {2006, 0.0122}, {2007, 0.0214}, {2008, 0.0325}, {2009, 0.0425},
            {2010, 0.0115}, {2011, 0.0250}, {2012, 0.0500}, {2013, 0.0825},
            {2014, 0.0100}, {2015, 0.0250}, {2016, 0.0331}, {2017, 0.0412},
            {2018, 0.0129}, {2019, 0.0261}, {2020, 0.0304}, {2021, 0.0469},
            {2022, 0.0112}, {2023, 0.0297}, {2024, 0.0362}, {2025, 0.0750},
            {2026, 0.0850}
        };

        /// <summary>
        /// 返回所有利率数据的副本（避免外部直接修改内部字典）。
        /// "new Dictionary(...)" 创建了一个与原字典内容相同但独立的新字典副本。
        /// </summary>
        public Dictionary<int, double> GetAllRates()
        {
            return new Dictionary<int, double>(_interestRates);
        }

        /// <summary>
        /// 获取指定年份的利率。
        /// 如果该年份没有数据，抛出 KeyNotFoundException 异常，提示调用方数据缺失。
        /// </summary>
        public double GetRate(int year)
        {
            if (_interestRates.TryGetValue(year, out double rate))
                return rate;
            throw new KeyNotFoundException($"未找到{year}年度的利率数据。");
        }

        /// <summary>
        /// 用新的利率数据替换当前内存中的所有利率。
        /// 在"设置"界面用户修改并保存利率后，调用此方法更新内存数据。
        /// </summary>
        public void UpdateRates(Dictionary<int, double> rates)
        {
            // 创建新字典副本替换原有数据，防止外部传入的字典被修改后影响内部状态
            _interestRates = new Dictionary<int, double>(rates);
        }

        /// <summary>
        /// 将利率字典转换为按年份升序排列的 RateData 列表，供界面绑定展示。
        /// OrderBy 按年份从小到大排序，Select 将每个键值对转换为 RateData 对象，
        /// ToList 将结果收集为列表。
        /// </summary>
        public List<RateData> GetRateDataList()
        {
            return _interestRates
                .OrderBy(kv => kv.Key)           // 按年份升序排列
                .Select(kv => new RateData(kv.Key, kv.Value))  // 转换为 RateData 对象
                .ToList();                        // 收集为列表
        }
    }
}
