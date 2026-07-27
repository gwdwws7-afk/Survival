# Inventory & Equipment — System GDD

> **Status**: 🔒 LOCKED v1.0
> **Author**: Mavis + 用户
> **Last Updated**: 2026-07-27
> **Implements Pillar**: Woven Power（编织之力）+ Waxing Moon（反节奏的爽感）

---

## A. Overview

**Inventory & Equipment 是 Ravensong 的"物品中枢"——所有"非能力、非时间"的东西都从这里管。** 玩家采集的资源、敌人掉落的战利品、编织的产物、招募英灵带回的礼物，**全部**经过这里。装备则把物品转化为"被玩家使用的能力"。

**关键创新**：Ravensong 的装备有 **Day-Night 专属加成**——一把"月光矛"白天是普通武器，夜晚伤害 +20%；一面"日耀盾"反过来。这让**换装成为日夜节奏的一部分**，把 Inventory 与 Day-Night 这两个 L2 系统**真正咬合**。

数据层全部由 `ItemSO`（data-config.md C.2 类型 2）驱动；本 GDD 专注于**运行时行为**（怎么拾取、怎么堆叠、怎么装备、怎么用）。

---

## B. Player Fantasy

### 主幻想
> "月光矛在手，夜晚谁与争锋——日耀盾在身，白天无惧灼烧。"

### 关键体验时刻
- **战利品掉落时**：敌人死后，物品直接飞入背包（带视觉特效），玩家感到**收获的爽感**
- **装备月光矛**（首次）：角色攻击动作 +cyan 拖尾，UI 提示"夜间伤害 +20%"
- **日出时**穿着日耀盾：角色周围有金色光圈保护，**视觉化"白天有 buff"**
- **背包满了**：决策点——是扔掉普通资源还是丢掉用过的装备？**这是 Ravensong 的"取舍玩法"**
- **第一次**看到月光矛的 Day-Night 加成描述："白天：普通；夜晚：+20% 伤害"——**这是 Ravensong 的"发现玩法"**

---

## C. Detailed Design

### C.1 核心规则（Core Rules）

#### 规则 1：所有物品必须来自 `ItemSO`（id 引用）
- 不允许在代码中硬编码物品属性
- 装备/卸下/拾取/丢弃的逻辑**只引用 id**，不引用 SO 实例
- 运行时通过 `DataRegistry.GetItem(id)` 解析

#### 规则 2：物品分 5 大类
- **Resource**：可堆叠的原材料（木材、矿石、兽皮等）
- **Equipment**：可装备的武器/防具/配件
- **Consumable**：可消耗的消耗品（药水、食物）
- **Quest**：剧情物品，不入"普通"背包（放特殊栏）
- **Token**：特殊标记（如"英灵殿祝福"——永久 buff 标记）

#### 规则 3：堆叠规则
| 类别 | 可堆叠 | 最大堆叠 | 备注 |
|---|---|---|---|
| Resource | ✓ | 99 | 标准 |
| Equipment | ✗ | 1 | 永远独立 |
| Consumable | ✓ | 20 | 数量限制（防止滥用） |
| Quest | ✗ | 1 | 永远独立 |
| Token | ✗ | 1 | 永久标记 |

#### 规则 4：装备槽位固定为 4 个
- **Main Hand**（主手武器）
- **Off Hand**（副手：盾 / 法器 / 双手武器时禁用）
- **Armor**（身体防具）
- **Accessory**（饰品：项链 / 戒指 / 符文）

#### 规则 5：装备有 Day-Night 专属加成（⭐ Ravensong 特色）
- 见 §C.5

#### 规则 6：Inventory 满时不允许拾取
- 满 → 物品落到地面生成物理对象
- 地面物品 30 游戏分钟后自动消失（避免堆场）

#### 规则 7：装备改变**属性**，不改变**模型**（MVP）
- MVP 阶段：装备只改 StatBlock，不换 sprite
- Vertical Slice 阶段：可选地加视觉变化（v1.1 决策）

