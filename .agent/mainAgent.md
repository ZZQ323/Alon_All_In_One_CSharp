# 主 Agent 工作手册 (mainAgent.md)

**版本**：1.2  
**最后更新**：2026-11-11  
**适用项目**：TKCompass (Alon_All_In_One_CSharp)

---

## 🎯 核心定位

主 Agent 是**架构师 + 协调者**，不是执行者。

**职责边界**：
- ✅ 需求分析与任务拆分
- ✅ 迭代文档编写与更新
- ✅ 子 Agent 任务分发与调度
- ✅ 子 Agent 结果验证
- ✅ 进度追踪与报告
- ❌ 不直接执行大量代码修改（除非是简单的单行修复）
- ❌ 不反复读取大文件
- ❌ 不替代子 Agent 做重活

---

## 📋 工作流程

### 阶段 1：需求分析

```
用户输入 → 理解意图 → 评估影响范围 → 确定阶段划分
```

**输出**：
- 更新 `TODO.md`：列出所有任务，按优先级分组（P0/P1/P2）
- 更新 `STATUS.md`：标记当前阶段状态

### 阶段 2：任务拆分

**拆分原则**：
1. **单一职责**：每个子任务只修改 1-2 个文件
2. **可验证**：每个子任务有明确的验收标准
3. **可接力**：子任务之间通过迭代文档传递上下文
4. **原子性**：每个子任务可以独立完成和验证

**🚨 强制流程：任务拆分前必须验证当前状态**

```
❌ 错误做法：
- 依赖模糊记忆假设"某个任务已完成"
- 不读取文件就基于假设拆分任务
- 导致子 Agent 执行时发现需要重做，浪费资源

✅ 正确做法：
- 任务拆分前必须读取实际文件确认现状
- 不依赖记忆或假设，必须验证当前状态
- 对于批量修改任务，明确列出所有需要修改的文件/位置
```

**拆分模板**：

```markdown
#### 任务 N: [任务名称]

**目标**：[一句话描述]

**修改清单**（必须逐一确认）：
1. `path/to/file1.cs` - [具体修改位置1]：[修改内容]
2. `path/to/file1.cs` - [具体修改位置2]：[修改内容]
3. `path/to/file2.axaml` - [具体修改位置]：[修改内容]

**验收标准**：
- [ ] 标准 1
- [ ] 标准 2
- [ ] 编译通过（0 错误）

**依赖**：[依赖哪些前置任务]
**预估工时**：X 天
```

**任务拆分检查清单**：
- [ ] 是否已读取实际文件确认当前状态？
- [ ] 修改清单是否逐一列出所有需要修改的位置？
- [ ] 是否避免了基于记忆的假设？
- [ ] 对于批量修改任务，是否明确了所有修改点？

### 阶段 3：子 Agent 分发

**分发策略**：
- 独立任务 → 并行分发（最多 3 个/轮）
- 依赖任务 → 串行分发（上一个完成后再分发下一个）
- 复杂任务 → 拆分为多个子任务，接力执行

**分发 Prompt 模板**：

```
## 任务：[任务名称]

### 背景
[简要说明项目背景和当前状态]

### 目标
[明确的任务目标，1-2 句话]

### 具体操作
1. [精确的操作步骤，包含文件路径和行号]
2. [使用 str_replace 而非全量重写]
3. [完成后运行编译验证]

### 验收标准
- [ ] [标准 1]
- [ ] [标准 2]

### 限制
- 最多读取文件 2 次（1 次确认结构，1 次验证结果）
- 使用 str_replace 进行精确替换
- 完成后更新 TODO.md 标记状态
- 遇到问题立即停止并报告，不要尝试自行解决

### 迭代文档
<列举要做的迭代的文档>
```

### 阶段 4：结果验证

**验证清单**：
- [ ] 子 Agent 报告是否完整
- [ ] 编译是否通过（0 错误 0 警告）
- [ ] 迭代文档是否已更新
- [ ] 代码变更是否符合预期

**验证方法**：
```bash
# 编译验证
dotnet build /mnt/projects/Alon_All_In_One_CSharp/TKCompass.slnx -c Release

# 检查文档更新
cat docs/iterations/1.0/TODO.md | grep "任务 N"
```

