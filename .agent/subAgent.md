# 子 Agent 执行手册 (subAgent.md)

**版本**：1.0  
**最后更新**：2026-11-11  
**适用项目**：TKCompass (Alon_All_In_One_CSharp)

---

## 🎯 核心定位

子 Agent 是**精确执行者**，不是决策者。

**职责边界**：
- ✅ 严格按照分发 Prompt 执行操作
- ✅ 使用 `str_replace` 进行精确替换
- ✅ 完成后编译验证
- ✅ 更新迭代文档标记进度
- ✅ 遇到问题立即报告
- ❌ 不自行决定修改范围
- ❌ 不探索性读取大量文件
- ❌ 不尝试修复 Prompt 未提及的问题

---

## 📋 执行流程

### 步骤 1：理解任务（不读取文件）

从分发 Prompt 中获取：
- 目标是什么
- 需要修改哪些文件
- 具体修改内容
- 验收标准

### 步骤 2：确认文件结构（第 1 次读取）

**只读取目标区域**，不读取整个文件：

```
# ✅ 正确：读取目标区域 ±20 行
read_file(path, start_line=100, end_line=140)

# ❌ 错误：读取整个文件
read_file(path)
```

### 步骤 3：执行修改（使用 str_replace）

**必须使用 str_replace**，不使用 write_file 重写整个文件：

```
# ✅ 正确：精确替换
str_replace(path, old_str="具体代码", new_str="新代码")

# ❌ 错误：全量重写
write_file(path, content="整个文件内容")
```

### 步骤 4：验证结果（第 2 次读取）

**只读取修改区域确认正确**：

```
read_file(path, start_line=修改行号-5, end_line=修改行号+5)
```

### 步骤 5：编译验证

```bash
dotnet build /mnt/projects/Alon_All_In_One_CSharp/TKCompass.slnx -c Release
```

### 步骤 6：更新迭代文档

在 TODO.md 中标记任务状态：
- `[ ]` → `[x]`
- 添加 `✅ 完成` 标记

---

## 🚨 硬性限制

### 1. 文件读取限制

| 规则 | 限制 |
|------|------|
| 总读取次数 | ≤ 3 次（确认结构 + 验证结果 + 必要时排查错误） |
| 每次读取范围 | ≤ 50 行（除非文件本身很短） |
| 禁止全量读取 | 超过 100 行的文件禁止不指定行号读取 |

**违反后果**：浪费 token 预算，可能导致任务失败。

### 2. 文件修改限制

| 规则 | 限制 |
|------|------|
| 修改方式 | 必须使用 `str_replace` |
| 单次修改范围 | ≤ 30 行 |
| 禁止全量重写 | 禁止使用 `write_file` 覆盖已有文件 |

**例外**：创建新文件时可以使用 `write_file`。

### 3. 错误处理限制

| 规则 | 限制 |
|------|------|
| 重试次数 | 同一操作最多重试 2 次 |
| 自行修复范围 | 仅限 Prompt 中明确提到的文件 |
| 遇到问题 | 立即停止并报告，不要扩大修复范围 |

---

## 📝 报告模板

完成任务后，按以下格式报告：

```markdown
## 任务完成报告

### 任务：[任务名称]

### 执行结果：✅ 成功 / ❌ 失败

### 修改文件列表
1. `path/to/file1.cs`：[修改内容摘要]
2. `path/to/file2.axaml`：[修改内容摘要]

### 编译结果
- 错误数：0
- 警告数：0

### 文档更新
- [ ] TODO.md 已更新
- [ ] STATUS.md 已更新（如需要）

### 执行统计
- 文件读取次数：N
- str_replace 次数：N
- 编译次数：N

### 备注
[如有异常情况，在此说明]
```

---

## 🔄 接力执行模式

当任务被拆分为多个子任务时，使用接力模式：

```
子 Agent A 执行任务 13a
  ↓ 完成后更新 TODO.md
  ↓ 报告给主 Agent

主 Agent 验证 13a 结果
  ↓ 分发任务 13b

子 Agent B 执行任务 13b
  ↓ 读取 TODO.md 获取上下文
  ↓ 执行修改
  ↓ 完成后更新 TODO.md
  ↓ 报告给主 Agent

...继续接力
```

**关键点**：
- 每个子 Agent 只需关注当前任务
- 上下文通过迭代文档传递
- 主 Agent 负责协调和验证

---

## 📊 执行效率指标

| 指标 | 目标值 | 说明 |
|------|--------|------|
| 文件读取次数 | ≤ 3 | 确认 + 验证 + 排查 |
| str_replace 次数 | ≤ 5 | 精准修改 |
| 编译次数 | 1-2 | 首次验证 + 修复后验证 |
| 总 token 消耗 | ≤ 20K | 高效执行 |

---

## ❌ 典型错误案例

### 案例 1：过度读取

```
# 错误示范
read_file("MainViewModel.cs")  # 读取 500 行
read_file("MainViewModel.cs", start_line=1, end_line=100)  # 又读 100 行
read_file("MainViewModel.cs", start_line=100, end_line=200)  # 再读 100 行
...重复 5+ 次

# 正确做法
read_file("MainViewModel.cs", start_line=95, end_line=145)  # 只读目标区域 ±25 行
# 执行 str_replace
read_file("MainViewModel.cs", start_line=95, end_line=145)  # 验证修改
```

### 案例 2：超出修改范围

```
# 错误示范
Prompt 要求修改 A 文件的 X 函数
子 Agent 发现 B 文件也有类似问题
子 Agent 自行修改了 B 文件  # ❌ 超出范围

# 正确做法
Prompt 要求修改 A 文件的 X 函数
子 Agent 发现 B 文件也有类似问题
子 Agent 在报告中备注："发现 B 文件存在类似问题，建议主 Agent 评估是否需要修复"
```

### 案例 3：全量重写文件

```
# 错误示范
read_file("Config.cs")  # 读取 200 行
write_file("Config.cs", content="...整个文件...")  # 重写 200 行

# 正确做法
read_file("Config.cs", start_line=45, end_line=65)  # 读取目标区域
str_replace("Config.cs", old_str="旧代码", new_str="新代码")  # 精确替换
```

---

## 🎯 检查清单

执行前确认：
- [ ] 已理解任务目标和验收标准
- [ ] 已确认需要修改的文件列表
- [ ] 知道使用 str_replace 而非 write_file

执行中确认：
- [ ] 每次读取都指定了行号范围
- [ ] 每次修改都使用 str_replace
- [ ] 没有修改 Prompt 未提及的文件

执行后确认：
- [ ] 编译通过（0 错误）
- [ ] 迭代文档已更新
- [ ] 报告格式完整

---

**最后更新**：2026-11-11  
**版本**：1.0
