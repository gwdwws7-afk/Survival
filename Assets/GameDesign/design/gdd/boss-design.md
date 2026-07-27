# Boss Design — System GDD

> **Status**: 🔒 LOCKED v1.0
> **Author**: Mavis + 用户
> **Last Updated**: 2026-07-27
> **Implements Pillar**: **Foundation**（决定 Ravensong 战斗体验"高点"）
> **See Also**: `combat.md`（战斗机制 + 3 阶段 boss 框架）/ `world-exploration.md` §C.2 群系 5（4 boss 位置）/ `oath-system.md` §C.2 誓言 3 / 5（荒野之誓 5.2 / 苍穹之誓 5.2 触发）

---

## A. Overview

**Boss 是 Ravensong 的"战斗体验高点"——4 个 boss，每个 boss 都是"已学技能的考试 + 情绪高峰 + 机制变体 + 叙事节点"的综合设计。** Boss **不**是"更大的血条"（避免成为 1 个 generic action game 的"血牛"），而是**对玩家在 Ravensong 之前学到的技能的"重新组合、扭转和放大"**——每个 boss 都**测试一种不同的战斗风格**。

Ravensong 的 Boss 设计哲学来自 `20_Boss设计.md` 框架：
- **Skill exam**（技能考试）—— boss 是"对玩家已学技能的考试"
- **Move readability**（招式可读性）—— boss 攻击前**有 0.5-1 秒预警**
- **Phase structure**（阶段结构）—— 3 阶段（100-60% / 60-30% / 30-0%）
- **Spectacle vs clarity**（视觉与可读性平衡）—— boss 巨大但**不**阻挡信息
- **True reason for difficulty**（真正的难度原因）—— 不是血量高，是**机制变体**

**4 个 boss 各自测试不同的技能**（避免重复）：
- **骨王**（Bone King）—— **记忆型**（Memory）—— 玩家重学所有战斗技能
- **深渊之主**（Abyss Lord）—— **适应型**（Adaptation）—— 玩家需调整战术
- **寒霜巨人**（Frost Giant）—— **耐久型**（Endurance）—— 玩家需长时间专注
- **神话守卫者**（Mythic Guardian）—— **综合型**（Composite）—— 前面 3 boss 的组合

**3 阶段结构**（与 combat.md `combatBossPhasesCount = 3` 锁定）：
- **P1 (100-60%)** —— 教学期（玩家学 boss 招式）
- **P2 (60-30%)** —— 强化期（boss 招式变体）
- **P3 (30-0%)** —— 终局期（boss 拼尽全力的最后阶段）

**必在夜晚**（world-exploration.md 决策 #1 锁定）：3 个 boss 必在夜晚触发，第 4 boss 可全天候。

数据层由**新增**的 `BossDetailSO`（data-config.md C.2 类型 17）驱动（已有 `BossSO` 类型 6 用于基本定义；`BossDetailSO` 用于**详细阶段设计**）；本 GDD 专注于**4 boss 详细设计、阶段变化、招式可读性、失败归因**。

---

## B. Player Fantasy

### 主幻想
> "我打骨王时，3 分钟过去了我才发现——他用的招式都是之前狼群和尸鬼的'升级版'。这就是 Ravensong：boss 不教你新东西，他让你**重学你已经会的东西**。"

### 关键体验时刻

- **第一次**进入 boss 房：3 min 演出 + 奥丁之眼扫视 + 玩家站在 boss 房中央
- **第一次**boss 阶段转换：P1 → P2 时 boss 站起 / 咆哮 / 屏幕黑 1 秒 + 新视觉
- **第一次**看到 boss 招式预警：boss 抬手 0.5s 蓄力 → 玩家**必须**躲避
- **第一次**boss 死亡：5 秒金色光柱 + 道具掉落 + 永久士气 buff
- **第一次**打 4 boss 全部：解锁苍穹之誓 + 触发奥丁审判

---

## C. Detailed Design

### C.1 4 Boss 共同规则

#### 规则 1：3 阶段结构（与 combat.md 锁定）
- **P1 (100-60%)** —— 教学期
- **P2 (60-30%)** —— 强化期
- **P3 (30-0%)** —— 终局期

#### 规则 2：必在夜晚（3 个 boss）
- 骨王 / 深渊之主 / 寒霜巨人 = **必**夜晚触发
- 神话守卫者 = 全天候可触发
- 玩家在白天进入 boss 房 → 提示"boss 必在夜晚"