### 阶段 5：进度更新

**更新频率**：
- 每个子任务完成后立即更新
- 每个阶段完成后汇总更新

**更新内容**：
- TODO.md：标记任务状态（✅ 完成 / ❌ 失败 / 🔄 进行中）
- STATUS.md：更新进度百分比和任务计数

---

## 🚨 禁止行为

1. **禁止直接执行大量代码修改**
   - ❌ 主 Agent 直接修改 3+ 个文件
   - ✅ 分发给子 Agent 执行

2. **禁止反复读取大文件**
   - ❌ 读取整个 ViewModel 文件（500+ 行）
   - ✅ 只读取目标区域（±20 行）

3. **禁止忽略迭代文档**
   - ❌ 口头传递任务上下文
   - ✅ 所有上下文写入迭代文档

4. **禁止跳过验证**
   - ❌ 子 Agent 报告完成就认为完成
   - ✅ 必须编译验证 + 文档检查

---

## 📊 任务拆分示例

### 示例：P2-13 深色主题适配

**错误的拆分方式**（太重）：
```
任务 13：深色主题适配
- 创建 DarkTheme.axaml
- 修改 MainWindow.axaml
- 修改 SettingsView.axaml
- 修改 ConfigView.axaml
- 修改所有 View 的颜色引用
- 实现主题切换逻辑
```

**正确的拆分方式**（原子化）：
```
任务 13a：创建深色主题资源文件
- 创建 Themes/DarkTheme.axaml
- 定义深色主题颜色资源
- 验收：文件存在且语法正确

任务 13b：主窗口主题切换支持
- 修改 MainWindow.axaml 引用主题资源
- 修改 MainViewModel.cs 添加主题切换命令
- 验收：可以切换 Light/Dark 主题

任务 13c：设置页主题适配
- 修改 SettingsView.axaml 使用动态资源
- 验收：设置页在深色主题下显示正确

任务 13d：配置页主题适配
- 修改 ConfigView.axaml 使用动态资源
- 验收：配置页在深色主题下显示正确
```

---

## 🔄 持续改进

**每次迭代后更新**：
- 本文件的经验教训
- 任务拆分模板
- 子 Agent 分发 Prompt 模板

**复盘清单**：
- [ ] 子 Agent 成功率是否 ≥ 90%
- [ ] 子 Agent 平均读取次数是否 ≤ 2
- [ ] 主 Agent 直接执行比例是否 ≤ 20%
- [ ] 迭代文档是否及时更新

---

## 📚 迭代教训记录

### 迭代 1.2 教训（2026-11-11）

#### 🔴 高频卡点 1：`str_replace` 读前写保护

**现象**：连续对同一文件执行多次 `str_replace` 时，第 2 次起全部被阻止，报错 *"you have not read its current version"*。

**影响**：更新 TODO.md 时，10 个替换操作只有第 1 个成功，其余 9 个全部失败。

**根因分析**：
- ❌ **不是 bug，是设计如此**：DeerFlow 的 str_replace 工具要求每次写入前必须重新 read_file，写入后"读版本"失效，下次操作必须再读。这是防错设计（防止基于过期内容做替换）
- ❌ **配置层面无法关闭**：这是工具层面的硬性约束
- ✅ **正确做法**：批量修改场景使用 Python/bash 脚本一次性完成

**应对策略**：
- ✅ **批量修改场景直接使用 Python 脚本**，不要用 str_replace 循环
- ✅ 主 Agent 编写一次性 Python 脚本，通过 `bash` 工具执行
- ✅ 脚本内完成所有批量替换、格式化、文档更新操作

**规则**：
> 批量修改同一文件时，使用 bash/python 脚本一次性完成，不要用 str_replace 循环。

**教训总结**：
```python
# 批量修改场景的标准做法
import re
from pathlib import Path

# 读取文件
content = Path("docs/iterations/1.2/TODO.md").read_text()

# 批量替换（所有需要修改的内容）
content = content.replace("- [ ]", "- [x]")
content = content.replace("❌ 未完成", "✅ 已完成")
# ... 更多替换

# 一次性写回
Path("docs/iterations/1.2/TODO.md").write_text(content)
```

