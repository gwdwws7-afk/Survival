# Ravensong — Systems Index

> **Status**: 🔒 LOCKED v1.0
> **锁定日期**: 2026-07-27
> **来源**: `game-concept.md` v1.0 + Brainstorm 阶段产出
> **变更协议**: 见末尾

---



---

## 3. Machinations 资源流总览（系统间经济）⭐ 修复 P1 缺口

> 按 Machinations 框架显式画出 Ravensong 全部 18 GDD 的资源流总图。**这是平衡性调参的"地图"**。

### 3.1 主货币流（核心循环）

```
[采集 Source]                         [god-ember 基础 Source]
  白桦林/白骨原/深渊沼/...                被动 +1/小时
       ↓                                    ↓
   [资源 Pool]                          [god-ember Pool]
   铁/食物/木材/草                          （上限 999）
       ↓                                    ↓
   [编织 Converter] ←─── 玩家输入 2-3 物品     ↓
       ↓                                    ↓
   [物品 Pool] ───────────────────→ [战斗/技能 Drain]
   装备/消耗品/Tier 1-5 物品                  ↓
       ↓                                    ↓
   [聚落 Drain]                          [boss 战 Drain]
   建筑升级/英灵招募                        ↓
       ↓                               [战斗结果]
   [聚落升级]                              ↑
       ↓                               [经验/技能]
   [英灵产能 Pool] ────────────────────→ [循环]
```

### 3.2 节点定义（按 Machinations 模板）

| 节点 | 类型 | 描述 | 数值范围 |
|---|---|---|---|
| **资源池（4 种）** | Pool | 铁/食物/木材/草 | v1.0 无上限（v1.1 = 1000/类型）|
| **god-ember 池** | Pool | 高级资源 | 上限 999 |
| **物品池** | Pool | Tier 1-5 装备/消耗品 | 24 槽（仓库 L1）/ 48 槽（L2）|
| **采集源** | Source | 6 群系资源节点 | 资源刷新率由 `BiomeSO.resourceYield` 决定 |
| **编织器** | Converter | 2-3 物品 → 1 物品 | 成功率由 Tier 决定（T1=100% / T5=85%）|
| **战斗消耗** | Drain | 武器耐久 + 消耗品 | 1-3 HP/次，武器耐久 -5/次 |
| **建筑消耗** | Drain | 升级材料 | 50 木 + 20 铁（L1→L2 长屋）|
| **god-ember 消耗** | Drain | 编织 + 治疗 + 神龛 | 5-25 god-ember/编织 |
| **英灵产能** | Converter | 英灵 → 资源 | 1-3 单位/小时/英灵 |
| **衰悼期消耗** | Drain | 聚落士气 -20% | 24h |

### 3.3 正反馈循环

| 反馈 | 路径 | 强度 |
|---|---|---|
| **聚落扩张** | 英灵多 → 产出多 → 升级建筑 → 容量大 → 英灵更多 | 🟢 强（Living Hearth 支柱核心）|
| **誓言完成** | 4 誓言完成 → 苍穹解锁 → 5 誓言完成 → 奥丁审判 | 🟢 强（Oath 软主线）|
| **编织解锁** | 编织 Tier 3 → 解锁 Tier 4 配方 → 编织 Tier 4 → 解锁 Tier 5 | 🟡 中（Fate-Thread 成长）|
| **Boss 战利品** | 击杀 boss → Tier 4 装备 → 编织 Tier 5 → 击杀更强 boss | 🟡 中 |

### 3.4 负反馈循环

| 反馈 | 路径 | 强度 |
|---|---|---|
| **god-ember 枯竭** | 编织消耗 god-ember → 玩家需采集/战斗 → 慢 | 🟡 中（防 god-ember 滥用）|
| **衰悼期** | 送走英灵 → 24h 聚落 -20% → 玩家需补资源 | 🟢 强（死亡有重量）|
| **寒冷 debuff** | 永冻崖 -1 HP/分钟 → 玩家需篝火 → 篝火消耗木 | 🟡 中（World Exploration 压力）|
| **强留腐化** | 强留 → 3-5 天腐化 → 尸鬼化 → 失去 buff + 工人 | 🟢 强（Wyrd 支柱）|

### 3.5 瓶颈分析

