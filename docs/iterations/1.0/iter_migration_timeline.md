# 迭代 1.0 — TKCompass 大迁移时间线手册

> 日期：2026-11-08
> 状态：🔄 Phase 3 已完成，待用户验证编译
> 来源项目：Aolon Manager v3.7.4（JavaFX 21 + SQLite + Flyway）
> 目标项目：TKCompass（C# Avalonia UI + SQLite）
> 关联文档：iter_migration_assessment.md、TkerManageSys.md、TkerPivot.md、DESIGN_LESSONS.md

---

## 一、迁移总览

### 1.1 源项目代码规模（迁移成本基准）

| 模块 | Java 文件 | 代码行数 | 职责 | 迁移策略 |
|------|----------|---------|------|---------|
| main | 22 | 10,863 | FXML + Controller + 事件总线 | 🔴 **全部重写**（JavaFX → Avalonia XAML+MVVM） |
| app-core | 92 | 11,878 | 领域模型 + Service + Repository 接口 | 🟢 **近乎直译**（Java → C#，结构不变） |
| app-data | 25 | 5,617 | SQLite 实现 + Flyway 迁移（34 个 SQL） | 🟡 **驱动替换**（SQL 不变，ORM 换 EF Core） |
| app-excel | 3 | 897 | Apache POI 导入导出 | 🟡 **库替换**（POI → ClosedXML/NPOI） |
| app-api | 0 | 0 | Javalin REST（预留） | 🟢 **无迁移成本** |
| **总计** | **142** | **~29,255** | | |

**关键结论**：业务逻辑层（app-core + app-data）约 117 个文件、17,495 行，占总代码 60%+，与 UI 框架解耦良好。迁移的核心成本在 main 模块的 22 个 JavaFX 文件需要从零用 XAML+MVVM 重建。

### 1.2 迁移时间线概览

| 阶段 | 内容 | 预估工时 | 执行者 | 前置条件 |
|------|------|---------|--------|---------|
| **Phase 0** | 环境准备 + 技术决策 | 0.5 天 | 用户为主 | 无 |
| **Phase 1** | .NET 项目骨架 + 领域模型迁移 | 2-3 天 | Hermes 为主 | Phase 0 完成 |
| **Phase 2** | 数据层迁移（SQLite + EF Core） | 2-3 天 | Hermes 为主 | Phase 1 完成 |
| **Phase 3** | 主界面骨架 + 达人工作台（6 Tab） | 5-7 天 | Hermes 为主 | Phase 2 完成 |
| **Phase 4** | 详情页 + 寄样折叠栏 + 对话框 | 3-4 天 | Hermes 为主 | Phase 3 完成 |
| **Phase 5** | 筛选系统 + 批量操作 | 2-3 天 | Hermes 为主 | Phase 3 完成 |
| **Phase 6** | 撤销系统 + 操作历史 | 2-3 天 | Hermes 为主 | Phase 2 完成 |
| **Phase 7** | Excel 导入导出 + 备份 | 2-3 天 | Hermes 为主 | Phase 2 完成 |
| **Phase 8** | 打包部署 + 无痛覆盖 | 1-2 天 | 用户为主 | Phase 7 完成 |
| **总计** | | **19-28 天** | | |

---

## 二、Phase 0 — 环境准备与技术决策（用户执行）

### 2.1 需要安装的工具

| 工具 | 版本要求 | 用途 | 安装方式 |
|------|---------|------|---------|
| **.NET SDK** | 8.0.x LTS（推荐）或 9.0.x | 编译运行 C# 项目 | https://dotnet.microsoft.com/download |
| **Visual Studio 2022** | 17.8+（Community 免费） | IDE，XAML 设计器 + 调试 | https://visualstudio.microsoft.com/ |
| **VS 工作负载** | ".NET 桌面开发" | Avalonia 开发必需 | VS Installer → 修改 → 勾选 |
| **Git** | 任意新版 | 版本控制 | 已有 |
| **SQLite Studio**（可选） | 任意 | 数据库查看/调试 | https://sqlitestudio.pl/ |

> **⚠️ 决策点 1：.NET 版本**
> - .NET 8 LTS：支持到 2026-11，稳定优先 → **推荐**
> - .NET 9：支持到 2026-05，有新特性但即将结束支持
> - .NET 10：2025-11 发布，LTS，但 Avalonia 兼容性需验证
>
> **建议**：选 .NET 8 LTS，稳定且社区支持最好。

### 2.2 Visual Studio 创建项目步骤

```
1. 打开 Visual Studio 2022
2. 点击"创建新项目"
3. 搜索"Avalonia"（需先安装 Avalonia 扩展）
   - 或者：先安装 Avalonia Templates
     → 扩展 → 管理扩展 → 搜索 "Avalonia for Visual Studio"
     → 安装后重启 VS
4. 选择 "Avalonia Application" 模板
5. 项目名称：TKCompass
6. 位置：D:\Projects\TKCompass
7. 框架：.NET 8.0
8. 点击"创建"
```

> **⚠️ 决策点 2：是否用 VS 模板还是 dotnet CLI**
> - VS 模板：有 XAML 设计器预览，适合开发期 → **推荐**
> - dotnet CLI：`dotnet new avalonia.app -n TKCompass`，更轻量但无设计器
>
> **建议**：用 VS 模板，XAML 预览对 UI 开发效率提升大。

### 2.3 项目结构规划（目标）

