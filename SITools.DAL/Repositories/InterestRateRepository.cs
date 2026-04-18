using SITools.Models.Entities;

namespace SITools.DAL.Repositories
{
    /// <summary>
    /// 社保记账利率仓储实现（内存数据，来自政策文件）
    /// </summary>
    public class InterestRateRepository : IInterestRateRepository
    {
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

        public Dictionary<int, double> GetAllRates()
        {
            return new Dictionary<int, double>(_interestRates);
        }

        public double GetRate(int year)
        {
            if (_interestRates.TryGetValue(year, out double rate))
                return rate;
            throw new KeyNotFoundException($"未找到{year}年度的利率数据。");
        }

        public void UpdateRates(Dictionary<int, double> rates)
        {
            _interestRates = new Dictionary<int, double>(rates);
        }

        public List<RateData> GetRateDataList()
        {
            return _interestRates
                .OrderBy(kv => kv.Key)
                .Select(kv => new RateData(kv.Key, kv.Value))
                .ToList();
        }
    }
}
