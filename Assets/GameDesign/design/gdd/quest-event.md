# Quest & Event — System GDD

> **Status**: 🔒 LOCKED v1.0
> **Author**: Mavis + 用户
> **Last Updated**: 2026-07-27
> **Implements Pillar**: **Foundation**（Oath 软主线 + 世界叙事触发器）
> **See Also**: `data-config.md` §C.2 类型 7（WorldEventSO）/ `oath-system.md` §C.2（5 誓言里程碑触发）/ `world-exploration.md` §C.7（远征记录）/ `ui-hud.md` §C.4（任务日志）

---

## A. Overview

**Quest & Event 是 Ravensong 的"叙事触发器"——所有"Oath 软主线 + 世界事件 + 远征任务 + 日常事件"都通过这个系统统一管理。** 玩家在 Ravensong 的"任务体验"不是"接任务→打怪→交任务"（避免成为 1 个 MMO 任务系统），而是 **"叙事时刻被触发"**——任务作为"叙事钩子"把游戏世界的事件**主动**呈现给玩家。

Ravensong 的任务设计哲学是 **"叙事性任务"（Narrative Quest）**：任务**不**是"数值目标"（不要"杀 10 只狼，奖励 50 经验"），而是"叙事节点"（"森林里的狼越来越多了——去调查原因"）。**任务的"奖励"是叙事推进 + 永久资源/buff，不是经验值/金币**。

4 类任务（v1.0 锁定）：
- **主线（Main Quest）**——Oath 里程碑的"叙事钩子"（约 10 个 v1.0）
- **支线（Side Quest）**——剧情英灵 / 隐藏 POI / 特殊事件（约 15 个 v1.0）
- **远征（Expedition Quest）**——远征基地的远征任务（约 18 个，已在 world-exploration.md 定义）
- **日常（Daily Quest）**——周期性触发的小事件（约 10 个 v1.0）

**世界事件（WorldEventSO）**是"非任务触发"——天气异常 / 奥丁诏令 / 神秘商人 / 瘟疫。**v1.0 简化**：世界事件 = 任务的"氛围层"，不单独 UI。

数据层由**新增**的 `QuestSO`（data-config.md C.2 类型 14）驱动；本 GDD 专注于**任务架构、4 类任务、触发机制、奖励系统、与世界事件协同**。

---

## B. Player Fantasy

### 主幻想
> "我接到 Eirik 的请求：'森林里的狼越来越多了，去调查原因'——我去了，发现是一头巨狼首领在指挥。这不是任务，是'活着的世界'。"

### 关键体验时刻

- **第一次**接任务：聚落吟游诗人唱"森林里的狼" + 任务日志自动出现"调查狼群"
- **第一次**完成主线任务：Oath 进度条 +1 + 永久 buff 出现
- **第一次**支线任务：剧情英灵 Eirik 来聚落请求帮助
- **第一次**远征任务：远征基地 + 路线 + 风险选择
- **第一次**日常任务：聚落篝火旁出现"神秘商人" 24h 后消失
- **第一次**世界事件：天空变红 + 奥丁之诏令 + 全局 modifier 24h

---

## C. Detailed Design

### C.1 核心规则（Core Rules）

#### 规则 1：4 类任务 + 1 类世界事件
- 主线：~10 个（Oath 软主线触发）
- 支线：~15 个（剧情英灵 / 隐藏 POI / 特殊事件）
- 远征：~18 个（world-exploration.md 已定义）
- 日常：~10 个（周期性）
- 世界事件：~20 个（v1.0 简化：与任务协同）

#### 规则 2：任务自动触发，玩家不主动"接"
- 主线：在 Oath 里程碑条件满足时**自动**出现
- 支线：剧情英灵招募时**自动**分配
- 远征：远征基地**始终**显示（玩家选哪个走哪个）
- 日常：每 24 真实小时**自动**刷新 1 个

#### 规则 3：任务不可放弃
- v1.0 决策：**不可**放弃（"承诺有重量"）
- 任务失败 = 玩家**死亡**或**超时**（仅日常任务）
- 失败后任务从日志消失 + 不重新触发

