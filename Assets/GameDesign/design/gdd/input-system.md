# Input System — System GDD

> **Status**: 🔒 LOCKED v1.0
> **Author**: Mavis + 用户
> **Last Updated**: 2026-07-27
> **Implements Pillar**: **Foundation**（支持全部 4 根支柱）

---

## A. Overview

**Input System 是 Ravensong 的"输入中枢"——所有系统都通过统一的 Action Map 与玩家输入对接。** 任何系统都不直接读键盘/手柄原始输入，全部通过 `InputAction` 抽象层。这保证：
- **键鼠/手柄双设备等价**（PC 玩家用任何设备都不影响体验）
- **可重映射**（v1.1 让玩家自定义）
- **无输入延迟**（直接处理，无中间层）
- **跨系统隔离**（UI 不响应战斗输入，对话不响应移动）

数据层由 `GameConfigSO.input*` 字段驱动；本 GDD 专注于**Action Map 设计、缓冲、设备适配**。

---

## B. Player Fantasy

> 关键体验时刻（不是叙事，是手感）：
- 按下攻击键 → **立即**响应（无 1 帧延迟）
- 战斗连击：轻击+重击 +轻击+重击 → 流畅触发，**不丢输入**
- 切换手柄/键鼠 → 操作**完全等价**
- 进入对话 / 菜单 → 战斗输入自动屏蔽，**不误触**
- 死亡画面 → 按键立即响应，**不卡死**

### 关键体验目标
- "**我按什么，游戏就做什么**"—— 这是 Input System 的最高准则
- 任何"按了没反应"或"没按却触发"都是 Input 系统的 bug

---

## C. Detailed Design

### C.1 核心规则（Core Rules）

#### 规则 1：所有系统订阅 InputAction，不读 raw input
- 不允许 `Input.GetKey(...)` / `Input.GetButton(...)` 直接调用
- 必须通过 `InputAction.Performed` / `InputAction.Canceled` 事件订阅
- 例外：UI 系统的 raycast 仍用 `EventSystem`

#### 规则 2：4 个 Action Map 互斥
- **Gameplay**：默认激活，战斗中 / 探索中
- **UI**：打开 Inventory / Map / Oath 时激活
- **Dialogue**：对话播放时激活
- **Cutscene**：演出时激活

任意时刻**只有一个 Action Map 激活**。切换通过 `InputManager.SwitchMap(mapName)`。

#### 规则 3：Action 3 种类型
- **Button**：单次触发（按 / 释放 / 持续）
- **Vector2**：方向输入（移动、瞄准、UI 导航）
- **Pass-through**：组合输入（WASD 整合为 Vector2）

#### 规则 4：所有 Action 必须可在键鼠 + 手柄上完成
- 每个 Action 必须有键鼠绑定 + 手柄绑定
- 例外：少数 UI-only Actions 可只支持键鼠（MVP 不考虑）

#### 规则 5：输入缓冲（Input Buffer）
- 战斗连击（轻击 → 重击）支持输入缓冲
- 缓冲窗口：**0.2 秒**（0.2s 内输入的下一个动作会被"记住"）
- 仅 Combat 系统使用（其他系统不需要）

### C.2 Action Maps 详解

#### Action Map 1: Gameplay（默认）

