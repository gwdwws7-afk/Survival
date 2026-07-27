# Death & Send-off — System GDD ⭐

> **Status**: 🔒 LOCKED v1.0
> **Author**: Mavis + 用户
> **Last Updated**: 2026-07-27
> **Implements Pillar**: ⭐ **Wyrd（命运重量）** + Living Hearth（情感面）
> **See Also**: `einherjar.md` §C.5（英灵腐化机制细节） / `oath-system.md` §C.2 誓言 4（亡者之誓 5 里程碑）

---

## A. Overview

**Death & Send-off 是 Ravensong 的"命运终局"——所有"会死的东西"在 Ravensong 都有"被玩家看到、感受到、决定如何告别"的过程。** 这是 Ravensong 与其他生存游戏**最根本的机制差异**：其他游戏的死亡 = "数据消失"（尸体消失、装备消失、NPC 消失），Ravensong 的死亡 = **"叙事时刻 + 玩家选择 + 永久纪念"**。

**核心动词是 Send-off（送别）**——面对"已经死"或"即将死"的存在，玩家必须**主动选择**如何送它离开。**没有"什么都不做"的选项**——不选就是默认"强留"，而强留有 3-5 天腐化的代价。这是 Ravensong 强迫玩家**面对死亡**的设计哲学。

死亡对象**不限于英灵**——任何"曾经活过、有名字/有 ID 的存在"都可以被送别：
- **英灵**（v1.0 完整机制）—— Living Hearth 支柱的情感主体
- **远征兵**（v1.1 扩展）—— 玩家外派执行任务的伙伴
- **世界 boss / 精英生物**（v1.0）—— 击杀后如何处理尸体也是选择
- **被玩家驯服的野兽**（v1.0）—— 老死后可以送别

**5 种送别方式**覆盖了 Norse 神话对死亡的不同诠释：
- **送英灵殿（Valhalla）**——正典 + 永久聚落 buff
- **战斗葬礼（Battle Sendoff）**——Norse 海盗传统，把尸体送战区让野兽啃食
- **简单葬礼（Simple Burial）**——聚落外墓地的低调安息
- **强留（Refuse to Let Go）**——3-5 天腐化 → 尸鬼（玩家亲手造成的悲剧）
- **让其安息（Let Rest）**——自然腐化不干预，5-7 天自然消失，无尸鬼风险

**Wyrd 支柱**的完整含义是**"你带走的所有人/物的命运都在你手上"**——不是被动承受，而是主动选择。亡者之誓（Oath #4）的 5 个里程碑是这条支柱的"长线化身"。

数据层由**新增**的 `SendoffSO`（data-config.md C.2 类型 10）驱动；本 GDD 专注于**死亡触发、5 种送别方式、纪念碑、送别演出、亡者之誓、衰悼期、Wyrd 锚点**。

---

## B. Player Fantasy

### 主幻想
> "Eirik 临终前的最后一句话，我听到的是'炉火还在燃烧'。我决定送他走。他化光时，整个聚落都静了 3 秒——连夜鸟都不叫。后来聚落中央立起他的小石碑，每次走过我都想起他。"

### 关键体验时刻

- **第一次**看到英灵"衰老"：UI 出现"老年"标记 + 工作效率自然下降
- **第一次**听到"垂死挽歌"：Eirik 躺在炉火旁的吟唱 + 24 小时 UI 弹窗
- **第一次**做 dying 选择：5 选 1（送英灵殿 / 战斗葬礼 / 简单葬礼 / 强留 / 让其安息）
- **第一次**送英灵殿：金色光柱 + Eirik 化光 + 永久 buff 出现 + 24h 衰悼期
- **第一次**强留失败：看着 Eirik 5 天内皮肤变灰、眼睛发青、变尸鬼——**你亲手造成的悲剧**
- **第一次**发现"纪念碑"：聚落中央广场立起 Eirik 的小型石碑（头像 + 名字 + 最后一句话）
- **第一次**完成亡者之誓：5 个里程碑全亮，聚落出现"英灵殿祭坛"+ 1 个新剧情英灵加入
- **第一次**送别 boss：击杀世界 boss 后可选"简单处理"（默认）或"送别仪式"（消耗 god-ember，给额外奖励 + 永久士气 buff）

---


### B.1 FADT 三维分析（叙事性审计）

> 按 FADT 框架（Intention / Perceivable Consequence / Story）显式审计 Death & Send-off 的"承诺有重量"叙事。

#### Intention（意图清晰度）

| 玩家常见意图 | 系统支持度 | 断裂点 |
|---|---|---|
| 5 选 1 送别 | 🟢 高 | 弹窗 5 按钮清晰 + 警告"选择不可逆" |
| 24h 决策窗口 | 🟢 高 | UI 倒计时 + 默认强留提示 |
| 强留 5 天腐化 | 🟢 高 | 视觉渐变（腐化等级 0-5）|
| 送英灵殿 | 🟢 高 | 金色光柱 + 5s 仪式 |
| 立纪念碑 | 🟡 中 | 位置是聚落中央广场，玩家 v1.0 不能自定义（建议 v1.1 决策）|

#### Perceivable Consequence（结果可感知度）

| 玩家行为 | 系统反馈 | 归因清晰度 |
|---|---|---|
| 选送英灵殿 | 永久 buff 立即生效 | 🟢 清晰 |
| 选战斗葬礼 | 野兽攻击 +5%（永久）| 🟢 清晰 |
| 选简单葬礼 | 简易石碑 + 衰悼 | 🟢 清晰 |
| 选强留 | 24h 内可反悔（腐化等级 0）| 🟢 清晰（可补救）|
| 强留 5/5 → 尸鬼化 | boss 战 + 失去 buff + 聚落阴影 | 🟢 清晰（后果明确）|
| 选让其安息 | 5-7 天自然腐化消失 | 🟢 清晰（无风险）|

