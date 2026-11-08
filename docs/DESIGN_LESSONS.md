# TKCompass 设计教训

> 记录开发过程中遇到的设计问题和解决方案，避免重复踩坑。

---

## 一、Repository 模式设计

### 教训 1：Service 层方法签名必须与 Repository 接口匹配

**问题**：`CreatorContactService.DeleteAsync(long contactId)` 直接传 `long` 给 `_repository.DeleteAsync(contactId)`，但 `IRepository<T>.DeleteAsync(T entity)` 期望实体对象。

**根因**：Service 层和 Repository 层的方法签名不一致，导致类型转换错误。

**解决方案**：
```csharp
// ❌ 错误：直接传 Id
public async Task DeleteAsync(long contactId)
{
    await _repository.DeleteAsync(contactId);  // 类型不匹配
}

// ✅ 正确：先查询实体，再删除
public async Task DeleteAsync(long contactId)
{
    var contact = await _repository.GetByIdAsync(contactId);
    if (contact != null)
    {
        await _repository.DeleteAsync(contact);
    }
}
```

**设计原则**：
1. Repository 接口定义统一的 CRUD 操作，接受实体对象而非 Id
2. Service 层如果需要接受 Id，应先查询实体再调用 Repository
3. 接口与实现必须同步更新，修改实现后必须更新接口定义

---

### 教训 2：返回类型必须与底层实现匹配

**问题**：`CreatorContactService.UpdateAsync` 返回 `Task<CreatorContact>`，但 `BaseRepository.UpdateAsync` 返回 `Task`（void）。

**根因**：Service 层期望返回更新后的实体，但 Repository 层没有返回。

**解决方案**：
```csharp
// ❌ 错误：返回类型不匹配
public async Task<CreatorContact> UpdateAsync(CreatorContact contact)
{
    return await _repository.UpdateAsync(contact);  // Repository 返回 void
}

// ✅ 正确：返回类型匹配
public async Task UpdateAsync(CreatorContact contact)
{
    await _repository.UpdateAsync(contact);
}
```

**设计原则**：
1. 如果不需要返回更新后的实体，Service 和 Repository 都返回 `Task`
2. 如果需要返回，Repository 应该在 `UpdateAsync` 后调用 `SaveChangesAsync` 并返回实体
3. 保持整个调用链的返回类型一致

---

## 二、Avalonia 项目结构

### 教训 3：命名空间必须与 XAML 的 x:Class 完全匹配

**问题**：`App.axaml` 中 `x:Class="TKCompass.App"`，但 `App.axaml.cs` 的命名空间是 `namespace TKCompass.AppBase`，导致 XAML 找不到对应的类。

**根因**：Avalonia 的 XAML 编译器会根据 `x:Class` 生成 partial class，如果命名空间不匹配会报"找不到类型"错误。

**解决方案**：
```csharp
// ❌ 错误：命名空间不匹配
namespace TKCompass.AppBase  // XAML 期望 TKCompass.App
{
    public partial class App : Application { }
}

// ✅ 正确：命名空间匹配
namespace TKCompass;  // 对应 x:Class="TKCompass.App"
{
    public partial class App : Application { }
}
```

**设计原则**：
1. `x:Class="Namespace.ClassName"` 要求 C# 代码中必须是 `namespace Namespace { class ClassName }`
2. 不要使用嵌套命名空间（如 `TKCompass.AppBase`），除非 XAML 中也写完整路径
3. 使用文件范围命名空间（`namespace TKCompass;`）可以简化代码

---

### 教训 4：删除旧项目时必须清理所有缓存

**问题**：删除 `TKCompass/` 旧项目目录后，VS 仍报"重复 App 类"错误。

**根因**：Visual Studio 的 `.vs` 目录和 `obj/bin` 缓存了旧项目的编译结果，即使删除源文件，缓存仍会干扰编译。

**解决方案**：
```powershell
# 完整清理步骤
cd D:\Projects\TKCompass

# 1. 关闭 Visual Studio
# 2. 删除旧项目目录
Remove-Item -Recurse -Force TKCompass

# 3. 清理所有 obj/bin
Get-ChildItem -Path src -Recurse -Directory -Include obj,bin | Remove-Item -Recurse -Force

# 4. 清理 .vs 目录（必须关闭 VS）
Remove-Item -Recurse -Force .vs

# 5. 重新打开解决方案
# 6. 重新生成：Ctrl + Shift + B
```

**设计原则**：
1. 项目重构后，必须删除不再使用的旧项目目录
2. 删除后必须清理 `obj`、`bin`、`.vs` 缓存
3. VS 运行时会锁定 `.vs` 目录，需先关闭 VS 再删除

---

## 三、Avalonia XAML 绑定

### 教训 5：相对绑定语法必须正确指向 DataContext

**问题**：`$parent[ItemsControl:3].DataContext.MarkAsAcknowledgedCommand` 报错"属性不存在于 FollowUpItem"。

**根因**：`$parent[ItemsControl:3]` 语法在 Avalonia 中无法正确解析，或者指向的 DataContext 是 `FollowUpGroup` 而非 `TodayFollowUpViewModel`。

