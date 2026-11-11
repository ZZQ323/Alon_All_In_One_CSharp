# Aolon Manager — 产品设计 & 功能清单

> 整理日期：2026-10-19
> 基于 docs/iterations/ 全部迭代文档 + 代码库（15,663 行 Java / 123 文件）综合梳理
>
> **C# 重构版本**: TKCompass（.NET 10 + Avalonia 12.1.2），开发中
> 最后同步：2026-11-10 | 状态：业务逻辑层 80% 完成，UI 层 30% 完成

---

## 〇、打包与发布（快速开始）

### 打包命令（Windows PowerShell / CMD）

```powershell
cd D:\Projects\Alon_All_In_One\Aolon_Manager

# 一键打包（编译 + shadow JAR + jpackage）
.\gradlew.bat :app-packaging:jpackage
```

**输出**：`app-packaging\build\jpackage\Aolon达人管理系统-3.7.0.msi`

### 环境要求

| 工具 | 版本 | 说明 |
|------|------|------|
| JDK | 21+ | 必须与目标平台匹配 |
| WiX 3 | 3.14+ | 仅 Windows MSI 需要，`winget install WiX.WiX3` |
| Gradle | 8.x | 项目自带 `gradlew.bat` |

### 分步执行

```powershell
.\gradlew.bat clean              # 清理
.\gradlew.bat build              # 编译
.\gradlew.bat test               # 测试（可选）
.\gradlew.bat :app-packaging:shadowJar    # 创建 fat JAR
.\gradlew.bat :app-packaging:jpackage     # 生成安装包
```

### 产物说明

- **安装包**：`Aolon达人管理系统-3.7.0.msi`（自带 JRE，用户无需安装 Java）
- **免安装版**：`app-packaging/build/jpackage/AolonManager/`（绿色版，可直接运行）

### 其他平台

```bash
# macOS
./gradlew :app-packaging:jpackage   # 输出 .pkg

# Linux
./gradlew :app-packaging:jpackage   # 输出 .deb
```

> ⚠️ **WSL2 用户注意**：`/mnt/d/` 路径有已知 I/O 问题，打包必须在 Windows 原生环境执行。

📖 详细打包文档：`app-packaging/README.md`

---

## 一、产品定位

**Aolon Manager** 是一款面向 TikTok 达人合作运营的 **单机桌面管理系统**（JavaFX 21 + SQLite），由运营团队内部使用。

**核心业务场景**：运营人员通过 TikTok 联系海外达人（KOL/KOC），完成「发送合作邀请 → 寄样 → 收视频 → 收广告代码 → 追踪出单」的全链路。系统把原本在 Excel 里手工维护的 2000+ 达人数据（含寄样、视频、代码的关联关系）搬到结构化数据库，并提供"今日待跟进"工作流。

**技术栈**：
- 前端：JavaFX 21（FXML + CSS 深色主题）
- 后端：纯 Java 分层架构（app-core / app-data / app-api / app-excel / app-packaging / main）
- 数据库：SQLite（单文件 aolon.db）
- 迁移：Flyway（V15–V25）
- 打包：jpackage

**设计哲学**：
- 单机优先，不依赖网络/服务器
- 数据可恢复（整库备份 + 应用层 Undo）
- 操作可审计（状态流转历史 + 操作日志快照）
- 导入可重复（清洗脚本 + 标准模板）

---

## 二、产品设计：状态体系

系统有 **6 套独立的状态维度**，各自解决不同的业务问题。这是整个产品设计的核心。

### 2.1 合作状态机（cooperation_status）

**解决的问题**：达人从"第一次联系"到"寄样完成"的合作推进进度。

```
未触达 ──(新建达人)──▶ 已触达 ──(发送合作)──▶ 已发送合作
                                                  │
                                            (同意合作+寄样)
                                                  │
                                                  ▼
                                              已寄样
```

