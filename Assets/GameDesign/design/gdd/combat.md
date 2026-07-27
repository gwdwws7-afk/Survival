# Combat — System GDD

> **Status**: 🔒 LOCKED v1.0
> **Author**: Mavis + 用户
> **Last Updated**: 2026-07-27
> **Implements Pillar**: Woven Power（编织之力）+ Waxing Moon（反节奏的爽感）

---

## A. Overview

**Combat 是 Ravensong 的"考验场"——所有 L2/L3 系统在战斗中真实跑通。** 玩家用 Light / Heavy / Block / Dodge / 织线 5 种动作对付 4 种敌人 + 3 阶段 boss。**丝线绑定**是 Ravensong 战斗的签名机制（systems-index 锁定决策 #7）——战斗中玩家可以**用命运丝线绑定 boss 招式**，这是其他 ARPG 都没有的体验。

Combat 高度集成已锁的 4 个 GDD：
- **Day-Night** —— 玩家伤害 + 敌人警觉范围 + 月相词缀
- **Inventory** —— 装备的 StatBlock + Day-Night 加成
- **Input** —— 输入缓冲（combo）+ Action Map 切换
- **Day-Night §C.10** —— 敌人警觉范围可视化（红/绿光圈）

数据层由 `BossSO` / `EnemySO` 驱动；本 GDD 专注于**动作、状态、伤害、AI、boss 机制、丝线战斗应用**。

---

## B. Player Fantasy

### 主幻想
> "我用命运丝线绑住 boss 的雷电，绕到背后挥出致命一击——月光矛在夜色中燃起 cyan 的拖尾。"

### 关键体验时刻
- **第一次**轻击 → 看到剑光 + 敌人血条减少 + 命中粒子
- **第一次**重击 → 角色大幅挥砍，敌人被击退 2 格
- **第一次**闪避 → 敌人攻击"穿过"你（i-frames），玩家感到"反应够快就无敌"
- **第一次**boss 战 → boss 巨大身姿 + 3 阶段 + 屏幕震动
- **第一次**织线绑住 boss 招式 → cyan 丝线缠住 boss 的雷击 2 秒，**boss 硬直**
- **第一次**满月夜击 boss → 月相"神显"词缀让传说武器伤害 +50%

---

## C. Detailed Design

### C.1 核心规则（Core Rules）

#### 规则 1：5 种玩家动作
- **Light Attack**（轻击）：低伤害，快，硬直短
- **Heavy Attack**（重击）：高伤害，慢，硬直长
- **Block**（格挡）：减少 70% 伤害，持续按住
- **Dodge**（闪避）：0.3s i-frames，移动 4 格
- **Fate-Thread Bind**（织线绑定）：5s 冷却，绑定 boss/敌人招式 2s

#### 规则 2：4 方向攻击
- 玩家面朝 4 方向之一（N/S/E/W）
- 攻击方向 = 玩家当前面朝方向
- 面朝 = 鼠标位置（默认）或最后移动方向
- 移动 = 4 方向 8 帧动画（与 v1 art 一致）

#### 规则 3：3 段连击（Combo）
- Light → Light → Heavy 是标准 combo
- Input Buffer（0.2s，Input GDD）让 combo 流畅
- Heavy 不能起手（必须有 Light 在前）
- Combo 第三击（Heavy）有**+30% 伤害**（combo bonus）

#### 规则 4：伤害公式
```
finalDamage = baseDamage * playerDamageMult * dayNightMult * equipmentStatMult * comboBonus
```

- `baseDamage` = 武器基础伤害（ItemSO.stats.damage）
- `playerDamageMult` = 1.0（基础）
- `dayNightMult` = Day 0.75 / Night 1.2（GameConfigSO）
- `equipmentStatMult` = 装备 StatBlock 加成（含 Day-Night 物品加成）
- `comboBonus` = 1.0 / 1.0 / 1.3（3 段 combo）

#### 规则 5：Stamina 系统
- 最大 Stamina = 100
- 攻击/闪避/格挡消耗 Stamina
- 消耗：Light 10 / Heavy 25 / Dodge 20 / Block 持续 5/秒
- 恢复：每秒 +15（不攻击时 2x = +30）
- Stamina = 0 时**只能 Block**（其他动作禁用）

#### 规则 6：Hit & Death
- 玩家 HP 归 0 → 死亡
- 死亡 → 触发 Save System 紧急 autosave + 重启
- 死亡**不**丢失物品（Inventory 完整保留）
- 英灵**不**受玩家死亡影响（继续在聚落工作）

