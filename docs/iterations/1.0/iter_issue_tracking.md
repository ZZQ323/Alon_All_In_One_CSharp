# TKCompass 迭代 1.0 — 问题排查与修复记录

> 日期：2026-11-08
> 状态：✅ 核心问题已修复
> 目的：记录开发过程中遇到的问题、排查方案、执行结果

---

## 问题 #1：缺少 .gitignore 导致大文件污染仓库 ✅ 已修复

**发现日期**：2026-11-08  
**问题类型**：配置缺失  
**严重程度**：🔴 高

### 问题描述

项目缺少 `.gitignore` 文件，导致 `bin/` 和 `obj/` 目录被 Git 跟踪。发现多个 80MB+ 的 PDB 调试符号文件：
- `libSkiaSharp.pdb` (82.74 MB)
- `libHarfBuzzSharp.pdb` (20 MB)

这些文件会：
1. 显著增加 Git 仓库体积
2. 拖慢 clone/pull 速度
3. 污染版本历史

### 修复方案

创建 `.gitignore` 文件，排除：
- `bin/` 和 `obj/` 目录
- NuGet 包缓存
- Visual Studio 用户文件
- 构建产物
- SQLite 数据库文件（可选）
- IDE 配置文件

### 修复文件

- `/mnt/d/Projects/TKCompass/.gitignore`（新建）

### 验证状态

- [x] `.gitignore` 文件已创建
- [x] 配置正确排除 bin/obj 目录
- [ ] 清理已跟踪的 `bin/obj` 文件（需用户执行 `git rm -r --cached bin obj`）
- [ ] 提交 `.gitignore` 到 Git

### 经验总结

1. **新项目第一步**：创建 `.gitignore`，避免大文件污染仓库
2. **.NET 项目标准排除**：`bin/`, `obj/`, `*.user`, `.vs/`, `packages/`
3. **SQLite 数据库**：根据项目需求决定是否排除（开发期可排除，生产期需包含）

---

## 问题 #2：ExcelImportViewModel 导入逻辑未实现 ✅ 已修复

**发现日期**：2026-11-08  
**问题类型**：功能未完成  
**严重程度**：🟡 中

### 问题描述

`ExcelImportViewModel.ImportAsync()` 方法中有 TODO 注释，实际导入逻辑未实现：

```csharp
// TODO: 实现实际的导入逻辑
// 需要注入 ICreatorService, IVideoService 等服务
await Task.Delay(1000); // 模拟导入过程
```

### 修复方案

1. 注入 `ICreatorService`, `IVideoService`, `IShipmentService`, `ICreatorContactService`, `IOperationLogService` 等服务
2. 遍历 `Preview.CreatorRows` 和 `Preview.VideoRows`，调用对应的 `CreateAsync` 方法
3. 实现冲突处理（跳过已存在的达人）
4. 创建达人、寄样、视频、联系方式
5. 发布 `CreatorUpdatedMessage` 通知 UI 刷新

### 修复文件

- `/mnt/d/Projects/TKCompass/src/TKCompass.App/ViewModels/ExcelImportViewModel.cs`
- `/mnt/d/Projects/TKCompass/src/TKCompass.App/ViewModels/MainViewModel.cs`（更新构造函数）

### 验证状态

- [x] 代码修复完成
- [x] 注入所有必需的服务
- [x] 实现完整的导入逻辑
- [x] 发布 CreatorUpdatedMessage
- [ ] 编译验证（需用户在 Windows 环境执行 `dotnet build`）
- [ ] 功能测试（导入真实 Excel 文件）

### 经验总结

1. **服务注入**：ViewModel 需要的所有服务必须在构造函数中注入
2. **数据导入流程**：达人 → 寄样 → 视频 → 联系方式，按依赖顺序创建
3. **冲突处理**：跳过已存在的达人，避免重复导入

---

## 问题 #3：OperationHistoryViewModel 撤销功能未实现 ✅ 已修复