| Action | 类型 | 键鼠 | 手柄 | 用途 |
|---|---|---|---|---|
| **Move** | Vector2 | WASD | Left Stick | 角色移动 |
| **LookAim** | Vector2 | Mouse Position | Right Stick | 攻击/织线方向 |
| **Attack** | Button (按) | Left Click | RT (Right Trigger) | 轻攻击 |
| **HeavyAttack** | Button (按) | Right Click | RB (Right Bumper) | 重攻击 |
| **Block** | Button (持续) | Right Mouse Hold | LT (Left Trigger) | 防御 |
| **Dodge** | Button (按) | Space | B button | 闪避 |
| **Interact** | Button (按) | E | A button | 交互（拾取 / 对话 / 招募） |
| **UseQuickbar1** | Button (按) | 1 | D-pad Up | 使用热键 1 |
| **UseQuickbar2** | Button (按) | 2 | D-pad Right | 使用热键 2 |
| **UseQuickbar3** | Button (按) | 3 | D-pad Down | 使用热键 3 |
| **UseQuickbar4** | Button (按) | 4 | D-pad Left | 使用热键 4 |
| **DropItem** | Button (按) | Q | Y button | 丢弃物品 |
| **OpenInventory** | Button (按) | Tab | Start | 打开背包 |
| **OpenMap** | Button (按) | M | Back / Select | 打开地图 |
| **OpenOath** | Button (按) | O | Right Stick Click | 打开誓言页 |
| **Pause** | Button (按) | Escape | Start (长按) | 暂停 |
| **Confirm** | Button (按) | Enter | A button | 通用确认 |
| **Cancel** | Button (按) | Escape | B button | 通用取消 |

**Gameplay 中按 OpenInventory / OpenMap / OpenOath → 切换到 UI Map**

#### Action Map 2: UI

| Action | 类型 | 键鼠 | 手柄 | 用途 |
|---|---|---|---|---|
| **Navigate** | Vector2 | Arrow keys / WASD | Left Stick | UI 导航 |
| **Submit** | Button (按) | Enter / Left Click | A button | 确认 |
| **Cancel** | Button (按) | Escape / Right Click | B button | 取消 |
| **Point** | Vector2 | Mouse Position | Right Stick | 鼠标/手柄指针 |
| **Click** | Button (按) | Left Click | RT | 点击 |
| **RightClick** | Button (按) | Right Click | RB | 右键 |
| **Scroll** | Vector2 | Mouse Scroll | D-pad | 滚动 / 翻页 |
| **TabLeft** | Button (按) | Q | LB | 切到上一 Tab |
| **TabRight** | Button (按) | E | RB | 切到下一 Tab |

**UI 中按 Cancel / 关闭键 → 切回 Gameplay Map**

#### Action Map 3: Dialogue

| Action | 类型 | 键鼠 | 手柄 | 用途 |
|---|---|---|---|---|
| **NextLine** | Button (按) | Space / Left Click | A button | 下一句对话 |
| **Skip** | Button (持续) | Shift | Y button | 加速对话 |
| **ChooseOption1-4** | Button (按) | 1-4 | D-pad | 选择分支 |
| **Cancel** | Button (按) | Escape | B button | 退出对话 |

#### Action Map 4: Cutscene

| Action | 类型 | 键鼠 | 手柄 | 用途 |
|---|---|---|---|---|
| **Skip** | Button (按) | Escape | Start | 跳过演出 |

**Cutscene 持续时间 ≥ 5 秒才允许 Skip**（防止误触）

### C.3 Action 状态机

```
Action Created (enabled = true)
        ↓
Performed (按下)         Canceled (释放)
        ↓                      ↓
Trigger callback         Trigger callback
        ↓                      ↓
Action Disabled (enabled = false)
        ↓
No callbacks
```

- **Performed**：按下瞬间触发（用于"按一下"类）
- **Canceled**：释放瞬间触发（用于"按着"类）
- **Started**：按下开始（用于"按住"动画）
- **Hold**（持续）：按住期间每帧触发

### C.4 输入设备适配

#### 键盘 + 鼠标
- 移动：WASD
- 瞄准：Mouse Position → 屏幕坐标 → 世界坐标
- 攻击：Left Click
- 重攻击：Right Click
- 防御：Right Mouse Hold

#### 手柄
- 移动：Left Stick（带 0.2 deadzone）
- 瞄准：Right Stick（带 0.15 deadzone）
- 攻击：RT（扳机）
- 重攻击：RB
- 防御：LT
- 菜单：Start / Back / Y

#### 设备切换
- Unity Input System **自动检测**最近活动设备
- 玩家可以用任意设备操作，无需手动切换
- UI 提示根据当前设备显示对应图标（手柄时显示 🎮，键鼠时显示 ⌨️）