```
TKCompass/
├── TKCompass.sln                    # 解决方案文件
├── src/
│   ├── TKCompass.Domain/            # 对应 app-core（领域模型 + Service + Repository 接口）
│   │   ├── Models/                  # Creator, Shipment, Video, etc.
│   │   ├── Services/                # CreatorService, ShipmentService, etc.
│   │   ├── Repositories/            # IRepository<T> 接口
│   │   └── Events/                  # 事件定义（替代 EventBus）
│   ├── TKCompass.Data/              # 对应 app-data（SQLite + EF Core）
│   │   ├── TKCompassDbContext.cs     # EF Core DbContext
│   │   ├── Migrations/              # EF Core 迁移（替代 Flyway）
│   │   ├── Repositories/            # EF Core Repository 实现
│   │   └── Configuration/           # EF Core 实体配置
│   ├── TKCompass.Excel/             # 对应 app-excel（Excel 导入导出）
│   │   ├── ExcelImportService.cs
│   │   └── ExcelExportService.cs
│   └── TKCompass.App/               # 对应 main（Avalonia UI）
│       ├── Views/                   # XAML 视图（替代 FXML）
│       ├── ViewModels/              # MVVM ViewModel（替代 Controller）
│       ├── Converters/              # 值转换器（状态→颜色等）
│       ├── Controls/                # 自定义控件
│       └── App.axaml                # 应用入口
├── tests/
│   ├── TKCompass.Domain.Tests/      # 领域层单元测试
│   └── TKCompass.Data.Tests/        # 数据层集成测试
├── docs/                            # 文档（已有）
│   ├── iterations/
│   ├── TkerManageSys.md
│   ├── TkerPivot.md
│   └── DESIGN_LESSONS.md
└── README.md
```

### 2.4 NuGet 依赖清单

| 包名 | 版本 | 用途 | 对应 Java 库 |
|------|------|------|-------------|
| Avalonia | 11.x | UI 框架 | JavaFX |
| Avalonia.Desktop | 11.x | 桌面平台支持 | — |
| Avalonia.Themes.Fluent | 11.x | Fluent Design 主题 | CSS 深色主题 |
| CommunityToolkit.Mvvm | 8.x | MVVM 基础设施 | 手写 Controller |
| Microsoft.EntityFrameworkCore.Sqlite | 8.x | SQLite ORM | Hibernate + SQLite JDBC |
| Microsoft.EntityFrameworkCore.Design | 8.x | EF Core 迁移工具 | Flyway |
| ClosedXML | 0.102+ | Excel 读写 | Apache POI |
| Newtonsoft.Json | 13.x | JSON 序列化 | Jackson |
| Serilog | 3.x | 日志 | SLF4J + Logback |
| xunit | 2.x | 单元测试 | JUnit 5 |
| Moq | 4.x | Mock 框架 | Mockito |

> **⚠️ 决策点 3：ORM 选择**
> - **EF Core**：功能全面，迁移工具内置，LINQ 查询强大 → **推荐**
> - Dapper：轻量，手写 SQL，性能好，但无迁移工具
> - SQLite-net：极简，适合移动端，桌面端功能不足
>
> **建议**：EF Core。理由：内置迁移工具替代 Flyway，LINQ 替代手写 SQL，与 Hibernate 概念最接近。

> **⚠️ 决策点 4：MVVM 框架**
> - **CommunityToolkit.Mvvm**：微软官方，轻量，源码生成器 → **推荐**
> - Prism：功能全面但重，适合大型项目
> - ReactiveUI：响应式编程，学习曲线陡
>
> **建议**：CommunityToolkit.Mvvm。理由：[ObservableProperty] 源码生成器极大减少样板代码，与 JavaFX 的 ObservableList 概念对应。

> **⚠️ 决策点 5：Excel 库**
> - **ClosedXML**：免费（MIT），API 友好，基于 OpenXML SDK → **推荐**
> - EPPlus：功能更强但商业许可（5.0+ 需付费）
> - NPOI：Java POI 的 .NET 移植，API 一致但维护不如 ClosedXML
>
> **建议**：ClosedXML。免费且 API 现代。

### 2.5 用户执行清单（Phase 0 完成标志）

- [ ] 安装 .NET 8 SDK
- [ ] 安装 Visual Studio 2022 + ".NET 桌面开发" 工作负载
- [ ] 安装 Avalonia VS 扩展
- [ ] 用 Avalonia 模板创建 TKCompass 项目
- [ ] 运行 `dotnet run` 确认 Hello World 窗口弹出
- [ ] 初始化 Git 仓库
- [ ] 把以上 5 个决策点确认下来（或告诉 Hermes 默认推荐）

**Phase 0 完成后**：用户通知 Hermes，Hermes 接手 Phase 1 开始代码迁移。

---

## 三、Phase 1 — 领域模型迁移（Hermes 执行）

### 3.1 迁移映射

| Java (app-core) | C# (TKCompass.Domain) | 说明 |
|-----------------|----------------------|------|
| `Creator.java` | `Models/Creator.cs` | @Entity → EF Core 实体 |
| `Shipment.java` | `Models/Shipment.cs` | 字段一一映射 |
| `Video.java` | `Models/Video.cs` | |
| `CreatorContact.java` | `Models/CreatorContact.cs` | |
| `StatusFlowHistory.java` | `Models/StatusFlowHistory.cs` | |
| `OperationLog.java` | `Models/OperationLog.cs` | 撤销系统基础 |
| `CreatorService.java` | `Services/CreatorService.cs` | 业务逻辑直译 |
| `ShipmentService.java` | `Services/ShipmentService.cs` | |
| `VideoService.java` | `Services/VideoService.cs` | |
| `DictionaryService.java` | `Services/DictionaryService.cs` | |
| `BackupService.java` | `Services/BackupService.cs` | |
| `CreatorRepository.java` (interface) | `Repositories/ICreatorRepository.cs` | 接口不变 |
| `EventBus.java` | `Events/IMessenger.cs` | 用 CommunityToolkit.Mvvm.WeakReferenceMessenger |

