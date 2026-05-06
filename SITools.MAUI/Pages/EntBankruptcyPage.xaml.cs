using System.Collections.ObjectModel;
using SITools.BLL.Services;
using SITools.Models.Entities;
using SITools.MAUI.ViewModels;

namespace SITools.MAUI.Pages;

/// <summary>
/// 企业破产清算计算页面（Android版）。
/// 功能：手动录入欠费记录，选择计算选项，执行计算后跳转结果页。
/// （Windows版支持从Excel导入，Android版仅支持手动录入）
/// </summary>
public partial class EntBankruptcyPage : ContentPage
{
    private readonly IInterestCalculationService _interestSvc;
    private readonly ILateFeeCalculationService _lateFeeSvc;
    private readonly ObservableCollection<BankruptcyRecordViewModel> _recordVMs = new();

    public EntBankruptcyPage(
        IInterestCalculationService interestSvc,
        ILateFeeCalculationService lateFeeSvc)
    {
        _interestSvc = interestSvc;
        _lateFeeSvc = lateFeeSvc;
        InitializeComponent();

        cvRecords.ItemsSource = _recordVMs;
        dpInterestEnd.Date = DateTime.Today;
        dpLateFeeEnd.Date = DateTime.Today;
    }

    /// <summary>
    /// "添加记录"按钮：验证输入并将欠费记录加入列表。
    /// </summary>
    private void OnAddRecord(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(entryName.Text))
        { ShowError("请输入姓名！"); return; }
        if (string.IsNullOrWhiteSpace(entryIdCard.Text))
        { ShowError("请输入身份证号码！"); return; }
        if (string.IsNullOrWhiteSpace(entryInsType.Text))
        { ShowError("请输入险种类型！"); return; }
        if (!int.TryParse(entryPeriod.Text, out int period) || period < 100001 || period > 999912)
        { ShowError("费款所属期格式错误，请输入6位期号（如202001）！"); return; }
        if (!double.TryParse(entryBase.Text, out double baseAmount) || baseAmount < 0)
        { ShowError("月缴费基数不能为负数！"); return; }
        if (!double.TryParse(entryUnit.Text, out double unitAmount) || unitAmount < 0)
        { ShowError("单位应缴金额不能为负数！"); return; }
        if (!double.TryParse(entryPersonal.Text, out double personalAmount) || personalAmount < 0)
        { ShowError("个人应缴金额不能为负数！"); return; }

        var record = new BankruptcyRecord
        {
            Name = entryName.Text.Trim(),
            IdCardNo = entryIdCard.Text.Trim(),
            InsuranceTypeName = entryInsType.Text.Trim(),
            Period = entryPeriod.Text.Trim(),
            MonthlyBase = baseAmount,
            UnitAmount = unitAmount,
            PersonalAmount = personalAmount,
        };

        _recordVMs.Add(new BankruptcyRecordViewModel(record));
        UpdateRecordCount();
        ClearInputFields();
    }

    /// <summary>
    /// "删除"按钮：从列表中移除对应记录。
    /// </summary>
    private void OnDeleteRecord(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is BankruptcyRecordViewModel vm)
        {
            _recordVMs.Remove(vm);
            UpdateRecordCount();
        }
    }

    /// <summary>
    /// "计算"按钮：执行计算后跳转到结果页面。
    /// </summary>
    private async void OnCalculate(object sender, EventArgs e)
    {
        if (_recordVMs.Count == 0)
        { ShowError("请先添加至少一条欠费记录！"); return; }
        if (pickerCalcInterest.SelectedIndex < 0)
        { ShowError("请选择是否计算利息！"); return; }
        if (pickerCalcLateFee.SelectedIndex < 0)
        { ShowError("请选择是否计算滞纳金！"); return; }

        bool calcInterest = pickerCalcInterest.SelectedIndex == 0;
        bool calcLateFee = pickerCalcLateFee.SelectedIndex == 0;
        DateTime interestEndDate = dpInterestEnd.Date ?? DateTime.Today;
        DateTime lateFeeEndDate = dpLateFeeEnd.Date ?? DateTime.Today;

        var records = _recordVMs.Select(vm => vm.Record).ToList();

        try
        {
            await Navigation.PushAsync(
                new EntBankruptcyResultPage(records, _interestSvc, _lateFeeSvc,
                    calcInterest, interestEndDate, calcLateFee, lateFeeEndDate));
        }
        catch (Exception ex)
        {
            ShowError($"计算失败：{ex.Message}");
        }
    }

    /// <summary>
    /// "重置"按钮：清空所有输入和记录。
    /// </summary>
    private void OnReset(object sender, EventArgs e)
    {
        _recordVMs.Clear();
        UpdateRecordCount();
        ClearInputFields();
        pickerCalcInterest.SelectedIndex = -1;
        pickerCalcLateFee.SelectedIndex = -1;
        dpInterestEnd.Date = DateTime.Today;
        dpLateFeeEnd.Date = DateTime.Today;
    }

    private void ClearInputFields()
    {
        entryName.Text = string.Empty;
        entryIdCard.Text = string.Empty;
        entryInsType.Text = string.Empty;
        entryPeriod.Text = string.Empty;
        entryBase.Text = string.Empty;
        entryUnit.Text = string.Empty;
        entryPersonal.Text = string.Empty;
    }

    private void UpdateRecordCount()
    {
        labelRecordCount.Text = $"已添加记录：{_recordVMs.Count}条";
    }

    private void ShowError(string message)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
            await DisplayAlert("输入错误", message, "确定"));
    }
}