#### 规则 4：任务奖励分级
| 任务类型 | 奖励类型 | 奖励量 |
|---|---|---|
| **主线** | Oath 进度 + 永久 buff | 大（Oath 里程碑推进）|
| **支线** | 资源 + 隐藏配方 + 聚落事件 | 中（剧情推进）|
| **远征** | 资源 + 英灵 + 隐藏配方 | 中-大（远征奖励）|
| **日常** | god-ember + 小资源 | 小（节奏）|

#### 规则 5：世界事件 = 任务的"氛围层"
- 世界事件**不**是任务，但**可触发**任务
- 例："狼群异常" 世界事件 → 触发"调查狼群" 主线任务
- 世界事件持续 24h，期间有持续效果

#### 规则 6：任务进度自动追踪
- 玩家**不**需要手动报"完成"
- 系统监听 `OnXxxCompleted` 事件（如 `OnEnemyKilled` / `OnItemCrafted` / `OnBiomeVisited`）
- 任务条件满足时**自动**完成 + Toast

#### 规则 7：任务日志 UI（参考 ui-hud.md §C.4）
- 主线 / 支线 / 远征 / 日常 4 个标签页
- 任务卡片：名字 + 描述 + 目标 + 进度 + 奖励
- 点击任务 = 展开详情

#### 规则 8：任务持久化
- 所有任务进度写入 save
- 任务完成后 = 永久消失
- 世界事件 = 24h 倒计时

---

### C.2 4 类任务详细

#### 类型 1：主线任务（Main Quest）⭐ 10 个

**主线任务列表（v1.0）**：

| # | 任务名 | 触发 | 目标 | 奖励 | 关联 Oath |
|---|---|---|---|---|---|
| 1 | 炉火初燃 | 游戏开始 | 招募 1 个英灵 | 长屋 +1 槽 | 誓言 2.1 |
| 2 | 森林里的狼 | 狼群事件 | 击杀 5 狼 + 1 巨狼 | 1 god-ember + 桦树皮 | 誓言 3.1 |
| 3 | 第一束丝线 | 编织教学 | 编织 1 件 Tier 2 物品 | 1 新 Tier 2 配方 | 誓言 1.1 |
| 4 | 深夜的祭坛 | 奥丁之眼扫描 | 在神龛前祈祷 3 次 | 永久 buff：聚落效率 +5% | 誓言 5.1 |
| 5 | 永冻崖的呼唤 | 4 个群系已访问 | 到达永冻崖 + 击杀 1 寒狼 | 寒铁 ×5 + Tier 4 配方 | 誓言 1.4 |
| 6 | 亡者的低语 | 首次英灵 dying | 送 1 个英灵到英灵殿 | 永久 buff：英灵产出 +10% | 誓言 2.4 + 4.1 |
| 7 | 骨王的觉醒 | 白骨原 boss 房 | 击杀骨王 | 1 Tier 4 装备 + 50 god-ember | 誓言 3.4 |
| 8 | 4 神龛的呼召 | 神龛 Level 2 | 4 次衰悼期加速 | Wyrd 锚点提示 | 誓言 4.5 |
| 9 | 奥丁的审判前夜 | 4 誓言完成 | 完成苍穹之誓 5/5 | 苍穹祭坛出现 | 誓言 5.5 |
| 10 | 终局 | 苍穹祭坛 | 进入深渊之心 | 奥丁审判触发 | 誓言 5.5 |

**主线任务特点**：
- 顺序触发（前 1 完成 → 后 1 出现）
- 不可跳过
- 完成 = 永久记录
- 与 Oath 1:1 绑定

#### 类型 2：支线任务（Side Quest）⭐ 15 个

**支线任务列表（v1.0 15 个）**：

