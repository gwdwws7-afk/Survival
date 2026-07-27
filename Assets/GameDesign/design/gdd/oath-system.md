# Oath — System GDD ⭐

> **Status**: 🔒 LOCKED v1.0
> **Author**: Mavis + 用户
> **Last Updated**: 2026-07-27
> **Implements Pillar**: Woven Power + Living Hearth + Wyrd + Waxing Moon（汇聚所有）

---

## A. Overview

**Oath 是 Ravensong 的"长线驱动 + 终局"——5 条平行誓言，5 个里程碑/条，25 个总目标，**汇聚到苍穹之誓的"奥丁审判"作为游戏终局**。每条誓言是 Ravensong 一个支柱的**长线化身**：锻冶之誓 = Woven Power / 炉火之誓 = Living Hearth / 荒野之誓 = Waxing Moon / 亡者之誓 = Wyrd / 苍穹之誓 = 终局汇聚点。

5 条誓言**可并行**（无需选一条放弃其他），但每条都需要 10+ 小时的深度投入。完成任意 4 条后解锁苍穹之誓，触发"奥丁审判"——你必须决定**回阿斯加德当棋子，还是留在中庭做自己的神**。

数据层由 `OathSO`（data-config.md C.2 类型 5）驱动；本 GDD 专注于**誓言设计、里程碑、苍穹之誓、奥丁审判、终局叙事**。

---

## B. Player Fantasy

### 主幻想
> "我完成了 4 条誓言，奥丁的审判降临——他问我：'你回来，还是留下？' 我知道，无论哪个选择，Ravensong 都将改变。"

### 关键体验时刻
- **第一次**打开誓言页：看到 5 条誓言 + 25 个里程碑，**清晰知道 1 周目要做什么**
- **第一次**完成里程碑：UI 弹"誓言进度 +1"+ 该誓言图标亮起
- **第一次**完成 1 条誓言（5 个里程碑）：永久聚落 buff + 视觉仪式动画
- **第 4 条誓言完成**：苍穹之誓解锁 + 全屏"奥丁的审判"事件
- **奥丁审判**：奥丁的独白 + 玩家 2 选 1 决策
- **离开** vs **留下**的结局动画（**2 种不同结局**）

---


### B.1 FADT 三维分析（叙事性审计）

> 按 FADT 框架（Intention / Perceivable Consequence / Story）显式审计 Oath System 的"承诺有重量"叙事。

#### Intention（意图清晰度）

| 玩家常见意图 | 系统支持度 | 断裂点 |
|---|---|---|
| 完成 5 誓言 25 里程碑 | 🟢 高 | 无（进度条 + Toast 清晰）|
| 解锁苍穹之誓 | 🟢 高 | 触发条件明确（4 誓言 5/5）|
| 完成苍穹祭坛仪式 | 🟡 中 | "准备" 状态不显式（建议加 §C.6 加准备度显示）|
| 奥丁审判 2 选 1 | 🟢 高 | 弹出 Modal 不可关闭 |
| 永久 buff 应用 | 🟢 高 | 触发时聚落 VFX 变化 |

#### Perceivable Consequence（结果可感知度）

| 玩家行为 | 系统反馈 | 归因清晰度 |
|---|---|---|
| 完成 1 个里程碑 | Toast + 进度条 +X | 🟢 清晰 |
| 完成 1 条誓言（5/5）| 金色光柱 5s + 誓言建筑出现 | 🟢 清晰 |
| 完成 4 条誓言 | 苍穹之誓解锁（自动）| 🟢 清晰 |
| 苍穹之誓 5/5 | 3 min 奥丁审判演出 | 🟢 清晰 |
| 死亡选择 | 24h 衰悼期 -20% | 🟢 清晰（视觉冷色 + VFX）|

#### Story（可叙述性）

| 玩家经历 | 故事元素 | 可叙述性 |
|---|---|---|
| 第 1 次完成 1 个誓言 | "锻冶圣坛在聚落燃起" | 🟢 高（金色光柱 + 5s 仪式）|
| 第 4 条誓言完成 | "苍穹祭坛召唤" | 🟢 高（自动触发 + 视觉）|
| 奥丁独白 | "炉火还在燃烧" | 🟢 高（3 min 演出 + 字幕）|
| 2 选 1 结局 | "你选择回阿斯加德/留在中庭" | 🟢 高（动画 + 不可逆）|
| 通关后回顾 | 5 誓言 × 25 里程碑全亮 | 🟢 高（誓言页永久记录）|

