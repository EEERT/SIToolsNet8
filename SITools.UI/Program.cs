using SITools.BLL.Services;
using SITools.DAL.Repositories;
using SITools.UI.Forms;

namespace SITools.UI;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // 构建依赖（手工依赖注入，分层架构）
        var rateRepo = new InterestRateRepository();
        var baseRepo = new ContributionBaseRepository();
        var pensionSvc = new PensionCalculationService(baseRepo);
        var interestSvc = new InterestCalculationService(rateRepo);
        var lateFeeSvc = new LateFeeCalculationService();
        var calcFacade = new EntPensionCalculationFacade(pensionSvc, interestSvc, lateFeeSvc);
        var excelSvc = new ExcelService();

        Application.Run(new MainForm(pensionSvc, interestSvc, lateFeeSvc, calcFacade, excelSvc));
    }
}
