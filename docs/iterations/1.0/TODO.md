# TKCompass 待办清单

> 创建日期：2026-11-08  
> 当前状态：启动框架已完成，业务功能待实现

---

## 一、当前状态

### ✅ 已完成（启动框架）

- [x] Phase 0: 环境准备（.NET 10 + Avalonia 12.1.2 + EF Core 10）
- [x] Phase 1: 领域层骨架（16 个实体模型、5 个枚举、14 个仓储接口、25 个服务类）
- [x] Phase 2: 数据层骨架（14 个仓储实现、DbContext、实体配置）
- [x] Phase 3: 应用层骨架（18 个 View、19 个 ViewModel、MainWindow 导航）
- [x] 编译错误修复（命名空间对齐、类型转换、缓存清理）

### 🟡 当前问题

所有 View/ViewModel 文件已创建，但大部分是**空壳**，业务逻辑未实现。

---

## 二、待办任务（按优先级）

### 🔴 P0: 核心业务功能

#### 1. 达人工作台（CreatorDashboardView）

**当前状态**：View 和 ViewModel 已创建，无业务逻辑

**待实现**：
- [ ] 6 个 Tab 数据加载（总表/已删除/已交付/已提醒/正在交付/今日提醒）
- [ ] DataGrid 绑定达人列表
- [ ] 批量操作（全选/取消全选/批量删除/批量提醒）
- [ ] 状态推进（一键推进合作状态）
- [ ] 统计标签更新（今日待处理/已提醒/已交付等）

**依赖**：CreatorService、ShipmentService、VideoService

**预估工时**：2-3 天

---

#### 2. 筛选系统（FilterBar）

**当前状态**：View 和 ViewModel 已创建，无筛选逻辑

**待实现**：
- [ ] 7 字段筛选（Handle/状态/等级/日期/产品代码等）
- [ ] 筛选条件组合
- [ ] 筛选结果实时刷新工作台
- [ ] 产品代码多选对话框集成

**依赖**：CreatorService、DictionaryService、ProductCodeService

**预估工时**：1-2 天

---

#### 3. 达人详情页（CreatorDetailView）

**当前状态**：View 和 ViewModel 已创建，无数据加载

**待实现**：
- [ ] 达人基本信息展示
- [ ] 寄样记录折叠栏（Expander）
- [ ] 视频记录列表
- [ ] 联系方式动态添加/删除
- [ ] 编辑按钮跳转到 CreatorEditWindow

**依赖**：CreatorService、ShipmentService、VideoService、CreatorContactService

**预估工时**：1-2 天

---

### 🟡 P1: 重要功能

#### 4. 达人编辑对话框（CreatorEditWindow）

**当前状态**：View 和 ViewModel 已创建，无保存逻辑

**待实现**：
- [ ] 新增/编辑模式切换
- [ ] 表单验证（Handle 唯一性检查）
- [ ] 保存到数据库
- [ ] 发布 CreatorUpdatedMessage 刷新工作台

**依赖**：CreatorService、Messenger

**预估工时**：1 天

---

#### 5. 寄样编辑对话框（ShipmentEditWindow）

**当前状态**：View 和 ViewModel 已创建，无保存逻辑

**待实现**：
- [ ] 寄样记录新增/编辑
- [ ] 日期选择（DatePicker）
- [ ] 保存到数据库
- [ ] 发布更新消息

**依赖**：ShipmentService、Messenger

**预估工时**：0.5 天

---

#### 6. 视频编辑对话框（VideoEditWindow）

**当前状态**：View 和 ViewModel 已创建，无保存逻辑

**待实现**：
- [ ] 视频记录新增/编辑
- [ ] 保存到数据库
- [ ] 发布更新消息

**依赖**：VideoService、Messenger

**预估工时**：0.5 天

---

#### 7. 配置管理（ConfigView）

**当前状态**：View 和 ViewModel 已创建，无数据加载

**待实现**：
- [ ] 4 个配置 Tab 数据加载（达人类型/产品代码/质量等级/联系方式渠道）
- [ ] 增删改操作
- [ ] 保存到数据库
- [ ] 发布配置更新消息

**依赖**：CreatorTypeService、ProductCodeService、DictionaryService

**预估工时**：1-2 天

---

#### 8. 撤销系统（OperationHistoryView）

**当前状态**：View 和 ViewModel 已创建，无撤销逻辑

**待实现**：
- [ ] 操作历史列表展示
- [ ] 单步撤销（UndoService）
- [ ] 批量撤销（按 group_id）
- [ ] Ctrl+Z 全局快捷键绑定
- [ ] 撤销后刷新工作台

**依赖**：UndoService、OperationLogService、Messenger

**预估工时**：1-2 天

---

### 🟢 P2: 次要功能

#### 9. Excel 导入（ExcelImportView）

**当前状态**：View 和 ViewModel 已创建，无导入逻辑

