# Domain 层功能差距分析

> **生成时间**: 2026-11-11  
> **分析范围**: 原项目 Java Domain 层 (app-core) vs 新项目 C# Domain 层 (TKCompass.Domain)

---

## 一、模型层 (Models) 对比

### ✅ 已迁移（完整）

| 原项目 | 新项目 | 状态 | 备注 |
|--------|--------|------|------|
| `Creator` | `Creator.cs` | ✅ 完整 | 含 V2.0 字段 (国家/地区/内容分类)，废弃字段已注释 |
| `Shipment` | `Shipment.cs` | ✅ 完整 | 含 V2.0 字段 |
| `Video` | `Video.cs` | ✅ 完整 | 含广告码/代码回收日期 |
| `Relationship` | `Relationship.cs` | ✅ 完整 | S/A/B 达人关系 |
| `ProductCode` | `ProductCode.cs` | ✅ 完整 | 产品代码字典 |
| `CreatorType` | `CreatorType.cs` | ✅ 完整 | 达人类型字典 |
| `TagDefinition` | `TagDefinition.cs` | ✅ 完整 | 含 IsSystem/TagType/Color |
| `CreatorTag` | `CreatorTag.cs` | ✅ 完整 | 达人-标签关联 |
| `CreatorContact` | `CreatorContact.cs` | ✅ 完整 | 动态多值联系方式 |
| `OperationLog` | `OperationLog.cs` | ✅ 完整 | BeforeJson/AfterJson/GroupId |
| `StatusFlowHistory` | `StatusFlowHistory.cs` | ✅ 完整 | 状态流转历史 |
| `Dictionary` | `Dictionary.cs` | ✅ 完整 | 通用字典 |
| `BatchRemindLog` | `BatchRemindLog.cs` | ✅ 完整 | 批量提醒日志 |

### ✅ 已迁移（DTO）

| 原项目 | 新项目 | 状态 | 备注 |
|--------|--------|------|------|
| `CreatorDashboard` | `CreatorDashboardDto.cs` | ✅ 完整 | 工作台视图 DTO |
| `FollowUpStatus` | `FollowUpStatusDto.cs` | ✅ 完整 | 跟进状态 DTO |
| `TodayFollowUp` | `TodayFollowUpDto.cs` | ✅ 完整 | 今日跟进 DTO |
| `FulfillmentInfo` | `FulfillmentInfoDto.cs` | ✅ 完整 | 履约状态 DTO |

### ❌ 未迁移

| 原项目 | 功能描述 | 优先级 | 建议 |
|--------|----------|--------|------|
| `BestVideo` | S/A/B CRM 代表视频 | **P2** | 可在 API 层动态查询，无需独立实体 |

---

## 二、服务层 (Services) 对比

### ✅ 已迁移（完整）

| 原项目 | 新项目 | 状态 | 核心方法 |
|--------|--------|------|----------|
| `CreatorService` | `CreatorService.cs` | ✅ 完整 | CRUD/软删除/恢复/状态流转/标记交付/Handle唯一性 |
| `ShipmentService` | `ShipmentService.cs` | ✅ 完整 | CRUD + 关联达人 |
| `VideoService` | `VideoService.cs` | ✅ 完整 | CRUD + 关联达人/寄样 |
| `RelationshipService` | `RelationshipService.cs` | ✅ 完整 | 达人关系管理 |
| `ProductCodeService` | `ProductCodeService.cs` | ✅ 完整 | 产品代码 CRUD |
| `CreatorTypeService` | `CreatorTypeService.cs` | ✅ 完整 | 达人类型 CRUD |
| `TagService` | `TagService.cs` | ✅ 完整 | 标签管理 |
| `DictionaryService` | `DictionaryService.cs` | ✅ 完整 | 通用字典 |
| `CreatorContactService` | `CreatorContactService.cs` | ✅ 完整 | 联系方式管理 |
| `BatchRemindLogService` | `BatchRemindLogService.cs` | ✅ 完整 | 批量提醒日志 |
| `DataExportService` | `DataExportService.cs` | ✅ 完整 | 11表 JSON→ZIP 导出 |
| `DataImportService` | `DataImportService.cs` | ✅ 完整 | ZIP/JSON 全量覆盖导入 |
| `UndoService` | `UndoService.cs` | ✅ 完整 | 撤销/回滚引擎 |