#### 规则 3：boss 房演出（与 death-sendoff.md 演出协同）
- 玩家进入 boss 房 → 3 min 演出
- 演出：boss 出现 + 群系色调 + 玩家定位 + 屏幕黑 1 秒
- 演出结束 → boss 战开始

#### 规则 4：boss 招式预警 0.5-1 秒
- 所有 boss 攻击前**必须**有 0.5-1 秒预警
- 预警：蓄力动画 + 红光 / 蓝光 / cyan 光圈
- 玩家**不**需要"背板"——通过视觉即可判断

#### 规则 5：boss 不会逃跑
- 玩家进 boss 房 → boss 战**不可逆**
- 玩家死亡 = boss 战失败
- 玩家退出 boss 房 = boss 战失败（24h 冷却）

#### 规则 6：boss 战无时间压力
- **不**做"限时击杀"机制
- 玩家可以**慢打**（v1.0 决策：避免 soulslike 焦虑）
- 玩家需**重复尝试**学习 boss 招式

#### 规则 7：boss 死亡 = 5-10 秒演出
- 5-10 秒金色光柱 + 道具掉落
- 玩家**不**能跳过
- 演出结束 → 玩家回聚落（v1.0 自动）

#### 规则 8：boss 死亡 = 永久记录
- 4 boss 死亡状态写入 save
- v1.1 决策：NG+ 时 boss 难度 +50%

---

### C.2 Boss 1：骨王（Bone King）⭐ 记忆型

**位置**：白骨原 boss 房
**难度**：⭐⭐（第一个 boss，教学用）
**测试技能**：玩家在前 1-2 周学到的所有基础战斗技能

#### 骨王阶段变化表

| # | 阶段 | 血量% | 新增压力 | 视觉变化 | 机制变化 |
|---|------|------|---------|---------|---------|
| 1 | **P1 教学** | 100-60% | 普通攻击 + 1 招式 | 骷髅王外形 + 红光 | 测试：基础攻击 + 闪避 |
| 2 | **P2 强化** | 60-30% | + 1 召唤（4 骷髅兵）| 骨王站立 + 召唤光环 | 测试：群战 + Fate-Thread Bind（boss 必中）|
| 3 | **P3 终局** | 30-0% | + 全屏冲击波 | 骨王狂暴 + 红雾 | 测试：完美闪避 + 终局爆发 |

#### 骨王招式详解

| 招式 | 阶段 | 预警 | 攻击 | 测试技能 |
|---|---|---|---|---|
| **横扫** | P1 | 抬手 0.5s | 180° 横扫 5 单位 | 闪避 |
| **骨刺** | P1 | 蓄力 0.8s | 5 个骨刺向前 | 走位 |
| **召唤骷髅** | P2 | 咆哮 1s | 4 骷髅兵召唤 | 群战 |
| **骨墙** | P2 | 双手触地 1s | 升起骨墙阻挡 | Fate-Thread Bind 破骨墙 |
| **冲击波** | P3 | 后仰 1s | 全屏冲击波（10 单位）| 完美闪避 + 远程 |

#### 骨王失败归因
- **死在 P1**：玩家没掌握基础闪避 → 重学 combat.md §C.1 基础
- **死在 P2**：玩家没用 Fate-Thread Bind 破骨墙 → 学习战斗主动技能
- **死在 P3**：玩家没远程攻击 → 换 Tier 2-3 武器

#### 骨王叙事
- 骨王是白骨原的"第一个主人"——一个战死的英灵，因为没有被收容而腐化为王
- 战胜骨王 = "让他安息"（参考 death-sendoff 送别机制）
- 5 秒金色光柱 + 送别演出

---

### C.3 Boss 2：深渊之主（Abyss Lord）⭐ 适应型

**位置**：深渊沼 boss 房
**难度**：⭐⭐⭐（第二个 boss，要求玩家适应新环境）
**测试技能**：玩家在深渊沼的"夜魔资源 + 群系特点"经验 + 适应能力

#### 深渊之主阶段变化表