### C.5 输入缓冲（Input Buffer）

**目的**：让战斗连击"流畅"——玩家快速按轻击+重击，不会因为两个动作间隔太短而漏输入。

```csharp
// Combat 系统订阅
combatInputBuffer.BufferAction("HeavyAttack", HeavyAttack.performed);
combatInputBuffer.BufferAction("Attack", Attack.performed);

void OnAttackPerformed() {
  if (currentState == State.Attacking) {
    combatInputBuffer.TryConsumeBuffered("HeavyAttack");  // 立即触发
  } else {
    // 触发当前攻击
  }
}
```

**规则**：
- 缓冲窗口：**0.2 秒**（GameConfigSO.inputBufferWindowSec）
- 每个 Action 缓冲**最近一次**（同 Action 多次按只记最后一次）
- Combat 状态机决定是否消费 buffer
- 离开战斗状态 → 清空 buffer

### C.6 Deadzones

| 设备 | 死区 | 理由 |
|---|---|---|
| Left Stick | 0.2 | 避免摇杆漂移（很多手柄有） |
| Right Stick | 0.15 | 瞄准用，比移动略灵敏 |
| Left Trigger | 0.1 | 防御要求即时响应 |
| Right Trigger | 0.1 | 攻击要求即时响应 |

**可调参**（`GameConfigSO`）：
- `inputLeftStickDeadzone` (默认 0.2)
- `inputRightStickDeadzone` (默认 0.15)
- `inputTriggerDeadzone` (默认 0.1)

### C.7 重映射（v1.1 决策）

- **MVP 不做重映射**（键位 hardcode）
- **v1.1 添加重映射 UI**：
  - 玩家进入 Settings → Controls
  - 选择要重映射的 Action
  - 按下要绑定的新键
  - 写入 PlayerPrefs（持久化）
  - **冲突检测**：已绑定的键不能再绑（除非用户确认覆盖）

### C.8 鼠标灵敏度

**MVP 硬编码**：
- 瞄准：`Mouse Position`（绝对位置 → 世界坐标）—— **不需要灵敏度**
- 移动：WASD 数字输入 → **不需要灵敏度**

**v1.1 可加**：
- 鼠标手柄切换（手柄瞄准 = 相对移动，需要灵敏度）
- 灵敏度：默认 1.0，可调 0.5-2.0

### C.9 暂停行为

| Action Map | Pause 行为 |
|---|---|
| Gameplay | 按 Pause → 暂停游戏，激活 UI Map 的 Pause 子面板 |
| UI | 按 Pause → 关闭 UI 子面板，恢复 Gameplay |
| Dialogue | Pause 不可用（对话期间不暂停） |
| Cutscene | Pause 不可用（演出期间不暂停） |

**暂停时**：
- Time.timeScale = 0
- 所有动画停止
- Audio 暂停
- Input 仍可接收（玩家可以点 Pause 解除）

### C.10 与其他系统的交互

| 系统 | 怎么用 Input |
|---|---|
| **Combat** | Attack / HeavyAttack / Block / Dodge + 缓冲 |
| **Movement** | Move（Vector2）→ 玩家位置 |
| **Inventory** | OpenInventory / DropItem / Interact |
| **Fate-Thread** | LookAim（方向） + Attack（释放丝线） |
| **Day-Night** | 不直接订阅 Input |
| **Einherjar** | Interact（招募） + 后续分配的快捷键 |
| **Oath** | OpenOath |
| **World Exploration** | Interact（POI 进入） |
| **Quest-Event** | ChooseOption（事件选项） |
| **UI/HUD** | 所有 UI Action |
| **Save** | 不直接订阅 Input（手动 Save 通过菜单） |

---

## D. Formulas

### D.1 摇杆死区过滤
```csharp
Vector2 ApplyDeadzone(Vector2 input, float deadzone) {
  float magnitude = input.magnitude;
  if (magnitude < deadzone) return Vector2.zero;
  // 重映射：deadzone ~ 1 → 0 ~ 1
  float scaledMagnitude = (magnitude - deadzone) / (1f - deadzone);
  return input.normalized * scaledMagnitude;
}
```