**解决方案**：
```xml
<!-- ❌ 错误：相对绑定语法问题 -->
<Button Command="{Binding $parent[ItemsControl:3].DataContext.MarkAsAcknowledgedCommand}" />

<!-- ✅ 正确：直接绑定到 UserControl 的 DataContext -->
<Button Command="{Binding $parent[UserControl].DataContext.MarkAsAcknowledgedCommand}" />
```

**设计原则**：
1. 使用 `$parent[UserControl]` 直接访问 UserControl 的 DataContext（即 ViewModel）
2. 避免使用 `$parent[ItemsControl:N]` 这种不稳定的相对绑定
3. 如果绑定上下文复杂，考虑使用 `ElementName` 绑定或命名引用

---

## 四、NuGet 包版本管理

### 教训 6：NuGet 包版本必须与 Avalonia 主版本匹配

**问题**：`Avalonia.Controls.DataGrid` 版本 12.1.4 不存在，最接近版本是 12.1.2。

**根因**：Avalonia 生态系统的包版本必须与主包版本一致，不能随意升级。

**解决方案**：
```xml
<!-- ❌ 错误：版本不存在 -->
<PackageReference Include="Avalonia.Controls.DataGrid" Version="12.1.4" />

<!-- ✅ 正确：与 Avalonia 主版本匹配 -->
<PackageReference Include="Avalonia" Version="12.1.2" />
<PackageReference Include="Avalonia.Controls.DataGrid" Version="12.1.2" />
```

**设计原则**：
1. Avalonia 相关包的版本必须与主包版本一致
2. 升级 Avalonia 时，必须同步升级所有相关包
3. 使用 `dotnet list package` 检查包版本一致性

---

## 五、连锁反应错误处理

### 教训 7：从底层项目开始修复编译错误

**问题**：出现大量"找不到元数据文件 TKCompass.Domain.dll"错误。

**根因**：这是连锁反应错误。Domain 项目编译失败 → 没有输出 DLL → 依赖它的项目找不到 DLL。

**解决方案**：
1. 先修复最底层项目的错误（Domain → Data → App）
2. 不要单独修复"找不到元数据文件"错误，修复根因后会自动消失
3. 使用 `dotnet build` 从底层项目开始逐个编译，定位真正的错误

**设计原则**：
1. 多项目解决方案中，编译顺序是：Domain → Data → App
2. 底层项目编译失败会导致上层项目出现大量连锁错误
3. 错误列表中的前几个错误通常是根因，后面的错误是连锁反应

---

## 六、C# 与 Java 的差异

### 教训 8：C# 的强类型检查比 Java 更严格

**问题**：Java 中可以隐式转换的地方，C# 必须显式转换。

**示例**：
```csharp
// Java 可以这样写
repository.delete(contactId);  // 自动装箱

// C# 必须显式处理
await _repository.DeleteAsync(contact);  // 必须传实体对象
```

**设计原则**：
1. C# 是强类型语言，类型不匹配会立即报错
2. 不要依赖隐式转换，显式处理类型转换
3. 使用 IDE 的错误提示快速定位类型不匹配问题

---

## 七、调试技巧

### 教训 9：使用 dotnet build 而不是 VS 的错误列表

**问题**：VS 的错误列表可能显示过时的错误，或者缓存了旧的编译结果。

**解决方案**：
```powershell
# 清理并重新构建
dotnet clean
dotnet build TKCompass.slnx

# 查看详细的编译错误
dotnet build TKCompass.slnx --verbosity normal
```

**设计原则**：
1. VS 的错误列表可能不准确，使用 `dotnet build` 验证
2. 清理缓存后重新编译，避免旧错误干扰
3. 使用 `--verbosity normal` 查看详细的编译过程

---

## 八、总结

### 核心设计原则

1. **接口一致性**：Service 层和 Repository 层的方法签名必须匹配
2. **命名空间对齐**：XAML 的 `x:Class` 必须与 C# 的命名空间完全匹配
3. **缓存清理**：项目重构后必须清理所有缓存（obj/bin/.vs）
4. **版本一致性**：NuGet 包版本必须与主包版本匹配
5. **从底层修复**：多项目编译错误从底层项目开始修复
6. **显式类型转换**：C# 的强类型检查要求显式处理类型转换

### 常见错误快速修复

| 错误 | 根因 | 修复 |
|------|------|------|
| 无法从 `long` 转换为 `CreatorContact` | Repository 期望实体对象 | 先查询实体再删除 |
| 无法将 `void` 转换为 `CreatorContact` | 返回类型不匹配 | 修改返回类型为 `Task` |
| 命名空间已经包含 `App` 的定义 | 重复类定义 | 删除旧项目，清理缓存 |
| 找不到元数据文件 | 连锁反应错误 | 修复底层项目的根因 |
| 属性不存在于 `FollowUpItem` | 相对绑定语法错误 | 使用 `$parent[UserControl]` |
| 找不到版本为 12.1.4 的包 | 版本不存在 | 使用 12.1.2 |

---

*最后更新：2026-11-08*
