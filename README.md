# SIToolsNet8 — 社会保险费征缴管理工具（.NET 8.0 重构版）

## 项目简介

本项目是对原 `SIToolsApp`（.NET Framework 4.x + MetroFramework）的全面重构，采用 **C#、WinForms、.NET 8.0** 和**分层架构设计**。

---

## 分层架构说明

```
SIToolsNet8/
├── SITools.Models/        # 模型层：数据实体、枚举定义
├── SITools.DAL/           # 数据访问层：仓储接口与实现（历年利率、缴费基数）
├── SITools.BLL/           # 业务逻辑层：计算服务、验证服务、Excel服务
└── SITools.UI/            # 表现层：WinForms 窗体
```

### 各层职责

| 层次 | 项目 | 职责 |
|------|------|------|
| 模型层 | `SITools.Models` | 定义 `ContributionRecord`（补缴记录）、`CalculationResultDetail`（明细）、`CalculationResultSummary`（汇总）、`BankruptcyRecord`（清算记录）等实体及枚举 |
| 数据访问层 | `SITools.DAL` | 提供 `IInterestRateRepository`（历年利率）、`IContributionBaseRepository`（缴费基数上下限）接口及内存实现 |
| 业务逻辑层 | `SITools.BLL` | `PensionCalculationService`（本金计算）、`InterestCalculationService`（利息计算）、`LateFeeCalculationService`（滞纳金计算）、`ExcelService`（Excel导入导出）、`ValidationService`（输入验证）、`EntPensionCalculationFacade`（门面服务） |
| 表现层 | `SITools.UI` | `MainForm`（主窗体）、`EntPensionForm`（补缴测算）、`EntBankruptcyForm`（破产清算）、`SettingForm`（参数设置）、`AboutForm`（关于） |

---

## 主要功能

### 1. 企业社会保险费补缴测算（EntPensionForm）
- 支持手动录入或从 Excel 批量导入补缴信息
- 支持多种补缴类型：
  - 职工158号文补缴
  - 职工补中断
  - 历史陈欠清算
  - 个体缴费
  - 62号文件补缴（合同工/固定工）
- 自动按缴费基数上下限校正（可选）
- 计算统筹部分本金/利息、个人部分本金/利息、滞纳金
- 输出逐月明细表 + 按人员汇总表
- 支持导出 Excel

### 2. 企业破产社保费清算（EntBankruptcyForm）
- 从 Excel 导入清算数据（含险种类型、月缴费基数、单位/个人应缴金额）
- 计算利息（仅企业职工基本养老保险）、滞纳金
- 按险种类型汇总
- 支持导出 Excel

### 3. 征缴参数查询（SettingForm）
- 显示历年社保记账利率（1986—2026）
- 支持在线修改利率参数

---

## 核心计算逻辑

### 利息计算（复利）
```
第一年利息 = 本金 × 年利率 × (12 - 起始月 + 1) / 12
中间年利息 = (本金 + 第一年利息 + 历年累计利息) × 年利率
最后年利息 = (本金 + 第一年利息 + 历年累计利息) × 年利率 × (截止月 - 1) / 12
```

### 滞纳金计算
```
日滞纳金率 = 0.05%（0.0005）
滞纳金 = 本金 × 天数 × 日滞纳金率
最高不超过本金
```
- 158号文补缴：从 2011-07-01 起算
- 其他类型：从费款所属期次月起算

---

## 技术依赖

- .NET 8.0 + Windows Forms
- [ClosedXML](https://github.com/ClosedXML/ClosedXML)：Excel 读写（替代 Office Interop）
- 无数据库依赖（数据存储在内存字典中）

---

## 构建与运行

```bash
cd SIToolsNet8
dotnet restore
dotnet build
dotnet run --project SITools.UI
```

> **注意**：WinForms 项目仅支持在 Windows 系统上运行。