### C.2 Item Categories 详解

#### Resource（资源）
- 5 个子类：Wood / Ore / Hide / Plant / Cloth
- 编织时作为输入
- 采集产出
- 例：`item_ash_branch`, `item_iron_ore`, `item_wolf_hide`

#### Equipment（装备）
- 4 个子类对应 4 个槽位
- 子类内部有 `tier`（1-5），影响基础属性
- 例：`item_silver_spear` (Main Hand, tier 3), `item_sunforged_shield` (Off Hand, tier 4, dayBonus)

#### Consumable（消耗品）
- 4 个子类：HealingPotion / StaminaPotion / BuffFood / UtilityScroll
- 使用后从 inventory 移除 1 个
- 例：`item_minor_healing_potion`, `item_moonlight_elixir` (夜晚 buff)

#### Quest（剧情物品）
- 不可丢弃、不可出售、不可堆叠
- 单独 Quest Log 显示进度
- 例：`item_odin_eye_token`, `item_ragnar_horn`

#### Token（标记）
- 永久存在的标记（直到使用或剧情结束）
- 例：`token_valhalla_blessing_eirik`（英灵 Eirik 送你后获得的永久标记）
- 用于 buff 系统的"事实标记"

### C.3 Inventory 模型

**模型：固定槽位（24 槽）+ 物品堆叠**——**简单但有取舍**。

| 决策 | 选择 | 理由 |
|---|---|---|
| 槽位数 | **24 槽** | 16 太少、32 太多；24 够装"一次 raid"的所有物品 |
| 槽位类型 | 同质（不分类） | 简单；UI 用 Tab 切换不同类别显示 |
| 重量系统 | **无** | 女武神能飞，物理重量不适用；24 槽本身已限制 |
| 跨槽堆叠 | **自动** | 多个堆叠自动合并；满了不让拾取 |

**为什么不选 Tetris 网格**：
- 2D 顶视游戏 + 24 槽已足够
- 网格复杂度 × 5，但收益不显著
- UI 成本高（拖拽对齐）

**为什么不选重量系统**：
- 不符合 Ravensong 神幻主题（女武神携带重量不直观）
- 24 槽已经强制取舍
- 重量 × Day-Night 复杂度过高

### C.4 Equipment Slots

| 槽位 | 主手 | 副手 | 护甲 | 饰品 |
|---|---|---|---|---|
| 主要内容 | 武器 | 盾 / 法器 / 双手禁用 | 身体防具 | 项链 / 戒指 |
| 影响属性 | damage, attackSpeed | defense, blockChance | defense, resistances | bonuses, special abilities |
| 双手武器规则 | ✓ | ✗ | ✓ | ✓ |
| 例 | `item_silver_spear` | `item_iron_shield` | `item_bronze_chest` | `item_rune_pendant` |

**双手武器**：
- 双持（如月光大剑） = Main Hand 装备 + Off Hand 强制为空
- 显示"双手"标记
- Off Hand 装备时不能装备双手武器

**槽位解锁**（EA 阶段）：
- MVP：4 槽全开
- EA：可加 5-6 槽（如 Back / Trinket / Belt）

### C.5 Day-Night Item Bonuses（⭐ Ravensong 特色）

**这是 Ravensong Inventory 区别于其他生存游戏的机制级创新。**

```csharp
[Serializable]
public class DayNightItemBonus {
  public StatBlock stats;          // 装备时附加的属性
  [TextArea] public string description;  // UI 提示
}

// In ItemSO:
public DayNightItemBonus? dayBonus;
public DayNightItemBonus? nightBonus;
```

#### 规则
- 装备**总是**提供基础 StatBlock
- 如果装备有 `dayBonus` 且当前是 Day：基础 + dayBonus
- 如果装备有 `nightBonus` 且当前是 Night：基础 + nightBonus
- 转换期间（Dawn/Dusk）：用 smoothstep 插值，避免突变

