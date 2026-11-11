# TKCompass 待办清单

> 创建日期：2026-11-08  
> 最后更新：2026-11-10  
> 当前状态：🟢 ViewModel 业务逻辑 100% 完成，XAML 视图绑定待完善

---

## 一、当前状态

### ✅ 已完成

- [x] Phase 0: 环境准备（.NET 10 + Avalonia 12.1.2 + EF Core 10）
- [x] Phase 1: 领域层骨架（16 个实体模型、5 个枚举、14 个仓储接口、25 个服务类）
- [x] Phase 2: 数据层骨架（14 个仓储实现、DbContext、实体配置）
- [x] Phase 3: 应用层骨架（18 个 View、19 个 ViewModel、MainWindow 导航）
- [x] 编译错误修复（命名空间对齐、类型转换、缓存清理）
- [x] **ViewModel 业务逻辑实现**（2026-11-10 完成）

### ✅ ViewModel 业务逻辑实现详情

| ViewModel | 状态 | 已实现功能摘要 |
|-----------|------|---------------|
| MainViewModel | ✅ | 页面导航（CurrentView）、10 个子 VM 管理、对话框（CreatorEdit/About/Settings）、状态栏、Ctrl+Z、Dispose |
| CreatorDashboardViewModel | ✅ | 6 Tab 分类加载、Messenger 刷新、7 字段筛选、批量操作（删除/恢复/已交付/提醒）、状态推进、统计标签 |
| FilterViewModel | ✅ | 7 筛选字段、ApplyFilterCommand、ResetCommand、事件触发 |
| CreatorDetailViewModel | ✅ | 达人/寄样/视频/联系方式数据加载、增删命令、履约统计计算 |
| CreatorEditViewModel | ✅ | 新增/编辑模式、表单验证、保存逻辑、RequestClose 事件 |
| ConfigViewModel | ✅ | 4 配置 Tab 增删管理（达人类型/产品代码/质量等级/联系渠道） |
| OperationHistoryViewModel | ✅ | 操作日志加载、单步/组撤销、Messenger 刷新 |
| ExcelImportViewModel | ✅ | 文件解析、预览、批量导入（达人→寄样→视频→联系方式）、冲突检测 |
| CrmViewModel | ✅ | 达人列表加载、等级筛选属性、Messenger 刷新 |
| TodayFollowUpViewModel | ✅ | 跟进项构建、分页、状态标记（Pending/Acknowledged/Completed）、分组统计 |

---

## 二、待办任务（按优先级）

### 🔴 P0: XAML 视图绑定与交互完善

#### 1. 达人工作台 XAML 绑定（CreatorDashboardView）

**当前状态**：ViewModel 业务逻辑 ✅ 完成，XAML 绑定待完善

**待完善**：
- [ ] DataGrid 列绑定（Handle/Nickname/QualityLevel/CooperationStatus 等）
- [ ] 6 个 Tab 的 ItemsSource 绑定（AllItems/FilteredItems/DeliveredItems 等）
- [ ] 筛选栏控件绑定（FilterViewModel 属性）
- [ ] 批量操作按钮 Command 绑定
- [ ] 统计标签 TextBlock 绑定
- [ ] Tab 切换时的 SelectedItem/CurrentTab 同步

**预估工时**：1-2 天

---

#### 2. 达人详情页 XAML 绑定（CreatorDetailView）

**当前状态**：ViewModel 业务逻辑 ✅ 完成，XAML 绑定待完善

**待完善**：
- [ ] 达人基本信息区域绑定
- [ ] 寄样折叠栏（Expander）ItemsSource 绑定
- [ ] 每个折叠栏内的视频列表 DataGrid 绑定
- [ ] 联系方式动态列表绑定（ItemsControl + 删除按钮）
- [ ] 新增寄样/视频按钮 Command 绑定

**预估工时**：1 天

---

#### 3. 达人编辑对话框 XAML 绑定（CreatorEditWindow）

**当前状态**：ViewModel 业务逻辑 ✅ 完成，XAML 绑定待完善

**待完善**：
- [ ] 表单字段 TextBox/ComboBox 绑定
- [ ] 验证错误提示（DataValidation）
- [ ] 保存/取消按钮 Command 绑定
- [ ] 对话框关闭行为（RequestClose → Window.Close）

**预估工时**：0.5 天

---

#### 4. 配置管理 XAML 绑定（ConfigView）

**当前状态**：ViewModel 业务逻辑 ✅ 完成，XAML 绑定待完善

**待完善**：
- [ ] 4 个 Tab 的 DataGrid ItemsSource 绑定
- [ ] 新增表单（TextBox + Button）绑定
- [ ] 删除按钮 Command 绑定（CommandParameter 传递选中项）

**预估工时**：0.5 天

---

### 🟡 P1: ViewModel TODO 项修复

#### 5. 产品代码多选筛选逻辑

**位置**：`CreatorDashboardViewModel.ApplyFilters()`

**待实现**：
- [ ] 当 `SelectedProductCodes` 有值时，筛选寄样 SKU 包含选中产品代码的达人
- [ ] 需关联 Shipment 或 Dictionary 数据

**预估工时**：0.5 天

---

#### 6. 产品名搜索筛选逻辑

**位置**：`CreatorDashboardViewModel.ApplyFilters()`

**待实现**：
- [ ] 当 `ProductNameSearch` 有值时，筛选寄样 product_name 包含关键词的达人

**预估工时**：0.5 天

---

#### 7. 达人详情页新增寄样/视频参数传入

**位置**：`CreatorDetailViewModel.AddShipmentAsync()` / `AddVideoAsync()`

**待实现**：
- [ ] 打开对话框获取快递号、产品代码、产品名、要求视频数等参数
- [ ] 打开对话框获取视频链接、关联寄样等参数
- [ ] 替代当前的硬编码/空值

**预估工时**：1 天

---

#### 8. CRM 视图筛选逻辑

**位置**：`CrmViewModel.FilterCreators()`

**待实现**：
- [ ] 按 SearchText 过滤 Handle/Nickname
- [ ] 按 SelectedLevel 过滤 QualityLevel
- [ ] 组合筛选条件

**预估工时**：0.5 天

---

#### 9. Excel 导入文件选择对话框

**位置**：`ExcelImportViewModel.SelectFileAsync()`

**待实现**：
- [ ] 集成 Avalonia OpenFileDialog
- [ ] 需要 Window 引用（通过 View 传入或使用 IDialogService）

**预估工时**：0.5 天

---

### 🟢 P2: UI 美化与体验优化

#### 