#### 规则 7：战斗时 Time.timeScale = 1（正常）
- 但**boss 战**开始时**冻结 Day-Night 时间**（per Day-Night GDD C.8）
- 战斗结束 → 时间解冻

#### 规则 8：战斗状态独立于 Day-Night 状态
- 战斗时玩家**不能**触发菜单 / 切换装备（避免中断）
- 只有 Pause 可用（暂停游戏）

### C.2 Combat States

```
[Idle]
  ↓ Attack pressed (Light/Heavy)
[Attacking_Light] / [Attacking_Heavy]
  ↓ (animation 0.3-0.6s)
[Recovery]
  ↓ (animation 0.2-0.4s)
[Idle]

[Idle]
  ↓ Block pressed
[Blocking]
  ↓ Block released / Stamina = 0
[Idle]

[Idle]
  ↓ Dodge pressed
[Dodging]
  ↓ (i-frames 0.3s)
[Idle]

[Idle]
  ↓ Thread Bind pressed (5s cooldown ready)
[ThreadBinding]
  ↓ (animation 0.5s)
[Idle] + target bound 2s

[Idle/Attacking/Blocking]
  ↓ Take damage (not blocked, not i-frames)
[Hit]
  ↓ (stagger 0.3s)
[Idle]

[Any state]
  ↓ HP = 0
[Dead]
  ↓ (respawn animation)
[Idle]
```

### C.3 Player Actions 详解

#### Light Attack
- **时间**：0.3s 挥砍 + 0.2s 恢复 = 0.5s 总
- **伤害**：baseDamage × 1.0
- **Stamina**：10
- **范围**：玩家前方 1.5 格扇形（90°）
- **Hit Frame**：0.2s（挥砍到一半时判定命中）
- **可以**起手（无前置动作）
- **打断**敌人正在执行的攻击（打断其 0.5s 进度）

#### Heavy Attack
- **时间**：0.6s 挥砍 + 0.4s 恢复 = 1.0s 总
- **伤害**：baseDamage × 1.8
- **Stamina**：25
- **范围**：玩家前方 2 格扇形（120°）
- **Hit Frame**：0.4s
- **不可**起手（必须有 Light 在前）
- **击退**敌人 2 格
- **combo 第三击**：再 +30% 伤害

#### Block
- **时间**：持续按住
- **效果**：减少 70% 伤害
- **Stamina**：5/秒
- **完美格挡**（Perfect Block）：在敌人攻击命中前 0.1s 按下 Block
  - 0% 伤害 + 敌人**硬直** 1.5s
  - Stamina 消耗：5（一次性）
  - VFX：蓝色闪光
- **Block 期间玩家移速**：-50%

#### Dodge
- **时间**：0.3s 动画 + 0.1s 恢复 = 0.4s 总
- **i-frames**：0.3s（无敌）
- **Stamina**：20
- **距离**：4 格（按方向）
- **无伤害**（纯位移）
- **打断**自己当前动作（紧急闪避）
- **链式**：闪避后 0.1s 内可接 Light Attack（"闪避反击"）

#### Fate-Thread Bind（⭐ 签名机制）
- **时间**：0.5s 投线 + 2s 绑定持续 = 2.5s
- **Stamina**：30
- **God-Ember 消耗**：15
- **冷却**：5 秒
- **范围**：8 格（中等距离）
- **目标**：必须有 `threadBindableAttacks` 标签的敌人/boss 招式
- **效果**：敌人当前招式被中断，进入**硬直** 2 秒
- **视觉**：cyan 丝线缠住敌人 + 符文浮现
- **声音**：Ragnarok 钟声 + 丝线嗡鸣

**为什么这是 Ravensong 特色**：
- 其他 ARPG 玩家只能"看招反击"或"翻滚躲"
- Ravensong 玩家可以**用丝线锁定 boss 招式**——这要求**事先看穿 boss 套路**
- 是**主动技术**而不是**反应技术**——符合 Fate-Thread "编织之力"支柱

### C.4 Combo System

| Combo | Sequence | Bonus |
|---|---|---|
| 单击 | Light | × 1.0 |
| 单击 | Heavy | × 1.8（**不可**起手） |
| 2 段 | Light → Light | × 1.0 + × 1.0 = 总 × 1.0 |
| 3 段 | Light → Light → Heavy | × 1.0 + × 1.0 + × 1.8 × 1.3 = 总 × 1.55 |

