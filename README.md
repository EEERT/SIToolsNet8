# SIToolsNet8 — 社会保险费征缴管理工具（.NET 8.0 重构版）

## 项目简介

本项目是对原 `SIToolsApp`（.NET Framework 4.x + MetroFramework）的全面重构，采用 **C#、WinForms、.NET 8.0** 和**分层架构设计**。

---

## 分层架构说明

```
SIToolsNet8/
├── SITools.Models/        # 模型层：数据实体、枚举定义（跨平台）
├── SITools.DAL/           # 数据访问层：仓储接口与实现（历年利率、缴费基数）（跨平台）
├── SITools.BLL/           # 业务逻辑层：计算服务、验证服务、Excel服务（跨平台）
├── SITools.UI/            # 表现层：WinForms 窗体（Windows 桌面）
└── SITools.MAUI/          # 表现层：.NET MAUI（Android 移动端）
```

### 各层职责

| 层次 | 项目 | 职责 |
|------|------|------|
| 模型层 | `SITools.Models` | 定义 `ContributionRecord`（补缴记录）、`CalculationResultDetail`（明细）、`CalculationResultSummary`（汇总）、`BankruptcyRecord`（清算记录）等实体及枚举 |
| 数据访问层 | `SITools.DAL` | 提供 `IInterestRateRepository`（历年利率）、`IContributionBaseRepository`（缴费基数上下限）接口及内存实现 |
| 业务逻辑层 | `SITools.BLL` | `PensionCalculationService`（本金计算）、`InterestCalculationService`（利息计算）、`LateFeeCalculationService`（滞纳金计算）、`ExcelService`（Excel导入导出）、`ValidationService`（输入验证）、`EntPensionCalculationFacade`（门面服务） |
| 表现层（桌面） | `SITools.UI` | `MainForm`（主窗体）、`EntPensionForm`（补缴测算）、`EntBankruptcyForm`（破产清算）、`SettingForm`（参数设置）、`AboutForm`（关于） |
| 表现层（Android） | `SITools.MAUI` | .NET MAUI Android 端，复用 BLL/DAL/Models 层，目标框架 `net10.0-android`，最低支持 Android 5.0（API 21） |

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

- .NET 8.0 + Windows Forms（桌面端）
- .NET 10.0 + .NET MAUI（Android 端）
- [ClosedXML](https://github.com/ClosedXML/ClosedXML)：Excel 读写（替代 Office Interop）
- 无数据库依赖（数据存储在内存字典中）

---

## 构建与运行

### Windows 桌面端

```bash
cd SIToolsNet8
dotnet restore
dotnet build
dotnet run --project SITools.UI
```

> **注意**：WinForms 项目仅支持在 Windows 系统上运行。

---

## Android APK 打包

### 1. 环境准备

在打包前，请确认以下环境已就绪：

| 工具 | 要求 | 说明 |
|------|------|------|
| .NET SDK | ≥ 10.0 | [下载地址](https://dotnet.microsoft.com/download) |
| .NET MAUI 工作负载 | 最新版 | 见下方安装命令 |
| JDK | 17 或以上 | Android 构建工具依赖 |
| Android SDK | API 21+ | 随 MAUI 工作负载自动安装，或通过 Android Studio 管理 |

安装 .NET MAUI 工作负载：

```bash
dotnet workload install maui-android
```

验证安装：

```bash
dotnet workload list
```

---

### 2. 更新版本号

在发布新版本前，先修改 `SITools.MAUI/SITools.MAUI.csproj` 中的版本字段：

```xml
<!-- 用户可见的版本号，如 1.1、2.0 -->
<ApplicationDisplayVersion>1.0</ApplicationDisplayVersion>

<!-- 整数构建号，每次发布递增，用于应用市场版本比较 -->
<ApplicationVersion>1</ApplicationVersion>
```

例如，发布第二个版本时改为：

```xml
<ApplicationDisplayVersion>1.1</ApplicationDisplayVersion>
<ApplicationVersion>2</ApplicationVersion>
```

---

### 3. 构建 Debug APK（开发测试）

```bash
cd SIToolsNet8
dotnet restore
dotnet build SITools.MAUI/SITools.MAUI.csproj -f net10.0-android
```

输出路径：

```
SITools.MAUI/bin/Debug/net10.0-android/com.sitools.android-Signed.apk
```

---

### 4. 构建 Release APK（正式发布）

```bash
dotnet publish SITools.MAUI/SITools.MAUI.csproj -f net10.0-android -c Release
```

输出路径：

```
SITools.MAUI/bin/Release/net10.0-android/publish/com.sitools.android-Signed.apk
```

> Debug 包由 .NET MAUI 自动使用调试密钥签名，可直接安装到设备进行测试。  
> Release 包默认也会自动使用调试密钥签名，如需上传应用市场，请参考下方"签名配置"步骤。

---

### 5. 签名配置（正式发布）

#### 5.1 生成密钥库

```bash
keytool -genkey -v -keystore sitools.keystore \
  -alias sitools -keyalg RSA -keysize 2048 -validity 10000
```

按提示输入密码和证书信息，密钥库文件 `sitools.keystore` 请妥善保管，**切勿提交至版本库**。

#### 5.2 在 `.csproj` 中配置签名

在 `SITools.MAUI/SITools.MAUI.csproj` 的 `<PropertyGroup>` 中添加：

```xml
<AndroidKeyStore>true</AndroidKeyStore>
<AndroidSigningKeyStore>sitools.keystore</AndroidSigningKeyStore>
<AndroidSigningKeyAlias>sitools</AndroidSigningKeyAlias>
<AndroidSigningKeyPass>your_key_password</AndroidSigningKeyPass>
<AndroidSigningStorePass>your_store_password</AndroidSigningStorePass>
```

> **安全提示**：密码建议通过环境变量或 CI/CD secrets 传入，避免明文写入 `.csproj`：
>
> ```bash
> dotnet publish SITools.MAUI/SITools.MAUI.csproj -f net10.0-android -c Release \
>   -p:AndroidSigningKeyPass=$KEY_PASS \
>   -p:AndroidSigningStorePass=$STORE_PASS
> ```

#### 5.3 发布已签名 APK

```bash
dotnet publish SITools.MAUI/SITools.MAUI.csproj -f net10.0-android -c Release \
  -p:AndroidKeyStore=true \
  -p:AndroidSigningKeyStore=sitools.keystore \
  -p:AndroidSigningKeyAlias=sitools \
  -p:AndroidSigningKeyPass=your_key_password \
  -p:AndroidSigningStorePass=your_store_password
```

---

### 6. 安装 APK 到设备

确保 Android 设备已启用"USB 调试"或"允许安装未知来源应用"，然后执行：

```bash
adb install SITools.MAUI/bin/Release/net10.0-android/publish/com.sitools.android-Signed.apk
```

---

### 7. 常见问题

| 问题 | 解决方案 |
|------|----------|
| `ANDROID_HOME` 未设置 | 安装 Android Studio 或通过 `dotnet workload install maui-android` 自动配置 |
| 找不到 JDK | 安装 JDK 17+，并设置 `JAVA_HOME` 环境变量 |
| 构建时 MAUI 工作负载缺失 | 执行 `dotnet workload install maui-android` |
| APK 无法安装（版本冲突）| 先卸载旧版本，或递增 `<ApplicationVersion>` 后重新构建 |
| 签名失败 | 检查密钥库路径、别名与密码是否正确 |
