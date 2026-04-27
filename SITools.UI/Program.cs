using SITools.BLL.Services;
using SITools.DAL.Repositories;
using SITools.UI.Forms;

namespace SITools.UI;

/// <summary>
/// 程序入口类。
/// 这是应用程序启动时最先执行的代码，负责：
///   1. 初始化应用程序（字体、界面缩放等基础设置）
///   2. 手工组装依赖关系（依赖注入）：按照从底层到上层的顺序创建各层对象
///   3. 启动主窗体（MainForm）
///
/// ★ 分层架构说明：
///   本项目采用典型的三层架构（Three-Layer Architecture）：
///     DAL（数据访问层）→ BLL（业务逻辑层）→ UI（用户界面层）
///   上层依赖下层，但只通过接口（interface）依赖，不直接依赖具体实现。
///   在 Program.Main() 中，手工按依赖顺序创建各对象并逐层传递，
///   这种做法称为"手工依赖注入"（Manual Dependency Injection）。
/// </summary>
static class Program
{
    /// <summary>
    /// 应用程序主入口方法。
    /// [STAThread] 特性表示主线程使用单线程单元（STA）模型，这是 WinForms 应用的必要设置。
    /// </summary>
    [STAThread]
    static void Main()
    {
        // 初始化 WinForms 应用配置（DPI缩放、默认字体等），.NET 6+ 推荐方式
        ApplicationConfiguration.Initialize();

        // ---------------------------------------------------------------
        // 手工依赖注入：按照 DAL → BLL → UI 的顺序创建对象
        // ---------------------------------------------------------------

        // DAL 层：创建数据仓储（内置历年政策数据，无需连接数据库）
        var rateRepo = new InterestRateRepository();   // 历年社保记账利率仓储
        var baseRepo = new ContributionBaseRepository(); // 历年缴费基数上下限仓储

        // BLL 层：创建业务服务（注入所需仓储）
        var pensionSvc = new PensionCalculationService(baseRepo);  // 本金计算服务（依赖基数仓储）
        var interestSvc = new InterestCalculationService(rateRepo); // 利息计算服务（依赖利率仓储）
        var lateFeeSvc = new LateFeeCalculationService();           // 滞纳金计算服务（无外部依赖）

        // BLL 门面：将三个服务组合为一个协调器
        var calcFacade = new EntPensionCalculationFacade(pensionSvc, interestSvc, lateFeeSvc);

        // 工具服务：Excel 导入导出（无依赖）
        var excelSvc = new ExcelService();

        // UI 层：创建主窗体（注入所有服务），并启动应用程序消息循环
        // Application.Run 会持续运行直到主窗体关闭
        Application.Run(new MainForm(pensionSvc, interestSvc, lateFeeSvc, calcFacade, excelSvc));
    }
}
