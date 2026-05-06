using System.Collections.ObjectModel;
using SITools.BLL.Services;
using SITools.Models.Entities;
using SITools.MAUI.ViewModels;

namespace SITools.MAUI.Pages;

/// <summary>
/// 企业养老保险补缴计算页面（Android版）。
/// 功能：手动录入补缴记录，选择计算选项，执行计算后跳转结果页。
/// </summary>
public partial class EntPensionPage : ContentPage
{
    private readonly IPensionCalculationService _pensionSvc;
    private readonly EntPensionCalculationFacade _calcFacade;
    private readonly ObservableCollection<ContributionRecordViewModel> _recordVMs = new();

    public EntPensionPage(IPensionCalculationService pensionSvc, EntPensionCalculationFacade calcFacade)
    {
        _pensionSvc = pensionSvc;
        _calcFacade = calcFacade;
        InitializeComponent();

        // 初始化补缴类型下拉框
        var typeNames = _pensionSvc.GetContributionTypeNames();
        foreach (var name in typeNames)
            pickerType.Items.Add(name);

        cvRecords.ItemsSource = _recordVMs;
        dpInterestEnd.Date = DateTime.Today;
        dpLateFeeEnd.Date = DateTime.Today;
    }

    /// <summary>
    /// "添加记录"按钮：验证输入并将记录加入列表。
    /// </summary>
    private void OnAddRecord(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(entryName.Text))
        { ShowError("请输入姓名！"); return; }
        if (string.IsNullOrWhiteSpace(entryIdCard.Text))
        { ShowError("请输入身份证号码！"); return; }
        if (pickerType.SelectedIndex < 0)
        { ShowError("请选择补缴类型！"); return; }
        if (!int.TryParse(entryBegin.Text, out int begin) || begin < 100001 || begin > 999912)
        { ShowError("补缴起始期格式错误，请输入6位期号（如198601）！"); return; }
        if (!int.TryParse(entryEnd.Text, out int end) || end < 100001 || end > 999912)
        { ShowError("补缴截止期格式错误，请输入6位期号（如202412）！"); return; }
        if (begin > end)
        { ShowError("起始期不能晚于截止期！"); return; }
        if (!double.TryParse(entryBase.Text, out double baseAmount) || baseAmount <= 0)
        { ShowError("月缴费基数必须为正数！"); return; }
        if (pickerApplyLimit.SelectedIndex < 0)
        { ShowError("请选择是否按基数上下限保底！"); return; }

        string typeName = pickerType.Items[pickerType.SelectedIndex];
        var contributionType = _pensionSvc.ParseContributionType(typeName);
        bool applyLimit = pickerApplyLimit.SelectedIndex == 0;

        var record = new ContributionRecord
        {
            Name = entryName.Text.Trim(),
            IdCardNo = entryIdCard.Text.Trim(),
            BeginPeriod = begin,
            EndPeriod = end,
            MonthlyBase = baseAmount,
            ContributionType = contributionType,
            ApplyBaseLimit = applyLimit,
        };

        _recordVMs.Add(new ContributionRecordViewModel(record, typeName));
        UpdateRecordCount();
        ClearInputFields();
    }

    /// <summary>
    /// "删除"按钮：从列表中移除对应记录。
    /// </summary>
    private void OnDeleteRecord(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is ContributionRecordViewModel vm)
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
        { ShowError("请先添加至少一条补缴记录！"); return; }
        if (pickerCalcInterest.SelectedIndex < 0)
        { ShowError("请选择是否计算利息！"); return; }
        if (pickerCalcLateFee.SelectedIndex < 0)
        { ShowError("请选择是否计算滞纳金！"); return; }

        bool calcInterest = pickerCalcInterest.SelectedIndex == 0;
        bool calcLateFee = pickerCalcLateFee.SelectedIndex == 0;
        DateTime interestEndDate = dpInterestEnd.Date ?? DateTime.Today;
        DateTime lateFeeEndDate = dpLateFeeEnd.Date ?? DateTime.Today;

        var records = _recordVMs.Select(vm => vm.Record).ToList();

        List<CalculationResultDetail> details;
        List<CalculationResultSummary> summaries;
        try
        {
            details = _calcFacade.Calculate(records, calcInterest, interestEndDate, calcLateFee, lateFeeEndDate);
            summaries = _calcFacade.Summarize(details);
        }
        catch (Exception ex)
        {
            ShowError($"计算失败：{ex.Message}");
            return;
        }

        await Navigation.PushAsync(new EntPensionResultPage(details, summaries));
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
        entryBegin.Text = string.Empty;
        entryEnd.Text = string.Empty;
        entryBase.Text = string.Empty;
        pickerType.SelectedIndex = -1;
        pickerApplyLimit.SelectedIndex = -1;
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