**Combo 规则**：
- 必须在 0.5s 内连击，否则 combo 断
- Input Buffer（Input GDD）保证 0.2s 内输入不被丢
- 任何动作（Block / Dodge / Thread Bind）打断 combo
- Combo 期间玩家**可以移动**（但移速 -30%）

### C.5 Damage System

#### 伤害来源分类
```csharp
enum DamageSource {
  Player_Light,
  Player_Heavy,
  Player_ComboBonus,
  Player_ThreadBind,        // 绑定时 0 伤害，但触发 boss 硬直
  Player_EquipmentDayBonus, // 装备 dayBonus
  Player_EquipmentNightBonus, // 装备 nightBonus
  Enemy_Melee,
  Enemy_Ranged,
  Enemy_BossPhase1/2/3,
  Environmental,             // 坠落 / 毒
}
```

#### 伤害计算公式
```csharp
float CalculateFinalDamage(
  float baseDamage,
  float playerDamageMult,
  float dayNightMult,
  float equipmentStatMult,
  float comboBonus,
  float defenseMult
) {
  float raw = baseDamage * playerDamageMult * dayNightMult 
              * equipmentStatMult * comboBonus;
  float final = raw * defenseMult;  // 防御减免
  return Mathf.Max(1f, final);  // 至少 1 点
}
```

#### 防御公式
```csharp
float CalculateDefenseMult(float defense) {
  // 防御 0 = 100% 伤害
  // 防御 100 = 50% 伤害
  // 防御 200 = 33% 伤害
  return 100f / (100f + defense);
}
```

#### 暴击（v1.1）
- MVP 不做暴击
- v1.1 加暴击率 / 暴击伤害

### C.6 Enemy AI

#### Enemy 类型
- **Melee**（近战）：血量 30，攻击 5
- **Ranged**（远程）：血量 20，攻击 4，攻击距离 5 格
- **Heavy**（重型）：血量 60，攻击 12，慢速
- **Fast**（轻型）：血量 15，攻击 3，快速连击

#### Enemy 状态机
```
[Idle]
  ↓ Player in alert range
[Chasing]
  ↓ In attack range
[Attacking]
  ↓ Animation done
[Recovery]
  ↓ Animation done
[Idle]
```

#### Day-Night 对 AI 的影响（per GameConfigSO）
- **Day**：敌人警觉范围 × 1.3（警觉），攻击间隔 × 0.8（更频繁）
- **Night**：敌人警觉范围 × 1.0，攻击间隔 × 1.2（更慢）
- **蛾眉月/亏月"沉默"**：警觉范围 × 0.8
- **满月"洞察"**：玩家**也**能看见敌人（已经实现）

#### Enemy 警觉范围可视化（per Day-Night GDD §C.10）
- 敌人脚下有圆形光圈
- 颜色：Day 红 / Night 绿 / 沉默月 蓝
- 玩家进入光圈时光圈变亮（alpha 60%）

### C.7 Boss Combat（⭐ 重点）

**3 阶段是设计铁律**——所有 world boss 都按这个结构。

#### Boss 阶段（来自 BossSO）
```csharp
public class BossSO : ScriptableObject {
  public string id;
  public BossPhase[] phases = new BossPhase[3];  // 固定 3 阶段
}

public class BossPhase {
  public string phaseName;
  public float healthThreshold;     // 0.66, 0.33
  public BossAttack[] attacks;
  public BossModifier[] modifiers;   // 攻击 +X% / 移速 +X%
  public string phaseTransitionQuote; // 阶段转换时 boss 说的话
}
```

#### Boss 状态机
```
[Phase 1: 100% - 66% HP]
  ↓ HP < 66%
[Phase 2: 66% - 33% HP]
  ↓ HP < 33%
[Phase 3: 33% - 0% HP]
  ↓ HP = 0
[Dead]
```

#### Boss 攻击类型
- **Melee Combo**（近战连击）：3-4 段，玩家需 Block 或 Dodge
- **Ranged Attack**（远程）：投掷 / 喷射，玩家需 Dodge
- **AOE Attack**（范围攻击）：大范围，玩家需离开区域
- **Charged Attack**（蓄力攻击）：3s 蓄力，**可被丝线绑定**
- **Summon**（召唤）：召唤小怪，2 只一组