#### Story（可叙述性）

| 玩家经历 | 故事元素 | 可叙述性 |
|---|---|---|
| 第 1 次送英灵殿 | "Eirik 化光时炉火更亮" | 🟢 高（金色光柱 5-10s）|
| 第 1 次强留尸鬼化 | "我亲手造成的悲剧" | 🟢 高（5 天视觉渐变 + 攻击）|
| 纪念碑出现 | "聚落中央立起 Eirik 的石碑" | 🟢 高（永久物理痕迹）|
| 亡者之誓 5/5 | "我学会了面对死亡" | 🟢 高（Wyrd 锚点激活）|
| 通关后散步聚落 | "我走过 8 个英灵的石碑" | 🟢 高（永久士气 +8%）|

**FADT 审计结论**：Death & Send-off 在 Intention / Perceivable Consequence / Story 三维**全部高支持**——这是 Ravensong "承诺有重量"叙事的**核心承载**。P1 改进项：纪念碑位置 v1.1 可自定义（v1.0 = 固定位置已锁）。

---

### B.1 FADT 三维分析（叙事性审计）

> 按 FADT 框架（Intention / Perceivable Consequence / Story）显式审计 Death & Send-off 的"承诺有重量"叙事。

#### Intention（意图清晰度）

| 玩家常见意图 | 系统支持度 | 断裂点 |
|---|---|---|
| 5 选 1 送别 | 🟢 高 | 弹窗 5 按钮清晰 + 警告"选择不可逆" |
| 24h 决策窗口 | 🟢 高 | UI 倒计时 + 默认强留提示 |
| 强留 5 天腐化 | 🟢 高 | 视觉渐变（腐化等级 0-5）|
| 送英灵殿 | 🟢 高 | 金色光柱 + 5s 仪式 |
| 立纪念碑 | 🟡 中 | 位置是聚落中央广场，玩家 v1.0 不能自定义（建议 v1.1 决策）|

#### Perceivable Consequence（结果可感知度）

| 玩家行为 | 系统反馈 | 归因清晰度 |
|---|---|---|
| 选送英灵殿 | 永久 buff 立即生效 | 🟢 清晰 |
| 选战斗葬礼 | 野兽攻击 +5%（永久）| 🟢 清晰 |
| 选简单葬礼 | 简易石碑 + 衰悼 | 🟢 清晰 |
| 选强留 | 24h 内可反悔（腐化等级 0）| 🟢 清晰（可补救）|
| 强留 5/5 → 尸鬼化 | boss 战 + 失去 buff + 聚落阴影 | 🟢 清晰（后果明确）|
| 选让其安息 | 5-7 天自然腐化消失 | 🟢 清晰（无风险）|

#### Story（可叙述性）

| 玩家经历 | 故事元素 | 可叙述性 |
|---|---|---|
| 第 1 次送英灵殿 | "Eirik 化光时炉火更亮" | 🟢 高（金色光柱 5-10s）|
| 第 1 次强留尸鬼化 | "我亲手造成的悲剧" | 🟢 高（5 天视觉渐变 + 攻击）|
| 纪念碑出现 | "聚落中央立起 Eirik 的石碑" | 🟢 高（永久物理痕迹）|
| 亡者之誓 5/5 | "我学会了面对死亡" | 🟢 高（Wyrd 锚点激活）|
| 通关后散步聚落 | "我走过 8 个英灵的石碑" | 🟢 高（永久士气 +8%）|

**FADT 审计结论**：Death & Send-off 在 Intention / Perceivable Consequence / Story 三维**全部高支持**——这是 Ravensong "承诺有重量"叙事的**核心承载**。P1 改进项：纪念碑位置 v1.1 可自定义（v1.0 = 固定位置已锁）。

---
## C. Detailed Design

### C.1 核心规则（Core Rules）

#### 规则 1：Wyrd 支柱 = 主动选择，不是被动承受
- 任何"会死的东西"死亡时，**必须**有玩家选择
- 24 小时不做选择 → 默认"强留"（v1.0 锁定）
- 不可逆（一旦选了就无法撤销，但 24h 内可改）

#### 规则 2：5 种送别方式覆盖 Norse 死亡神学
- **送英灵殿**（Valhalla）——正典，最强 buff
- **战斗葬礼**（Battle）——Norse 海盗传统
- **简单葬礼**（Burial）——低调安息
- **强留**（Refuse）——3-5 天腐化风险
- **让其安息**（Let Rest）——自然消失，最安全

#### 规则 3：3 类死亡对象（v1.0 完整 / v1.1 扩展）
| 类型 | v1.0 | 描述 | 送别选项 |
|---|---|---|---|
| **英灵** | ✅ 完整 | 自然衰老 / 疾病 | 全部 5 种 |
| **世界 boss** | ✅ 简化 | 玩家击杀 | 简单 / 送别（5 种中 2 种） |
| **被驯服野兽** | ✅ 简化 | 自然老死 | 简单 / 送英灵殿（2 种） |
| **远征兵** | ❌ v1.1 | 任务死亡 | 全部 5 种 |
| **普通生物** | ❌ 不送别 | 击杀后默认尸体消失 | 无 |

#### 规则 4：送别的物理仪式 = 5-15 秒
- 不可跳过（v1.0 锁定）
- 玩家角色无法移动
- 视觉演出 + 音频 + UI 字幕
- 不同送别方式演出不同

#### 规则 5：纪念碑永远存在
- 物理位置在聚落中央广场
- 每个被"送英灵殿"或"简单葬礼"的英灵 = 1 个小石碑
- 走过时士气永久 +1%
- 强留尸鬼化的英灵 = **不立**碑（这是耻辱）

