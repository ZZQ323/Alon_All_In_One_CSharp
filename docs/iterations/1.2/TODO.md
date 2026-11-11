# 迭代 1.2 — 任务清单

**创建时间**: 2026-11-11  
**目标**: 完成 P1 核心缺失功能，提升用户体验  
**状态**: ✅ 已完成

---

## 任务概览

| 优先级 | 任务数 | 完成数 | 完成率 |
|--------|--------|--------|--------|
| P1 | 7 | 7 | 100% |
| **总计** | **7** | **7** | **100%** |

---

## P1 核心任务

### 任务 1: 创建 FulfillmentService（履约业务逻辑）

**目标**: 实现履约状态查询和业务逻辑  
**修改文件**:
- `TKCompass.Domain/Services/IFulfillmentService.cs` (新建)
- `TKCompass.Domain/Services/FulfillmentService.cs` (新建)
- `TKCompass.Domain/Repositories/IFulfillmentRepository.cs` (新建)
- `TKCompass.Data/Repositories/FulfillmentRepository.cs` (新建)

**验收标准**:
- [x] 接口定义完整（GetFulfillmentInfoAsync, UpdateFulfillmentStatusAsync）
- [x] 服务实现包含业务逻辑
- [x] 仓储实现查询 v_creator_fulfillment 视图
- [x] 编译通过（0 错误）
- [x] DI 注册完成

**依赖**: 无  
**预估工时**: 0.5 天

---

### 任务 2: 创建 FollowUpService（跟进业务逻辑）

**目标**: 实现跟进状态管理和提醒逻辑  
**修改文件**:
- `TKCompass.Domain/Services/IFollowUpService.cs` (新建)
- `TKCompass.Domain/Services/FollowUpService.cs` (新建)

**验收标准**:
- [x] 接口定义完整（GetTodayFollowUpsAsync, MarkAsFollowedAsync）
- [x] 服务实现包含业务逻辑
- [x] 依赖 IFollowUpRepository（已在 1.1 创建）
- [x] 编译通过（0 错误）
- [x] DI 注册完成

**依赖**: 无  
**预估工时**: 0.5 天

---

### 任务 3: 创建 TodayTaskService（今日任务聚合）

**目标**: 聚合今日待办任务（跟进+履约）  
**修改文件**:
- `TKCompass.Domain/Services/ITodayTaskService.cs` (新建)
- `TKCompass.Domain/Services/TodayTaskService.cs` (新建)

**验收标准**:
- [x] 接口定义完整（GetTodayTasksAsync）
- [x] 聚合 FollowUpService 和 FulfillmentService 数据
- [x] 返回统一的 TodayTaskDto 列表
- [x] 编译通过（0 错误）
- [x] DI 注册完成

**依赖**: 任务 1, 2  
**预估工时**: 0.5 天

---

### 任务 4: 创建 BackupService（数据库备份/恢复）

**目标**: 实现数据库备份和恢复功能  
**修改文件**:
- `TKCompass.Domain/Services/IBackupService.cs` (新建)
- `TKCompass.Domain/Services/BackupService.cs` (新建)

**验收标准**:
- [x] 接口定义完整（BackupAsync, RestoreAsync, GetBackupListAsync）
- [x] 实现手动备份（复制 SQLite 文件）
- [x] 实现恢复功能（替换 SQLite 文件）
- [x] 实现备份列表查询（按时间排序）
- [x] 实现备份保留策略（默认保留 7 天）
- [x] 编译通过（0 错误）
- [x] DI 注册完成

**依赖**: 无  
**预估工时**: 1 天

---

### 任务 5: 完善设置持久化

**目标**: 实现用户设置保存和加载  
**修改文件**:
- `TKCompass.App/ViewModels/SettingsViewModel.cs` (修改)

**验收标准**:
- [x] 实现 SaveSettings() 方法（保存到 appsettings.json）
- [x] 实现 LoadSettings() 方法（启动时加载）
- [x] 支持数据库路径、备份路径、自动备份开关
- [x] 编译通过（0 错误）

**依赖**: 无  
**预估工时**: 0.5 天

---

### 任务 6: 实现文件选择对话框

**目标**: 实现浏览文件/文件夹选择功能  
**修改文件**:
- `TKCompass.App/ViewModels/SettingsViewModel.cs` (修改)

**验收标准**:
- [x] 实现 BrowseDatabasePath() 方法（选择 SQLite 文件）
- [x] 实现 BrowseBackupPath() 方法（选择备份文件夹）
- [x] 使用 Avalonia StorageProvider API
- [x] 编译通过（0 错误）

**依赖**: 无  
**预估工时**: 0.5 天

---

### 任务 7: 实现达人列表排序

**目标**: 启用列头点击排序功能  
**修改文件**:
- `TKCompass.App/Views/CreatorDashboardView.axaml` (修改)
- `TKCompass.App/ViewModels/CreatorDashboardViewModel.cs` (修改)

**验收标准**:
- [x] DataGrid 启用 CanUserSortColumns（6个Tab全部启用）
- [x] Avalonia DataGrid 自动处理排序逻辑（无需额外 ViewModel 代码）
- [x] 支持升序/降序切换（DataGrid 内置功能）
- [x] 编译通过（0 错误）

**依赖**: 无  
**预估工时**: 0.5 天

---

## 任务依赖关系

```
任务 1 (FulfillmentService) ──┐
                              ├──> 任务 3 (TodayTaskService)
任务 2 (FollowUpService) ─────┘

任务 4 (BackupService) — 独立
任务 5 (设置持久化) — 独立
任务 6 (文件选择对话框) — 独立
任务 7 (列表排序) — 独立
```

---

## 执行计划

### Batch 1（并行）
- 任务 1: FulfillmentService
- 任务 2: FollowUpService
- 任务 4: BackupService

### Batch 2（依赖 Batch 1）
- 任务 3: TodayTaskService（依赖任务 1, 2）
- 任务 5: 设置持久化
- 任务 6: 文件选择对话框

### Batch 3（独立）
- 任务 7: 列表排序

---

## 完成标记

- [x] 任务 1: FulfillmentService ✅
- [x] 任务 2: FollowUpService ✅
- [x] 任务 3: TodayTaskService ✅
- [x] 任务 4: BackupService ✅
- [x] 任务 5: 设置持久化 ✅
- [x] 任务 6: 文件选择对话框 ✅
- [x] 任务 7: 列表排序 ✅

---

## 🎉 迭代 1.2 完成总结

**完成时间**: 2026-11-11  
**完成度**: 7/7 (100%)  
**编译状态**: ✅ 0 错误，26 警告（警告为 AVLN5001 TextBox.Watermark 过时提示）

### 已完成工作

| 任务 | 说明 | 状态 |
|------|------|------|
| FulfillmentService | 履约业务逻辑层（接口+实现+仓储） | ✅ |
| FollowUpService | 跟进业务逻辑层 | ✅ |
| TodayTaskService | 今日任务聚合服务 | ✅ |
| BackupService | 数据库备份/恢复服务 | ✅ |
| 设置持久化 | SettingsViewModel 保存/加载配置 | ✅ |
| 文件选择对话框 | Avalonia StorageProvider 集成 | ✅ |
| 达人列表排序 | 6个 DataGrid 启用排序功能 | ✅ |

---

*文档结束 — 最后更新 2026-11-11 — 迭代 1.2 圆满完成 🎊*