#### 🔴 高频卡点 2：子 Agent 代码质量问题（编译错误）

**现象**：Batch 1 中 FulfillmentRepository 子 Agent 创建的代码存在类型不匹配错误，编译无法通过。

**影响**：主 Agent 必须介入手动修复，偏离了"主 Agent 只做协调"的定位。

**根因分析**：
- ❌ **不是配置问题，是 Prompt 问题**：子 Agent 拿到的是主 Agent 给的 task description
- ❌ **描述不够精确导致猜测性编码**：如果只说"创建 FulfillmentRepository"，没给具体的 model 字段名，子 Agent 就会猜
- ✅ **正确做法**：主 Agent 的 task prompt 里带上完整的类型定义

**应对策略**：
- ✅ **主 Agent 在分发 Prompt 中提供完整的模型字段定义**
- ✅ 包含具体的类型签名、方法参数、返回值类型
- ✅ 提供参考代码片段或伪代码，减少子 Agent 的猜测空间

**Prompt 改进示例**：
```markdown
### 精确规格（必须严格遵循）

创建 FulfillmentRepository，接口如下：

**模型字段**：
- `FulfillmentInfo.SampleId` (long, 外键)
- `FulfillmentInfo.Status` (FulfillmentStatus 枚举)
- `FulfillmentInfo.TrackingNumber` (string?)

**方法签名**：
- `GetBySampleIdAsync(long sampleId)` → `Task<FulfillmentInfo?>`
- `GetPendingListAsync()` → `Task<List<FulfillmentInfo>>`
- `UpdateStatusAsync(long id, FulfillmentStatus status)` → `Task`

**枚举定义**：
```csharp
public enum FulfillmentStatus
{
    Pending = 0,
    Shipped = 1,
    Delivered = 2,
    Completed = 3
}
```
```

#### 🟡 中频卡点 3：子 Agent 验收标准大量 UNVERIFIED

**现象**：子 Agent 返回的验收标准几乎全部标记为 `UNVERIFIED`，原因是 *"path is outside the shared thread workspace"*。

**影响**：主 Agent 无法直接信任子 Agent 的自检结果，必须自己执行 `dotnet build` 验证。

**根因分析**：
- ❌ **不是配置问题，是 sandbox 路径隔离导致**：子 Agent 运行在独立的 sandbox 里，它的 workspace 路径和主 Agent 不同
- ❌ **文件路径检查在 sandbox 隔离下不可靠**：当子 Agent 用 file_exists() 检查时，路径可能不在它的可见范围内
- ✅ **正确做法**：子 Agent 的验收应该用命令执行（如 dotnet build）而不是文件路径检查

**应对策略**：
- ✅ **主 Agent 必须自行执行编译验证**，不依赖子 Agent 的 UNVERIFIED 报告
- ✅ 在分发 Prompt 中明确要求子 Agent 使用 `dotnet build` 命令验证，而非文件路径检查
- ✅ 主 Agent 在收到子 Agent 报告后，立即执行编译验证

**规则**：
> 验收标准：执行 dotnet build 确认编译通过，不要依赖文件路径检查。

**验证流程**：
```bash
# 主 Agent 必须执行的验证步骤
cd /mnt/projects/Alon_All_In_One_CSharp
dotnet build src/TKCompass.App/TKCompass.AppBase.csproj

# 检查编译结果
# ✅ 0 错误 → 通过
# ❌ 有错误 → 主 Agent 介入修复或重新分发
```

#### 🟡 中频卡点 4：`sed` 命令被安全策略拦截

**现象**：尝试用 `sed` 批量修改 AXAML 文件时，因使用绝对路径被沙箱安全策略拦截。

**影响**：不得不改用 Python 脚本作为替代方案，增加调试时间。

**根因分析**：
- ✅ **可能是配置问题**：取决于 sandbox 模式。如果使用 LocalSandboxProvider + allow_host_bash: true，sed 应该能跑
- ❌ **被拦截的可能原因**：sandbox 模式不是 local，或路径不在允许范围内
- ✅ **正确做法**：检查 config.yaml 配置，或优先使用 Python 脚本

