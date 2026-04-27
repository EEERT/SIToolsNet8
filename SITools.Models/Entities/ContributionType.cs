namespace SITools.Models.Entities
{
    /// <summary>
    /// 社会保险费补缴类型枚举。
    /// 枚举（enum）是一种特殊的数据类型，用于给一组有限的常量取一个有意义的名字。
    /// 这里列出了本系统支持的所有社保补缴业务类型，每种类型对应不同的缴费比例和计算规则。
    /// 枚举值（等号后的数字）是系统内部编号，不影响业务含义。
    /// </summary>
    public enum ContributionType
    {
        /// <summary>
        /// 职工158号文补缴。
        /// 依据劳动部158号文件，用于补缴1996年1月至2011年6月期间的职工社保费。
        /// 单位缴费比例固定为20%，个人缴费比例固定为8%。
        /// </summary>
        Doc158 = 158,

        /// <summary>
        /// 职工补中断。
        /// 用于补缴因中断缴费而欠下的社保费（2011年7月起适用）。
        /// 单位缴费比例随政策阶段不同而变化（20%→19%→16%），个人固定为8%。
        /// </summary>
        SupplementInterruption = 159,

        /// <summary>
        /// 历史陈欠清算。
        /// 用于清算1986年至今历史上拖欠的社保费，
        /// 单位和个人缴费比例均随历史阶段不同而变化。
        /// </summary>
        HistoricalDebt = 160,

        /// <summary>
        /// 个体缴费。
        /// 适用于个体工商户、灵活就业人员补缴社保费，
        /// 单位部分12%，个人部分8%。
        /// </summary>
        Individual = 161,

        /// <summary>
        /// 62号文件补缴（合同工）。
        /// 依据62号文件，用于补缴合同制职工1986年1月至1995年12月的社保费。
        /// </summary>
        Doc62ContractWorker = 1620,

        /// <summary>
        /// 62号文件补缴（固定工）。
        /// 依据62号文件，用于补缴固定编制职工1986年1月至1995年12月的社保费。
        /// </summary>
        Doc62RegularWorker = 1621,
    }

    /// <summary>
    /// 险种类型枚举。
    /// 社会保险包括多个险种，本系统目前主要支持企业职工基本养老保险和破产清算场景，
    /// 其余险种供后续扩展使用。
    /// </summary>
    public enum InsuranceType
    {
        /// <summary>企业职工基本养老保险（最常见的险种）</summary>
        EnterprisePension,
        /// <summary>机关事业单位养老保险</summary>
        CivilServicePension,
        /// <summary>工伤保险</summary>
        WorkInjury,
        /// <summary>失业保险</summary>
        Unemployment,
        /// <summary>欠费清算（用于破产清算场景）</summary>
        DebtClearance,
    }
}
