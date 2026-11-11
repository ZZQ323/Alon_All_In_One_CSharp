# 迭代 3.7.4 — 技术栈迁移评估

> 日期：2026-11-08
> 状态：评估阶段，尚未启动迁移

---

## 一、背景与动机

Aolon Manager 经历了 v1.0 → v3.7.3 共 47+ 个迭代，已部署生产环境，管理 2000+ 达人数据。当前技术栈（JavaFX 21 + SQLite + Flyway + jpackage）在进一步开发中遇到明显阻力：

### 1.1 当前痛点

| 痛点 | 具体表现 |
|------|---------|
| **UI 丑** | JavaFX 深色主题需要大量手写 CSS，组件库匮乏，视觉效果远逊于现代 Web/移动端 |
| **响应式差** | 没有 v-model、没有组件化、没有声明式 UI。ObservableList + setAll() 是手动刷新的 workaround，不是真正的响应式 |
| **EventBus 负担** | 34 处事件发布，页面间通信靠手动 publish/subscribe，新增功能容易遗漏 |
| **编译部署重** | shadowJar → jlink → jpackage 三步打包，WSL2 有 I/O 问题必须切 Windows，覆盖安装需要版本检测+自动备份 |
| **生态局限** | JavaFX 社区萎缩，第三方组件少，移动端无可能 |

### 1.2 代码规模（迁移成本基准）

| 类型 | 文件数 | 代码行数 |
|------|--------|---------|
| Java 总计 | 142 | ~29,255 |
| FXML | 14 | ~1,157 |
| CSS | 2 | - |
| SQL 迁移 | 78 | - |

**模块分布**：

| 模块 | Java 文件 | 职责 | 迁移难度 |
|------|----------|------|---------|
| main（JavaFX UI） | 109 | FXML + Controller + 事件总线 | 🔴 全部重写 |
| app-core | 92 | 领域模型 + Service + Repository 接口 | 🟢 几乎不变 |
| app-data | 25 | SQLite 实现 + Flyway 迁移 | 🟡 驱动换但 SQL 不变 |
| app-excel | 3 | Apache POI 导入导出 | 🟡 库替换 |
| app-api | 0 | Javalin REST（预留） | 🟢 无迁移成本 |

**关键结论**：业务逻辑层（app-core + app-data）约 117 个文件，占总代码 80%+，与 UI 框架解耦良好。迁移的核心成本在 main 模块的 109 个 JavaFX 文件，业务层可大部分复用。

---

## 二、候选方案评估

### 2.1 方案 A：C# 桌面迁移（WPF / WinUI 3 / Avalonia）

#### 技术选型

| 框架 | 平台 | 成熟度 | 推荐度 |
|------|------|--------|--------|
| **WPF** | Windows only | 成熟稳定，社区庞大 | ⭐⭐⭐⭐ |
| **WinUI 3** | Windows 11+ | 微软主推，但生态仍在完善 | ⭐⭐⭐ |
| **Avalonia UI** | Win/Mac/Linux | 跨平台，类 WPF XAML | ⭐⭐⭐⭐⭐ |

**推荐 Avalonia UI**，理由：
- 跨平台（如果未来需要 macOS/Linux）
- XAML + MVVM 与 WPF 几乎一致，学习资源通用
- 社区活跃，控件库丰富（Material、Fluent）
- .NET 8+ AOT 编译，可生成自包含 exe（替代 jpackage）

#### 迁移映射

| 当前（JavaFX） | 目标（Avalonia/C#） | 说明 |
|---------------|-------------------|------|
| FXML + Controller | XAML + ViewModel (MVVM) | 声明式 UI + 数据绑定，天然响应式 |
| ObservableList + setAll() | ObservableCollection + INotifyPropertyChanged | 真正的响应式，v-model 等价物 |
| EventBus（34 处） | Messenger / EventAggregator | 仍有需要但 .NET 生态有成熟实现 |
| SQLite JDBC | Microsoft.Data.Sqlite / EF Core | SQL 不变，驱动替换 |
| Flyway | FluentMigrations / DbUp | 迁移脚本需要改写为 .NET 格式 |
| Apache POI | ClosedXML / EPPlus | Excel 读写 |
| jpackage | dotnet publish + Inno Setup / WiX | 自包含部署 |
| Java 分层架构 | .NET 分层（Domain / Data / UI） | 结构可一一对应 |

#### 优势