#### 规则 6：亡者之誓的 5 个里程碑（完整版）
> 详见 `oath-system.md` §C.2 誓言 4。本 GDD 简述：
- 4.1：送走 1 个英灵（任何方式）
- 4.2：见证 1 个英灵死亡（不强留）
- 4.3：维持聚落满 8 个英灵 5 天
- 4.4：经历 1 次 3-5 天腐化（不治疗）
- 4.5：送走 3 个英灵到英灵殿

**完成整条誓言**：聚落出现"英灵殿祭坛"（永久建筑）+ 1 个新剧情英灵自动加入 + Wyrd 锚点激活。

#### 规则 7：Wyrd 锚点 = 5/5 亡者之誓的终极 buff
- 永久激活，无法移除
- 聚落所有效率 +10%（v1.0 锁定值）
- 强留英灵**不再腐化**（v1.0 锁定：亡者之誓完成 = 玩家已"学会面对"，强留不再是悲剧）
- 这是 Living Hearth 支柱给 Wyrd 的"回响"

#### 规则 8：衰悼期 = 24h 全聚落 -20%
- 任何英灵被送走（任何方式）后触发
- 吟游诗人自动播放挽歌
- 聚落色调变冷（VFX）
- 24h 后自动恢复

---

### C.2 死亡对象（Who Dies）

#### 类型 1：英灵（v1.0 完整机制）
- **详见 `einherjar.md` §C.5**
- 触发：`willDie = true` 的英灵 `daysToDeath` 倒计时归 0
- 状态机：`Alive → Dying（24h 决策窗口）→ Send-off | Unstable | Rest`
- 送别选项：5 种全部可用

#### 类型 2：被驯服野兽（v1.0 简化）
- 触发：野兽 `ageDays > maxLifespan` 后**老死**
- 状态机：`Alive → Resting（48h）→ Dead → Send-off 决策`
- 送别选项：仅 2 种（送英灵殿 / 简单葬礼）—— 野兽不立碑
- 玩家没有驯服野兽 = 此类型不存在

#### 类型 3：世界 boss（v1.0 简化）
- 触发：玩家击杀 boss 触发 `OnBossKilled` 事件
- 玩家**立即**面对送别选择（不是 24h 倒计时）
- 送别选项：2 种（简单处理默认 / 战斗葬礼 +god-ember 消耗）
- 战斗葬礼奖励：永久聚落士气 +5% + 1 个 Tier 4 装备
- 简单处理：无奖励
- **不立** boss 纪念碑（太大了），但聚落 VFX 保留 24h"战斗印记"

#### 类型 4：远征兵（v1.1 扩展）
- 触发：远征任务失败 / 远征中遭遇意外
- v1.0：**不存在此类型**（远征兵系统本身在 v1.1）
- 预留接口：`SendoffSO.targetType = Expeditioner` 字段保留

#### 类型 5：普通生物（v1.0 不送别）
- 玩家击杀后尸体自动消失
- 无任何送别机制
- 这是设计哲学：普通生物 = "无名字的存在"，不需要告别

---

### C.3 死亡触发（When）

#### 触发源 1：自然衰老（英灵）
- 招募时设定 `ageAtRecruitment`（25-60 岁）
- 每游戏日 `daysInSettlement` +1
- `willDie = true` 的英灵在 `daysToDeath` 倒计时归 0 时触发
- 倒计时范围：30-180 游戏日（取决于英灵）

#### 触发源 2：疾病 / 意外（英灵）
- v1.0 **不**实现（v1.1 决策）
- 预留 `EinherjarSO.suddenDeathChance` 字段（= 0 in v1.0）

#### 触发源 3：玩家击杀（boss / 野兽 / 普通生物）
- 玩家击杀触发 `OnKilled` 事件
- 野兽 `ageDays > maxLifespan` 触发老死
- boss 立即触发送别选择
- 普通生物 = 无送别

#### 触发源 4：远征失败（v1.1）
- 远征兵 v1.1 才有
- 死亡时立即触发送别选择

---

### C.4 5 种送别方式（Send-off Types）⭐

**这是 Death & Send-off GDD 的核心。**

#### 方式 1：送英灵殿（Valhalla Send-off）⭐ 最正典

**这是 Norse 神话对"战士死后归宿"的经典诠释**——奥丁的英灵殿接纳战死的勇士。

**前置条件**：英灵必须 `willDie = true`（剧情英灵）或玩家**主动**对 `willDie = false` 英灵使用（消耗 100 god-ember）

**执行流程**：
```
玩家在 dying 选择 UI 选"送英灵殿"
    ↓
[5 秒仪式动画]
- Eirik 躺在炉火旁
- 头顶出现奥丁之眼金色符文
- 一道金色光柱从天空落下
- Eirik 化光（粒子效果）
- 字幕："他去了英灵殿。聚落因他而更强。"
    ↓
[永久 buff 生效]
- `EinherjarSO.valhallaBuff` 应用到聚落
- 24h 衰悼期开始
- 聚落中央广场立起 Eirik 纪念碑
    ↓
[亡者之誓进度]
- 4.1（送走 1 个英灵）+1
- 4.5（送走 3 个英灵到英灵殿）+1（仅这种送别计数）
```

**永久 buff（来自 `EinherjarSO.valhallaBuff`）**：
- 聚落产出 +10-20%（取决于英灵 Profession）
- 士气 +X
- 持续：永久

**纪念碑内容**：
- 小型石碑（视觉）
- Eirik 头像（spritesheet）
- "炉火还在燃烧"（最后一句话，从 `EinherjarSO.valhallaSendLine`）
- 送走日期

#### 方式 2：战斗葬礼（Battle Sendoff）⭐ Norse 传统