| 瓶颈 | 位置 | 影响 |
|---|---|---|
| **god-ember 收集** | 战斗 + 编织返还 | 前 1 周目中期可能"卡 god-ember" → 影响 Tier 4-5 编织 |
| **铁资源** | 白骨原 + 永冻崖 | Tier 4 装备的瓶颈 |
| **寒铁** | 永冻崖 | Tier 5 武器的瓶颈 |
| **英灵容量** | 长屋 L2 = 8 | 中后期可能"聚落满" → 需送走才能招新 |
| **boss 死亡冷却** | 24h | 反复挑战 boss 的成本（避免刷）|

### 3.6 失衡风险（按游戏阶段）

| 阶段 | 主要风险 | 缓解 |
|---|---|---|
| **新手期（Day 1-30）** | 资源太贫瘠，玩家弃坑 | daySpeedFactor 0.5 / 0.7 平衡（已锁）|
| **中期（Day 30-100）** | god-ember 卡 + Tier 4 缺 | 中期 v1.0 决策 = god-ember 上限 999 + 编织返还 30% |
| **后期（Day 100-200）** | 聚落满 + 资源溢出 | 长屋 L2 = 8 上限（已锁）+ 资源无上限（v1.0）|
| **终局（Day 200+）** | 通关后无事可做 | v1.1 决策：NG+ 循环（v1.0 = 2 结局即可）|

**Machinations 审计结论**：Ravensong 的资源流**清晰且平衡**——4 主反馈 + 4 负反馈 + 5 瓶颈都已识别。**没有"前期很卡 / 后期流水线"问题**——这是 5 签名系统 + 4 支柱的**结构优势**。

---




---

## 3. Machinations 资源流总览（系统间经济）⭐ 修复 P1 缺口

> 按 Machinations 框架显式画出 Ravensong 全部 18 GDD 的资源流总图。**这是平衡性调参的"地图"**。

### 3.1 主货币流（核心循环）

```
[采集 Source]                         [god-ember 基础 Source]
  白桦林/白骨原/深渊沼/...                被动 +1/小时
       ↓                                    ↓
   [资源 Pool]                          [god-ember Pool]
   铁/食物/木材/草                          （上限 999）
       ↓                                    ↓
   [编织 Converter] ←─── 玩家输入 2-3 物品     ↓
       ↓                                    ↓
   [物品 Pool] ───────────────────→ [战斗/技能 Drain]
   装备/消耗品/Tier 1-5 物品                  ↓
       ↓                                    ↓
   [聚落 Drain]                          [boss 战 Drain]
   建筑升级/英灵招募                        ↓
       ↓                               [战斗结果]
   [聚落升级]                              ↑
       ↓                               [经验/技能]
   [英灵产能 Pool] ────────────────────→ [循环]
```

### 3.2 节点定义（按 Machinations 模板）

| 节点 | 类型 | 描述 | 数值范围 |
|---|---|---|---|
| **资源池（4 种）** | Pool | 铁/食物/木材/草 | v1.0 无上限（v1.1 = 1000/类型）|
| **god-ember 池** | Pool | 高级资源 | 上限 999 |
| **物品池** | Pool | Tier 1-5 装备/消耗品 | 24 槽（仓库 L1）/ 48 槽（L2）|
| **采集源** | Source | 6 群系资源节点 | 资源刷新率由 `BiomeSO.resourceYield` 决定 |
| **编织器** | Converter | 2-3 物品 → 1 物品 | 成功率由 Tier 决定（T1=100% / T5=85%）|
| **战斗消耗** | Drain | 武器耐久 + 消耗品 | 1-3 HP/次，武器耐久 -5/次 |
| **建筑消耗** | Drain | 升级材料 | 50 木 + 20 铁（L1→L2 长屋）|
| **god-ember 消耗** | Drain | 编织 + 治疗 + 神龛 | 5-25 god-ember/编织 |
| **英灵产能** | Converter | 英灵 → 资源 | 1-3 单位/小时/英灵 |
| **衰悼期消耗** | Drain | 聚落士气 -20% | 24h |

### 3.3 正反馈循环