#### 例子（典型 Ravensong 物品）
| 物品 | 基础 | Day Bonus | Night Bonus | 效果 |
|---|---|---|---|---|
| `item_silver_spear` | damage 10 | — | damage +4 (夜晚额外) | 夜战神器 |
| `item_sunforged_shield` | defense 5 | defense +3 (白天额外) | — | 白天防御 |
| `item_moonlit_charm` | — | — | speed +10% (夜晚额外) | 夜行加速 |
| `item_odin_eye_pendant` | — | vision +20% (白天额外) | — | 白天视野补偿 |

#### 平衡
- 大部分装备**只有 1 个** Day-Night 加成（避免过强）
- 少数传说装备可以**两个都有**（如"永昼之剑"白天+夜晚都生效）
- Bonus 不超过基础属性的 50%（避免单件装备过强）

#### UI 表现
- 装备图标上有一个小图标显示加成时段：☀️（day）/🌙（night）/⚖️（both）
- 鼠标悬停时显示"白天：+X damage" / "夜晚：+X speed"
- 当前时间生效时，StatBlock 数字**实时更新**

### C.6 拾取与丢弃（Pickup & Drop）

#### 拾取
- 玩家走到物品附近（1 网格内）按 E / interact
- 物品**优先入背包**：
  - 同类已存在且未满 → 合并堆叠
  - 背包有空槽 → 占新槽
  - 背包满 → **不**入背包，留在地面
- 拾取时有视觉反馈：物品飞向玩家，UI "+1 item"
- 远程拾取（5 网格内）：长按 E（0.5 秒）

#### 丢弃
- 玩家按 Q 丢弃选中物品 1 个
- 物品在玩家脚下生成 30 秒地面对象
- 30 秒后自动消失（防堆场）
- 装备：卸下后自动入背包（如果有空）；否则先卸到地面再捡回来

#### 自动拾取（v1.1 决策）
- MVP：手动拾取
- v1.1：可加"自动拾取资源"开关

### C.7 战利品分布

#### 战斗掉落
- 敌人死亡 → 立即生成 1-3 个物品
- 物品**直接飞入背包**（如果背包有空间且同类未满）
- 背包满 → 落到敌人死亡地点（地面对象）
- 玩家在战斗结束 5 秒后可以拾取

#### 编织产出
- 编织成功 → 产出物直接入背包
- 同上逻辑

#### 采集产出
- 玩家采集动作完成 → 资源入背包
- 同上逻辑

#### 远征奖励
- 远征任务完成 → UI 显示奖励列表 → 玩家按"确认"入背包
- 如果背包满 → **必须先腾空间**才能领取（这是设计强制的取舍点）

### C.8 快速使用热键栏（Quick-Use Hotbar）

- 4 个槽位（数字键 1-4）
- 玩家从 Inventory 拖 Consumable 到热键栏
- 战斗中按数字键立即使用
- 物品耗尽时热键槽变灰，自动从 inventory 找下一个同类

**热键栏规则**：
- 只能放 Consumable（不能放装备、Resource）
- 切换热键时 0.3 秒冷却（防止误触）
- 战斗中可以用，但有 0.5 秒"使用中"硬直

### C.9 排序与过滤

**MVP 排序**：
- 按类别（Resource / Equipment / Consumable）
- 按名称（字母）
- 按获取时间

**MVP 过滤**：
- 全部 / Resource / Equipment / Consumable / Quest

**EA 过滤**：
- 按 Tier（1-5）
- 按 Day-Night 加成（有 / 无）
- 按属性（damage, defense, speed）

### C.10 状态与转换（States and Transitions）

```
[Empty Inventory]
        ↓ 拾取
[Inventory: N items]
        ↓ 装备
[Equipment: 4 slots filled]
        ↓ Day-Night change
[Equipment Stats: bonus applied]
        ↓ 死亡
[Inventory preserved] (Save 持久化)
        ↓ 重生
[Restore on respawn]
```

**Inventory 状态**：
- `Empty`：0 物品
- `Partial`：1-23 物品
- `Full`：24 物品（不允许新拾取）

