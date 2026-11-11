# UI层 + Excel + 基础设施 迁移差距分析

> **分析日期**: 2026-11-11
> **原项目**: JavaFX 桌面应用 (main 模块, 22 Java + 14 FXML)
> **新项目**: Avalonia UI 桌面应用 (C# / .NET 10)

---

## 一、新项目 UI 相关文件清单

### Views (17个 .axaml + 17个 .axaml.cs)
| 文件 | 对应原项目 |
|------|-----------|
| `MainWindow.axaml` | MainWindow.fxml |
| `CreatorDashboardView.axaml` | CreatorListView.fxml (合并了列表+仪表盘) |
| `CreatorDetailView.axaml` | CreatorDetailView.fxml |
| `CreatorEditWindow.axaml` | CreatorEditDialog.fxml |
| `CreatorContactsView.axaml` | *(新增, 原项目内嵌在详情中)* |
| `ShipmentDetailView.axaml` | ShipmentDetailView.fxml |
| `ShipmentEditWindow.axaml` | ShipmentEditDialog.fxml |
| `VideoEditWindow.axaml` | VideoEditDialog.fxml |
| `CrmView.axaml` | CrmView.fxml |
| `TodayFollowUpView.axaml` | TodayFollowUpView.fxml |
| `ExcelImportView.axaml` | ExcelImportView.fxml |
| `OperationHistoryView.axaml` | OperationHistoryView.fxml |
| `ConfigView.axaml` | ConfigView.fxml |
| `SettingsView.axaml` | SettingsView.fxml |
| `BatchRemindLogView.axaml` | BatchRemindLogDialog.fxml |
| `CreatorTypeMultiSelectView.axaml` | CreatorTypeMultiSelectDialog.fxml |
| `ProductMultiSelectView.axaml` | ProductMultiSelectDialog.fxml |

### ViewModels (19个)
| 文件 | 对应原项目 |
|------|-----------|
| `MainViewModel.cs` | MainWindowController |
| `CreatorDashboardViewModel.cs` | CreatorListController + CreatorDashboardController |
| `CreatorDetailViewModel.cs` | CreatorDetailController |
| `CreatorEditViewModel.cs` | CreatorEditDialogController |
| `CreatorContactsViewModel.cs` | *(新增)* |
| `ShipmentDetailViewModel.cs` | ShipmentDetailController |
| `ShipmentEditViewModel.cs` | ShipmentEditDialogController |
| `VideoEditViewModel.cs` | VideoEditDialogController |
| `CrmViewModel.cs` | CrmViewController |
| `TodayFollowUpViewModel.cs` | TodayFollowUpController |
| `ExcelImportViewModel.cs` | ExcelImportController |
| `OperationHistoryViewModel.cs` | OperationHistoryController |
| `ConfigViewModel.cs` | ConfigViewController |
| `SettingsViewModel.cs` | SettingsViewController |
| `BatchRemindLogViewModel.cs` | BatchRemindLogDialog |
| `CreatorTypeMultiSelectViewModel.cs` | CreatorTypeMultiSelectDialog |
| `ProductMultiSelectViewModel.cs` | ProductMultiSelectDialog |
| `FilterViewModel.cs` | *(新增, 筛选逻辑独立)* |
| `ViewModelBase.cs` | *(新增, 基类)* |

### Controls (1个)
| 文件 | 说明 |
|------|------|
| `LoadingOverlay.axaml` | 加载遮罩层 |

### Services (1个)
| 文件 | 说明 |
|------|------|
| `NotificationService.cs` | 通知/确认对话框服务 |

### Excel 模块 (3个)
| 文件 | 说明 |
|------|------|
| `ExcelImportService.cs` | Excel 导入解析 |
| `ExcelExportService.cs` | Excel 导出 |
| `ImportPreview.cs` | 导入预览 DTO |

---

## 二、逐项对比

### 2.1 启动入口

| 原项目 | 新项目 | 状态 |
|--------|--------|------|
| `Launcher.java` (main方法) | `Program.cs` (Avalonia 入口) | ✅ 已迁移 |
| `AolonApp.java` (JavaFX Application) | `App.axaml.cs` (Avalonia Application) | ✅ 已迁移 |
| Spring DI 初始化 | `DependencyInjection.cs` + `Microsoft.Extensions.DependencyInjection` | ✅ 已迁移 |

### 2.2 UI 控制器/视图

| 原项目控制器 | 新项目 View + ViewModel | 状态 | 备注 |
|-------------|------------------------|------|------|
| `MainWindowController` | `MainWindow` + `MainViewModel` | ✅ 已迁移 | 导航、侧边栏、视图切换均已实现 |
| `CreatorListController` | `CreatorDashboardView` + `CreatorDashboardViewModel` | ⚠️ 部分迁移 | 列表+仪表盘合并; 缺少排序功能; 缺少批量导出按钮 |
| `CreatorDetailController` | `CreatorDetailView` + `CreatorDetailViewModel` | ✅ 已迁移 | 含联系人子视图 |
| `CreatorEditDialogController` | `CreatorEditWindow` + `CreatorEditViewModel` | ✅ 已迁移 | 弹窗改为独立窗口 |
| `CreatorDashboardController` | `CreatorDashboardView` + `CreatorDashboardViewModel` | ✅ 已迁移 | 统计卡片已实现 |
| `CrmViewController` | `CrmView` + `CrmViewModel` | ✅ 已迁移 | S/A/B 关系管理 |
| `TodayFollowUpController` | `TodayFollowUpView` + `TodayFollowUpViewModel` | ✅ 已迁移 | |
| `ShipmentDetailController` | `ShipmentDetailView` + `ShipmentDetailViewModel` | ✅ 已迁移 | |
| `ShipmentEditDialogController` | `ShipmentEditWindow` + `ShipmentEditViewModel` | ✅ 已迁移 | |
| `VideoEditDialogController` | `VideoEditWindow` + `VideoEditViewModel` | ✅ 已迁移 | |
| `ExcelImportController` | `ExcelImportView` + `ExcelImportViewModel` | ✅ 已迁移 | |
| `OperationHistoryController` | `OperationHistoryView` + `OperationHistoryViewModel` | ✅ 已迁移 | |
| `ConfigViewController` | `ConfigView` + `ConfigViewModel` | ✅ 已迁移 | 产品代码/达人类型/标签管理 |
| `SettingsViewController` | `SettingsView` + `SettingsViewModel` | ⚠️ 部分迁移 | 见下方详细分析 |
| `CreatorTypeMultiSelectDialog` | `CreatorTypeMultiSelectView` + `CreatorTypeMultiSelectViewModel` | ✅ 已迁移 | 穿梭框控件 |
| `ProductMultiSelectDialog` | `ProductMultiSelectView` + `ProductMultiSelectViewModel` | ✅ 已迁移 | 穿梭框控件 |

### 2.3 Excel 模块

| 原项目 | 新项目 | 状态 | 备注 |
|--------|--------|------|------|
| `ExcelImportService` | `ExcelImportService.cs` | ✅ 已迁移 | ClosedXML 实现, 支持复杂质量等级映射 |
| `ExcelExportService` | `ExcelExportService.cs` | ✅ 已迁移 | 达人列表+详情导出, 自动调整列宽 |
| `ImportPreview` | `ImportPreview.cs` | ✅ 已迁移 | DTO 完整 |
| *(无)* | `ExcelImportViewModel.cs` | ✅ 新增 | 导入流程编排 |

**Excel 模块功能完整性**: ✅ 完整迁移, 无缺失

### 2.4 基础设施

#### 2.4.1 EventBus → Messenger

| 原项目 | 新项目 | 状态 |
|--------|--------|------|
| `EventBus` (自定义发布/订阅) | `CommunityToolkit.Mvvm.Messaging.IMessenger` | ✅ 已迁移 |

**实现方式**:
- 使用 `WeakReferenceMessenger` 单例 (在 `App.axaml.cs` 中注册)
- 已定义的消息类型:
  - `CreatorUpdatedMessage` — 达人数据更新通知
  - `OpenCreatorDetailMessage` — 请求打开达人详情页
- ViewModel 通过 `IRecipient<T>` 接口接收消息

#### 2.4.2 NotificationService

| 原项目 | 新项目 | 状态 |
|--------|--------|------|
| `NotificationService` (Toast/弹窗) | `NotificationService.cs` (基于 Avalonia Window) | ✅ 已迁移 |

**已实现功能**:
- `ShowInfoAsync()` — 信息对话框
- `ShowWarningAsync()` — 警告对话框
- `ShowErrorAsync()` — 错误对话框
- `ConfirmAsync()` — 确认对话框 (返回 bool)
- 支持 `SetWindowResolver()` 获取宿主窗口

#### 2.4.3 多选组件

| 原项目 | 新项目 | 状态 |
|--------|--------|------|
| `MultiSelectTagComponent` | `CreatorTypeMultiSelectView` + `ProductMultiSelectView` | ✅ 已迁移 |

**实现方式**: 统一三列布局穿梭框 (可选列表 | 操作按钮组 | 已选列表)

#### 2.4.4 批量提醒日志弹窗

| 原项目 | 新项目 | 状态 |
|--------|--------|------|
| `BatchRemindLogDialog` (弹窗) | `BatchRemindLogView` (页面) | ⚠️ 部分迁移 |

**差异**:
- 原项目: 弹窗形式 (Dialog), 可从任意页面触发
- 新项目: 独立页面 (View), 通过导航进入
- 功能: 日志列表展示 + 刷新 ✅ 已实现

#### 2.4.5 设置页功能

| 功能 | 原项目 | 新项目 | 状态 |
|------|--------|--------|------|
| 数据库路径浏览 | ✅ | ⚠️ TODO | `BrowseDatabasePath()` 未实现 |
| 备份路径浏览 | ✅ | ⚠️ TODO | `BrowseBackupPath()` 未实现 |
| 保存设置 | ✅ | ⚠️ TODO | `SaveSettings()` 仅打印消息, 未持久化 |
| 自动备份开关 | ✅ | ✅ UI 已实现 | 但无实际备份逻辑 |
| 日志清理 | ✅ | ✅ 已实现 | `CleanOldLogsAsync()` 完整 |
| **数据库备份** | ✅ | ❌ 未实现 | 无备份功能 |
| **数据库恢复** | ✅ | ❌ 未实现 | 无恢复功能 |
| **导入/导出** | ✅ | ❌ 未实现 | 设置页无导入导出入口 |
| **关于信息** | ✅ | ❌ 未实现 | 无版本号/作者/许可信息 |

---

## 三、应用打包/启动入口

| 项目 | 状态 | 详情 |
|------|------|------|
| 启动入口 | ✅ | `Program.cs` → `BuildAvaloniaApp()` → `AppMain()` |
| DI 初始化 | ✅ | `App.axaml.cs` 中 `ConfigureServices()` |
| 发布配置 | ✅ | `win-x64.pubxml` 已配置 |
| 目标框架 | ✅ | .NET 10.0, SelfContained=true |
| 单文件发布 | ❌ | `PublishSingleFile=false` (可能需要优化) |
| 跨平台打包 | ⚠️ | 仅有 win-x64, 缺少 linux-x64 / osx-x64 配置 |

---

## 四、总结

### 已迁移 (功能 + 状态)

| 模块 | 功能 | 状态 |
|------|------|------|
| **启动** | 应用入口 + DI 容器 | ✅ 完成 |
| **主窗口** | 导航、侧边栏、视图切换 | ✅ 完成 |
| **达人工作台** | 列表展示、筛选、批量删除/恢复 | ✅ 完成 |
| **达人详情** | 详情查看、联系人管理 | ✅ 完成 |
| **达人编辑** | 编辑弹窗 (独立窗口) | ✅ 完成 |
| **仪表盘** | 统计卡片 (总数/已交付/已删除/已提醒/正在交付/今日跟进) | ✅ 完成 |
| **CRM 管理** | S/A/B 关系管理 | ✅ 完成 |
| **今日跟进** | 跟进列表与操作 | ✅ 完成 |
| **寄样详情** | 寄样列表查看 | ✅ 完成 |
| **寄样编辑** | 寄样编辑弹窗 | ✅ 完成 |
| **视频编辑** | 视频编辑弹窗 | ✅ 完成 |
| **Excel 导入** | 导入预览与确认 | ✅ 完成 |
| **Excel 导出** | 达人列表/详情导出 | ✅ 完成 |
| **操作历史** | 日志查看 + 撤销 | ✅ 完成 |
| **配置管理** | 产品代码/达人类型/标签 CRUD | ✅ 完成 |
| **多选穿梭框** | 达人类型/产品代码多选 | ✅ 完成 |
| **通知服务** | Info/Warning/Error/Confirm 对话框 | ✅ 完成 |
| **事件总线** | IMessenger 替代 EventBus | ✅ 完成 |
| **发布配置** | win-x64 自包含发布 | ✅ 完成 |

### 未迁移 (功能 + 优先级)

| 功能 | 优先级 | 说明 |
|------|--------|------|
| **数据库备份** | 🔴 高 | 设置页核心功能, 用户数据安全依赖 |
| **数据库恢复** | 🔴 高 | 与备份配套, 灾难恢复必需 |
| **关于页面** | 🟡 中 | 版本号、作者、许可、检查更新 |
| **设置持久化** | 🔴 高 | `SaveSettings()` 为空实现, 用户配置无法保存 |
| **文件选择对话框** | 🟡 中 | `BrowseDatabasePath/BrowseBackupPath` 为 TODO |
| **跨平台打包** | 🟢 低 | 仅 win-x64, 如需 Linux/macOS 需补充 |
| **单文件发布** | 🟢 低 | 当前多文件发布, 可优化为单文件 |

### 部分迁移 (缺失内容)

| 模块 | 缺失内容 |
|------|----------|
| **设置页** | ① 备份/恢复功能完全缺失 ② 设置持久化未实现 (TODO) ③ 文件浏览对话框未实现 (TODO) ④ 关于信息页缺失 ⑤ 导入/导出入口缺失 |
| **达人列表** | ① 排序功能缺失 (原项目支持列头排序) ② 批量导出选中项按钮缺失 |
| **批量提醒日志** | 由弹窗改为独立页面, 快捷查看便利性降低; 建议保留从仪表盘一键打开的能力 |
| **发布配置** | ① 仅 Windows x64, 缺少 Linux/macOS ② `PublishSingleFile=false`, 发布产物为多文件 |

---

## 五、建议的下一步行动

1. **P0 - 设置页完善** (建议迭代 1.2)
   - 实现 `SaveSettings()` — 使用 `System.Text.Json` 序列化到 `appsettings.json`
   - 实现 `BrowseDatabasePath()` / `BrowseBackupPath()` — Avalonia `OpenFileDialog` / `OpenFolderDialog`
   - 实现数据库备份/恢复功能

2. **P1 - 关于页面** (建议迭代 1.2)
   - 新增 "关于" 区块或独立页面
   - 显示版本号、构建信息、作者、开源许可

3. **P2 - 列表排序** (建议迭代 1.3)
   - `CreatorDashboardView` DataGrid 启用 `CanUserSortColumns="True"`
   - 绑定列的 `SortMemberPath`

4. **P3 - 跨平台打包** (建议迭代 2.0)
   - 补充 `linux-x64.pubxml` 和 `osx-x64.pubxml`
   - 考虑使用 GitHub Actions 自动化多平台构建
