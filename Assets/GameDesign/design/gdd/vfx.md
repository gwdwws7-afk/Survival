# VFX — System GDD

> **Status**: 🔒 LOCKED v1.0
> **Author**: Mavis + 用户
> **Last Updated**: 2026-07-27
> **Implements Pillar**: **Foundation**（L5 表现层；锚定 `style-bible.md` 油画风）
> **See Also**: `style-bible.md`（油画风锚定）/ `world-exploration.md` §C.6（群系过渡带）/ `death-sendoff.md` §C.7（送别演出）/ `fate-thread.md` §C.1 规则 8（编织 VFX）

---

## A. Overview

**VFX（视觉特效）是 Ravensong 的"叙事性视觉"——所有"非游戏对象"都通过 VFX 呈现：粒子、光柱、天气、群系氛围、仪式演出、战斗反馈。** VFX 不是"花哨的视觉效果"（避免成为 1 个 Unreal Engine 5 demo reel），而是**叙事性 VFX**——每个粒子效果都**对应一个游戏时刻**，让玩家**感受到**那一刻的重量。

Ravensong 的 VFX 设计哲学是 **"叙事性粒子"（Narrative Particles）**：
- **少而精**——每帧粒子数 < 500（避免性能问题）
- **油画质感**——所有粒子带 5-10% 油画纹理锚定 style-bible
- **配色统一**——navy / cyan / gold 为主，避免多彩
- **仪式感**——重要时刻（金色光柱 / 化光 / 神显）粒子更密

VFX 6 大类（v1.0）：
- **群系氛围**（Atmosphere）——每群系独特的"空气感"
- **仪式演出**（Ritual）——编织 / 送别 / 誓言 / 奥丁审判
- **战斗反馈**（Combat）——攻击 / 受击 / 死亡 / 状态效果
- **状态效果**（Status）——燃烧 / 冰冻 / 中毒 / 祝福
- **天气系统**（Weather）——雨 / 雪 / 雾 / 风
- **UI VFX**（UI）——淡入 / 淡出 / Toast / 通知

数据层由**新增**的 `VFXPresetSO`（data-config.md C.2 类型 15）驱动；本 GDD 专注于**6 类 VFX 清单、视觉规格、性能预算、跨系统 VFX 协同**。

---

## B. Player Fantasy

### 主幻想
> "我在永冻崖看到远处有淡蓝色的光柱——走近，是奥丁的审判点。光柱上有卢恩符文缓缓转动，每帧不超过 100 个粒子。这就是 Ravensong。"

### 关键体验时刻

- **第一次**看到群系 VFX：进入白桦林，桦树摇曳 + 阳光斜射 + 远处有淡金色光点
- **第一次**编织 VFX：3 秒内 cyan 丝线 + 符文浮现 + "叮" 的一声 + 物品发光
- **第一次**送英灵殿 VFX：5-10 秒金色光柱 + Eirik 化光 + 聚落变亮
- **第一次**Boss 战 VFX：3 min 演出 + 暗色巨物 + cyan 符文攻击
- **第一次**天气切换：突然下雪，整个世界色调变冷 + 篝火更亮
- **第一次**奥丁审判 VFX：金色双翼展开 + 奥丁头像浮现 + 整个聚落光辉

---

## C. Detailed Design

### C.1 核心规则（Core Rules）

#### 规则 1：每帧粒子总数 < 500
- 单个 VFX：< 100 粒子
- 群系氛围：< 50 粒子
- 仪式演出：< 200 粒子
- Boss 战：< 300 粒子
- 多个 VFX 同时触发时，优先级 = 仪式 > 战斗 > 状态 > 氛围

#### 规则 2：所有 VFX 锚定 style-bible 配色
- **主色**：navy（深蓝）/ cyan（青）/ gold（金）
- **辅色**：米色 / 灰
- **避免**：纯红 / 纯绿 / 纯紫（破坏油画风）
- **特殊**：奥丁审判用金 + 银（神话色）

#### 规则 3：所有 VFX 0.5-1 秒淡入
- **不**用粒子"突然出现"
- 0.5-1 秒 = "有出现感"但不打扰
- 0.3 秒淡出 = 干净退出