| 状态 | 含义 | 进入条件 |
|------|------|----------|
| 未触达 | 系统默认初始态 | 实体创建时（但业务上立即流转） |
| 已触达 | 已建立联系，尚未发合作邀请 | 新建达人后立即设置；或"重新联系"回退 |
| 已发送合作 | 已发送合作邀请，等待回复 | 点击"进度推进" |
| 已同意合作 | 达人同意，已寄出样品 | 同意合作 → 弹出寄样表单 → 保存后流转 |
| 已寄样 | 已寄出达人视频 | 寄样记录创建后自动流转 |

**为什么这么设计**：
- **线性推进**：合作流程是单向的（联系→邀请→同意→寄样），不需要分支/回退，线性状态机最简洁。
- **延迟流转**：同意合作时不立即推进状态，而是等寄样表单保存成功后一次性流转到"已寄样"。避免用户取消表单导致状态与数据不一致。
- **新建即触达**：`CreatorService.createCreator()` 创建后立即设为"已触达"，符合业务语义（新建 = 已录入联系信息）。

### 2.2 今日任务状态（today_status）

**解决的问题**：每天"哪些达人需要跟进"的日维度工作流，独立于合作进度。

| today_status | 含义 | UI 表现 |
|--------------|------|---------|
| null / 待处理 | 今日待跟进 | 出现在"今日提醒" Tab |
| 已提醒 | 今日已联系，明日继续 | 从今日列表隐藏，出现在"已提醒" Tab |
| 已解决 | 今日有进度推进 | 仍在今日列表（不隐藏） |

**每日重置**：每天首次启动时，`today_status` 全部重置为 null，让所有未交付达人重新进入今日待跟进。

**为什么这么设计**：
- 合作状态是永久的，但"今日要不要跟进"是日维度的。分离后，每日重置不影响合作进度。
- 支持"已提醒=今日隐藏，明日继续"的工作流——运营不需要看到今天已经联系过的人，但明天他们又会回来。
- "已解决"不隐藏，因为有进度推进的达人可能需要当天继续跟进。

### 2.3 已交付标记（delivered boolean）

**解决的问题**：标记"暂时不合作"的达人，与合作进度正交。

| delivered | 含义 |
|-----------|------|
| false | 正常跟进中 |
| true | 已交付（暂时不合作），进入"已交付" Tab |

**为什么用独立 boolean 而非 cooperation_status='已交付'**：
- "已交付"是运营主观判断（"暂时不合作"），与合作进度（已触达→已寄样）无关。
- 用独立字段可以保留合作状态信息，支持"重新联系"时恢复到正确的合作阶段。
- 已交付达人从今日提醒、已提醒、总表中**全部排除**。
- 通过"重新联系"恢复：`delivered=false` + `cooperation_status='已触达'` + 清空 `today_status`。

### 2.4 跟进状态（follow_up_status）

**解决的问题**：按「达人 × 寄样」粒度追踪跟进任务。

| 状态 | 含义 |
|------|------|
| pending | 待跟进 |
| acknowledged | 已确认（已联系但未完成） |
| completed | 已完成 |

**为什么这么设计**：
- 一个达人可能有多个寄样，每个寄样的跟进进度不同。
- 粒度到「达人 × 寄样」才能精确追踪"哪个寄样催了视频、哪个寄样催了代码"。

### 2.5 履约状态（fulfillment_status，视图计算）

**解决的问题**：一眼看出达人的视频/代码交付缺口。

| 状态 | 含义 | 颜色 |
|------|------|------|
| 催视频 | 寄样后还没收到视频 | 🔴 red |
| 视频履约不足 | 视频数 < 要求数 | 🟡 yellow |
| 优先催代码 | 视频够了但缺代码 | 🟠 orange |
| 代码不足 | 部分视频缺代码 | 🟡 yellow |
| 基础履约完成 | 视频和代码都齐了 | 🟢 green |

**为什么这么设计**：
- 运营的核心焦虑是"谁还没交视频"和"谁还没给代码"。
- 用颜色+emoji 让状态一目了然，不需要心算。
- 由 `v_creator_fulfillment` 视图实时计算，不需要持久化。

### 2.6 广告代码状态（code_status）

**解决的问题**：单个视频是否已收到广告代码。

