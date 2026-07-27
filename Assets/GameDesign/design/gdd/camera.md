# Camera — System GDD

> **Status**: 🔒 LOCKED v1.0
> **Author**: Mavis + 用户
> **Last Updated**: 2026-07-27
> **Implements Pillar**: **Foundation**（L5 表现层；2D 俯视相机）
> **See Also**: `style-bible.md`（油画风锚定）/ `ui-hud.md` §C.2（HUD 元素位置）/ `world-exploration.md` §C.6（群系过渡）

---

## A. Overview

**Camera 是 Ravensong 的"叙事镜头"——所有"游戏画面"通过 2D 俯视相机呈现。** Camera **不**是"跟随玩家的相机"（避免成为 1 个 generic 2D game），而是**叙事性 Camera**——相机的位置、缩放、跟随方式都**对应一个游戏时刻**，让玩家**通过镜头看到** Ravensong 的世界。

Ravensong 的 Camera 设计哲学是 **"叙事性镜头"（Narrative Camera）**：
- **2D 俯视**（Top-down 2D）——永恒不变
- **跟随玩家**——大部分时间
- **战斗偏移**——战斗时相机略微偏移
- **仪式拉远**——仪式时相机拉远看全景
- **群系过渡缩放**——群系过渡时相机短暂缩放

Camera v1.0 范围（精简）：
- **跟随模式**（Follow）——默认
- **战斗模式**（Combat）——战斗时偏移
- **仪式模式**（Ritual）——仪式时拉远
- **群系过渡**（Transition）——过渡时缩放
- **死亡 / 复活**——黑屏 + 复位

数据层**不**新增 SO（Camera 行为简单，参数都在 `GameConfigSO`）；本 GDD 专注于**5 种模式、镜头边界、性能预算、与 VFX 协同**。

---

## B. Player Fantasy

### 主幻想
> "我送别 Eirik 时，相机缓缓拉远——我看到整个聚落 + Eirik 化光 + 金色光柱。这就是 Ravensong 的'镜头叙事'。"

### 关键体验时刻

- **第一次**跟随：相机自然跟随玩家移动
- **第一次**群系过渡：相机短暂缩放，看群系交汇
- **第一次**仪式：相机拉远看全景
- **第一次**Boss 战：相机略微偏移到战斗最佳位置
- **第一次**死亡：黑屏 + 相机复位

---

## C. Detailed Design

### C.1 核心规则（Core Rules）

#### 规则 1：2D 俯视 + 正交相机
- 永远 2D 俯视
- 永远正交（orthographic）
- 永远从正上方看
- **不**做透视 / 3D

#### 规则 2：5 种相机模式
- Follow（默认）
- Combat（战斗）
- Ritual（仪式）
- Transition（群系过渡）
- Death（死亡）

#### 规则 3：相机跟随 + 边界
- 玩家移动 → 相机跟随
- 相机**不**超出世界边界
- 相机**不**在战斗时抖动（v1.0 决策：避免头晕）

#### 规则 4：相机缩放
- 默认缩放 = 1.0
- 群系过渡 = 0.8-1.2 短暂缩放
- 仪式 = 0.7-0.9 拉远
- 死亡 = 0.5-0.7 拉远 + 黑屏

#### 规则 5：相机性能预算
- 跟随：< 1ms / 帧
- 模式切换：< 2ms（一次性）
- 缩放过渡：< 2ms（持续 0.5s）

#### 规则 6：相机边界
- 玩家**不**能走到世界边缘
- 相机**不**显示世界外区域
- 6 群系 + 聚落 + 远征基地 都在世界内

#### 规则 7：相机输入
- 玩家**不能**手动控制相机（v1.0 决策：避免分散注意力）
- v1.1 决策：可选"自由相机"（缩放 + 略微旋转）

#### 规则 8：相机 + Day-Night 协同
- 白天相机色调 = 暖
- 夜晚相机色调 = 冷
- Dawn/Dusk = 插值
- v1.0 决策：相机**不**做"夜视"（玩家需自己适应）

---

### C.2 5 种相机模式

#### 模式 1：Follow（默认）⭐

**行为**：
- 相机位置 = 玩家位置 + 偏移
- 偏移 = (0, 0, -10)  // 正交相机
- 跟随速度 = 5 单位/秒（**不**是即时跟随，有 0.1s 延迟）