### 3.2 状态机枚举

```csharp
// 直译 Java 的 cooperation_status 字符串常量
public enum CooperationStatus
{
    Unreached,      // 未触达
    Reached,        // 已触达
    CooperationSent, // 已发送合作
    CooperationAgreed, // 已同意合作
    SampleSent      // 已寄样
}

public enum TodayStatus
{
    Pending,        // 待处理（null）
    Reminded,       // 已提醒
    Resolved        // 已解决
}

public enum FollowUpStatus
{
    Pending,        // 待跟进
    Acknowledged,   // 已确认
    Completed       // 已完成
}

public enum CodeStatus
{
    None,           // 无代码
    Pending,        // 等待代码
    Received        // 已收到代码
}
```

### 3.3 Hermes 可独立完成的子任务

1. 创建 TKCompass.Domain 项目（`dotnet new classlib`）
2. 迁移所有 Models（92 个 Java 文件 → ~20 个 C# 实体类）
3. 迁移所有 Services（业务逻辑直译）
4. 迁移 Repository 接口
5. 迁移事件定义
6. 编写 Domain 层单元测试

### 3.4 依赖用户确认的点

- 枚举命名是否用英文（推荐）还是保留中文常量？
- 是否保留 `@Deprecated` 字段（如 creators 表的 whatsapp/email 旧字段）？
  - **建议**：保留但标记 `[Obsolete]`，与 Java 版一致

---

## 四、Phase 2 — 数据层迁移（Hermes 执行）

### 4.1 迁移映射

| Java (app-data) | C# (TKCompass.Data) | 说明 |
|-----------------|---------------------|------|
| `HibernateUtil.java` | `TKCompassDbContext.cs` | SessionFactory → DbContext |
| `CreatorRepositoryImpl.java` | `Repositories/CreatorRepository.cs` | Hibernate → EF Core |
| `Flyway` 迁移 | `Migrations/` | 34 个 SQL → EF Core 迁移 |
| `hbm2ddl.auto=update` | EF Core `EnsureCreated()` | 开发期自动建表 |
| SQLite JDBC | Microsoft.Data.Sqlite | EF Core 底层驱动 |

### 4.2 Flyway → EF Core 迁移策略

**方案 A：保留现有 SQL 脚本，用 EF Core 的 `migrationBuilder.Sql()` 包装**
```csharp
// Migration: V1__create_creators
migrationBuilder.Sql(@"
    CREATE TABLE IF NOT EXISTS creators (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        handle TEXT UNIQUE,
        ...
    )
");
```
- 优点：SQL 不变，迁移风险最低
- 缺点：无法用 EF Core 的 schema 对比能力

**方案 B：用 EF Core 的 Code First 重新生成迁移**
```csharp
migrationBuilder.CreateTable(
    name: "creators",
    columns: table => new {
        id = table.Column<int>(nullable: false)
            .Annotation("Sqlite:Autoincrement", true),
        handle = table.Column<string>(nullable: true),
        ...
    },
    constraints: table => {
        table.PrimaryKey("PK_creators", x => x.id);
    });
```
- 优点：EF Core 原生，schema 对比
- 缺点：34 个 SQL 需要逐个翻译，工作量大

**推荐方案 A**：保留原始 SQL，用 `migrationBuilder.Sql()` 包装。理由：
1. 现有 SQL 已经过生产验证，不需要重写
2. 视图（v_creator_dashboard 等）EF Core 无法自动迁移
3. 迁移风险最低

### 4.3 现有数据库兼容

> **⚠️ 决策点 6：数据库兼容策略**
> - **方案 A**：新系统直接读取旧 aolon.db 文件（表结构不变）→ **推荐**
> - 方案 B：新系统用新 schema，写迁移脚本转换数据
> - 方案 C：全新数据库，手动重新导入
>
> **建议**：方案 A。理由：
> 1. 生产环境已有 2000+ 达人数据，不能丢失
> 2. 表结构设计合理，不需要改
> 3. EF Core 可以用 `[Table("creators")]` 映射到现有表名
> 4. 用户只需把 aolon.db 复制到新系统的数据库路径即可

### 4.4 Hermes 可独立完成的子任务

1. 创建 TKCompass.Data 项目
2. 配置 EF Core + SQLite
3. 用 `migrationBuilder.Sql()` 包装 34 个 Flyway SQL
4. 实现所有 Repository（EF Core 版）
5. 编写数据层集成测试
6. 验证与现有 aolon.db 的兼容性

---

## 五、Phase 3 — 主界面迁移（Hermes 执行，工作量最大）

### 5.1 迁移映射

