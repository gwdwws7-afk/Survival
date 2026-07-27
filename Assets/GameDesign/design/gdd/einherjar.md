# Einherjar — System GDD ⭐

> **Status**: 🔒 LOCKED v1.0
> **Author**: Mavis + 用户
> **Last Updated**: 2026-07-27
> **Implements Pillar**: ⭐ **Living Hearth（活的炉火）**

---

## A. Overview

**Einherjar 是 Ravensong 的"灵魂库"——你收容的阵亡勇士，是你的家人。** 8 个英灵住在你的聚落，每个有职业、年龄、特质、关系。他们会老、会病、会死——**而你必须决定**他们死后是**送入英灵殿**（获得永久聚落祝福），还是**强留身边**（3-5 天腐化后变尸鬼反噬）。

这是 Ravensong **最情感化的系统**——没有"最优解"，只有"你的价值观选择"。**Living Hearth 支柱**的全部承载。

数据层由 `EinherjarSO`（data-config.md C.2 类型 3）驱动；本 GDD 专注于**招募循环、职业系统、老化/死亡、3-5 天腐化机制、关系、Day-Night 行为**。

---

## B. Player Fantasy

### 主幻想
> "我送走 Bjorn 到英灵殿时，聚落升起一道金光——他化作了炉火的守护。"

### 关键体验时刻
- **第一次**招募：在战区找到阵亡者 → 互动 → "你愿意跟我走吗？" → "是" → 他成为你的英灵
- **第一次**看英灵工作：夜晚聚落内，铁匠敲打、火花飞溅、吟游诗人吟唱——"我的聚落活了"
- **第一次**被告知英灵要死："Eirik 时日无多" UI 弹窗
- **第一次**面对 Eirik 的死亡：挽歌、最终对话、"送他走" or "强留" 的选择
- **第一次**送英灵殿：金色光柱 + 永久 buff 出现（聚落产出 +15%）
- **第一次**强留：3-5 天后看着 Eirik 变尸鬼攻击聚落，**你亲手造成的悲剧**

---


### B.1 FADT 三维分析（叙事性审计）

> 按 FADT 框架（Intention / Perceivable Consequence / Story）显式审计 Einherjar 的"活炉火"叙事。

#### Intention（意图清晰度）

| 玩家常见意图 | 系统支持度 | 断裂点 |
|---|---|---|
| 招募英灵 | 🟢 高 | E 互动 + "你愿意跟我走吗" 弹窗 |
| 维持聚落 8 个英灵 | 🟢 高 | 长屋 L2 容量 = 8 |
| 切换英灵 Profession | 🟡 中 | 24h 冷却提示不显式（建议 §C.3 加 UI 倒计时）|
| 见证英灵 dying | 🟢 高 | 挽歌 + UI 弹窗 + 24h 倒计时 |
| 关系提升 | 🟡 中 | 关系"友好"状态无显式提示（建议 §C.7 加状态栏）|

#### Perceivable Consequence（结果可感知度）

| 玩家行为 | 系统反馈 | 归因清晰度 |
|---|---|---|
| 招募 1 英灵 | 5s 招募 VFX + 长屋 -1 槽 | 🟢 清晰 |
| 切换 Profession | 24h 冷却（不立即生效）| 🟡 模糊（v1.0 决策：避免 UI 过载，建议 v1.1 加进度条）|
| 死亡选择 | 5 选 1 弹窗 | 🟢 清晰 |
| 送英灵殿 | 永久 buff + 纪念碑 | 🟢 清晰 |
| 强留 5/5 | boss 战 + 失去 buff | 🟢 清晰 |
| 关系 +1（友好）| 效率 × 1.2 | 🟢 清晰（生产数据）|

#### Story（可叙述性）

| 玩家经历 | 故事元素 | 可叙述性 |
|---|---|---|
| 第 1 次招募 | "陌生人，我愿追随你" | 🟢 高（greetingLine 播放）|
| 第 1 次看英灵工作 | "我的聚落活了" | 🟢 高（夜间 VFX 燃起）|
| 第 1 次被告知 dying | "Eirik 时日无多" | 🟢 高（挽歌 + 弹窗）|
| 第 1 次送英灵殿 | "聚落升起一道金光" | 🟢 高（金色光柱 + 永久 buff）|
| 第 1 次强留尸鬼化 | "我亲手造成的悲剧" | 🟢 高（5 天视觉渐变 + 攻击）|
| 8 英灵满聚落 | "这是我的 Ravensong" | 🟢 高（篝火 8 头像 + 夜间最美时刻）|

