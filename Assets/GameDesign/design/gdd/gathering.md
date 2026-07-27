# Gathering — System GDD

> **Status**: 🔒 LOCKED v1.0
> **Author**: Mavis + 用户
> **Last Updated**: 2026-07-27
> **Implements Pillar**: Waxing Moon（反节奏的爽感）

---

## A. Overview

**Gathering 是 Ravensong 的"原料来源"——玩家从世界中获取 Resource，输入到 Fate-Thread 编织。** 5 种资源类型（Wood / Ore / Hide / Plant / Special），5 种采集动作（Chop / Mine / Pick / Hunt / Fish），全部受 **Day-Night 强烈影响**：白天 -30% 效率（慢 + 少），夜晚 +50% 效率（快 + 多），且部分稀有资源**只在特定时间出现**。

Gathering 是 Day-Night 反差在"生产端"的具体化——白天**只能**采基础资源（勉强够用），夜晚**可以**采稀有资源（解锁编织）。这与"白天守、夜晚出"的战斗节奏完美对应。

数据层全部由 `BiomeSO.resources[]` 和 `ItemSO` 驱动；本 GDD 专注于**采集动作、工具、Day-Night 协同**。

---

## B. Player Fantasy

### 主幻想
> "白天的米德加尔特是贫瘠的——夜晚才是收获的时节。"

### 关键体验时刻
- **第一次**在白天砍树：速度慢 1.5 倍，收获少 30%
- **第一次**经历夜晚在森林里砍同一棵树：速度**翻倍**（+50%），**多 50% 木材**
- **第一次**发现 Moonflower（夜间才开的花）："原来夜晚的森林有这种宝藏"
- **第一次**工具坏了：必须回聚落修理（或换新工具）
- **第一次**Wolf Pelt（夜间更易得）：意识到"夜晚打猎"才是真出路
- **第一次**资源耗尽（节点全砍光）：意识到"森林会恢复，但要等"

---

## C. Detailed Design

### C.1 核心规则（Core Rules）

#### 规则 1：5 种 Resource 类型
- **Wood**：树木采集（如 `item_ash_branch`）
- **Ore**：矿石采集（如 `item_iron_ore`）
- **Hide**：动物猎取（如 `item_wolf_hide`）
- **Plant**：植物采集（如 `item_moonflower`）
- **Special**：特殊来源（如 `item_ice_crystal` from frozen tundra）

#### 规则 2：5 种 Gathering Action
- **Chop**（砍）：Wood，需 Axe
- **Mine**（挖）：Ore，需 Pickaxe
- **Pick**（摘）：Plant，无需工具
- **Hunt**（猎）：Hide（+肉），需 Bow
- **Fish**（钓）：Special 鱼，需 Fishing Rod

#### 规则 3：所有动作都是"定时进度条"
- 玩家按交互键 → 进入"采集动作" → 进度条填充 → 完成
- 期间玩家**可移动取消**（释放键），但损失 50% 已完成进度
- 期间玩家**被攻击则打断**（0% 进度）

#### 规则 4：必须有相应工具才能进行 Chop / Mine / Hunt / Fish
- Pick（摘植物）不需要工具
- 工具耐久度耗尽 = 工具消失，必须修理或换新
- 没有工具 = 动作**不可用**（UI 灰显）

#### 规则 5：Day-Night 强烈影响效率（game-concept §3.2 锁定）
- 白天：**效率 × 0.7**（-30%）
- 夜晚：**效率 × 1.5**（+50%）
- 转换期（Dawn/Dusk）：smoothstep 插值
- 庇护所内采集（如聚落内部小花园）：不受 Day-Night 影响

#### 规则 6：部分 Resource 只在特定时间出现
- **Night-only**：Moonflower / 夜行动物（Wolf Pelt 概率 +50%）
- **Day-only**：Sunpetal / 阳光矿石
- **全时段**：基础 Wood / Ore / 普通 Plant / Hide

#### 规则 7：节点分两类
- **Renewable Node**：可再生（植物、矿石小堆）—— 采集后 N 小时重置
- **Limited Node**：不可再生（树、大矿石）—— 采集后**永久消失**，靠生物群系"自然再生"机制补充

### C.2 Resource Types 详解