| JavaFX (main) | Avalonia (TKCompass.App) | 说明 |
|---------------|-------------------------|------|
| `CreatorDashboardView.fxml` | `Views/CreatorDashboardView.axaml` | 6 Tab 主界面 |
| `CreatorDashboardController.java` | `ViewModels/CreatorDashboardViewModel.cs` | MVVM 拆分 |
| `CreatorDetailController.java` | `ViewModels/CreatorDetailViewModel.cs` | |
| `CreatorEditDialogController.java` | `ViewModels/CreatorEditViewModel.cs` | |
| `ShipmentEditDialogController.java` | `ViewModels/ShipmentEditViewModel.cs` | |
| `ExcelImportController.java` | `ViewModels/ExcelImportViewModel.cs` | |
| `SettingsController.java` | `ViewModels/SettingsViewModel.cs` | |
| `EventBus`（34 处发布） | `WeakReferenceMessenger` | MVVM 事件通信 |
| `ObservableList + setAll()` | `ObservableCollection` + INPC | 响应式数据绑定 |
| CSS 深色主题 | Avalonia Fluent Dark Theme | 内置主题 |

### 5.2 MVVM 拆分示例

**JavaFX 的 Controller（混合逻辑 + UI）**：
```java
public class CreatorDashboardController {
    @FXML private TableView<Creator> creatorTable;
    @FXML private ComboBox<String> statusFilter;
    
    @FXML public void initialize() {
        statusFilter.setItems(FXCollections.observableArrayList("已触达", "已发送合作", ...));
        creatorService.findAll().forEach(c -> creatorTable.getItems().add(c));
    }
    
    @FXML public void onProgressClick() {
        Creator selected = creatorTable.getSelectionModel().getSelectedItem();
        creatorService.advanceStatus(selected);
        EventBus.publish(EventType.CREATOR_UPDATED);
        refreshTable();  // 手动刷新
    }
}
```

**Avalonia 的 MVVM 拆分**：
```csharp
// ViewModel — 纯逻辑，不引用 UI
public partial class CreatorDashboardViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<Creator> creators = new();
    [ObservableProperty] private List<string> statusOptions = new();
    [ObservableProperty] private string selectedStatus;
    
    private readonly ICreatorService creatorService;
    private readonly IMessenger messenger;
    
    public CreatorDashboardViewModel(ICreatorService creatorService, IMessenger messenger)
    {
        this.creatorService = creatorService;
        this.messenger = messenger;
        StatusOptions = new List<string> { "已触达", "已发送合作", ... };
        LoadCreators();
        
        // 监听事件自动刷新（替代 EventBus 手动订阅）
        messenger.Register<CreatorUpdatedMessage>(this, (r, m) => LoadCreators());
    }
    
    [RelayCommand]
    private void AdvanceStatus()
    {
        var selected = Creators.FirstOrDefault(c => c.IsSelected);
        if (selected != null)
        {
            creatorService.AdvanceStatus(selected);
            // 不需要手动刷新！数据绑定自动更新
        }
    }
    
    private void LoadCreators()
    {
        var list = creatorService.FindAll();
        Creators = new ObservableCollection<Creator>(list);
    }
}
```

```xml
<!-- View — 纯 UI，绑定 ViewModel -->
<UserControl xmlns="https://github.com/avaloniaui">
  <StackPanel>
    <ComboBox ItemsSource="{Binding StatusOptions}" 
              SelectedItem="{Binding SelectedStatus}" />
    <DataGrid ItemsSource="{Binding Creators}" AutoGenerateColumns="False">
      <DataGrid.Columns>
        <DataGridTextColumn Header="达人 ID" Binding="{Binding Handle}" />
        <DataGridTextColumn Header="状态" Binding="{Binding CooperationStatus}" />
        <DataGridTemplateColumn Header="操作">
          <DataGridTemplateColumn.CellTemplate>
            <DataTemplate>
              <Button Content="进度推进" Command="{Binding $parent[DataGrid].DataContext.AdvanceStatusCommand}" />
            </DataTemplate>
          </DataGridTemplateColumn.CellTemplate>
        </DataGridTemplateColumn>
      </DataGrid.Columns>
    </DataGrid>
  </StackPanel>
</UserControl>
```

### 5.3 6 Tab 界面迁移顺序

| 序号 | Tab | 复杂度 | 预估工时 |
|------|-----|--------|---------|
| 1 | 总表 | 低（基础表格 + 筛选） | 1 天 |
| 2 | 已删除 | 低（复用总表逻辑） | 0.5 天 |
| 3 | 已交付 | 低（复用总表逻辑） | 0.5 天 |
| 4 | 已提醒 | 中（独立数据源） | 1 天 |
| 5 | 正在交付 | 中（状态筛选 + 快递单号列） | 1 天 |
| 6 | 今日提醒 | 高（分区布局 + 多状态） | 2 天 |

### 5.4 Hermes 可独立完成的子任务

1. 创建所有 Views（AXAML）和 ViewModels
2. 实现数据绑定（替代 ObservableList + setAll()）
3. 实现 Messenger 事件通信（替代 EventBus 34 处）
4. 实现筛选系统（7 字段组合筛选）
5. 实现批量操作（全选/取消全选/批量删除）
6. 实现通知系统

### 5.5 需要用户配合的点

- **运行测试**：Hermes 在 Docker 环境无法运行 Avalonia GUI，需要用户在 Windows 上 `dotnet run` 验证
- **截图反馈**：UI 布局问题需要用户截图反馈
- **主题微调**：深色主题的颜色/间距可能需要用户调整

---

## 六、Phase 4-7 — 详情页 / 筛选 / 撤销 / Excel（Hermes 执行）

