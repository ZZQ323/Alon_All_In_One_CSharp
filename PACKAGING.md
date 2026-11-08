# TKCompass 打包部署指南

## 发布流程

### 1. 发布应用程序

在 Windows PowerShell 中执行：

```powershell
cd D:\Projects\TKCompass
dotnet publish src\TKCompass.App\TKCompass.App.csproj -c Release -r win-x64 --self-contained -o publish
```

### 2. 创建安装程序

1. 安装 [Inno Setup](https://jrsoftware.org/isdl.php)
2. 打开 `installer\TKCompass.iss`
3. 点击 "Build" → "Compile" (Ctrl+F9)
4. 安装程序将生成在 `installer\Output\TKCompass_Setup_1.0.0.exe`

### 3. 安装和运行

- 双击安装程序，按向导完成安装
- 安装完成后，桌面和开始菜单会创建快捷方式
- 首次运行会自动创建数据库（%APPDATA%\TKCompass\tkcompass.db）

## 数据迁移

### 从旧系统（Aolon Manager）迁移

1. 在旧系统中导出数据：
   - 打开 Aolon Manager
   - 点击 "数据" → "导出全部数据"
   - 保存 ZIP 文件

2. 在新系统中导入数据：
   - 打开 TKCompass
   - 点击 "数据" → "导入数据"
   - 选择刚才导出的 ZIP 文件
   - 确认导入（会覆盖现有数据）

### 数据库位置

- 数据库文件：`%APPDATA%\TKCompass\tkcompass.db`
- 备份目录：`%APPDATA%\TKCompass\backups\`

## 自动备份

- 版本升级时自动备份旧数据库
- 保留最近 10 个备份
- 手动备份：点击 "数据" → "备份数据库"

## 卸载

1. 打开 "控制面板" → "程序和功能"
2. 找到 "TKCompass"，点击 "卸载"
3. 如需删除数据，手动删除 `%APPDATA%\TKCompass` 目录

## 故障排除

### 应用程序无法启动

1. 检查日志文件：`%APPDATA%\TKCompass\logs\`
2. 确认数据库文件存在且未被锁定
3. 尝试删除 `%APPDATA%\TKCompass` 目录重新初始化

### 数据导入失败

1. 确认 ZIP 文件格式正确（包含 metadata.json）
2. 检查导入日志：`%APPDATA%\TKCompass\logs\import.log`
3. 导入前会自动备份，可从备份恢复

## 技术支持

如有问题，请联系开发团队或查看项目文档。