| 状态 | 含义 |
|------|------|
| none | 无代码 |
| pending | 等待代码 |
| received | 已收到代码 |

### 2.7 软删除（deleted boolean）

`deleted=1` 的记录从所有业务查询中过滤，但保留在数据库以支持恢复。

**冲突处理**：新建达人时若 handle 与已删除记录冲突，弹出对话框让用户选择"恢复原数据"或"覆盖"。

### 2.8 状态流转历史（status_flow_history 表）

每次状态变更自动记录：`creator_id, from_status, to_status, action_type, action_date, notes`。

支持的 action_type：新建、进度推进、已提醒、已交付、重新联系、每日重置。

### 2.9 操作日志（operation_log 表，V25）

应用层 Undo 基础设施，记录每次写操作的 before/after JSON 快照。

| 字段 | 作用 |
|------|------|
| operation_type | CREATE / UPDATE / DELETE / BATCH_IMPORT / STATE_TRANSITION / UNDO |
| before_json / after_json | 整行实体快照（撤销核心数据） |
| group_id | 批量操作分组（一次导入 100 条 = 一组） |
| session_id | 应用会话 ID（Ctrl+Z 只撤销当前会话） |

---

## 三、状态转换全景图

```
                    ┌─────────────────────────────────────────────┐
                    │          cooperation_status 状态机           │
                    └─────────────────────────────────────────────┘

    [新建达人]          [进度推进]          [同意合作+寄样]
  ┌──────────┐      ┌──────────────┐      ┌────────────────┐
  │  未触达   │─────▶│   已触达      │─────▶│  已发送合作     │
  └──────────┘      └──────┬───────┘      └───────┬────────┘
                           │                      │
                           │    [重新联系]         │
                           │◀─────────────────────┘
                           │
                           │                      ┌────────────────┐
                           └─────────────────────▶│   已寄样       │
                                      [寄样保存]   └────────────────┘

                    ┌─────────────────────────────────────────────┐
                    │          delivered 独立维度                   │
                    └─────────────────────────────────────────────┘

    任意 cooperation_status ──[已交付]──▶ delivered=true（进入已交付 Tab）
    delivered=true ──[重新联系]──▶ delivered=false + cooperation_status=已触达

                    ┌─────────────────────────────────────────────┐
                    │          today_status 日维度                  │
                    └─────────────────────────────────────────────┘

    null/待处理 ──[已提醒]──▶ 已提醒（今日隐藏，明日继续）
    null/待处理 ──[进度推进]──▶ 已解决（仍在今日列表）
    已提醒 ──[每日重置]──▶ null（次日重新进入待跟进）

                    ┌─────────────────────────────────────────────┐
                    │     follow_up_status（达人 × 寄样 粒度）      │
                    └─────────────────────────────────────────────┘

    pending ──[确认]──▶ acknowledged ──[完成]──▶ completed

                    ┌─────────────────────────────────────────────┐
                    │     fulfillment_status（视图实时计算）         │
                    └─────────────────────────────────────────────┘

    寄样后无视频 → 催视频(红)
    视频数 < 要求数 → 视频履约不足(黄)
    视频够了缺代码 → 优先催代码(橙)
    部分视频缺代码 → 代码不足(黄)
    视频+代码齐了 → 基础履约完成(绿)
```

---

## 四、功能清单

### 4.1 达人管理

| 功能 | 说明 |
|------|------|
| 新建达人 | 简化表单（handle + 主页 + 标签 + 备注），默认状态"已触达"，默认 nickname=handle |
| 编辑达人 | 详情页编辑基本信息 + 动态联系方式 + 关系信息 |
| 删除达人 | 软删除（deleted=1），支持恢复 |
| 批量删除 | 总表多选删除 |
| 软删除冲突处理 | 新建相同 handle 时检测已删除记录，提供"恢复"或"覆盖"选项 |
| 达人详情页 | 折叠栏设计：每个寄样一个 TitledPane，展开显示视频列表 |
| 达人搜索 | 按 handle（达人 ID）/ 昵称 / 联系方式搜索 |
| 达人排序 | 按最后联系时间降序（最近联系排最前） |
| 达人筛选 | 统一筛选栏：按质量等级（S/A/B/C/D）、联系位置、状态、寄样时间范围、最后联系时间范围、产品代码（多选）、产品名筛选 |