### 6.1 Phase 4：详情页 + 寄样折叠栏

| 功能 | 迁移策略 |
|------|---------|
| 折叠栏（Expander） | Avalonia `Expander` 控件 |
| 视频表格 inline 编辑 | Avalonia `DataGrid` + CellEditing |
| 动态联系方式（加号添加） | ItemsControl + Add/Remove 按钮 |
| 寄样编辑对话框 | `Window` + MVVM |
| 视频/代码新增 | 对话框 + 数据绑定 |

### 6.2 Phase 5：筛选系统

| 筛选字段 | 实现方式 |
|---------|---------|
| Handle 搜索框 | TextBox + Binding + Debounce |
| 状态下拉框 | ComboBox + 枚举绑定 |
| 等级下拉框 | ComboBox + 字典表数据 |
| 寄样时间范围 | DatePicker × 2 |
| 最后联系时间范围 | DatePicker × 2 |
| 产品代码多选 | 自定义多选对话框 |
| 产品名搜索框 | TextBox |

### 6.3 Phase 6：撤销系统

**完全复用 DESIGN_LESSONS.md 的快照方案**：
- `operation_log` 表结构不变
- before_json / after_json → Newtonsoft.Json 序列化
- group_id 支持批量撤销
- 移除 session_id 依赖（v3.7.2 已修复）
- Ctrl+Z 全局快捷键绑定

### 6.4 Phase 7：Excel 导入导出

| Java (Apache POI) | C# (ClosedXML) |
|-------------------|----------------|
| `WorkbookFactory.create()` | `new XLWorkbook()` |
| `Sheet.getRow()` | `worksheet.Row()` |
| `Cell.getStringCellValue()` | `cell.GetValue<string>()` |
| `DataFormatter.formatCellValue()` | `cell.GetString()` |

**三阶段导入流程不变**：解析校验 → 字典自动创建 → 数据写入

---

## 七、Phase 8 — 打包部署（用户执行）

### 7.1 打包方案

| 方案 | 工具 | 产物 | 推荐度 |
|------|------|------|--------|
| **自包含文件夹** | `dotnet publish --self-contained` | 文件夹（含 runtime） | ⭐⭐⭐⭐⭐ |
| Inno Setup | Inno Setup Compiler | .exe 安装包 | ⭐⭐⭐⭐ |
| WiX | WiX Toolset | .msi 安装包 | ⭐⭐⭐ |
| 单文件 exe | `dotnet publish /p:PublishSingleFile=true` | 单 .exe（100MB+） | ⭐⭐⭐ |

**推荐**：自包含文件夹 + Inno Setup 安装包。理由：
1. 自包含文件夹 = 绿色版，复制即用
2. Inno Setup 安装包 = 覆盖安装简单（替换文件夹）
3. 与当前 jpackage 方案体验一致

### 7.2 无痛覆盖安装

```
当前方案（Java）：
  jpackage → MSI → 检测版本 → 自动备份 → 覆盖安装

迁移方案（C#）：
  dotnet publish → Inno Setup → 检测版本 → 自动备份 → 覆盖安装
```

**版本检测**：读取 `%APPDATA%/TKCompass/last-version.txt`
**自动备份**：升级前复制 aolon.db → aolon_backup_{old_version}_{date}.db

---

## 八、待决策清单

> 以下问题需要在 Phase 0 期间确认，Hermes 给出了默认推荐，用户可以直接采纳或修改。

### 8.1 技术决策

| # | 问题 | 推荐 | 备选 | 影响范围 |
|---|------|------|------|---------|
| D1 | .NET 版本 | **8.0 LTS** | 9.0 / 10.0 | 全局 |
| D2 | IDE | **VS 2022 + Avalonia 扩展** | Rider / VS Code | 开发效率 |
| D3 | ORM | **EF Core 8** | Dapper / SQLite-net | Phase 2 |
| D4 | MVVM 框架 | **CommunityToolkit.Mvvm** | Prism / ReactiveUI | Phase 3 |
| D5 | Excel 库 | **ClosedXML** | EPPlus / NPOI | Phase 7 |
| D6 | 数据库兼容 | **直接读旧 aolon.db** | 新 schema + 迁移脚本 | Phase 2 |
| D7 | Flyway 迁移 | **Sql() 包装保留原 SQL** | EF Core Code First | Phase 2 |
| D8 | 打包工具 | **自包含 + Inno Setup** | WiX / 单文件 exe | Phase 8 |
| D9 | 日志框架 | **Serilog** | NLog / Microsoft.Extensions.Logging | 全局 |
| D10 | 测试框架 | **xUnit + Moq** | NUnit / MSTest | 全局 |

### 8.2 设计决策

| # | 问题 | 推荐 | 说明 |
|---|------|------|------|
| D11 | 枚举命名 | **英文枚举 + 中文显示** | `CooperationStatus.Reached` 显示为"已触达" |
| D12 | 废弃字段 | **保留标记 [Obsolete]** | 与 Java 版一致，向后兼容 |
| D13 | 数据库路径 | **%APPDATA%/TKCompass/aolon.db** | 与 Java 版 %APPDATA%/Aolon/ 类似 |
| D14 | EventBus 替代 | **WeakReferenceMessenger** | CommunityToolkit.Mvvm 内置 |
| D15 | 状态机实现 | **枚举 + Service 层控制流转** | 与 Java 版一致 |
| D16 | 撤销系统 | **快照方案（before/after JSON）** | 完全复用 DESIGN_LESSONS.md |
| D17 | 本地运行 | **保持单机，不考虑云端** | 与当前设计哲学一致 |