**发现日期**：2026-11-08  
**问题类型**：功能未完成  
**严重程度**：🟡 中

### 问题描述

`OperationHistoryViewModel` 只有加载日志功能，缺少：
- 撤销按钮命令
- 批量撤销（按 group_id）
- Ctrl+Z 全局快捷键绑定

### 修复方案

1. 注入 `IUndoService` 和 `IMessenger`
2. 添加 `UndoCommand` 和 `UndoGroupCommand`
3. 调用 `UndoService.UndoLastAsync()` 和 `UndoService.UndoGroupAsync(groupId)`
4. 撤销成功后发布 `CreatorUpdatedMessage` 刷新 UI
5. 在 `OperationHistoryView.axaml` 中添加撤销按钮

### 修复文件

- `/mnt/d/Projects/TKCompass/src/TKCompass.App/ViewModels/OperationHistoryViewModel.cs`
- `/mnt/d/Projects/TKCompass/src/TKCompass.App/Views/OperationHistoryView.axaml`
- `/mnt/d/Projects/TKCompass/src/TKCompass.App/DependencyInjection.cs`（注册 IUndoService）
- `/mnt/d/Projects/TKCompass/src/TKCompass.App/ViewModels/MainViewModel.cs`（注入 IUndoService）

### 验证状态

- [x] 代码修复完成
- [x] 注入 IUndoService 和 IMessenger
- [x] 实现 UndoCommand 和 UndoGroupCommand
- [x] 添加撤销按钮到 UI
- [x] 注册 IUndoService 到 DI 容器
- [x] 撤销成功后发布 CreatorUpdatedMessage
- [ ] 编译验证（需用户在 Windows 环境执行 `dotnet build`）
- [ ] 功能测试（撤销操作）

### 经验总结

1. **撤销系统集成**：UI 层调用 Service 层，Service 层操作数据库
2. **批量撤销**：通过 group_id 识别同一次导入的所有操作
3. **UI 刷新**：撤销成功后必须发布消息刷新所有 Tab

---

## 问题 #4：FilterBar 产品代码多选未集成 ⏸️ 暂缓

**发现日期**：2026-11-08  
**问题类型**：功能未完成  
**严重程度**：🟢 低

### 问题描述

`CreatorDashboardViewModel.ApplyFilter()` 中有 TODO：

```csharp
if (!string.IsNullOrEmpty(Filter.ProductNameSearch))
{
    // TODO: 需要关联产品名搜索
}
```

产品代码多选筛选器未实现。

### 暂缓原因

1. 优先级较低，基础筛选功能已可用
2. 需要更多 UI 设计工作（多选对话框集成）
3. 可在后续迭代中完善

### 修复方案（待实施）

1. 在 `FilterViewModel` 中添加 `SelectedProductCodes` 属性
2. 在 `FilterBar.axaml` 中添加产品代码多选按钮
3. 在 `ApplyFilter()` 中实现产品代码筛选逻辑（LIKE 查询）

### 修复文件

- `/mnt/d/Projects/TKCompass/src/TKCompass.App/ViewModels/FilterViewModel.cs`
- `/mnt/d/Projects/TKCompass/src/TKCompass.App/Views/FilterBar.axaml`
- `/mnt/d/Projects/TKCompass/src/TKCompass.App/ViewModels/CreatorDashboardViewModel.cs`

### 验证状态

- [ ] 代码修复完成
- [ ] 编译验证
- [ ] 功能测试（产品代码筛选）

---

## 问题 #5：v-model 全局刷新机制未完全实现 ✅ 已修复

**发现日期**：2026-11-08  
**问题类型**：架构缺陷  
**严重程度**：🟡 中

### 问题描述

用户要求："所有的设计修改数据表的操作，在修改保存后，都应当能触发任意页面、任意 tab 的刷新"