**参数**：
- `cameraFollowOffset` = (0, 0, -10)
- `cameraFollowLerpSpeed` = 5

**触发**：
- 默认状态
- 玩家不在战斗 / 仪式 / 过渡 / 死亡

---

#### 模式 2：Combat（战斗）⭐

**行为**：
- 相机位置 = 玩家 + Boss 中点 + 偏移
- 偏移 = (0, 0, -10)
- 相机**略微偏移**到战斗最佳位置（玩家和 Boss 之间）

**参数**：
- `cameraCombatOffsetMultiplier` = 1.2（战斗相机略远）
- `cameraCombatLerpSpeed` = 3（比 Follow 慢，给玩家反应时间）

**触发**：
- 玩家进入战斗
- 战斗结束 → 回到 Follow

---

#### 模式 3：Ritual（仪式）⭐

**行为**：
- 相机位置 = 仪式中心 + 偏移
- 相机**拉远**到仪式最佳位置
- 缩放 = 0.8（拉远）

**参数**：
- `cameraRitualZoom` = 0.8
- `cameraRitualLerpSpeed` = 2（缓慢拉远）

**触发**：
- 编织开始 → Ritual 模式
- 编织完成 → 仪式结束 → 回到 Follow
- 送别开始 → Ritual 模式
- 誓言完成 → Ritual 模式
- 奥丁审判 → Ritual 模式（持续 3 min）

---

#### 模式 4：Transition（群系过渡）⭐

**行为**：
- 相机位置 = 群系 A + 群系 B 中点 + 偏移
- 缩放 = 1.2（拉近，看群系细节）
- 1 秒后 → Follow

**参数**：
- `cameraTransitionZoom` = 1.2
- `cameraTransitionDurationSec` = 1f

**触发**：
- 玩家进入群系过渡带
- 离开过渡带 → 回到 Follow

---

#### 模式 5：Death（死亡）

**行为**：
- 黑屏（0.5s 渐黑）
- 相机停止移动
- 玩家复活后 → 相机复位 + Follow

**参数**：
- `cameraDeathFadeSec` = 0.5f
- `cameraDeathZoom` = 0.7

**触发**：
- 玩家死亡
- 玩家复活 → 回到 Follow

---

### C.3 相机边界

#### 世界边界
- 6 群系 + 聚落 + 远征基地 全部在世界内
- 世界大小：约 1000x1000 单位
- 玩家**不**能走出世界

#### 相机边界
- 相机 = 玩家位置 + 偏移
- 相机**不**超出世界边界
- 玩家在边界时相机**不**跟随

#### 群系边界
- 群系 A 内部 → 相机在 A 色调
- 过渡带 → 相机 1 秒渐变
- 群系 B 内部 → 相机在 B 色调

---

### C.4 相机缩放

#### 缩放范围
- 最小：0.5（拉远，看全景）
- 最大：2.0（拉近，看细节）
- 默认：1.0（平衡）

#### 缩放时机
- **Ritual**：缩放 0.7-0.9（拉远）
- **Transition**：缩放 1.2（拉近）
- **Death**：缩放 0.7（拉远）
- **Follow**：缩放 1.0（默认）

#### 缩放过渡
- 缩放变化 0.5 秒 smoothstep
- 避免突兀的视角变化

---

### C.5 相机 + 群系主题

#### 群系色调应用
- 相机背景 = 群系色调
- 例：白桦林 = 暖米色 / 永冻崖 = 冷白
- 群系过渡 = 1 秒渐变

#### 群系光照应用
- 相机光照 = 群系光照
- 例：奥丁圣所 = 神光 / 深渊之心 = 永久暗

---

### C.6 相机 + VFX / 仪式协同

#### 仪式时相机行为
- 编织：相机拉远看编织动画（3s）
- 送英灵殿：相机拉远看光柱（10s）
- 奥丁审判：相机拉远看头像（3 min）

#### Boss 战时相机行为
- 相机偏移到玩家和 Boss 之间
- **不**抖动（v1.0 决策）
- Boss 死亡后 → 缓慢拉远看演出

#### VFX 触发时相机
- VFX 触发**不**改变相机（避免分散注意力）
- v1.1 决策：可选"VFX 重点相机"

---

### C.7 相机输入

