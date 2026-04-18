namespace SITools.DAL.Repositories
{
    /// <summary>
    /// 历年养老保险缴费基数上下限仓储实现
    /// </summary>
    public class ContributionBaseRepository : IContributionBaseRepository
    {
        // 历年企业养老保险缴费基数下限
        private static readonly Dictionary<int, double> _baseMin = new Dictionary<int, double>
        {
            {1986, 111}, {1987, 111}, {1988, 111}, {1989, 111}, {1990, 111},
            {1991, 121}, {1992, 113}, {1993, 124}, {1994, 145}, {1995, 222},
            {1996, 222}, {1997, 261}, {1998, 262}, {1999, 385}, {2000, 355},
            {2001, 363}, {2002, 451}, {2003, 524}, {2004, 510}, {2005, 714},
            {2006, 791}, {2007, 892}, {2008, 1023}, {2009, 1231}, {2010, 1444},
            {2011, 1678}, {2012, 1860}, {2013, 2121}, {2014, 2423}, {2015, 2626},
            {2016, 3026}, {2017, 3290}, {2018, 3582}, {2019, 3236}, {2020, 3463},
            {2021, 3726}, {2022, 4071}, {2023, 4246}, {2024, 4511}, {2025, 4588}
        };

        // 历年企业养老保险缴费基数上限
        private static readonly Dictionary<int, double> _baseMax = new Dictionary<int, double>
        {
            {1986, 458}, {1987, 458}, {1988, 467}, {1989, 479}, {1990, 492},
            {1991, 503}, {1992, 549}, {1993, 615}, {1994, 740}, {1995, 1007},
            {1996, 1161}, {1997, 1289}, {1998, 1407}, {1999, 1485}, {2000, 1619},
            {2001, 1861}, {2002, 2242}, {2003, 2567}, {2004, 2899}, {2005, 3516},
            {2006, 3957}, {2007, 4463}, {2008, 5328}, {2009, 6260}, {2010, 7141},
            {2011, 8278}, {2012, 9481}, {2013, 10778}, {2014, 12255}, {2015, 13431},
            {2016, 15130}, {2017, 16445}, {2018, 17908}, {2019, 16179}, {2020, 17317},
            {2021, 18630}, {2022, 20355}, {2023, 21228}, {2024, 22555}, {2025, 22938}
        };

        public double GetMinBase(int year)
        {
            if (_baseMin.TryGetValue(year, out double val))
                return val;
            throw new KeyNotFoundException($"未找到{year}年度的缴费基数下限数据。");
        }

        public double GetMaxBase(int year)
        {
            if (_baseMax.TryGetValue(year, out double val))
                return val;
            throw new KeyNotFoundException($"未找到{year}年度的缴费基数上限数据。");
        }

        public bool ContainsYear(int year)
        {
            return _baseMin.ContainsKey(year);
        }
    }
}