**FADT 审计结论**：Oath System 在 Intention / Perceivable Consequence / Story 三维**全部高支持**。P1 改进项：苍穹祭坛"准备" 状态显示（可在 v1.1 完善）。

---

### B.1 FADT 三维分析（叙事性审计）

> 按 FADT 框架（Intention / Perceivable Consequence / Story）显式审计 Oath System 的"承诺有重量"叙事。

#### Intention（意图清晰度）

| 玩家常见意图 | 系统支持度 | 断裂点 |
|---|---|---|
| 完成 5 誓言 25 里程碑 | 🟢 高 | 无（进度条 + Toast 清晰）|
| 解锁苍穹之誓 | 🟢 高 | 触发条件明确（4 誓言 5/5）|
| 完成苍穹祭坛仪式 | 🟡 中 | "准备" 状态不显式（建议加 §C.6 加准备度显示）|
| 奥丁审判 2 选 1 | 🟢 高 | 弹出 Modal 不可关闭 |
| 永久 buff 应用 | 🟢 高 | 触发时聚落 VFX 变化 |

#### Perceivable Consequence（结果可感知度）

| 玩家行为 | 系统反馈 | 归因清晰度 |
|---|---|---|
| 完成 1 个里程碑 | Toast + 进度条 +X | 🟢 清晰 |
| 完成 1 条誓言（5/5）| 金色光柱 5s + 誓言建筑出现 | 🟢 清晰 |
| 完成 4 条誓言 | 苍穹之誓解锁（自动）| 🟢 清晰 |
| 苍穹之誓 5/5 | 3 min 奥丁审判演出 | 🟢 清晰 |
| 死亡选择 | 24h 衰悼期 -20% | 🟢 清晰（视觉冷色 + VFX）|

#### Story（可叙述性）

| 玩家经历 | 故事元素 | 可叙述性 |
|---|---|---|
| 第 1 次完成 1 个誓言 | "锻冶圣坛在聚落燃起" | 🟢 高（金色光柱 + 5s 仪式）|
| 第 4 条誓言完成 | "苍穹祭坛召唤" | 🟢 高（自动触发 + 视觉）|
| 奥丁独白 | "炉火还在燃烧" | 🟢 高（3 min 演出 + 字幕）|
| 2 选 1 结局 | "你选择回阿斯加德/留在中庭" | 🟢 高（动画 + 不可逆）|
| 通关后回顾 | 5 誓言 × 25 里程碑全亮 | 🟢 高（誓言页永久记录）|

**FADT 审计结论**：Oath System 在 Intention / Perceivable Consequence / Story 三维**全部高支持**。P1 改进项：苍穹祭坛"准备" 状态显示（可在 v1.1 完善）。

---
## C. Detailed Design

### C.1 核心规则（Core Rules）

#### 规则 1：5 条誓言
- 锻冶之誓（Smithing）—— Woven Power 化身
- 炉火之誓（Hearth）—— Living Hearth 化身
- 荒野之誓（Wild）—— Waxing Moon 化身
- 亡者之誓（Death）—— Wyrd 化身
- 苍穹之誓（Sky）—— 终局汇聚（锁定 4 条誓言后才解锁）

#### 规则 2：每条誓言 5 个里程碑
- 5 条 × 5 = 25 个总里程碑
- 里程碑**可并行完成**（同时推进多条誓言）
- 单个里程碑**不可重置**（一旦完成，永久）
- 单条誓言完成 = 5 个里程碑都完成

#### 规则 3：里程碑任务类型（5 种）
- **Kill**（击杀）：击败 X 敌人 / Y boss
- **Craft**（编织）：编织 X 件物品 / Y 个 Tier 5 配方
- **Explore**（探索）：发现 X 个隐藏配方 / 访问 Y 个群系
- **Send**（送别）：送走 X 个英灵到英灵殿
- **Tame**（驯服）/ **其他**：特定行为