**Norse 海盗传统**——把尸体送回战区，让野兽啃食。这是 Vikings 真实历史的风俗，相信这样战士的灵魂能"再战斗一次"。

**前置条件**：仅英灵可用（v1.0）

**执行流程**：
```
玩家选"战斗葬礼"
    ↓
玩家把 Eirik 抬到战区（物理移动，10-15 秒）
    ↓
[5 秒仪式]
- Eirik 被放在战区中心
- 周围野兽被吸引过来（不会攻击玩家，只围住尸体）
- Eirik 化光消失
- 字幕："他的灵魂与战场的野兽一同长眠。"
    ↓
[奖励]
- 永久 buff：玩家对野兽攻击伤害 +5%
- 持续：永久
- **不立**纪念碑（v1.0 决策：Norse 传统认为"战死之人不应有碑"）
- 亡者之誓 4.1 +1
```

**v1.0 决策原因**：战斗葬礼 = 强战斗向玩家的选择，奖励偏战斗；正典向玩家更倾向送英灵殿。

#### 方式 3：简单葬礼（Simple Burial）

**最朴素的告别**——聚落外的墓地下葬。

**前置条件**：英灵 / 野兽

**执行流程**：
```
玩家选"简单葬礼"
    ↓
[3 秒动画]
- 玩家挖坑 + 放下尸体 + 掩埋
- 聚落吟游诗人吟唱挽歌
- 字幕："他安息了。"
    ↓
[奖励]
- **无**永久 buff（朴素）
- **立**简易石碑（只有名字，无头像、无最后一句话）
- 亡者之誓 4.1 +1
- 24h 衰悼期
```

**v1.0 决策原因**：简单葬礼 = "尊重但不张扬"的选择，给"中立派"玩家一个无负担选项。

#### 方式 4：强留（Refuse to Let Go）⚠️ 风险选项

**最情感化的选择**——"我不想失去他"。但 Ravensong 不允许纯粹"完美保留"——强留有代价。

**前置条件**：仅英灵

**执行流程**：
```
玩家选"强留"
    ↓
Eirik 留在聚落，进入 `Unstable` 状态
    ↓
[3-5 天腐化期]
- 视觉渐变（详见 einherjar.md §C.5）
- Eirik **不**工作
- 玩家可以"反悔"（在腐化完成前**仍**可以送走）——但每过 1 天，腐化等级 +1
- 腐化等级 0-4：可送走（需"治疗仪式"v1.1）
- 腐化等级 5：尸鬼化，不可逆
    ↓
[腐化完成]
- 尸鬼化：Eirik 变敌对生物，攻击聚落
- 玩家**必须**杀死他
- 失去：潜在 buff + 工人 + 永久聚落阴影
- 亡者之誓 4.1 **不**计数（未"送走"）
- 亡者之誓 4.2（见证 1 个英灵死亡）+1
- 亡者之誓 4.4（经历 1 次腐化）+1
```

**v1.0 决策原因**：强留 = Ravensong 最"悲剧向"的选择，3-5 天腐化期强迫玩家**亲眼看着后果**。

#### 方式 5：让其安息（Let Rest）⭐ 最安全选项

**最"放手"的选择**——不做任何仪式，让尸体自然腐化。

**前置条件**：英灵 / 野兽

**执行流程**：
```
玩家选"让其安息"
    ↓
[无仪式，0 秒]
- Eirik 尸体被放置在聚落外
- 5-7 天自然腐化消失
- **无**任何视觉演出
    ↓
[奖励]
- **无**永久 buff
- **无**纪念碑
- 亡者之誓 4.1 +1
- **无**衰悼期（玩家已"放手"）
- 亡者之誓 4.2（见证死亡）+1
```

**v1.0 决策原因**：让其安息 = "完全放手"，给"理性派"玩家一个"无负担送走"选项；与强留形成"过度挽留 vs 完全放手"的对比。

---

### C.5 亡者之誓（Death Oath）⭐ 完整内容

> 亡者之誓是 Wyrd 支柱的"长线化身"。5 个里程碑分 3 类：
> - **送走类**（4.1 / 4.5）：鼓励玩家做"完成告别"
> - **见证类**（4.2）：鼓励玩家接受死亡
> - **坚持类**（4.3 / 4.4）：强迫玩家长期面对

#### 里程碑 4.1：送走 1 个英灵（任何方式）
- **完成条件**：任意 5 种送别方式中任一种
- **奖励**：治疗仪式 +1 次（v1.1）
- **设计意图**：最低门槛的里程碑，**所有**玩家都能完成

#### 里程碑 4.2：见证 1 个英灵死亡（不强留）
- **完成条件**：英灵死亡（任何方式）且玩家**未**强留
- **覆盖**：送英灵殿 / 战斗葬礼 / 简单葬礼 / 让其安息
- **不**包括：强留 + 尸鬼化（因为那是"玩家造成的"死亡，不是"见证"）
- **奖励**：永久 buff：英灵死亡时获得额外 50 god-ember
- **设计意图**：强迫玩家"面对"死亡的必然性

#### 里程碑 4.3：维持聚落满 8 个英灵 5 天
- **完成条件**：连续 5 个游戏日聚落 = 8/8 英灵
- **奖励**：1 个新剧情英灵到来（自动招募）
- **设计意图**：让玩家"留住人"的能力

#### 里程碑 4.4：经历 1 次 3-5 天腐化（不治疗）
- **完成条件**：英灵 `Unstable` 状态持续到 5/5 腐化等级（不强送走）
- **奖励**：永久 buff：英灵腐化 +50% 时间（变成 4.5-7.5 天）——给"我再试一次"机会
- **设计意图**：让玩家"看到后果"——不治疗是 Learning Experience