### D.2 鼠标位置转世界坐标
```csharp
Vector3 GetMouseWorldPosition(Camera cam) {
  Vector3 mousePos = Mouse.current.position.ReadValue();
  mousePos.z = -cam.transform.position.z;  // 2D 相机
  return cam.ScreenToWorldPoint(mousePos);
}
```

### D.3 输入缓冲
```csharp
class InputBuffer {
  Dictionary<string, float> buffered = new();
  
  void BufferAction(string actionName) {
    buffered[actionName] = Time.time;
  }
  
  bool TryConsumeBuffered(string actionName) {
    if (!buffered.ContainsKey(actionName)) return false;
    float bufferedTime = buffered[actionName];
    if (Time.time - bufferedTime > inputBufferWindowSec) {
      buffered.Remove(actionName);
      return false;
    }
    buffered.Remove(actionName);
    return true;
  }
}
```

### D.4 设备检测
```csharp
InputDevice GetActiveDevice() {
  // Unity Input System 自动提供
  if (Keyboard.current.anyKey.wasPressedThisFrame) return InputDevice.Keyboard;
  if (Gamepad.current.buttonSouth.wasPressedThisFrame) return InputDevice.Gamepad;
  if (Mouse.current.leftButton.wasPressedThisFrame) return InputDevice.Mouse;
  return InputDevice.LastUsed;
}
```

### D.5 手柄扳机阈值
```csharp
bool IsTriggerPressed(InputAction triggerAction, float threshold = 0.1f) {
  return triggerAction.ReadValue<float>() > threshold;
}
```

---

## E. Edge Cases

| 情况 | 处理 |
|---|---|
| 键鼠 + 手柄同时操作 | Unity 自动处理，最后操作设备优先 |
| 设备断开（手柄拔了） | 自动切回键鼠；UI 提示"手柄断开" |
| 同时按多个键 | Unity 多键支持，键位不会冲突 |
| 摇杆漂移 | Deadzone 过滤（0.2 默认） |
| 输入延迟（高延迟显示器） | 关闭 VSync 提升响应（v1.1 选项） |
| Pause 期间按 Pause | 关闭 Pause 子面板，恢复 Gameplay |
| 对话期间按 Attack | 屏蔽（Dialogue Map 激活，Attack 不存在） |
| 死亡画面期间 | 玩家按键继续 → 复活或重试 |
| 多语言键盘（AZERTY vs QWERTY） | MVP 默认 QWERTY；v1.1 加键盘布局检测 |
| 输入硬件不支持 | 优雅降级（例如没有手柄时所有手柄 Action 无效，不报错） |
| 同 Action 多次 rapid press | 输入缓冲只记**最后一次**（不堆叠） |
| Cutscene 期间按 Skip（< 5 秒） | 屏蔽（防误触） |
| Inventory 打开时按 Move | 屏蔽（UI Map 中 Move 不存在） |
| 触发 Action Map 切换时残留输入 | 切换时清空所有 Action 状态 |

---

## F. Dependencies

### 上游（Input 依赖谁）

- **Data Config** —— 调参字段（deadzone、buffer window）
- **Unity Input System Package** —— InputAction 抽象层

### 下游（谁依赖 Input）

- **所有 Gameplay 系统**（Combat / Movement / Inventory / Fate-Thread / etc.）
- **所有 UI 系统**（InventoryUI / MapUI / DialogueUI / etc.）

**Input 是 Ravensong 的"第一道关"**——所有交互系统的依赖根。

---

## G. Tuning Knobs

> 调参字段建议加到 `GameConfigSO`（data-config v1.3 阶段）

