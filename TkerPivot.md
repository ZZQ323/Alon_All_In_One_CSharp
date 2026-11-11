# Aolon 达人管理系统 — 项目文档

> V3.0 · 2026-10-14

---

## 一、系统定位

运营每天打开系统，**只看达人**。系统告诉他：

- **今天该跟谁**（今日提醒 Tab）
- **这个达人走到哪一步了**（状态机：未触达 → 已触达 → 已发送合作 → 已同意合作 → 已寄样）
- **哪些达人暂时不合作了**（已交付 Tab）

系统不替运营说话（话术运营自己选），只负责**呈现现状、提醒行动**。

---

## 二、核心概念

### 2.1 业务层级

```
达人（Creator）
  ├── 寄样（Shipment）—— 一个快递单号 = 一次寄样
  │     └── 视频（Video）—— 一个视频链接 = 一条记录
  ├── 合作状态（cooperation_status）—— 状态机流转
  ├── 已交付标记（delivered）—— bool，人工标记"暂时不合作"
  └── 跟进记录（status_flow_history）—— 完整状态流转历史
```

### 2.2 合作状态机

```
未触达 → 已触达 → 已发送合作 → 已同意合作 → 已寄样
```

- 每个阶段有三个操作按钮：**进度推进** / **已提醒** / **已交付**
- **进度推进**：流转到下一状态（保留在今日列表）
- **已提醒**：今日隐藏，出现在"已提醒"Tab，明天继续
- **已交付**：标记 `delivered=true`，进入"已交付"Tab（暂时不合作）

### 2.3 已交付的含义

**已交付 = 人工标记"暂时不会找你合作了"**，是一个简单的 bool 字段，跟 SKU/视频数量无关。

- 运营手动点击"已交付"按钮标记
- 已交付达人从今日提醒、已提醒、总表中排除
- 可通过"已交付"Tab 查看，点击"重新联系"恢复到"已触达"状态

### 2.4 寄样与视频

- 一个寄样默认要求 3 个视频（`required_video_count = 3`）
- 一个达人可能有多个寄样（多次合作）
- 每个视频绑定广告代码，系统不自动匹配
- 缺视频数、缺代码数由视图自动计算

---

## 三、页面设计

### 3.1 达人工作台（首页）

Tab 结构（6 个 Tab）：

| Tab | 内容 |
|---|---|
| **今日提醒** | 今日待跟进达人，分"未同意合作"和"已同意合作"两区 |
| **正在交付** | 已同意合作或已寄样的达人（delivered=false），支持"标记已交付" |
| **已提醒** | 今日已提醒的达人列表，可"重新联系" |
| **已交付** | 已交付达人列表（暂时不合作），可"重新联系" |
| **总表** | 所有达人，支持批量删除 |
| **已删除** | 软删除的达人，支持恢复/永久删除 |

**顶部统一筛选栏**（所有 Tab 共用）：
- Handle 搜索框（达人 ID）
- 状态下拉框（含"正在交付"虚拟状态）
- 等级下拉框（S/A/B/C/D）
- 寄样时间范围（开始日期 - 结束日期）
- 最后联系时间范围
- 产品代码多选（从字典表勾选）
- 产品名搜索框
- 筛选/重置按钮

**第二层工具栏**（Tab 下方）：
- 全选/取消全选按钮
- 批量操作按钮（根据当前 Tab 动态变化：批量删除/批量永久删除/批量标记已交付）
- 新建达人按钮

**今日提醒 Tab 内部结构：**

- **未同意合作区**：达人触达表 + 发送合作表
- **已同意合作区**：已寄样-交付一个视频 + 已寄样-交付两个视频 + 基础履约完成

每个表格的操作按钮：详情 + 阶段特定按钮（进度推进/同意合作/已提醒/已交付/新增视频/新增代码）

**按钮语义变更**：
- 未合作达人（未同意合作状态）："已交付"按钮改为"删除"按钮，需填写删除原因
- 已合作达人（已同意合作/已寄样状态）："已交付"按钮标记 delivered=true

### 3.2 达人详情