#### 里程碑 4.5：送走 3 个英灵到英灵殿
- **完成条件**：3 次**仅**"送英灵殿"送别（其他 4 种**不**计数）
- **奖励**：永久 buff：英灵阶位最高 4 级（v1.1，v1.0 上限 3）
- **设计意图**：让玩家选"最正典"送别的回报最高

#### 亡者之誓完成奖励
- 聚落出现"英灵殿祭坛"（永久建筑）
- 1 个新剧情英灵自动加入（"英灵殿的守护者"）
- **Wyrd 锚点激活**：永久 buff（详见规则 7）

---

### C.6 纪念碑（Memorials）⭐ 聚落遗产

**送别不应只是"那一刻"——应该在物理空间留下痕迹。**

#### 物理位置
- 聚落中央广场的"纪念碑区"
- 初始 = 0 块
- 最多 = 8 块（聚落英灵上限）
- v1.1 扩展 = 16 块（远征兵可立碑）

#### 视觉
- **送英灵殿 / 战斗葬礼 / 简单葬礼** → 立碑
  - 大小：小（0.5×1 米）
  - 材质：石质 + 卢恩符文
  - 内容：头像 / 名字 / Profession / 最后一句话 / 送走日期
- **让其安息** → **不立**碑
- **强留 + 尸鬼化** → **不立**碑（这是耻辱）
- **强留 + 反悔送走** → 立碑（与送英灵殿相同）
- **boss / 普通生物** → **不立**碑

#### 机制效果
- 每个纪念碑 = 永久士气 +1%
- 8 个纪念碑 = 8% 士气（v1.0 锁定）
- 走过时：UI 字幕"你想起了 {name}"（每年一次）
- 死亡英灵的"幽灵语音"v1.1 决策

#### 视觉 / VFX
- 夜间纪念碑区有蓝色卢恩符文发光
- 吟游诗人经过时会"对话"（v1.1）

---

### C.7 送别演出（Sendoff Cinematic）

#### 演出规格
| 送别方式 | 时长 | 不可跳 | VFX | SFX |
|---|---|---|---|---|
| **送英灵殿** | 10 秒 | ✅ | 金色光柱 + 化光 | 奥丁号角 + 挽歌 |
| **战斗葬礼** | 5 秒 | ✅ | 野兽聚集 + 化光 | 野兽低吼 + 战斗号角 |
| **简单葬礼** | 5 秒 | ✅ | 挖坑 + 掩埋 | 挖土声 + 吟游诗人吟唱 |
| **强留** | 3 秒 | ✅ | 英灵躺下（停止工作） | 沉默 + 炉火减弱 |
| **让其安息** | 0 秒 | n/a | 无（直接放置） | 无 |

#### 演出细节
- 玩家角色**不可**移动 / 攻击
- 演出期间**不**触发任何 UI 弹窗
- 演出后**立即**应用 buff + 衰悼期
- 演出期间**不**自动保存（演出完成后保存）

#### v1.0 决策：不可跳过的原因
- 送别是 Ravensong 的"情感核心"——跳过 = 失去叙事力量
- 演出只有 3-10 秒，不会破坏 flow

---

### C.8 衰悼与恢复（Mourning & Recovery）

#### 衰悼期机制
- 触发：任何英灵被送走（5 种中**前 4 种**，因为第 5 种"让其安息"无仪式 → 无衰悼）
- 时长：24 游戏小时
- 效果：
  - 聚落效率 -20%（`settlementEfficiency * 0.8`）
  - 吟游诗人**自动**播放挽歌（覆盖 workLine）
  - 聚落色调 VFX 变冷（蓝色 +20%）
  - **不**影响聚落建筑功能

#### 恢复
- 24h 后自动结束
- 吟游诗人恢复 workLine
- 色调恢复正常
- 效率恢复正常
- 衰悼期间**又**送走一个英灵 → 重置 24h 倒计时

#### 24h 决策窗口的同步
- 英灵 dying 状态 = 24h 决策窗口
- 玩家**未**做选择 → 默认"强留"（v1.0 锁定决策）
- 24h 倒计时**与**衰悼期是**两个**独立系统（dying 倒计时是 dying → send-off，衰悼是 send-off 后）

---

### C.9 与其他系统的交互

| 系统 | 怎么用 Death-Send-off |
|---|---|
| **Einherjar** | `EinherjarSO.valhallaBuff` / `valhallaSendLine` / `dyingLine` 字段在送别时使用 |
| **Settlement** | 纪念碑区是聚落建筑（v1.1 决策：MVP 是默认结构，v1.1 可自定义） |
| **Day-Night** | 夜间纪念碑发光；挽歌在夜间最清晰 |
| **Oath** | 亡者之誓 5 里程碑驱动本系统；Wyrd 锚点是亡者之誓 5/5 奖励 |
| **Save** | 纪念碑状态 / 亡者之誓进度 / 衰悼期倒计时都需持久化 |
| **VFX-Audio** | 送别演出 + 挽歌 + 纪念碑 VFX |
| **Fate-Thread** | 部分英灵的"送别"可触发隐藏配方（v1.1 决策） |
| **UI/HUD** | dying 弹窗 / 纪念碑 UI / 亡者之誓进度条 |
| **Quest-Event** | 部分事件 = 特定英灵的死亡（如"瘟疫事件"） |

---

## D. Formulas

### D.1 死亡倒计时（英灵）
```csharp
void TickDeathCountdown(Einherjar einherjar) {
  if (einherjar.willDie && einherjar.daysInSettlement >= einherjar.daysToDeath) {
    if (einherjar.state == EinherjarState.Alive) {
      einherjar.state = EinherjarState.Dying;
      OnEinherjarDying?.Invoke(einherjar);  // 触发 24h 决策窗口 UI
    }
  }
}
```