#### 规则 4：VFX 性能预算
- 单个 VFX：< 4ms / 帧
- 群系氛围：< 2ms / 帧
- 仪式演出：< 8ms / 帧
- Boss 战：< 16ms / 帧
- 总 VFX：< 16ms / 帧（60 FPS 预算）

#### 规则 5：VFX 跨系统协同
- 编织 VFX（fate-thread）—— 3 秒 cyan 丝线
- 送别 VFX（death-sendoff）—— 5-10 秒金色光柱
- 誓言 VFX（oath-system）—— 5 秒仪式动画
- 编织/送别/誓言**共享**金色光柱预设

#### 规则 6：VFX LOD
- 距离>20m：粒子数 -50%
- 距离>50m：粒子数 -75%
- 距离>100m：粒子**不**渲染
- 屏幕外：粒子**不**更新

#### 规则 7：VFX 触发音效（v1.1 决策：vfx-audio）
- v1.0：VFX 独立，**不**绑定音效
- v1.1 决策：vfx-audio 联动，VFX 触发时同步 SFX

#### 规则 8：VFX 可在设置关闭
- 设置菜单"减少 VFX" 开关
- 开启时：所有 VFX 减半
- **不**完全关闭（破坏叙事）

---

### C.2 群系氛围 VFX（Atmosphere）⭐

> 每个群系独特的"空气感"——粒子 + 光照 + 色调。

#### 群系 1：白桦林（Birch Forest）
- **粒子**：飘落桦树皮（白色 / 米色，每秒 5 个）
- **光照**：阳光斜射（金色 ray）
- **色调**：偏暖（米黄 / 淡金）
- **声音锚点**：鸟鸣（vfx-audio v1.1 联动）
- **总粒子数**：30

#### 群系 2：白骨原（Bone Field）
- **粒子**：飘落骨粉（灰白，每秒 3 个）
- **光照**：低角度阳光（橙红）
- **色调**：偏冷灰（白 + 灰）
- **声音锚点**：风声（vfx-audio v1.1 联动）
- **总粒子数**：20

#### 群系 3：深渊沼（Abyss Marsh）
- **粒子**：飘浮深渊水晶（cyan，每秒 2 个）
- **光照**：永远阴沉（暗蓝）
- **色调**：偏冷（深蓝 + 紫）
- **雾**：50% 视觉降低
- **总粒子数**：15

#### 群系 4：永冻崖（Frost Cliff）
- **粒子**：飘落雪花（白色，每秒 10 个）
- **光照**：冰反射（cyan + 银）
- **色调**：偏冷（白 + 银）
- **总粒子数**：40

#### 群系 5：奥丁圣所（Odin's Sanctum）
- **粒子**：金色卢恩符文（每秒 1 个）
- **光照**：神光（金色 ray）
- **色调**：偏金（金 + 银）
- **总粒子数**：10

#### 群系 6：深渊之心（Heart of the Abyss）
- **粒子**：暗物质漂浮（深紫，每秒 2 个）
- **光照**：永久暗（无光）
- **色调**：偏深紫（紫 + 黑）
- **总粒子数**：10

#### 群系过渡 VFX（30m 过渡带）
- 群系 A 粒子 + 群系 B 粒子**同时**渲染，密度各 50%
- 1 秒渐变（从 A 100% → B 100%）
- 视觉"群系交汇" 感

---

### C.3 仪式演出 VFX（Ritual）⭐

> 5 签名系统的"关键时刻"——所有 VFX 中最重要的部分。

#### 仪式 1：编织（Fate-Thread）⭐
- **时长**：3 秒
- **VFX**：
  - 0-0.5s：2 输入物品间出现 cyan 丝线
  - 0.5-2.5s：符文浮现 + 物品旋转
  - 2.5-3s：物品发光 + 丝线消失
- **粒子数**：50（丝线 + 符文）
- **光柱**：无
- **成功 / 失败**：成功 = 物品化光 / 失败 = 物品变灰

#### 仪式 2：送英灵殿（Valhalla Send-off）⭐
- **时长**：10 秒
- **VFX**：
  - 0-2s：Eirik 躺在炉火旁 + 头顶出现奥丁之眼
  - 2-7s：金色光柱从天空落下（垂直）
  - 7-10s：Eirik 化光（粒子向上飘）
- **粒子数**：150（金色光柱 + 化光）
- **光柱**：金色（垂直）
- **演出终止**：聚落变亮 0.5s