**Charged Attack 是丝线绑定的目标**——这是 Ravensong 战斗的"考试"。

#### 阶段转换
- HP 跌破阈值 → 触发**阶段转换动画**（3s）
- boss 不可被攻击
- 播放 `phaseTransitionQuote`（如果 BossSO 配了 dialogue）
- 转换后：进入新 phase，应用 modifiers（攻击 +X% / 移速 +X%）

#### Boss 战特殊规则
- 时间冻结（Day-Night GDD C.8）
- 满月"神显"词缀**冻结**到 boss 战结束
- 玩家死亡 → 紧急 autosave + 重启
- boss 死亡 → 自动存档 + 战利品掉落到 ground → 玩家拾取

### C.8 织线战斗应用（⭐ 签名机制详细）

**这是 Combat GDD 的核心机制**。

#### 触发条件
1. 玩家在战斗中
2. 目标（敌人/boss）正在执行 `threadBindableAttacks` 中的招式
3. 玩家有 ≥ 15 God-Ember
4. 玩家 Stamina ≥ 30
5. Thread Bind 5s 冷却已就绪

#### 流程
```
玩家按 Thread Bind 键
    ↓ (0.5s 投线动画)
[判定]: 距离 ≤ 8 格 + 目标正在执行可绑定招式
    ↓ 成功
目标招式**中断**，进入**硬直 2 秒**
    ↓
消耗 15 God-Ember + 30 Stamina
    ↓
5s 冷却开始
    ↓
[目标进入硬直] 期间玩家可自由攻击（无反击风险）
    ↓ 2 秒后
目标恢复 AI
```

#### 视觉
- 玩家手上 cyan 丝线甩出
- 丝线缠住目标当前招式
- 目标身上出现 cyan 符文
- 持续 2 秒

#### 平衡
- **5 秒冷却**（不能滥用）
- **15 God-Ember**（1 战斗约 5-7 次）
- **只能绑定 `threadBindableAttacks`**（非所有招式）
- **只有** boss + Heavy 类型敌人才有可绑定招式

#### 与其他 ARPG 的对比
| 游戏 | 应对 boss 招式的机制 |
|---|---|
| Dark Souls | 看招 + 翻滚 |
| Hades | 看招 + dash |
| Sekiro | 看招 + 完美格挡 |
| **Ravensong** | **看招 + 织线绑定**（主动技术） |

**织线是 Ravensong 区别于其他 ARPG 的核心战斗创新**。

### C.9 Death & Respawn

#### 玩家死亡
- HP 归 0 → 触发死亡动画（1s）
- 触发 Save System 紧急 autosave
- **不**丢失物品（Inventory 完整）
- **不**丢失英灵（Einherjar 继续在聚落）
- **不**丢失进度（除非 autosave 失败）

#### 重启位置
- 玩家在**聚落**重生（最近聚落中心）
- HP 满 / Stamina 满
- Day-Night 时间**不变**（死亡不消耗时间）
- 所有 buff/debuff **清除**

#### Boss 战死亡
- boss 战**结束**（boss HP 不变）
- 玩家回聚落
- boss 需**再次触发**（不能立即重打）

### C.10 与其他系统的交互

| 系统 | 怎么用 Combat |
|---|---|
| **Day-Night** | 玩家伤害 / 敌人警觉 / 月相 / boss 时间冻结 |
| **Inventory** | 装备影响 StatBlock；消耗消耗品回血 |
| **Input** | 输入缓冲（combo） + Action Map（战斗锁定） |
| **Fate-Thread** | 织线绑定（签名） + god-ember 消耗 |
| **Gathering** | 战斗不打断 gathering（gathering 时被攻击则取消） |
| **Einherjar** | 战斗不影响英灵（继续工作） |
| **Oath** | 部分里程碑要求"击败 X 敌人" / "击败 Y boss" |
| **World Exploration** | 战斗在群系内发生；敌人警觉范围 vs 群系 |
| **Quest-Event** | 部分事件触发战斗 |
| **Save** | 玩家位置 + boss HP + 死亡紧急存档 |
| **UI/HUD** | 玩家血条 + Stamina + 敌人血条 + 伤害数字 |
| **VFX** | 攻击粒子 / 命中粒子 / 丝线 / 暴击 / 完美格挡 |

---

## D. Formulas