**FADT 审计结论**：Einherjar 在 Intention / Perceivable Consequence / Story 三维**全部高支持**——Living Hearth 支柱的**核心载体**。P1 改进项：Profession 切换冷却 UI 进度条（v1.0 决策：v1.0 = 仅提示，v1.1 加进度条）。

---

### B.1 FADT 三维分析（叙事性审计）

> 按 FADT 框架（Intention / Perceivable Consequence / Story）显式审计 Einherjar 的"活炉火"叙事。

#### Intention（意图清晰度）

| 玩家常见意图 | 系统支持度 | 断裂点 |
|---|---|---|
| 招募英灵 | 🟢 高 | E 互动 + "你愿意跟我走吗" 弹窗 |
| 维持聚落 8 个英灵 | 🟢 高 | 长屋 L2 容量 = 8 |
| 切换英灵 Profession | 🟡 中 | 24h 冷却提示不显式（建议 §C.3 加 UI 倒计时）|
| 见证英灵 dying | 🟢 高 | 挽歌 + UI 弹窗 + 24h 倒计时 |
| 关系提升 | 🟡 中 | 关系"友好"状态无显式提示（建议 §C.7 加状态栏）|

#### Perceivable Consequence（结果可感知度）

| 玩家行为 | 系统反馈 | 归因清晰度 |
|---|---|---|
| 招募 1 英灵 | 5s 招募 VFX + 长屋 -1 槽 | 🟢 清晰 |
| 切换 Profession | 24h 冷却（不立即生效）| 🟡 模糊（v1.0 决策：避免 UI 过载，建议 v1.1 加进度条）|
| 死亡选择 | 5 选 1 弹窗 | 🟢 清晰 |
| 送英灵殿 | 永久 buff + 纪念碑 | 🟢 清晰 |
| 强留 5/5 | boss 战 + 失去 buff | 🟢 清晰 |
| 关系 +1（友好）| 效率 × 1.2 | 🟢 清晰（生产数据）|

#### Story（可叙述性）

| 玩家经历 | 故事元素 | 可叙述性 |
|---|---|---|
| 第 1 次招募 | "陌生人，我愿追随你" | 🟢 高（greetingLine 播放）|
| 第 1 次看英灵工作 | "我的聚落活了" | 🟢 高（夜间 VFX 燃起）|
| 第 1 次被告知 dying | "Eirik 时日无多" | 🟢 高（挽歌 + 弹窗）|
| 第 1 次送英灵殿 | "聚落升起一道金光" | 🟢 高（金色光柱 + 永久 buff）|
| 第 1 次强留尸鬼化 | "我亲手造成的悲剧" | 🟢 高（5 天视觉渐变 + 攻击）|
| 8 英灵满聚落 | "这是我的 Ravensong" | 🟢 高（篝火 8 头像 + 夜间最美时刻）|

**FADT 审计结论**：Einherjar 在 Intention / Perceivable Consequence / Story 三维**全部高支持**——Living Hearth 支柱的**核心载体**。P1 改进项：Profession 切换冷却 UI 进度条（v1.0 决策：v1.0 = 仅提示，v1.1 加进度条）。

---
## C. Detailed Design

### C.1 核心规则（Core Rules）

#### 规则 1：聚落最多 8 个英灵
- 上限硬编码在 `GameConfigSO.maxEinherjarInSettlement = 8`
- 第 9 个会触发"聚落已满"提示
- 玩家需要**先送走或失去一个**才能招新的

#### 规则 2：英灵会老会死
- 招募时设定 `ageAtRecruitment`（25-60 岁）
- `daysToDeath`（如果 `willDie = true`）= 招募后多少天会死
- 大部分英灵有 `willDie = false`（永驻型），少数有 `willDie = true`（剧情型）

#### 规则 3：英灵不会在战斗中死亡
- 战斗是玩家的事
- 英灵只在聚落内活动，**不参与战斗**
- 英灵死亡 = 自然衰老 / 疾病 / 意外

#### 规则 4：英灵死亡是玩家的选择
- **送英灵殿**：永久聚落 buff + 失去工人
- **强留**：3-5 天腐化时间 → 变尸鬼（敌对）+ 失去 buff + 失去工人
- **没有第三选择**

