# TKCompass 迭代 1.0 错误日志

> 创建日期：2026-11-08
> 用途：记录开发过程中遇到的编译错误、运行时错误及其修复方案

---

## 错误 #1：DateOnly? 到 DateOnly 的类型转换错误

**发现日期**：2026-11-08  
**错误类型**：编译错误 CS1503  
**错误信息**：参数 3: 无法从"System.DateOnly?"转换为"System.DateOnly"

### 问题描述

在 `CreatorDetailViewModel.cs` 的 `AddShipmentAsync` 方法中，调用 `CreateShipmentAsync` 时传入的 `SampleDate` 参数类型不匹配。

### 根因分析

- **模型定义**：`Shipment.SampleDate` 的类型是 `DateOnly?`（可空）
- **方法签名**：`IShipmentService.CreateShipmentAsync(long creatorId, string trackingNumber, DateOnly sampleDate)` 要求 `DateOnly`（非空）
- **类型不匹配**：直接将 `DateOnly?` 传给 `DateOnly` 参数，编译器无法自动转换

### 修复方案

在调用处使用空值合并运算符 `??` 提供默认值：

```csharp
// 修复前
var saved = await _shipmentService.CreateShipmentAsync(
    Creator.Id, 
    shipment.TrackingNumber ?? "", 
    shipment.SampleDate);  // ❌ DateOnly? 无法转换为 DateOnly

// 修复后
var saved = await _shipmentService.CreateShipmentAsync(
    Creator.Id, 
    shipment.TrackingNumber ?? "", 
    shipment.SampleDate ?? DateOnly.FromDateTime(DateTime.Now));  // ✅ 提供默认值
```

### 修复文件

- `/src/TKCompass.App/ViewModels/CreatorDetailViewModel.cs` (第 83-86 行)

### 设计一致性

此修复与 `ShipmentEditViewModel.cs:63` 的处理方式保持一致：

```csharp
SampleDate = Shipment.SampleDate ?? DateOnly.FromDateTime(DateTime.Now);
```

### 验证状态

- [x] 代码修复完成
- [ ] 编译验证（需在 Windows 环境执行 `dotnet build`）
- [ ] 功能测试（创建寄样记录，验证日期正确性）

### 经验总结

1. **可空类型处理**：当模型字段是 `T?` 而方法参数是 `T` 时，必须在调用处显式处理空值
2. **默认值策略**：对于日期字段，使用当前日期作为默认值是合理的业务逻辑
3. **一致性检查**：修复时应参考项目中类似场景的处理方式，保持代码风格一致

---

## 错误 #2：Relationship 实体缺少主键配置

**发现日期**：2026-11-08  
**错误类型**：运行时异常 `System.InvalidOperationException`  
**错误信息**：The entity type 'Relationship' requires a primary key to be defined.

### 问题描述

应用启动时 EF Core 抛出异常，提示 `Relationship` 实体类型需要定义主键。

### 根因分析

- **模型定义**：`Relationship` 类的主键字段是 `CreatorId`（非标准命名）
- **EF Core 约定**：默认查找 `Id` 或 `{ClassName}Id` 作为主键
- **配置缺失**：没有对应的 `RelationshipConfiguration.cs` 文件显式指定主键
- **结果**：EF Core 无法推断主键，启动验证失败

### 修复方案

创建 `RelationshipConfiguration.cs`，显式配置主键和关系映射：

