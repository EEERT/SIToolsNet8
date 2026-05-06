using SITools.BLL.Services;
using SITools.Models.Entities;

namespace SITools.MAUI.Pages;

/// <summary>
/// 企业破产清算计算结果页面。
/// 接收欠费记录和计算参数，执行计算并显示逐条明细与险种汇总两个视图。
/// </summary>
public partial class EntBankruptcyResultPage : ContentPage
{
    public EntBankruptcyResultPage(
        List<BankruptcyRecord> records,
        IInterestCalculationService interestSvc,
        ILateFeeCalculationService lateFeeSvc,
        bool calcInterest,
        DateTime interestEndDate,
        bool calcLateFee,
        DateTime lateFeeEndDate)
    {
        InitializeComponent();

        var details = CalculateDetails(records, interestSvc, lateFeeSvc,
            calcInterest, interestEndDate, calcLateFee, lateFeeEndDate);
        var summaries = BuildSummaries(details);

        cvDetail.ItemsSource = details.Select(d => new
        {
            HeaderText = $"{d.Name}  {d.IdCardNo}  {d.Period}  {d.InsuranceTypeName}",
            DetailText = $"基数：{d.MonthlyBase:F0}  单位本金：{d.UnitPrincipal:F2}  利息：{d.UnitInterest:F2}\n个人本金：{d.PersonalPrincipal:F2}  利息：{d.PersonalInterest:F2}  滞纳金：{d.LateFee:F2}\n合计：{d.Total:F2}元",
        }).ToList();

        cvSummary.ItemsSource = summaries.Select(s => new
        {
            HeaderText = $"{s.Name}  {s.IdCardNo}  {s.InsuranceTypeName}  {s.PeriodRange}",
            DetailText = $"基数合计：{s.TotalBase:F0}  单位本金：{s.UnitPrincipal:F2}  利息：{s.UnitInterest:F2}\n个人本金：{s.PersonalPrincipal:F2}  利息：{s.PersonalInterest:F2}  滞纳金：{s.LateFee:F2}\n合计：{s.Total:F2}元",
        }).ToList();
    }

    private static List<BankruptcyDetailResult> CalculateDetails(
        List<BankruptcyRecord> records,
        IInterestCalculationService interestSvc,
        ILateFeeCalculationService lateFeeSvc,
        bool calcInterest,
        DateTime interestEndDate,
        bool calcLateFee,
        DateTime lateFeeEndDate)
    {
        var results = new List<BankruptcyDetailResult>();
        foreach (var rec in records)
        {
            var dueDate = DateTime.ParseExact(rec.Period, "yyyyMM", null);
            double unitPrincipal = Math.Round(rec.UnitAmount, 2);
            double personalPrincipal = Math.Round(rec.PersonalAmount, 2);

            double unitInterest = 0.0, personalInterest = 0.0;
            if (calcInterest && rec.InsuranceTypeName == "企业职工基本养老保险")
            {
                unitInterest = Math.Round(interestSvc.CalculateInterest(unitPrincipal, dueDate, interestEndDate), 2);
                personalInterest = Math.Round(interestSvc.CalculateInterest(personalPrincipal, dueDate, interestEndDate), 2);
            }

            double lateFee = 0.0;
            if (calcLateFee)
            {
                double totalPrincipal = unitPrincipal + personalPrincipal;
                lateFee = Math.Round(lateFeeSvc.CalculateLateFee(totalPrincipal, lateFeeEndDate, dueDate, "欠费清算"), 2);
            }

            double total = Math.Round(unitPrincipal + unitInterest + personalPrincipal + personalInterest + lateFee, 2);

            results.Add(new BankruptcyDetailResult
            {
                Name = rec.Name,
                IdCardNo = rec.IdCardNo,
                InsuranceTypeName = rec.InsuranceTypeName,
                Period = rec.Period,
                MonthlyBase = rec.MonthlyBase,
                UnitPrincipal = unitPrincipal,
                UnitInterest = unitInterest,
                PersonalPrincipal = personalPrincipal,
                PersonalInterest = personalInterest,
                LateFee = lateFee,
                Total = total,
            });
        }
        return results;
    }

    private static List<BankruptcySummaryResult> BuildSummaries(List<BankruptcyDetailResult> details)
    {
        return details
            .GroupBy(d => new { d.IdCardNo, d.InsuranceTypeName })
            .Select(grp => new BankruptcySummaryResult
            {
                Name = grp.First().Name,
                IdCardNo = grp.Key.IdCardNo,
                InsuranceTypeName = grp.Key.InsuranceTypeName,
                PeriodRange = $"{grp.Min(d => d.Period)}-{grp.Max(d => d.Period)}",
                TotalBase = Math.Round(grp.Sum(d => d.MonthlyBase), 2),
                UnitPrincipal = Math.Round(grp.Sum(d => d.UnitPrincipal), 2),
                UnitInterest = Math.Round(grp.Sum(d => d.UnitInterest), 2),
                PersonalPrincipal = Math.Round(grp.Sum(d => d.PersonalPrincipal), 2),
                PersonalInterest = Math.Round(grp.Sum(d => d.PersonalInterest), 2),
                LateFee = Math.Round(grp.Sum(d => d.LateFee), 2),
                Total = Math.Round(grp.Sum(d => d.Total), 2),
            })
            .ToList();
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

    // -----------------------------------------------------------------------
    // Internal result DTOs (defined here to keep the result page self-contained)
    // -----------------------------------------------------------------------

    private class BankruptcyDetailResult
    {
        public string Name { get; set; } = string.Empty;
        public string IdCardNo { get; set; } = string.Empty;
        public string InsuranceTypeName { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public double MonthlyBase { get; set; }
        public double UnitPrincipal { get; set; }
        public double UnitInterest { get; set; }
        public double PersonalPrincipal { get; set; }
        public double PersonalInterest { get; set; }
        public double LateFee { get; set; }
        public double Total { get; set; }
    }

    private class BankruptcySummaryResult
    {
        public string Name { get; set; } = string.Empty;
        public string IdCardNo { get; set; } = string.Empty;
        public string InsuranceTypeName { get; set; } = string.Empty;
        public string PeriodRange { get; set; } = string.Empty;
        public double TotalBase { get; set; }
        public double UnitPrincipal { get; set; }
        public double UnitInterest { get; set; }
        public double PersonalPrincipal { get; set; }
        public double PersonalInterest { get; set; }
        public double LateFee { get; set; }
        public double Total { get; set; }
    }
}