- 折叠栏展示：每个寄样一个折叠栏
- 折叠栏顶部：寄样信息（快递代码、日期、产品代码、产品名、要求视频数、缺视频数、缺代码数）
- 折叠栏右侧：+ 视频 / + 代码 按钮
- 视频表格中广告代码列可直接编辑（inline editing）
- 保存按钮：仅底部"保存关闭"（合并保存+关闭功能），移除基本信息区域的独立保存按钮
- 动态联系方式：
  - 默认显示"新增联系方式"行（下拉框选择类型 + 输入框 + 添加按钮）
  - 已添加的联系方式显示为：类型标签 + 数值 + 删除按钮
  - 支持多个同类型联系方式（如 2 个 WhatsApp、3 个邮箱）
  - 联系方式类型：WhatsApp/邮箱/TT后台/达人后台/其他
  - 语种字段移动到联系方式上方

### 3.3 寄样表单

- 新建时不显示"寄样状态"字段，默认"已寄样"
- 字段：快递代码、寄样日期、产品代码（多选）、产品名（自动填充）、要求视频数、备注、寄样状态（编辑时显示）
- 产品代码输入：多选对话框，从 product_codes 字典表勾选，逗号分隔存储（如 `AGTR3BK,AGTR3GD`）
- 产品名自动填充：根据选中的产品代码自动填充对应的中文显示名（如 `GTR3 黑色,GTR3 金色`）

---

## 四、数据模型

### 4.1 核心关系

```
Creator 1 ── N Shipment 1 ── N Video
Creator 1 ── N StatusFlowHistory
Creator 1 ── N CreatorContact
```

### 4.2 核心表

#### creators

| 字段 | 类型 | 说明 |
|---|---|---|
| id | INTEGER PK | 自增 |
| handle | TEXT UNIQUE | TikTok @xxx（UI 显示为"达人 ID"） |
| nickname | TEXT | 昵称 |
| quality_level | TEXT | S/A/B/C/D |
| cooperation_status | TEXT | 合作状态（状态机） |
| delivered | INTEGER DEFAULT 0 | 已交付标记（bool） |
| delivered_reason | TEXT | 已交付/删除原因 |
| creator_type_id | FK | 达人类型 |
| today_status | TEXT | 今日状态（待处理/已提醒/已解决） |
| today_resolved_date | DATE | 今日已解决日期 |
| next_follow_up_date | DATE | 下次跟进日期 |
| latest_contact_date | DATE | 最近联系日期 |
| deleted | INTEGER DEFAULT 0 | 软删除 |
| tiktok_homepage_url | TEXT | TikTok 主页链接 |
| preferred_language | TEXT | 语种 |
| contact_location | TEXT | 联系位置 |
| tiktok_backend_id | TEXT | TikTok 后台 ID（已废弃，迁移到 creator_contacts） |
| instagram | TEXT | Instagram（已废弃，迁移到 creator_contacts） |
| whatsapp | TEXT | WhatsApp（已废弃，迁移到 creator_contacts） |
| email | TEXT | 邮箱（已废弃，迁移到 creator_contacts） |
| other_contact | TEXT | 其他联系方式（已废弃，迁移到 creator_contacts） |

**注意**：whatsapp/email/tiktok_backend_id/instagram/other_contact 字段已废弃（V27），数据迁移到 creator_contacts 表，支持多值同类型联系方式。

#### creator_contacts（V27 新增）

| 字段 | 类型 | 说明 |
|---|---|---|
| id | INTEGER PK | 自增 |
| creator_id | FK | 关联达人 |
| contact_type | TEXT | 联系方式类型（whatsapp/email/tiktok_backend/influencer_backend/other） |
| contact_value | TEXT | 联系方式值 |
| created_at | DATETIME | 创建时间 |

#### shipments

| 字段 | 类型 | 说明 |
|---|---|---|
| id | INTEGER PK | 自增 |
| creator_id | FK | 关联达人 |
| tracking_number | TEXT | 快递单号 |
| sku | TEXT | 产品代码（逗号分隔，如 `AGTR3BK,AGTR3GD`） |
| product_name | TEXT | 产品名（逗号分隔，如 `GTR3 黑色,GTR3 金色`） |
| sample_date | DATE | 寄样日期 |
| required_video_count | INTEGER DEFAULT 3 | 要求视频数 |
| is_completed_manual | INTEGER DEFAULT 0 | 手动标记完成 |
| deleted | INTEGER DEFAULT 0 | 软删除 |