### D.2 腐化等级（强留）
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

### D.3 24h 决策窗口倒计时
```csharp
void TickDyingWindow(Einherjar einherjar) {
  if (einherjar.state == EinherjarState.Dying) {
    einherjar.dyingHoursRemaining -= TimeManager.realHoursPerGameHour;
    if (einherjar.dyingHoursRemaining <= 0) {
      // 默认强留
      ApplyRefuseToLetGo(einherjar);
    }
  }
}
```

### D.4 纪念碑士气加成
```csharp
float CalculateMemorialBonus(Settlement settlement) {
  return settlement.memorials.Count * 0.01f;  // 每个 +1%
}
```

### D.5 亡者之誓里程碑判定
```csharp
void CheckDeathOathMilestones() {
  if (AnyEinherjarSentOff() && !milestonesCompleted.Contains(4.1f)) {
    CompleteMilestone(4.1f);
  }
  if (AnyEinherjarDiedNotRefused() && !milestonesCompleted.Contains(4.2f)) {
    CompleteMilestone(4.2f);
  }
  // ... 4.3 / 4.4 / 4.5 同理
}
```

### D.6 衰悼期效率惩罚
```csharp
float GetSettlementEfficiency(Settlement settlement) {
  float base = baseEfficiency;
  if (settlement.mourningHoursRemaining > 0) {
    base *= 0.8f;  // -20%
  }
  return base;
}
```

### D.7 Wyrd 锚点效果
```csharp
StatBlock GetWyrdAnchorBuff() {
  if (deathOathCompleted) {
    return new StatBlock {
      settlementEfficiency = 1.10f,  // +10%
      // 强留英灵不再腐化（其他系统检查这个）
    };
  }
  return StatBlock.Zero;
}
```

### D.8 送别选择概率分布（设计期望）
```csharp
// 期望分布（基于 playtest 假设）
float ExpectedDistribution() {
  // 送英灵殿: 50% (正典)
  // 战斗葬礼: 10% (战斗向)
  // 简单葬礼: 25% (中立)
  // 强留: 10% (情感向, 大部分会反悔)
  // 让其安息: 5% (理性派)
}
```

---

## E. Edge Cases

| 情况 | 处理 |
|---|---|
| 玩家在 dying 状态不选择（24h 后） | **自动**强留（v1.0 锁定） |
| 强留英灵腐化到 5/5 → 尸鬼 | 不可逆，触发 boss 战（v1.0 简化版：直接击杀） |
| 玩家**所有**8 个英灵都死了 | 聚落空 → 触发"重建聚落"事件（v1.1 决策，v1.0 仅提示） |
| 亡者之誓完成时聚落空 | Wyrd 锚点**仍**激活（无聚落效率可加） |
| 强留英灵时玩家退出游戏 | 退出 → 重进：腐化等级**保留**（持久化） |
| 强留英灵被腐化到 5/5 后反悔 | 不可逆，必须战斗击杀 |
| 玩家在 24h 决策窗口内切换游戏 | 倒计时**不**暂停（按 real time 算） |
| 玩家**反复**在 5 种送别间切换（24h 内） | v1.0 锁定：24h 内**无**切换次数限制（"最后一次点才算"） |
| boss 击杀时玩家立即退游戏 | boss 死亡**已**持久化，玩家回来时"送别"提示仍在 |
| 野兽被驯服后老死 | 简化送别（2 选 1） |
| 纪念碑数量达到 8 个 | **不**再立新碑，新送别的英灵"用 Eirik 的旧碑更新"（v1.0 决策：避免聚落过度拥挤） |
| 玩家 v1.0 没有远征兵系统 | 远征兵送别是 v1.1，v1.0 UI 上**不**显示此选项 |
| 玩家**同时**有多个 dying 英灵 | 24h 决策窗口**独立**（每个英灵独立倒计时） |
| 玩家**未**送走任何英灵就完成 4 条誓言 | 苍穹之誓仍可解锁（亡者之誓**未**完成） |

---

## F. Dependencies

### 上游（这个系统依赖谁）
- **Einherjar** —— 英灵是死亡对象 1，`EinherjarSO` 提供 `valhallaBuff` / `valhallaSendLine` / `dyingLine`
- **Day-Night** —— 24h 决策窗口使用 `realSecondsPerGameHour`
- **Data Config** —— `SendoffSO` 是新类型 10，`GameConfigSO` 新增字段

### 下游（谁依赖这个系统）
- **Oath** —— 亡者之誓 5 里程碑
- **Settlement** —— 纪念碑区 + 衰悼期
- **VFX-Audio** —— 送别演出 + 挽歌
- **UI/HUD** —— dying 弹窗 + 纪念碑 UI
- **Save** —— 亡者之誓进度 + 纪念碑 + 衰悼期倒计时

---

## G. Tuning Knobs（12 字段）

| 旋钮 | 默认值 | 范围 | 决策编号 | 影响 |
|---|---|---|---|---|
| `dyingDecisionWindowHours` | 24f | 12-48 | #2 | dying 状态决策窗口时长 |
| `dyingDefaultChoice` | Refuse | enum | #2 | 24h 未选默认行为 |
| `sendOffCinematicSkipEnabled` | false | bool | #3 | 送别演出可跳过 |
| `memorialMoodBonusPerMemorial` | 0.01f | 0-0.05 | #4 | 每个纪念碑士气加成 |
| `memorialMaxCount` | 8 | 1-16 | #4 | 聚落纪念碑上限 |
| `mourningEfficiencyPenalty` | 0.2f | 0-0.5 | #5 | 衰悼期效率惩罚 |
| `mourningDurationHours` | 24f | 12-48 | #5 | 衰悼期时长 |
| `wyrdAnchorEfficiencyBonus` | 0.1f | 0-0.3 | #6 | Wyrd 锚点聚落效率加成 |
| `wyrdAnchorStopsDecay` | true | bool | #7 | Wyrd 锚点阻止强留腐化 |
| `battleSendoffGodEmberCost` | 0 | 0-50 | #8 | 战斗葬礼消耗（v1.0 = 0 = 免费） |
| `letRestDurationDays` | 5-7 | 3-10 | #9 | 让其安息自然腐化时长 |
| `deathOathMilestoneProgressBar` | true | bool | #10 | 亡者之誓进度条可见 |