#### 规则 5：英灵是聚落的核心
- 聚落效率 = 所有英灵效率之和
- 没有英灵 = 聚落无产出
- 英灵数量 = 玩家"进度"的指标

#### 规则 6：英灵有情感
- 每个英灵有 personality（性格）
- 玩家对待他们的方式影响 `relationship` 状态
- 关系影响生产效率 ±20%

#### 规则 7：英灵有死亡叙事
- `willDie = true` 的英灵有"挽歌" dialogue
- 死亡时有 `dyingLine`（最后一句话）
- 送走后聚落有 24 小时"哀悼期"（产出 -20%）

### C.2 Recruitment（招募）

#### 招募流程
```
玩家在战区发现"阵亡者"光点
    ↓
按 E 互动
    ↓
"你愿意跟我走吗？"（Einherjar 的 dying wish）
    ↓ 玩家选"是"
[招募动画] 玩家伸出援手，英灵站起
    ↓
英灵加入聚落，自动分配 Profession
    ↓
聚落"炉火"燃起，标志新成员
```

#### 招募条件
- 玩家背包**有空间**（不阻塞）
- 聚落**未满 8 个**
- 英灵**未在玩家聚落**（同 ID 不可重复招募）
- 玩家**在战区**（阵亡者只出现在战斗/远征区域）

#### 招募来源
- **战区发现**：50% 概率
- **远征奖励**：30% 概率
- **特殊事件触发**：20% 概率（如英灵自己来到聚落请求加入）

#### 英灵 8 个槽位的来源
- 5 个**预定义**（有名有姓有名言的剧情英灵）
- 3 个**程序生成**（每次新游戏有不同面孔）

### C.3 Profession System（职业系统）

#### 5 个 Profession
| Profession | 产出 | 工具 | 工作时间 | 工资 |
|---|---|---|---|---|
| **Blacksmith** 铁匠 | 金属制品 / 武器升级 | Forge | 夜班 | 8h |
| **Hunter** 猎人 | Hide / 肉类 | 工作不需要工具 | 夜班 | 6h |
| **Skald** 吟游诗人 | 配方提示 / 隐藏线索 | 表演 | 全天 | 4h |
| **Farmer** 农夫 | 食物 / 草药 | 田地 | 夜班 | 6h |
| **Warrior** 战士 | 聚落防御（未来 v1.1） | 武器 | 夜班 | 8h |

#### Profession 切换
- 玩家可随时切换英灵的 Profession
- 切换有 **24 游戏小时冷却**（英灵需要"学习"新工作）
- 切换不影响 `willDie` 状态

#### Profession 限制
- 每个 Profession 在聚落**只能有 2 个**（避免单一职业过多）
- 例：3 个铁匠 = 提示"铁匠已满，请选其他"

#### Profession 升级
- 英灵在同 Profession 工作 5 游戏日后 → 升级
- 升级后 `workEfficiency` +20%
- 最多升 3 级（base × 1.6）

### C.4 Aging & Death Cycle（老化与死亡循环）⭐

**这是 Ravensong 情感核心**。

#### 老化机制
- 招募时 `ageAtRecruitment` 设定（25-60 岁）
- 每游戏日 +1 岁（感知）
- **不**影响工作能力（纯粹是叙事标记）
- 60+ 岁的英灵在 UI 上有"老年"标记

#### 死亡触发
- `willDie = true` 的英灵在 `daysToDeath` 倒计时归 0 时触发"垂死"事件
- 触发后进入 `dying` 状态
- 玩家**必须**在 24 游戏小时内做出选择（送走 or 强留）

#### 死亡选择 UI
```
[英灵 - Eirik]
[生命最后时刻]

"他躺在炉火旁，目光平静。"

[按钮] 送他到英灵殿（永久祝福聚落）
[按钮] 强留身边（3-5 天后会腐化）

[警告] 选择不可逆
```

#### 死亡后果
- **送英灵殿**：
  - 永久聚落 buff（产出 +X% / 士气 +X%）
  - 英灵"化光"消失
  - 24 小时哀悼期
  - 聚落士气永久 +X
  
- **强留**：
  - 英灵继续"活着" 3-5 天（无工作）
  - 期间**缓慢腐化**（视觉渐变）
  - 3-5 天后变尸鬼（敌对，攻击聚落）
  - 玩家必须**杀死**它
  - 失去潜在 buff + 失去工人 + 永久聚落阴影

### C.5 Death & Send-off 详细机制（⭐ 签名）

**这是 "Wyrd" + "Living Hearth" 双支柱的体现**。