**Equipment 状态**：
- `Unequipped`：4 槽全空（初始）
- `Partial`：1-3 槽装备
- `Full`：4 槽全装备
- `Invalid`：装备了不兼容（如双持 + Off Hand）

### C.11 与其他系统的交互

| 系统 | 怎么用 Inventory |
|---|---|
| **Gathering** | 产出物入背包 |
| **Fate-Thread** | 消耗 Resource 编织 → 产出物入背包 |
| **Combat** | 死亡掉落入背包；装备影响伤害/防御 |
| **Day-Night** | Day-Night 加成实时影响装备属性 |
| **Oath** | 部分里程碑要求"装备 X 物品"或"收集 Y 资源" |
| **Einherjar** | 招募带回礼物入背包；送别后获得 Token |
| **Death & Send-off** | 死亡后 Inventory 保留 |
| **Save** | Inventory 状态 + Equipment 状态都持久化 |
| **UI/HUD** | Inventory UI + Equipment UI + Quickbar UI |
| **World Exploration** | 地面物品可拾取 |
| **Quest-Event** | 部分事件奖励入背包 |

---

## D. Formulas

### F.1 物品拾取（入背包判定）
```csharp
bool CanAddItem(string itemId, int quantity) {
  // 同类已存在且未满
  var existingStack = FindStack(itemId);
  if (existingStack != null && existingStack.quantity < GetMaxStack(itemId)) {
    int space = GetMaxStack(itemId) - existingStack.quantity;
    return quantity <= space;
  }
  // 背包有空槽
  return GetEmptySlotCount() > 0;
}
```

### F.2 装备属性计算（含 Day-Night 加成）
```csharp
StatBlock CalculateEffectiveStats(EquipmentSlot slot) {
  var item = GetEquippedItem(slot);
  if (item == null) return StatBlock.Zero;

  StatBlock baseStats = item.baseStats;
  StatBlock bonus = StatBlock.Zero;

  if (IsDay() && item.dayBonus.HasValue) {
    bonus = item.dayBonus.Value.stats;
  } else if (IsNight() && item.nightBonus.HasValue) {
    bonus = item.nightBonus.Value.stats;
  } else if (IsDawn() || IsDusk()) {
    // 转换期插值
    float t = GetTransitionProgress();
    StatBlock day = item.dayBonus?.stats ?? StatBlock.Zero;
    StatBlock night = item.nightBonus?.stats ?? StatBlock.Zero;
    bonus = StatBlock.Lerp(day, night, t);
  }

  return baseStats + bonus;
}
```

### F.3 拾取距离判定
```csharp
bool IsInPickupRange(Vector3 playerPos, Vector3 itemPos, bool isLongPress) {
  float range = isLongPress ? 5f : 1f;  // 长按 = 5 格
  return Vector3.Distance(playerPos, itemPos) <= range;
}
```

### F.4 地面物品消失时间
```csharp
float GROUND_ITEM_LIFETIME_HOURS = 0.5f;  // 30 游戏分钟
// 即 0.5 游戏小时 = 0.5 × 30 真实秒 = 15 真实秒
```

### F.5 Quickbar 切换冷却
```csharp
float QUICKBAR_SWITCH_COOLDOWN_SEC = 0.3f;
```

### F.6 双持武器占用判定
```csharp
bool CanEquipInOffHand(ItemSO item) {
  if (item == null) return true;  // 卸下任何 Off Hand 都可以
  var mainHand = GetEquippedItem(EquipmentSlot.MainHand);
  if (mainHand != null && mainHand.isTwoHanded) return false;  // 双手占用时不能装 Off
  return true;
}
```

---

## E. Edge Cases