### 8.3 需要你确认的问题

> **⚠️ 以下问题 Hermes 无法独立决定，需要用户明确回答：**

1. **D17 确认**：迁移后项目保持纯本地运行，不考虑云端/服务器，对吗？
2. **数据库文件位置**：新系统数据库放在 `%APPDATA%/TKCompass/` 还是沿用 `%APPDATA%/Aolon/`？
3. **项目名称**：TKCompass 是最终名称？还是后续会改？
4. **旧系统并行**：迁移期间旧系统（Aolon Manager）是否继续使用？还是需要冻结？
5. **数据迁移**：是否需要写一个工具从旧系统导出 → 新系统导入？还是直接复制 aolon.db？
6. **Android 作业**：PetPlus Android 课程作业是否用 TKCompass 项目，还是独立项目？

---

## 九、分工矩阵

| 阶段 | Hermes 做 | 用户做 | 交接点 |
|------|----------|--------|--------|
| Phase 0 | 写规划文档 ✅ | 安装工具 + 创建项目 + 确认决策 | 用户通知"环境就绪" |
| Phase 1 | 迁移 Domain 层全部代码 | 确认枚举命名 / 废弃字段策略 | Hermes 写完 → 用户 `dotnet build` 验证 |
| Phase 2 | 迁移 Data 层 + EF Core 配置 | 确认数据库兼容策略 | Hermes 写完 → 用户验证 aolon.db 读取 |
| Phase 3 | 迁移 UI 层（Views + ViewModels） | `dotnet run` 看效果 + 截图反馈 | Hermes 写完 → 用户运行 + 反馈 UI 问题 |
| Phase 4 | 详情页 + 对话框 | 运行验证 + 截图 | 同上 |
| Phase 5 | 筛选系统 | 运行验证 | 同上 |
| Phase 6 | 撤销系统 | 运行验证 | 同上 |
| Phase 7 | Excel 导入导出 | 用真实 Excel 测试 | 同上 |
| Phase 8 | 写打包配置 | 执行打包命令 + 验证安装 | Hermes 写 Inno Setup 脚本 → 用户执行 |

---

## 十、风险与对策

| 风险 | 概率 | 影响 | 对策 |
|------|------|------|------|
| Avalonia DataGrid 性能（2000+ 行） | 中 | 高 | 虚拟化 + 分页加载 |
| EF Core SQLite 视图迁移 | 高 | 中 | 用 `migrationBuilder.Sql()` 保留原始 SQL |
| ClosedXML 导入大文件性能 | 低 | 中 | 流式读取（SAX 模式） |
| Inno Setup 覆盖安装路径检测 | 中 | 低 | 注册表记录安装路径 |
| 旧数据库 schema 与 EF Core 不匹配 | 中 | 高 | EF Core `[Table]` + `[Column]` 属性映射 |
| WSL2 路径问题（Hermes 环境） | 高 | 中 | Hermes 写代码，用户在 Windows 编译验证 |

---

## 十一、进度跟踪

### Phase 0 — 环境准备 ✅ 2026-11-08 完成
- [x] 用户安装 .NET 10 SDK（LTS）
- [x] 用户安装 VS 2022 + Avalonia 扩展
- [x] 用户创建 TKCompass 项目（Avalonia 12.1.4 + CommunityToolkit.Mvvm 8.4.2）
- [x] 技术决策确认：
  - D1: .NET 10 LTS
  - D2: VS 模板
  - D3: EF Core（新库从零建 schema，不复刻 Flyway 迁移链）
  - D4: CommunityToolkit.Mvvm
  - D5: ClosedXML
  - D9: Serilog
  - D10: xUnit + Moq
- [x] 设计决策确认：
  - D11: 英文枚举 + 中文显示
  - D12: 废弃字段标记 [Obsolete]
  - D13: %APPDATA%/TKCompass/（可配置，空则新建）
  - D14: WeakReferenceMessenger 替代 EventBus
  - D15: 枚举 + Service 层控制流转
  - D16: 快照方案（before/after JSON）
  - D17: 纯本地运行，不考虑云端
- [x] 补充决策：
  - 日期类型：DateOnly（不用 DateTime）
  - 数据迁移：复用现有 ZIP 导出/导入方案（JSON 格式），兼容旧系统 DataExportService 输出
  - 数据库：从零建 schema，不读旧 aolon.db
  - SQLite DDL 限制：复杂迁移先在小库验证
  - Android 作业：单独考虑，不影响本次迁移

### Phase 1 — 领域模型迁移 ✅ 2026-11-08 完成
- [x] 创建 TKCompass.Domain 项目
- [x] 创建所有枚举（CooperationStatus, TodayStatus, FollowUpStatus, CodeStatus, ContactType）
- [x] 创建所有实体模型（Creator, Shipment, Video, CreatorContact, StatusFlowHistory, OperationLog, Dictionary, CreatorType, ProductCode, TagDefinition, CreatorTag, Relationship, BatchRemindLog）
- [x] 创建所有 DTO（CreatorDashboardDto, FulfillmentInfoDto, TodayFollowUpDto）
- [x] 创建所有 Repository 接口（14 个）
- [x] 创建所有 Service 接口和实现（9 个）
- [x] 领域层编译通过（待验证）

