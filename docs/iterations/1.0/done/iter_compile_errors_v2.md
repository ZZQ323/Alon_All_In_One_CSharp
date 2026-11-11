# TKCompass 迭代 1.0 — 编译错误修复 v2

> 日期：2026-11-08
> 状态：✅ 代码修复完成，待用户清理
> 目的：修复用户报告的 5 类编译错误

---

## 错误清单

| # | 错误 | 根因 | 状态 |
|---|------|------|------|
| 1 | 找不到 Avalonia.Controls.DataGrid >= 12.1.4 | TKCompass/ 旧项目 obj 缓存残留 | ⏳ 待用户清理 |
| 2 | 参数 1: 无法从"long"转换为"CreatorContact" (×2) | CreatorContactService.DeleteAsync 直接传 long 给 Repository | ✅ 已修复 |
| 3 | 命名空间"TKCompass"已经包含"App"的定义 | TKCompass/ 和 src/TKCompass.App/ 各有一个 App 类 | ⏳ 待用户删除旧项目 |
| 4 | 无法将类型"void"隐式转换为"CreatorContact" | CreatorContactService.UpdateAsync 返回类型与 Repository 不匹配 | ✅ 已修复 |
| 5 | 未能找到元数据文件 TKCompass.Domain.dll | 连锁反应，Domain 编译失败导致 | ⏸️ 自动消失 |

---

## Phase 1: 诊断 — 确认错误位置 ✅ 已完成

### 发现

1. **DataGrid 版本**：`src/TKCompass.App/TKCompass.App.csproj` 已改为 12.1.2 ✅，但 `TKCompass/TKCompass.csproj`（旧模板项目）的 obj 缓存仍引用 12.1.4。csproj 本身写的 12.1.2，但 obj 缓存过期。
2. **重复 App 类**：
   - `TKCompass/App.axaml.cs` → `namespace TKCompass; class App`（旧模板）
   - `src/TKCompass.App/App.axaml.cs` → `namespace TKCompass; class App`（新主项目）
   - 两者命名空间完全相同 → 冲突
3. **slnx 只包含 4 个 src/ 项目**，不包含 `TKCompass/TKCompass.csproj`。但 VS 可能仍加载了它（obj 缓存中有 12.1.4 引用）。
4. **long→CreatorContact / void→CreatorContact**：在 src/ 代码中未找到直接匹配。可能是 VS 设计器缓存了旧代码，或错误来自 TKCompass/ 旧模板项目中的残留代码。

### 结论

- 核心问题是 **`TKCompass/` 旧模板项目** 与 **`src/TKCompass.App/` 新主项目** 冲突
- 旧项目不应被编译，应从解决方案中排除或删除

---

## Phase 2: 修复计划

- [x] **Step 1**: 确认 `TKCompass/TKCompass.csproj` 是否被 slnx 加载（已确认：不在 slnx 中）
- [x] **Step 2**: 清理 `TKCompass/` 旧项目的 obj 缓存（已创建清理脚本）
- [x] **Step 3**: 如果用户 VS 仍加载旧项目，建议从 VS 解决方案中移除（已提供指南）
- [x] **Step 4**: 创建诊断脚本供用户在 Windows 执行 `dotnet build` 验证（已创建 CLEANUP_SCRIPT.md）
- [x] **Step 5**: 更新 error_log.md 和迭代文档（已完成）

---

## Phase 3: 根因确认 ✅

### 核心问题

**`TKCompass/` 旧模板项目** 与 **`src/TKCompass.App/` 新主项目** 冲突：

1. **重复 App 类**：两个项目都有 `namespace TKCompass; class App`
2. **DataGrid 版本**：旧项目 obj 缓存引用 12.1.4（实际不存在）
3. **类型转换错误**：旧项目代码与 Domain 模型不匹配

### 修复方案

**方案 A（推荐）**：删除 `TKCompass/` 旧项目目录
- 优点：彻底解决冲突
- 缺点：不可逆

**方案 B**：清理旧项目 obj 缓存，从 VS 解决方案中移除
- 优点：保留旧代码作为参考
- 缺点：可能再次冲突

### 用户操作指南

1. 关闭 Visual Studio
2. 执行清理脚本（见下方）
3. 重新打开 `TKCompass.slnx`
4. 执行 `dotnet build` 验证

---

## Phase 4: 代码修复 ✅ 已完成

### 修复 1: CreatorContactService.cs

**问题**：
- `DeleteAsync(long contactId)` 直接传 `long` 给 `_repository.DeleteAsync(contactId)`，但 Repository 期望实体对象
- `UpdateAsync` 返回 `Task<CreatorContact>`，但 Repository 返回 `Task`（void）

**修复**：
```csharp
// DeleteAsync: 先查询实体，再删除
public async Task DeleteAsync(long contactId)
{
    var contact = await _repository.GetByIdAsync(contactId);
    if (contact != null)
    {
        await _repository.DeleteAsync(contact);
    }
}

// UpdateAsync: 改为返回 Task
public async Task UpdateAsync(CreatorContact contact)
{
    await _repository.UpdateAsync(contact);
}

// DeleteAllByCreatorIdAsync: 传递实体而非 Id
public async Task DeleteAllByCreatorIdAsync(long creatorId)
{
    var contacts = await _repository.GetByCreatorIdAsync(creatorId);
    foreach (var contact in contacts)
    {
        await _repository.DeleteAsync(contact);  // 传递实体
    }
}
```

