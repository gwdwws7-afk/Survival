# Settlement — System GDD

> **Status**: 🔒 LOCKED v1.0
> **Author**: Mavis + 用户
> **Last Updated**: 2026-07-27
> **Implements Pillar**: **Foundation**（支持 Living Hearth 落地 + 所有签名系统场景）
> **See Also**: `einherjar.md` §C.6（聚落效率）/ `oath-system.md` §C.2 誓言 2-4（祭坛/英灵殿/锻冶圣坛/驯兽场）/ `death-sendoff.md` §C.6（纪念碑区）/ `ui-hud.md` §C.4（聚落菜单）

---

## A. Overview

**Settlement（聚落）是 Ravensong 的"家"——所有签名系统的成果都汇聚在这里。** 5 签名系统（Day-Night / Fate-Thread / Einherjar / Oath / Death-Send-off）都在聚落里有"物理位置"：长屋住英灵、工坊编织、篝火辉映、祭坛展现誓言、纪念碑区存放送别。聚落是 Ravensong 的"叙事中心"——**玩家在聚落内的时间 ≈ 30%，但这 30% 是情感密度最高的部分**。

Ravensong 的聚落设计哲学是 **"叙事性聚落"（Narrative Settlement）**：**不**做"自由建造"沙盒（避免成为 1 个 base-builder），而做"5 类基础建筑 + 等级 1-2"的**固定蓝图**。v1.0 玩家可以**升级**已有建筑（5 类），但**不**能自由增加新建筑或大幅改变布局。**v1.1 决策**：开放"自由建造"模式，让玩家放置 v1.0 已解锁的建筑。

5 类基础建筑：
- **长屋（Longhouse）**——英灵住处，容量决定 `maxEinherjar`
- **工坊（Forge）**——编织/铁匠工作，升级解锁 Tier 4-5 配方
- **篝火（Hearth）**——Living Hearth 支柱核心，士气 +X%，夜间 VFX 燃起
- **仓库（Storage）**——背包容量扩展
- **神龛（Shrine）**——奥丁之眼 + 衰悼期仪式

**特殊建筑（v1.0 由誓言解锁）**：
- **锻冶圣坛**（誓言 1 完成）——编织效率 +30% 永久
- **英灵殿**（誓言 2 完成）——士气永久满
- **驯兽场**（誓言 3 完成）——动物主动不攻击
- **英灵殿祭坛**（誓言 4 完成）——亡者之誓 5/5 + Wyrd 锚点
- **苍穹祭坛**（誓言 5 完成）——奥丁审判触发点

数据层由**新增**的 `SettlementSO`（data-config.md C.2 类型 12）驱动；本 GDD 专注于**建筑蓝图、升级机制、聚落状态、布局规则、v1.0 限制**。

---

## B. Player Fantasy

### 主幻想
> "我聚落的长屋住了 8 个英灵，篝火在夜晚烧得通红——他们围着火，低声唱着 Norse 挽歌。这就是 Ravensong。"

### 关键体验时刻

- **第一次**到达聚落：从战区返回，篝火图标出现在小地图（罗盘）上
- **第一次**升级建筑：长屋 Level 1 → Level 2，容量从 4 提升到 6，"我们的家更大了"
- **第一次**完成誓言 1：聚落出现"锻冶圣坛"——金色光柱，编织声变得更清脆
- **第一次**送别英灵：纪念碑区出现 Eirik 的小型石碑
- **第一次**聚落达到满级：5 类基础建筑全部 Level 2 + 4 个誓言建筑出现 = "我的 Ravensong 完整了"
- **第一次**衰悼期：篝火色调变冷，吟游诗人自动播放挽歌

---

## C. Detailed Design

### C.1 核心规则（Core Rules）

#### 规则 1：5 类基础建筑 + 0-5 誓言建筑
- 基础建筑：5 类，玩家**必须**先有 Level 1 才能解锁 Level 2
- 誓言建筑：由对应誓言 5/5 完成**自动**出现，玩家**不能**主动建造

#### 规则 2：聚落固定位置（v1.0 决策）
- 玩家**不能**移动聚落
- 聚落初始位置：在世界地图中央
- 玩家**不能**增加新基础建筑类型
- v1.1 决策：开放"自由建造"模式

