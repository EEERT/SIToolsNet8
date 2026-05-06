using SITools.Models.Entities;

namespace SITools.MAUI.Pages;

/// <summary>
/// 企业养老补缴计算结果页面。
/// 显示逐月明细（Detail）和人员汇总（Summary）两个视图，可通过顶部按钮切换。
/// </summary>
public partial class EntPensionResultPage : ContentPage
{
    public EntPensionResultPage(
        List<CalculationResultDetail> details,
        List<CalculationResultSummary> summaries)
    {
        InitializeComponent();

        cvDetail.ItemsSource = details.Select(d => new
        {
            HeaderText = $"{d.Name}  {d.IdCardNo}  {d.Period}  {d.ContributionTypeName}",
            DetailText = $"基数：{d.MonthlyBase:F0}  单位本金：{d.UnitPrincipal:F2}  利息：{d.UnitInterest:F2}\n个人本金：{d.PersonalPrincipal:F2}  利息：{d.PersonalInterest:F2}  滞纳金：{d.LateFee:F2}\n合计：{d.Total:F2}元",
        }).ToList();

        cvSummary.ItemsSource = summaries.Select(s => new
        {
            HeaderText = $"{s.Name}  {s.IdCardNo}  {s.PeriodRange}",
            DetailText = $"基数合计：{s.TotalBase:F0}  单位本金：{s.UnitPrincipal:F2}  利息：{s.UnitInterest:F2}\n个人本金：{s.PersonalPrincipal:F2}  利息：{s.PersonalInterest:F2}  滞纳金：{s.LateFee:F2}\n合计：{s.Total:F2}元",
        }).ToList();
    }

    private void OnShowDetail(object sender, EventArgs e)
    {
        cvDetail.IsVisible = true;
        cvSummary.IsVisible = false;
        btnTabDetail.BackgroundColor = Color.FromArgb("#1565C0");
        btnTabSummary.BackgroundColor = Color.FromArgb("#0D47A1");
    }

    private void OnShowSummary(object sender, EventArgs e)
    {
        cvDetail.IsVisible = false;
        cvSummary.IsVisible = true;
        btnTabDetail.BackgroundColor = Color.FromArgb("#0D47A1");
        btnTabSummary.BackgroundColor = Color.FromArgb("#1565C0");
    }
}