```csharp
public class RelationshipConfiguration : IEntityTypeConfiguration<Relationship>
{
    public void Configure(EntityTypeBuilder<Relationship> builder)
    {
        builder.ToTable("relationships");
        
        // 显式指定主键为 CreatorId（一对一关系）
        builder.HasKey(e => e.CreatorId);
        
        // 配置字段约束
        builder.Property(e => e.PreferredName).HasMaxLength(100);
        builder.Property(e => e.RelationshipNotes).HasMaxLength(2000);
        builder.Property(e => e.VideoStyle).HasMaxLength(200);
        builder.Property(e => e.NextAction).HasMaxLength(500);
        builder.Property(e => e.CurrentCommission).HasMaxLength(100);
        
        // 一对一关系：Relationship 与 Creator 共享主键
        builder.HasOne(e => e.Creator)
            .WithOne()
            .HasForeignKey<Relationship>(e => e.CreatorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

### 修复文件

- `/src/TKCompass.Data/Configuration/RelationshipConfiguration.cs`（新建）

### 设计说明

`Relationship` 与 `Creator` 是一对一关系，共享 `CreatorId` 作为主键。这种设计：
- 避免了额外的 `Id` 字段
- 通过外键约束保证数据完整性
- 级联删除确保删除达人时自动清理关系数据

### 验证状态

- [x] 代码修复完成
- [ ] 编译验证（需在 Windows 环境执行 `dotnet build`）
- [ ] 运行时验证（启动应用，确认无异常）

### 经验总结

1. **EF Core 主键约定**：非标准命名的主键必须通过 `HasKey()` 显式配置
2. **配置分离**：每个实体应有独立的 `IEntityTypeConfiguration<T>` 实现类
3. **一对一关系**：共享主键的一对一关系需要同时配置 `HasKey()` 和 `HasForeignKey()`

---

## 错误 #3：CreatorContactService 类型转换错误

**发现日期**：2026-11-08  
**错误类型**：编译错误 CS1503, CS0029  
**错误信息**：
- 参数 1: 无法从"long"转换为"TKCompass.Domain.Models.CreatorContact"（2处）
- 无法将类型"void"隐式转换为"TKCompass.Domain.Models.CreatorContact"

### 问题描述

`CreatorContactService` 中的方法签名与 `IRepository<T>` 接口不匹配：

1. **DeleteAsync**：Service 接受 `long contactId`，但直接传给 `_repository.DeleteAsync(contactId)`，而 Repository 期望实体对象 `T entity`
2. **UpdateAsync**：Service 返回 `Task<CreatorContact>`，但 `BaseRepository.UpdateAsync` 返回 `Task`（void）

### 根因分析

- **IRepository<T> 接口定义**：
  ```csharp
  Task UpdateAsync(T entity);  // 返回 Task（void）
  Task DeleteAsync(T entity);  // 接受实体对象
  ```

- **CreatorContactService 错误实现**：
  ```csharp
  // ❌ 错误：直接传 long
  public async Task DeleteAsync(long contactId)
  {
      await _repository.DeleteAsync(contactId);  // 类型不匹配
  }
  
  // ❌ 错误：返回类型不匹配
  public async Task<CreatorContact> UpdateAsync(CreatorContact contact)
  {
      return await _repository.UpdateAsync(contact);  // Repository 返回 void
  }
  ```

### 修复方案

**修复 1: DeleteAsync — 先查询实体，再删除**

```csharp
public async Task DeleteAsync(long contactId)
{
    var contact = await _repository.GetByIdAsync(contactId);
    if (contact != null)
    {
        await _repository.DeleteAsync(contact);
    }
}
```

**修复 2: UpdateAsync — 改为返回 Task**

```csharp
public async Task UpdateAsync(CreatorContact contact)
{
    await _repository.UpdateAsync(contact);
}
```

**修复 3: DeleteAllByCreatorIdAsync — 传递实体而非 Id**

```csharp
public async Task DeleteAllByCreatorIdAsync(long creatorId)
{
    var contacts = await _repository.GetByCreatorIdAsync(creatorId);
    foreach (var contact in contacts)
    {
        await _repository.DeleteAsync(contact);  // 传递实体
    }
}
```

**修复 4: ICreatorContactService 接口 — 匹配实现**

```csharp
Task UpdateAsync(CreatorContact contact);  // 改为返回 Task
```

### 修复文件

- `/src/TKCompass.Domain/Services/CreatorContactService.cs` ✅
- `/src/TKCompass.Domain/Services/ICreatorContactService.cs` ✅

### 验证状态

- [x] 代码修复完成
- [ ] 编译验证（需用户在 Windows 环境执行 `dotnet build`）

### 经验总结

1. **Repository 模式一致性**：Service 层的方法签名必须与 Repository 接口匹配
2. **DeleteAsync 设计**：如果 Service 接受 Id，应先查询实体再删除；或直接接受实体对象
3. **返回类型匹配**：`UpdateAsync` 如果不需要返回更新后的实体，应返回 `Task` 而非 `Task<T>`
4. **接口与实现同步**：修改实现后必须同步更新接口定义

---

## 错误 #4：重复 App 类定义

**发现日期**：2026-11-08  
**错误类型**：编译错误 CS0101  
**错误信息**：命名空间"TKCompass"已经包含"App"的定义

### 问题描述

项目中存在两个 `App` 类，都在 `namespace TKCompass` 中：

1. `TKCompass/App.axaml.cs` — 旧模板项目
2. `src/TKCompass.App/App.axaml.cs` — 新主项目

### 根因分析

- `TKCompass/` 是项目初始化时 Avalonia 模板创建的旧项目
- `src/TKCompass.App/` 是重构后的新主项目
- 两者不在同一个解决方案（`TKCompass.slnx` 只包含 `src/` 下的项目）
- 但 Visual Studio 可能仍加载了旧项目，导致命名空间冲突

### 修复方案

**删除旧项目目录**：

```powershell
cd D:\Projects\TKCompass
Remove-Item -Recurse -Force TKCompass
```

### 修复文件

- `/TKCompass/` 目录（整个删除）

### 验证状态

- [ ] 用户执行删除操作
- [ ] 编译验证

### 经验总结

1. **项目重构时清理旧文件**：重构后应删除不再使用的旧项目目录
2. **命名空间冲突**：同一命名空间下不能有重复的类名
3. **Visual Studio 缓存**：即使不在解决方案中，VS 仍可能加载旧项目

---

## 错误 #5：Avalonia.Controls.DataGrid 版本不存在

**发现日期**：2026-11-08  
**错误类型**：NuGet 包解析错误  
**错误信息**：找不到版本为 (>= 12.1.4) 的包 Avalonia.Controls.DataGrid，最接近版本: 12.1.2

### 问题描述

旧项目 `TKCompass/TKCompass.csproj` 的 obj 缓存引用了 Avalonia 12.1.4，但该版本不存在。

### 根因分析

- `src/TKCompass.App/TKCompass.App.csproj` 已正确引用 12.1.2
- 但 `TKCompass/TKCompass.csproj` 的 obj 缓存（`project.assets.json`）仍引用 12.1.4
- Visual Studio 加载了旧项目，尝试解析不存在的包版本

### 修复方案

**删除旧项目并清理缓存**：

```powershell
cd D:\Projects\TKCompass