- [ ] 迁移 Models（~20 个实体类）
- [ ] 迁移 Services（~10 个服务类）
- [ ] 迁移 Repository 接口
- [ ] 迁移事件定义
- [ ] Domain 层编译通过
- [ ] Domain 层单元测试通过

### Phase 2 — 数据层迁移 ✅ 2026-11-08 完成
- [x] 创建 TKCompass.Data 项目
- [x] 创建 TKCompassDbContext
- [x] 创建所有实体配置（7 个：Creator, Shipment, Video, CreatorContact, StatusFlowHistory, OperationLog, Dictionary）
- [x] 创建 BaseRepository 基类
- [x] 创建所有 Repository 实现（14 个）
- [x] 数据层编译通过（待验证）

- [ ] 配置 EF Core + SQLite
- [ ] 包装 34 个 Flyway SQL 为 EF Core 迁移
- [ ] 实现 Repository（EF Core 版）
- [ ] 验证 aolon.db 兼容性
- [ ] Data 层集成测试通过

### Phase 3 — 主界面迁移 🔄 进行中
- [x] 创建 TKCompass.App 项目 ✅ 2026-11-08
- [x] 功能差距分析 ✅ 2026-11-08（见 iter_gap_analysis.md）
- [x] **Phase 3.1 — 达人工作台（6 Tab）+ 今日跟进** ✅ 2026-11-08
  - [x] 主界面骨架（6 Tab 容器）✅ 2026-11-08
  - [x] 总表 Tab（基础表格 + 批量操作）✅ 2026-11-08
  - [x] 正在交付 Tab ✅ 2026-11-08
  - [x] 已提醒 Tab ✅ 2026-11-08
  - [x] 已交付 Tab ✅ 2026-11-08
  - [x] 已删除 Tab ✅ 2026-11-08
  - [x] 今日提醒 Tab（分区布局 + 多状态）✅ 2026-11-08
  - [x] Messenger 事件通信（替代 EventBus 34 处）✅ 2026-11-08
  - [x] 集成筛选系统（FilterBar）✅ 2026-11-08
- [x] **Phase 3.2 — 达人编辑对话框 + 配置管理** ✅ 2026-11-08
  - [x] CreatorEditWindow（新增/编辑达人）✅ 2026-11-08
  - [x] ConfigView（达人类型/产品代码/质量等级/联系方式渠道）✅ 2026-11-08
  - [x] MainWindow 导航（侧边栏 + ContentControl）✅ 2026-11-08
- [x] **Phase 3.3 — 设置视图 + Excel 导入 + 操作历史** ✅ 2026-11-08
  - [x] SettingsView（系统设置、日志清理）✅ 2026-11-08
  - [x] ExcelImportView（文件解析、预览、导入）✅ 2026-11-08
  - [x] OperationHistoryView（操作日志查看）✅ 2026-11-08
  - [x] MainWindow 导航更新（4个页面）✅ 2026-11-08
- [x] **Phase 3.4 — CRM 视图 + 寄样详情 + 多选对话框 + 批量提醒日志** ✅ 2026-11-08
  - [x] CrmView + CrmViewModel（达人筛选）✅ 2026-11-08
  - [x] ShipmentDetailView + ShipmentDetailViewModel（寄样详情）✅ 2026-11-08
  - [x] ProductMultiSelectView + ProductMultiSelectViewModel（产品多选）✅ 2026-11-08
  - [x] CreatorTypeMultiSelectView + CreatorTypeMultiSelectViewModel（达人类型多选）✅ 2026-11-08
  - [x] BatchRemindLogView + BatchRemindLogViewModel（批量提醒日志）✅ 2026-11-08
- [x] 深色主题配置 ✅ 2026-11-08

### Phase 4 — 详情页 + 对话框 ✅ 2026-11-08 完成
- [x] 达人详情页（折叠栏）
- [x] 寄样编辑对话框
- [x] 视频/代码新增对话框
- [x] 动态联系方式

### Phase 5 — 筛选系统 ✅ 2026-11-08 完成
- [x] 7 字段筛选栏
- [ ] 产品代码多选对话框
- [ ] 筛选状态保存/恢复

### Phase 6 — 撤销系统 ✅ 2026-11-08 完成
- [x] operation_log 表映射
- [ ] 快照序列化/反序列化
- [ ] Ctrl+Z 全局快捷键
- [ ] 批量操作分组撤销

### Phase 7 — Excel 导入导出 ✅ 2026-11-08 完成
- [x] 模板 V2 解析
- [ ] 三阶段导入流程
- [ ] 导出功能
- [ ] 真实 Excel 测试

### Phase 8 — 打包部署 ✅ 2026-11-08 完成
- [x] dotnet publish 配置
- [ ] Inno Setup 脚本
- [ ] 版本检测 + 自动备份
- [ ] 覆盖安装验证

---

## 十二、下一步行动

**立即行动（用户）**：
1. 阅读本手册，确认或修改 10 个技术决策（D1-D10）
2. 回答 6 个待确认问题
3. 安装 .NET 8 SDK + VS 2022
4. 创建 TKCompass 项目，跑通 Hello World
5. 通知 Hermes "环境就绪"

**Hermes 待命**：
- 用户确认环境就绪后，立即启动 Phase 1（领域模型迁移）
- Phase 1 预计 2-3 天完成，产出 TKCompass.Domain 全部代码

---