| 类型 | 例子 | 主要采集 | 工具需求 | 节点类型 |
|---|---|---|---|---|
| Wood | ash_branch, oak_log | Chop | Axe | Limited |
| Ore | iron_ore, copper_ore | Mine | Pickaxe | Limited |
| Hide | wolf_hide, deer_hide | Hunt | Bow | Mobile（动物） |
| Plant | moonflower, sunpetal, wild_berry | Pick | None | Renewable |
| Special | ice_crystal, dragon_scale | Fish | Fishing Rod | Limited |

### C.3 Action Types 详解

#### Chop（砍）
- **目标**：Tree（树）
- **时间**：3 秒基础（Day 4.5s，Night 1.5s）
- **产出**：1-3 个 Wood（基础）+ 1-2 个 Wood（night bonus）
- **节点类型**：Limited（树被砍后永久消失，靠森林再生）
- **视觉反馈**：斧头挥动、树抖动、砍完后倒下变 stump

#### Mine（挖）
- **目标**：Rock（矿石）
- **时间**：4 秒基础
- **产出**：1-2 个 Ore
- **节点类型**：Limited
- **视觉反馈**：镐挥动、矿石裂缝、最后碎裂

#### Pick（摘）
- **目标**：Plant（植物）
- **时间**：1 秒基础
- **产出**：1-3 个 Plant
- **节点类型**：Renewable（24 游戏小时再生）
- **视觉反馈**：手轻抚、植物缩小消失
- **特殊**：Night-only Plant 只在 Night 出现

#### Hunt（猎）
- **目标**：Animal（动物）
- **时间**：5 秒基础（含战斗）
- **产出**：1-2 个 Hide + 1 个 Meat（Consumable）
- **节点类型**：Mobile（动物会跑、反击）
- **难度**：动物血量 = 3，普通攻击 5 次
- **视觉反馈**：弓拉满、箭射出、动物血条减少

#### Fish（钓）
- **目标**：Water（水域）
- **时间**：4 秒基础
- **产出**：1 个 Special 鱼
- **节点类型**：Limited（鱼塘一段时间后恢复）
- **视觉反馈**：浮标下沉、收线、鱼跳起

### C.4 Tools

**4 种工具 + 5 个 Tier**（tier 越高 = 越快 + 越耐久 + 越多产出）

| 工具 | Tier 1 | Tier 2 | Tier 3 | Tier 4 | Tier 5 |
|---|---|---|---|---|---|
| **Axe** | Wood Axe | Iron Axe | Steel Axe | Skyforged Axe | World Tree Axe |
| **Pickaxe** | Wood Pick | Iron Pick | Steel Pick | Skyforged Pick | Mjolnir Pick |
| **Bow** | Hunter Bow | Composite Bow | Longbow | Valkyrie Bow | Yggdrasil Bow |
| **Fishing Rod** | Twig Rod | Reed Rod | Bamboo Rod | Silver Rod | Rune Rod |

#### 工具属性
```csharp
public class ToolSO : ItemSO {
  public ToolType toolType;        // Axe/Pick/Bow/FishingRod
  public int tier;                  // 1-5
  public float gatherSpeedMult;     // 1.0 - 3.0
  public float gatherYieldMult;     // 1.0 - 2.0
  public int durability;            // 50-500 次使用
  public DayNightItemBonus? dayBonus;     // 部分工具有 Day-Night 加成
  public DayNightItemBonus? nightBonus;
}
```

#### 工具耐久
- 每次使用 -1 耐久
- 0 耐久 = 工具**消失**（从 inventory 移除）
- 修理 = 用对应材料修复（铁锭修铁工具等）
- 修理费用 = 基础材料的 30%

#### 工具获得
- Tier 1-2：从商人 / 任务奖励 / 编织产出
- Tier 3-4：高级编织（fate-thread）
- Tier 5：传说装备，世界 boss 掉落

### C.5 Day-Night 效率修正（⭐ Ravensong 核心）

**这是 Gathering 与 Day-Night 的具体协同。**

#### 全局效率公式
```csharp
float efficiencyMult = IsDay() ? 0.7f : 1.5f;
// Day: 0.7（-30%）
// Night: 1.5（+50%）
// Dawn/Dusk: smoothstep 插值
```

#### 影响的两个维度
1. **速度**：基础时间 × (1 / efficiencyMult)
   - Day：3 秒砍树 → 3 / 0.7 = 4.3 秒
   - Night：3 秒砍树 → 3 / 1.5 = 2 秒