#### 仪式 3：强留（Refuse）⚠️
- **时长**：3 秒
- **VFX**：
  - 0-1s：英灵躺下（停止工作）
  - 1-3s：炉火变蓝 + 聚落色调变冷
- **粒子数**：20（蓝雾）
- **光柱**：无
- **演出终止**：24h 衰悼期

#### 仪式 4：英灵招募（Recruit）
- **时长**：5 秒
- **VFX**：
  - 0-1s：玩家伸出援手
  - 1-3s：英灵站起 + cyan 光晕
  - 3-5s：英灵走向聚落
- **粒子数**：30（cyan 光晕）
- **光柱**：无

#### 仪式 5：誓言完成（Oath Complete）
- **时长**：5 秒
- **VFX**：
  - 0-1s：誓言图标亮起
  - 1-3s：金色光柱从聚落中央升起
  - 3-5s：永久 buff 出现 + 聚落变亮
- **粒子数**：100（金色光柱）
- **光柱**：金色（垂直）

#### 仪式 6：奥丁审判（Odin Trial）⭐ 终局
- **时长**：3 分钟
- **VFX**：
  - 0-30s：奥丁头像出现（大型）
  - 30-150s：奥丁独白（头像 + 字幕 + 背景金色 ray）
  - 150-180s：奥丁审判选择弹窗
- **粒子数**：200（金色符文 + 神光）
- **光柱**：金色（垂直，多光柱）

#### 仪式 7：聚落升级（Building Upgrade）
- **时长**：24 小时（VFX 持续）
- **VFX**：
  - 0-1s：建筑发光
  - 1-24h：建筑工地粒子（灰尘）
  - 24h 末：建筑完成 + 金色闪
- **粒子数**：50（灰尘）
- **演出终止**：建筑 Level 提升

---

### C.4 战斗 VFX（Combat）⭐

> 玩家与敌人战斗的"反馈"——攻击 / 受击 / 死亡 / 状态效果。

#### 攻击 VFX
- **轻击**：白色剑光（0.2s，5 粒子）
- **重击**：橙色剑光（0.4s，15 粒子）
- **Fate-Thread Bind**：cyan 丝线（1s，30 粒子）
- **Boss 攻击**：暗色巨物（0.5s，50 粒子）

#### 受击 VFX
- **玩家受击**：红闪（0.2s，0 粒子 = 全屏红色 overlay）
- **敌人受击**：白闪（0.1s，5 粒子）
- **Boss 受击**：金色闪（0.3s，15 粒子）

#### 死亡 VFX
- **普通敌人**：白烟（1s，10 粒子）
- **精英**：金烟（1.5s，20 粒子）
- **Boss**：金色光柱（5s，100 粒子）+ 演出
- **尸鬼化（强留 5/5）**：黑烟（2s，30 粒子）+ 攻击

#### 状态效果 VFX
- **燃烧**：火焰（持续，5 粒子 / 秒）
- **冰冻**：冰晶（持续，3 粒子 / 秒）
- **中毒**：绿色气泡（持续，2 粒子 / 秒）
- **祝福**：金色光晕（持续，3 粒子 / 秒）

#### Fate-Thread Bind VFX（关键）
- 0-0.3s：cyan 丝线从玩家射向敌人
- 0.3-0.7s：敌人被绑 + 不可移动
- 0.7-1s：丝线变金色 + 敌人受额外伤害
- **粒子数**：30

---

### C.5 状态效果 VFX（Status）

> 长期状态效果——持续渲染，粒子较少但稳定。

#### 持续效果
- **篝火燃烧**：3 帧火焰 + 烟雾（持续，10 粒子 / 秒）
- **衰悼期**：蓝色火焰 + 雨（持续，15 粒子 / 秒）
- **Wyrd 锚点激活**：金色光柱（持续，20 粒子 / 秒）
- **满月神显**：cyan 符文环绕（持续，10 粒子 / 秒）
- **奥丁之眼扫描**：金色全屏覆盖（3s，30 粒子）

#### 一次性效果
- **装备获得**：金闪（0.5s，10 粒子）
- **聚落升级完成**：金闪（2s，30 粒子）
- **任务完成**：金 Toast（4s，0 粒子 = UI）
- **英灵加入**：cyan 光环（3s，20 粒子）