初始状态：
- `WeakReferenceMessenger` 已配置
- `CreatorUpdatedMessage` 已定义
- `CreatorDashboardViewModel` 已订阅并刷新
- **但其他 ViewModel（ConfigViewModel, SettingsViewModel 等）未订阅**

### 修复方案

1. 在所有需要刷新的 ViewModel 中实现 `IRecipient<CreatorUpdatedMessage>`
2. 在所有数据修改操作后发布 `CreatorUpdatedMessage`
3. 确保字典变更（ConfigViewModel）也触发刷新

### 修复文件

**发布消息的 ViewModel**（数据修改后发送）：
- `/mnt/d/Projects/TKCompass/src/TKCompass.App/ViewModels/CreatorEditViewModel.cs` ✅ 已实现
- `/mnt/d/Projects/TKCompass/src/TKCompass.App/ViewModels/ShipmentEditViewModel.cs` ✅ 已修复
- `/mnt/d/Projects/TKCompass/src/TKCompass.App/ViewModels/VideoEditViewModel.cs` ✅ 已修复
- `/mnt/d/Projects/TKCompass/src/TKCompass.App/ViewModels/CreatorContactsViewModel.cs` ✅ 已修复
- `/mnt/d/Projects/TKCompass/src/TKCompass.App/ViewModels/ConfigViewModel.cs` ✅ 已实现
- `/mnt/d/Projects/TKCompass/src/TKCompass.App/ViewModels/ExcelImportViewModel.cs` ✅ 已修复
- `/mnt/d/Projects/TKCompass/src/TKCompass.App/ViewModels/OperationHistoryViewModel.cs` ✅ 已修复

**订阅消息的 ViewModel**（接收消息后刷新）：
- `/mnt/d/Projects/TKCompass/src/TKCompass.App/ViewModels/CreatorDashboardViewModel.cs` ✅ 已实现
- `/mnt/d/Projects/TKCompass/src/TKCompass.App/ViewModels/CrmViewModel.cs` ✅ 已修复
- `/mnt/d/Projects/TKCompass/src/TKCompass.App/ViewModels/TodayFollowUpViewModel.cs` ✅ 已修复

### 验证状态

- [x] 代码修复完成
- [x] 所有修改数据的 ViewModel 发布消息
- [x] 所有需要刷新的 ViewModel 订阅消息
- [x] 构造函数更新（注入 IMessenger）
- [ ] 编译验证（需用户在 Windows 环境执行 `dotnet build`）
- [ ] 功能测试（修改配置后工作台自动刷新）

### 经验总结

1. **v-model 核心机制**：WeakReferenceMessenger + CreatorUpdatedMessage
2. **发布-订阅模式**：修改数据 → 发布消息 → 所有订阅者刷新
3. **构造函数注入**：所有需要通信的 ViewModel 必须注入 IMessenger
4. **IRecipient 接口**：实现 Receive 方法自动接收消息

---

## 修复总结

### 已完成修复

1. ✅ **.gitignore 创建**：排除 bin/obj 等大文件
2. ✅ **ExcelImport 导入逻辑**：实现完整的导入流程
3. ✅ **OperationHistory 撤销功能**：集成 UndoService，添加撤销按钮
4. ✅ **v-model 全局刷新**：所有修改数据的 ViewModel 发布消息，所有需要刷新的 ViewModel 订阅消息

### 待验证项目

所有修复需要在 Windows 环境执行以下验证：

```powershell
cd D:\Projects\TKCompass
dotnet build TKCompass.sln
dotnet run --project src/TKCompass.App
```

### 待用户执行

1. 清理已跟踪的 bin/obj 文件：
   ```powershell
   git rm -r --cached .
   git add .
   git commit -m "chore: remove bin/obj from tracking"
   ```

2. 验证编译通过

3. 测试以下功能：
   - Excel 导入
   - 操作历史撤销
   - 修改配置后工作台自动刷新
   - 修改达人信息后其他页面自动刷新

---

*最后更新：2026-11-08*