| # | 阶段 | 血量% | 新增压力 | 视觉变化 | 机制变化 |
|---|------|------|---------|---------|---------|
| 1 | **P1 教学** | 100-60% | 暗影爪 + 1 召唤 | 暗物质围绕 | 测试：暗视 + 单目标 |
| 2 | **P2 强化** | 60-30% | + 毒雾（持续伤害）| 深渊之主隐形 50% | 测试：视野管理 + 抗毒 |
| 3 | **P3 终局** | 30-0% | + 全场暗影爆发 | 暗物质实体化 | 测试：终局 + 净化仪式 |

#### 深渊之主招式详解

| 招式 | 阶段 | 预警 | 攻击 | 测试技能 |
|---|---|---|---|---|
| **暗影爪** | P1 | 暗物质聚拢 0.8s | 3 爪击 | 闪避 |
| **暗影步** | P1 | 0.5s 烟雾 | 瞬移到玩家背后 | 视野 |
| **毒雾** | P2 | 0.5s 绿光 | 8 单位毒雾（-2 HP/秒）| 抗毒 / 走位 |
| **隐形** | P2 | 0.8s 暗化 | 50% 隐形 | 视野 / 听声辨位 |
| **暗影爆发** | P3 | 1.5s 蓄力 | 全场暗影爆发 | 净化仪式（gathering 资源）|

#### 深渊之主失败归因
- **死在 P1**：玩家视野管理差 → 提升暗视（深渊沼的夜魔草加成）
- **死在 P2**：玩家没带抗毒消耗品 → 制作 1 个抗毒药水
- **死在 P3**：玩家没准备净化仪式 → 收集 5 个夜魔草 + 1 个神龛

#### 深渊之主叙事
- 深渊之主是被"奥丁的审判"放逐的英灵——他**没有**被奥丁认可
- 战胜深渊之主 = "给奥丁一个交代"
- 5 秒暗物质散去 + 神圣光柱

---

### C.4 Boss 3：寒霜巨人（Frost Giant）⭐ 耐久型

**位置**：永冻崖 boss 房
**难度**：⭐⭐⭐⭐（第三个 boss，要求玩家长时间专注）
**测试技能**：耐久 + 寒冷 debuff 管理 + 篝火使用

#### 寒霜巨人阶段变化表

| # | 阶段 | 血量% | 新增压力 | 视觉变化 | 机制变化 |
|---|------|------|---------|---------|---------|
| 1 | **P1 教学** | 100-60% | 寒冰攻击 + 减速 | 冰甲 + 雪地 | 测试：寒冷 debuff + 篝火 |
| 2 | **P2 强化** | 60-30% | + 冰风暴（全场）| 寒霜巨人狂暴 | 测试：耐久 + 篝火持续 |
| 3 | **P3 终局** | 30-0% | + 冰封世界（地图永久冰）| 寒霜巨人完全冰化 | 测试：终局 + 火焰攻击 |

#### 寒霜巨人招式详解

| 招式 | 阶段 | 预警 | 攻击 | 测试技能 |
|---|---|---|---|---|
| **寒冰爪** | P1 | 寒气聚拢 0.8s | 3 爪击 + 减速 50% | 走位 + 篝火 |
| **冰柱** | P1 | 蓄力 1s | 5 冰柱向前 | 闪避 |
| **冰风暴** | P2 | 0.5s 寒风 | 全场冰风暴（-1 HP/秒）| 篝火持续 |
| **冰甲** | P2 | 0.8s 闪光 | 寒霜巨人加 50% 防御 | Fate-Thread Bind 必中 |
| **冰封世界** | P3 | 1.5s 大蓄力 | 地图永久冰（篝火 -50% 效果）| 火焰攻击 |

#### 寒霜巨人失败归因
- **死在 P1**：玩家没准备篝火 → 永冻崖入口放篝火
- **死在 P2**：玩家篝火频率不够 → 提升篝火质量（gathering 资源）
- **死在 P3**：玩家没带火焰 Tier 3+ 武器 → 编织 1 个火焰剑

#### 寒霜巨人叙事
- 寒霜巨人是 Ravensong 的"寒冰祖先"——他是这个世界的最初居民
- 战胜寒霜巨人 = "世界被温暖覆盖"——篝火燃起
- 5 秒冰化散去 + 篝火燃起 + 永冻崖变暖

---

### C.5 Boss 4：神话守卫者（Mythic Guardian）⭐ 综合型