### D.1 最终伤害公式
```csharp
float CalculateFinalDamage(
  float baseDamage,
  float dayNightMult,
  float equipmentStatMult,
  float comboBonus,
  float defense
) {
  float raw = baseDamage * dayNightMult * equipmentStatMult * comboBonus;
  float damageAfterDefense = raw * (100f / (100f + defense));
  return Mathf.Max(1f, damageAfterDefense);
}
```

### D.2 Stamina 消耗
```csharp
int CalculateStaminaCost(ActionType action) {
  switch (action) {
    case ActionType.LightAttack: return 10;
    case ActionType.HeavyAttack: return 25;
    case ActionType.Dodge: return 20;
    case ActionType.Block: return 5;  // per second
    case ActionType.ThreadBind: return 30;
  }
}
```

### D.3 警觉范围（Day-Night 影响）
```csharp
float GetAlertRange(Enemy enemy) {
  float base = enemy.alertRange;  // from EnemySO
  float mult = TimeManager.IsDay() ? 1.3f : 1.0f;
  
  // 月相加成
  if (TimeManager.GetMoonPhase() == MoonPhase.WaningCrescent) {
    mult *= 0.8f;  // 沉默
  }
  
  return base * mult;
}
```

### D.4 完美格挡窗口
```csharp
bool IsPerfectBlock(EnemyAttack attack, float blockStartTime) {
  float blockTime = blockStartTime - attack.startTime;
  return blockTime >= attack.windupDuration - 0.1f  // 攻击命中前 0.1s
      && blockTime <= attack.windupDuration;
}
```

### D.5 Combo 状态
```csharp
enum ComboState { None, Light, LightLight }

ComboState AdvanceCombo(ComboState current) {
  if (current == ComboState.None) return ComboState.Light;
  if (current == ComboState.Light) return ComboState.LightLight;
  if (current == ComboState.LightLight) return ComboState.None;  // 打完重置
  return ComboState.None;
}
```

### D.6 Thread Bind 命中判定
```csharp
bool CanThreadBind(BossAttack attack, Vector3 playerPos, Vector3 bossPos) {
  if (!attack.threadBindable) return false;
  if (Vector3.Distance(playerPos, bossPos) > 8f) return false;
  if (player.godEmber < 15) return false;
  if (player.stamina < 30) return false;
  if (Time.time - lastBindTime < 5f) return false;  // 冷却
  return true;
}
```

---

## E. Edge Cases

| 情况 | 处理 |
|---|---|
| 玩家在 combo 中按 Block | combo 断，进入 Block |
| 玩家在闪避中按 Light | 闪避反击（0.1s 内）= Light |
| 玩家在闪避中超过 0.1s 按 Light | 普通 Light（combo 断） |
| 玩家 Stamina = 0 | 仅 Block 可用，其他动作禁用 |
| 玩家在绑定中受攻击 | 绑定**不**被打断（i-frames） |
| Boss 战时玩家死亡 | 紧急存档 + 回聚落 + boss HP 满血 |
| Boss 在阶段转换时被织线绑定 | 绑定**无效**（boss 不可选中） |
| 玩家攻击无敌敌人 | 命中但 0 伤害，敌人硬直 0.3s |
| 敌人死亡时玩家被攻击 | 攻击**无效**（目标已死） |
| 远程敌人被近战 | 玩家必须靠近（受敌人警觉范围） |
| 多敌人同时攻击 | 玩家必须**选择**（不能同时 Block 所有） |
| Combo 中玩家受击 | combo 断，进入 Hit 状态 |
| 玩家在 Boss 战时用回城卷轴 | **禁止**（boss 战锁定） |
| 玩家在 Boss 战时按 Pause | 可暂停，但 boss 也暂停（正常 pause） |
| 玩家在 Boss 战时退出游戏 | autosave 触发（boss HP 已存），下次回到 boss 战 |
| 玩家攻击时切换装备 | 攻击**进行中**时锁定装备切换 |
| 织线绑定普通敌人 | **可**（但效果弱，1s 硬直），boss 才有 2s 硬直 |
| 满月"神显" + boss 战 | "神显"词缀**冻结**到 boss 战结束 |
| 死亡时 boss HP 重置 | **不**保留（避免刷 boss） |

---

## F. Dependencies

### 上游（Combat 依赖谁）

- **Data Config** —— BossSO / EnemySO / ItemSO 调参
- **Day-Night** —— 玩家伤害 / 敌人警觉 / 月相 / boss 时间冻结
- **Inventory** —— 装备 StatBlock + Day-Night 加成
- **Input** —— 输入缓冲（combo）+ Action Map（战斗锁定）
- **Fate-Thread** —— 织线绑定的 god-ember 消耗