### 4.2 达人字段

| 字段 | 说明 |
|------|------|
| handle（达人 ID） | 主键，UNIQUE 约束，UI 显示为"达人 ID" |
| nickname | 昵称 |
| creatorTypeId | 达人类型（字典表，自动创建） |
| preferredLanguage | 语种（英语/西语/葡语等） |
| qualityLevel | 达人质量（S/A/B/C/D） |
| tiktokHomepageUrl | TikTok 主页链接 |
| contactLocation | 联系位置 |
| tiktokBackendId | TikTok 后台 ID（已废弃，迁移到 creator_contacts） |
| instagram | Instagram（已废弃，迁移到 creator_contacts） |
| whatsapp | WhatsApp 号码（已废弃，迁移到 creator_contacts） |
| email | 邮箱（已废弃，迁移到 creator_contacts） |
| otherContact | 其他联系方式（已废弃，迁移到 creator_contacts） |
| notes | 备注（TextArea 可编辑） |
| cooperationStatus | 合作状态 |
| todayStatus | 今日任务状态 |
| todayResolvedDate | 今日解决日期 |
| nextFollowUpDate | 下次跟进日期 |
| latestContactDate | 最后联系时间 |
| delivered | 是否已交付（boolean） |
| deliveredReason | 已交付/删除原因 |
| deleted | 软删除标记 |

**动态联系方式**（creator_contacts 表，V27 新增）：
- 支持多个同类型联系方式（如 2 个 WhatsApp、3 个邮箱）
- 联系方式类型：WhatsApp/邮箱/TT后台/达人后台/其他
- UI 采用加号添加模式：无数据时显示加号按钮，点击后选择类型并输入值

### 4.3 寄样管理

| 功能 | 说明 |
|------|------|
| 新建寄样 | 新建时隐藏"寄样状态"字段，默认"已寄样" |
| 编辑寄样 | 可修改状态等字段 |
| 删除寄样 | 软删除 |
| 寄样-视频关联 | 视频必须关联到具体寄样（shipmentId），UI 提供下拉选择 |
| 折叠栏展示 | 每个寄样一个折叠栏，顶部显示快递号/日期/产品代码/产品名/要求视频数/缺视频数/缺代码数 |
| 寄样完成标记 | 手动标记完成 + 记录原因和时间 |
| 产品代码多选 | 从 product_codes 字典表勾选多个产品代码，逗号分隔存储（如 `AGTR3BK,AGTR3GD`） |
| 产品名自动填充 | 根据选中的产品代码自动填充对应的中文显示名（如 `GTR3 黑色,GTR3 金色`） |

### 4.4 视频 & 广告代码管理

| 功能 | 说明 |
|------|------|
| 新增视频 | 关联到指定寄样 |
| 新增广告代码 | 关联到指定寄样 |
| 视频链接复制 | 操作列复制按钮 + 点击链接直接复制 |
| 广告代码 inline 编辑 | 视频表格中直接编辑代码列，自动保存 |
| 视频统计 | 总表展示：视频数、缺视频数、缺代码数 |
| 视频数据追踪 | 播放量/点赞/评论/分享/出单/GMV |

### 4.5 今日工作台（达人工作台）

| Tab | 内容 |
|-----|------|
| 今日提醒 | 今日待跟进达人，按"未同意合作"和"已同意合作（一个寄样/多个寄样）"分组 |
| 正在交付 | 已同意合作或已寄样的达人（delivered=false），支持"标记已交付" |
| 已提醒 | 今日已联系达人，明日继续 |
| 已交付 | 标记为"暂时不合作"的达人 |
| 总表 | 全部未删除达人，含视频统计列 |
| 已删除 | 软删除的达人，支持恢复/永久删除 |

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