**位置**：奥丁圣所 boss 房
**难度**：⭐⭐⭐⭐⭐（第 4 个 boss，前面 3 boss 的组合）
**测试技能**：**所有** Ravensong 战斗技能 + 奥丁之眼 + 编织 + 死亡

#### 神话守卫者阶段变化表

| # | 阶段 | 血量% | 新增压力 | 视觉变化 | 机制变化 |
|---|------|------|---------|---------|---------|
| 1 | **P1 综合** | 100-60% | 3 boss 招式混合 | 神话守卫者 + 神光 | 测试：所有基础技能 |
| 2 | **P2 强化** | 60-30% | + 奥丁之眼扫描 | 神话守卫者狂暴 | 测试：奥丁之眼应对 |
| 3 | **P3 终局** | 30-0% | + 神话之怒（全场）| 神话守卫者神化 | 测试：所有 Ravensong 技能 + 终局 |

#### 神话守卫者招式详解

| 招式 | 阶段 | 预警 | 攻击 | 测试技能 |
|---|---|---|---|---|
| **混合三式** | P1 | 0.5s 蓄力 | 横扫 + 暗影 + 寒冰 | 完美闪避 |
| **奥丁之眼** | P2 | 1.5s 红光 | 全屏奥丁之眼 | 奥丁之眼应对（神龛 Level 2）|
| **命运之线** | P2 | 0.8s 丝线 | 召唤玩家的"命运之线" | Fate-Thread 反编织 |
| **神话之怒** | P3 | 2s 大蓄力 | 全场神话冲击波 | 终极闪避 + 篝火 + 神龛 |

#### 神话守卫者失败归因
- **死在 P1**：玩家基础技能不够 → 重学前面 3 boss
- **死在 P2**：玩家没应对奥丁之眼 → 升级神龛到 Level 2
- **死在 P3**：玩家综合能力不够 → v1.0 设计 = 这是**最难的 boss**

#### 神话守卫者叙事
- 神话守卫者是奥丁的"最后试炼"——他**测试**玩家是否准备好接受审判
- 战胜神话守卫者 = "你准备好面对奥丁了"
- 5 秒金色光柱 + 神光 + 奥丁头像出现 → 触发苍穹之誓 + 奥丁审判

---

### C.6 Boss 战共同机制

#### 玩家血量
- 玩家最大 HP = `combatPlayerMaxHP = 100`
- boss 攻击 = 1-2 击秒玩家
- 玩家**必须**有 2-3 个 HP 消耗品

#### 战斗节奏
- 玩家**可以**吃 god-ember 治疗（限制 1 个/分钟）
- 玩家**可以**使用篝火 / 神龛（神龛 30m 内）
- 玩家**可以**切换装备 / 编织

#### 失败重试
- 玩家死亡 = boss 战失败
- 玩家返回聚落
- boss 24h 冷却（不能让玩家刷）
- 玩家**需要**回聚落补给后再挑战

#### 胜负持久化
- boss 死亡 = 永久记录
- boss 房**仍可进入**（看纪念碑）但 boss **不复活**
- v1.1 决策：NG+ 时 boss 复活 + 难度 +50%

---

### C.7 Boss 演出（与 vfx.md / death-sendoff.md 协同）

#### 进入 boss 房演出（3 min）
- 0-30s：奥丁之眼扫视 + 群系色调
- 30-90s：boss 出现 + 玩家定位
- 90-150s：boss 介绍（VFX + Audio）
- 150-180s：玩家准备时间 + 屏幕黑 1 秒 → 战斗开始

#### 阶段转换演出（5-10s）
- 屏幕黑 1 秒
- boss 咆哮 + 站起 / 狂暴
- 视觉变化（新色 + 新光）
- Audio：紧张弦乐 + 鼓点

#### Boss 死亡演出（5-10s）
- 金色光柱
- boss 化光（向上飘）
- 玩家 + 群系变亮 0.5s
- 道具掉落
- Audio：挽歌 + 神圣号角

---

### C.8 Boss 与群系 / 仪式协同

#### Boss 触发
- 玩家**抵达** boss 房 + 夜晚（3 boss）→ 演出 + 战斗
- 玩家**抵达** boss 房 + 白天 → 提示"boss 必在夜晚" + 等待

