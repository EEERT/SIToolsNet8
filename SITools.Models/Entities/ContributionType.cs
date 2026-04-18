namespace SITools.Models.Entities
{
    /// <summary>
    /// 社会保险费补缴类型枚举
    /// </summary>
    public enum ContributionType
    {
        /// <summary>职工158号文补缴</summary>
        Doc158 = 158,
        /// <summary>职工补中断</summary>
        SupplementInterruption = 159,
        /// <summary>历史陈欠清算</summary>
        HistoricalDebt = 160,
        /// <summary>个体缴费</summary>
        Individual = 161,
        /// <summary>62号文件补缴（合同工）</summary>
        Doc62ContractWorker = 1620,
        /// <summary>62号文件补缴（固定工）</summary>
        Doc62RegularWorker = 1621,
    }

    /// <summary>
    /// 险种类型
    /// </summary>
    public enum InsuranceType
    {
        /// <summary>企业职工基本养老保险</summary>
        EnterprisePension,
        /// <summary>机关养老保险</summary>
        CivilServicePension,
        /// <summary>工伤保险</summary>
        WorkInjury,
        /// <summary>失业保险</summary>
        Unemployment,
        /// <summary>欠费清算</summary>
        DebtClearance,
    }
}