#### v1.0 玩家**不能**控制相机
- 玩家**不能**缩放（v1.0 决策：避免分散注意力）
- 玩家**不能**旋转
- 玩家**不能**自由移动

#### v1.1 决策
- 玩家可选"自由相机"模式
- 缩放（滚轮）
- 略微旋转（鼠标右键）
- v1.0 不实现

---

### C.8 相机性能

#### 性能预算
- 跟随：< 1ms / 帧
- 模式切换：< 2ms（一次性）
- 缩放过渡：< 2ms（持续 0.5s）
- 总计：< 4ms / 帧

#### 60 FPS 目标
- 16ms / 帧总预算
- 相机 4ms / 帧 = 25% 预算

---

### C.9 与其他系统的协同

| 系统 | 协同 Camera |
|---|---|
| **Day-Night** | 相机色调 + 光照 |
| **Fate-Thread** | 编织时拉远 |
| **Death-Send-off** | 送别时拉远 |
| **Oath** | 誓言完成时拉远 |
| **Einherjar** | 招募时拉远（5s）|
| **Settlement** | 聚落时**不**特殊处理（默认 Follow）|
| **Combat** | 战斗时偏移 |
| **VFX** | 仪式 VFX 拉远 |
| **UI/HUD** | HUD 元素位置基于相机 |

---

## D. Formulas

### D.1 相机跟随
```csharp
Vector3 GetCameraPosition() {
  Vector3 target = player.position + cameraFollowOffset;
  return Vector3.Lerp(transform.position, target, cameraFollowLerpSpeed * Time.deltaTime);
}
```

### D.2 战斗相机偏移
```csharp
Vector3 GetCombatCameraPosition() {
  Vector3 midpoint = (player.position + boss.position) / 2f;
  return midpoint + cameraFollowOffset * cameraCombatOffsetMultiplier;
}
```

### D.3 仪式相机拉远
```csharp
void EnterRitualMode() {
  StartCoroutine(ZoomTo(cameraRitualZoom, cameraRitualLerpSpeed));
}
```

### D.4 缩放过渡
```csharp
IEnumerator ZoomTo(float targetZoom, float duration) {
  float startZoom = camera.orthographicSize;
  for (float t = 0; t < duration; t += Time.deltaTime) {
    camera.orthographicSize = Mathf.Lerp(startZoom, targetZoom, t / duration);
    yield return null;
  }
  camera.orthographicSize = targetZoom;
}
```

### D.5 群系色调插值
```csharp
Color4 GetBiomeTint(BiomeSO from, BiomeSO to, float t) {
  return Color4.Lerp(from.cameraTint, to.cameraTint, t);
}
```

---

## E. Edge Cases

| 情况 | 处理 |
|---|---|
| 玩家在世界边界 | 相机**不**跟随 |
| 玩家在战斗 + 群系过渡 | 战斗模式优先 |
| 玩家死亡在仪式中 | 死亡模式优先 |
| Boss 战在群系过渡带 | 战斗模式优先 |
| 仪式在聚落内 | Ritual 模式生效（拉远）|
| 多个仪式同时 | **不**可能（仪式是顺序的）|
| 相机超出世界 | clamp 到世界边界 |
| 玩家长按移动键 | 相机持续跟随 |
| 玩家在过渡带持续 10s | 1 秒后回 Follow（避免持续拉近）|

---

## F. Dependencies

### 上游（这个系统依赖谁）
- **Style-bible** —— 油画风锚定
- **Day-Night** —— 相机色调 + 光照
- **World-Exploration** —— 群系边界

### 下游（谁依赖这个系统）
- **所有 14 个 GDD** —— Camera 是它们的"镜头"
- **UI/HUD** —— HUD 元素位置基于相机

---

## G. Tuning Knobs（12 字段）