| 反馈 | 路径 | 强度 |
|---|---|---|
| **聚落扩张** | 英灵多 → 产出多 → 升级建筑 → 容量大 → 英灵更多 | 🟢 强（Living Hearth 支柱核心）|
| **誓言完成** | 4 誓言完成 → 苍穹解锁 → 5 誓言完成 → 奥丁审判 | 🟢 强（Oath 软主线）|
| **编织解锁** | 编织 Tier 3 → 解锁 Tier 4 配方 → 编织 Tier 4 → 解锁 Tier 5 | 🟡 中（Fate-Thread 成长）|
| **Boss 战利品** | 击杀 boss → Tier 4 装备 → 编织 Tier 5 → 击杀更强 boss | 🟡 中 |

### 3.4 负反馈循环

| 反馈 | 路径 | 强度 |
|---|---|---|
| **god-ember 枯竭** | 编织消耗 god-ember → 玩家需采集/战斗 → 慢 | 🟡 中（防 god-ember 滥用）|
| **衰悼期** | 送走英灵 → 24h 聚落 -20% → 玩家需补资源 | 🟢 强（死亡有重量）|
| **寒冷 debuff** | 永冻崖 -1 HP/分钟 → 玩家需篝火 → 篝火消耗木 | 🟡 中（World Exploration 压力）|
| **强留腐化** | 强留 → 3-5 天腐化 → 尸鬼化 → 失去 buff + 工人 | 🟢 强（Wyrd 支柱）|

### 3.5 瓶颈分析

| 瓶颈 | 位置 | 影响 |
|---|---|---|
| **god-ember 收集** | 战斗 + 编织返还 | 前 1 周目中期可能"卡 god-ember" → 影响 Tier 4-5 编织 |
| **铁资源** | 白骨原 + 永冻崖 | Tier 4 装备的瓶颈 |
| **寒铁** | 永冻崖 | Tier 5 武器的瓶颈 |
| **英灵容量** | 长屋 L2 = 8 | 中后期可能"聚落满" → 需送走才能招新 |
| **boss 死亡冷却** | 24h | 反复挑战 boss 的成本（避免刷）|

### 3.6 失衡风险（按游戏阶段）

| 阶段 | 主要风险 | 缓解 |
|---|---|---|
| **新手期（Day 1-30）** | 资源太贫瘠，玩家弃坑 | daySpeedFactor 0.5 / 0.7 平衡（已锁）|
| **中期（Day 30-100）** | god-ember 卡 + Tier 4 缺 | 中期 v1.0 决策 = god-ember 上限 999 + 编织返还 30% |
| **后期（Day 100-200）** | 聚落满 + 资源溢出 | 长屋 L2 = 8 上限（已锁）+ 资源无上限（v1.0）|
| **终局（Day 200+）** | 通关后无事可做 | v1.1 决策：NG+ 循环（v1.0 = 2 结局即可）|

**Machinations 审计结论**：Ravensong 的资源流**清晰且平衡**——4 主反馈 + 4 负反馈 + 5 瓶颈都已识别。**没有"前期很卡 / 后期流水线"问题**——这是 5 签名系统 + 4 支柱的**结构优势**。

---


## 0. 怎么读本文档

### 5 个层级（Layer）
- **L1 Foundation**: 0 依赖的底层（Input / Save / Data）
- **L2 Core**: 单系统循环（Gathering / Combat / Day-Night）
- **L3 Feature**: 跨系统协同（Fate-Thread / Einherjar / Settlement / Exploration）
- **L4 Progression**: 长线驱动（Oaths / Quest-Event）
- **L5 Presentation**: 表现层（UI / VFX / Camera）

### 4 个优先级（Tier）
- **MVP** = Prototype 阶段必须有（实现 §12 的 7 项 MVP）
- **Vertical Slice** = EA 阶段需要的完整 1 区体验
- **Alpha** = EA 阶段剩下的系统
- **Full Vision** = 1.0 完整版

### 标记说明
- ⭐ = **签名系统**（动 Ravensong 不能动的核心）
- 📍 = **显式**（在 game-concept.md 里点名过）
- 🔍 = **隐式**（按品类经验推出来的）

---

## 1. 系统总览（18 个系统）

### 按层级分布