#### Boss 与誓言里程碑
- **誓言 3.4**（荒野之誓）：击杀 1 个 world boss → 奖励月光武器 +1 tier
- **誓言 5.2**（苍穹之誓）：击杀所有 4 个 world boss → 奖励 Boss 战利品 +1
- **主线任务 7**（骨王的觉醒）：击杀骨王 → 1 Tier 4 装备 + 50 god-ember

#### Boss 与死亡
- boss 战死亡 = 玩家在远征中死亡 → 触发远征失败
- 玩家**不**触发"boss 战死亡送别"（boss 不是英灵）
- 玩家死亡 = 玩家 v1.0 死亡 = 回聚落

#### Boss 与聚落
- 战胜 boss = 聚落永久士气 +5%
- 4 boss 全胜 = 聚落 +20% 士气（永久）

---

### C.9 与其他系统的交互

| 系统 | 怎么用 Boss |
|---|---|
| **Combat** | boss 3 阶段结构（`combatBossPhasesCount = 3`）+ 招式系统 |
| **VFX** | boss 演出 + 阶段变化 + 死亡演出 |
| **VFX-Audio** | boss 演出 3 min + 紧张弦乐 |
| **World-Exploration** | 4 boss 房位置（白骨原/深渊沼/永冻崖/奥丁圣所）|
| **Oath** | 誓言 3.4 / 5.2 触发条件 |
| **Quest-Event** | 主线 7（骨王的觉醒）/ 支线 12（奥丁的试炼）|
| **Death-Send-off** | boss 死亡 = "送别"（骨王 / 深渊之主特殊）|
| **Fate-Thread** | 骨王 P2 骨墙 / 寒霜巨人 P2 冰甲 / 神话守卫者 P2 命运之线 |
| **Settlement** | 战胜 boss = 聚落士气 +5% |

---

## D. Formulas

### D.1 Boss 血量计算
```csharp
float CalculateBossHP(BossDetailSO boss) {
  return boss.baseHP * (1 + boss.difficultyMultiplier);  // 1.0 - 2.0
}
```

### D.2 阶段转换判定
```csharp
void CheckPhaseTransition(Boss boss) {
  float hpRatio = boss.currentHP / boss.maxHP;
  if (hpRatio <= 0.3f && boss.phase != 3) EnterPhase3();
  else if (hpRatio <= 0.6f && boss.phase != 2) EnterPhase2();
}
```

### D.3 招式预警
```csharp
void CastMove(BossMove move) {
  StartCoroutine(ShowWarning(move.warningDurationSec));  // 0.5-1s
  yield return new WaitForSeconds(move.warningDurationSec);
  ExecuteMove(move);
}
```

### D.4 Boss 失败重试
```csharp
void OnPlayerDeathInBoss() {
  boss.enabled = false;
  cooldownHours = 24;  // 24h 冷却
  player.position = settlementSpawnPoint;  // 回聚落
}
```

### D.5 Boss 死亡奖励
```csharp
void OnBossDeath(Boss boss) {
  StartCoroutine(DeathCinematic(5f));  // 5 秒演出
  yield return new WaitForSeconds(5f);
  DropLoot(boss.lootTable);
  settlement.moraleBonus += 0.05f;  // +5%
  OathManager.CompleteMilestone(3, 4);  // 誓言 3.4
}
```

### D.6 Boss 难度系数
```csharp
float GetBossDifficultyMultiplier(BossDetailSO boss) {
  return boss.id switch {
    "boss_bone_king" => 1.0f,         // 教学
    "boss_abyss_lord" => 1.3f,        // 适应
    "boss_frost_giant" => 1.6f,       // 耐久
    "boss_mythic_guardian" => 2.0f,   // 综合
    _ => 1.0f
  };
}
```

---

## E. Edge Cases

| 情况 | 处理 |
|---|---|
| 玩家白天进入 boss 房 | 提示"boss 必在夜晚" + 等待 |
| 玩家死亡在 P1 | boss 战失败 + 24h 冷却 + 回聚落 |
| 玩家死亡在 P3 | boss 战失败 + 24h 冷却 + 回聚落 |
| 玩家用 god-ember 治疗 | 限制 1 个/分钟 |
| 玩家用篝火治疗 | 神龛 30m 内有效 |
| 玩家退出 boss 房 | boss 战失败（24h 冷却）|
| 玩家在 boss 战死亡 | **不**触发 boss 死亡演出 |
| boss 4 全胜后玩家死亡 | NG+ v1.1 决策 = 难度 +50% |
| 玩家在 24h 冷却内尝试 | boss 房**不**可进入 |
| 玩家召唤英灵帮忙 | **不**允许（英灵不参战）|
| Boss 演出中玩家退出 | 演出**继续** + 玩家**不**死 |
| 阶段转换时玩家攻击 | 转换期 1s 无敌（玩家**不**能利用）|
| Boss 死亡时玩家受击 | 死亡演出覆盖受击 |