| 情况 | 处理 |
|---|---|
| 背包满时拾取 | 物品**不**入背包，落到地面 |
| 背包满时领取远征奖励 | 强制弹"清理背包"提示，**不**入背包直到腾空间 |
| 装备有 Day-Night 加成，转换期间 | smoothstep 插值（避免突变） |
| 双手武器已装备，尝试装备 Off Hand | 拒绝，UI 提示"请先卸下 Main Hand" |
| 死亡时背包满 | 物品**全部保留**在死亡地点（不会丢） |
| 装备了限制级物品（如剧情神器） | 不可卸下（直到剧情结束） |
| 地面物品被另一个玩家干扰（PvP） | MVP 单机，不考虑 |
| 同一物品 ID 在不同位置（如 stash） | 未来扩展，MVP 不支持 |
| 物品使用过程中被打断 | 使用进入**硬直状态**，不能被取消 |
| 远程拾取中移动 | 拾取**取消**（避免误触） |
| 装备附魔/Buff 物品（有 StatusEffect） | v1.1 才考虑，MVP 不支持附魔 |
| Token 物品误丢弃 | **禁止丢弃** Token |
| Quest 物品误丢弃 | **禁止丢弃** Quest 物品 |
| 装备槽已装备时切换同类 | 旧的入背包，**新装备立即生效** |

---

## F. Dependencies

### 上游（Inventory 依赖谁）

- **Data Config** —— 所有物品数据来自 `ItemSO`；`GameConfigSO` 提供 maxStack 调参
- **Day-Night** —— Day-Night 加成需要当前时间
- **Input** —— 拾取 / 丢弃 / 切换的输入抽象
- **Save** —— 状态持久化

### 下游（谁依赖 Inventory）

- **Fate-Thread** —— 读 inventory 物品作为输入；产出物入 inventory
- **Combat** —— 装备属性影响伤害/防御；战利品入 inventory
- **Gathering** —— 产出物入 inventory
- **Oath** —— 部分里程碑要求"装备 X 物品"或"收集 Y 资源"
- **Einherjar** —— 招募带回礼物入 inventory
- **Death & Send-off** —— 死亡后 inventory 保留
- **UI/HUD** —— 渲染 inventory / equipment / quickbar

**Inventory 是"中段"枢纽**——它本身简单（只是 24 槽 + 4 装备槽），但被几乎所有系统消费。

---

## G. Tuning Knobs

> 调参字段已加到 `GameConfigSO`（data-config v1.2 同步升级）

| 参数 | 默认值 | 调参影响 |
|---|---|---|
| `inventoryMaxSlots` | 24 | 越大 → 玩家携带越多，越少取舍 |
| `inventoryGroundItemLifetimeHours` | 0.5 | 越短 → 防堆场强；越长 → 玩家可回头捡 |
| `quickbarSlotCount` | 4 | 越多 → 战斗中可用物品越多 |
| `quickbarSwitchCooldownSec` | 0.3 | 越短 → 反应越快；越长 → 防止误触 |
| `pickupRange` | 1.0 | 拾取近距离（格） |
| `pickupLongPressRange` | 5.0 | 长按拾取距离（格） |
| `pickupLongPressDurationSec` | 0.5 | 长按判定时间 |
| `defaultMaxStackResource` | 99 | Resource 默认最大堆叠 |
| `defaultMaxStackConsumable` | 20 | Consumable 默认最大堆叠 |
| `dayNightBonusMaxRatio` | 0.5 | Bonus 不超过基础属性的 50% |

**§C.5 特色相关的设计开关**：
- `enableDayNightItemBonuses` = true —— 必须开，否则 Ravensong 特色丢失

---

## H. Acceptance Criteria

### AC-1: 24 槽 Inventory 准确
**测试**：
1. 拾取 24 个不同物品
2. 第 25 个物品落地不拾取
3. 卸下 1 个后，第 25 个可以拾取
4. **期望**：恰好 24 槽，第 25 拒绝

### AC-2: 堆叠合并正确
**测试**：
1. 拾取 50 个 Resource A → 占 1 槽（堆叠 50）
2. 再拾取 49 个 Resource A → 同一槽（堆叠 99）
3. 再拾取 1 个 Resource A → 满 99，新槽占 1
4. **期望**：堆叠规则正确