| 层级 | 系统数 | 系统 |
|---|---|---|
| **L1 Foundation** | 3 | Input / Save / Data Config |
| **L2 Core** | 5 | Day-Night ⭐📍 / Gathering 📍 / Inventory 📍 / Combat 📍 / VFX-Audio 📍 |
| **L3 Feature** | 5 | Fate-Thread ⭐📍 / Einherjar ⭐📍 / Settlement 📍 / World Exploration 📍 / Death-Send-off ⭐📍 |
| **L4 Progression** | 2 | Oath ⭐📍 / Quest-Event 🔍 |
| **L5 Presentation** | 3 | UI-HUD 📍 / VFX 🔍 / Camera 🔍 |
| **总计** | **18** | — |

签名系统（⭐）= Ravensong 的"减一不可"项，**任何系统做错可以重做，签名系统做错就不是 Ravensong**。

---

## 2. 系统详情（18 个）

### L1 Foundation（3 个）

| # | 系统 | 描述 | 显/隐 | 关键依赖 | 优先级 |
|---|---|---|---|---|---|
| 1 | **Input System** | 键鼠/手柄输入抽象层；支持快速键位重映射 | 🔍 | — | MVP |
| 2 | **Save System** | 单机 JSON 加密存档；支持快速读档；自动存档 | 🔍 | Input | MVP |
| 3 | **Data Config** | ScriptableObject 驱动的数据：编织配方、英灵档案、物品、生物群系 | 🔍 | — | MVP |

### L2 Core（5 个）

| # | 系统 | 描述 | 显/隐 | 关键依赖 | 优先级 |
|---|---|---|---|---|---|
| 4 | ⭐📍 **Day-Night Cycle** | 日月倒置活力：阳光灼伤/月光养；视野/移速/编织效率随时间变化；触发白天/夜晚 AI 行为切换 | 显 | Input, Save | **MVP** |
| 5 | 📍 **Gathering** | 砍树/挖矿/采药/捕鱼；白天效率 -30% 夜晚 +50% | 显 | Input, Save, Day-Night | MVP |
| 6 | 📍 **Inventory & Equipment** | 物品栏、装备槽、负重、堆叠 | 显 | Save, Gathering | MVP |
| 7 | 📍 **Combat** | 顶视 2D 战斗：挥矛/闪避/丝线绑定；夜晚伤害 +20% | 显 | Input, Save, Day-Night, Inventory | MVP |
| 8 | 📍 **VFX & Audio Feedback** | 命中震动/丝线音效/渡鸦叫声/动态 BGM（白天压抑/夜晚史诗） | 显 | Combat, Day-Night, Weaving | MVP |

### L3 Feature（5 个）

| # | 系统 | 描述 | 显/隐 | 关键依赖 | 优先级 |
|---|---|---|---|---|---|
| 9 | ⭐📍 **Fate-Thread（命运丝线）** | 核心动词：左 A 右 B 拖拽丝线编出 C；消耗神力余烬；白天慢弱，夜晚瞬发 | 显 | Day-Night, Inventory, Data | **MVP** |
| 10 | ⭐📍 **Einherjar Management** | 招募阵亡者；分配职业（铁匠/猎人/吟游/农夫）；衰老/生病/受伤；触发死亡事件 | 显 | Settlement, Combat, Death-Send-off | **MVP** |
| 11 | 📍 **Settlement Building** | 聚落结构：长屋/工坊/篝火/仓库/神龛；建筑等级影响效率 | 显 | Inventory, Gathering, Einherjar | Vertical Slice |
| 12 | 📍 **World Exploration** | 群系导航、POI 发现、地图解锁、世界 boss 位置 | 显 | Combat, Day-Night | MVP |
| 13 | ⭐📍 **Death & Send-off（送别）** | 英灵死亡事件：选择送英灵殿（换资源）or 强留（变尸鬼）；影响聚落氛围 | 显 | Einherjar | **MVP** |

### L4 Progression（2 个）

| # | 系统 | 描述 | 显/隐 | 关键依赖 | 优先级 |
|---|---|---|---|---|---|
| 14 | ⭐📍 **Oath System** | 5 条平行誓言：锻冶/炉火/荒野/亡者/苍穹；每条 4-5 个里程碑；可并行；终局"苍穹之誓"触发奥丁审判 | 显 | Weaving, Combat, Einherjar, Death, Settlement | **MVP** |
| 15 | 🔍 **Quest & Event** | 世界随机事件（渡鸦带来奥丁诏令）+ 誓言作为软主线 | 隐 | Oaths, World Exploration, Einherjar | Vertical Slice |