#### videos

| 字段 | 类型 | 说明 |
|---|---|---|
| id | INTEGER PK | 自增 |
| creator_id | FK | 达人 |
| shipment_id | FK | 关联寄样 |
| video_url | TEXT | 视频链接 |
| ad_code | TEXT | 广告代码 |
| video_date | DATETIME | 发布时间 |
| is_effective | INTEGER DEFAULT 0 | 是否计入履约 |
| views / orders / gmv | INTEGER | 基础数据 |
| deleted | INTEGER DEFAULT 0 | 软删除 |

#### status_flow_history

| 字段 | 类型 | 说明 |
|---|---|---|
| id | INTEGER PK | 自增 |
| creator_id | FK | 达人 |
| from_status | TEXT | 原状态 |
| to_status | TEXT | 新状态 |
| action_type | TEXT | 操作类型 |
| action_date | DATE | 操作日期 |
| notes | TEXT | 备注 |

---

## 五、技术栈

| 层 | 技术 | 说明 |
|---|---|---|
| UI | JavaFX 21 + FXML | 桌面 UI |
| 构建 | Gradle 8.x | 多模块 |
| 语言 | Java 21 LTS | |
| 数据库 | SQLite 3.45+ | 单文件本地 DB |
| 迁移 | Flyway 10.17.0 + Hibernate hbm2ddl | Schema 管理 |
| Excel | Apache POI 5.3.0 | 导入导出 |
| 打包 | jpackage + jlink | 生成 exe，自带 JRE |
| 事件总线 | EventBus（单例模式） | 跨页面数据刷新通知 |

---

## 五.一、打包与发布（快速开始）

### 打包命令（Windows PowerShell / CMD）

```powershell
cd D:\Projects\Alon_All_In_One\Aolon_Manager

# 一键打包（编译 + jlink + jpackage）
.\gradlew.bat :app-packaging:jpackage
```

**输出**：`app-packaging\build\jpackage\TkerPivot-3.7.0.exe`

### 环境要求

| 工具 | 版本 | 说明 |
|------|------|------|
| JDK | 21+ | 必须与目标平台匹配 |
| WiX 3 | 3.14+ | 仅 Windows MSI 需要，`winget install WiX.WiX3` |
| Gradle | 8.x | 项目自带 `gradlew.bat` |
| JavaFX jmods | 21.0.2 | 放在 `app-packaging/javafx-jmods/javafx-jmods-21.0.2/` |

### 分步执行

```powershell
.\gradlew.bat clean              # 清理
.\gradlew.bat build              # 编译
.\gradlew.bat test               # 测试（可选）
.\gradlew.bat :app-packaging:shadowJar    # 创建 fat JAR
.\gradlew.bat :app-packaging:createRuntime  # 创建自定义 runtime
.\gradlew.bat :app-packaging:jpackage    # 生成安装包
```

### 产物说明

- **安装包**：`TkerPivot-3.7.0.exe`（自带 JRE，用户无需安装 Java）
- **免安装版**：`app-packaging/build/jpackage/TkerPivot/`（绿色版，可直接运行）

### 其他平台

```bash
# macOS
./gradlew :app-packaging:jpackage   # 输出 .pkg

# Linux
./gradlew :app-packaging:jpackage   # 输出 .deb
```

> ⚠️ **WSL2 用户注意**：`/mnt/d/` 路径有已知 I/O 问题，打包必须在 Windows 原生环境执行。

📖 详细打包文档：`app-packaging/README.md`

### 打包配置关键要点（经验总结）

#### 1. Shadow JAR 配置

```groovy
shadowJar {
    archiveBaseName.set('TkerPivot')
    archiveClassifier.set('')
    archiveVersion.set(version)
    mergeServiceFiles()
    
    manifest {
        attributes 'Main-Class': 'org.Aolon.main.Launcher'
    }
    
    // 排除签名文件
    exclude 'META-INF/*.SF'
    exclude 'META-INF/*.DSA'
    exclude 'META-INF/*.RSA'
    
    // 关键：排除所有 module-info.class，避免 jpackage 误认为是模块化 JAR
    exclude '**/module-info.class'
    exclude 'META-INF/versions/*/module-info.class'
}
```