### AC-3: Day-Night 加成实时切换
**测试**：
1. 装备 `item_moonlit_spear`（nightBonus +20% damage）
2. 在 Day 攻击 → damage = 基础 10
3. 玩到 Night → damage = 基础 10 + 2 = 12
4. 玩到 Dawn → damage 在 1-2 秒内 smoothstep 插值回 10
5. **期望**：加成实时准确，转换平滑

### AC-4: 装备属性计算正确
**测试**：
1. 同时装备 4 件装备（Main, Off, Armor, Accessory）
2. 玩家总 damage = 所有基础 StatBlock 之和 + 适用的 Day-Night 加成
3. **期望**：属性聚合正确（不漏不重）

### AC-5: 双手武器规则
**测试**：
1. 装备双手武器 `item_great_sword`
2. 尝试装备 Off Hand → 拒绝
3. 卸下 Main Hand → 可以装备 Off Hand
4. **期望**：双手武器占用规则正确

### AC-6: 拾取 / 丢弃 / 死亡 / 重生
**测试**：
1. 拾取 10 个物品
2. 丢弃 1 个 → 背包 9 个，地面 1 个
3. 等 15 真实秒 → 地面物品消失
4. 死亡 → 死亡时 9 物品都保留
5. 重生 → 物品还在
6. **期望**：行为符合预期

### AC-7: Quickbar 切换
**测试**：
1. 装备 `item_minor_healing_potion` 到 hotbar 1
2. 按数字键 1 → 立即使用，物品 -1
3. 装备另一种 Consumable 到 hotbar 1
4. 立即按 1 → 使用新装备的物品
5. **期望**：切换和使用流畅

### AC-8: 性能预算
**测试**：
1. 满 inventory 24 个物品
2. 每次拾取 / 丢弃 < 1ms
3. Day-Night 切换时属性重算 < 0.5ms
4. 100 个玩家 1 帧 = 100 × 0.5ms = 50ms（**需要缓存机制**）

### AC-9: UI 响应
**测试**：
1. 打开 Inventory UI
2. 切换 Tab / 排序 / 过滤 < 16ms
3. **期望**：60 FPS 流畅

### AC-10: Day-Night 加成在 UI 上可见
**测试**：
1. 鼠标悬停 `item_sunforged_shield`（dayBonus）
2. **期望**：tooltip 显示"☀️ 白天：+3 defense"
3. 当前是 Day 时：StatBlock 数字高亮（实时）
4. **期望**：玩家**看见** Day-Night 切换的效果

---

## 10. 已锁定决策（Locked Decisions）

> 2026-07-27 用户拍板，6 个开放问题全部锁定。已落地为 G 旋钮 + data-config v1.2。

| # | 决策点 | 锁定值 | 落地位置 |
|---|---|---|---|
| 1 | **Inventory 模型** | **24 槽固定 + 堆叠**（不选重量/Tetris） | §C.3 Inventory 模型 |
| 2 | **双手武器** | **支持**（Main Hand 占位 + 禁止 Off Hand） | §C.4 装备槽 + §D.6 双持判定 |
| 3 | **Day+Night 加成叠加** | **普通装备最多 1 个 / 传说装备可 2 个** | §C.5 Day-Night 加成 |
| 4 | **远程拾取** | **MVP 不做**（v1.1 加） | §C.6 拾取规则 |
| 5 | **地面物品消失时间** | **30 真实秒 = 30 游戏分钟**（战场余韵时间） | §C.6 丢弃规则 + §D.4 |
| 6 | **Quickbar 槽数** | **4 槽**（4 类消耗品各 1 = 满配） | §C.8 Quickbar |

### 决策之间的协同

- **#1 + #2 + #5**：24 槽 + 双手武器 + 30 秒战场留痕——三者形成"取舍玩法"的完整回路。**24 槽限制了你能带多少，30 秒决定了你能抢回多少，双手武器让你在带得少时还能打**
- **#3 + Day-Night GDD**：传说装备的双重加成让 Day-Night 加成系统**有了"奖品层"**——普通装备是工具，传说装备是仪式品
- **#4 MVP 不做**：避免 v1.0 加便利性优化拖累核心循环；v1.1 加是给"已经跑通的核心"做润色