#### 规则 3：建筑等级 1-2（v1.0）/ 1-3（v1.1）
- Level 1：基础功能
- Level 2：解锁额外功能（如长屋容量 4→6，工坊解锁 Tier 4 配方）
- Level 3：v1.1 决策（v1.0 不实现）

#### 规则 4：建筑升级消耗资源
- 5 类基础建筑升级公式：资源消耗 = `baseCost * level`
- 例：长屋 Level 1 → Level 2 = 50 木 + 20 铁
- 工坊 Level 2 → Level 3（v1.1）= 100 木 + 50 铁 + 5 god-ember

#### 规则 5：聚落状态 5 维度
- **人口**（英灵数 / 容量）
- **士气**（0-100%）
- **产出**（所有 Profession × 效率）
- **资源**（4 种：铁/食物/木材/草）
- **建筑**（5 类 Level 状态）

#### 规则 6：聚落夜间 VFX 燃起
- 篝火：火 + 烟
- 长屋：窗户发光
- 工坊：Forge 火花
- 仓库：门口火把
- 神龛：奥丁之眼（夜视更亮）

#### 规则 7：聚落庇护所效应
- 聚落内**不**触发战斗
- 玩家在聚落内**不**受敌对生物攻击
- v1.1 决策：聚落可能被外敌攻破（v1.0 不会）

#### 规则 8：衰悼期 VFX
- 24 小时内聚落色调变冷
- 篝火火焰变蓝
- 长屋窗户变暗
- 吟游诗人自动播放挽歌

---

### C.2 5 类基础建筑 ⭐

#### 建筑 1：长屋（Longhouse）—— 英灵住处

| 等级 | 容量 | 升级消耗 | 视觉差异 |
|---|---|---|---|
| Level 1 | 4 英灵 | - | 1 层小木屋 |
| Level 2 | 8 英灵（=全上限） | 50 木 + 20 铁 | 2 层大木屋 + 烟囱 |
| Level 3 (v1.1) | 12 英灵 | 100 木 + 50 铁 + 5 god-ember | 3 层大木屋 + 阳台 |

**机制**：
- 容量 = 招募上限（`maxEinherjarInSettlement` 实际值）
- 玩家**不能**招募超出容量的英灵
- 升级后 24 小时内英灵"搬家"（VFX：搬家具）
- v1.0 锁定：长屋 Level 2 = 8 英灵 = `GameConfigSO.maxEinherjarInSettlement` 锁定值

#### 建筑 2：工坊（Forge）—— 编织/铁匠工作

| 等级 | 功能 | 升级消耗 | 视觉差异 |
|---|---|---|---|
| Level 1 | Tier 1-3 配方 + 1 个 Forge 槽 | - | 单炉 |
| Level 2 | Tier 1-4 配方 + 2 个 Forge 槽 | 50 木 + 30 铁 | 双炉 + 工具架 |
| Level 3 (v1.1) | Tier 1-5 配方 + 3 个 Forge 槽 | 100 木 + 50 铁 + 10 god-ember | 三炉 + 神秘符文 |

**机制**：
- 配方 Tier 受工坊 Level 限制
- Forge 槽 = 同时编织数（多英灵可同时工作）
- 升级后 24 小时内工坊"扩建"（VFX：搭建新墙）

#### 建筑 3：篝火（Hearth）—— Living Hearth 支柱核心 ⭐

| 等级 | 士气加成 | 升级消耗 | 视觉差异 |
|---|---|---|---|
| Level 1 | +10% 士气 | - | 单堆火 |
| Level 2 | +25% 士气 | 30 木 + 10 铁 | 围石火堆 + 烤架 |
| Level 3 (v1.1) | +50% 士气 | 80 木 + 30 铁 + 5 god-ember | 大篝火 + 围坐区 |

**机制**：
- **Living Hearth 支柱的"场景化身"**——篝火燃烧状态 = 聚落生命状态
- 篝火熄灭（v1.0 决策：永不熄灭）= 聚落死（v1.1 决策）
- 衰悼期 24h：篝火变蓝（VFX）
- 夜间：篝火最亮（VFX + SFX）