**待实现**：
- [ ] 文件选择对话框
- [ ] Excel 解析（ClosedXML）
- [ ] 数据预览
- [ ] 冲突检测（Handle 重复）
- [ ] 批量导入（达人 → 寄样 → 视频）
- [ ] 导入进度展示
- [ ] 发布批量更新消息

**依赖**：ExcelImportService、CreatorService、ShipmentService、VideoService

**预估工时**：2-3 天

---

#### 10. 今日跟进（TodayFollowUpView）

**当前状态**：View 和 ViewModel 已创建，无数据加载

**待实现**：
- [ ] 待跟进达人列表
- [ ] 跟进状态标记（已提醒/已解决）
- [ ] 快速操作按钮
- [ ] 统计标签更新

**依赖**：CreatorService、RelationshipService

**预估工时**：1 天

---

#### 11. CRM 视图（CrmView）

**当前状态**：View 和 ViewModel 已创建，无数据加载

**待实现**：
- [ ] 等级筛选（S/A/B 复选框）
- [ ] 达人列表展示
- [ ] 详情双栏布局

**依赖**：CreatorService

**预估工时**：0.5 天

---

#### 12. 设置视图（SettingsView）

**当前状态**：View 和 ViewModel 已创建，无功能实现

**待实现**：
- [ ] 数据库备份
- [ ] 数据库恢复
- [ ] 日志清理
- [ ] 系统信息展示

**依赖**：无

**预估工时**：0.5 天

---

#### 13. 多选对话框

**当前状态**：View 和 ViewModel 已创建，无选择逻辑

**待实现**：
- [ ] ProductMultiSelectView：产品代码多选
- [ ] CreatorTypeMultiSelectView：达人类型多选
- [ ] BatchRemindLogView：批量提醒日志展示

**依赖**：ProductCodeService、CreatorTypeService、BatchRemindLogService

**预估工时**：1 天

---

## 三、实现顺序建议

### 阶段 1：核心工作台（3-5 天）

1. **达人工作台**（P0-1）- 最核心功能
2. **筛选系统**（P0-2）- 工作台依赖
3. **达人详情页**（P0-3）- 查看详情

### 阶段 2：编辑功能（2-3 天）

4. **达人编辑对话框**（P1-4）
5. **寄样编辑对话框**（P1-5）
6. **视频编辑对话框**（P1-6）

### 阶段 3：配置与撤销（2-3 天）

7. **配置管理**（P1-7）
8. **撤销系统**（P1-8）

### 阶段 4：导入导出（2-3 天）

9. **Excel 导入**（P2-9）

### 阶段 5：辅助功能（2-3 天）

10. **今日跟进**（P2-10）
11. **CRM 视图**（P2-11）
12. **设置视图**（P2-12）
13. **多选对话框**（P2-13）

**总预估工时**：11-17 天

---

## 四、技术要点

### v-model 全局刷新机制

所有数据修改操作后必须发布 `CreatorUpdatedMessage`：

```csharp
// 在 ViewModel 中注入 IMessenger
private readonly IMessenger _messenger;

// 数据修改后发布消息
_messenger.Send(new CreatorUpdatedMessage());

// 在需要刷新的 ViewModel 中订阅
_messenger.Register<CreatorUpdatedMessage>(this, (r, m) => LoadData());
```

### 依赖注入

所有 Service 在 `App.axaml.cs` 中注册，ViewModel 通过构造函数注入：

```csharp
public class CreatorDashboardViewModel : ViewModelBase
{
    private readonly ICreatorService _creatorService;
    
    public CreatorDashboardViewModel(ICreatorService creatorService)
    {
        _creatorService = creatorService;
    }
}
```

### 数据加载模式

```csharp
[RelayCommand]
private async Task LoadDataAsync()
{
    IsLoading = true;
    try
    {
        var items = await _service.GetAllAsync();
        Items = new ObservableCollection<T>(items);
    }
    catch (Exception ex)
    {
        // 错误处理
    }
    finally
    {
        IsLoading = false;
    }
}
```

---

## 五、验收标准

每个功能完成后需要验证：

1. **编译通过**：`dotnet build TKCompass.slnx` 无错误
2. **运行正常**：`dotnet run --project src/TKCompass.App` 启动无异常
3. **功能可用**：手动测试核心功能
4. **数据刷新**：修改数据后，相关页面自动刷新（v-model 机制）
5. **错误处理**：异常情况有友好提示

---

## 六、下一步行动

**立即开始**：阶段 1 - 达人工作台

1. 打开 `CreatorDashboardViewModel.cs`
2. 实现 `LoadDataAsync()` 方法，加载达人列表
3. 绑定到 DataGrid
4. 实现 Tab 切换逻辑
5. 用户在 Windows 环境验证

---

*文档状态：待办清单已整理，准备开始实现业务功能*