| # | 任务名 | 触发英灵 | 目标 | 奖励 |
|---|---|---|---|---|
| 1 | Eirik 的酒 | Eirik 招募 | 收集 5 蜂蜜酒 | Eirik 关系 +20% + 1 资源 |
| 2 | 失踪的祭司 | 吟游诗人 | 找 3 个古代祭坛 | 1 隐藏配方提示 + 50 god-ember |
| 3 | 猎人的故事 | 猎人 | 击杀 1 巨熊 | Tier 3 装备 + 1 资源 |
| 4 | 铁匠的请求 | 铁匠 | 编织 10 件铁制品 | 锻造工具 +1 tier + 50 god-ember |
| 5 | 农夫的担忧 | 农夫 | 治愈 1 病田 | 食物 ×20 + 1 隐藏资源 |
| 6 | 吟游诗人的歌 | 吟游诗人 | 找 5 个 Norse 符文 | Tier 3 配方 + 50 god-ember |
| 7 | 巨狼的复仇 | Eirik | 击杀 1 巨狼首领 | 1 Tier 4 装备 + 100 god-ember |
| 8 | 永冻崖的失踪者 | 剧情英灵 | 找 1 个失踪远征兵 | 1 隐藏配方 + 50 god-ember |
| 9 | 奥丁的礼物 | 奥丁之眼 | 在 4 神龛前各祈祷 1 次 | 永久 buff：god-ember 收集 +20% |
| 10 | 亡者的安息 | 送走 1 英灵后 | 在纪念碑前放 1 朵花 | 士气 +5% + 50 god-ember |
| 11 | 月光下的织机 | 满月 | 在满月编织 1 件 Tier 4 | 1 Tier 5 配方线索 |
| 12 | 奥丁的试炼 | 4 誓言完成 | 在深渊之心前看终局预告 | 奥丁头像 + 50 god-ember |
| 13 | 6 群系巡礼 | 6 群系访问 | 6 群系各放 1 个路标 | 探索成就 + 50 god-ember |
| 14 | 北欧的告别 | 8 英灵全部招募 | 8 英灵各对话 1 次 | 永久 buff：英灵关系 +10% |
| 15 | Ravensong 完成 | 全部探索 | 完成所有主线 + 支线 | "Ravensong" 成就 |

**支线任务特点**：
- 触发 = 剧情英灵招募 / 群系事件 / 满月特殊
- 可乱序
- 完成 = 永久消失
- 与特定英灵/事件绑定

#### 类型 3：远征任务（Expedition Quest）—— world-exploration 已定义

**详见 `world-exploration.md` §C.4**：
- 约 18 个远征任务（6 基地 × 3 难度）
- 玩家亲自远行
- 完成后从基地消失

#### 类型 4：日常任务（Daily Quest）⭐ 10 个

**日常任务列表（v1.0 10 个，24h 周期）**：

| # | 任务名 | 触发 | 目标 | 奖励 |
|---|---|---|---|---|
| 1 | 神秘商人 | 篝火旁 | 与商人交易 1 次 | 1 折扣物品 + 10 god-ember |
| 2 | 狼群威胁 | 任意群系 | 击杀 3 狼 | 10 god-ember + 1 皮革 |
| 3 | 木材收集 | 白桦林 | 砍 10 桦树 | 20 木 + 5 god-ember |
| 4 | 编织练习 | 工坊 Level 1 | 编织 3 件物品 | 15 god-ember + 1 god-ember |
| 5 | 聚落清理 | 聚落 | 击杀 5 入侵生物 | 30 god-ember + 1 资源 |
| 6 | 渡鸦的礼物 | 奥丁之眼 | 在神龛前 1 次 | 5 god-ember + 1 食物 |
| 7 | 夜巡 | 永冻崖 | 夜晚击杀 1 巨狼 | 20 god-ember + 1 Tier 3 装备 |
| 8 | 远方访客 | 任意远征基地 | 完成 1 个短途远征 | 30 god-ember + 1 资源 |
| 9 | 聚落扩展 | 任意建筑 | 升级 1 个建筑 | 20 god-ember + 1 资源 |
| 10 | 月光散步 | 满月 | 在月光下 1 个游戏日 | 10 god-ember + 1 god-ember |

**日常任务特点**：
- 24h 真实时间刷新
- 完成 = 自动消失
- 不完成 = 24h 后**强制**消失（不算失败）
- 奖励**小**（god-ember 为主）

---

### C.3 世界事件（World Event）⭐

> `WorldEventSO` 已定义（data-config.md §C.2 类型 7）。本 GDD 简述协同方式。

#### 世界事件类型（v1.0 20 个）