#### 建筑 4：仓库（Storage）—— 背包容量

| 等级 | 背包容量 | 升级消耗 | 视觉差异 |
|---|---|---|---|
| Level 1 | 24 槽（基础） | - | 小木屋 |
| Level 2 | 48 槽 | 40 木 + 15 铁 | 中木屋 + 货架 |
| Level 3 (v1.1) | 72 槽 | 80 木 + 30 铁 | 大仓库 + 分类系统 |

**机制**：
- 容量 = `inventoryMaxSlots` 实际值
- 与 `inventory.md` 协同
- v1.0 锁定：仓库 Level 1 = 24 槽（默认），Level 2 = 48 槽

#### 建筑 5：神龛（Shrine）—— 奥丁之眼 + 衰悼期仪式

| 等级 | 功能 | 升级消耗 | 视觉差异 |
|---|---|---|---|
| Level 1 | 奥丁之眼扫描 + 衰悼仪式 | - | 小石龛 |
| Level 2 | 奥丁之眼扫描 + 衰悼仪式 + 神龛祭坛（god-ember 加速） | 30 木 + 20 铁 + 3 god-ember | 中型石龛 + 卢恩符文 |
| Level 3 (v1.1) | 全部 + 天空祈祷（v1.1 决策） | 60 木 + 30 铁 + 10 god-ember | 大型神龛 + 奥丁头像 |

**机制**：
- **奥丁之眼扫描**：`GameConfigSO.odinEyeScanChance` 在神龛旁生效
- **衰悼仪式**：玩家可主动在神龛前"祈福"，加速衰悼期从 24h 减到 12h（消耗 20 god-ember）
- **god-ember 加速**：神龛 Level 2 周围 1 格内 god-ember 被动获取 × 1.5

---

### C.3 誓言建筑（v1.0 由誓言解锁）⭐

> 5 个誓言建筑由对应誓言 5/5 完成**自动**出现。**玩家不能主动建造或拆除**。

#### 建筑 6：锻冶圣坛（誓言 1 完成）
- **位置**：工坊旁
- **效果**：编织效率 +30% 永久
- **视觉**：金色卢恩符文 + 神秘 Forge
- **不可拆**

#### 建筑 7：英灵殿（誓言 2 完成）
- **位置**：聚落东侧
- **效果**：士气永久满（=100%）
- **视觉**：石质大厅 + 8 个英灵雕像
- **不可拆**

#### 建筑 8：驯兽场（誓言 3 完成）
- **位置**：聚落外围
- **效果**：动物主动不攻击
- **视觉**：围栏 + 喂食槽
- **不可拆**

#### 建筑 9：英灵殿祭坛（誓言 4 完成 / 亡者之誓）
- **位置**：纪念碑区中心
- **效果**：Wyrd 锚点激活（聚落效率 +10% + 阻止强留腐化）
- **视觉**：金色祭坛 + 奥丁之眼
- **不可拆**

#### 建筑 10：苍穹祭坛（誓言 5 完成 / 终局）
- **位置**：聚落北侧高处
- **效果**：奥丁审判触发点
- **视觉**：奥丁雕像 + 双翼
- **不可拆**（触发奥丁审判后 v1.1 决策：消失）

---

### C.4 聚落状态（Settlement State）⭐

#### 5 维度状态

| 维度 | 计算公式 | UI 显示 |
|---|---|---|
| **人口** | `einherjars.Count / maxCapacity` | "6/8" |
| **士气** | `base * hearthBonus * mourningPenalty` | "85%" |
| **产出** | `sum(einherjar.production)` | 数字（每小时） |
| **资源** | 4 种资源 | 图标 + 数字 |
| **建筑** | 5 类 Level | 卡片（Lv.1/2/3） |

#### 状态条位置（参考 ui-hud.md §C.7）
- 顶部：人口 + 士气
- 底部：资源（铁/食物/木材/草）
- 聚落菜单内：详细

