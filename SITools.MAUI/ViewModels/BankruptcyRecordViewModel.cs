using SITools.Models.Entities;

namespace SITools.MAUI.ViewModels
{
    /// <summary>
    /// 破产清算记录视图模型，包装 BankruptcyRecord，添加用于列表展示的文本属性。
    /// </summary>
    public class BankruptcyRecordViewModel
    {
        public BankruptcyRecord Record { get; }
        public string DisplayText { get; }

        public BankruptcyRecordViewModel(BankruptcyRecord record)
        {
            Record = record;
            DisplayText = $"{record.Name}  |  {record.IdCardNo}\n险种：{record.InsuranceTypeName}  期号：{record.Period}\n基数：{record.MonthlyBase:F0}  单位：{record.UnitAmount:F2}  个人：{record.PersonalAmount:F2}";
        }
    }
}