| 类别 | 数量 | 例子 | 效果 |
|---|---|---|---|
| **奥丁诏令** | 5 | "奥丁宣告：今夜狼群凶猛" | 狼群攻击 +50% 持续 24h |
| **神秘商人** | 3 | "北方商人带来稀有配方" | 商人聚落出现 24h |
| **天气异常** | 5 | "永冻崖暴风雪" | 寒冷 debuff ×2 持续 24h |
| **剧情触发** | 4 | "Eirik 找到失散的兄弟" | 支线任务出现 |
| **聚落事件** | 3 | "聚落收到匿名礼物" | 资源 +10 |

#### 世界事件 vs 任务

| 维度 | 世界事件 | 任务 |
|---|---|---|
| **持续时间** | 24h（临时）| 永久（直到完成）|
| **玩家参与** | 被动（自动生效）| 主动（需要做）|
| **奖励** | 全局 modifier | 具体奖励 |
| **触发** | 时间 / 概率 | 任务条件 |

#### 世界事件触发机制
- 每 12 真实小时**最多**触发 1 个世界事件
- 同时存在**最多**3 个世界事件
- 触发概率 = `WorldEventSO.weight / sum(all weights)`
- v1.0 决策：1 周目**至少**触发 5 个世界事件

#### 世界事件 vs 主线联动
- "狼群异常" 世界事件 → 触发"森林里的狼" 主线任务
- "奥丁诏令" 世界事件 → 触发"奥丁的审判前夜" 主线任务

---

### C.4 任务触发与条件

#### 触发机制（4 类）

| 触发类型 | 说明 | 示例 |
|---|---|---|
| **自动** | 条件满足时**自动**出现 | 主线 1（招募 1 英灵）|
| **剧情** | 剧情英灵招募时**自动**分配 | 支线 1（Eirik 招募）|
| **远征** | 远征基地**始终**显示 | 远征（已定义）|
| **周期** | 24h 真实时间刷新 | 日常任务 |

#### 条件类型（6 种）

| 条件 | 字段 | 示例 |
|---|---|---|
| **击杀** | `KillCondition` | 击杀 5 狼 |
| **采集** | `GatherCondition` | 砍 10 桦树 |
| **编织** | `CraftCondition` | 编织 3 件物品 |
| **探索** | `ExploreCondition` | 访问 6 群系 |
| **送别** | `SendOffCondition` | 送 1 英灵到英灵殿 |
| **招募** | `RecruitCondition` | 招募 1 英灵 |

#### 条件求值
```csharp
bool CheckQuestCondition(QuestCondition cond) {
  return cond switch {
    KillCondition k => player.killCount[k.enemyId] >= k.required,
    GatherCondition g => player.gatherCount[g.resourceId] >= g.required,
    CraftCondition c => player.craftCount[c.recipeId] >= c.required,
    ExploreCondition e => player.visitedBiomes.Contains(e.biomeId),
    SendOffCondition s => player.sendOffCount[s.sendoffType] >= s.required,
    RecruitCondition r => player.einherjars.Count >= r.required,
    _ => false
  };
}
```

---

### C.5 奖励系统

#### 奖励类型

| 类型 | 描述 | 示例 |
|---|---|---|
| **Oath 进度** | OathSO.milestones[i].completed = true | 主线 1（誓言 2.1）|
| **永久 buff** | 给聚落 / 玩家永久 modifier | 主线 4（聚落效率 +5%）|
| **资源** | 资源池 +X | 支线 1（蜂蜜酒 ×5）|
| **物品** | 物品入背包 | 远征 1（Tier 3 装备）|
| **god-ember** | 资源池 +X | 日常 1（10 god-ember）|
| **隐藏配方** | 配方入发现列表 | 支线 11（Tier 5 配方线索）|
| **建筑/聚落事件** | 触发聚落事件 | 支线 10（士气 +5%）|

#### 奖励发放时机
- 任务**完成**时立即发放
- 多个任务同时完成 → 奖励**各自**独立发放
- 任务奖励**不**叠加（多次完成 = 多次奖励）

---

### C.6 任务日志 UI（参考 ui-hud.md §C.4）

#### 任务日志结构
```
[任务日志]（按 J 打开）
├── 主线（5-10 个 active）
├── 支线（3-5 个 active）
├── 远征（6 个 base 列表）
└── 日常（1-2 个 active，24h 周期）
```