#### 状态计算示例
```csharp
float CalculateMorale(Settlement s) {
  float base = 1.0f;
  base *= 1.0f + s.hearth.level * 0.15f;  // Level 1=+15%, L2=+30%, L3=+50%
  if (s.mourningHoursRemaining > 0) {
    base *= 0.8f;  // 衰悼 -20%
  }
  return Mathf.Clamp(base, 0f, 1f);
}
```

---

### C.5 升级机制

#### 升级流程
```
玩家打开聚落菜单（按 C）
    ↓
点击建筑卡片（如长屋）
    ↓
建筑详情面板显示：
- 当前 Level + 效果
- 下一 Level 效果
- 升级消耗（资源 + god-ember）
- [升级] 按钮
    ↓
玩家确认 + 资源足够
    ↓
[24 小时升级 VFX]
- 长屋：搬家具 + 加层
- 工坊：搭建新墙
- 篝火：堆大柴
- 仓库：加货架
- 神龛：刻新符文
    ↓
新 Level 生效
```

#### 升级消耗（v1.0）
- 长屋 L1→L2：50 木 + 20 铁
- 长屋 L2→L3 (v1.1)：100 木 + 50 铁 + 5 god-ember
- 工坊 L1→L2：50 木 + 30 铁
- 工坊 L2→L3 (v1.1)：100 木 + 50 铁 + 10 god-ember
- 篝火 L1→L2：30 木 + 10 铁
- 篝火 L2→L3 (v1.1)：80 木 + 30 铁 + 5 god-ember
- 仓库 L1→L2：40 木 + 15 铁
- 仓库 L2→L3 (v1.1)：80 木 + 30 铁
- 神龛 L1→L2：30 木 + 20 铁 + 3 god-ember
- 神龛 L2→L3 (v1.1)：60 木 + 30 铁 + 10 god-ember

#### 升级失败
- 资源不足 → 按钮**灰**
- 已在升级中 → 按钮**灰**（24h 冷却）
- 资源足够 + 等级已满（v1.0 = L2 已封顶）→ 按钮**不显示**

---

### C.6 布局规则（v1.0 锁定）

#### 初始布局
```
        [苍穹祭坛 v1.0 后]
              ↑
[驯兽场 v1.0 后]  [英灵殿祭坛 v1.0 后]
              ↑
[长屋]——[篝火]——[神龛]
              ↑
[工坊]——[仓库]
              ↓
        [英灵殿 v1.0 后]
              ↑
        [锻冶圣坛 v1.0 后]
```

#### 位置规则
- 长屋：聚落北侧
- 篝火：聚落**中心**（永远）
- 工坊：聚落南侧
- 仓库：聚落西南
- 神龛：聚落东南
- 誓言建筑：固定位置（不可改）

#### 装饰（v1.0 简化）
- v1.0：**不**做"自由装饰"系统
- v1.1 决策：开放"装饰品"（花盆/地毯/壁画）

---

### C.7 衰悼期 VFX ⭐

#### 触发
- 任何英灵被送走（送英灵殿/战斗葬礼/简单葬礼/让其安息）
- 24 小时（`GameConfigSO.mourningDurationHours`）

#### 视觉
- **篝火**：火焰变蓝（持续 24h）
- **长屋窗户**：变暗
- **聚落色调**：饱和度 -20%
- **吟游诗人**：自动播放挽歌（覆盖 workLine）
- **粒子**：雨（轻微）

#### 恢复
- 24h 后自动结束
- 篝火恢复红色
- 窗户恢复
- 吟游诗人恢复 workLine

#### 衰悼期内升级
- 升级**不**禁用
- 升级 VFX 仍触发（但聚落色调仍冷）

---

### C.8 资源生产

#### 资源类型（4 种）
- **铁**（Iron）：工坊生产（铁匠）
- **食物**（Food）：仓库接收（农夫/猎人）
- **木材**（Wood）：仓库接收（玩家砍树）
- **草**（Herbs）：仓库接收（农夫）

#### 生产公式
```csharp
float CalculateResourceProduction(Einherjar e, ResourceType type) {
  if (e.profession != GetProducerProfession(type)) return 0;
  float base = e.production;  // 1-3 单位/小时
  base *= e.workEfficiency;
  base *= e.relationship switch { Friendly => 1.2f, Neutral => 1.0f, Hostile => 0.8f, _ => 1.0f };
  base *= TimeManager.IsNight() ? 1.5f : 0.5f;  // 夜晚 +50%
  return base;
}
```