#### 规则 4：每条誓言有专属永久 buff
- 锻冶：编织效率 +20%
- 炉火：聚落产出 +25%
- 荒野：月相"洞察"永久激活
- 亡者：英灵升 4 级（v1.1，MVP 3 级）
- 苍穹：终局选择 → 触发结局

#### 规则 5：苍穹之誓锁定
- 必须**完成 4 条**非苍穹誓言后才解锁
- 完成苍穹之誓 = 游戏**结束**

#### 规则 6：奥丁审判是终局
- 苍穹之誓 5 个里程碑完成 → 触发奥丁审判
- 玩家选"回阿斯加德" → 结局 1
- 玩家选"留在中庭" → 结局 2
- 选择**不可逆**

#### 规则 7：誓言**不可重玩**
- 完成苍穹之誓 = 游戏结束
- v1.1 才有"新游戏+"模式（保留部分进度）

### C.2 The 5 Oaths（详细）

#### 誓言 1：锻冶之誓（Smithing Oath）—— Woven Power

| Milestone | 任务 | 奖励（每完成） |
|---|---|---|
| 1.1 | 编织 10 件物品 | 1 个新 Tier 2 配方 |
| 1.2 | 编织 5 件 Tier 3 物品 | 1 个新 Tier 3 配方 |
| 1.3 | 发现 3 个隐藏配方 | 1 个新隐藏配方提示 |
| 1.4 | 编织 1 件 Tier 4 物品 | 锻造工具 +1 tier |
| 1.5 | 编织 5 件 Tier 5 物品 | 永久 buff：编织效率 +20% |

**完成整条誓言**：聚落出现"锻冶圣坛"（永久建筑）+ 编织速度 +30%（叠加）

#### 誓言 2：炉火之誓（Hearth Oath）—— Living Hearth

| Milestone | 任务 | 奖励（每完成） |
|---|---|---|
| 2.1 | 招募 3 个英灵 | 长屋容量 +1 |
| 2.2 | 维持聚落 5 个英灵 7 天 | 聚落防御 +10% |
| 2.3 | 治愈 1 个重病英灵 | 治疗仪式 +1 次 |
| 2.4 | 送走 1 个英灵到英灵殿 | 永久 buff：英灵产出 +15% |
| 2.5 | 维持聚落 8 个英灵 3 天 | 永久 buff：聚落产出 +25% |

**完成整条誓言**：聚落出现"英灵殿"（永久建筑）+ 聚落士气永久满

#### 誓言 3：荒野之誓（Wild Oath）—— Waxing Moon

| Milestone | 任务 | 奖励（每完成） |
|---|---|---|
| 3.1 | 击杀 50 个敌人 | 月相"洞察"持续时间 × 1.5 |
| 3.2 | 采集 30 个 Night-only 资源 | 1 个新隐藏配方 |
| 3.3 | 访问 3 个不同群系 | 群系移动速度 +10% |
| 3.4 | 击杀 1 个 world boss | 月光武器 +1 tier |
| 3.5 | 经历 1 个完整月相循环 | 永久 buff：月相"洞察"永久激活 |

**完成整条誓言**：聚落出现"驯兽场"（永久建筑）+ 动物主动不攻击

#### 誓言 4：亡者之誓（Death Oath）—— Wyrd

| Milestone | 任务 | 奖励（每完成） |
|---|---|---|
| 4.1 | 送走 1 个英灵（任何） | 治疗仪式 +1 次 |
| 4.2 | 见证 1 个英灵死亡（不强留） | 永久 buff：英灵死亡时获得额外 god-ember |
| 4.3 | 维持聚落满 8 个英灵 5 天 | 1 个新英灵到来（剧情） |
| 4.4 | 经历 1 次 3-5 天腐化（不治疗） | 永久 buff：英灵腐化 +50% 时间 |
| 4.5 | 送走 3 个英灵到英灵殿 | 永久 buff：英灵阶位最高 4 级（v1.1） |

**完成整条誓言**：聚落出现"英灵殿祭坛"（永久建筑）+ 1 个新剧情英灵自动加入

#### 誓言 5：苍穹之誓（Sky Oath）—— 终局

> 🔒 **锁定决策**：必须完成 4 条非苍穹誓言后解锁

