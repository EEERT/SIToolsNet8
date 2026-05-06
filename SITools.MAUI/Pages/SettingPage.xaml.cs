using System.Collections.ObjectModel;
using System.ComponentModel;
using SITools.BLL.Services;

namespace SITools.MAUI.Pages;

/// <summary>
/// 利率参数设置页面。
/// 允许用户查看并修改历年社保记账利率。修改后保存，当次运行期间生效。
/// </summary>
public partial class SettingPage : ContentPage
{
    private readonly IInterestCalculationService _interestSvc;
    private readonly ObservableCollection<RateRowViewModel> _rateRows = new();

    public SettingPage(IInterestCalculationService interestSvc)
    {
        _interestSvc = interestSvc;
        InitializeComponent();
        cvRates.ItemsSource = _rateRows;
        LoadRates();
    }

    private void LoadRates()
    {
        _rateRows.Clear();
        var rates = _interestSvc.GetRateDataList();
        foreach (var r in rates)
            _rateRows.Add(new RateRowViewModel(r.Year, r.Rate));
    }

    private async void OnSave(object sender, EventArgs e)
    {
        var updatedRates = new Dictionary<int, double>();
        foreach (var row in _rateRows)
        {
            if (double.TryParse(row.RateText, out double rate))
                updatedRates[row.Year] = rate;
        }
        _interestSvc.UpdateRates(updatedRates);
        await DisplayAlert("提示", "利率参数已更新！", "确定");
    }

    private void OnReset(object sender, EventArgs e)
    {
        LoadRates();
    }
}

/// <summary>
/// 利率行视图模型，支持双向绑定编辑利率。
/// </summary>
public class RateRowViewModel : INotifyPropertyChanged
{
    public int Year { get; }
    public string YearText => $"{Year}年";

    private string _rateText;
    public string RateText
    {
        get => _rateText;
        set
        {
            _rateText = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RateText)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public RateRowViewModel(int year, double rate)
    {
        Year = year;
        _rateText = rate.ToString("0.0000");
    }
}