# 删除旧项目
Remove-Item -Recurse -Force TKCompass

# 清理所有 obj/bin
Get-ChildItem -Path src -Recurse -Directory -Include obj,bin | Remove-Item -Recurse -Force

# 重新构建
dotnet clean
dotnet build TKCompass.slnx
```

### 修复文件

- `/TKCompass/` 目录（删除）
- `/src/*/obj/` 目录（清理）

### 验证状态

- [ ] 用户执行清理操作
- [ ] 编译验证

### 经验总结

1. **NuGet 缓存问题**：obj 目录中的 `project.assets.json` 可能引用不存在的包版本
2. **清理缓存**：遇到包版本错误时，删除 obj/bin 目录重新还原
3. **旧项目干扰**：不在解决方案中的旧项目仍可能干扰构建

---

## 错误 #6：元数据文件找不到

**发现日期**：2026-11-08  
**错误类型**：编译错误 CS0006  
**错误信息**：
- 未能找到元数据文件"TKCompass.Domain.dll"
- 未能找到元数据文件"TKCompass.Data.dll"
- 未能找到元数据文件"TKCompass.Excel.dll"

### 问题描述

引用这些项目的项目无法找到编译输出的 DLL 文件。

### 根因分析

这是**连锁反应**错误：

1. `TKCompass.Domain` 项目因类型转换错误（错误 #3）编译失败
2. Domain 编译失败 → 没有输出 `TKCompass.Domain.dll`
3. `TKCompass.Data` 依赖 Domain → 找不到 Domain.dll → 编译失败
4. `TKCompass.Excel` 依赖 Domain → 找不到 Domain.dll → 编译失败
5. `TKCompass.App` 依赖所有项目 → 找不到任何 DLL → 编译失败

### 修复方案

**修复根因（错误 #3、#4、#5）后，此错误自动消失**。

### 验证状态

- [x] 根因已修复
- [ ] 编译验证（修复根因后自动消失）

### 经验总结

1. **连锁反应错误**：底层项目编译失败会导致上层项目出现大量"找不到元数据文件"错误
2. **从底层开始修复**：遇到多个错误时，先修复最底层的项目（Domain → Data → App）
3. **不要单独修复连锁错误**：修复根因后，连锁错误自动消失

---

*最后更新：2026-11-08*
