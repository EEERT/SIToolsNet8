using Microsoft.Extensions.Logging;
using SITools.BLL.Services;
using SITools.DAL.Repositories;
using SITools.MAUI.Pages;

namespace SITools.MAUI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// DAL 层：数据仓储（单例，历年数据内置于代码中）
		builder.Services.AddSingleton<IInterestRateRepository, InterestRateRepository>();
		builder.Services.AddSingleton<IContributionBaseRepository, ContributionBaseRepository>();

		// BLL 层：业务服务（单例，保证利率修改在整个运行期间生效）
		builder.Services.AddSingleton<IPensionCalculationService, PensionCalculationService>();
		builder.Services.AddSingleton<IInterestCalculationService, InterestCalculationService>();
		builder.Services.AddSingleton<ILateFeeCalculationService, LateFeeCalculationService>();
		builder.Services.AddSingleton<EntPensionCalculationFacade>();

		// UI 层：页面（每次导航时创建新实例）
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<EntPensionPage>();
		builder.Services.AddTransient<EntBankruptcyPage>();
		builder.Services.AddTransient<SettingPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
