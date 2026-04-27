namespace SITools.DAL.Repositories
{
    /// <summary>
    /// 养老保险缴费基数上下限仓储接口。
    /// "接口"（interface）是一份"契约"，规定了实现它的类必须提供哪些功能，
    /// 但不规定具体怎么实现。这样做的好处是：将来可以替换不同的数据来源（如数据库、文件等），
    /// 而上层业务代码不需要任何改动。
    /// 本接口定义了获取历年养老保险缴费基数上下限的三个方法。
    /// </summary>
    public interface IContributionBaseRepository
    {
        /// <summary>
        /// 获取指定年月的缴费基数下限（即"最低基数"，单位：元）。
        /// 参保人的实际缴费基数不得低于此值。
        /// 对于2019年等存在年度内基数调整的年份，会按月份返回对应的标准。
        /// </summary>
        /// <param name="year">年份（如 2023）</param>
        /// <param name="month">月份（1-12）</param>
        /// <returns>该年月对应的缴费基数下限（元）</returns>
        double GetMinBase(int year, int month);

        /// <summary>
        /// 获取指定年月的缴费基数上限（即"最高基数"，单位：元）。
        /// 参保人的实际缴费基数不得高于此值。
        /// 对于2019年等存在年度内基数调整的年份，会按月份返回对应的标准。
        /// </summary>
        /// <param name="year">年份（如 2023）</param>
        /// <param name="month">月份（1-12）</param>
        /// <returns>该年月对应的缴费基数上限（元）</returns>
        double GetMaxBase(int year, int month);

        /// <summary>
        /// 判断指定年度是否有基数数据。
        /// 如果某年份没有数据，说明系统尚不支持该年份的基数约束，
        /// 此时应直接使用用户输入的原始基数，不做上下限约束。
        /// </summary>
        /// <param name="year">要查询的年份</param>
        /// <returns>true 表示有数据；false 表示无数据</returns>
        bool ContainsYear(int year);
    }
}
