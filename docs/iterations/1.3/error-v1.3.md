# v1.3 修改记录

## 修改文件清单

### 1. CreatorDashboardViewModel.cs
**修改内容：**
- 提取 `UpdateStatistics(List<Creator> allCreators)` 方法，封装统计数字更新逻辑
- 在 `LoadDataAsync()` 末尾调用 `UpdateStatistics()`
- 在 `ApplyFilter()` 末尾调用 `UpdateStatistics()`
- 在 `ClearFilter()` 末尾调用 `UpdateStatistics()`

**目的：** 确保筛选操作后顶部统计数字联动更新

### 2. CreatorDashboardView.axaml.cs
**修改内容：**
- 实现 `IRecipient<OpenCreateCreatorDialogMessage>` 接口
- 在构造函数中订阅 `WeakReferenceMessenger.Default.Register<OpenCreateCreatorDialogMessage>(this)`
- 实现 `Receive()` 方法处理弹窗打开逻辑
- 删除旧的 `CreateCreatorButton_Click` 事件处理器

**目的：** 通过 Messenger 模式解耦弹窗打开逻辑，解决 DI Scoped 生命周期问题

### 3. FilterBar.axaml
**状态：** 已确认按钮位置正确（在 Border 内部靠右，使用 DockPanel.Dock="Right"）

## 编译结果

```
0 Error(s)
23 Warning(s)
```

**Warning 类型：**
- CS8604: 可能的 null 引用参数（MainViewModel.cs）
- CS0169: 未使用的字段（ConfigViewModel.cs）
- AVLN5001: TextBox.Watermark 已过时，建议使用 PlaceholderText（多个 View 文件）

## 注意事项

1. **统计联动逻辑**：`UpdateStatistics()` 方法现在在三个地方被调用，确保任何数据变化都会触发统计更新
2. **Messenger 订阅**：View 层使用 `WeakReferenceMessenger` 避免内存泄漏
3. **DI 解析**：弹窗打开时从 `App.Services` 解析 `ICreatorService` 和 `IMessenger`，确保使用 Scoped 实例
4. **向后兼容**：所有修改保持原有功能不变，仅修复绑定和联动问题