#### 资源消耗
- 升级建筑
- 编织（输入物品消耗）
- 招募（v1.0 不消耗）

#### 资源上限
- v1.0：**无上限**（仓库只影响背包槽，**不**限制聚落资源池）
- v1.1 决策：聚落资源池上限 = 1000/类型

---

### C.9 与其他系统的交互

| 系统 | 怎么用 Settlement |
|---|---|
| **Einherjar** | 聚落 = 英灵的家；建筑升级影响英灵效率；长屋容量 = 招募上限 |
| **Oath** | 5 誓言建筑由誓言 5/5 触发 |
| **Death-Send-off** | 纪念碑区在聚落中央；送别 VFX 在聚落内 |
| **Day-Night** | 夜间 VFX（篝火/窗户/Forge 火花）；庇护所效应 |
| **Fate-Thread** | 工坊 Level 决定可用 Tier；玩家在工坊编织 |
| **UI/HUD** | 聚落菜单（按 C）；状态条（HUD）|
| **Inventory** | 仓库 Level 决定背包容量 |
| **Save** | 5 建筑 Level + 誓言建筑状态 + 资源池 |
| **Quest-Event** | 任务可触发"聚落事件"（瘟疫/外敌 v1.1）|

---

## D. Formulas

### D.1 升级消耗
```csharp
ResourceCost CalculateUpgradeCost(BuildingType type, int currentLevel) {
  return new ResourceCost {
    wood = baseWood[type] * currentLevel,
    iron = baseIron[type] * currentLevel,
    godEmber = baseGodEmber[type] * (currentLevel - 1),  // L2 = 0, L3 = 5/10
  };
}
```

### D.2 士气计算
```csharp
float CalculateMorale(Settlement s) {
  float base = 1.0f;
  base += s.hearth.level * 0.15f;  // L1=+15%, L2=+30%, L3=+50%
  if (s.mourningHoursRemaining > 0) base *= 0.8f;  // 衰悼
  return Mathf.Clamp(base, 0f, 1f);
}
```

### D.3 产出计算
```csharp
float CalculateSettlementProduction(Settlement s) {
  float total = 0;
  foreach (var e in s.einherjars) {
    total += e.CalculateProduction();
  }
  total *= s.shrine.level * 0.1f + 1f;  // 神龛 L1=+10%, L2=+20%, L3=+30%
  return total;
}
```

### D.4 升级 VFX 时长
```csharp
float UpgradeVfxDurationHours = 24f;  // 玩家在升级完成前看不见新 Level
```

### D.5 资源生产（按小时）
```csharp
float GetHourlyProduction(Settlement s, ResourceType type) {
  return s.einherjars
    .Where(e => e.profession == GetProducerProfession(type))
    .Sum(e => CalculateResourceProduction(e, type));
}
```

### D.6 篝火熄灭（v1.1 决策）
```csharp
// v1.0: 篝火永不熄灭
// v1.1: 篝火可能在衰悼 + 0 士气时熄灭
bool ShouldHearthExtinguish(Settlement s) {
  return s.morale <= 0 && s.mourningHoursRemaining > 24;
}
```

---

## E. Edge Cases

| 情况 | 处理 |
|---|---|
| 资源不足升级 | 按钮**灰** + 提示"缺 X 资源" |
| 升级进行中（24h 内）再次升级 | 按钮**灰** + 提示"升级进行中" |
| 长屋 Level 1 已满 4 英灵，升级中 | 玩家**不**能招募第 5 个，等升级完成 |
| 篝火熄灭（v1.1 决策） | 聚落**暂停**所有功能；玩家需"重新点燃" |
| 誓言 5/5 完成后聚落无神龛 | 触发"神龛自动建造"事件 |
| 多个誓言 5/5 同时完成 | 多个誓言建筑**依次**出现（间隔 1 天）|
| 玩家退出游戏时升级中 | 升级进度**保留**（持久化）|
| 聚落被外敌攻破（v1.1 决策） | 部分英灵**死**（取决于 v1.1 设计）|
| 衰悼期内触发衰悼 | 重置 24h 倒计时 |
| 衰悼期内升级建筑 | 升级**正常**进行，VFX 不变 |
| 玩家 v1.0 死 | 聚落**不**被攻击；英灵**不**死 |
| 5 基础建筑全 Level 2 + 5 誓言建筑 | "完整聚落"成就 + Ravensong 标志 |
| 资源 < 0（升级失败） | 资源**不**变（不会扣到负数）|
| 玩家在聚落外死亡 | 聚落**不**被攻击（v1.0 永远安全）|