---

## H. Acceptance Criteria

### AC-1: 死亡倒计时触发
- **条件**：英灵 `daysInSettlement >= daysToDeath` 时
- **结果**：UI 弹 dying 弹窗 + 24h 倒计时 + Eirik 状态变 `Dying`

### AC-2: 5 种送别选项可见
- **条件**：dying 状态
- **结果**：UI 显示 5 个按钮（送英灵殿 / 战斗葬礼 / 简单葬礼 / 强留 / 让其安息）

### AC-3: 24h 决策窗口
- **条件**：dying 状态 24h 过去
- **结果**：自动强留（v1.0 锁定）

### AC-4: 送英灵殿演出
- **条件**：玩家选"送英灵殿"
- **结果**：10 秒演出 + 永久 buff + 纪念碑 + 亡者之誓 4.1/4.5 进度

### AC-5: 战斗葬礼演出
- **条件**：玩家选"战斗葬礼"
- **结果**：5 秒演出 + 野兽聚集 + 永久 buff + **不立**碑

### AC-6: 简单葬礼演出
- **条件**：玩家选"简单葬礼"
- **结果**：5 秒演出 + 简易石碑 + 亡者之誓 4.1 进度

### AC-7: 强留机制
- **条件**：玩家选"强留"
- **结果**：英灵进入 `Unstable` + 3-5 天腐化 + 可反悔（腐化 < 5）

### AC-8: 腐化完成尸鬼化
- **条件**：腐化等级 = 5
- **结果**：英灵变敌对生物 + 攻击聚落 + 玩家击杀

### AC-9: 让其安息
- **条件**：玩家选"让其安息"
- **结果**：5-7 天自然腐化消失 + 无衰悼期 + 亡者之誓 4.1/4.2 进度

### AC-10: 纪念碑永久
- **条件**：任何英灵被送英灵殿/战斗葬礼/简单葬礼
- **结果**：聚落中央立碑 + 永久士气 +1%

### AC-11: 亡者之誓 5/5
- **条件**：4.1-4.5 全部完成
- **结果**：英灵殿祭坛出现 + 新英灵加入 + Wyrd 锚点激活

### AC-12: Wyrd 锚点效果
- **条件**：亡者之誓 5/5
- **结果**：聚落效率 +10% + 强留英灵**不再**腐化

---

## 10. 已锁定决策（Locked Decisions）

> 2026-07-27 用户拍板，12 个开放问题全部锁定。已落地为 §G 旋钮 + data-config v1.7。

| # | 决策点 | 锁定值 | 落地位置 |
|---|---|---|---|
| 1 | **24h 决策窗口默认** | **24h 后自动强留**（强迫面对） | §C.1 规则 1 + 规则 3 |
| 2 | **送别演出可跳过** | **不可跳过**（保留情感） | §C.7 |
| 3 | **战斗葬礼 god-ember** | **0**（v1.0 免费，鼓励尝试） | §C.4 方式 2 |
| 4 | **纪念碑数量上限** | **8 块**（=英灵上限） | §C.6 |
| 5 | **衰悼期效率惩罚** | **-20%**（明显但不致命） | §C.8 + §G |
| 6 | **Wyrd 锚点加成** | **+10%**（明显的完成奖励） | §C.1 规则 7 |
| 7 | **Wyrd 锚点阻止强留腐化** | **是**（"已学会面对"） | §C.1 规则 7 |
| 8 | **让其安息时长** | **5-7 天**（不短不长） | §C.4 方式 5 |
| 9 | **亡者之誓进度条** | **可见**（透明） | §C.5 + §G |
| 10 | **纪念碑满 8 后** | **用旧碑更新**（不拥挤） | §E Edge Cases |
| 11 | **强留+反悔治疗仪式** | **v1.0 不实现**（v1.1） | §C.1 + v1.1 决策 |
| 12 | **boss 送别** | **2 选 1**（简单 / 战斗） | §C.2 类型 3 |

### 决策之间的协同

- **#1 + #5 + #6 + #7**：24h 强留 + 衰悼 -20% + Wyrd +10% + 阻止腐化 = **"死亡有重量，但完成后是解脱"**——Wyrd 支柱完整
- **#2 + #3 + #4 + #10**：不可跳 + 战斗免费 + 8 块上限 + 旧碑更新 = **"情感时刻但聚落不爆"**——Living Hearth 协同
- **#8 + #9 + #11**：让其安息 5-7 天 + 进度可见 + 治疗 v1.1 = **"理性玩家有出路，但需要等待"**——给所有玩家类型一个选择
- **#12**：boss 仅 2 选 1 = **MVP 范围控制**——v1.0 不在 boss 上做全套 5 选 1

### 仍待 playtest 调参（不阻塞 GDD 锁定）

| 待调项 | 候选范围 | 调参位置 |
|---|---|---|
| 5 种送别期望分布 | 50/10/25/10/5 vs 40/15/25/15/5 | playtest 数据 |
| 纪念碑士气上限 | 8% vs 12% vs 16% | `GameConfigSO.memorialMoodBonusPerMemorial` |
| 挽歌曲目数量 | 3 vs 5 vs 8 | `DialogueSO` 设计 |
| Wyrd 锚点效果 | +10% vs +15% 聚落效率 | `GameConfigSO.wyrdAnchorEfficiencyBonus` |
| 强留腐化加速 | 4.5-7.5 天 vs 3-4 天 | `GameConfigSO.wyrdAnchorStopsDecay`（true=不腐化） |