#### 3-5 天腐化时间
- 强留后英灵**不**死，但进入"unstable" 状态
- `GameConfigSO.corpseDecayMinDays = 3` / `MaxDays = 5` 决定腐化时间
- 期间英灵**仍然住在聚落**，但**停止工作**
- 玩家可以**反悔**（在腐化完成前**仍**可以送走）—— 但每过 1 天，腐化等级 +1
- 腐化等级 0 = 健康（可正常送走）
- 腐化等级 1-4 = 渐变（可送走，但需要"治疗仪式"）
- 腐化等级 5 = 尸鬼化（不可逆，必须战斗）

#### 视觉表现
- 腐化等级 0：英灵正常
- 腐化等级 1：眼睛开始发青
- 腐化等级 2：皮肤变灰，动作迟缓
- 腐化等级 3：身上出现裂纹
- 腐化等级 4：周围开始有黑气
- 腐化等级 5：完全尸鬼化，攻击聚落

#### 治疗仪式（v1.1 决策）
- 腐化等级 1-4 时可触发
- 需要消耗 50 god-ember + 1 传说材料
- 成功：英灵恢复正常
- 失败：腐化 +1

#### 强留 vs 送走的命运
| 选择 | 短期 | 长期 | 适合 |
|---|---|---|---|
| **送英灵殿** | 失去工人 | 永久聚落 buff + 永久士气 | "他该去更好的地方" |
| **强留** | 保留工人 3-5 天 | 大概率失去 + 聚落阴影 | "我不想失去他" |

#### 价值观选择
- 玩家有"理性派"（每个都送英灵殿）和"感性派"（每个都强留看看）
- 两种选择**无对错**，只是不同的"游戏风格"
- 但聚落效率会因为"决策风格"长期变化

### C.6 Production & Work（生产与工作）

#### 工作效率公式
```csharp
float CalculateProduction(Einherjar einherjar) {
  float base = einherjar.workEfficiency;          // 0.5 - 2.0
  float professionMult = einherjar.profession.tier; // 1.0 - 1.6
  float relationshipMult = einherjar.relationship;   // 0.8 - 1.2
  float dayNightMult = TimeManager.IsNight() ? 1.5f : 0.5f;
  return base * professionMult * relationshipMult * dayNightMult;
}
```

#### 每日产出
- 每个英灵每天产出 `production / 24` 资源（按小时算）
- 例：铁匠 8 小时工作 × efficiency 1.5 = 12 单位铁锭

#### 资源类型
- 铁匠 → Metal Ore / Refined Metal
- 猎人 → Hide / Meat
- 吟游诗人 → Recipe Hints（不是物质资源）
- 农夫 → Food / Herbs
- 战士 → 防御值（v1.1）

### C.7 Relationships（关系）⭐

**英灵之间的关系**让聚落"活"。

#### 关系类型
- **Friendly**（友好）：+20% 效率
- **Neutral**（中性）：100% 效率
- **Hostile**（敌对）：-20% 效率

#### 关系建立
- 同一聚落生活 3+ 天 → +1 friendship
- 同一 Profession → 偶尔互动
- 玩家"撮合"两个英灵 → 加速
- 死亡/送走 → 整个聚落关系重置

#### 关系冲突（v1.1 决策）
- MVP：关系不影响产出，只影响对话
- v1.1：关系冲突时可能争吵，影响士气

### C.8 Day-Night Behavior

#### 白天（Day 状态）
- 英灵**休息**（效率 × 0.5）
- 视觉：聚落"灰暗"，英灵在长屋睡觉
- 部分英灵在白天做"轻工作"（吟游诗人吟唱、农夫种菜）

#### 夜晚（Night 状态）
- 英灵**全效率工作**（效率 × 1.5）
- 视觉：聚落"灯火通明"，英灵在 Forge / Field / Stage
- 这是 Ravensong 聚落最美的时刻

#### 转换期
- Dawn：英灵"起床"，准备白天活动
- Dusk：英灵"换班"，进入夜班

#### 庇护所效应
- 聚落本身是英灵的庇护所
- 玩家**不用**担心英灵被攻击
- 但如果聚落被外敌攻破 → 英灵可能死（v1.1 决策）

### C.9 Voice Lines（语音/对话）