---

### C.6 天气 VFX（Weather）⭐

> 6 个群系各自的天气系统——雨 / 雪 / 雾 / 风。

#### 天气类型

| 天气 | 触发 | 群系 | 视觉 |
|---|---|---|---|
| **晴朗** | 默认 | 所有 | 无额外 VFX |
| **多云** | 随机 | 白桦林 | 灰色调（10%）|
| **雨** | 概率 | 白桦林 / 白骨原 | 雨滴粒子（30 粒子 / 秒）|
| **雪** | 概率 | 永冻崖 | 雪花粒子（40 粒子 / 秒）|
| **雾** | 概率 | 深渊沼 | 雾 80% 视觉降低 |
| **暴风雪** | 概率 | 永冻崖 | 雪 + 风（50 粒子 / 秒 + 屏幕抖动）|
| **神显** | 满月 | 所有 | cyan 符文环绕（20 粒子 / 秒）|

#### 天气触发概率
- 每 6 真实小时**最多**触发 1 个天气变化
- 触发概率：晴 60% / 多云 15% / 雨 10% / 雪 5% / 雾 5% / 暴风雪 2% / 神显 3%（满月时）
- 天气持续 6-24 真实小时

#### 天气对游戏的影响
- **雨 / 雪**：篝火需要更频繁添加柴
- **雾**：视野 -50%
- **暴风雪**：永冻崖寒冷 debuff ×2
- **神显**：编织 Tier 5 配方出现率 +50%

---

### C.7 UI VFX

> UI 元素的视觉效果——淡入 / 淡出 / Toast / 通知。

#### UI 淡入
- HUD 元素：0.5s 淡入（参考 ui-hud.md §G）
- 弹窗：0.5s 淡入
- 菜单：0.5s 淡入 + 0.3s 淡出

#### Toast 通知
- 资源拾取：金色 +N（2s）
- 任务完成：金色 Toast（4s）
- 教程提示：米色（5s）
- 错误提示：红色（2s）

#### UI 反馈
- 按钮悬停：cyan 描边（0.1s）
- 按钮点击：cyan 填充（0.1s）
- 弹窗打开：低 "咚"（音效 v1.1 联动）
- 弹窗关闭：反向 "咚"（音效 v1.1 联动）

#### 主题切换（Day-Night）
- HUD 颜色 smoothstep 1 秒
- 菜单色调 1 秒
- 弹窗边框色 1 秒

---

### C.8 VFX 性能预算

#### 单帧预算分配
| VFX 类型 | 预算 | 优先级 |
|---|---|---|
| 仪式演出 | 8ms | 最高 |
| Boss 战 | 16ms | 高 |
| 战斗反馈 | 4ms | 高 |
| 群系氛围 | 2ms | 中 |
| 状态效果 | 2ms | 中 |
| 天气 | 4ms | 中 |
| UI VFX | 2ms | 低 |

#### 总预算
- 60 FPS 目标：< 16ms / 帧
- 30 FPS 降级：< 33ms / 帧

---

### C.9 与其他系统的协同

| 系统 | 协同 VFX |
|---|---|
| **Day-Night** | 群系氛围 + 篝火昼夜差异 |
| **Fate-Thread** | 编织 VFX（3 秒 cyan 丝线）|
| **Death-Send-off** | 5 种送别 VFX |
| **Oath** | 誓言完成 VFX（5 秒仪式）|
| **Einherjar** | 招募 VFX（5 秒 cyan 光环）|
| **Settlement** | 升级 VFX（24h 工地）|
| **Combat** | 攻击 / 受击 / 死亡 VFX |
| **UI/HUD** | UI 淡入 / Toast / 主题切换 |
| **Weather** | 6 类天气 VFX |

---

## D. Formulas

### D.1 粒子 LOD
```csharp
int CalculateParticleCount(int baseCount, float distance) {
  if (distance > 100f) return 0;
  if (distance > 50f) return baseCount / 4;
  if (distance > 20f) return baseCount / 2;
  return baseCount;
}
```

### D.2 群系 VFX 渐变（30m 过渡带）
```csharp
float GetBiomeVFXAlpha(BiomeSO from, BiomeSO to, float t) {
  // t = 0 at from, 1 at to
  return Mathf.SmoothStep(0f, 1f, t);
}
```