| Milestone | 任务 | 奖励（每完成） |
|---|---|---|
| 5.1 | 编织 1 件传说中的物品 | 装备 +1 tier（Tier 6 传说） |
| 5.2 | 击杀所有 4 个 world boss | Boss 战利品 +1 |
| 5.3 | 维持聚落满 8 个英灵 7 天 | 聚落永久 +25% 产出 |
| 5.4 | 编织 1 件 Tier 5 + 满月 "神显" | 装备"羽翼碎片" |
| 5.5 | 收集全部 4 个"永夜符文" | **触发奥丁审判** |

**完成整条誓言**：进入**奥丁审判** → 终局 2 选 1

### C.3 Milestone 进度

#### 状态机
```
[Not Started]
    ↓ 玩家开始该 milestone 任务
[In Progress]
    ↓ 任务完成
[Completed]  (永久)
```

#### 进度可见
- **Oath UI** 始终显示 5 条誓言 + 25 个里程碑
- 玩家随时可看
- **不可隐藏**（不像一些游戏的"完成度隐藏"）

#### 进度提示
- **In Progress** 状态时，UI 显示当前进度（"3/10"）
- **Completed** 状态时，UI 标记 ✓
- **每完成 1 个** 触发短暂动画 + 提示音

### C.4 Oath Progression（誓言推进）

#### 视觉进度
- 誓言页面有 5 个**大图标**（5 条誓言）
- 每个图标周围有 5 个**小点**（5 个里程碑）
- 完成的里程碑：实心 ✓
- 未完成的：空心
- 进行中的：脉动

#### 仪式动画
- 完成 1 个里程碑：UI 弹"誓言进度 +1"，该里程碑图标变亮
- 完成 1 条誓言（5/5 全部完成）：
  - 5 秒仪式动画（金色光柱 + 配音）
  - 该誓言图标旋转 360° + 永久发光
  - 永久 buff 应用

### C.5 Oath Completion Rewards

#### 单个里程碑奖励
- 道具（新配方 / 新工具 / 新资源）
- 解锁（建筑 / 群系 / 敌人）
- 临时 buff（**不**永久）

#### 整条誓言完成奖励（永久）
- 永久聚落 buff
- 永久聚落建筑
- 全局属性提升

#### 4 条誓言完成 → 苍穹解锁
- 聚落"炉火"变金色
- 5 个誓言图标全部变金色
- 苍穹之誓解锁（金色脉动）
- 触发"奥丁之眼"事件：5 秒全球画面 + 1 周目里程碑

### C.6 The Sky Oath（苍穹之誓）⭐ 终局

**这是 Ravensong 的"游戏结束"**。

#### 触发
- 完成 4 条非苍穹誓言（25 个里程碑的 20 个）
- 苍穹之誓**自动解锁**
- 触发"奥丁之眼"事件

#### 苍穹之誓里程碑（5 个）
- 5.1：编织 Tier 6 传说物品
- 5.2：击杀 4 个 world boss
- 5.3：维持 8 英灵 7 天
- 5.4：满月"神显"+ Tier 5
- 5.5：收集 4 个"永夜符文"（来自 4 个 boss）

#### 完成苍穹之誓
- **所有 25 个里程碑完成**
- 触发**奥丁审判**（不可逆）
- 游戏进入**结局阶段**

### C.7 The Odin Trial（奥丁审判）⭐ 终局

**这是 Ravensong 的"最终选择"**。

#### 审判前置
- 苍穹之誓完成
- 玩家回到聚落
- **奥丁的独白**（3 分钟演出）：
  - "我的女儿，你已超越了我给你的枷锁"
  - "回到阿斯加德，你会是我的将军，永恒"
  - "或者留下，在你建的世界里做自己的神"
  - "你的选择将决定 Ravensong 的命运"

#### 玩家选择
```
[奥丁的审判]
[对话继续...]

[按钮 1] 回到阿斯加德（成为奥丁的将军）
[按钮 2] 留在中庭（成为自己的神）

[警告] 不可逆
```

#### 结局 1：回阿斯加德
- **叙事**：你选择回到阿斯加德，成为奥丁的将军
- **视觉**：金色光柱 + 玩家飞向天空 + 阿斯加德闪亮
- **后效**：游戏结束，可"新游戏+"
- **v1.1 决策**：v1.1 新游戏+可继承部分 buff