### 下游（谁依赖 Combat）

- **Oath** —— 部分里程碑要求击败 X 敌人 / Y boss
- **World Exploration** —— 群系内战斗 / 敌人密度
- **Quest-Event** —— 部分事件触发战斗
- **Save** —— 玩家位置 + boss HP + 死亡紧急存档
- **UI/HUD** —— 玩家血条 + 敌人血条 + 伤害数字
- **VFX** —— 攻击粒子 / 命中粒子 / 丝线

**Combat 是 Ravensong 的"集成测试场"**——所有 L1/L2 系统在这里真实跑通。

---

## G. Tuning Knobs

> 调参字段建议加到 `GameConfigSO`（data-config v1.4 阶段）

| 参数 | 默认值 | 调参影响 |
|---|---|---|
| `combatPlayerMaxHP` | 100 | 玩家血量上限 |
| `combatPlayerMaxStamina` | 100 | 玩家 Stamina 上限 |
| `combatStaminaRegenPerSec` | 15 | 攻击时 Stamina 恢复 |
| `combatStaminaRegenIdleMultiplier` | 2.0 | 不攻击时恢复 × 2 |
| `combatLightAttackDamageMult` | 1.0 | 轻击伤害乘数 |
| `combatHeavyAttackDamageMult` | 1.8 | 重击伤害乘数 |
| `combatComboBonusMult` | 1.3 | 3 段 combo 第 3 击伤害加成 |
| `combatBlockDamageReduction` | 0.7 | Block 减少伤害（70%） |
| `combatPerfectBlockWindowSec` | 0.1 | 完美格挡窗口 |
| `combatDodgeIFrameSec` | 0.3 | 闪避无敌帧 |
| `combatDodgeDistance` | 4 | 闪避距离（格） |
| `combatThreadBindCooldownSec` | 5 | 织线绑定冷却 |
| `combatThreadBindStunSec` | 2 | 织线绑定硬直（boss） |
| `combatThreadBindStunSecNormal` | 1 | 织线绑定硬直（普通敌人） |
| `combatThreadBindGodEmberCost` | 15 | 织线绑定 God-Ember 消耗 |
| `combatThreadBindStaminaCost` | 30 | 织线绑定 Stamina 消耗 |
| `combatThreadBindRange` | 8 | 织线绑定距离（格） |
| `combatComboWindowSec` | 0.5 | Combo 续击最大间隔 |
| `combatEnemyAlertRangeDayMult` | 1.3 | 白天敌人警觉 × 1.3 |
| `combatEnemyAlertRangeNightMult` | 1.0 | 夜晚敌人警觉 × 1.0 |
| `combatBossPhasesCount` | 3 | Boss 阶段数（**永远 3**） |
| `combatBossTimeFreeze` | true | Boss 战时间冻结 |
| `combatDropOnGroundLifetimeSec` | 15 | 战利品落地时间（30 游戏分钟） |

---

## H. Acceptance Criteria

### AC-1: 5 种玩家动作都可用
**测试**：
1. 玩家对 Melee 敌人按 Light → 0.5s 内敌人受 1 击，-5 HP
2. 按 Heavy → 1.0s 内敌人受 1 击，-9 HP，击退
3. 按 Block（按住） → 敌人攻击伤害 -70%
4. 按 Dodge → 0.4s 内移动 4 格，期间无敌
5. 按 Thread Bind（5s 冷却） → boss 招式被打断 2s
6. **期望**：5 个动作都符合预期

### AC-2: Combo 3 段伤害正确
**测试**：
1. Light → Light → Heavy
2. **期望**：第 3 击 = baseDamage × 1.8 × 1.3 = base × 2.34
3. 单击 Heavy（无 Light 在前）→ 攻击**不**触发（combo 锁定）

### AC-3: Stamina 系统
**测试**：
1. 玩家满 Stamina 100
2. 连按 Light 5 次（每次 -10）→ Stamina 50
3. 连按 Heavy 2 次（每次 -25）→ Stamina 0
4. 满 Stamina = 100 时按 Thread Bind → Stamina 70
5. 静止 5 秒 → Stamina 恢复 100