### D.3 VFX 优先级
```csharp
int CalculateVFXPriority(VFXType type) {
  return type switch {
    Ritual => 4,
    Boss => 3,
    Combat => 2,
    Status => 1,
    Weather => 1,
    Atmosphere => 0,
    _ => 0
  };
}
```

### D.4 主题颜色插值
```csharp
Color4 GetThemeColor(Color4 dayColor, Color4 nightColor, float dayNightT) {
  return Color4.Lerp(dayColor, nightColor, dayNightT);
}
```

### D.5 天气触发
```csharp
WeatherType RollWeather(BiomeSO biome) {
  float roll = Random.value;
  return biome.weatherTable.GetWeather(roll);
}
```

---

## E. Edge Cases

| 情况 | 处理 |
|---|---|
| VFX 触发时玩家在聚落 | VFX **不**自动应用（聚落是庇护所）|
| 多个 VFX 同时触发 | 按优先级排序（仪式 > 战斗 > 状态 > 氛围）|
| 玩家在 VFX 中退出 | VFX 进度**保留**（持久化）|
| 玩家设备性能不足 | "减少 VFX" 开关（设置菜单）|
| 玩家色弱 | VFX 仍触发（颜色为主，**不**完全依赖颜色）|
| 多个英灵同时送别 | 5 秒后**串行**触发（避免粒子爆）|
| 暴风雪 + 篝火 | 篝火需要更频繁添加柴 |
| 满月 + 神显 | 编织 Tier 5 +50% |
| VFX 与战斗 UI 冲突 | 战斗 UI 优先 |
| 聚落 VFX 与天气冲突 | 聚落 VFX 优先（庇护所）|
| 玩家死亡 VFX | 单独黑屏 VFX（无粒子）|
| 奥丁审判中 | VFX **不**暂停（演出连贯）|

---

## F. Dependencies

### 上游（这个系统依赖谁）
- **Style-bible** —— 油画风锚定 + 配色
- **Data Config** —— `VFXPresetSO` 是新类型 15
- **所有签名系统** —— 仪式 VFX 来源

### 下游（谁依赖这个系统）
- **所有 14 个 GDD** —— VFX 是它们的"叙事时刻"
- **UI/HUD** —— UI VFX

---

## G. Tuning Knobs（12 字段）

| 旋钮 | 默认值 | 范围 | 决策编号 | 影响 |
|---|---|---|---|---|
| `vfxMaxParticlesPerFrame` | 500 | 200-1000 | #1 | 每帧总粒子上限 |
| `vfxFpsTarget` | 60 | 30-60 | #2 | VFX 目标帧率 |
| `vfxFadeInSec` | 0.5f | 0-2 | #3 | VFX 淡入时长 |
| `vfxFadeOutSec` | 0.3f | 0-2 | #3 | VFX 淡出时长 |
| `vfxLodNearMeters` | 20f | 10-50 | #4 | LOD 近距离阈值 |
| `vfxLodMidMeters` | 50f | 30-100 | #4 | LOD 中距离阈值 |
| `vfxLodFarMeters` | 100f | 50-200 | #4 | LOD 远距离阈值 |
| `vfxReducedMode` | false | bool | #5 | 减少 VFX 模式（玩家设置）|
| `vfxReducedMultiplier` | 0.5f | 0.1-0.8 | #5 | 减少 VFX 模式乘数 |
| `weatherChangeIntervalHours` | 6f | 3-12 | #6 | 天气变化最小间隔 |
| `fullMoonGodsightChance` | 0.5f | 0-1 | #7 | 满月神显 Tier 5 +50% |
| `vfxDebugShowBounds` | false | bool | #8 | 调试显示 VFX 边界 |

---

## H. Acceptance Criteria

### AC-1: 群系 VFX 渲染
- **条件**：玩家进入任一群系
- **结果**：群系氛围 VFX 自动渲染

### AC-2: 仪式 VFX 触发
- **条件**：玩家编织 / 送别 / 完成誓言
- **结果**：仪式 VFX 3-10 秒演出

### AC-3: 战斗 VFX 反馈
- **条件**：玩家攻击 / 受击 / 击杀
- **结果**：战斗 VFX 0.1-1 秒反馈

