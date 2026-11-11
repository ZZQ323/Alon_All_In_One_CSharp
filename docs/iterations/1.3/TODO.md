# 迭代 1.3 — 任务清单

**创建时间**: 2026-11-11  
**目标**: 重构达人工作台交互、补全配置/设置功能、完善左侧导航  
**状态**: 🔄 进行中

---

## 任务概览

| 优先级 | 任务数 | 完成数 | 完成率 |
|--------|--------|--------|--------|
| P1 | 10 | 0 | 0% |
| **总计** | **10** | **0** | **0%** |

---

## P1 核心任务

### 任务 1: 重构左侧导航栏（可折叠 + 选中态）

**目标**: 将左侧导航改为可折叠 Expander 结构，达人工作台展开后显示子项（今日提醒、正在交付、总表、已交付、已删除），选中项有高亮反馈  
**修改文件**:
- `TKCompass.App/Views/MainWindow.axaml`
- `TKCompass.App/ViewModels/MainViewModel.cs`

**验收标准**:
- [ ] 达人工作台改为 Expander，展开后显示 5 个子项
- [ ] 选中导航项有明显的视觉区分（背景色/边框变化）
- [ ] 移除"今日跟进"独立导航项
- [ ] 编译通过（0 错误）

**依赖**: 无  
**预估工时**: 0.5 天

---

### 任务 2: 重构 CreatorDashboardView 为 3 Tab 结构

**目标**: 将达人工作台从 6 Tab 精简为 3 Tab（今日提醒、正在交付、总表），移除独立的"已删除"Tab  
**修改文件**:
- `TKCompass.App/Views/CreatorDashboardView.axaml`
- `TKCompass.App/ViewModels/CreatorDashboardViewModel.cs`

**验收标准**:
- [ ] 仅保留 3 个 TabItem：今日提醒、正在交付、总表
- [ ] 移除"已删除"Tab（已删除数据通过左侧导航子项访问）
- [ ] 编译通过（0 错误）

**依赖**: 无  
**预估工时**: 0.5 天

---

### 任务 3: 实现"今日提醒" Tab 功能

**目标**: 今日提醒 Tab 显示今日需跟进的达人列表，支持状态流转操作（推进状态、标记已提醒、标记暂不合作）  
**修改文件**:
- `TKCompass.App/ViewModels/CreatorDashboardViewModel.cs`
- `TKCompass.App/Views/CreatorDashboardView.axaml`

**验收标准**:
- [ ] 列表按"最后联系时间"升序排序（最久未联系优先）
- [ ] 每行显示 3 个操作按钮：推进状态、标记已提醒、标记暂不合作
- [ ] 状态流转需校验必填项（如推进至"寄样"必须填写物流单号）
- [ ] 标记已提醒后从列表移除（隐藏至明日）
- [ ] 编译通过（0 错误）

**依赖**: 任务 2  
**预估工时**: 1 天

---

### 任务 4: 实现"正在交付" Tab 功能

**目标**: 正在交付 Tab 显示已同意合作但未完成交付的达人，支持批量提醒和状态流转  
**修改文件**:
- `TKCompass.App/ViewModels/CreatorDashboardViewModel.cs`
- `TKCompass.App/Views/CreatorDashboardView.axaml`

**验收标准**:
- [ ] 列表显示状态为"同意合作"和"寄样中"的达人
- [ ] 支持多选，提供"批量提醒"按钮
- [ ] 批量提醒生成独立日志（写入操作历史）
- [ ] 显示视频/广告码缺失状态（颜色指示：红=缺失，绿=完成）
- [ ] 编译通过（0 错误）

**依赖**: 任务 2  
**预估工时**: 1 天

---

### 任务 5: 实现"总表" Tab 功能

**目标**: 总表显示所有未删除的达人数据，支持筛选和状态流转  
**修改文件**:
- `TKCompass.App/ViewModels/CreatorDashboardViewModel.cs`
- `TKCompass.App/Views/CreatorDashboardView.axaml`

**验收标准**:
- [ ] 显示所有未删除达人（与"已删除"形成补集）
- [ ] 支持按状态筛选
- [ ] 删除操作为物理删除（数据库删除，不可恢复）
- [ ] 编译通过（0 错误）

**依赖**: 任务 2  
**预估工时**: 0.5 天

---

### 任务 6: 实现"已交付"和"已删除"子视图

**目标**: 通过左侧导航子项访问已交付和已删除的达人列表  
**修改文件**:
- `TKCompass.App/Views/CreatorDashboardView.axaml`
- `TKCompass.App/ViewModels/CreatorDashboardViewModel.cs`

**验收标准**:
- [ ] 左侧导航"已交付"显示所有已完成交付的达人
- [ ] 左侧导航"已删除"显示所有已删除的达人（物理删除后不可恢复）
- [ ] 已删除列表提供"彻底删除"按钮（物理删除）
- [ ] 编译通过（0 错误）

**依赖**: 任务 1, 任务 2  
**预估工时**: 0.5 天

---

### 任务 7: 实现新建达人弹窗

