# Data 层（Repository实现 + 数据库基础设施）差距分析

> 生成时间：2026-11-11  
> 对比范围：原项目 app-data（18个Java文件） vs 新项目 TKCompass.Data（C# / EF Core）

---

## 一、新项目 Data 层文件清单

### DbContext（1个）
| 文件 | 说明 |
|------|------|
| `TKCompassDbContext.cs` | EF Core 上下文，13个 DbSet |

### Repositories（14个）
| # | 文件 | 对应原项目 |
|---|------|-----------|
| 1 | `BaseRepository.cs` | 通用 CRUD 基类（新增抽象层） |
| 2 | `CreatorRepository.cs` | SqliteCreatorRepository |
| 3 | `ShipmentRepository.cs` | SqliteShipmentRepository |
| 4 | `VideoRepository.cs` | SqliteVideoRepository |
| 5 | `RelationshipRepository.cs` | SqliteRelationshipRepository |
| 6 | `OperationLogRepository.cs` | SqliteOperationLogRepository |
| 7 | `StatusFlowHistoryRepository.cs` | SqliteStatusFlowHistoryRepository |
| 8 | `BatchRemindLogRepository.cs` | SqliteBatchRemindLogRepository |
| 9 | `DictionaryRepository.cs` | SqliteDictionaryRepository |
| 10 | `ProductCodeRepository.cs` | SqliteProductCodeRepository |
| 11 | `CreatorTypeRepository.cs` | SqliteCreatorTypeRepository |
| 12 | `TagDefinitionRepository.cs` | SqliteTagDefinitionRepository |
| 13 | `CreatorTagRepository.cs` | SqliteCreatorTagRepository |
| 14 | `CreatorContactRepository.cs` | SqliteCreatorContactRepository |

### Configuration（8个）
| # | 文件 | 映射实体 |
|---|------|---------|
| 1 | `CreatorConfiguration.cs` | Creator |
| 2 | `CreatorContactConfiguration.cs` | CreatorContact |
| 3 | `DictionaryConfiguration.cs` | Dictionary |
| 4 | `OperationLogConfiguration.cs` | OperationLog |
| 5 | `RelationshipConfiguration.cs` | Relationship |
| 6 | `ShipmentConfiguration.cs` | Shipment |
| 7 | `StatusFlowHistoryConfiguration.cs` | StatusFlowHistory |
| 8 | `VideoConfiguration.cs` | Video |

---

## 二、已迁移（类名 + 状态）

| 原项目仓储 | 新项目实现 | 状态 | 备注 |
|-----------|-----------|------|------|
| SqliteCreatorRepository | `CreatorRepository` | ✅ 已迁移 | CRUD + 软删除 + 状态更新 + 恢复 |
| SqliteShipmentRepository | `ShipmentRepository` | ✅ 已迁移 | CRUD + 按达人查询 + 最新寄样 |
| SqliteVideoRepository | `VideoRepository` | ✅ 已迁移 | CRUD + 按达人/寄样查询 |
| SqliteRelationshipRepository | `RelationshipRepository` | ✅ 已迁移 | CRUD + 按达人查询 + Include导航 |
| SqliteOperationLogRepository | `OperationLogRepository` | ✅ 已迁移 | CRUD + 最近日志 + 按组查询 + 日志清理 |
| SqliteStatusFlowHistoryRepository | `StatusFlowHistoryRepository` | ✅ 已迁移 | CRUD + 按达人查询 + 按达人删除 |
| SqliteBatchRemindLogRepository | `BatchRemindLogRepository` | ✅ 已迁移 | CRUD + 最近日志 |
| SqliteDictionaryRepository | `DictionaryRepository` | ✅ 已迁移 | CRUD + 按类型查询 + 按类型删除 |
| SqliteProductCodeRepository | `ProductCodeRepository` | ✅ 已迁移 | CRUD + 按代码查询 |
| SqliteCreatorTypeRepository | `CreatorTypeRepository` | ✅ 已迁移 | CRUD + 按名称查询 |
| SqliteTagDefinitionRepository | `TagDefinitionRepository` | ✅ 已迁移 | CRUD |
| SqliteCreatorTagRepository | `CreatorTagRepository` | ✅ 已迁移 | CRUD + 按达人/标签查询 |
| SqliteCreatorContactRepository | `CreatorContactRepository` | ✅ 已迁移 | CRUD + 按达人查询 |

---

## 三、未迁移（功能 + 优先级）