### ⚠️ 部分迁移

| 原项目 | 新项目 | 状态 | 缺失功能 |
|--------|--------|------|----------|
| `CooperationStateMachine` | 无独立类 | **部分缺失** | 状态流转逻辑内嵌在 `CreatorService` 中，无独立状态机类 |
| `OperationLogger` | 无接口抽象 | **部分缺失** | 日志记录直接内嵌在 Service 中，无 `IOperationLogger` 接口 |
| `DefaultOperationLogger` | 无 | **部分缺失** | 同上 |
| `OperationContext` | 无 | **部分缺失** | 无操作上下文（SessionId/User 传递） |

### ❌ 未迁移

| 原项目 | 功能描述 | 优先级 | 建议 |
|--------|----------|--------|------|
| `FulfillmentService` | 履约状态查询 | **P1** | 建议从 Repository 层视图查询实现 |
| `FollowUpService` | 跟进状态管理 | **P1** | 建议从 Repository 层视图查询实现 |
| `TodayTaskService` | 今日待办 | **P1** | 建议从 Repository 层视图查询实现 |
| `BackupService` | 数据库备份（手动/定时/保留策略） | **P2** | 可后续扩展，当前非核心 |

### ❌ 未迁移（异常类）

| 原项目 | 功能描述 | 优先级 | 建议 |
|--------|----------|--------|------|
| `RemindedCreatorExistsException` | 已提醒达人异常 | **P2** | 可用业务规则验证替代 |
| `DeletedCreatorExistsException` | 已删除达人异常 | **P2** | 可用业务规则验证替代 |
| `DeliveredCreatorExistsException` | 已交付达人异常 | **P2** | 可用业务规则验证替代 |

---

## 三、仓储接口 (Repositories) 对比

### ✅ 已迁移（完整）

| 原项目 | 新项目 | 状态 |
|--------|--------|------|
| `ICreatorRepository` | `ICreatorRepository.cs` | ✅ 完整 |
| `ICreatorDashboardRepository` | `ICreatorDashboardRepository.cs` | ✅ 完整 |
| `IShipmentRepository` | `IShipmentRepository.cs` | ✅ 完整 |
| `IVideoRepository` | `IVideoRepository.cs` | ✅ 完整 |
| `IRelationshipRepository` | `IRelationshipRepository.cs` | ✅ 完整 |
| `IProductCodeRepository` | `IProductCodeRepository.cs` | ✅ 完整 |
| `ICreatorTypeRepository` | `ICreatorTypeRepository.cs` | ✅ 完整 |
| `ITagDefinitionRepository` | `ITagDefinitionRepository.cs` | ✅ 完整 |
| `ICreatorTagRepository` | `ICreatorTagRepository.cs` | ✅ 完整 |
| `ICreatorContactRepository` | `ICreatorContactRepository.cs` | ✅ 完整 |
| `IOperationLogRepository` | `IOperationLogRepository.cs` | ✅ 完整 |
| `IStatusFlowHistoryRepository` | `IStatusFlowHistoryRepository.cs` | ✅ 完整 |
| `IBatchRemindLogRepository` | `IBatchRemindLogRepository.cs` | ✅ 完整 |
| `IDictionaryRepository` | `IDictionaryRepository.cs` | ✅ 完整 |

### ❌ 未迁移

| 原项目 | 功能描述 | 优先级 | 建议 |
|--------|----------|--------|------|
| `IFulfillmentRepository` | 履约状态视图仓储 | **P1** | 需配合 FulfillmentService |
| `IFollowUpRepository` | 跟进状态视图仓储 | **P1** | 需配合 FollowUpService |

---

## 四、筛选框架 (Filter) 对比

### ❌ 完全未迁移

| 原项目 | 功能描述 | 优先级 | 建议 |
|--------|----------|--------|------|
| `FilterCriteria` | 筛选条件（简单+复合 AND/OR） | **P0** | 核心功能，需完整迁移 |
| `FilterOperator` | 15种操作符枚举 | **P0** | 核心功能，需完整迁移 |
| `FilterLogicOperator` | AND/OR 逻辑运算符 | **P0** | 核心功能，需完整迁移 |