#### 必备语音
- **greetingLine**（招募时）："谢谢你，陌生人。我愿追随你。"
- **workLine**（工作时喃喃）：吟唱 / 哼歌 / 铁锤声
- **dyingLine**（临终）："炉火... 还在... 燃烧..."
- **valhallaSendLine**（送走时）："谢谢你让我走。聚落... 要继续燃烧。"

#### 触发时机
- 招募：greetingLine 播放 1 次
- 工作：workLine 每天播放 1-3 次（随机）
- 临终：dyingLine 播放 1 次
- 送走：valhallaSendLine 播放 1 次 + 24 小时"幽灵"语音（v1.1）

#### 多个英灵时的协调
- 同时 8 个英灵，**不**会同时说话（避免 UI 混乱）
- 工作时偶尔**对话**（"Eirik 看着 Skald 唱"）

### C.10 与其他系统的交互

| 系统 | 怎么用 Einherjar |
|---|---|
| **Settlement** | 英灵住在聚落；聚落结构影响英灵效率 |
| **Day-Night** | 工作效率 + 行为 |
| **Death-Send-off** | 死亡 + 强留 + 送英灵殿 |
| **Oath** | 部分誓言与英灵相关（亡者之誓 = 送走 X 个英灵） |
| **Save** | 英灵状态持久化（健康、关系、腐化等级） |
| **Inventory** | 战利品 / 产出物 |
| **Gathering** | 农夫 / 猎人补充资源 |
| **Fate-Thread** | 部分配方需特定英灵存在 / 死亡触发 |
| **UI/HUD** | 英灵 UI（头像、状态、关系） |
| **Quest-Event** | 触发新英灵加入事件 |
| **Save** | 8 个英灵 + 关系 + 腐化等级 |

---

## D. Formulas

### D.1 招募判定
```csharp
bool CanRecruit(EinherjarSO einherjar) {
  return player.einherjars.Count < 8
      && !player.einherjars.Any(e => e.einherjarId == einherjar.id);
}
```

### D.2 工作效率
```csharp
float CalculateProduction(Einherjar einherjar) {
  float base = einherjar.workEfficiency;
  float professionMult = einherjar.profession.tierMult;
  float relationshipMult = einherjar.relationship switch {
    Friendly => 1.2f,
    Neutral => 1.0f,
    Hostile => 0.8f,
    _ => 1.0f
  };
  float dayNightMult = TimeManager.IsNight() ? 1.5f : 0.5f;
  return base * professionMult * relationshipMult * dayNightMult;
}
```

### D.3 死亡倒计时
```csharp
void TickDeathCountdown(Einherjar einherjar) {
  if (einherjar.willDie && einherjar.daysInSettlement >= einherjar.daysToDeath) {
    if (einherjar.state == EinherjarState.Alive) {
      einherjar.state = EinherjarState.Dying;
      OnEinherjarDying?.Invoke(einherjar);  // 触发 UI
    }
  }
}
```

### D.4 腐化等级
```csharp
void TickCorpseDecay(Einherjar einherjar) {
  if (einherjar.state == EinherjarState.Unstable) {
    einherjar.decayLevel += 1;
    if (einherjar.decayLevel >= 5) {
      einherjar.state = EinherjarState.Wight;
      OnEinherjarBecameWight?.Invoke(einherjar);
    }
  }
}
```

### D.5 衰悼期
```csharp
float GetSettlementEfficiency(Settlement settlement) {
  float base = baseEfficiency;
  if (settlement.mourningDaysRemaining > 0) {
    base *= 0.8f;  // 衰悼期 -20%
  }
  return base;
}
```

### D.6 Profession 升级
```csharp
void CheckProfessionLevelUp(Einherjar einherjar) {
  if (einherjar.daysInCurrentProfession >= 5 
      && einherjar.professionLevel < 3) {
    einherjar.professionLevel += 1;
    einherjar.workEfficiency *= 1.2f;  // +20%
    einherjar.daysInCurrentProfession = 0;
  }
}
```

### D.7 送英灵殿 buff
```csharp
StatBlock CalculateValhallaBuff(Einherjar einherjar) {
  return einherjar.valhallaBuff;  // 从 EinherjarSO 读
}
```

---

## E. Edge Cases