**为什么排除 module-info.class？**
- 第三方库（如 slf4j）可能包含 `META-INF/versions/9/module-info.class`
- jpackage 检测到这些文件会误判 fat JAR 为模块化 JAR
- 导致启动时模块系统冲突，报 "Failed to launch JVM"

#### 2. jlink 配置

```groovy
tasks.register('createRuntime') {
    dependsOn shadowJar
    
    def runtimeDir = file("${buildDir}/runtime").absolutePath
    def javaHome = System.getenv('JAVA_HOME')
    def jdkJmods = file("${javaHome}/jmods").absolutePath
    def javafxJmods = file("${projectDir}/javafx-jmods/javafx-jmods-21.0.2").absolutePath
    
    doFirst {
        // 验证 JAVA_HOME
        if (!javaHome) {
            throw new GradleException("JAVA_HOME environment variable is not set")
        }
        
        // 验证 jmods 存在
        if (!file(jdkJmods).exists()) {
            throw new GradleException("JDK jmods not found at: ${jdkJmods}")
        }
        if (!file(javafxJmods).exists()) {
            throw new GradleException("JavaFX jmods not found at: ${javafxJmods}")
        }
    }
    
    doLast {
        def rtDir = file(runtimeDir)
        if (rtDir.exists()) {
            rtDir.deleteDir()
        }
        
        // 使用 File.pathSeparator 自动适配平台（Windows: ; Linux/macOS: :）
        def modulePath = jdkJmods.toString() + File.pathSeparator + javafxJmods.toString()
        
        // 使用绝对路径调用 jlink
        def os = org.gradle.internal.os.OperatingSystem.current()
        def jlinkExe = "${javaHome}/bin/jlink"
        if (os.windows) {
            jlinkExe += ".exe"
        }
        
        def command = [
            jlinkExe,
            '--module-path', modulePath,
            // 关键：必须包含 java.naming 模块（logback 需要）
            '--add-modules', 'java.base,java.desktop,java.logging,java.sql,java.xml,java.naming,javafx.base,javafx.controls,javafx.fxml,javafx.graphics',
            '--output', runtimeDir.toString(),
            '--no-header-files',
            '--no-man-pages',
            '--strip-debug'
        ]
        
        def pb = new ProcessBuilder((List<String>) command.collect { it.toString() })
        pb.redirectErrorStream(false)
        def process = pb.start()
        
        def output = process.inputStream.text
        def errorOutput = process.errorStream.text
        def exitCode = process.waitFor()
        
        if (exitCode != 0) {
            throw new GradleException(
                "jlink failed with exit code ${exitCode}\n" +
                "STDOUT:\n${output}\n" +
                "STDERR:\n${errorOutput}"
            )
        }
    }
}
```

**为什么需要 java.naming 模块？**
- logback-classic 在初始化时需要 `javax.naming.NamingException`
- 如果 runtime 缺少 `java.naming` 模块，启动时会报 `NoClassDefFoundError`
- 这是 logback 的依赖，不是应用代码的依赖

#### 3. jpackage 配置