2. **产出**：基础产出 × efficiencyMult
   - Day：1-3 个 Wood × 0.7 = 0.7-2.1（取整 1-2）
   - Night：1-3 个 Wood × 1.5 = 1.5-4.5（取整 2-4）

#### 特殊资源时间窗口
| Resource | 时间窗口 | 效果 |
|---|---|---|
| `item_moonflower` | 18:00 - 06:00 | Night only |
| `item_sunpetal` | 06:00 - 18:00 | Day only |
| `item_wolf_hide` | 全时段 | 夜晚概率 +50% |
| `item_ice_crystal` | 18:00 - 06:00 | Night only + 寒冷环境 |
| `item_ash_branch` | 全时段 | 无时间限制 |

**这些时间窗口是 Ravensong 的"采集日历"**——玩家会**记笔记**："月圆夜 = 冰晶 + 月花 + 狼皮"。

### C.6 节点（Resource Nodes）

#### 节点类型
- **Static Node**：固定位置（树、矿石、植物）—— 玩家到达位置才能采集
- **Mobile Node**：移动（动物）—— 玩家追击
- **Renewable**：可再生（植物 / 小矿石堆 / 鱼塘）
- **Limited**：不可再生（大树 / 大矿石）—— 采集后靠生物群系再生

#### 节点视觉
| 类型 | Sprite 数量 | 视觉 |
|---|---|---|
| Tree | 3 个变体 | 中等大小，2-3 帧 idle 动画（轻摇） |
| Rock | 2 个变体 | 小堆，0 动画 |
| Plant | 5+ 个变体 | 小，1 帧 idle |
| Animal | 4 个变体 | 中等，2 帧 walk 动画 |
| Water (鱼) | 1 个 tile | 整片水域 tile |

#### 节点状态
- **Idle**：等待玩家交互
- **Being Gathered**：进度条显示，玩家锁定动作
- **Depleted**：采集后变 stump / 碎石 / 空地
- **Renewing**（仅 Renewable）：淡入动画，N 小时后变回 Idle

### C.7 Spawn Rules（节点生成规则）

#### 初始生成
- 玩家进入新生物群系时，根据 `BiomeSO.resources[]` 预生成节点
- 节点位置 = 随机分布在生物群系内
- 节点密度由 `BiomeSO.resourceDensity` 控制

#### Limited 节点再生
- Limited 节点被采集后**不会立即再生**
- 玩家离开生物群系后，**每 24 游戏小时有 30% 概率**触发"群系再生"
- 触发时：群系内 30% 的 Limited 节点被补充
- 平衡：玩家不能"榨干"一个区域，但也不能一直等

#### Renewable 节点再生
- 节点被采集后进入"Depleted"状态
- 24 游戏小时后自动恢复为 Idle（无需玩家离开）
- 简单可预测

### C.8 Yields（产出）

#### 基础产出表
| 动作 | 基础最小 | 基础最大 | Day 实际 | Night 实际 |
|---|---|---|---|---|
| Chop | 1 | 3 | 0.7-2 | 2-4 |
| Mine | 1 | 2 | 0.7-1 | 2-3 |
| Pick | 1 | 3 | 0.7-2 | 2-4 |
| Hunt | 1 hide + 1 meat | 2 hide + 2 meat | 同上 | 同上 |
| Fish | 1 | 1 | 1 | 1 |

#### 工具加成
- Tier 1：× 1.0
- Tier 2：× 1.2
- Tier 3：× 1.5
- Tier 4：× 2.0
- Tier 5：× 3.0

#### 公式
```
finalYield = baseYield * efficiencyMult * toolYieldMult
```

### C.9 视觉与音频反馈

#### 视觉
- **进度条**：玩家头顶 / 节点上方，UI 显示百分比
- **挥动动画**：工具随进度播放
- **节点抖动**：受击时节点轻微抖动
- **粒子**：完成时节点碎裂 / 植物消失粒子
- **掉落**：资源飞向玩家（类似 Inventory 的拾取效果）

#### 音频
- **挥动**：axe swing / pick swing / bow draw
- **击中**：wood thunk / rock crack / plant rustle
- **完成**：node break VFX + collect chime
- **背景音**：forest ambient + day/night music