| 旋钮 | 默认值 | 范围 | 决策编号 | 影响 |
|---|---|---|---|---|
| `cameraFollowLerpSpeed` | 5f | 1-10 | #1 | 跟随速度 |
| `cameraCombatOffsetMultiplier` | 1.2f | 1-1.5 | #2 | 战斗相机偏移 |
| `cameraCombatLerpSpeed` | 3f | 1-5 | #2 | 战斗相机速度 |
| `cameraRitualZoom` | 0.8f | 0.5-1.0 | #3 | 仪式缩放（<1 = 拉远）|
| `cameraRitualLerpSpeed` | 2f | 1-5 | #3 | 仪式速度 |
| `cameraTransitionZoom` | 1.2f | 1.0-1.5 | #4 | 过渡缩放（>1 = 拉近）|
| `cameraTransitionDurationSec` | 1f | 0.5-3 | #4 | 过渡时长 |
| `cameraDeathFadeSec` | 0.5f | 0.2-2 | #5 | 死亡淡黑时长 |
| `cameraDeathZoom` | 0.7f | 0.5-1.0 | #5 | 死亡缩放 |
| `cameraFollowOffsetZ` | -10f | -20-0 | #6 | Z 偏移 |
| `cameraMaxZoom` | 2.0f | 1.5-3 | #7 | 最大缩放（v1.1）|
| `cameraMinZoom` | 0.5f | 0.3-1 | #7 | 最小缩放（v1.1）|

---

## H. Acceptance Criteria

### AC-1: 相机跟随玩家
- **条件**：玩家移动
- **结果**：相机跟随 + 0.1s 延迟

### AC-2: 战斗相机偏移
- **条件**：玩家进入战斗
- **结果**：相机偏移到玩家 + Boss 中点

### AC-3: 仪式相机拉远
- **条件**：玩家编织 / 送别 / 誓言完成
- **结果**：相机拉远看全景

### AC-4: 群系过渡缩放
- **条件**：玩家在群系过渡带
- **结果**：相机 1 秒缩放

### AC-5: 死亡黑屏
- **条件**：玩家死亡
- **结果**：0.5 秒渐黑 + 相机拉远

### AC-6: 相机边界
- **条件**：玩家在世界边界
- **结果**：相机**不**跟随（玩家也**不**能继续走）

### AC-7: 相机 + 群系色调
- **条件**：玩家进入新群系
- **结果**：相机色调 1 秒渐变

### AC-8: 相机 + Day-Night
- **条件**：Day-Night 转换
- **结果**：相机色调 smoothstep 1 秒

### AC-9: Boss 战相机
- **条件**：Boss 战开始
- **结果**：相机偏移（**不**抖动）

### AC-10: 奥丁审判相机
- **条件**：玩家进入深渊之心
- **结果**：相机拉远 3 分钟

### AC-11: 相机性能
- **条件**：所有相机模式生效
- **结果**：< 4ms / 帧

### AC-12: 相机 + VFX
- **条件**：仪式 VFX 触发
- **结果**：相机拉远看 VFX

---

## 10. 已锁定决策（Locked Decisions）

> 2026-07-27 用户拍板，12 个开放问题全部锁定。已落地为 §G 旋钮 + data-config v2.4。

| # | 决策点 | 锁定值 | 落地位置 |
|---|---|---|---|
| 1 | **相机跟随速度** | **5**（0.1s 延迟） | §C.2 模式 1 + §G |
| 2 | **战斗相机偏移** | **×1.2** | §C.2 模式 2 + §G |
| 3 | **战斗相机速度** | **3**（比 Follow 慢） | §C.2 模式 2 + §G |
| 4 | **仪式缩放** | **0.8**（<1 = 拉远） | §C.2 模式 3 + §G |
| 5 | **仪式速度** | **2**（缓慢） | §C.2 模式 3 + §G |
| 6 | **过渡缩放** | **1.2**（>1 = 拉近） | §C.2 模式 4 + §G |
| 7 | **过渡时长** | **1 秒** | §C.2 模式 4 + §G |
| 8 | **死亡淡黑时长** | **0.5 秒** | §C.2 模式 5 + §G |
| 9 | **死亡缩放** | **0.7**（拉远） | §C.2 模式 5 + §G |
| 10 | **Z 偏移** | **-10**（正交相机） | §C.2 模式 1 + §G |
| 11 | **最大缩放（v1.1）** | **2.0** | §C.7 + §G |
| 12 | **最小缩放（v1.1）** | **0.5** | §C.7 + §G |

### 决策之间的协同

- **#1 + #2 + #3 + #10**：5 跟随 + 1.2 战斗 + 3 战斗 + -10Z = **"叙事性镜头"**——战斗偏移但不抖动
- **#4 + #5 + #6 + #7**：0.8 仪式 + 2 速度 + 1.2 过渡 + 1s = **"仪式感"**——仪式拉远，过渡拉近
- **#8 + #9 + 死亡复活**：0.5s 黑屏 + 0.7 缩放 = **"死亡时刻"**——清晰但不冗长
- **#11 + #12**：v1.1 缩放范围 = **"留给玩家控制"**——v1.0 不实现