```groovy
tasks.register('jpackage') {
    dependsOn shadowJar, createRuntime
    
    def os = org.gradle.internal.os.OperatingSystem.current()
    def inputDir = "${buildDir}/libs"
    def outputDir = "${buildDir}/jpackage"
    def runtimeDir = "${buildDir}/runtime"
    def iconFile = "${projectDir}/app-icon.ico"
    
    doFirst {
        // 验证 JAVA_HOME
        def javaHome = System.getenv('JAVA_HOME')
        if (!javaHome) {
            throw new GradleException("JAVA_HOME is not set")
        }
        
        // 使用绝对路径调用 jpackage
        def jpackageExe = "${javaHome}/bin/jpackage"
        if (os.windows) {
            jpackageExe += ".exe"
        }
        
        // 验证 runtime 完整（必须包含 java.exe）
        def javaExe = file("${runtimeDir}/bin/java.exe")
        if (!javaExe.exists()) {
            throw new GradleException(
                "Runtime is incomplete: java.exe not found at ${javaExe.absolutePath}"
            )
        }
        
        def command = [
            jpackageExe,
            '--type', 'exe',
            '--name', 'TkerPivot',
            '--app-version', version.toString(),
            '--input', file(inputDir).absolutePath,
            '--main-jar', "TkerPivot-${version}.jar",
            '--main-class', 'org.Aolon.main.Launcher',
            '--runtime-image', file(runtimeDir).absolutePath,
            '--dest', file(outputDir).absolutePath,
            '--icon', file(iconFile).absolutePath,
            '--vendor', 'Aolon',
            '--copyright', 'Copyright 2026 Aolon',
            '--description', 'TkerPivot - TikTok 达人跟进与视频履约管理',
            // 关键：加载 JavaFX 模块并允许反射访问
            '--java-options', '--add-modules=javafx.controls,javafx.fxml,javafx.graphics,javafx.base',
            '--java-options', '--add-opens=javafx.graphics/javafx.application=ALL-UNNAMED',
            '--java-options', '--add-opens=javafx.graphics/javafx.stage=ALL-UNNAMED',
            '--java-options', '--add-opens=javafx.controls/javafx.scene.control=ALL-UNNAMED',
            '--win-dir-chooser',
            '--win-menu',
            '--win-shortcut'
        ]
        
        // GString 转 String
        def pb = new ProcessBuilder((List<String>) command.collect { it.toString() })
        pb.redirectErrorStream(false)
        def process = pb.start()
        
        def output = process.inputStream.text
        def errorOutput = process.errorStream.text
        def exitCode = process.waitFor()
        
        if (exitCode != 0) {
            throw new GradleException(
                "jpackage failed with exit code ${exitCode}\n" +
                "STDOUT:\n${output}\n" +
                "STDERR:\n${errorOutput}"
            )
        }
    }
}
```

**为什么需要 --add-modules 和 --add-opens？**
- fat JAR 没有 JavaFX 类（在 runtime 的 modules 里）
- 需要 `--add-modules` 告诉 JVM 加载 JavaFX 模块
- 需要 `--add-opens` 允许 JavaFX 内部类的反射访问（JavaFX 框架需要）

#### 4. Launcher 类

```java
package org.Aolon.main;

import javafx.application.Application;

public class Launcher {
    public static void main(String[] args) {
        // 使用标准方式启动 JavaFX 应用
        Application.launch(AolonApp.class, args);
    }
}
```

**为什么需要 Launcher 类？**
- JavaFX 要求 main class 不能是 Application 子类（从 fat JAR 启动时）
- Launcher 作为入口，调用 `Application.launch(AolonApp.class, args)`

#### 5. 常见错误排查

| 错误 | 原因 | 解决方案 |
|------|------|----------|
| `Failed to launch JVM` | runtime 缺少 java.exe | 检查 `build/runtime/bin/java.exe` 是否存在 |
| `NoClassDefFoundError: javax/naming/NamingException` | runtime 缺少 java.naming 模块 | jlink 添加 `--add-modules java.naming` |
| `module-info.class` 冲突 | fat JAR 包含第三方库的 module-info | shadowJar 排除 `**/module-info.class` |
| `arraycopy: element type mismatch` | Groovy GString 传给 ProcessBuilder | 使用 `command.collect { it.toString() }` 转换 |
| `--runtime-image` 和 `--add-modules` 互斥 | jpackage 参数冲突 | 使用 `--runtime-image` 时不要加 `--add-modules` |

#### 6. 调试技巧

如果打包后启动失败，直接用 runtime 的 java.exe 运行 fat JAR：

```powershell
D:\Projects\AolonManager\runtime\bin\java.exe -jar D:\Projects\AolonManager\app\TkerPivot-3.7.0.jar
```

这会显示真实的 Java 错误，而不是 jpackage launcher 的 "Failed to launch JVM"。

#### 7. 环境诊断脚本

项目根目录提供 `diagnose-packaging.ps1`，检查 9 项环境依赖：

```powershell
.\diagnose-packaging.ps1
```