### C.10 与其他系统的交互

| 系统 | 怎么用 Gathering |
|---|---|
| **Day-Night** | 效率 × dayNight factor；时间窗口控制资源出现 |
| **Inventory** | 产出物直接入背包（满则落地） |
| **Fate-Thread** | Resource 作为编织输入 |
| **Equipment** | 工具耐久；某些工具有 Day-Night 加成 |
| **World Exploration** | 节点分布在 Biome 内 |
| **Oath** | 部分里程碑要求"采集 X 个 Y 资源" |
| **Einherjar** | 英灵职业影响采集效率（铁匠=矿，猎人=hide） |
| **Save** | 节点状态（Depleted / Renewable）持久化 |
| **UI/HUD** | 工具槽位、节点进度条、采集状态 |

---


#### 工具耐久警告机制（v1.0 强化）⭐ P2 修复

> 原 v1.0 决策：仅"工具断"模糊反馈。P2 修复后增加**3 阶段警告 + 归因日志**：

**3 阶段警告**：

| 阶段 | 耐久 | 视觉 | UI 反馈 | 工具效果 |
|---|---|---|---|---|
| **健康** | 100%-50% | 工具图标正常 | 无 | 100% 效率 |
| **警告** | 50%-20% | 工具图标黄边 | UI 工具栏黄色感叹号 | 100% 效率（不变）|
| **危险** | 20%-1% | 工具图标红边 + 闪烁 | UI 工具栏红色感叹号 + Toast "耐久 20%！"| 90% 效率（轻惩罚）|
| **断** | 0% | 工具图标灰 | 玩家下次使用时**立即断** + 强制回聚落修理 | 0% 效率 |

**耐久警告视觉**：
- 工具栏（右下 4 槽）= 工具图标 + 边框色
  - 健康：无边框
  - 警告：黄色 1px 边框
  - 危险：红色 1px 边框 + 闪烁（0.5s 间隔）
- 鼠标悬停工具 → 显示耐久百分比

**耐久 20% 警告 Toast**：
- 屏幕右下方（与资源拾取同位置）
- 3 秒
- 红色边框 + "工具 {name} 耐久 20%！请到工坊修理"
- 玩家**不**能关闭

**修理机制**（v1.0）：
- 玩家**只能**在工坊（聚落 Level 1+）修理
- 修理消耗 = 5 铁 + 5 木
- 修理时间 = 0.5 游戏日
- 玩家**不能**在野外修理（v1.0 简化）

**归因日志**（v1.0 强化）：
- 工具断时显示断因："工具 {name} 断因：耐久 0%（砍树 50 次）"
- 玩家**理解**为什么断
- v1.0 决策：归因日志**只**在断时显示，**不**实时计数（避免 UI 过载）

**修复 P2 缺口** ✅ —— 工具耐久从"模糊反馈"升级为"3 阶段警告 + 归因日志"。

**性能影响**：
- 工具栏边框色更新：< 1ms / 帧
- 耐久计算：< 1ms / 帧
- **总计**：< 2ms / 帧（v1.0 帧预算 16ms 内）


## D. Formulas

### F.1 基础时间公式
```csharp
float CalculateGatherTime(float baseTime, float efficiencyMult, float toolSpeedMult) {
  return baseTime / (efficiencyMult * toolSpeedMult);
}
```

### F.2 基础产出公式
```csharp
int CalculateYield(int baseMin, int baseMax, float efficiencyMult, float toolYieldMult) {
  float mult = efficiencyMult * toolYieldMult;
  int minYield = (int)Mathf.Ceil(baseMin * mult);
  int maxYield = (int)Mathf.Floor(baseMax * mult);
  return Random.Range(minYield, maxYield + 1);
}
```

### F.3 Day-Night 效率
```csharp
float GetEfficiencyMult() {
  TimeState state = TimeManager.GetCurrentState();
  if (state == TimeState.Day) return 0.7f;
  if (state == TimeState.Night) return 1.5f;
  // Dawn/Dusk: smoothstep 插值
  float t = TimeManager.GetTransitionProgress();
  return Mathf.Lerp(0.7f, 1.5f, t * t * (3f - 2f * t));
}
```

