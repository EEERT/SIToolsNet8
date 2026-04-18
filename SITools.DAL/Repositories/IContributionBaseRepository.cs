namespace SITools.DAL.Repositories
{
    /// <summary>
    /// 养老保险缴费基数上下限仓储接口
    /// </summary>
    public interface IContributionBaseRepository
    {
        /// <summary>获取指定年度缴费基数下限</summary>
        double GetMinBase(int year);
        /// <summary>获取指定年度缴费基数上限</summary>
        double GetMaxBase(int year);
        /// <summary>判断指定年度是否有基数数据</summary>
        bool ContainsYear(int year);
    }
}