---

## F. Dependencies

### 上游（这个系统依赖谁）
- **Combat** —— boss 3 阶段结构 + 战斗机制
- **VFX** —— boss 演出 + 阶段变化
- **VFX-Audio** —— boss 演出 3 min 音频
- **World-Exploration** —— 4 boss 房位置
- **Data Config** —— `BossDetailSO` 是新类型 17

### 下游（谁依赖这个系统）
- **Oath** —— 誓言 3.4 / 5.2 触发
- **Quest-Event** —— 主线 7 / 支线 12 触发
- **Death-Send-off** —— boss 死亡送别（骨王 / 深渊之主）
- **Settlement** —— 战胜 boss = 聚落士气 +5%
- **Save** —— 4 boss 死亡状态

---

## G. Tuning Knobs（12 字段）

| 旋钮 | 默认值 | 范围 | 决策编号 | 影响 |
|---|---|---|---|---|
| `bossPhaseCount` | 3 | 1-5 | #1 | boss 阶段数（与 combat.md 锁定 3）|
| `bossBaseHP` | 1000 | 500-2000 | #2 | 骨王基础血量（其他 boss 按难度系数）|
| `bossMoveWarningSec` | 0.8f | 0.3-1.5 | #3 | 招式预警时长 |
| `bossPhaseTransitionSec` | 1.0f | 0.5-2 | #4 | 阶段转换黑屏时长 |
| `bossCooldownHours` | 24f | 0-48 | #5 | boss 战失败冷却 |
| `bossDeathCinematicSec` | 5f | 3-10 | #6 | boss 死亡演出时长 |
| `bossEntryCinematicSec` | 180f | 60-300 | #7 | boss 房演出时长（3 min）|
| `bossGodEmberHealingLimit` | 1 | 0-3 | #8 | god-ember 治疗上限（每分钟）|
| `bossHealingShrineRange` | 30f | 10-50 | #9 | 神龛治疗范围 |
| `bossNightOnly` | true | bool | #10 | boss 必在夜晚（前 3 个）|
| `bossMoraleBonusOnKill` | 0.05f | 0-0.1 | #11 | 战胜 boss 聚落士气加成 |
| `bossDifficultyMultiplierBase` | 1.0f | 0.5-2 | #12 | boss 难度基础系数（骨王）|

---

## H. Acceptance Criteria

### AC-1: 3 阶段结构
- **条件**：玩家触发 boss 战
- **结果**：boss 100-60% / 60-30% / 30-0% 三阶段

### AC-2: 招式预警
- **条件**：boss 攻击
- **结果**：0.5-1 秒预警 + 攻击

### AC-3: 阶段转换黑屏
- **条件**：boss 进入新阶段
- **结果**：1 秒黑屏 + 视觉变化

### AC-4: Boss 死亡演出
- **条件**：boss HP = 0
- **结果**：5-10 秒金色光柱 + 道具掉落

### AC-5: Boss 房演出
- **条件**：玩家进入 boss 房
- **结果**：3 min 演出 + 战斗开始

### AC-6: Boss 必在夜晚
- **条件**：玩家白天进入 boss 房
- **结果**：提示"boss 必在夜晚" + 等待

### AC-7: Boss 失败重试
- **条件**：玩家在 boss 战死亡
- **结果**：24h 冷却 + 回聚落

### AC-8: Boss 战招数多样性
- **条件**：玩家打 P1 → P2 → P3
- **结果**：每阶段有不同招数（参考 4 boss 招式表）

### AC-9: Boss 测试独特技能
- **条件**：玩家打骨王 vs 深渊之主 vs 寒霜巨人 vs 神话守卫者
- **结果**：4 boss 测试不同技能（记忆/适应/耐久/综合）

### AC-10: Boss 死亡送别
- **条件**：骨王 / 深渊之主 死亡
- **结果**：5 秒金色光柱 + 送别演出（与 death-sendoff 协同）