### F.4 工具耐久消耗
```csharp
bool UseTool(ItemStack toolStack) {
  var tool = DataRegistry.GetItem(toolStack.itemId) as ToolSO;
  toolStack.durability -= 1;  // MVP 简化：每次用 -1
  if (toolStack.durability <= 0) {
    // 工具消失
    return false;  // 信号：工具已坏
  }
  return true;
}
```

### F.5 资源时间窗口判定
```csharp
bool IsResourceAvailable(ResourceSpawn spawn, TimeState current) {
  if (spawn.dayNightRequirement == DayNightRequirement.Any) return true;
  if (spawn.dayNightRequirement == DayNightRequirement.Day && current == TimeState.Day) return true;
  if (spawn.dayNightRequirement == DayNightRequirement.Night && current == TimeState.Night) return true;
  return false;
}
```

### F.6 群系再生判定
```csharp
bool ShouldRegrowBiome(BiomeSO biome, float hoursSinceLastRegrow) {
  if (hoursSinceLastRegrow < 24f) return false;
  return Random.value < 0.3f;  // 30% 概率 / 24h
}
```

### F.7 特殊资源概率加成
```csharp
float GetSpecialResourceChance(ResourceSpawn spawn) {
  float baseChance = spawn.spawnChance;  // e.g., 0.1 (10%)
  if (spawn.dayNightRequirement == DayNightRequirement.Night && IsNight()) {
    baseChance *= 1.5f;  // 夜战加成
  }
  return baseChance;
}
```

---

## E. Edge Cases

| 情况 | 处理 |
|---|---|
| 玩家打断采集动作 | 损失 50% 进度，**不**获得任何资源 |
| 玩家被攻击 | 采集**完全失败**，无资源获得 |
| 工具耐久到 0 | 工具**消失**（从 inventory 移除），采集动作**立即失败** |
| 玩家没有工具尝试 Chop | UI 提示"需要 Axe"；动作**不可用** |
| 背包满时产出 | 资源**落地**（不直接入包），15 真实秒后消失 |
| Night-only 资源白天尝试 | 节点不显示（玩家看不见）；尝试采集无效果 |
| 节点位于庇护所 | 采集不受 Day-Night 影响（玩家保护机制） |
| 群系没有这种资源 | 节点根本不生成 |
| 玩家在水中采木 | 不可能（水上没树） |
| 工具 tier 5 + Day | 仍然受 Day -30% 影响（Day-Night 是全局修正） |
| 资源节点在 boss 战区域 | 玩家进入 boss 战后节点不可交互（boss 优先） |
| 多个玩家同时采同一节点 | MVP 单机，不考虑 |
| 工具坏了没材料修 | 必须用基础工具（tier 1-2）替代 |

---

## F. Dependencies

### 上游（Gathering 依赖谁）

- **Data Config** —— 工具数据（`ItemSO` + `ToolSO`）、节点数据（`BiomeSO.resources[]`）
- **Day-Night** —— 效率修正 + 时间窗口
- **Inventory** —— 工具装备 + 产出物入背包
- **Equipment** —— 工具是装备的一部分
- **World Exploration** —— 节点分布
- **Save** —— 节点状态持久化

### 下游（谁依赖 Gathering）

- **Fate-Thread** —— 资源是编织输入
- **Inventory** —— 资源入包
- **Oath** —— 部分里程碑要求"采集 X 资源"
- **Einherjar** —— 英灵工作 = 自动采集（未来 v1.1）

**Gathering 是"中段"枢纽**——简单但被编织、誓言、英灵等关键系统消费。

---

## G. Tuning Knobs

> 调参字段建议加到 `GameConfigSO`（data-config v1.3 阶段）

| 参数 | 默认值 | 调参影响 |
|---|---|---|
| `gatherDayEfficiencyMult` | 0.7f | 白天效率系数（-30%） |
| `gatherNightEfficiencyMult` | 1.5f | 夜晚效率系数（+50%） |
| `gatherBaseTimeChop` | 3.0f | Chop 基础时间（秒） |
| `gatherBaseTimeMine` | 4.0f | Mine 基础时间 |
| `gatherBaseTimePick` | 1.0f | Pick 基础时间 |
| `gatherBaseTimeHunt` | 5.0f | Hunt 基础时间 |
| `gatherBaseTimeFish` | 4.0f | Fish 基础时间 |
| `toolDurabilityTier1` | 50 | Tier 1 工具耐久 |
| `toolDurabilityTier2` | 100 | Tier 2 |
| `toolDurabilityTier3` | 200 | Tier 3 |
| `toolDurabilityTier4` | 350 | Tier 4 |
| `toolDurabilityTier5` | 500 | Tier 5 |
| `biomeRegrowIntervalHours` | 24 | 群系再生间隔（游戏小时） |
| `biomeRegrowChance` | 0.3f | 每次判定再生概率 |
| `biomeRegrowRatio` | 0.3f | 一次再生补充的节点比例 |
| `nodeRenewableRespawnHours` | 24 | Renewable 节点再生时间 |
| `specialResourceNightBonus` | 1.5f | Night 特殊资源概率加成 |