| 参数 | 默认值 | 调参影响 |
|---|---|---|
| `inputBufferWindowSec` | 0.2f | 战斗缓冲窗口（越小越严格） |
| `inputLeftStickDeadzone` | 0.2f | 左摇杆死区（避免漂移） |
| `inputRightStickDeadzone` | 0.15f | 右摇杆死区（瞄准更灵敏） |
| `inputTriggerDeadzone` | 0.1f | 扳机死区（更即时响应） |
| `enableInputRebinding` | false | v1.1 开启重映射（MVP false） |
| `mouseAimSensitivity` | 1.0f | 鼠标瞄准灵敏度（v1.1） |
| `gamepadAimSensitivity` | 1.0f | 手柄瞄准灵敏度（v1.1） |
| `minCutsceneSkipDurationSec` | 5f | Cutscene 多少秒后允许 Skip |
| `pauseFreezeTime` | true | Pause 是否冻结时间 |

---

## H. Acceptance Criteria

### AC-1: 所有 Action 可用键鼠 + 手柄
**测试**：
1. 用键鼠：WASD 移动、Left Click 攻击、Tab 开包
2. 拔掉键鼠（或不接），用手柄：Left Stick 移动、RT 攻击、Start 开包
3. **期望**：所有 Action 都有响应

### AC-2: Action Map 切换正确
**测试**：
1. Gameplay 中按 Tab → Inventory 打开
2. **期望**：Gameplay Map 关闭，UI Map 激活
3. 在 Inventory 中按 Escape → Inventory 关闭
4. **期望**：UI Map 关闭，Gameplay Map 激活

### AC-3: 输入缓冲
**测试**：
1. 战斗中轻击
2. 轻击后 0.1 秒内按重击
3. **期望**：重击**立即触发**（缓冲生效）
4. 0.3 秒后按重击
5. **期望**：重击**不触发**（缓冲过期）

### AC-4: 死区过滤
**测试**：
1. 摇杆漂移（保持在中间）
2. **期望**：Move 收到 Vector2.zero
3. 推动 0.1 距离
4. **期望**：仍然 Vector2.zero（deadzone 0.2）
5. 推动 0.5 距离
6. **期望**：收到 Vector2（0.5 - 0.2 / 0.8 = 0.375 magnitude）

### AC-5: 跨 Map 屏蔽
**测试**：
1. 进入对话（Dialogue Map 激活）
2. 按 WASD 移动
3. **期望**：**不**响应（Dialogue Map 没有 Move Action）
4. 退出对话（Gameplay Map 激活）
5. 按 WASD
6. **期望**：正常移动

### AC-6: 设备切换
**测试**：
1. 用键鼠开始游戏
2. 拔掉键鼠（模拟）
3. 接手柄
4. **期望**：操作无间断（Unity 自动切换设备）

### AC-7: Pause 行为
**测试**：
1. 战斗中按 Pause
2. **期望**：游戏暂停，Time.timeScale = 0
3. 按 Pause 解除
4. **期望**：Time.timeScale = 1，游戏继续

### AC-8: 性能预算
**测试**：
1. 100 个 InputAction 同时启用
2. **期望**：每帧 < 0.5ms 输入处理
3. 玩家操作 60 FPS 流畅

### AC-9: UI 设备图标自适应
**测试**：
1. 玩家用键鼠按 Tab
2. **期望**：UI 提示显示"⌨️ Tab"或类似
3. 玩家用手柄按 Start
4. **期望**：UI 提示显示"🎮 Start"

---

## 10. 已锁定决策（Locked Decisions）

> 2026-07-27 用户拍板，6 个开放问题全部锁定。已落地为 G 旋钮 + data-config v1.3。

| # | 决策点 | 锁定值 | 落地位置 |
|---|---|---|---|
| 1 | **重映射** | **MVP 不做**（v1.1 加，hardcode 足够 playtest） | §C.7 |
| 2 | **鼠标瞄准灵敏度** | **MVP 不需要**（绝对位置瞄准） | §C.8 |
| 3 | **手柄振动** | **简单支持**（Unity Input System 自带） | §C.4 |
| 4 | **触屏支持** | **MVP 不做**（Steam 优先） | §C.4 |
| 5 | **输入缓冲范围** | **只 Combat**（其他系统不需要） | §C.5 |
| 6 | **Cutscene Skip** | **5 秒后允许跳过** | §C.4 Cutscene Map |