**目标**: 在达人工作台顶部提供"新建达人"按钮，点击后弹出弹窗填写达人信息  
**修改文件**:
- `TKCompass.App/Views/CreatorDashboardView.axaml`
- `TKCompass.App/ViewModels/CreatorDashboardViewModel.cs`
- `TKCompass.App/Views/CreateCreatorDialog.axaml` (新建)
- `TKCompass.App/ViewModels/CreateCreatorDialogViewModel.cs` (新建)

**验收标准**:
- [ ] 新建达人按钮位于 Tab 栏右侧（与筛选按钮同位置）
- [ ] 弹窗包含必填字段：昵称、平台账号、联系方式
- [ ] 提交后刷新当前 Tab 数据
- [ ] 编译通过（0 错误）

**依赖**: 任务 2  
**预估工时**: 0.5 天

---

### 任务 8: 重构配置管理为弹窗模式

**目标**: 配置管理改为顶部按钮，点击后弹出弹窗，支持新增/编辑配置项  
**修改文件**:
- `TKCompass.App/Views/ConfigView.axaml`
- `TKCompass.App/ViewModels/ConfigViewModel.cs`
- `TKCompass.App/Views/ConfigEditDialog.axaml` (新建)
- `TKCompass.App/ViewModels/ConfigEditDialogViewModel.cs` (新建)

**验收标准**:
- [ ] 移除页面内联输入框
- [ ] 每个配置 Tab 提供"新增"按钮，点击弹出编辑弹窗
- [ ] 弹窗支持新增和编辑模式
- [ ] 编译通过（0 错误）

**依赖**: 无  
**预估工时**: 1 天

---

### 任务 9: 补全设置页面（数据备份与恢复）

**目标**: 设置页面增加手动备份、数据恢复、备份历史记录功能  
**修改文件**:
- `TKCompass.App/Views/SettingsView.axaml`
- `TKCompass.App/ViewModels/SettingsViewModel.cs`

**验收标准**:
- [ ] 提供"手动备份"按钮，立即执行备份
- [ ] 提供"数据恢复"按钮，选择备份文件进行恢复
- [ ] 显示备份历史记录（时间、大小、备注）
- [ ] 编译通过（0 错误）

**依赖**: 无  
**预估工时**: 1 天

---

### 任务 10: 移除独立的"今日跟进"页面

**目标**: 删除 TodayFollowUpView 和 TodayFollowUpViewModel，相关功能已合并到达人工作台"今日提醒"Tab  
**修改文件**:
- `TKCompass.App/Views/TodayFollowUpView.axaml` (删除)
- `TKCompass.App/ViewModels/TodayFollowUpViewModel.cs` (删除)
- `TKCompass.App/ViewModels/MainViewModel.cs` (移除相关导航逻辑)

**验收标准**:
- [ ] 删除 TodayFollowUpView.axaml 和 TodayFollowUpViewModel.cs
- [ ] MainViewModel 移除 NavigateToTodayFollowUpCommand
- [ ] 编译通过（0 错误）

**依赖**: 任务 1, 任务 3  
**预估工时**: 0.5 天

---

## 任务依赖关系

```
任务 1（左侧导航） ──┐
                     ├─→ 任务 6（已交付/已删除视图）─→ 任务 10（移除今日跟进）
任务 2（3 Tab 结构）─┤
                     ├─→ 任务 3（今日提醒）─────────→ 任务 10
                     ├─→ 任务 4（正在交付）
                     ├─→ 任务 5（总表）
                     └─→ 任务 7（新建达人弹窗）

任务 8（配置管理弹窗）─→ 独立
任务 9（设置备份恢复）─→ 独立
```

---

## 执行批次建议

**批次 1（并行）**: 任务 1、任务 2、任务 8、任务 9  
**批次 2（并行）**: 任务 3、任务 4、任务 5、任务 7  
**批次 3**: 任务 6  
**批次 4**: 任务 10

---

## 关键设计决策

### 双状态体系

1. **达人状态**（业务状态机）：未触达 → 已触达 → 同意合作 → 寄样中 → 已交付
2. **显示状态**（UI 过滤）：今日提醒、正在交付、总表、已交付、已删除

### 状态流转时间自动填充

- **首次联系时间**：首次状态流转至"已触达"时自动填充
- **同意合作时间**：流转至"同意合作"时自动填充
- **寄样时间**：流转至"寄样中"时自动填充
- **完成时间**：流转至"已交付"时自动填充

### 删除策略

- **已删除 Tab**：逻辑删除（IsDeleted = true），可从"已删除"列表恢复
- **彻底删除**：物理删除（数据库 DELETE），不可恢复

### 批量提醒日志

- 批量提醒操作写入 `operation_logs` 表
- 日志类型：`BatchReminder`
- 日志内容：提醒的达人 ID 列表、提醒时间

---

## 风险与注意事项

1. **状态流转校验**：必须严格遵循状态机单向流转，禁止跳跃
2. **物理删除不可恢复**：已删除列表的"彻底删除"按钮需二次确认
3. **批量提醒性能**：大批量提醒时需异步执行，避免 UI 卡顿
4. **备份恢复安全**：恢复前需提示用户当前数据将被覆盖