| 情况 | 处理 |
|---|---|
| 聚落满 8 个，新英灵出现 | 提示"聚落已满，请先送走一个" |
| 玩家在 dying 状态不选择 | 24 小时后**自动**进入 unstable（强留默认） |
| 腐化等级 5 时玩家不在场 | 尸鬼自动生成，玩家回来时已经攻击 |
| 玩家送走 dying 状态英灵 | 正常（dying 状态可送走） |
| 玩家强留 dying 状态英灵 | 不可（必须先选） |
| 玩家改名英灵 | 可以（在 UI 重命名） |
| 关系满 100（friendly） | 触发"亲密对话"（v1.1） |
| 关系 0（hostile） | 英灵**可以**离开聚落（v1.1 决策） |
| 英灵送走后再发现同 ID | **不可**（永久标记 discovered） |
| 多个英灵同时 dying | 每个独立处理 |
| Profession 满 2 个还想换 | 提示"该 Profession 已满" |
| 切换 Profession 期间英灵 dying | 切换冷却期间无法切（dying 优先） |
| 玩家死亡 | 英灵**不**受影响（继续工作） |
| 玩家退出游戏 | 英灵状态持久化（autosave 触发） |
| 1 周目完成 | 全部英灵**保留**到下一周目（v1.1 决策） |
| 强留后英灵腐化 | 24 小时内可反悔；超过需"治疗仪式" |
| 8 个英灵全部死亡 | 聚落产出归零，提示"建立新聚落" |
| 招募后立刻 dying | 不可（dying 至少 3 天后触发） |

---

## F. Dependencies

### 上游（Einherjar 依赖谁）

- **Data Config** —— `EinherjarSO` 数据 + `GameConfigSO.maxEinherjarInSettlement = 8`
- **Day-Night** —— 工作效率 + 行为
- **Save** —— 状态持久化
- **Settlement** —— 聚落结构

### 下游（谁依赖 Einherjar）

- **Death & Send-off** —— 死亡机制
- **Oath** —— 部分里程碑（亡者之誓 = 送走 X 英灵）
- **Inventory** —— 产出物
- **Fate-Thread** —— 特殊配方
- **UI/HUD** —— 英灵 UI
- **World Exploration** —— 招募来源

**Einherjar 是 Ravensong 的"情感核心"**——几乎所有系统都和它有交叉。

---

## G. Tuning Knobs

> 调参字段建议加到 `GameConfigSO`（data-config v1.5 阶段，与 Fate-Thread 一起升级）

| 参数 | 默认值 | 调参影响 |
|---|---|---|
| `maxEinherjarInSettlement` | 8 | 已有（v1.2） |
| `corpseDecayMinDays` | 3 | 已有（v1.0） |
| `corpseDecayMaxDays` | 5 | 已有（v1.0） |
| `professionMaxCount` | 2 | 同 Profession 上限 |
| `professionLevelUpDays` | 5 | 升级所需天数 |
| `professionLevelMax` | 3 | 最大升级等级 |
| `relationshipFriendlyMult` | 1.2f | Friendly 关系效率加成 |
| `relationshipHostileMult` | 0.8f | Hostile 关系效率减值 |
| `mourningPeriodHours` | 24 | 衰悼期小时数 |
| `mourningEfficiencyMult` | 0.8f | 衰悼期 -20% |
| `decayLevelMax` | 5 | 最大腐化等级 |
| `einherjarDayEfficiencyMult` | 0.5f | 白天英灵效率 |
| `einherjarNightEfficiencyMult` | 1.5f | 夜晚英灵效率 |
| `dyingDecisionWindowHours` | 24 | 垂死状态决策窗口 |
| `professionSwitchCooldownHours` | 24 | Profession 切换冷却 |
| `valhallaBuffApplication` | true | 送走是否立即应用 buff |

---

## H. Acceptance Criteria

### AC-1: 招募流程
**测试**：
1. 玩家在战区发现阵亡者
2. 按 E → "你愿意跟我走吗？" → 选"是"
3. **期望**：英灵加入聚落
4. **期望**：聚落英灵数 +1
5. **期望**：英灵开始工作（夜晚）

### AC-2: 8 个英灵上限
**测试**：
1. 招满 8 个英灵
2. 战区再发现 1 个
3. 尝试招募
4. **期望**：提示"聚落已满，请先送走一个"

### AC-3: Profession 工作
**测试**：
1. 1 个铁匠在工作
2. 夜晚 1 小时
3. **期望**：产出 1.5 单位 Metal
4. 白天 1 小时
5. **期望**：产出 0.5 单位 Metal

### AC-4: Profession 限制
**测试**：
1. 已有 2 个铁匠
2. 尝试切英灵到铁匠
3. **期望**：提示"铁匠已满，请选其他"