---

## F. Dependencies

### 上游（这个系统依赖谁）
- **Einherjar** —— 英灵是聚落核心；建筑升级影响英灵效率
- **Day-Night** —— 夜间 VFX + 衰悼期
- **Inventory** —— 仓库决定背包容量
- **Fate-Thread** —— 工坊 Level 决定可用 Tier
- **Data Config** —— `SettlementSO` 是新类型 12

### 下游（谁依赖这个系统）
- **Oath** —— 5 誓言建筑由誓言触发
- **Death-Send-off** —— 纪念碑区在聚落中央
- **UI/HUD** —— 聚落菜单 + 状态条
- **Save** —— 5 建筑 Level + 资源池 + 誓言建筑状态

---

## G. Tuning Knobs（12 字段）

| 旋钮 | 默认值 | 范围 | 决策编号 | 影响 |
|---|---|---|---|---|
| `longhouseCapacityLevel1` | 4 | 2-6 | #1 | 长屋 L1 容量 |
| `longhouseCapacityLevel2` | 8 | 6-10 | #1 | 长屋 L2 容量（=v1.0 满）|
| `longhouseCapacityLevel3` | 12 | 10-16 | #1 | 长屋 L3 容量 (v1.1) |
| `forgeMaxTierLevel1` | 3 | 1-3 | #2 | 工坊 L1 最大 Tier |
| `forgeMaxTierLevel2` | 4 | 3-4 | #2 | 工坊 L2 最大 Tier |
| `hearthMoraleBonusLevel1` | 0.15f | 0-0.3 | #3 | 篝火 L1 士气加成 |
| `hearthMoraleBonusLevel2` | 0.30f | 0-0.5 | #3 | 篝火 L2 士气加成 |
| `storageSlotsLevel1` | 24 | 12-48 | #4 | 仓库 L1 容量 |
| `storageSlotsLevel2` | 48 | 24-72 | #4 | 仓库 L2 容量 |
| `shrineGodEmberMultiplierLevel2` | 1.5f | 1-2 | #5 | 神龛 L2 周围 god-ember 加速 |
| `shrineMourningAccelerateCost` | 20 | 0-50 | #5 | 衰悼期加速消耗 god-ember |
| `buildingUpgradeVfxHours` | 24f | 0-48 | #6 | 升级 VFX 时长 |

---

## H. Acceptance Criteria

### AC-1: 5 类基础建筑初始
- **条件**：玩家第一次到达聚落
- **结果**：5 类基础建筑全部 Level 1 出现

### AC-2: 升级流程
- **条件**：玩家在聚落菜单点击建筑 + 资源足够
- **结果**：24h 升级 VFX + 建筑 Level 提升

### AC-3: 长屋容量变化
- **条件**：长屋 L1 → L2
- **结果**：`maxEinherjarInSettlement` 从 4 提升到 8

### AC-4: 工坊 Tier 解锁
- **条件**：工坊 L1 → L2
- **结果**：Tier 4 配方在编织 UI 中**可用**

### AC-5: 篝火士气加成
- **条件**：篝火 L1
- **结果**：聚落士气 +15%（`morale = 1.0 + 0.15`）

### AC-6: 仓库容量
- **条件**：仓库 L1 → L2
- **结果**：`inventoryMaxSlots` 从 24 提升到 48

### AC-7: 神龛衰悼加速
- **条件**：衰悼期 + 玩家在神龛前祈福
- **结果**：消耗 20 god-ember + 衰悼期倒计时从 24h 减半到 12h