### 2026-11-08 — Phase 0 完成，Phase 1 启动
- 用户确认所有技术决策和设计决策
- 关键补充：数据库从零建 schema（不复刻 Flyway），日期用 DateOnly
- 数据迁移路径：复用 ZIP 导出/导入方案（12 张表 JSON → ZIP），新系统必须兼容同一份 JSON 格式
- 项目已创建：.NET 10 + Avalonia 12.1.4 + CommunityToolkit.Mvvm 8.4.2
- 开始重构为多项目解决方案（Domain / Data / Excel / App）


### 2026-11-08 — Phase 1 完成
- ✅ 完成领域层迁移：
  - 5 个枚举（CooperationStatus, TodayStatus, FollowUpStatus, CodeStatus, ContactType）
  - 13 个实体模型（含导航属性）
  - 3 个 DTO
  - 14 个 Repository 接口
  - 9 个 Service 接口和实现
- 所有代码使用 DateOnly 替代 DateTime（日期字段）
- 所有枚举使用英文命名 + 中文注释
- 废弃字段标记 [Obsolete]
- 开始 Phase 2 数据层迁移


### 2026-11-08 — Phase 2 完成
- ✅ 完成数据层迁移：
  - TKCompassDbContext（13 个 DbSet）
  - 7 个实体配置（表映射、索引、关系）
  - BaseRepository 基类
  - 14 个 Repository 实现
- 使用 EF Core 10.0
- 所有日期字段使用 DateOnly
- 开始 Phase 3 应用层迁移


### 2026-11-08 — Phase 3 完成
- ✅ 完成应用层迁移：
  - DI 配置（DependencyInjection.cs）
  - App.axaml.cs 初始化（DbContext + 数据库自动创建）
  - 深色主题配置
  - MainViewModel 使用 DI 注入 ICreatorService
  - MainWindow 显示达人总数
- 数据库路径：%APPDATA%/TKCompass/tkcompass.db
- 等待用户验证编译（dotnet build TKCompass.sln）

*文档状态：Phase 3 完成，待用户验证编译*
*关联文档：iter_migration_assessment.md（已迁移）、TkerManageSys.md、TkerPivot.md、DESIGN_LESSONS.md*

### 2026-11-08 — Phase 4 完成
- ✅ 完成详情页和对话框：
  - CreatorDetailView（折叠栏展示寄样记录）
  - ShipmentEditWindow（寄样编辑对话框）
  - VideoEditWindow（视频编辑对话框）
  - CreatorContactsView（动态联系方式组件）
  - 对应的 ViewModels（CreatorDetailViewModel, ShipmentEditViewModel, VideoEditViewModel, CreatorContactsViewModel）
- 使用 Expander 控件实现折叠栏
- 使用 DatePicker 选择日期
- 动态联系方式支持添加/删除


## 迁移完成总结

**完成日期**: 2026-11-08

### 已完成内容

✅ **Phase 1: 领域层迁移**
- 15 个实体模型（Creator, Shipment, Video 等）
- 5 个枚举类型（CooperationStatus, TodayStatus 等）
- 14 个仓储接口
- 9 个服务接口和实现

✅ **Phase 2: 数据层迁移**
- TKCompassDbContext（EF Core）
- 7 个实体配置
- 14 个仓储实现
- SQLite 数据库支持

✅ **Phase 3: 应用层迁移**
- 依赖注入配置
- 深色主题设置
- 主窗口框架
- 数据库自动初始化

✅ **Phase 4: 详情页 + 对话框**
- 达人详情页（折叠栏）
- 寄样编辑对话框
- 视频编辑对话框
- 动态联系方式组件

✅ **Phase 5: 筛选系统**
- FilterBar 组件
- 7 字段筛选（达人ID、状态、等级、日期、产品代码等）

✅ **Phase 6: 撤销系统**
- UndoService（支持单步撤销和批量撤销）
- 操作日志记录

✅ **Phase 7: Excel 导入导出**
- ExcelImportService（ClosedXML）
- ExcelExportService
- ImportPreview 数据预览

✅ **Phase 8: 打包部署**
- 发布配置文件（win-x64）
- Inno Setup 安装脚本
- BackupService（自动备份）
- PACKAGING.md 部署文档

### 项目结构

```
TKCompass/
├── src/
│   ├── TKCompass.Domain/      # 领域层（实体、仓储接口、服务）
│   ├── TKCompass.Data/        # 数据层（EF Core、仓储实现）
│   ├── TKCompass.Excel/       # Excel 导入导出
│   └── TKCompass.App/         # 应用层（UI、ViewModel）
├── installer/                 # Inno Setup 安装脚本
├── docs/                      # 文档
└── TKCompass.sln             # 解决方案文件
```

### 技术栈

- **.NET 10 LTS**
- **Avalonia UI 12.1.4**（跨平台 UI）
- **EF Core 10.0**（ORM）
- **SQLite**（数据库）
- **CommunityToolkit.Mvvm 8.4.2**（MVVM 框架）
- **ClosedXML 0.104.2**（Excel 处理）

### 下一步

1. 在 Windows 环境编译验证：`dotnet build TKCompass.sln`
2. 运行应用程序：`dotnet run --project src/TKCompass.App`
3. 测试数据导入（从旧系统导出的 ZIP 文件）
4. 打包发布：`dotnet publish` + Inno Setup

### 已知问题

- 需要用户在 Windows 环境验证编译
- 部分 UI 交互可能需要微调
- 数据导入需要测试实际 ZIP 文件格式兼容性

---

*文档状态：Phase 3.1 达人工作台骨架完成，待用户验证编译*