**三按钮语义**：
| 按钮 | 含义 |
|------|------|
| 进度推进 | 有进度更新，达人仍在今日列表 |
| 已提醒 | 今日无进度，今日隐藏，明日继续 |
| 已交付/删除 | 未合作达人→"删除"（带原因）；已合作达人→标记 delivered=true |

### 4.6 通知系统

| 功能 | 说明 |
|------|------|
| 启动通知 | 应用启动时自动检查今日待跟进任务 |
| 通知弹窗 | 自定义 Stage，右下角，深色蓝色背景，5 秒自动消失 |
| 通知内容 | 任务数量 + 前 3 位达人的 handle 和状态 |
| 状态栏 | 显示今日待跟进数量 |

### 4.7 数据恢复 / 撤销（Undo）

| 功能 | 说明 |
|------|------|
| Ctrl+Z | 撤销当前会话最近一次操作，UI 即时刷新 |
| 操作历史面板 | 左侧导航新增入口，展示最近 50 条操作 |
| 一键回滚 | 点击任意操作的"回滚"按钮，回滚到该操作之前 |
| 批量操作分组 | 一次 Excel 导入的所有操作共享 group_id，可一键回滚整次导入 |
| 审计链 | 撤销操作本身也记录到 operation_log |
| 保留策略 | 默认保留 30 天或至少 1000 条，启动时自动 prune |
| 整库备份 | BackupService 提供每半小时自动备份（保留最近 48 个，覆盖 24 小时）+ 手动备份/还原 |
| 操作历史持久化 | 操作历史面板显示所有最近 50 条操作（不按会话过滤），重启后仍可查看 |

### 4.8 Excel 导入

| 功能 | 说明 |
|------|------|
| 标准模板 V2 | 15 列标准格式（TT账号/昵称/达人类型/语种/质量/WhatsApp/INS/Gmail/快递单号/SKU/寄样时间/视频链接/广告代码/视频发布日期/备注） |
| 三阶段自动导入 | 解析校验 → 字典自动创建 → 数据写入 |
| 字典自动创建 | 达人类型和产品代码不存在时自动插入 |
| 状态推断 | 根据是否有视频/寄样时间/SKU 自动推断 cooperation_status |
| 视频-寄样关联 | 时间就近原则（videoDate 匹配最近的 sampleDate） |
| 清洗脚本 | Python 脚本将原始 Excel 转为标准模板（可重复执行） |
| 导入报告 | 概览 + 字典创建清单 + 异常数据清单 + 待人工处理清单 |
| 异常处理 | handle 为空跳过、快递单号科学计数法标记、日期异常放入备注 |

### 4.9 关系管理（S/A/B 级达人）

| 功能 | 说明 |
|------|------|
| 关系信息维护 | 偏好称呼、视频风格、当前佣金率 |
| 跟进计划 | 下次跟进日期、下次行动计划 |
| 最佳视频管理 | 为 S/A/B 级达人维护最佳视频展示（CRM 用途） |
| 逾期提醒 | 自动检测下次跟进日期已过的达人 |
| 联系天数统计 | 距上次联系天数 |

### 4.10 标签管理

| 功能 | 说明 |
|------|------|
| 标签定义 | 系统标签 + 自定义标签，支持颜色 |
| 标签分类 | 按类型分组（creator / shipment / video） |
| 达人打标签 | 多对多关联，支持按产品/寄样维度 |
| 批量打标 | 一次给多个达人打同一组标签 |

### 4.11 字典管理

| 字典 | 说明 |
|------|------|
| creator_types | 达人类型（头部/腰部/尾部预置，支持自动创建） |
| product_codes | 产品代码 / SKU（P001/P002 预置，支持自动创建） |
| dictionary | 通用键值字典（系统配置） |

### 4.12 系统配置

| 功能 | 说明 |
|------|------|
| 统计卡片 | 达人列表顶部展示聚合统计 |
| 系统配置页 | 系统参数配置 |
| 备份管理 | 手动备份 / 选择备份文件还原 / 自动每日备份（最多保留 10 份） |
| 撤销保留策略 | 设置页调整 operation_log 保留天数/条数 |

