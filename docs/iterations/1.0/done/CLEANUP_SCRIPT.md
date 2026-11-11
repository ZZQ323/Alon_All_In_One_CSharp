# TKCompass 编译错误清理脚本

> 日期：2026-11-08
> 用途：修复编译错误的 PowerShell 脚本

## 问题根因

1. **重复 App 类**：`TKCompass/App.axaml.cs` 和 `src/TKCompass.App/App.axaml.cs` 都定义了 `namespace TKCompass; class App`
2. **DataGrid 版本**：旧项目 obj 缓存引用 12.1.4（实际不存在）
3. **类型转换错误**：`CreatorContactService` 中的方法签名与 Repository 不匹配（已修复）

## 修复步骤

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

## 预期结果

- ✅ 重复 App 类错误消失
- ✅ DataGrid 版本错误消失
- ✅ 类型转换错误消失
- ✅ 元数据文件错误自动消失

## 如果仍有错误

如果删除 `TKCompass/` 后仍有错误，请执行：

```powershell
# 检查是否还有其他项目引用
dotnet sln TKCompass.slnx list

# 应该只显示 4 个项目：
# - src/TKCompass.Domain/TKCompass.Domain.csproj
# - src/TKCompass.Data/TKCompass.Data.csproj
# - src/TKCompass.Excel/TKCompass.Excel.csproj
# - src/TKCompass.App/TKCompass.App.csproj
```

---

*最后更新：2026-11-08*