检查项：
1. JAVA_HOME 环境变量
2. Java 版本
3. jlink 工具
4. JDK jmods 目录
5. JavaFX jmods 目录
6. WiX Toolset（仅 MSI 需要）
7. jpackage 工具
8. 图标文件
9. Gradle Wrapper

### v-model 全局刷新机制

**核心原则**：所有修改数据表的操作，在保存后都应当能触发任意页面、任意 tab 的刷新。

**实现方式**：
- **EventBus 全局事件总线**（`org.Aolon.ui.EventBus`）：单例模式，发布/订阅架构
- **事件类型**：CREATOR_CREATED/UPDATED/DELETED、SHIPMENT_*、VIDEO_*、CONTACT_*、DICTIONARY_*、BATCH_IMPORT_COMPLETED 等
- **订阅机制**：CreatorDashboardController 订阅所有事件（通配符 `*`），收到事件后在 JavaFX 应用线程中调用 `refreshAllTabs()` 刷新所有 6 个 Tab
- **发布机制**：所有 Controller 在数据修改后发布对应事件（共 34 处发布调用）
- **字典变更**：额外刷新筛选器选项（initializeFilters + initializeProductCodeFilter），确保新增的字典值立即出现在下拉框中

**优势**：
- 解耦页面间通信，新增页面只需订阅事件即可自动刷新
- 无需修改其他页面代码，符合开闭原则
- 所有数据变更都能自动传播到 UI，保证数据一致性

### 项目结构

```
Aolon_Manager/
├── app-core/          # 领域层：模型 + Service + Repository 接口
├── app-data/          # 数据层：SQLite 实现 + Flyway 迁移
├── app-excel/         # Excel 导入导出
├── app-api/           # HTTP API（预留）
├── main/              # JavaFX UI
├── app-packaging/     # jpackage 配置
└── docs/
    ├── AolonManagement.md          # 本文档
    └── iterations/                 # 迭代文档（永久保留）
```

---

## 六、交付标准

| 项目 | 标准 |
|---|---|
| 安装包 | `Aolon达人管理-3.0.0.exe`，自带 JRE |
| 数据存储 | `%APPDATA%/Aolon/aolon.db`，SQLite 单文件 |
| 自动备份 | 启动时 + 每日首次使用 |
| 单实例 | 防止重复启动 |

---

## 七、迭代历史

迭代文档永久保留在 `docs/iterations/` 目录，不删除。

| 迭代 | 内容 |
|---|---|
| iter_v3_statemachine.md | V3.0 状态机 + 达人工作台重构 |
| iter_status_flow_history.md | 状态流转历史 + 三按钮语义重构 |
| iter_detail_page_folderview.md | 达人详情页折叠栏 + 寄样表单简化 |
| iter_notification.md | 今日待跟进任务通知 |
| iter_delivered_tab.md | 已交付达人独立 Tab |
| iter_feedback_v2.1.md | 搜索筛选增强 + 已删除视图 + 冲突提醒 + 详情页字段优化 |
| iter_v2.2_per_field_filter.md | 统一筛选栏（5 个 Tab 共用）+ 产品代码写入 |
| iter_v2.3_filter_fixes.md | 全选/取消全选修复 + 最后联系时间筛选 + 寄样时间列 + 产品代码/产品名逻辑 |
| iter_v2.4_responsive_and_multiselect.md | 搜索框响应式布局 + 寄样产品代码多选对话框 |
| iter_v2.5_tab_cleanup_and_sku_fix.md | 新增"正在交付"Tab + SKU 列改名 + 产品代码列显示修复 |
| iter_v2.6_compile_fix_and_product_code_filter.md | 编译修复 + 产品代码多选筛选（MenuButton） |
| iter_v2.6_ui_refinement.md | 分层按钮布局 + Handle 改名"达人 ID" + 详情页保存按钮精简 + 联系方式加号模式 + Tab 顺序调整 |
| iter_v2.7_dynamic_contacts.md | 动态联系方式管理（creator_contacts 表，支持多值同类型） |
| iter_v3.7.3_filter_reset.md | 筛选重置功能修复 + 清除按钮（2026-10-25） |

---

*文档版本：3.1 · 最后更新：2026-10-18*