---

## 五、核心设计决策 & 理由

### 5.1 为什么用独立 boolean 而非状态字符串表示"已交付"？
"已交付"是运营主观判断（"暂时不合作"），与合作进度（已触达→已寄样）正交。用独立字段可以保留合作状态信息，支持"重新联系"时恢复到正确的合作阶段。

### 5.2 为什么用 today_status 独立维度？
合作状态是永久的，但"今日要不要跟进"是日维度的。分离后，每日重置不影响合作进度，且支持"已提醒=今日隐藏，明日继续"的工作流。

### 5.3 为什么用 Operation Log + Snapshot 而非 Event Sourcing？
Event Sourcing 改造成本灾难级（整个 Repository/Service 层重写，18 个单元测试全部失效）。快照方案改造成本可控（只在 Service 层加拦截），持久化，业务语义清晰。

### 5.4 为什么延迟状态流转（同意合作不立即推进）？
避免用户取消寄样表单后状态已推进但无寄样记录的数据不一致。取消 = 不改变状态，符合用户预期。

### 5.5 为什么用持久化 ObservableList 而非每次 setItems()？
JavaFX TableView 虚拟单元格在替换整个 ObservableList 时不会立即重绘，导致按钮消失。持久化列表 + setAll() 让 TableView 检测到内容变化并正确触发 updateItem()。

### 5.6 为什么导入模板是纵向展开（一行=一个视频）？
原始 Excel 横向展开（多视频占多列）无法确定视频-寄样关联关系。纵向展开后每行一个明确的视频-寄样对应，导入逻辑简单可靠。

### 5.7 为什么快递单号是关键字段？
快递单号是关联视频、代码的关键 ID。原始数据中快递单号被 Excel 存为科学计数法导致精度丢失无法恢复，是导入的前提条件（必须从飞书重新导出为文本格式）。

### 5.8 为什么跟进状态粒度到「达人 × 寄样」？
一个达人可能有多次寄样，每次寄样的视频/代码交付进度不同。只有按寄样粒度追踪，运营才能精确知道"哪个寄样催视频、哪个寄样催代码"。

---

## 六、数据库迁移历史

| 版本 | 内容 |
|------|------|
| V15 | creators 表新增 cooperation_status, creator_type_id, today_status, today_resolved_date, next_follow_up_date |
| V16 | 创建 creator_types 字典表 |
| V17 | 创建 product_codes 字典表 |
| V18 | creators 表新增 contact_location |
| V19 | 创建 status_flow_history 表 |
| V24 | creators 表新增 tiktok_backend_id, instagram |
| V25 | 创建 operation_log 表（应用层 Undo 基础设施） |
| V26 | shipments 表新增 product_name 字段（产品中文显示名） |
| V27 | 创建 creator_contacts 表（动态联系方式，支持多值同类型），迁移 creators 表的 whatsapp/email/tiktok_backend_id/instagram/other_contact 数据到新表 |

---

## 七、模块结构

```
Aolon_Manager/
├── app-core/       领域模型 + Service + Repository 接口
├── app-data/       SQLite 实现 + Flyway 迁移脚本
├── app-api/        Javalin REST API（预留）
├── app-excel/      Excel 导入导出
├── app-packaging/  jpackage 打包配置
└── main/           JavaFX UI（FXML + Controller）
```

**代码规模**：109 个 Java 文件 / 约 12,000 行 Java 代码（截至 2026-10-18）

---

## 八、前端刷新机制（v-model）

### 8.1 核心原则

**所有修改数据表的操作，在保存后都应当能触发任意页面、任意 tab 的刷新。** 这是最基础的前端机制。

### 8.2 EventBus 全局事件总线

采用发布/订阅模式，解耦页面间通信：