### AC-8: 誓言建筑自动出现
- **条件**：誓言 1-5 中任一 5/5
- **结果**：对应誓言建筑**立即**出现 + VFX

### AC-9: 衰悼期 VFX
- **条件**：英灵被送走
- **结果**：篝火变蓝 + 聚落色调变冷 + 24h 后恢复

### AC-10: 资源生产按小时
- **条件**：农夫在聚落内
- **结果**：食物 +X/小时（按 profession + efficiency 计算）

### AC-11: 升级失败
- **条件**：资源不足
- **结果**：升级按钮**灰** + 提示"缺资源"

### AC-12: 聚落持久化
- **条件**：退出 + 重新进入游戏
- **结果**：5 建筑 Level + 资源池 + 誓言建筑状态**全部**保留

---

## 10. 已锁定决策（Locked Decisions）

> 2026-07-27 用户拍板，12 个开放问题全部锁定。已落地为 §G 旋钮 + data-config v1.9。

| # | 决策点 | 锁定值 | 落地位置 |
|---|---|---|---|
| 1 | **长屋 L1 容量** | **4**（v1.0 上半段） | §C.2 建筑 1 + §G |
| 2 | **长屋 L2 容量** | **8**（=v1.0 上限） | §C.2 建筑 1 + §G |
| 3 | **工坊 L1 最大 Tier** | **3**（v1.0 早期） | §C.2 建筑 2 + §G |
| 4 | **工坊 L2 最大 Tier** | **4**（v1.0 后期） | §C.2 建筑 2 + §G |
| 5 | **篝火 L1 士气加成** | **+15%**（明显但不破坏） | §C.2 建筑 3 + §G |
| 6 | **篝火 L2 士气加成** | **+30%**（满士气附近的明显值） | §C.2 建筑 3 + §G |
| 7 | **仓库 L1 容量** | **24**（基础） | §C.2 建筑 4 + §G |
| 8 | **仓库 L2 容量** | **48**（2 倍扩展） | §C.2 建筑 4 + §G |
| 9 | **神龛 L2 god-ember 加速** | **×1.5**（明显加速） | §C.2 建筑 5 + §G |
| 10 | **衰悼期加速消耗** | **20 god-ember**（有成本但不高） | §C.7 + §G |
| 11 | **升级 VFX 时长** | **24 小时**（足够"建造"感觉） | §C.5 + §G |
| 12 | **誓言建筑位置** | **固定**（v1.0 不开放自定义） | §C.3 + §C.6 |

### 决策之间的协同

- **#1 + #2 + #5 + #6**：长屋 4→8 + 篝火 15%→30% = **"前 4 英灵 + 半满士气 → 8 英灵 + 满士气"**——Living Hearth 成长曲线清晰
- **#3 + #4 + #7 + #8**：工坊 Tier 3→4 + 仓库 24→48 = **"早期 Tier 3 配方 + 24 槽够用 → 后期 Tier 4 配方 + 48 槽 + Tier 5 需誓言"**——Fate-Thread 成长曲线
- **#9 + #10 + #11**：神龛 1.5× + 20 god-ember + 24h VFX = **"聚落有加速器但有成本"**——避免"一键加速"破坏节奏
- **#12**：誓言建筑固定 = **MVP 范围控制**——v1.0 不做"自由布局"

### 仍待 playtest 调参（不阻塞 GDD 锁定）

| 待调项 | 候选范围 | 调参位置 |
|---|---|---|
| 5 基础建筑升级顺序 | 长屋 vs 工坊先行 | playtest 数据 |
| 衰悼期 VFX 强度 | 蓝色 vs 灰黑 vs 雪 | VFX 设计 |
| 篝火粒子数 | 50 vs 100 vs 200 | 性能调优 |
| 资源生产效率 | 当前 vs ×1.5 | `EinherjarSO.production` |
| 誓言建筑出现顺序 | 同时 vs 间隔 1 天 | 触发逻辑 |

→ 这些都是 Prototype 阶段的**视觉/数值调参工作**，通过 `GameConfigSO` / VFX 资产直接改即可，不阻塞任何 GDD。

---

> 12 个开放问题待用户拍板。