#### 任务卡片样式
```
[任务名]                        [完成度 3/5]
[任务描述]
[目标 1: 已完成 ✓]
[目标 2: 进行中 3/5]
[目标 3: 待开始]
[奖励: Oath 进度 + 永久 buff]
```

#### 任务详情面板
- 任务名（衬线体 24px）
- 描述（衬线体 16px）
- 6 条件状态（✓ / 进行中 / 待开始）
- 奖励预览

#### 任务完成 Toast
- 屏幕中央
- 4s 显示
- 金色边框
- 任务名 + 奖励摘要

---

### C.7 任务失败与重置

#### 任务失败条件

| 任务类型 | 失败条件 | 后果 |
|---|---|---|
| **主线** | 永不失败（除非玩家死）| n/a |
| **支线** | 永不失败 | n/a |
| **远征** | 玩家死亡 | 远征失败 + 记录 |
| **日常** | 24h 真实时间过去 | 任务消失 + 不重新触发 |

#### 玩家死亡时
- 主线任务**继续**（不死于"主线任务"）
- 远征任务**失败**（玩家在远征中死亡）
- 日常任务**继续**（玩家死后返回聚落）

#### 任务重置
- v1.0：**不**做任务重置
- v1.1 决策：v1.1 NG+ 时全部任务重置

---

### C.8 与其他系统的交互

| 系统 | 怎么用 Quest-Event |
|---|---|
| **Oath** | 10 个主线任务 = 5 誓言里程碑的"叙事钩子"|
| **World-Exploration** | 18 远征任务 + 70 POI 触发支线 |
| **Death-Send-off** | 送别事件触发支线 10（亡者的安息）|
| **Einherjar** | 招募触发支线 1-6（剧情英灵）|
| **Day-Night** | 满月触发支线 11（月光织机）|
| **Fate-Thread** | 编织触发主线 3 + 日常 4 |
| **UI/HUD** | 任务日志（按 J）+ 完成 Toast |
| **Save** | 任务进度 + 完成状态 + 失败状态 |

---

## D. Formulas

### D.1 任务条件判定
```csharp
bool IsQuestComplete(QuestSO quest) {
  return quest.conditions.All(c => CheckCondition(c));
}
```

### D.2 日常任务刷新
```csharp
void TickDailyQuests() {
  if (TimeManager.realHoursPassed - lastDailyRefresh >= 24f) {
    lastDailyRefresh = TimeManager.realHoursPassed;
    SpawnNewDailyQuest();
  }
}
```

### D.3 世界事件触发
```csharp
bool ShouldTriggerWorldEvent(WorldEventSO[] allEvents) {
  if (activeWorldEvents.Count >= 3) return false;
  if (TimeManager.realHoursPassed - lastEventTrigger < 12f) return false;
  
  float totalWeight = allEvents.Sum(e => e.weight);
  float roll = Random.value * totalWeight;
  float cumulative = 0;
  foreach (var e in allEvents) {
    cumulative += e.weight;
    if (roll <= cumulative) return true;
  }
  return false;
}
```

### D.4 主线触发
```csharp
bool ShouldTriggerMainQuest(int mainQuestIndex) {
  if (mainQuestIndex == 0) return true;  // 第一个自动
  var prevQuest = allMainQuests[mainQuestIndex - 1];
  return prevQuest.state == QuestState.Completed;
}
```

### D.5 支线触发
```csharp
bool ShouldTriggerSideQuest(SideQuestSO quest) {
  return quest.triggerCondition switch {
    RecruitTrigger t => player.einherjars.Any(e => e.einherjarId == t.einherjarId),
    BiomeEventTrigger t => player.visitedBiomes.Contains(t.biomeId),
    EventTrigger t => activeWorldEvents.Any(e => e.id == t.eventId),
    _ => false
  };
}
```