### AC-11: Boss 难度系数
- **条件**：玩家打 4 boss
- **结果**：骨王 1.0× / 深渊之主 1.3× / 寒霜巨人 1.6× / 神话守卫者 2.0×

### AC-12: Boss 死亡永久记录
- **条件**：boss 死亡
- **结果**：4 boss 死亡状态写入 save + 永久消失

---

## 10. 已锁定决策（Locked Decisions）

> 2026-07-27 用户拍板，12 个开放问题全部锁定。已落地为 §G 旋钮 + data-config v2.5。

| # | 决策点 | 锁定值 | 落地位置 |
|---|---|---|---|
| 1 | **boss 阶段数** | **3**（与 combat.md 锁定一致） | §C.1 规则 1 + §G |
| 2 | **骨王基础血量** | **1000**（其他 boss 按难度系数） | §C.2 + §G |
| 3 | **招式预警时长** | **0.8 秒**（足够反应） | §C.1 规则 4 + §G |
| 4 | **阶段转换黑屏** | **1 秒**（明显但不冗长） | §C.1 + §G |
| 5 | **boss 战失败冷却** | **24 真实小时**（避免刷） | §C.1 规则 5 + §G |
| 6 | **boss 死亡演出** | **5 秒** | §C.1 规则 7 + §G |
| 7 | **boss 房演出** | **3 分钟**（仪式感） | §C.7 + §G |
| 8 | **god-ember 治疗上限** | **1 个/分钟**（避免无限治疗） | §C.6 + §G |
| 9 | **神龛治疗范围** | **30 米**（明显但不破坏） | §C.6 + §G |
| 10 | **boss 必在夜晚** | **是**（前 3 个；神话守卫者全天候） | §C.1 规则 2 + §G |
| 11 | **战胜 boss 士气加成** | **+5%** | §C.8 + §G |
| 12 | **boss 难度基础系数** | **1.0×**（骨王为基准） | §C.2 + §G |

### 决策之间的协同

- **#1 + #2 + #12**：3 阶段 + 1000 基础血 + 1.0× 难度 = **"标准 boss 框架"**——骨王是教学 boss
- **#3 + #4 + #5**：0.8s 预警 + 1s 转换 + 24h 冷却 = **"可学习但不可刷"**——玩家有挑战但不被惩罚
- **#6 + #7 + #11**：5s 死亡 + 3min 房演出 + +5% 士气 = **"仪式感"**——boss 战是"重要时刻"
- **#8 + #9 + #10**：1/min 治疗 + 30m 神龛 + 夜晚必 = **"公平战斗"**——玩家有工具但需用对时机

### 4 boss 难度阶梯

| Boss | 难度系数 | 测试技能 | 位置 |
|---|---|---|---|
| 骨王 | 1.0× | 记忆型（基础）| 白骨原 |
| 深渊之主 | 1.3× | 适应型（视野+抗毒）| 深渊沼 |
| 寒霜巨人 | 1.6× | 耐久型（篝火+寒冷）| 永冻崖 |
| 神话守卫者 | 2.0× | 综合型（前面 3 boss 组合）| 奥丁圣所 |

→ 这是 Ravensong 的"boss 学习曲线"——从教学到综合。

### 仍待 playtest 调参（不阻塞 GDD 锁定）

| 待调项 | 候选范围 | 调参位置 |
|---|---|---|
| 4 boss 招式数量 | 当前 4-5 vs 6-8 | `BossDetailSO.phaseXMoves` |
| 阶段转换演出 | 1s vs 2s vs 3s | 视觉/演出调优 |
| Boss 战前准备时间 | 30s vs 1min | playtest 体验 |
| 神龛治疗 vs god-ember 治疗 平衡 | 50/50 vs 70/30 | playtest |
| 4 boss 演出音乐 | 当前 vs 重做 | 资产制作 |

→ 这些都是 Prototype 阶段调参，不阻塞任何 GDD。

---

> 12 个开放问题待用户拍板。

1. **boss 阶段数**
   - 我的推荐：**3**（与 combat.md 锁定）
2. **骨王基础血量**
   - 我的推荐：**1000**（其他 boss 按难度系数）
3. **招式预警时长**
   - 我的推荐：**0.8 秒**（足够反应）
4. **阶段转换黑屏时长**
   - 我的推荐：**1 秒**（明显但不冗长）