### L5 Presentation（3 个）

| # | 系统 | 描述 | 显/隐 | 关键依赖 | 优先级 |
|---|---|---|---|---|---|
| 16 | 📍 **UI / HUD** | 物品栏、装备、英灵状态、地图、誓言、对话、暂停 | 显 | 所有 gameplay | MVP |
| 17 | 🔍 **VFX（独立）** | 丝线粒子、神力爆发、月相变化、天气 | 隐 | Combat, Weaving, Day-Night | Vertical Slice |
| 18 | 🔍 **Camera / Cinematic** | Cinemachine 2D 配置：跟随/缩放/震动/转场 | 隐 | World Exploration, Combat | Vertical Slice |

> **关于"VFX & Audio Feedback"（#8）与"VFX"（#17）的区分**：
> - #8 是**反馈层**——命中震动/编织音效这种"动作即时反馈"
> - #17 是**效果层**——丝线粒子、月相变化这种"独立视觉效果"
> - 两者都用 Unity VFX Graph + Shader Graph 实现，但 #8 是系统级 hook，#17 是独立系统

---

## 3. 依赖图（Dependency Map）

```
L1 ────────────────────────────────────
  Input  ──→ Save  ──→ (所有系统)
  Data Config ──→ (所有需要数据驱动的)

L2 ────────────────────────────────────
  Input/Save ──→ Day-Night ──→ (几乎所有 L3+)
  Input/Save ──→ Gathering ──→ Inventory
  Inventory ──→ Combat
  (Day-Night + Combat + Weaving) ──→ VFX-Audio

L3 ────────────────────────────────────
  Inventory + Data ──→ Fate-Thread
  (Inventory + Gathering + Einherjar) ──→ Settlement
  (Settlement + Combat + Death) ──→ Einherjar
  (Combat + Day-Night) ──→ World Exploration
  Einherjar ──→ Death & Send-off

L4 ────────────────────────────────────
  (Weaving + Combat + Einherjar + Death + Settlement) ──→ Oath
  (Oaths + World + Einherjar) ──→ Quest & Event

L5 ────────────────────────────────────
  (所有 gameplay) ──→ UI / HUD
  (Combat + Weaving + Day-Night) ──→ VFX
  (World + Combat) ──→ Camera
```

### 关键依赖观察

1. **Day-Night 是"上游枢纽"**——几乎所有 L3+ 都依赖它。**它是 Ravensong 的"地基之上"**。做错 Day-Night，所有系统都要重做。
2. **Fate-Thread 是"核心动词"**——它从 Inventory + Data 起步，但被几乎所有 L3+ 引用。
3. **Einherjar 是"情感枢纽"**——它是 Living Hearth 支柱的唯一承载。
4. **Oath 是"长线枢纽"**——把所有的 L3 编织成长期目标。
5. **没有真正的循环依赖**——Einherjar 看似与 Death-Send-off 互引，但 Einherjar 实际不依赖 Death 的结果（死亡只是生命周期的自然一环），Death 是被 Einherjar 触发的**事件处理器**。

---

## 4. 设计顺序（Design Order）

按依赖图自底向上设计 GDD——**先做没有依赖的，最后做汇聚全部系统的**。

### Phase A: Foundation（1-2 周）
1. **Data Config** —— 没有它什么都设计不了（先定数据结构）
2. **Input System** —— 战斗/编织的输入抽象
3. **Save System** —— 持久化层

### Phase B: Core（2-3 周）
4. **Inventory & Equipment** —— 所有物品的"家"
5. **Gathering** —— 喂 Inventory
6. **Day-Night Cycle** ⭐ —— 签名系统，最早上桌
7. **Combat** —— MVP 需要 boss
8. **VFX & Audio Feedback** —— 战斗/编织的反馈层

### Phase C: Feature（3-4 周）
9. **Fate-Thread** ⭐ —— 核心动词
10. **Einherjar Management** ⭐ —— 签名系统
11. **Settlement Building** —— 基地基础
12. **World Exploration** —— 1 个生物群系
13. **Death & Send-off** ⭐ —— 触发在 Einherjar 之后