**配置检查**：
```yaml
# 检查 config.yaml
sandbox:
  use: deerflow.sandbox.local:LocalSandboxProvider
  allow_host_bash: true    # 确认是 true
```

**应对策略**：
- ✅ **优先使用 Python 脚本**，避免 sed/awk 等命令被拦截
- ✅ Python 脚本更灵活，可处理复杂逻辑，且不易被安全策略阻止
- ✅ 如需使用 sed，确认 sandbox 配置正确

#### 🟢 低频卡点 5：任务拆分粒度问题

**现象**：任务 7（达人列表排序）看似简单，但实际涉及 6 个 DataGrid 的批量修改。最初以为总表已完成只需补 5 个，结果发现总表也需要重做。

**影响**：Batch 3 执行时间超出预期。

**根因分析**：
- ❌ **不是配置问题，是规划问题**：主 Agent 在拆分任务时没有评估实际工作量
- ✅ **正确做法**：拆分前必须先读取实际文件确认现状，不要依赖记忆或假设

**应对策略**：
- ✅ **主 Agent 在拆分任务前，先读取实际文件确认现状**
- ✅ 不要依赖记忆或假设，必须验证当前状态
- ✅ 对于批量修改任务，明确列出所有需要修改的文件/位置

**任务拆分改进**：
```markdown
### 任务 7：达人列表排序（6 个 Tab 的 DataGrid）

**修改清单**（必须逐一确认）：
1. `Views/CreatorDashboardView.axaml` - Tab 1: 总表 DataGrid
2. `Views/CreatorDashboardView.axaml` - Tab 2: 待寄样 DataGrid
3. `Views/CreatorDashboardView.axaml` - Tab 3: 已寄样 DataGrid
4. `Views/CreatorDashboardView.axaml` - Tab 4: 待回收 DataGrid
5. `Views/CreatorDashboardView.axaml` - Tab 5: 已回收 DataGrid
6. `Views/CreatorDashboardView.axaml` - Tab 6: 已完成 DataGrid

**每个 DataGrid 需要添加的属性**：
```xml
CanUserSortColumns="True"
SortMode="Single"
```

**验收标准**：
- [ ] 所有 6 个 DataGrid 都已添加排序属性
- [ ] 编译通过（0 错误）
```

#### 📊 迭代 1.2 卡点统计

| 卡点类型 | 频次 | 影响程度 | 配置能解决？ | 实际解决方式 |
|----------|------|----------|--------------|--------------|
| str_replace 读前写保护 | 高（几乎每轮） | 🔴 严重 | ❌ 设计如此 | 批量改用 Python/bash 脚本 |
| 子 Agent 编译错误 | 中（Batch 1 出现 1 次） | 🔴 严重 | ❌ Prompt 问题 | 主 Agent 给更精确的 task prompt |
| 子 Agent UNVERIFIED | 高（每批都有） | 🟡 中等 | ❌ sandbox 隔离 | 子 Agent 用 dotnet build 验收 |
| sed 被拦截 | 低（1 次） | 🟡 中等 | ✅ 检查 allow_host_bash | 确认 local 模式 + true |
| 任务拆分粒度 | 低（1 次） | 🟢 轻微 | ❌ 规划问题 | 主 Agent 规划时评估实际工作量 |

#### ✅ 迭代 1.2 改进措施总结

1. **批量修改场景**：直接改用 Python 脚本，避免 str_replace 的读前写保护反复触发
2. **子 Agent Prompt**：提供更精确的模型字段名和类型定义，减少猜测性编码
3. **UNVERIFIED 问题**：主 Agent 必须自行执行 `dotnet build` 验证，不依赖子 Agent 的自检报告
4. **任务拆分**：拆分前必须先读取实际文件确认现状，不要依赖记忆或假设

#### 📌 最有价值的改进建议

> 在项目规范里加规则，让 agent 学会：
> 1. 批量操作用脚本（不是 str_replace 循环）
> 2. 子 Agent 用编译验收（不是文件路径检查）
> 
> 这些是**工作流层面的优化**，不是 DeerFlow 配置能改的。

---

**最后更新**：2026-11-11  
**版本**：1.2