### AC-5: Profession 升级
**测试**：
1. 1 个英灵在 Profession 工作 5 天
2. **期望**：升级到 Level 2，效率 +20%

### AC-6: 死亡流程
**测试**：
1. 1 个 `willDie = true` 英灵
2. 等到 `daysToDeath` 倒计时归 0
3. **期望**：英灵进入 dying 状态
4. UI 弹"他/她时日无多"提示
5. 24 小时内玩家选"送英灵殿"或"强留"

### AC-7: 送英灵殿
**测试**：
1. dying 英灵 → 选"送英灵殿"
2. **期望**：英灵化光消失
3. **期望**：聚落获得永久 buff（产出 +X%）
4. **期望**：聚落 24 小时衰悼期
5. **期望**：英灵数 -1

### AC-8: 强留
**测试**：
1. dying 英灵 → 选"强留"
2. **期望**：英灵进入 unstable 状态
3. 等 3-5 天
4. **期望**：英灵变尸鬼
5. **期望**：尸鬼攻击聚落
6. 玩家杀死尸鬼
7. **期望**：英灵数 -1，无 buff

### AC-9: 关系影响效率
**测试**：
1. 2 个英灵 Friendly 关系
2. 各自工作
3. **期望**：效率 +20%

### AC-10: Day-Night 影响
**测试**：
1. 8 个英灵，夜晚
2. **期望**：聚落灯火通明，8 个全在岗
3. 白天
4. **期望**：英灵休息，工作效率 -50%

### AC-11: Voice Lines
**测试**：
1. 招募 Eirik
2. **期望**：播放 greetingLine 1 次
3. 夜晚 30 分钟
4. **期望**：播放 workLine 1-3 次

### AC-12: 持久化
**测试**：
1. 招募 3 个英灵
2. Save Game
3. 退出
4. Load Game
5. **期望**：3 个英灵 + 关系 + 腐化等级 + 工作状态都恢复

### AC-13: 性能
**测试**：
1. 8 个英灵同时工作
2. **期望**：每帧 < 0.3ms 计算

---

## 10. 已锁定决策（Locked Decisions）

> 2026-07-27 用户拍板，10 个开放问题全部锁定。已落地为 G 旋钮 + data-config v1.5。

| # | 决策点 | 锁定值 | 落地位置 |
|---|---|---|---|
| 1 | **8 个英灵程序生成** | **5 预定义 + 3 程序**（每次新游戏不同面孔） | §C.2 |
| 2 | **关系可见** | **MVP 可见**（UI 显示，v1.1 加互动） | §C.7 |
| 3 | **英灵被外敌攻击** | **MVP 不**（聚落是安全区，v1.1 加） | §C.1 规则 6 |
| 4 | **英灵离开聚落** | **MVP 不**（永不离开） | §C.10 |
| 5 | **垂死不选默认** | **24h 后默认强留**（有代价） | §C.4 |
| 6 | **willDie 比例** | **80% 永驻 + 20% 剧情死亡** | §C.4 |
| 7 | **送走 buff 累加** | **累加**（送走 3 个 = 3 buff 叠加） | §C.5 |
| 8 | **Profession 切换冷却** | **24 小时**（英灵需"学习"新工作） | §C.3 |
| 9 | **战斗死亡** | **MVP 不**（聚落是安全区，v1.1 加） | §C.1 规则 3 |
| 10 | **强留后 100% 救回** | **可以**（治疗仪式，消耗大） | §C.5 |

### 决策之间的协同

- **#1 + #6**：5 预定义 + 3 程序 + 80% 永驻 = **新游戏总有不同的英灵组合**——预定义保证剧情质量，程序保证新鲜感，永驻减少"必死的哀伤"
- **#2 + #4**：关系可见 + 英灵不离 = **聚落真正"活"**——玩家看着他们的关系变化，看着他们工作，但不会被"英灵跑去探险"的剧情分散注意力
- **#3 + #9**：MVP 不被外敌攻击 + 不战斗死亡 = **聚落是绝对安全区**——英灵是"灵魂库"不是"战士"
- **#5 + #7 + #10**：24h 默认强留 + buff 累加 + 100% 可救回 = **有"代价"但不"无解"**——玩家犯了错可以救回，但每次救回消耗资源

### 仍待 playtest 调参（不阻塞 GDD 锁定）