### AC-4: Day-Night 影响玩家伤害
**测试**：
1. 装备月光矛（nightBonus +20% damage）
2. 白天攻击 → damage = 10 × 0.75 × 1.0 = 7.5
3. 夜晚攻击 → damage = 10 × 1.2 × 1.2 = 14.4
4. 满月夜 + 轻击 → damage = 10 × 1.2 × 1.2 = 14.4
5. **期望**：Day-Night 系数正确生效

### AC-5: 敌人警觉范围可视化
**测试**：
1. 玩家接近 Melee 敌人（base 警觉 5 格）
2. 白天 → 警觉光圈 6.5 格（5 × 1.3）
3. 夜晚 → 警觉光圈 5 格
4. 蛾眉月夜 → 警觉光圈 4 格
5. **期望**：光圈大小随时间变化

### AC-6: Boss 3 阶段
**测试**：
1. 与 boss 战斗，boss HP 100% → 66% = Phase 1
2. 跌破 66% → 阶段转换动画 3s
3. HP 33% → Phase 3
4. HP 0 → boss 死亡，掉落
5. **期望**：3 阶段严格按阈值切换

### AC-7: 织线绑定 boss
**测试**：
1. boss 执行可绑定 Charged Attack
2. 玩家按 Thread Bind（冷却已就绪）
3. 0.5s 后丝线缠住 boss
4. boss 招式**中断**，进入 2s 硬直
5. 玩家自由攻击 2 秒
6. boss 恢复 AI
7. 5s 冷却开始
8. **期望**：织线绑定是 boss 战"考试"

### AC-8: Boss 战时间冻结
**测试**：
1. 进入 boss 战时 Day-Night 时间为 14:00
2. 战斗 5 分钟（真实）
3. 退出 boss 战
4. Day-Night 时间**仍为 14:00**（冻结）
5. **期望**：boss 战不消耗 Day-Night 时间

### AC-9: 死亡 + 重启
**测试**：
1. 玩家 HP 归 0 → 死亡动画
2. 紧急 autosave 触发
3. 玩家在聚落重生，HP 满
4. inventory **完整**保留
5. Day-Night 时间**不变**
6. **期望**：死亡损失 < 30 真实分钟进度

### AC-10: 完美格挡
**测试**：
1. 敌人 Charged Attack 蓄力 3s
2. 玩家在 2.9s 按 Block
3. 0% 伤害 + 敌人硬直 1.5s
4. VFX 蓝色闪光
5. **期望**：完美格挡是高级技术

### AC-11: 性能预算
**测试**：
1. 战斗 5 个敌人 + 1 个 boss
2. **期望**：每帧 < 0.5ms 战斗计算
3. 60 FPS 流畅

### AC-12: 战斗时 UI
**测试**：
1. 玩家血条 / Stamina 显示
2. 每个敌人血条显示（敌人头顶）
3. 伤害数字浮出（白色）
4. 织线 VFX 可见
5. **期望**：所有 UI 元素清晰

---

## 10. 已锁定决策（Locked Decisions）

> 2026-07-27 用户拍板，9 个开放问题全部锁定。已落地为 G 旋钮 + data-config v1.4。

| # | 决策点 | 锁定值 | 落地位置 |
|---|---|---|---|
| 1 | **攻击方向** | **4 方向**（N/S/E/W，简化操作） | §C.1 规则 2 |
| 2 | **Stamina 系统** | **需要**（防止无限连击） | §C.1 规则 5 |
| 3 | **Stamina=0 仍可 Block** | **是**（Block 是基础防御） | §C.3 Block |
| 4 | **远程敌人** | **支持**（dodge 主应对） | §C.6 Enemy AI |
| 5 | **boss 死亡时玩家** | **玩家继续**（战后奖励） | §C.9 |
| 6 | **织线绑普通敌人** | **5s 冷却**（避免滥用） | §C.8 |
| 7 | **Combo 第四击** | **MVP 不做**（v1.1 加） | §C.4 |
| 8 | **完美格挡** | **MVP 加**（高级玩家必用） | §C.3 Block |
| 9 | **boss 战死后 boss HP** | **重置**（避免刷 boss） | §C.9 |

### 决策之间的协同