### 决策之间的协同

- **#1 + #4**：MVP 不做重映射 + 不做触屏 = **最简实现**——所有玩家都用同一键位，移动端玩家暂不支持
- **#2 + #3**：绝对位置瞄准 + 手柄振动 = **键鼠和手柄体验等价**——键鼠无延迟瞄准，手柄有反馈补偿
- **#5 + #6**：Combat 缓冲 + Cutscene 5s Skip = **战斗流畅但演出不可打断**——避免玩家误触

### 仍待 playtest 调参（不阻塞 GDD 锁定）

| 待调项 | 候选范围 | 调参位置 |
|---|---|---|
| 缓冲窗口精度 | 0.2s vs 0.15s vs 0.25s | `GameConfigSO.inputBufferWindowSec` |
| 死区精度 | 0.2 / 0.15 / 0.1 vs 0.25 / 0.2 / 0.15 | `GameConfigSO.inputLeftStickDeadzone` 等 |

→ 这些都是 Prototype 阶段的**数值调参工作**，通过 `GameConfigSO` 直接改即可，不阻塞任何 GDD。

---

## Unity Implementation Notes

### 核心脚本（`Assets/Scripts/Input/`）
- `InputManager.cs` —— Action Map 切换单例
- `InputBuffer.cs` —— Combat 缓冲逻辑
- `DeviceDetector.cs` —— 当前活动设备检测
- `InputRebindingUI.cs` —— v1.1 重映射 UI
- `InputDebugger.cs` —— 调试用，开发者可看当前按下的所有 Action

### Input Actions Asset
- `Assets/Input/RavensongInput.inputactions` —— Unity Input System 资源
- 包含 4 个 Action Map + 所有 Action 定义
- 绑定配置：键鼠 + 手柄

### 事件订阅模式
```csharp
// Combat 系统订阅
Attack.performed += ctx => OnAttackPerformed();
HeavyAttack.performed += ctx => OnHeavyAttackPerformed();
Block.performed += ctx => OnBlockStarted();
Block.canceled += ctx => OnBlockReleased();
```

### Action Map 切换
```csharp
public class InputManager : MonoBehaviour {
  public void SwitchToGameplay() {
    GameplayMap.Enable();
    UIMap.Disable();
    DialogueMap.Disable();
    CutsceneMap.Disable();
  }
  
  public void SwitchToUI() {
    GameplayMap.Disable();
    UIMap.Enable();
    // ...
  }
}
```

### 性能预算
- Input Action 处理：< 0.1ms / 帧
- 设备检测：< 0.05ms / 帧
- 缓冲查询：< 0.01ms / 次

---

## 进度

| 章节 | 状态 |
|---|---|
| A. Overview | ✅ |
| B. Player Fantasy | ✅ |
| C. Detailed Design (10 小节) | ✅ |
| D. Formulas (5 个) | ✅ |
| E. Edge Cases (14 种) | ✅ |
| F. Dependencies | ✅ |
| G. Tuning Knobs (9 字段已落 v1.3) | ✅ |
| H. Acceptance Criteria (9 条) | ✅ |
| **10. Locked Decisions (6 决策)** | ✅ |
| Unity Implementation Notes | ✅ |

**总进度**: 10/10 (100%) 🔒

**🔒 已锁定 v1.0** —— 8 段全填 + 6 开放问题全部锁定 + 9 调参字段落 data-config v1.3。

---

## 变更日志

| 日期 | 版本 | 改动 | 责任人 |
|---|---|---|---|
| 2026-07-27 | v1.0 draft | 初版生成：10 段 + 公式 + Unity 实施 | Mavis |
| 2026-07-27 | **v1.0 LOCKED** | 6 开放问题用户拍板全部锁定；data-config v1.3 同步升级 | Mavis + 用户 |