1. **语法亲和**：C# 与 Java 语法 80% 相似（类、接口、泛型、异常），学习曲线平缓
2. **MVVM 原生**：数据绑定是框架内置能力，不需要 EventBus 手动刷新
3. **UI 美观**：现代控件库开箱即用（Material Design、Fluent Design）
4. **部署简单**：`dotnet publish -r win-x64 --self-contained` 直接输出 exe 文件夹，Inno Setup 打包
5. **性能**：.NET 8 AOT 编译，启动速度和内存占用优于 JVM
6. **桌面端成熟**：WPF/Avalonia 在桌面端有 15+ 年积累

#### 劣势

1. **移动端不支持**：无法覆盖 Android 需求
2. **Flyway 迁移脚本**：78 个 SQL 文件需要改写为 .NET 迁移格式（但 SQL 本身不变）
3. **JavaFX UI 全部重写**：109 个文件 + 14 个 FXML 需要从零用 XAML 重建
4. **团队学习成本**：需要学习 C# + MVVM + XAML（预计 1-2 周上手）

#### 迁移工作量估算

| 阶段 | 内容 | 预估工时 |
|------|------|---------|
| Phase 1 | .NET 项目骨架 + 领域模型迁移（app-core → Domain） | 2-3 天 |
| Phase 2 | 数据层迁移（SQLite + 迁移脚本） | 2-3 天 |
| Phase 3 | 主界面 + 达人工作台（6 Tab） | 5-7 天 |
| Phase 4 | 详情页 + 寄样折叠栏 + 对话框 | 3-4 天 |
| Phase 5 | 筛选系统 + 批量操作 | 2-3 天 |
| Phase 6 | 撤销系统 + 操作历史 | 2-3 天 |
| Phase 7 | Excel 导入 + 备份 | 2-3 天 |
| Phase 8 | 打包部署 + 无痛覆盖 | 1-2 天 |
| **总计** | | **19-28 天** |

---

### 2.2 方案 B：UniApp 迁移（Vue 3 + TypeScript）

#### 技术选型

UniApp 基于 Vue 3，一套代码编译到 H5 / Android / iOS / 微信小程序。

| 维度 | 评估 |
|------|------|
| 跨平台 | ✅ 一套代码多端运行 |
| UI 美观 | ✅ uView / uni-ui 组件库丰富 |
| 响应式 | ✅ Vue 3 Composition API，天然 v-model |
| 移动端 | ✅ 原生支持 Android / iOS |

#### 关键问题：桌面端怎么办？

UniApp **不直接支持桌面端**（Windows/macOS/Linux 原生应用）。可选的绕行方案：

| 方案 | 可行性 | 说明 |
|------|--------|------|
| UniApp H5 + Electron 壳 | ⚠️ 可行但重 | Electron 打包 H5，体积 100MB+，体验不如原生 |
| UniApp H5 + Tauri 壳 | ✅ 推荐 | Tauri 体积小（10MB），Rust 后端，性能好 |
| UniApp 仅做 Android | ✅ 直接 | 桌面端保留 JavaFX 或另选方案 |

#### 迁移映射

| 当前（JavaFX） | 目标（UniApp/Vue 3） | 说明 |
|---------------|-------------------|------|
| FXML + Controller | .vue SFC (template + script + style) | 声明式 + 响应式 |
| ObservableList | ref() / reactive() | Vue 3 响应式系统 |
| EventBus | mitt / provide-inject | 轻量事件 |
| SQLite | uni-app SQLite 插件 / 后端 API | 单机需原生插件 |
| jpackage | uni-app 云打包 / HBuilderX | Android APK |

#### 优势

1. **移动端一步到位**：Android 作业 + 未来移动端需求直接覆盖
2. **Vue 3 生态**：组件库、状态管理、路由全成熟
3. **开发体验好**：Vite + HMR，热更新秒级生效
4. **团队可能已有 TS 基础**：课程作业可能涉及

#### 劣势

1. **桌面端缺失**：UniApp 不原生支持 Windows 桌面应用，需要 Electron/Tauri 绕行
2. **SQLite 单机困难**：UniApp 的 SQLite 插件在 Android 端可用，但桌面端（H5）无法直接访问本地文件
3. **业务逻辑全部重写**：Java Service 层无法复用，需要用 TypeScript 重写
4. **生产环境无痛覆盖**：Android 端覆盖简单（APK 安装），但桌面端覆盖方案变复杂
5. **性能**：复杂表格（6 Tab + 筛选 + 批量操作）在 H5 端性能可能不如原生

#### 迁移工作量估算