### 仍待 playtest 调参（不阻塞 GDD 锁定）

| 待调项 | 候选范围 | 调参位置 |
|---|---|---|
| 24 槽 vs 32 槽 | 玩家跑测后决定 | `GameConfigSO.inventoryMaxSlots` |
| Day-Night Bonus 上限 50% | vs 30% vs 70% | `GameConfigSO.dayNightBonusMaxRatio` |
| 双手武器是否允许 Day-Night 双加成 | playtest 决定 | `ItemSO` schema |
| Quickbar 是否能放装备 | playtest 决定 | `Quickbar.cs` |

→ 这些都是 Prototype 阶段的**数值调参工作**，不阻塞任何 GDD。

---

## Unity Implementation Notes

### 核心脚本（`Assets/Scripts/Inventory/`）
- `Inventory.cs` —— 24 槽 Stack<ItemStack> + 拾取/丢弃/查询 API
- `Equipment.cs` —— 4 槽 + 属性聚合 + Day-Night 加成计算
- `ItemDatabase.cs` —— 包装 `DataRegistry.GetItem` + 缓存
- `GroundItem.cs` —— 地面物品物理对象 + 30 分钟消失
- `Quickbar.cs` —— 4 槽 Consumable 容器
- `InventoryUI.cs` —— Tab 切换 + 排序 + 过滤
- `EquipmentUI.cs` —— 4 槽视觉 + tooltip
- `QuickbarUI.cs` —— 4 槽热键栏
- `StatBlock.cs` —— 简单结构体（damage, defense, speed, attackSpeed 等）

### 数据结构
```csharp
[Serializable]
public struct ItemStack {
  public string itemId;
  public int quantity;
}

public class InventoryData {
  public List<ItemStack> stacks = new();  // max 24
  public Dictionary<EquipmentSlot, string> equipment = new();
  public List<string> questItems = new();  // 单独存
}
```

### 事件订阅
```csharp
public class Inventory : MonoBehaviour {
  public static event Action<string, int> OnItemAdded;       // itemId, qty
  public static event Action<string, int> OnItemRemoved;
  public static event Action<EquipmentSlot> OnEquipmentChanged;
  public static event Action OnInventoryFull;
}
```

### Day-Night 联动
- 订阅 `TimeManager.OnTimeStateChanged` 事件
- 状态变化时重算所有装备属性
- 触发 `OnEquipmentChanged` 让 UI 更新

### 性能预算
- 拾取 / 丢弃：O(N) 槽位扫描，< 0.1ms
- 装备聚合：4 件装备属性相加，< 0.05ms
- Day-Night 切换：重算 4 件装备，< 0.2ms

---

## 进度

| 章节 | 状态 |
|---|---|
| A. Overview | ✅ |
| B. Player Fantasy | ✅ |
| C. Detailed Design (11 小节) | ✅ |
| D. Formulas (6 个) | ✅ |
| E. Edge Cases (14 种) | ✅ |
| F. Dependencies | ✅ |
| G. Tuning Knobs (10 字段已落 v1.2) | ✅ |
| H. Acceptance Criteria (10 条) | ✅ |
| **10. Locked Decisions (6 决策)** | ✅ |
| Unity Implementation Notes | ✅ |

**总进度**: 10/10 (100%) 🔒

**🔒 已锁定 v1.0** —— 8 段全填 + 6 开放问题落地 + Day-Night Item Bonus 特色机制确认。

---

## 变更日志

| 日期 | 版本 | 改动 | 责任人 |
|---|---|---|---|
| 2026-07-27 | v1.0 draft | 初版生成：11 段 + 公式 + Unity 实施 | Mavis |
| 2026-07-27 | **v1.0 LOCKED** | 6 开放问题用户拍板全部锁定；§C.5 Day-Night 加成特色机制确认；data-config v1.2 同步升级 | Mavis + 用户 |