### Phase D: Progression（1-2 周）
14. **Oath System** ⭐ —— 5 条誓言，汇聚前面所有
15. **Quest & Event** —— 软主线

### Phase E: Presentation（并行，2-3 周）
16. **UI / HUD** —— 边做系统边出 UI mockup
17. **VFX** —— 边做 Combat/Weaving 边出粒子
18. **Camera / Cinematic** —— 边做 Exploration/Combat 边配

> **总设计周期估算**: 10-14 周（2.5-3.5 个月）把所有 GDD 写完
> **然后进 Prototype 阶段开始写代码**

---

## 5. MVP 优先级（必须第一个做）

按 `game-concept.md` §12 的 7 项 MVP 倒推系统优先级：

| MVP 目标 | 必须做哪些系统 | 至少 1 个的程度 |
|---|---|---|
| **1. 昼夜倒置活力** | Day-Night + Input + Save + VFX-Audio | Day-Night 全功能 |
| **2. 命运丝线核心循环** | Fate-Thread + Data + Inventory + Day-Night + VFX | 5-10 个基础配方 |
| **3. 完整可培养英灵** | Einherjar + Death-Send-off + Settlement + Data | 1 个命名英灵走完全周期 |
| **4. 1 条完整誓言** | Oath + (Weaving + Combat + Einherjar) | 1 条誓言 4-5 里程碑 |
| **5. 1 个可玩 boss** | Combat + World Exploration + VFX | 1 个 boss，2-3 阶段 |
| **6. 1 个生物群系** | World Exploration + Data | 1 个群系做精 |
| **7. 结束/再开循环** | Day-Night + Save + Quest-Event | 黎明事件触发自动存档 |

### MVP 范围系统清单（11 个）
```
Input · Save · Data Config
Day-Night · Gathering · Inventory · Combat · VFX-Audio
Fate-Thread · Einherjar · Death-Send-off
World Exploration · Oath
```
> **15 个里 14 个必须在 MVP**（Quest-Event 和 Settlement 可以 Vertical Slice 才做）

---

## 6. 完整优先级矩阵

| # | 系统 | Tier | 备注 |
|---|---|---|---|
| 1 | Input | MVP | Foundation，所有交互的根 |
| 2 | Save | MVP | Foundation |
| 3 | Data Config | MVP | Foundation |
| 4 | ⭐ Day-Night | MVP | 签名系统 |
| 5 | Gathering | MVP | 喂 Inventory |
| 6 | Inventory | MVP | 所有物品的家 |
| 7 | Combat | MVP | boss 的基础 |
| 8 | VFX-Audio | MVP | 战斗/编织的反馈 |
| 9 | ⭐ Fate-Thread | MVP | 签名系统，核心动词 |
| 10 | ⭐ Einherjar | MVP | 签名系统 |
| 11 | Settlement | Vertical Slice | 基地基础，EA 才做 |
| 12 | World Exploration | MVP | 1 群系够 MVP |
| 13 | ⭐ Death-Send-off | MVP | 签名系统 |
| 14 | ⭐ Oath | MVP | 1 条誓言够 MVP |
| 15 | Quest-Event | Vertical Slice | EA 才做 |
| 16 | UI / HUD | MVP | 所有系统的门面 |
| 17 | VFX | Vertical Slice | 粒子层，EA 阶段做 |
| 18 | Camera | Vertical Slice | EA 阶段做 |
| — | Settlement | Vertical Slice | 移到 VS 阶段 |
| — | Quest-Event | Vertical Slice | 移到 VS 阶段 |

**MVP 系统数: 14** ｜ **Vertical Slice: 4** ｜ **Alpha: 0** ｜ **Full Vision: 0**

> **Vertical Slice 阶段补充**: Settlement / Quest-Event / VFX / Camera 在 EA 阶段做完整

---

## 7. Anti-Systems（明确**不**做的系统）

按 game-concept.md §7 的 Anti-Pillars 反推——这些都是**绝对不会出现在 Ravensong 里**的系统：