- **#1 + #2 + #4**：4 方向 + Stamina + 远程敌人 = **简单但有取舍**——4 方向操作简单，Stamina 限制深度，远程敌人打破"砍砍砍"模式
- **#3 + #5**：Block 基础防御 + 玩家在 boss 死后继续 = **不死循环**——Block 是"基础权"，玩家永远有防御手段
- **#6 + #7**：织线绑普通敌人 5s 冷却 + combo 3 段 = **核心动作不过度**——每个动作都有合理限制
- **#8 + #9**：完美格挡 + boss HP 重置 = **玩家技术有回报 + 但 boss 不能刷**——避免"刷 boss 刷完美格挡"

### 仍待 playtest 调参（不阻塞 GDD 锁定）

| 待调项 | 候选范围 | 调参位置 |
|---|---|---|
| Combo 伤害精度 | × 1.0 / × 1.0 / × 1.3 vs × 1.2 / × 1.2 / × 1.5 | `GameConfigSO.combatLight/Heavy/Combo*Mult` |
| Stamina 恢复速度 | 15/秒 vs 20/秒 | `GameConfigSO.combatStaminaRegenPerSec` |
| 织线绑冷却 | 5s vs 3s vs 8s | `GameConfigSO.combatThreadBindCooldownSec` |
| 完美格挡窗口 | 0.1s vs 0.15s | `GameConfigSO.combatPerfectBlockWindowSec` |

→ 这些都是 Prototype 阶段的**数值调参工作**，通过 `GameConfigSO` 直接改即可，不阻塞任何 GDD。

---

## Unity Implementation Notes

### 核心脚本（`Assets/Scripts/Combat/`）
- `CombatManager.cs` —— 战斗状态机
- `PlayerCombatController.cs` —— 玩家 5 动作 + Stamina
- `EnemyAI.cs` —— 4 种敌人 + 警觉范围
- `BossController.cs` —— 3 阶段 + 特殊招式
- `DamageCalculator.cs` —— 伤害公式
- `ComboTracker.cs` —— combo 状态
- `ThreadBindSystem.cs` —— 织线绑定
- `CombatUI.cs` —— 血条 / Stamina / 伤害数字
- `CombatVFX.cs` —— 攻击粒子 / 命中粒子 / 丝线

### 敌人行为状态机
```csharp
public class EnemyAI : MonoBehaviour {
  enum State { Idle, Chasing, Attacking, Recovery, Hit, Dead }
}
```

### Boss 状态机
```csharp
public class BossController : MonoBehaviour {
  enum Phase { Phase1, Phase2, Phase3, Dead }
  enum State { Idle, Attacking, Recovery, Hit, Transitioning, Dead }
}
```

### TimeManager 联动
- 订阅 `TimeManager.OnTimeStateChanged` 重新计算敌人警觉范围
- 订阅 `TimeManager.OnMoonPhaseChanged` 应用月相加成
- Boss 战开始时调用 `TimeManager.FreezeTime()`，结束时 `UnfreezeTime()`

### 事件订阅
```csharp
public class CombatManager : MonoBehaviour {
  public static event Action<DamageInfo> OnDamageDealt;
  public static event Action<Enemy> OnEnemyKilled;
  public static event Action<Boss> OnBossDefeated;
  public static event Action OnPlayerDied;
  public static event Action<Player> OnPlayerHit;
}
```

### 性能预算
- 战斗计算：< 0.5ms / 帧
- 敌人 AI：< 0.1ms / 敌人
- Boss AI：< 0.3ms
- 织线 VFX：< 0.2ms / 帧

---

## 进度

| 章节 | 状态 |
|---|---|
| A. Overview | ✅ |
| B. Player Fantasy | ✅ |
| C. Detailed Design (10 小节) | ✅ |
| D. Formulas (6 个) | ✅ |
| E. Edge Cases (20 种) | ✅ |
| F. Dependencies | ✅ |
| G. Tuning Knobs (23 字段已落 v1.4) | ✅ |
| H. Acceptance Criteria (12 条) | ✅ |
| **10. Locked Decisions (9 决策)** | ✅ |
| Unity Implementation Notes | ✅ |

**总进度**: 10/10 (100%) 🔒

**🔒 已锁定 v1.0** —— 8 段全填 + 9 开放问题全部锁定 + 23 调参字段落 data-config v1.4。

---

## 变更日志

| 日期 | 版本 | 改动 | 责任人 |
|---|---|---|---|
| 2026-07-27 | v1.0 draft | 初版生成：10 段 + 公式 + Unity 实施 | Mavis |
| 2026-07-27 | **v1.0 LOCKED** | 9 开放问题用户拍板全部锁定；data-config v1.4 同步升级 | Mavis + 用户 |
