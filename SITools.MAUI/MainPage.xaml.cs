namespace SITools.MAUI;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnEntPensionClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("EntPensionPage");
    }

    private async void OnEntBankruptcyClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("EntBankruptcyPage");
    }

    private async void OnSettingClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("SettingPage");
    }
}