→ 这些都是 Prototype 阶段的**数值调参工作**，通过 `GameConfigSO` 直接改即可，不阻塞任何 GDD。

---

> 12 个开放问题待用户拍板。

1. **24h 决策窗口** 默认行为？
   - 我的推荐：**24h 后自动强留**（强迫面对）
2. **送别演出** 不可跳过？
   - 我的推荐：**不可跳过**（保留情感时刻）
3. **战斗葬礼** v1.0 消耗 god-ember？
   - 我的推荐：**消耗 0**（v1.0 免费，鼓励尝试）
4. **纪念碑** v1.0 数量上限？
   - 我的推荐：**8 块**（等于英灵上限）
5. **衰悼期** 24h 效率惩罚？
   - 我的推荐：**-20%**（明显但不致命）
6. **Wyrd 锚点** 聚落效率加成？
   - 我的推荐：**+10%**（明显的"完成奖励"）
7. **Wyrd 锚点** 阻止强留腐化？
   - 我的推荐：**是**（v1.0 锁定："已完成 5/5 = 玩家已学会面对"）
8. **让其安息** 自然腐化时长？
   - 我的推荐：**5-7 天**（不短不长）
9. **亡者之誓** 进度条 v1.0 可见？
   - 我的推荐：**是**（透明）
10. **纪念碑** 满 8 个后新送别？
    - 我的推荐：**用旧碑更新**（v1.0 决策，避免聚落过度拥挤）
11. **强留 + 反悔** v1.0 治疗仪式？
    - 我的推荐：**v1.0 不实现**（v1.1 决策，减少 MVP 范围）
12. **boss 送别** v1.0 简化？
    - 我的推荐：**仅 2 选 1**（简单处理 / 战斗葬礼，无 5 全套）

---

## Unity Implementation Notes

### 核心脚本（`Assets/Scripts/DeathSendoff/`）
- `SendoffManager.cs` —— 5 种送别方式管理
- `DyingStateUI.cs` —— dying 弹窗
- `MemorialManager.cs` —— 纪念碑
- `SendoffCinematic.cs` —— 送别演出
- `MourningSystem.cs` —— 衰悼期
- `WyrdAnchor.cs` —— Wyrd 锚点
- `DeathOathTracker.cs` —— 亡者之誓 5 里程碑
- `CorpseDecaySystem.cs` —— 强留腐化（调用 Einherjar 系统）

### 数据结构
```csharp
public class EinherjarSendoff {
  public string einherjarId;
  public SendoffType sendOffType;  // None / Valhalla / Battle / Burial / Refuse / LetRest
  public float dyingHoursRemaining;
  public int decayLevel;  // 0-5
  public EinherjarSendoffState state;  // Alive / Dying / Unstable / Sent / Resting / Wighted
}

public enum SendoffType {
  None,
  Valhalla,     // 送英灵殿
  Battle,       // 战斗葬礼
  Burial,       // 简单葬礼
  Refuse,       // 强留
  LetRest,      // 让其安息
}
```

### 状态机
```csharp
public enum EinherjarSendoffState {
  Alive,        // 正常
  Dying,        // 24h 决策窗口
  Unstable,     // 强留后 3-5 天腐化
  Resting,      // 让其安息 5-7 天自然腐化
  Sent,         // 已送走（Valhalla/Battle/Burial）
  Wighted,      // 尸鬼化
}
```

### 事件订阅
```csharp
public class SendoffManager : MonoBehaviour {
  public static event Action<Einherjar> OnEinherjarDying;
  public static event Action<Einherjar, SendoffType> OnSendoffChosen;
  public static event Action<Einherjar> OnMemorialCreated;
  public static event Action OnMourningStarted;
  public static event Action OnMourningEnded;
  public static event Action OnWyrdAnchorActivated;
  public static event Action OnDeathOathCompleted;
}
```

### 性能预算
- dying UI 弹窗：< 8ms
- 送别演出：< 8ms / 帧（VFX + 粒子）
- 纪念碑渲染：< 4ms / 帧
- 亡者之誓进度计算：< 2ms

### 演出制作
- **送英灵殿**：金色光柱 VFX（参考 Ravensong 风格） + 奥丁号角音频
- **战斗葬礼**：野兽聚集 VFX + 战斗号角音频
- **简单葬礼**：挖土动画 + 吟游诗人吟唱
- **强留**：英灵躺下 + 炉火减弱
- **纪念碑**：石质模型 + 卢恩符文（夜间发光）
- **挽歌**：3-5 首不同曲调，吟游诗人播放

---

## 进度

| 章节 | 状态 |
|---|---|
| A. Overview | ✅ |
| B. Player Fantasy | ✅ |
| C. Detailed Design (9 小节) | ✅ |
| D. Formulas (8 个) | ✅ |
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
| 2026-07-27 | v1.0 LOCKED | 12 开放问题用户拍板全部锁定；data-config v1.7 同步升级 + 新增 `SendoffSO`（类型 10） | Mavis + 用户 |
| 2026-07-27 | FADT 三表补充 | 按 FADT 框架补充 Intention / Perceivable Consequence / Story 三维分析（提升承诺有重量叙事的显式度） | Mavis + 用户 |
| 2026-07-27 | FADT 三表补充 | 按 FADT 框架补充 Intention / Perceivable Consequence / Story 三维分析（提升承诺有重量叙事的显式度） | Mavis + 用户 |