### AC-4: 状态效果 VFX
- **条件**：玩家 / 敌人有持续状态
- **结果**：状态 VFX 持续渲染

### AC-5: 天气 VFX 切换
- **条件**：6 真实小时过去
- **结果**：天气可能变化（按概率）

### AC-6: UI VFX 淡入
- **条件**：HUD 元素 / 弹窗 / 菜单显示
- **结果**：0.5 秒淡入

### AC-7: VFX 性能
- **条件**：所有 VFX 同时触发
- **结果**：< 16ms / 帧

### AC-8: VFX LOD
- **条件**：玩家距离 VFX > 20m
- **结果**：粒子数 -50%

### AC-9: 主题切换 VFX
- **条件**：Day-Night 转换
- **结果**：HUD 颜色 smoothstep 1 秒

### AC-10: 减少 VFX 模式
- **条件**：玩家开启设置
- **结果**：所有 VFX 减半

### AC-11: 群系过渡带 VFX
- **条件**：玩家在过渡带
- **结果**：群系 VFX 1 秒渐变

### AC-12: 奥丁审判 VFX
- **条件**：进入深渊之心
- **结果**：3 分钟奥丁审判 VFX 演出

---

## 10. 已锁定决策（Locked Decisions）

> 2026-07-27 用户拍板，12 个开放问题全部锁定。已落地为 §G 旋钮 + data-config v2.2。

| # | 决策点 | 锁定值 | 落地位置 |
|---|---|---|---|
| 1 | **每帧粒子上限** | **500**（性能 + 视觉平衡） | §C.1 规则 1 + §G |
| 2 | **VFX 目标帧率** | **60 FPS** | §C.1 规则 4 + §G |
| 3 | **VFX 淡入时长** | **0.5 秒** | §C.1 规则 3 + §G |
| 4 | **VFX 淡出时长** | **0.3 秒** | §C.1 规则 3 + §G |
| 5 | **VFX LOD 近距离** | **20 米** | §C.1 规则 6 + §G |
| 6 | **VFX LOD 中距离** | **50 米** | §C.1 规则 6 + §G |
| 7 | **VFX LOD 远距离** | **100 米**（不渲染） | §C.1 规则 6 + §G |
| 8 | **减少 VFX 模式乘数** | **0.5×**（减半但仍可见） | §C.1 规则 8 + §G |
| 9 | **天气变化最小间隔** | **6 真实小时** | §C.6 + §G |
| 10 | **满月神显 Tier 5 概率** | **+50%** | §C.6 + §G |
| 11 | **VFX 调试模式** | **v1.0 不实现**（v1.1） | §C.1 规则 8 + v1.1 决策 |
| 12 | **奥丁审判 VFX 粒子数** | **200 粒子** | §C.3 仪式 6 |

### 决策之间的协同

- **#1 + #2 + #7**：500 粒子 + 60 FPS + 100m 不渲染 = **"性能友好"**——v1.0 60 FPS 稳定
- **#3 + #4 + #11**：0.5s 淡入 + 0.3s 淡出 + 无调试 = **"干净不打扰"**——叙事时刻清晰
- **#5 + #6 + #7 + #8**：LOD 3 档 + 减少 0.5× = **"中低配设备友好"**——可访问性
- **#9 + #10**：6h 天气 + 满月 +50% = **"动态世界"**——天气不刷屏但有惊喜

### 仍待 playtest 调参（不阻塞 GDD 锁定）

| 待调项 | 候选范围 | 调参位置 |
|---|---|---|
| 群系 VFX 粒子数 | 当前 vs ×1.5 | `VFXPresetSO.particleCount` |
| 战斗 VFX 反馈强度 | 0.5× / 1× / 2× | playtest 体验 |
| 天气概率分布 | 60/15/10/5/5/2 vs 40/20/15/10/10/5 | 调参 |
| 奥丁审判演出 | 3 min vs 2 min vs 4 min | oath-system §G |
| VFX 调试粒子上限 | 500 vs 1000 | 性能 |

→ 这些都是 Prototype 阶段的**视觉/数值调参工作**，通过 `GameConfigSO` / `VFXPresetSO` 直接改即可，不阻塞任何 GDD。

---

> 12 个开放问题待用户拍板。

1. **每帧粒子上限**
   - 我的推荐：**500**（性能 + 视觉平衡）