> **重要提示**: 筛选框架是达人列表、寄样列表、视频列表等所有查询功能的核心依赖，**必须优先迁移**。

---

## 五、工具类 (Utils) 对比

### ❌ 完全未迁移

| 原项目 | 功能描述 | 优先级 | 建议 |
|--------|----------|--------|------|
| `AppPaths` | 应用路径管理 | **P2** | 可用 .NET Configuration 替代 |
| `AppVersion` | 应用版本信息 | **P2** | 可用 Assembly 信息替代 |
| `InstallLocation` | 安装位置检测 | **P2** | 桌面应用相关，Web 版可忽略 |
| `UpgradeDetector` | 升级检测 | **P2** | 桌面应用相关，Web 版可忽略 |
| `SingleInstanceChecker` | 单实例检查 | **P2** | 桌面应用相关，Web 版可忽略 |

> **说明**: 工具类主要针对桌面应用（JavaFX），新项目为 Web 应用，这些类可忽略或用 .NET 标准方式替代。

---

## 六、迁移优先级总结

### 🔴 P0 - 必须立即迁移（阻塞核心功能）

| 类别 | 类名 | 原因 |
|------|------|------|
| 筛选框架 | `FilterCriteria` | 所有列表查询的核心 |
| 筛选框架 | `FilterOperator` | 筛选操作符定义 |
| 筛选框架 | `FilterLogicOperator` | AND/OR 逻辑支持 |

### 🟡 P1 - 建议尽快迁移（影响业务完整性）

| 类别 | 类名 | 原因 |
|------|------|------|
| 服务 | `FulfillmentService` | 履约状态查询 |
| 服务 | `FollowUpService` | 跟进状态管理 |
| 服务 | `TodayTaskService` | 今日待办 |
| 仓储 | `IFulfillmentRepository` | 配合 FulfillmentService |
| 仓储 | `IFollowUpRepository` | 配合 FollowUpService |

### 🟢 P2 - 可延后迁移（非核心或可替代）

| 类别 | 类名 | 原因 |
|------|------|------|
| 模型 | `BestVideo` | 可动态查询 |
| 服务 | `BackupService` | 可后续扩展 |
| 服务 | `CooperationStateMachine` | 逻辑已内嵌，可后续重构 |
| 服务 | `OperationLogger` 接口 | 可后续重构 |
| 异常 | 3个自定义异常 | 可用业务规则替代 |
| 工具 | 5个工具类 | 桌面应用相关，Web 版可忽略 |

---

## 七、文件清单统计

### 新项目 Domain 层文件统计

| 类别 | 文件数 | 说明 |
|------|--------|------|
| Models | 14 | 含 4 个 DTO |
| Services | 14 | 含接口 + 实现 |
| Repositories | 14 | 全部为接口 |
| Enums | 3 | CooperationStatus/ContactType/QualityLevel |
| **总计** | **45** | - |

### 原项目 Domain 层文件统计

| 类别 | 文件数 | 说明 |
|------|--------|------|
| Models | 18 | 含 4 个 DTO |
| Services | 24 | 含接口 + 实现 + 异常类 |
| Repositories | 16 | 全部为接口 |
| Filter | 3 | 筛选框架 |
| Utils | 5 | 工具类 |
| **总计** | **66** | - |

### 差距统计

| 类别 | 原项目 | 新项目 | 差距 |
|------|--------|--------|------|
| Models | 18 | 14 | -4 (BestVideo + 3个独立异常类不算模型) |
| Services | 24 | 14 | -10 |
| Repositories | 16 | 14 | -2 |
| Filter | 3 | 0 | -3 |
| Utils | 5 | 0 | -5 |
| **总计** | **66** | **42** | **-24** |

---

## 八、下一步行动建议

1. **立即**: 迁移筛选框架 (`FilterCriteria`, `FilterOperator`, `FilterLogicOperator`)
2. **本周**: 实现视图相关服务 (`FulfillmentService`, `FollowUpService`, `TodayTaskService`)
3. **后续**: 考虑重构状态机为独立类、添加操作日志接口抽象

---

*文档结束*