#### 结局 2：留在中庭
- **叙事**：你选择留下，在中庭做自己的神
- **视觉**：玩家双脚扎根 + 聚落扩张 + 4 个群系向玩家跪拜
- **后效**：游戏结束，可"新游戏+"
- **特殊**：结局 2 后，"无尽模式"开启（继续游戏）

### C.8 Oath UI（誓言 UI）

#### 入口
- 玩家按 O 键 / 手柄右摇杆按键 → 打开誓言页
- 全屏半透明 + 居中

#### 布局
```
┌──────────────────────────────────────┐
│  Ravensong · 誓言                      │
├──────────────────────────────────────┤
│ [锻冶]    [炉火]    [荒野]    [亡者]   [苍穹]  │
│  ✓1.1     ✓2.1     ✓3.1     ✓4.1    🔒5.1  │
│  ✓1.2     ✓2.2     ✓3.2     ✓4.2    🔒5.2  │
│  ✓1.3     ⬤2.3     ✓3.3     ⬤4.3    🔒5.3  │
│  ⬤1.4     ⬤2.4     ⬤3.4     ⬤4.4    🔒5.4  │
│  ⬤1.5     ⬤2.5     ⬤3.5     ⬤4.5    🔒5.5  │
│  4/5      3/5      5/5      2/5     0/5    │
│  buff     buff     ✓DONE    buff    LOCKED │
└──────────────────────────────────────┘
```

#### 视觉
- ✓ = 完成（绿色）
- ⬤ = 进行中（脉动）
- ○ = 未开始（空心）
- 🔒 = 锁定（苍穹在 4 条誓言完成前）
- ✓ DONE = 整条誓言完成（金色发光）

#### 状态信息
- 每条誓言显示：**当前 / 5** 进度
- 整条誓言 buff 描述（tooltip）
- 苍穹之誓：显示解锁条件（"完成 4 条誓言"）

### C.9 与其他系统的交互

| 系统 | 怎么用 Oath |
|---|---|
| **Fate-Thread** | 里程碑：编织 X 物品 / Tier 5 配方 |
| **Einherjar** | 里程碑：送走 / 死亡 / 维持 |
| **Combat** | 里程碑：击杀 X 敌人 / 4 boss |
| **Gathering** | 里程碑：Night-only 资源 |
| **World Exploration** | 里程碑：访问 X 群系 |
| **Inventory** | 完成 buff 影响装备 |
| **Settlement** | 完成奖励建建筑 |
| **Save** | 所有里程碑进度持久化 |
| **UI/HUD** | Oath 页面 + 完成动画 |
| **Day-Night** | 荒野之誓 buff 影响月相 |

**Oath 是 Ravensong 的"长线汇聚点"**——所有系统都向它输出。

---

## D. Formulas

### D.1 里程碑完成判定
```csharp
bool IsMilestoneComplete(Milestone m, PlayerState player) {
  switch (m.type) {
    case MilestoneType.Kill:
      return player.totalKills >= m.targetValue;
    case MilestoneType.Craft:
      return player.totalCrafts >= m.targetValue;
    // ... etc
  }
}
```

### D.2 誓言完成判定
```csharp
bool IsOathComplete(OathSO oath, PlayerState player) {
  foreach (var milestone in oath.milestones) {
    if (!player.completedMilestones.Contains(milestone.id)) {
      return false;
    }
  }
  return true;
}
```

### D.3 苍穹之誓解锁
```csharp
bool IsSkyOathUnlocked(PlayerState player) {
  string[] requiredOaths = { "oath_smithing", "oath_hearth", "oath_wild", "oath_death" };
  foreach (var oathId in requiredOaths) {
    if (!player.completedOaths.Contains(oathId)) return false;
  }
  return true;
}
```

### D.4 永久 buff 应用
```csharp
void ApplyOathBuff(OathSO oath, PlayerState player) {
  player.permanentBuffs.Add(oath.completionBuff);
  player.settlementBuffs.Add(oath.settlementBuff);
  if (oath.completionBuilding != null) {
    player.settlement.AddBuilding(oath.completionBuilding);
  }
}
```

