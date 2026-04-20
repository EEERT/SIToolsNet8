namespace SITools.DAL.Repositories
{
    /// <summary>
    /// 养老保险缴费基数上下限仓储接口
    /// </summary>
    public interface IContributionBaseRepository
    {
        /// <summary>获取指定年月缴费基数下限（对于存在分段标准的年份，按月返回对应值）</summary>
        double GetMinBase(int year, int month);
        /// <summary>获取指定年月缴费基数上限（对于存在分段标准的年份，按月返回对应值）</summary>
        double GetMaxBase(int year, int month);
        /// <summary>判断指定年度是否有基数数据</summary>
        bool ContainsYear(int year);
    }
}
