namespace SITools.MAUI;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute("EntPensionPage", typeof(Pages.EntPensionPage));
		Routing.RegisterRoute("EntBankruptcyPage", typeof(Pages.EntBankruptcyPage));
		Routing.RegisterRoute("SettingPage", typeof(Pages.SettingPage));
	}
}