### D.5 誓言完成动画时长
```csharp
float oathCompleteAnimationDuration = 5.0f;  // 5 秒仪式
```

### D.6 奥丁审判时长
```csharp
float odinTrialDuration = 180.0f;  // 3 分钟演出
```

---

## E. Edge Cases

| 情况 | 处理 |
|---|---|
| 苍穹之誓未解锁时点击 | UI 灰显，提示"完成 4 条誓言" |
| 玩家死亡在奥丁审判中 | 不可（审判是演出，不可死） |
| 玩家在奥丁审判中退出 | autosave 触发，**不**保存审判状态（保留进入审判的进度） |
| 完成 1 条誓言后立即死亡 | **不**影响已应用 buff（死亡回聚落时 buff 在） |
| 4 条誓言全满后苍穹未触发 | 苍穹之誓 5 个里程碑出现，玩家可主动开始 |
| 玩家在审判中选后想反悔 | 不可逆（按钮一次性） |
| 审判结束后回到主菜单 | 加载最后存档（审判前），**不**进结局 |
| 完成苍穹 5 个里程碑之前游戏崩溃 | autosave 触发，进度保留 |
| 完成苍穹 5/5 后**不**选审判 | **强制**触发审判（5 秒倒计时） |
| 新游戏+模式 | v1.1：保留部分 buff + 5 誓言已锁 |
| 玩家想看 4 个结局变体 | 2 个结局变体（v1.1 扩展到 4 个） |
| 多人模式 | MVP 单机，不考虑 |
| 服务器端誓言进度 | MVP 单机，不考虑 |
| 玩家跨存档保留誓言进度 | v1.0 per-save，v1.1 meta |

---

## F. Dependencies

### 上游（Oath 依赖谁）

- **Data Config** —— `OathSO` + `GameConfigSO`
- **Fate-Thread** —— 编织里程碑
- **Einherjar** —— 英灵里程碑
- **Combat** —— 击杀 / boss 里程碑
- **Gathering** —— 资源里程碑
- **World Exploration** —— 群系里程碑
- **Save** —— 进度持久化

### 下游（谁依赖 Oath）

- **Save** —— 持久化所有进度
- **UI/HUD** —— Oath 页面 + 完成动画
- **Settlement** —— 永久建筑
- **VFX** —— 完成仪式

**Oath 是 Ravensong 的"长线汇聚点"**——所有系统向它输出，最终决定游戏结局。

---

## G. Tuning Knobs

> 调参字段建议加到 `GameConfigSO`（data-config v1.6 阶段）

| 参数 | 默认值 | 调参影响 |
|---|---|---|
| `oathMilestonesPerOath` | 5 | 决策 #2 锁定 |
| `oathCount` | 5 | 决策 #1 锁定 |
| `oathRequiredForSkyOath` | 4 | 锁定 |
| `oathCompleteAnimationSec` | 5 | 完成动画时长 |
| `oathMilestoneProgressBar` | true | UI 是否显示进度条 |
| `oathAutoApplyBuff` | true | 完成时是否自动应用 buff |
| `odinTrialDurationSec` | 180 | 奥丁独白时长 |
| `odinTrialMinSecondsBeforeChoice` | 10 | 玩家必须听 10 秒后才能选 |
| `endgameResetAfterChoice` | false | 选完后**不**重置游戏 |
| `newGamePlusEnabled` | false | v1.0 关闭，v1.1 开启 |
| `endgameCreditsRollSec` | 90 | 演职员表 90 秒 |
| `skyOathUnlockVisualDuration` | 8 | 苍穹解锁 8 秒仪式 |

---

## H. Acceptance Criteria

### AC-1: 誓言页 UI
**测试**：
1. 玩家按 O 键
2. **期望**：Oath 页打开
3. 显示 5 条誓言 + 25 个里程碑
4. **期望**：苍穹之誓显示 🔒（未解锁）

### AC-2: 里程碑进度
**测试**：
1. 玩家编织 10 件物品
2. **期望**：誓言 1.1 标记为 ✓
3. 提示动画 + 音

### AC-3: 单条誓言完成
**测试**：
1. 玩家完成 1.1-1.5 全部
2. **期望**：5 秒仪式动画
3. 永久 buff 应用（编织效率 +20%）
4. 锻冶圣坛出现在聚落