---

## H. Acceptance Criteria

### AC-1: Day-Night 效率正确
**测试**：
1. Day 砍树 → 记录时间和产出
2. Night 砍同样树 → 记录时间和产出
3. **期望**：Night 时间 = Day × (0.7/1.5) ≈ Day × 0.47（快 2 倍）
4. **期望**：Night 产出 ≈ Day × (1.5/0.7) ≈ Day × 2.14（多 1 倍）

### AC-2: 工具需求
**测试**：
1. 玩家无 Axe → 走到树前按 E
2. **期望**：UI 提示"需要 Axe"，无动作
3. 装备 Wood Axe → 按 E → 砍树
4. **期望**：3 秒完成，产出 1-2 Wood（Day）

### AC-3: 工具耐久
**测试**：
1. 用 Wood Axe（耐久 50）砍树 50 次
2. 第 51 次 → 工具消失，砍树失败
3. **期望**：耐久准确递减

### AC-4: Night-only 资源
**测试**：
1. Day 走到 Moonflower 节点位置
2. **期望**：节点**不显示**（看不见）
3. Night 同一位置 → 节点显示
4. 采集 → 成功

### AC-5: Renewable 节点再生
**测试**：
1. 摘一朵 Wild Berry
2. 等 24 游戏小时（12 真实分钟）
3. **期望**：节点自动重新出现
4. **期望**：可再次采集

### AC-6: Limited 节点永久消失
**测试**：
1. 砍一棵树
2. 同一位置等 24 游戏小时
3. **期望**：树不出现（Limited）
4. 离开群系 → 等 24 游戏小时 → 回到群系
5. **期望**：30% 概率出现新树

### AC-7: 进度条显示正确
**测试**：
1. 开始砍树
2. **期望**：头顶出现进度条，按时间填充
3. 释放交互键 → 进度条停止，**消失**
4. 再次按 E → 进度条重置为 0，重新填充

### AC-8: 性能预算
**测试**：
1. 视野内有 50 个 Renewable 节点
2. **期望**：每个节点 idle 动画 < 0.05ms / 帧
3. 采集动作启动 < 0.1ms

### AC-9: 资源直接入背包
**测试**：
1. 背包有 23 槽
2. 砍树产出 2 Wood
3. **期望**：直接入背包，不落地
4. 继续砍 3 次 → 背包满 24 槽
5. 第 4 次 → 资源**落地**（15 真实秒后消失）

### AC-10: 工具 Day-Night 加成
**测试**：
1. 装备 Tier 5 Axe（有 nightBonus）
2. Night 砍树
3. **期望**：总产出 = base × 1.5 (Day-Night) × 3.0 (Tier 5) = 4.5x
4. Day 同一棵树 = base × 0.7 × 3.0 = 2.1x

---

## 10. 已锁定决策（Locked Decisions）

> 2026-07-27 用户拍板，7 个开放问题全部锁定。已落地为 G 旋钮 + data-config v1.3。

| # | 决策点 | 锁定值 | 落地位置 |
|---|---|---|---|
| 1 | **工具耐久** | **做**（每次 -1，简单线性） | §C.4 Tools + F.4 耐久消耗 |
| 2 | **工具修理** | **MVP 不做**（v1.1 加），坏了直接换 | §C.4 |
| 3 | **节点视觉** | **sprite 集**（每种 2-5 变体） | §C.6 节点视觉 |
| 4 | **资源类型** | **5 种够**（未来可扩展 Cloth/Gem） | §C.2 |
| 5 | **Limited 节点再生** | **保持 30%/24h 群系再生** | §C.7 |
| 6 | **Hunt 难度** | **动物血量 = 3**（平衡好） | §C.3 Hunt |
| 7 | **Tier 5 工具 Day-Night 加成** | **是**（传奇装备特色） | §C.4 工具 + Inventory 联动 |