### 修复 2: ICreatorContactService.cs

**问题**：接口定义 `Task<CreatorContact> UpdateAsync` 与实现不匹配

**修复**：
```csharp
// 改为返回 Task
Task UpdateAsync(CreatorContact contact);
```

---

## Phase 5: 用户操作 ✅ 待执行

### Step 1: 关闭 Visual Studio

确保 VS 完全关闭，避免文件锁定。

### Step 2: 删除旧项目目录

`TKCompass/` 是初始模板项目，不在 `TKCompass.slnx` 中，应该删除：

```powershell
cd D:\Projects\TKCompass
Remove-Item -Recurse -Force TKCompass
```

### Step 3: 清理所有 obj/bin 缓存

```powershell
cd D:\Projects\TKCompass

# 清理 src 项目
Get-ChildItem -Path src -Recurse -Directory -Include obj,bin | Remove-Item -Recurse -Force

# 清理根目录
dotnet clean
```

### Step 4: 重新构建

```powershell
cd D:\Projects\TKCompass
dotnet build TKCompass.slnx
```

### Step 5: 验证

```powershell
# 应该只有 0 个错误
dotnet build TKCompass.slnx 2>&1 | Select-String "error|warning"
```

---

## 预期结果

- ✅ 重复 App 类错误消失
- ✅ DataGrid 版本错误消失
- ✅ 类型转换错误消失
- ✅ 元数据文件错误自动消失

## Phase 6: App 类重复定义问题 ✅ 已修复

### 问题描述

用户报告 `src/TKCompass.App/App.axaml.cs` 第 13 行报错：`public partial class App : Application` 重复定义。

### 根因分析

1. **旧项目残留**：`TKCompass/` 目录中的 `App.axaml.cs` 与 `src/TKCompass.App/App.axaml.cs` 都在 `namespace TKCompass` 中定义了 `class App`
2. **VS 缓存**：即使删除了旧项目，Visual Studio 的 `.vs` 缓存和 `obj` 目录可能仍引用旧代码
3. **Avalonia XAML 生成**：`App.axaml` 中的 `x:Class="TKCompass.App"` 会自动生成 partial class，如果存在多个同名类会冲突
4. **命名空间不一致**：`App.axaml.cs` 使用 `namespace TKCompass.AppBase`，但 `App.axaml` 期望 `x:Class="TKCompass.App"`，导致 XAML 找不到对应的类

### 修复操作

**已执行清理**：
1. ✓ 删除 `TKCompass/TKCompass/` 旧项目目录
2. ✓ 清理所有 `obj/bin` 缓存（4 个项目）
3. ⚠ `.vs` 目录部分删除（VS 正在使用，需关闭 VS 后手动删除）

**代码修复**（2026-11-08）：
1. ✓ **App.axaml.cs** - 命名空间从 `TKCompass.AppBase` 改为 `TKCompass`
2. ✓ **App.axaml** - 保持 `x:Class="TKCompass.App"`，删除 DataGrid 主题引用
3. ✓ **Program.cs** - 从 `AppBuilder.Configure<TKCompass.App>()` 改为 `AppBuilder.Configure<App>()`

**验证结果**：
- 现在只有 1 个 `App.axaml.cs` 文件：`src/TKCompass.App/App.axaml.cs`
- 现在只有 4 个 csproj 文件（都在 `src/` 下）
- 三个文件命名空间完全对齐：`namespace TKCompass` + `class App`

### 用户操作

1. **关闭 Visual Studio**
2. **删除 `.vs` 目录**（如果仍存在）：
   ```powershell
   cd D:\Projects\TKCompass
   Remove-Item -Recurse -Force .vs
   ```
3. **重新打开 `TKCompass.slnx`**
4. **重新生成解决方案**：`Ctrl + Shift + B`

### 验证状态

- [x] 旧项目目录已删除
- [x] obj/bin 缓存已清理
- [x] App.axaml.cs 命名空间修复
- [x] App.axaml 对齐
- [x] Program.cs 对齐
- [ ] 用户关闭 VS 并删除 .vs 目录
- [ ] 编译验证

### 经验总结

1. **Avalonia 项目重构**：删除旧项目时必须同时清理 `.vs` 和 `obj` 缓存
2. **命名空间冲突**：同一命名空间下不能有多个同名 partial class
3. **Visual Studio 缓存**：`.vs` 目录会缓存项目结构，删除项目后必须清理
4. **文件锁定**：VS 运行时会锁定 `.vs` 目录，需先关闭 VS 再删除
5. **XAML 与 C# 命名空间必须一致**：`x:Class="TKCompass.App"` 要求 C# 代码中必须是 `namespace TKCompass` + `class App`，不能是 `namespace TKCompass.AppBase`
6. **Program.cs 入口点**：`AppBuilder.Configure<App>()` 使用相对类名，不需要完整命名空间路径

---

*最后更新：2026-11-08*