### 仍待 playtest 调参

| 待调项 | 候选范围 | 调参位置 |
|---|---|---|
| 相机跟随延迟 | 0.05s vs 0.1s vs 0.2s | `GameConfigSO.cameraFollowLerpSpeed` |
| Boss 战是否抖动 | 不抖 vs 抖 | v1.0 锁定 = 不抖 |
| 仪式拉远程度 | 0.7 vs 0.8 vs 0.9 | `GameConfigSO.cameraRitualZoom` |
| 死亡黑屏时长 | 0.5s vs 1s | `GameConfigSO.cameraDeathFadeSec` |
| v1.1 自由相机 | 启用 vs 不启用 | v1.1 决策 |

→ 这些都是 Prototype 阶段调参，不阻塞任何 GDD。

---

> 12 个开放问题待用户拍板。

1. **相机跟随速度**
   - 我的推荐：**5**（0.1s 延迟）
2. **战斗相机偏移**
   - 我的推荐：**×1.2**（明显但不过分）
3. **战斗相机速度**
   - 我的推荐：**3**（比 Follow 慢）
4. **仪式缩放（<1=拉远）**
   - 我的推荐：**0.8**（明显拉远）
5. **仪式速度**
   - 我的推荐：**2**（缓慢）
6. **过渡缩放（>1=拉近）**
   - 我的推荐：**1.2**（略拉近）
7. **过渡时长**
   - 我的推荐：**1 秒**
8. **死亡淡黑时长**
   - 我的推荐：**0.5 秒**
9. **死亡缩放**
   - 我的推荐：**0.7**（拉远）
10. **Z 偏移**
    - 我的推荐：**-10**（正交相机）
11. **最大缩放（v1.1）**
    - 我的推荐：**2.0**
12. **最小缩放（v1.1）**
    - 我的推荐：**0.5**

---

## Unity Implementation Notes

### 核心脚本（`Assets/Scripts/Camera/`）
- `RavensongCamera.cs` —— 相机主控（5 模式）
- `CameraFollow.cs` —— 跟随模式
- `CameraCombat.cs` —— 战斗模式
- `CameraRitual.cs` —— 仪式模式
- `CameraTransition.cs` —— 群系过渡
- `CameraDeath.cs` —— 死亡模式
- `CameraBoundary.cs` —— 相机边界

### 数据结构
```csharp
public enum CameraMode {
  Follow,        // 默认
  Combat,        // 战斗
  Ritual,        // 仪式
  Transition,    // 群系过渡
  Death,         // 死亡
}

public class CameraState {
  public CameraMode mode;
  public float zoom;
  public Vector3 offset;
  public float lerpSpeed;
}
```

### 状态机
```csharp
public enum CameraState {
  Following,      // 跟随
  Combat,         // 战斗
  Ritual,         // 仪式
  Transition,     // 过渡
  Death,          // 死亡
  Idle,           // 静止
}
```

### 事件订阅
```csharp
public class RavensongCamera : MonoBehaviour {
  public static event Action<CameraMode> OnModeChanged;
  public static event Action<float> OnZoomChanged;
  public static event Action OnDeathFade;
}
```

### 性能预算
- 相机主控：< 1ms / 帧
- 模式切换：< 2ms（一次性）
- 缩放过渡：< 2ms（持续 0.5s）
- 总计：< 4ms / 帧

### 资产制作
- **1 个 Main Camera**（Unity 内置）
- **Cinemachine**（可选，用于平滑相机）
- **6 群系色调**（material tint）
- **2 死亡 / 仪式 overlay**（黑屏 fade）

---

## 进度

| 章节 | 状态 |
|---|---|
| A. Overview | ✅ |
| B. Player Fantasy | ✅ |
| C. Detailed Design (9 小节) | ✅ |
| D. Formulas (5 个) | ✅ |
| E. Edge Cases (9 种) | ✅ |
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
| 2026-07-27 | v1.0 LOCKED | 12 开放问题用户拍板全部锁定；data-config v2.4 同步升级（camera 字段） | Mavis + 用户 |