### D.6 奖励发放
```csharp
void GrantQuestReward(QuestSO quest) {
  foreach (var reward in quest.rewards) {
    reward switch {
      OathMilestoneReward o => OathManager.CompleteMilestone(o.milestoneIndex),
      StatBlockReward s => player.permanentBuffs += s.buff,
      ResourceReward r => settlement.resources += r.amount,
      ItemReward i => inventory.Add(i.item),
      GodEmberReward g => player.godEmber += g.amount,
      HiddenRecipeReward h => player.discoverRecipe(h.recipeId),
      _ => Debug.LogWarning("Unknown reward type"),
    };
  }
}
```

---

## E. Edge Cases

| 情况 | 处理 |
|---|---|
| 任务条件已满足但未触发 | 触发后**立即**显示完成 |
| 玩家死亡时任务进度 | 远征失败 + 其他**继续** |
| 多个任务同时完成 | 各自独立完成 + 各自 Toast |
| 玩家同时接 2 个主线 | **不**可能（主线顺序触发）|
| 任务奖励已满（如资源上限）| 资源 v1.0 **无上限**（v1.1 决策）|
| 远征任务失败但远征未开始 | **不**可能 |
| 日常任务超时 | 24h 后强制消失 + 不重发 |
| 玩家退出 + 重新进入 | 任务进度**保留** |
| 玩家 v1.0 死 | 远征失败 + 死亡演出 |
| 玩家在远征中触发主线 | 主线**继续**（远征独立）|
| 任务奖励触发建筑升级 | 升级 VFX 24h 触发（参考 settlement.md）|
| 玩家 v1.0 死 + 复活 | 任务进度**保留** |
| 任务链（主线 → 支线 → 主线）| 链**顺序**触发 |
| 任务奖励触发 Oath 完成 | Oath 完成时**独立**触发誓言建筑 |

---

## F. Dependencies

### 上游（这个系统依赖谁）
- **WorldEventSO**（已有类型 7）—— 世界事件定义
- **Oath**（已有）—— 主线任务关联誓言
- **World-Exploration** —— 远征任务
- **Day-Night** —— 满月触发 + 时间追踪
- **Data Config** —— `QuestSO` 是新类型 14

### 下游（谁依赖这个系统）
- **UI/HUD** —— 任务日志 + 完成 Toast
- **Save** —— 任务进度 + 完成状态
- **Oath** —— 主线任务完成 = Oath 进度
- **Settlement** —— 任务奖励触发聚落事件

---

## G. Tuning Knobs（12 字段）

| 旋钮 | 默认值 | 范围 | 决策编号 | 影响 |
|---|---|---|---|---|
| `mainQuestCount` | 10 | 5-20 | #1 | v1.0 主线任务数 |
| `sideQuestCount` | 15 | 10-30 | #2 | v1.0 支线任务数 |
| `dailyQuestCount` | 10 | 5-15 | #3 | v1.0 日常任务数（24h 周期）|
| `dailyQuestRefreshHours` | 24f | 12-48 | #3 | 日常任务刷新周期 |
| `worldEventMaxActive` | 3 | 1-5 | #4 | 同时存在的世界事件数 |
| `worldEventMinIntervalHours` | 12f | 6-24 | #4 | 世界事件触发最小间隔 |
| `worldEventMinTriggerPerPlaythrough` | 5 | 3-10 | #5 | 1 周目至少触发的世界事件数 |
| `questAbandonAllowed` | false | bool | #6 | 任务可放弃（v1.0 锁定 = false）|
| `mainQuestSequential` | true | bool | #7 | 主线任务顺序触发 |
| `questLogMaxActive` | 30 | 10-50 | #8 | 任务日志最大活动数 |
| `questCompleteToastSec` | 4f | 2-8 | #9 | 完成 Toast 显示时长 |
| `questRewardMultiplier` | 1.0f | 0.5-2 | #10 | 任务奖励乘数（playtest 调）|

---

## H. Acceptance Criteria

### AC-1: 主线任务自动触发
- **条件**：主线 1 完成
- **结果**：主线 2 自动出现 + 任务日志更新

### AC-2: 支线任务剧情触发
- **条件**：Eirik 招募
- **结果**：支线 1（Eirik 的酒）自动出现

### AC-3: 远征任务列表
- **条件**：玩家在任意远征基地
- **结果**：3-5 个远征任务**始终**显示

### AC-4: 日常任务 24h 刷新
- **条件**：24 真实小时过去
- **结果**：1 个新日常任务出现 + 旧任务消失