1. **长屋 L1 容量**
   - 我的推荐：**4**（v1.0 上半段）
2. **长屋 L2 容量**（=v1.0 上限）
   - 我的推荐：**8**（= `maxEinherjarInSettlement`）
3. **工坊 L1 最大 Tier**
   - 我的推荐：**3**（v1.0 早期）
4. **工坊 L2 最大 Tier**
   - 我的推荐：**4**（v1.0 后期）
5. **篝火 L1 士气加成**
   - 我的推荐：**+15%**（明显但不破坏）
6. **篝火 L2 士气加成**
   - 我的推荐：**+30%**（满士气附近的明显值）
7. **仓库 L1 容量**
   - 我的推荐：**24**（基础）
8. **仓库 L2 容量**
   - 我的推荐：**48**（2 倍扩展）
9. **神龛 L2 周围 god-ember 加速**
   - 我的推荐：**×1.5**（明显加速）
10. **衰悼期加速消耗 god-ember**
    - 我的推荐：**20**（有成本但不高）
11. **升级 VFX 时长**
    - 我的推荐：**24 小时**（足够"建造"感觉）
12. **誓言建筑位置**
    - 我的推荐：**固定位置**（v1.0 不开放自定义）

---

## Unity Implementation Notes

### 核心脚本（`Assets/Scripts/Settlement/`）
- `SettlementManager.cs` —— 聚落状态 + 5 建筑管理
- `Building.cs` —— 单个建筑（type + level + upgrade progress）
- `SettlementUI.cs` —— 聚落菜单（按 C）
- `UpgradeVfx.cs` —— 升级 VFX
- `HearthVfx.cs` —— 篝火 VFX（夜间 + 衰悼）
- `MourningController.cs` —— 衰悼期 VFX
- `OathBuildingTrigger.cs` —— 誓言 5/5 触发誓言建筑
- `ShrineRitual.cs` —— 神龛衰悼加速

### 数据结构
```csharp
public class Settlement {
  public Dictionary<BuildingType, Building> buildings;  // 5 类基础建筑
  public List<OathBuilding> oathBuildings;              // 0-5 誓言建筑
  public List<Memorial> memorials;                      // 纪念碑（与 death-sendoff 协同）
  public float mourningHoursRemaining;                  // 衰悼期倒计时
  public ResourcePool resources;                        // 4 种资源池
}

public class Building {
  public BuildingType type;  // Longhouse / Forge / Hearth / Storage / Shrine
  public int level;          // 1, 2 (3 in v1.1)
  public float upgradeProgressHours;  // 0-24h
  public ResourceCost upgradeCost;
  public Vector2Int position;  // 聚落内位置
}
```

### 状态机
```csharp
public enum BuildingLevel {
  Level1,    // 基础功能
  Level2,    // 升级（v1.0 上限）
  Level3,    // v1.1
  Upgrading, // 升级中（24h）
}
```

### 事件订阅
```csharp
public class SettlementManager : MonoBehaviour {
  public static event Action<Building> OnBuildingUpgraded;
  public static event Action<OathBuilding> OnOathBuildingSpawned;
  public static event Action<float> OnMourningStarted;
  public static event Action OnMourningEnded;
  public static event Action<ResourceType, float> OnResourceProduced;
}
```

### 性能预算
- 聚落菜单：< 8ms / 帧
- 篝火 VFX：< 4ms / 帧（粒子数 < 100）
- 升级 VFX：< 8ms / 帧（24h 内一次性）
- 誓言建筑出现 VFX：< 16ms / 帧（一次性）

### 资产制作
- **5 类基础建筑**：每类 L1 + L2 = 10 模型 + 10 纹理
- **篝火 VFX**：3 套（Day/Dusk/Night）
- **衰悼期 VFX**：1 套（篝火变蓝）
- **誓言建筑**：5 个（金色光柱 + 奥丁之眼）
- **聚落内 8 个英灵雕像**（v1.0 简化：1 套 + 8 名字）

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
| 2026-07-27 | v1.0 LOCKED | 12 开放问题用户拍板全部锁定；data-config v1.9 同步升级 + 新增 `SettlementSO`（类型 12） | Mavis + 用户 |