### 决策之间的协同

- **#1 + #2**：耐久做、修理不做 = "工具坏是真实的，但有规划就能避免"——避免玩家被"修理 UI"打断，**专注核心采集循环**
- **#3 + #5**：sprite 集视觉 + 30%/24h 再生 = "砍光一片林地有视觉反馈"——空地与森林的对比让"取舍得失"可感
- **#4 + #6 + #7**：5 资源 + Hunt 血 3 + Tier 5 加成 = "采集有节奏、有挑战、有奖励"——避免变成无脑点击

### 仍待 playtest 调参（不阻塞 GDD 锁定）

| 待调项 | 候选范围 | 调参位置 |
|---|---|---|
| Day-Night 效率精度 | 0.7/1.5 vs 0.6/1.6 | `GameConfigSO.gatherDayEfficiencyMult` |
| 工具耐久范围 | 50-500 vs 100-1000 | `GameConfigSO.toolDurabilityTier*` |
| 群系再生概率 | 30% vs 20% | `GameConfigSO.biomeRegrowChance` |

→ 这些都是 Prototype 阶段的**数值调参工作**，通过 `GameConfigSO` 直接改即可，不阻塞任何 GDD。

---

## Unity Implementation Notes

### 核心脚本（`Assets/Scripts/Gathering/`）
- `GatheringAction.cs` —— 进度条 + 状态机
- `ToolController.cs` —— 工具装备 + 耐久管理
- `ResourceNode.cs` —— 节点基类（Tree/Rock/Plant/Animal/Fish 各有子类）
- `BiomeSpawner.cs` —— 群系初始节点生成
- `BiomeRegrower.cs` —— Limited 节点再生判定
- `GatherYieldCalculator.cs` —— 公式封装
- `TimeWindowChecker.cs` —— 资源时间窗口判定

### 节点状态机
```csharp
public enum NodeState { Idle, BeingGathered, Depleted, Renewing }
```

### 事件订阅
```csharp
public class GatheringAction : MonoBehaviour {
  public static event Action<string, int> OnResourceGathered;  // itemId, qty
  public static event Action<ItemSO> OnToolBroken;
  public static event Action<string> OnNodeDepleted;  // nodeId
}
```

### TimeManager 联动
- 订阅 `TimeManager.OnTimeStateChanged` 重算效率
- 节点根据时间显示/隐藏（Night-only）
- 工具 Day-Night 加成实时生效

### 性能预算
- 节点 idle 动画：< 0.05ms / 节点 / 帧
- 进度条更新：每帧 0.01ms
- 群系再生判定：每 24 游戏小时 1 次（背景异步）
- 50 节点同屏：< 3ms 总开销

---

## 进度

| 章节 | 状态 |
|---|---|
| A. Overview | ✅ |
| B. Player Fantasy | ✅ |
| C. Detailed Design (10 小节) | ✅ |
| D. Formulas (7 个) | ✅ |
| E. Edge Cases (14 种) | ✅ |
| F. Dependencies | ✅ |
| G. Tuning Knobs (17 字段已落 v1.3) | ✅ |
| H. Acceptance Criteria (10 条) | ✅ |
| **10. Locked Decisions (7 决策)** | ✅ |
| Unity Implementation Notes | ✅ |

**总进度**: 10/10 (100%) 🔒

**🔒 已锁定 v1.0** —— 8 段全填 + 7 开放问题全部锁定 + 17 调参字段落 data-config v1.3。

---

## 变更日志

| 日期 | 版本 | 改动 | 责任人 |
|---|---|---|---|
| 2026-07-27 | v1.0 draft | 初版生成：10 段 + 公式 + Unity 实施 | Mavis |
| 2026-07-27 | **v1.0 LOCKED** | 7 开放问题用户拍板全部锁定；data-config v1.3 同步升级 | Mavis + 用户 |
| 2026-07-27 | 工具耐久警告补充 | P2 修复：§C.4 加 3 阶段警告 + 归因日志（解决"工具为什么突然坏了" P2 缺口）| Mavis + 用户 |