5. **boss 战失败冷却**
   - 我的推荐：**24 真实小时**（避免刷）
6. **boss 死亡演出时长**
   - 我的推荐：**5 秒**
7. **boss 房演出时长**
   - 我的推荐：**3 分钟**（仪式感）
8. **god-ember 治疗上限**
   - 我的推荐：**1 个/分钟**（避免无限治疗）
9. **神龛治疗范围**
   - 我的推荐：**30 米**（明显但不破坏）
10. **boss 必在夜晚？**
    - 我的推荐：**是**（前 3 个；神话守卫者全天候）
11. **战胜 boss 聚落士气加成**
    - 我的推荐：**+5%**
12. **boss 难度基础系数**
    - 我的推荐：**1.0×**（骨王为基准）

---

## Unity Implementation Notes

### 核心脚本（`Assets/Scripts/Boss/`）
- `BossManager.cs` —— 4 boss 管理
- `BossController.cs` —— 单个 boss 控制（3 阶段 + 招式）
- `BossPhaseTransition.cs` —— 阶段转换黑屏 + 视觉变化
- `BossMoveController.cs` —— boss 招式（预警 + 攻击 + 测试技能）
- `BossCinematic.cs` —— boss 房演出 3 min
- `BossDeathCinematic.cs` —— boss 死亡演出 5-10s
- `BossArena.cs` —— boss 房（场景边界 + 神龛范围）

### 数据结构
```csharp
public class Boss {
  public BossDetailSO data;               // Boss 详细 SO
  public BossSO baseData;                 // Boss 基础 SO（已有类型 6）
  public int currentPhase;                // 1 / 2 / 3
  public float currentHP;
  public float maxHP;
  public List<BossMove> phase1Moves;
  public List<BossMove> phase2Moves;
  public List<BossMove> phase3Moves;
  public bool isDead;
  public float cooldownHoursRemaining;    // 0-24
}

[Serializable]
public class BossMove {
  public string id;
  public float warningDurationSec;        // 0.5-1s
  public float executionDurationSec;
  public string visualEffect;             // 视觉预警
  public string audioEffect;              // 声音预警
  public string testedSkill;              // 测试技能
  public int phase;                       // 1 / 2 / 3
}
```

### 状态机
```csharp
public enum BossState {
  Cinematic,        // 演出
  Phase1,           // P1
  Phase2,           // P2
  Phase3,           // P3
  Transitioning,    // 阶段转换
  Dying,            // 死亡
  Dead,             // 已死亡
  Cooldown,         // 24h 冷却
}
```

### 事件订阅
```csharp
public class BossManager : MonoBehaviour {
  public static event Action<Boss> OnBossPhaseTransition;
  public static event Action<Boss> OnBossDeath;
  public static event Action<Boss> OnBossDefeated;
  public static event Action<Boss> OnBossCinematicStart;
  public static event Action<Boss> OnBossCinematicEnd;
}
```

### 性能预算
- Boss Controller：< 8ms / 帧
- 招式预警：< 2ms
- 阶段转换：< 16ms（一次性）
- Boss 死亡演出：< 8ms / 帧

### 资产制作（v1.0 关键 Boss）
- **4 Boss 模型**：每个 boss ~30 帧动画（idle / move / attack / phase / death）
- **12 招数 VFX**：每个 boss 3 阶段 × 平均 4 招数 = 12 招
- **4 boss 房场景**：每群系 boss 房 1 个
- **3 min boss 房演出**：每 boss 1 个演出
- **5-10s 死亡演出**：每 boss 1 个
- **总计：~30 套 boss 资产**（模型 + 动画 + VFX + Audio）

---

## 进度

| 章节 | 状态 |
|---|---|
| A. Overview | ✅ |
| B. Player Fantasy | ✅ |
| C. Detailed Design (9 小节) | ✅ |
| D. Formulas (6 个) | ✅ |
| E. Edge Cases (13 种) | ✅ |
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
| 2026-07-27 | v1.0 draft | 初版生成：4 boss × 阶段变化表 + 招式 + 12 决策 | Mavis |
| 2026-07-27 | v1.0 LOCKED | 12 开放问题用户拍板全部锁定；data-config v2.5 同步升级 + 新增 `BossDetailSO`（类型 17）；**修复 P0 缺口**（4 boss 详细设计） | Mavis + 用户 |