### AC-5: 任务进度追踪
- **条件**：玩家完成条件（如击杀 3 狼）
- **结果**：任务进度自动更新

### AC-6: 任务完成 Toast
- **条件**：任务条件全部满足
- **结果**：4s Toast + 奖励发放

### AC-7: 任务奖励分级
- **条件**：主线 / 支线 / 远征 / 日常 完成
- **结果**：奖励按类型分级（主线 > 支线 > 远征 > 日常）

### AC-8: 主线任务顺序
- **条件**：主线 N 完成
- **结果**：主线 N+1 自动出现 + 主线 N 永久记录

### AC-9: 远征失败
- **条件**：玩家在远征中死亡
- **结果**：远征任务失败 + 记录

### AC-10: 任务持久化
- **条件**：退出 + 重新进入
- **结果**：任务进度 + 完成状态**全部**保留

### AC-11: 世界事件触发
- **条件**：12 真实小时 + 玩家完成事件
- **结果**：世界事件触发 + 24h 全局 modifier

### AC-12: 世界事件 → 任务联动
- **条件**："狼群异常" 世界事件触发
- **结果**："森林里的狼" 主线任务出现

---

## 10. 已锁定决策（Locked Decisions）

> 2026-07-27 用户拍板，12 个开放问题全部锁定。已落地为 §G 旋钮 + data-config v2.1。

| # | 决策点 | 锁定值 | 落地位置 |
|---|---|---|---|
| 1 | **v1.0 主线任务数** | **10**（=5 誓言 × 2） | §C.2 类型 1 + §G |
| 2 | **v1.0 支线任务数** | **15** | §C.2 类型 2 + §G |
| 3 | **v1.0 日常任务数** | **10** | §C.2 类型 4 + §G |
| 4 | **日常任务刷新周期** | **24 真实小时** | §C.2 类型 4 + §G |
| 5 | **同时存在的世界事件数** | **3**（避免 UI 过载） | §C.3 + §G |
| 6 | **世界事件触发最小间隔** | **12 真实小时** | §C.3 + §G |
| 7 | **1 周目最少世界事件数** | **5**（保证事件体验） | §C.3 + §G |
| 8 | **任务可放弃** | **否**（承诺有重量） | §C.7 + §G |
| 9 | **主线任务顺序触发** | **是**（避免跳过） | §C.4 + §G |
| 10 | **任务日志最大活动数** | **30** | §C.6 + §G |
| 11 | **完成 Toast 时长** | **4 秒** | §C.6 + §G |
| 12 | **任务奖励乘数** | **×1.0**（v1.0 基准值） | §C.5 + §G |

### 决策之间的协同

- **#1 + #9 + Oath 5.x**：10 主线 + 顺序触发 + Oath 5 誓言 = **"叙事主线清晰"**——Oath 5.x 全部由主线任务推进
- **#2 + Einherjar + Death-Send-off**：15 支线 + 剧情英灵 + 送别 = **"支线丰富但有边界"**——v1.0 不可放弃 = 玩家深度参与
- **#3 + #4 + #5 + #6 + #7**：10 日常 + 24h + 3 世界事件 + 12h + 5/周目 = **"节奏感"**——v1.0 体验丰富但不刷屏
- **#8 + #10 + #11 + #12**：不可放弃 + 30 上限 + 4s + ×1.0 = **"承诺有重量 + 体验清晰"**

### 仍待 playtest 调参（不阻塞 GDD 锁定）

| 待调项 | 候选范围 | 调参位置 |
|---|---|---|
| 主线任务顺序 vs 支线重叠 | 严格顺序 vs 灵活 | playtest |
| 日常任务奖励量 | 5-30 god-ember | `GameConfigSO.questRewardMultiplier` |
| 世界事件权重 | 5 类权重比 | `WorldEventSO.weight` |
| 任务奖励分布 | 50% 资源 / 30% buff / 20% 配方 | `QuestSO.rewards` 设计 |
| 远征任务 vs 主线任务奖励对比 | 远征 < 主线 < 誓约 | `ExpeditionSO` / `QuestSO` |