| 待调项 | 候选范围 | 调参位置 |
|---|---|---|
| 8 英灵比例 | 5+3 vs 6+2 vs 4+4 | `EinherjarSO` 资产数量 |
| 垂死决策窗口 | 24h vs 12h vs 48h | `GameConfigSO.dyingDecisionWindowHours` |
| Profession 升级天数 | 5d vs 7d vs 10d | `GameConfigSO.professionLevelUpDays` |

→ 这些都是 Prototype 阶段的**数值调参工作**，通过 `GameConfigSO` 直接改即可，不阻塞任何 GDD。


---

## Unity Implementation Notes

### 核心脚本（`Assets/Scripts/Einherjar/`）
- `EinherjarManager.cs` —— 8 个英灵管理
- `EinherjarRecruitment.cs` —— 招募流程
- `ProfessionSystem.cs` —— 5 个 Profession 调度
- `AgingSystem.cs` —— 老化 + 死亡倒计时
- `DeathSendOffSystem.cs` —— 死亡 + 强留 + 送英灵殿
- `CorpseDecaySystem.cs` —— 3-5 天腐化
- `RelationshipSystem.cs` —— 关系管理
- `EinherjarUI.cs` —— 英灵 UI（头像、状态、关系）
- `EinherjarVoice.cs` —— 语音播放

### 数据结构
```csharp
public class EinherjarState {
  public string einherjarId;          // 引用 EinherjarSO
  public string displayName;          // 玩家可改名
  public Profession profession;
  public int professionLevel;          // 1-3
  public int daysInSettlement;
  public int daysInCurrentProfession;
  public int daysToDeath;              // -1 if willDie = false
  public int decayLevel;                // 0-5
  public int professionLevelDays;       // 升级累计
  public float health;                  // 0-100
  public Relationship relationship;
  public EinherjarState state;          // Alive / Dying / Unstable / Wight / Valhalla
  public bool isValhallaBlessed;        // 已送走（buff 仍生效）
  public StatBlock valhallaBuff;        // 已应用的 buff
}
```

### 状态机
```csharp
public enum EinherjarState {
  Alive,           // 正常
  Dying,           // 垂死（24h 内决策）
  Unstable,        // 强留中（3-5 天腐化）
  Wight,           // 尸鬼化（敌对）
  Valhalla,        // 已送走（buff 生效，物理消失）
}
```

### 事件订阅
```csharp
public class EinherjarManager : MonoBehaviour {
  public static event Action<EinherjarState> OnEinherjarRecruited;
  public static event Action<EinherjarState> OnEinherjarDying;
  public static event Action<EinherjarState> OnEinherjarSentToValhalla;
  public static event Action<EinherjarState> OnEinherjarBecameWight;
  public static event Action OnSettlementMourning;
}
```

### 性能预算
- 8 个英灵 AI：< 0.1ms / 帧
- 关系更新：< 0.05ms
- 老化 / 腐化 tick：每游戏小时 1 次
- UI 渲染：< 16ms

---

## 进度

| 章节 | 状态 |
|---|---|
| A. Overview | ✅ |
| B. Player Fantasy | ✅ |
| C. Detailed Design (10 小节) | ✅ |
| D. Formulas (7 个) | ✅ |
| E. Edge Cases (20 种) | ✅ |
| F. Dependencies | ✅ |
| G. Tuning Knobs (16 字段已落 v1.5) | ✅ |
| H. Acceptance Criteria (13 条) | ✅ |
| **10. Locked Decisions (10 决策)** | ✅ |
| Unity Implementation Notes | ✅ |

**总进度**: 10/10 (100%) 🔒

**🔒 已锁定 v1.0** —— 8 段全填 + 10 开放问题全部锁定 + 16 调参字段落 data-config v1.5。

---

## 变更日志

| 日期 | 版本 | 改动 | 责任人 |
|---|---|---|---|
| 2026-07-27 | v1.0 draft | 初版生成：10 段 + 公式 + Unity 实施 | Mavis |
| 2026-07-27 | **v1.0 LOCKED** | 10 开放问题用户拍板全部锁定；data-config v1.5 同步升级 | Mavis + 用户 |
| 2026-07-27 | FADT 三表补充 | 按 FADT 框架补充 Intention / Perceivable Consequence / Story 三维分析（提升承诺有重量叙事的显式度） | Mavis + 用户 |
| 2026-07-27 | FADT 三表补充 | 按 FADT 框架补充 Intention / Perceivable Consequence / Story 三维分析（提升承诺有重量叙事的显式度） | Mavis + 用户 |