```
┌─────────────────────────────────────────────────────────────┐
│                    EventBus（单例）                          │
│                                                             │
│  事件类型：                                                  │
│  CREATOR_CREATED / UPDATED / DELETED                        │
│  SHIPMENT_CREATED / UPDATED / DELETED                       │
│  VIDEO_CREATED / UPDATED / DELETED                          │
│  CONTACT_CREATED / UPDATED / DELETED                        │
│  DICTIONARY_UPDATED / CREATOR_TYPE_UPDATED / PRODUCT_CODE_  │
│  BATCH_IMPORT_COMPLETED                                     │
│                                                             │
│  通配符 "*"：订阅所有事件                                     │
└─────────────────────────────────────────────────────────────┘
        ▲ publish                    ▲ publish
        │                            │
   ┌────┴─────┐               ┌──────┴──────┐
   │ Config   │               │ Detail      │  ... 所有 Controller
   │ View     │               │ Controller  │
   └──────────┘               └─────────────┘
        │                            │
        └──────────┬─────────────────┘
                   │ subscribe("*")
                   ▼
        ┌──────────────────────┐
        │ CreatorDashboard     │
        │ → refreshAllTabs()   │
        │ → 6 个 Tab 全部刷新   │
        └──────────────────────┘
```

### 8.3 事件发布覆盖（34 处）

| Controller | 发布事件 | 数量 |
|------------|----------|------|
| CreatorDashboardController | CREATOR_UPDATED/DELETED（状态流转、批量操作、标记交付等） | 12 |
| ConfigViewController | CREATOR_TYPE_UPDATED / PRODUCT_CODE_UPDATED / DICTIONARY_UPDATED | 8 |
| CreatorDetailController | CREATOR_UPDATED / CONTACT_CREATED / SHIPMENT_DELETED / VIDEO_DELETED | 4 |
| VideoEditDialogController | VIDEO_CREATED / VIDEO_UPDATED | 2 |
| ShipmentEditDialogController | SHIPMENT_CREATED / SHIPMENT_UPDATED | 2 |
| ExcelImportController | BATCH_IMPORT_COMPLETED | 1 |
| CreatorListController | CREATOR_DELETED | 1 |
| ShipmentDetailController | VIDEO_DELETED | 1 |
| CreatorEditDialogController | CREATOR_CREATED | 1 |
| **合计** | | **34** |

### 8.4 字典变更的特殊处理

字典（达人等级/联系渠道/代码状态/达人类型/产品代码）变更后，除了刷新数据 Tab，还需要重新加载筛选器下拉框选项：

```java
if (eventType.startsWith("dictionary.") || 
    eventType.startsWith("creatorType.") || 
    eventType.startsWith("productCode.")) {
    initializeFilters();          // 重新加载等级/状态下拉框
    initializeProductCodeFilter(); // 重新加载产品代码多选
}
```

### 8.5 设计优势

1. **解耦**：页面之间不直接引用，通过事件通信
2. **可扩展**：新增页面只需订阅事件即可自动刷新，无需修改其他页面
3. **数据一致性**：任何数据变更都能自动传播到所有 UI 视图
4. **线程安全**：通过 `Platform.runLater()` 确保在 JavaFX 应用线程执行刷新

---

## 九、撤销系统（v3.7.2 更新）

### 9.1 核心原则

**撤销日志不再绑定 session ID。** 单机桌面应用，数据已持久化，撤销日志应跨重启可访问。

### 9.2 历史演进

1. **最初设计**：撤销栈在内存中，session 隔离是合理的（每次启动清空）
2. **功能扩展**：操作日志持久化到数据库（V25），session_id 顺带写入表中
3. **设计滞后**：撤销查询逻辑仍沿用"按 session 过滤"的老代码，未随功能扩展重新设计
4. **v3.7.2 修复**：移除 session_id 依赖，所有撤销操作改为全局（不按 session 过滤）

### 9.3 为什么不用 Windows 用户区分

- 每个 Windows 用户已经有独立的数据库文件（`aolon.db`）
- 数据库文件本身就隐含了用户隔离
- 在同一个数据库内再按 session 区分是多余的

### 9.4 实现细节

**数据兼容性策略**：
- 保留 `session_id` 字段（向后兼容，避免数据丢失）
- 新插入的数据 `session_id` 设为 NULL
- 所有查询方法不再按 `session_id` 过滤