### AC-4: 4 条誓言完成 → 苍穹解锁
**测试**：
1. 玩家完成 1/2/3/4 全部
2. **期望**：苍穹之誓解锁（金色脉动）
3. "奥丁之眼"事件触发（5 秒全球画面）

### AC-5: 苍穹之誓里程碑
**测试**：
1. 苍穹解锁后
2. 玩家完成 5.1-5.5
3. **期望**：25/25 全部完成
4. 触发"奥丁审判"

### AC-6: 奥丁审判演出
**测试**：
1. 苍穹完成
2. 玩家回聚落
3. 奥丁独白 3 分钟（不可跳过 < 10 秒）
4. 玩家选"回阿斯加德"或"留在中庭"
5. **期望**：触发对应结局动画

### AC-7: 结局 1 - 回阿斯加德
**测试**：
1. 选"回阿斯加德"
2. **期望**：金色光柱 + 玩家飞向天空 + 阿斯加德闪亮
3. 演职员表 90 秒
4. 回主菜单

### AC-8: 结局 2 - 留在中庭
**测试**：
1. 选"留在中庭"
2. **期望**：玩家双脚扎根 + 聚落扩张 + 4 个群系向玩家跪拜
3. 演职员表 90 秒
4. "无尽模式"开启（玩家可继续游戏）

### AC-9: 不可逆
**测试**：
1. 选完结局后
2. 玩家死亡 / 退出 / Load
3. **期望**：结局**不**可重选（已锁定）

### AC-10: 誓言进度持久化
**测试**：
1. 玩家完成 10 个里程碑
2. Save Game
3. 退出
4. Load Game
5. **期望**：10 个里程碑 ✓ 状态保留

### AC-11: 性能
**测试**：
1. 誓言页打开 / 关闭
2. **期望**：< 16ms
3. 完成动画：< 5ms / 帧

### AC-12: 里程碑提示
**测试**：
1. 玩家完成 1 个里程碑
2. **期望**：UI 弹"誓言进度 +1" 3 秒
3. 誓言页对应位置脉动
4. 不阻塞游戏

---

## 10. 已锁定决策（Locked Decisions）

> 2026-07-27 用户拍板，12 个开放问题全部锁定。已落地为 G 旋钮 + data-config v1.6。

| # | 决策点 | 锁定值 | 落地位置 |
|---|---|---|---|
| 1 | **5 誓言一开始可见** | **是**（透明） | §C.8 UI |
| 2 | **里程碑可重置** | **否**（承诺有意义） | §C.1 规则 3 |
| 3 | **整条 buff 永久** | **是**（永久应用） | §C.5 |
| 4 | **新游戏+ v1.0** | **否**（v1.1 才有） | §C.7 |
| 5 | **奥丁独白时长** | **3 分钟**（180 秒） | §C.7 |
| 6 | **独白可跳过** | **前 10 秒不可跳**，之后可 | §C.7 |
| 7 | **苍穹自动解锁** | **是**（4 条完成自动） | §C.6 |
| 8 | **结局 2 模式** | **继续**（当前存档，无尽模式） | §C.7 |
| 9 | **结局 1 玩家** | **回主菜单** | §C.7 |
| 10 | **完成动画时长** | **5 秒**（仪式感） | §C.4 |
| 11 | **25 里程碑数量** | **合适**（每条 5 个可管理） | §C.3 |
| 12 | **UI 入场动画** | **淡入 0.5 秒**（不花哨） | §C.8 |

### 决策之间的协同

- **#1 + #2 + #3**：5 誓言可见 + 不可重置 + 永久 buff = **承诺有重量**——玩家知道要做什么，做了不能反悔
- **#4 + #7 + #8**：v1.0 单结局 + 自动解锁 + 继续 = **MVP 完整但不冗余**——1 周目能完整结束
- **#5 + #6 + #9**：3 分钟独白 + 前 10 秒不可跳 + 结局 1 回主菜单 = **尊重终局**——玩家有时间感受，但不会被困
- **#10 + #11 + #12**：5 秒仪式 + 25 个里程碑 + 0.5 秒淡入 = **仪式感但不过度**——每个里程碑的完成都"小而重"