2. **VFX 目标帧率**
   - 我的推荐：**60 FPS**
3. **VFX 淡入时长**
   - 我的推荐：**0.5 秒**
4. **VFX 淡出时长**
   - 我的推荐：**0.3 秒**
5. **VFX LOD 近距离**
   - 我的推荐：**20 米**
6. **VFX LOD 中距离**
   - 我的推荐：**50 米**
7. **VFX LOD 远距离**
   - 我的推荐：**100 米**（不渲染）
8. **减少 VFX 模式乘数**
   - 我的推荐：**0.5×**（减半但仍可见）
9. **天气变化最小间隔**
   - 我的推荐：**6 真实小时**
10. **满月神显 Tier 5 概率**
    - 我的推荐：**+50%**（v1.0 决策：与 fate-thread 锁定）
11. **VFX 调试模式**
    - 我的推荐：**v1.0 不实现**（v1.1 加）
12. **奥丁审判 VFX 粒子数**
    - 我的推荐：**200 粒子**（与 1 决策一致）

---

## Unity Implementation Notes

### 核心脚本（`Assets/Scripts/VFX/`）
- `VFXManager.cs` —— VFX 管理（性能预算 + LOD）
- `BiomeAtmosphere.cs` —— 群系氛围 VFX（6 群系）
- `RitualVFX.cs` —— 仪式 VFX（7 类）
- `CombatVFX.cs` —— 战斗 VFX（攻击/受击/死亡/状态）
- `WeatherSystem.cs` —— 天气 VFX（6 类）
- `UIVFXController.cs` —— UI VFX（淡入/淡出/Toast）
- `VFXLODController.cs` —— VFX LOD（距离阈值）
- `ThemeVFX.cs` —— Day-Night 主题 VFX

### 数据结构
```csharp
public class VFXPreset {
  public VFXPresetSO data;               // VFX 预设 SO
  public VFXType type;                   // 7 类
  public int currentParticles;            // 当前粒子数
  public bool isPlaying;                 // 是否播放中
  public float elapsedTime;              // 演出时长
}

public enum VFXType {
  Atmosphere,        // 群系氛围
  Ritual,            // 仪式
  Combat,            // 战斗
  Status,            // 状态
  Weather,           // 天气
  UI,                // UI
  Theme,             // 主题
}
```

### 状态机
```csharp
public enum VFXState {
  Idle,          // 未触发
  FadeIn,        // 淡入
  Playing,       // 播放中
  FadeOut,       // 淡出
  Completed,     // 完成
}
```

### 事件订阅
```csharp
public class VFXManager : MonoBehaviour {
  public static event Action<VFXPreset> OnVFXTriggered;
  public static event Action<VFXPreset> OnVFXCompleted;
  public static event Action<WeatherType> OnWeatherChanged;
  public static event Action<VFXPreset> OnVFXLODReduced;
}
```

### 性能预算
- VFX Manager：< 2ms / 帧
- 单个 VFX：< 4ms / 帧
- 群系氛围：< 2ms / 帧
- 仪式演出：< 8ms / 帧
- Boss 战：< 16ms / 帧
- 总 VFX：< 16ms / 帧（60 FPS 预算）

### 资产制作（v1.0 关键 VFX）
- **6 群系氛围**：每群系 1 套
- **7 仪式演出**：每仪式 1 套
- **6 战斗 VFX**：攻击/受击/死亡/状态各 1 套
- **6 天气 VFX**：晴/多云/雨/雪/雾/暴风雪各 1 套
- **1 奥丁审判**：1 套（3 min 演出）
- **4 UI VFX**：淡入/淡出/Toast/主题各 1 套
- **总计：30 套 VFX**（每套 1-3 个 particle system + 1 个 material）

---

## 进度

| 章节 | 状态 |
|---|---|
| A. Overview | ✅ |
| B. Player Fantasy | ✅ |
| C. Detailed Design (9 小节) | ✅ |
| D. Formulas (5 个) | ✅ |
| E. Edge Cases (12 种) | ✅ |
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
| 2026-07-27 | v1.0 LOCKED | 12 开放问题用户拍板全部锁定；data-config v2.2 同步升级 + 新增 `VFXPresetSO`（类型 15） | Mavis + 用户 |