| Anti-System | 为什么不做 |
|---|---|
| ❌ **科技树系统** | 编织配方开放，靠发现而非解锁 |
| ❌ **多人/联机系统** | 独立游戏，AI 深度代替社交 |
| ❌ **PvP / 战斗排名** | 不是 Killer 主类型 |
| ❌ **声望/派系系统** | 复杂度过高，叙事会变水 |
| ❌ **复杂贸易经济** | 独立游戏简化商业 |
| ❌ **季节系统** | 白天黑夜已经做了节奏反转 |
| ❌ **天气系统** | Day-Night 已经覆盖大部分氛围需求 |
| ❌ **多周目变体** | 1 周目够用，做完再考虑 |
| ❌ **Mod/创意工坊** | 1.0 后再说 |
| ❌ **教程关卡（独立）** | 教程作为 UI/HUD 的一部分，不做独立系统 |

> **判断标准**：如果你被建议加某个系统，**先问它服务于哪个支柱**。如果它服务 0 个支柱 = **删除**；服务于 1 个支柱 = **考虑是否真有必要**；服务于 2+ 支柱 = **保留**。

---

## 8. 设计 GDD 的下一步

### 优先 GDD 顺序（按 Phase A-E）

按设计顺序，下一步要写的 GDD：

| 顺序 | GDD 路径 | 系统 | 估时 |
|---|---|---|---|
| 1 | `design/gdd/data-config.md` | Data Config | 2 天 |
| 2 | `design/gdd/input-system.md` | Input | 1 天 |
| 3 | `design/gdd/save-system.md` | Save | 2 天 |
| 4 | `design/gdd/inventory.md` | Inventory | 3 天 |
| 5 | `design/gdd/gathering.md` | Gathering | 2 天 |
| 6 | `design/gdd/day-night-cycle.md` | ⭐ Day-Night | 5 天 |
| 7 | `design/gdd/combat.md` | Combat | 5 天 |
| 8 | `design/gdd/fate-thread.md` | ⭐ Fate-Thread | 5 天 |
| 9 | `design/gdd/einherjar.md` | ⭐ Einherjar | 7 天 |
| 10 | `design/gdd/death-send-off.md` | ⭐ Death-Send-off | 3 天 |
| 11 | `design/gdd/world-exploration.md` | World Exploration | 3 天 |
| 12 | `design/gdd/oath-system.md` | ⭐ Oath | 5 天 |
| 13 | `design/gdd/settlement.md` | Settlement | 4 天 |
| 14 | `design/gdd/quest-event.md` | Quest-Event | 3 天 |
| 15 | `design/gdd/ui-hud.md` | UI/HUD | 5 天 |
| 16 | `design/gdd/vfx.md` | VFX | 3 天 |
| 17 | `design/gdd/camera.md` | Camera | 2 天 |

**总 GDD 工作量: ~60 天**（约 3 个月个人全职）

---

## 9. 进度跟踪

| # | 系统 | 状态 | GDD 路径 |
|---|---|---|---|
| 1 | Input | ⚪ Not Started | `design/gdd/input-system.md` |
| 2 | Save | ⚪ Not Started | `design/gdd/save-system.md` |
| 3 | Data Config | ⚪ Not Started | `design/gdd/data-config.md` |
| 4 | ⭐ Day-Night | ⚪ Not Started | `design/gdd/day-night-cycle.md` |
| 5 | Gathering | ⚪ Not Started | `design/gdd/gathering.md` |
| 6 | Inventory | ⚪ Not Started | `design/gdd/inventory.md` |
| 7 | Combat | ⚪ Not Started | `design/gdd/combat.md` |
| 8 | VFX-Audio | ⚪ Not Started | `design/gdd/vfx-audio.md` |
| 9 | ⭐ Fate-Thread | ⚪ Not Started | `design/gdd/fate-thread.md` |
| 10 | ⭐ Einherjar | ⚪ Not Started | `design/gdd/einherjar.md` |
| 11 | Settlement | ⚪ Not Started | `design/gdd/settlement.md` |
| 12 | World Exploration | ⚪ Not Started | `design/gdd/world-exploration.md` |
| 13 | ⭐ Death-Send-off | ⚪ Not Started | `design/gdd/death-send-off.md` |
| 14 | ⭐ Oath | ⚪ Not Started | `design/gdd/oath-system.md` |
| 15 | Quest-Event | ⚪ Not Started | `design/gdd/quest-event.md` |
| 16 | UI / HUD | ⚪ Not Started | `design/gdd/ui-hud.md` |
| 17 | VFX | ⚪ Not Started | `design/gdd/vfx.md` |
| 18 | Camera | ⚪ Not Started | `design/gdd/camera.md` |