| # | 缺失功能 | 原项目对应 | 优先级 | 说明 |
|---|---------|-----------|--------|------|
| 1 | **数据库视图 v_creator_dashboard** | DatabaseManager DDL | 🔴 高 | `CreatorRepository.GetDashboardDataAsync()` 返回空列表（TODO），需实现复杂 JOIN 统计查询 |
| 2 | **数据库视图 v_today_follow_up** | DatabaseManager DDL | 🔴 高 | 今日跟进视图未创建，`TodayFollowUpDto` 模型已存在但无仓储查询实现 |
| 3 | **数据库视图 v_creator_fulfillment** | DatabaseManager DDL | 🟡 中 | 履约状态视图未创建，`FulfillmentInfoDto` 模型已存在但无仓储查询实现 |
| 4 | **SqliteCreatorDashboardRepository** | 独立仓储类 | 🔴 高 | 仪表盘视图查询逻辑需整合到 `CreatorRepository` 或独立服务 |
| 5 | **SqliteFulfillmentRepository** | 独立仓储类 | 🟡 中 | 履约状态视图查询未实现 |
| 6 | **SqliteFollowUpRepository** | 独立仓储类 | 🔴 高 | 今日跟进视图查询未实现 |
| 7 | **数据库备份/恢复机制** | DatabaseManager | 🟡 中 | SQLite 文件级备份/恢复，EF Core 需自行实现或使用 SQLite 原生 API |
| 8 | **DDL 自动初始化** | DatabaseManager | 🟢 低 | EF Core 使用 Migrations 替代，但需确认已创建初始迁移 |
| 9 | **DatabaseManager 等价物** | 基础设施类 | 🟢 低 | 连接池管理、DDL 执行等由 EF Core + SQLite provider 替代 |
| 10 | **HibernateUtil 等价物** | 基础设施类 | ✅ 已替代 | `TKCompassDbContext` 替代 Hibernate 会话工厂 |

---

## 四、部分迁移（缺失内容）

### 4.1 CreatorRepository — 仪表盘查询缺失
- **现状**：`GetDashboardDataAsync()` 方法存在但返回空列表（TODO 注释）
- **缺失**：复杂的 JOIN 统计查询（达人数量、寄样统计、视频统计、状态分布等）
- **影响**：仪表盘页面无法展示汇总数据
- **建议**：实现 LINQ 查询或创建数据库视图映射

### 4.2 实体配置不完整（5个实体缺少 Configuration 文件）
| 实体 | 是否有 Configuration | 影响 |
|------|---------------------|------|
| BatchRemindLog | ❌ 缺失 | 依赖约定映射，无显式表名/索引定义 |
| CreatorTag | ❌ 缺失 | 依赖约定映射 |
| CreatorType | ❌ 缺失 | 依赖约定映射 |
| ProductCode | ❌ 缺失 | 依赖约定映射 |
| TagDefinition | ❌ 缺失 | 依赖约定映射 |

> 注：EF Core 约定映射可工作，但缺少显式配置可能导致表名不一致或索引缺失

### 4.3 DbContext 视图映射缺失
- **现状**：`TKCompassDbContext` 中 13 个 DbSet 全部映射到物理表
- **缺失**：无 `ToView()` 配置，无 `HasNoKey()` 的只读视图实体
- **影响**：无法利用数据库视图优化查询性能

### 4.4 EF Core Migrations 状态未知
- **现状**：未发现 Migrations 目录
- **缺失**：需确认是否已运行 `dotnet ef migrations add InitialCreate`
- **建议**：创建初始迁移以确保数据库 schema 正确

---

## 五、优先级总结

| 优先级 | 数量 | 关键项 |
|--------|------|--------|
| 🔴 高 | 4 | 仪表盘视图查询、今日跟进查询、Dashboard/FollowUp 仓储 |
| 🟡 中 | 3 | 履约状态视图、备份恢复、Fulfillment 仓储 |
| 🟢 低 | 2 | DDL 初始化、DatabaseManager 等价物 |
| ✅ 已替代 | 1 | HibernateUtil → DbContext |

---

## 六、迁移完成度评估

| 维度 | 原项目 | 新项目 | 完成度 |
|------|--------|--------|--------|
| 仓储实现数量 | 16 | 14 | 87.5% |
| 基础设施组件 | 2 (DatabaseManager, HibernateUtil) | 1 (DbContext) | 50% |
| 数据库视图 | 3 | 0 | 0% |
| 备份机制 | ✅ | ❌ | 0% |
| 实体配置 | N/A (JDBC) | 8/13 | 61.5% |
| **总体评估** | — | — | **~70%** |

---

## 七、建议行动计划

1. **P0（立即）**：实现 `GetDashboardDataAsync()` 的完整 LINQ 查询逻辑
2. **P0（立即）**：创建 `ITodayFollowUpRepository` 及实现，查询今日需跟进达人
3. **P1（本周）**：创建 `IFulfillmentRepository` 及实现，查询履约状态
4. **P1（本周）**：为 5 个缺失配置的实体补充 Configuration 文件
5. **P2（下周）**：实现数据库备份/恢复服务（使用 SQLite `Backup` API）
6. **P2（下周）**：确认 EF Core Migrations 已正确配置并生成初始迁移
7. **P3（后续）**：评估是否需要创建数据库视图以优化复杂查询性能