### 仍待 playtest 调参（不阻塞 GDD 锁定）

| 待调项 | 候选范围 | 调参位置 |
|---|---|---|
| 25 里程碑分布 | 5/5/5/5/5 vs 4/5/6/5/5 | `OathSO` 设计 |
| 奥丁独白时长 | 180s vs 120s vs 240s | `GameConfigSO.odinTrialDurationSec` |
| 完成动画时长 | 5s vs 3s vs 8s | `GameConfigSO.oathCompleteAnimationSec` |
| 苍穹解锁仪式 | 8s vs 5s vs 12s | `GameConfigSO.skyOathUnlockVisualDuration` |

→ 这些都是 Prototype 阶段的**数值调参工作**，通过 `GameConfigSO` 直接改即可，不阻塞任何 GDD。

---

## Unity Implementation Notes

### 核心脚本（`Assets/Scripts/Oath/`）
- `OathManager.cs` —— 5 条誓言管理
- `MilestoneTracker.cs` —— 25 个里程碑追踪
- `OathUI.cs` —— 誓言页面
- `OathCompleteAnimator.cs` —— 完成动画
- `SkyOathTrigger.cs` —— 苍穹解锁
- `OdinTrial.cs` —— 奥丁审判演出
- `EndingCinematic.cs` —— 结局 1 / 2 演出
- `NewGamePlusManager.cs` —— v1.1 新游戏+

### 数据结构
```csharp
public class OathProgress {
  public Dictionary<string, bool> completedMilestones = new();
  public Dictionary<string, bool> completedOaths = new();
  public bool skyOathUnlocked;
  public bool skyOathCompleted;
  public EndingChoice endingChoice;  // None / Asgard / Midgard
  public StatBlock permanentBuffs;
}
```

### 状态机
```csharp
public enum GameState {
  Playing,
  OdinTrial,
  Ending1_Asgard,
  Ending2_Midgard,
  EndlessMode,  // 结局 2 后
}
```

### 事件订阅
```csharp
public class OathManager : MonoBehaviour {
  public static event Action<Milestone> OnMilestoneCompleted;
  public static event Action<OathSO> OnOathCompleted;
  public static event Action OnSkyOathUnlocked;
  public static event Action OnOdinTrialStarted;
  public static event Action<EndingChoice> OnEndingChosen;
}
```

### 性能预算
- 誓言页 UI：< 16ms
- 完成动画：< 5ms / 帧
- 奥丁演出：< 8ms / 帧（高清视频）
- 结局演出：< 8ms / 帧

### 演出制作
- **奥丁独白**：3 分钟音频 + 文字 + 全屏背景
- **结局 1**：金色光柱 + 飞升动画
- **结局 2**：扎根动画 + 聚落扩张
- **演职员表**：90 秒滚动

---

## 进度

| 章节 | 状态 |
|---|---|
| A. Overview | ✅ |
| B. Player Fantasy | ✅ |
| C. Detailed Design (9 小节) | ✅ |
| D. Formulas (6 个) | ✅ |
| E. Edge Cases (14 种) | ✅ |
| F. Dependencies | ✅ |
| G. Tuning Knobs (12 字段) | ✅ |
| H. Acceptance Criteria (12 条) | ✅ |
| **10. Locked Decisions (12 决策)** | ✅ |
| Unity Implementation Notes | ✅ |

**总进度**: 10/10 (100%) 🔒

---

## 变更日志

| 日期 | 版本 | 改动 | 责任人 |
|---|---|---|---|
| 2026-07-27 | v1.0 draft | 初版生成：10 段 + 公式 + Unity 实施 | Mavis |
| 2026-07-27 | v1.0 LOCKED | 12 开放问题用户拍板全部锁定；data-config v1.6 同步升级 | Mavis + 用户 |
| 2026-07-27 | FADT 三表补充 | 按 FADT 框架补充 Intention / Perceivable Consequence / Story 三维分析（提升承诺有重量叙事的显式度） | Mavis + 用户 |
| 2026-07-27 | FADT 三表补充 | 按 FADT 框架补充 Intention / Perceivable Consequence / Story 三维分析（提升承诺有重量叙事的显式度） | Mavis + 用户 |