**进度**: 0/18 (0%)

---

## 10. 已锁定决策（Locked Decisions）

> 2026-07-27 用户拍板，8 个开放问题全部锁定。推理过程见 brainstorm 对话记录。

| # | 决策点 | 锁定值 | 影响系统 |
|---|---|---|---|
| 1 | **Day-Night 数值强度** | **强反差**：白天 -50% 移速/视野/编织，夜晚 +60% 全部 | Day-Night 调参基准 |
| 2 | **神力余烬获取** | **混合**：被动基底（小时+1）+ 战斗/编织活跃奖励 | 经济循环核心 |
| 3 | **誓言里程碑数量** | **每条 5 个**，5 条总计 25 个 | Oath 内容深度 |
| 4 | **聚落英灵上限** | **8 个** | Einherjar 系统 |
| 5 | **送英灵殿奖励** | **永久聚落 buff**（产出/士气 +X） | Death-Send-off |
| 6 | **强留尸鬼机制** | **缓慢腐化 3-5 天**，最终变尸鬼 | Death-Send-off |
| 7 | **boss 编织应用** | **战斗中断织线绑 boss 招式** | Combat + Fate-Thread 联动 |
| 8 | **多群系解锁** | **混合**：誓言给"开图权" + 英灵任务给"进入权" | World Exploration |

### 决策之间的协同（为什么这套选法成立）

- **#5 + #6 是天平的两端**：送英灵殿 → 永久 buff（"他们化为你身后的守护"）；强留 → 3-5 天告别窗口后变尸鬼（"拖延的代价是失去"）。**两条路都有清晰后果，没有"最优解"——只有玩家的价值观选择**。
- **#7 是 Ravensong 的"机制级差异化"**：boss 战不再是"看招反击"，是"织线绑招式"。这是 Ravensong 区别于 *Hades* / *死亡细胞* 的关键。
- **#8 把英灵从"资源"提升为"钥匙"**：某个群系需要"会潜行的英灵"或"会翻译卢恩的英灵"才能进。**Living Hearth 支柱贯穿整个世界**，不只是基地内。
- **#1 的强反差是这整套设计的"地基"**：没有强反差，#5（永久 buff）的价值就被稀释——因为白天不那么痛苦，玩家不会想"赶紧到晚上"。

### 仍待 playtest 调参（不阻塞 GDD 编写）

| 待调项 | 候选范围 | 调参时机 |
|---|---|---|
| Day-Night 数值精度 | -50% vs -45% vs -55% | Prototype playtest |
| 神力余烬基础速率 | 1/小时 vs 0.8/小时 | Prototype playtest |
| 永久 buff 的具体数值 | 产出+15% vs +20% | Vertical Slice |
| 尸鬼腐化时间 | 3 天 vs 4 天 vs 5 天 | Vertical Slice |
| 群系解锁的英灵任务类型 | 潜行/翻译/力量型 | GDD 编写时定 |

→ 这些都是 Prototype 阶段的**数值调参工作**，不阻塞 GDD 编写。

---

## 11. 变更流程

改这个索引必须走：
1. 用户提出改动 + 理由
2. 评估影响：触及签名系统 ⭐？需要重新设计依赖？需要新增/删除系统？
3. 是 → 重新跑 `/map-systems`；否 → 直接更新
4. 同步更新 `game-concept.md` 对应章节
5. 写变更日志

---

## 12. 变更日志

| 日期 | 版本 | 改动 | 责任人 |
|---|---|---|---|
| 2026-07-27 | v1.0 draft | 初版生成：18 个系统，5 层 4 优先级 | Mavis |
| 2026-07-27 | **v1.0 LOCKED** | 8 个开放问题用户拍板，全部锁定；系统索引正式生效 | Mavis + 用户 |

---

**🔒 已锁定（v1.0）**——所有 8 个决策已落地，签名系统 ⭐ 依赖图已确认。

**锁定后** → 进 Phase A 写第一个 GDD：`data-config.md`