**核心改动**：
1. `OperationContext` - 移除 `sessionId` 字段和构造函数参数
2. `DefaultOperationLogger` - 移除 `sessionId` 字段，创建日志时 `session_id` 设为 NULL
3. `OperationLogRepository` - 废弃 `findBySessionId()` 和 `findAfter()` 方法
4. `UndoService` - `listRecent()` 移除 `sessionId` 参数，直接调用 `findRecent()`
5. `MainWindowController` - Ctrl+Z 直接调用 `undoLast()`，不再传递 `sessionId`
6. `OperationHistoryController` - 移除 `sessionId` 字段，查询所有操作

### 9.5 用户行为变化

**v3.7.2 之前**：
- 每次启动生成新的 UUID 作为 sessionId
- 重启后无法撤销之前的操作（Ctrl+Z 只能撤销当前会话）
- 操作历史面板显示所有操作（已改为不按 session 过滤）

**v3.7.2 之后**：
- 不再生成 sessionId
- 重启后仍可撤销之前的操作（Ctrl+Z 撤销全局最近操作）
- 操作历史面板显示所有操作（包括重启前的）
- 可以回滚任意历史操作（不受 session 限制）

---

## 十、当前状态（2026-10-19）

**已完成**：
- V3 状态机 + 达人工作台 + 通知系统 + 已交付 Tab + 详情页折叠栏
- 操作历史/撤销系统（Phase 1-7 全部完成）
- 统一筛选栏（5 个 Tab 共用，支持 7 个字段组合筛选）
- 产品代码多选对话框（从字典表勾选，逗号分隔存储）
- 新增"正在交付"Tab + "已删除"Tab
- 分层按钮布局（筛选在第一层，批量操作在第二层）
- Handle 字段改名为"达人 ID"
- 详情页保存按钮精简（仅"保存关闭"）
- 动态联系方式管理（creator_contacts 表，支持多值同类型）
- 按钮语义变更（未合作达人→删除，已合作达人→已交付）
- **v3.2 修复**：INSERT/UPDATE 列数与占位符不匹配问题（2026-10-18）
  - 修复 INSERT SQL：32 列 = 32 个占位符（添加 existing_products 字段）
  - 修复 UPDATE SQL：WHERE id=? 使用正确的位置参数（位置 32）
  - 修复 created_at/updated_at 参数顺序（位置 31/32）
  - 添加 existing_products 字段支持（INSERT/UPDATE/mapRow）
- **v3.7.1 更新**：数据库备份频率调整 & 操作历史持久化（2026-10-19）
  - 数据库备份频率从每日一次调整为每半小时一次（保留最近 48 个备份，覆盖 24 小时）
  - 操作历史面板改为显示所有最近 50 条操作（不按会话过滤），重启后仍可查看
  - 版本号从 3.8.0 调整为 3.7.1
  - 新增 BackupService 定时调度功能（scheduleHourlyBackup / stopScheduledBackup）
  - 新增 OperationLogRepository.findRecent() 和 UndoService.listRecentAll() 方法
- **v3.7.2 更新**：移除撤销日志的 session ID 绑定（2026-10-19）
  - 移除 OperationContext 的 sessionId 字段
  - 移除 DefaultOperationLogger 的 sessionId 字段
  - 废弃 OperationLogRepository.findBySessionId() 和 findAfter() 方法
  - UndoService.listRecent() 移除 sessionId 参数，改为全局查询
  - Ctrl+Z 撤销操作不再限制当前会话，支持跨重启撤销
  - 操作历史面板显示所有操作（包括重启前的）
  - 创建 Flyway 迁移 V33 - 记录 session_id 字段废弃（保留字段，向后兼容）
  - 详见迭代文档：docs/iterations/1.2/iter_undo_session_removal.md

**待验证**：Windows 环境下编译、启动、迁移脚本执行、完整导入流程、jpackage 打包

**待实施**：达人导入模板 V2 的系统导入功能（Phase 2：parseTemplateV2 + 三阶段导入逻辑）+ 清洗脚本 choose_v2.py