| 阶段 | 内容 | 预估工时 |
|------|------|---------|
| Phase 1 | UniApp 项目骨架 + 数据模型 | 2-3 天 |
| Phase 2 | 本地数据库层（SQLite 插件 / 后端 API 设计） | 3-5 天 |
| Phase 3 | 达人工作台（6 Tab + 筛选） | 5-7 天 |
| Phase 4 | 详情页 + 寄样折叠栏 | 3-4 天 |
| Phase 5 | 撤销系统 + 操作历史 | 2-3 天 |
| Phase 6 | Excel 导入（Android 端文件选择 + 解析） | 3-4 天 |
| Phase 7 | 桌面端方案（Tauri 壳 / 保留 JavaFX） | 3-5 天 |
| **总计** | | **21-31 天** |

---

### 2.3 方案对比总结

| 维度 | C# (Avalonia) | UniApp (Vue 3) |
|------|--------------|----------------|
| **桌面端体验** | ⭐⭐⭐⭐⭐ 原生 | ⭐⭐ 需 Tauri 壳 |
| **移动端** | ❌ 不支持 | ⭐⭐⭐⭐⭐ 原生 |
| **UI 美观** | ⭐⭐⭐⭐ 控件库丰富 | ⭐⭐⭐⭐ 组件库丰富 |
| **响应式** | ⭐⭐⭐⭐ MVVM 数据绑定 | ⭐⭐⭐⭐⭐ Vue 3 最自然 |
| **语法亲和** | ⭐⭐⭐⭐⭐ C# ≈ Java | ⭐⭐⭐ TS 需学习 |
| **业务层复用** | ⭐⭐⭐⭐ 领域模型可移植 | ⭐⭐ 全部重写 |
| **部署简单** | ⭐⭐⭐⭐ 自包含 exe | ⭐⭐⭐⭐ APK 简单 |
| **生产无痛覆盖** | ⭐⭐⭐⭐ 覆盖安装 | ⭐⭐⭐⭐⭐ APK 覆盖 |
| **迁移工时** | 19-28 天 | 21-31 天 |
| **学习曲线** | 1-2 周 | 1-2 周 |

---

## 三、推荐方案

### 3.1 如果桌面端是核心 → C# (Avalonia)

**理由**：
- 当前系统是纯桌面应用，运营人员每天在 Windows 桌面使用
- C# 语法与 Java 高度相似，迁移心理成本最低
- MVVM 数据绑定天然解决 EventBus 34 处手动刷新的痛点
- 业务层（app-core 92 文件）的领域模型和 Service 逻辑可以近乎直译到 C#
- 部署：`dotnet publish --self-contained` + Inno Setup，覆盖安装只需替换文件夹

### 3.2 如果移动端是刚需 → UniApp

**理由**：
- 有 Android 课程作业需求，UniApp 一步到位
- Vue 3 响应式是最自然的开发体验
- 但桌面端需要额外方案（Tauri），架构变复杂

### 3.3 混合方案（推荐考虑）

**桌面端 C# + 移动端 UniApp**，共享后端 API：

```
┌─────────────┐    ┌─────────────┐
│  C# 桌面端   │    │ UniApp 移动端 │
│  (Avalonia)  │    │  (Vue 3)    │
└──────┬───────┘    └──────┬──────┘
       │                    │
       └────────┬───────────┘
                │ REST API
       ┌────────┴────────┐
       │   后端服务（可选）  │
       │  .NET / Spring   │
       └────────┬────────┘
                │
       ┌────────┴────────┐
       │   SQLite / PG    │
       └─────────────────┘
```

但这引入了服务器依赖，与当前"单机优先"的设计哲学冲突。**短期不建议**。

---

## 四、决策建议

基于当前情况（桌面端生产环境已部署、运营团队日常使用、Android 作业是小需求）：

**推荐 C# (Avalonia) 迁移**，理由：
1. 桌面端是核心场景，不能降级
2. C# 语法亲和，迁移阻力最小
3. MVVM 解决响应式痛点
4. 业务层可近乎直译，迁移成本可控
5. Android 作业可以用 UniApp 单独做（小作业不影响主系统）

**迁移优先级**：
1. 先迁移桌面端到 Avalonia（保证生产环境可维护）
2. Android 作业用 UniApp 单独开项目（不耦合）
3. 未来如果移动端成为核心需求，再考虑后端 API 化

---

## 五、下一步行动

- [ ] 确认迁移方向（C# / UniApp / 混合）
- [ ] 搭建目标框架的 Hello World 骨架
- [ ] 验证 SQLite 连接 + Flyway 替代方案
- [ ] 迁移一个简单页面（如设置页）验证端到端流程
- [ ] 评估无痛覆盖安装的实现方式

---

*文档状态：评估完成，待决策*
*关联文档：TkerManageSys.md、TkerPivot.md、DESIGN_LESSONS.md*