→ 这些都是 Prototype 阶段的**视觉/数值调参工作**，通过 `GameConfigSO` / `QuestSO` / `WorldEventSO` 直接改即可，不阻塞任何 GDD。

---

> 12 个开放问题待用户拍板。

1. **v1.0 主线任务数**
   - 我的推荐：**10**（=5 誓言 × 2 任务）
2. **v1.0 支线任务数**
   - 我的推荐：**15**（剧情英灵 + 群系事件）
3. **v1.0 日常任务数**
   - 我的推荐：**10**（24h 周期）
4. **日常任务刷新周期**
   - 我的推荐：**24 真实小时**
5. **同时存在的世界事件数**
   - 我的推荐：**3**（避免 UI 过载）
6. **世界事件触发最小间隔**
   - 我的推荐：**12 真实小时**
7. **1 周目最少触发世界事件数**
   - 我的推荐：**5**（保证 1 周目有事件体验）
8. **任务可放弃？**
   - 我的推荐：**否**（v1.0 锁定：承诺有重量）
9. **主线任务顺序触发？**
   - 我的推荐：**是**（v1.0 锁定：避免跳过）
10. **任务日志最大活动数**
    - 我的推荐：**30**（足够）
11. **完成 Toast 时长**
    - 我的推荐：**4 秒**（足够阅读）
12. **任务奖励乘数**
    - 我的推荐：**×1.0**（v1.0 基准值）

---

## Unity Implementation Notes

### 核心脚本（`Assets/Scripts/QuestEvent/`）
- `QuestManager.cs` —— 任务管理（4 类）
- `MainQuestLine.cs` —— 10 主线
- `SideQuestLine.cs` —— 15 支线
- `DailyQuestSystem.cs` —— 10 日常 + 24h 刷新
- `WorldEventManager.cs` —— 世界事件触发 + 持续
- `QuestLogUI.cs` —— 任务日志（按 J）
- `QuestCompleteToast.cs` —— 完成 Toast
- `QuestRewardSystem.cs` —— 奖励发放

### 数据结构
```csharp
public class Quest {
  public QuestSO data;                   // 任务 SO
  public QuestState state;               // Pending / Active / Completed / Failed
  public List<QuestCondition> conditions;
  public List<QuestReward> rewards;
  public float elapsedTime;              // 任务用时
  public DateTime startDate;             // 游戏内日期
}

public enum QuestState {
  Pending,        // 等待触发
  Active,         // 进行中
  Completed,      // 完成
  Failed,         // 失败
}

public enum QuestType {
  Main,
  Side,
  Expedition,
  Daily,
}
```

### 状态机
```csharp
public enum QuestState {
  Pending,        // 等待条件触发
  Active,         // 玩家可做
  Completed,      // 永久消失
  Failed,         // 远征死亡 / 日常超时
}
```

### 事件订阅
```csharp
public class QuestManager : MonoBehaviour {
  public static event Action<Quest> OnQuestTriggered;
  public static event Action<Quest> OnQuestCompleted;
  public static event Action<Quest> OnQuestFailed;
  public static event Action<WorldEventSO> OnWorldEventTriggered;
  public static event Action<WorldEventSO> OnWorldEventEnded;
}
```

### 性能预算
- 任务日志：< 4ms / 帧
- 完成 Toast：< 2ms / 帧
- 世界事件触发：< 1ms / 帧
- 条件检查：< 1ms / 任务

### 资产制作
- **10 主线 + 15 支线 = 25 QuestSO** + 每个 1 个 icon
- **10 日常 = 10 QuestSO**
- **18 远征 = 18 QuestSO**（与 world-exploration 共享）
- **20 WorldEventSO**（已有类型 7）
- **4 类 Toast VFX**（主线/支线/远征/日常 各自不同色）

---

## 进度

| 章节 | 状态 |
|---|---|
| A. Overview | ✅ |
| B. Player Fantasy | ✅ |
| C. Detailed Design (8 小节) | ✅ |
| D. Formulas (6 个) | ✅ |
| E. Edge Cases (15 种) | ✅ |
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
| 2026-07-27 | v1.0 LOCKED | 12 开放问题用户拍板全部锁定；data-config v2.1 同步升级 + 新增 `QuestSO`（类型 14） | Mavis + 用户 |
