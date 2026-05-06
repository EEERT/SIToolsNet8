using SITools.Models.Entities;

namespace SITools.MAUI.ViewModels
{
    /// <summary>
    /// 补缴记录视图模型，包装 ContributionRecord，添加用于列表展示的文本属性。
    /// </summary>
    public class ContributionRecordViewModel
    {
        public ContributionRecord Record { get; }
        public string DisplayText { get; }

        public ContributionRecordViewModel(ContributionRecord record, string typeName)
        {
            Record = record;
            DisplayText = $"{record.Name}  |  {record.IdCardNo}\n类型：{typeName}  基数：{record.MonthlyBase:F0}元\n期号：{record.BeginPeriod} - {record.EndPeriod}  保底：{(record.ApplyBaseLimit ? "是" : "否")}";
        }
    }
}
