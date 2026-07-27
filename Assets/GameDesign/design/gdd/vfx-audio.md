# VFX & Audio — System GDD

> **Status**: 🔒 LOCKED v1.0
> **Author**: Mavis + 用户
> **Last Updated**: 2026-07-27
> **Implements Pillar**: **Foundation**（L5 表现层；锚定 `style-bible.md` 油画风 + Norse 配乐）
> **See Also**: `style-bible.md`（油画风锚定）/ `vfx.md`（VFX 视觉）/ `ui-hud.md` §C.1 规则 6（UI 声音）/ `death-sendoff.md` §C.7（送别演出音效）

---

## A. Overview

**Audio 是 Ravensong 的"叙事性听觉"——所有"非视觉"都通过 Audio 呈现：配乐、音效、UI 反馈、群系氛围。** Audio **不**是"背景音乐 + 打击音效"（避免成为 1 个 generic indie game），而是**叙事性 Audio**——每个声音都**对应一个游戏时刻**，让玩家**听到**那一刻的重量。

Ravensong 的 Audio 设计哲学是 **"叙事性听觉"（Narrative Audio）**：
- **少而精**——同时播放的 AudioSource < 8 个
- **北欧风格**——norse folk / ambient / 黑金属（不带主旋律）
- **少文字**——语音以吟唱 / 哼唱为主，**不**用语音对话
- **动态混音**——Day-Night / 群系 / 仪式 → Audio 实时切换

Audio 4 大类（v1.0）：
- **配乐（Music）**——主菜单 / 游戏 / 聚落 / 战斗 / 仪式
- **环境音（Ambient）**——6 群系 + Day-Night + 天气
- **游戏音效（SFX）**——攻击 / 受击 / 拾取 / 编织 / 送别
- **UI 反馈（UI Sound）**——按钮 / 弹窗 / Toast / 教程

数据层由**新增**的 `AudioPresetSO`（data-config.md C.2 类型 16）驱动；本 GDD 专注于**4 类 Audio 清单、动态混音、性能预算、跨系统 Audio 协同**。

---

## B. Player Fantasy

### 主幻想
> "我在白桦林听到远处的风笛声 + 篝火噼啪声 + 鸟鸣——进入战斗后音乐突然转紧张，Boss 战后又回到平静。这就是 Ravensong 的'声音世界'。"

### 关键体验时刻

- **第一次**主菜单音乐：norse folk 弦乐 + 风声
- **第一次**进入群系：白桦林 = 鸟鸣 + 风声
- **第一次**编织音效：3 秒内"叮" + 符文浮动声
- **第一次**送英灵殿：奥丁号角 + 挽歌 + 化光声
- **第一次**Boss 战：紧张弦乐 + 鼓点 + Boss 咆哮
- **第一次**奥丁审判：奥丁独白（3 min 配音）

---

## C. Detailed Design

### C.1 核心规则（Core Rules）

#### 规则 1：同时播放 AudioSource < 8 个
- 配乐：1 个（背景）
- 环境音：2-3 个（群系 + 天气）
- 战斗 SFX：1-2 个（动态）
- UI 反馈：1 个（按需）
- 仪式音效：1-2 个（重要时刻）
- 总和 ≤ 8

#### 规则 2：Audio 锚定 Norse 风格
- **配乐**：norse folk / 黑金属 / ambient
- **不使用**：流行 / 古典 / 电子
- **音色**：北欧乐器（norse flute / tagelharpa / 鼓）

#### 规则 3：动态混音（关键）
- **Day-Night**：白天 = 鸟鸣 / 夜晚 = 虫鸣 + 风
- **群系**：白桦林 = 鸟 / 永冻崖 = 风 + 雪声
- **战斗**：紧张弦乐 / Boss 战 = 重鼓
- **仪式**：norse 号角 / 送别 = 挽歌
- **玩家位置**：聚落 = 篝火 / 野外 = 群系环境

#### 规则 4：Audio 性能预算
- 配乐：1 个 audio source（持续）
- 环境音：2-3 个 audio source（持续）
- 总 AudioSource：< 8
- 总 CPU：< 4ms / 帧

#### 规则 5：Audio 跨系统协同（vfx 联动）
- VFX 触发时**自动**播 SFX
- 例：编织 VFX = "叮" 声 / 送英灵殿 = 奥丁号角
- 玩家**不**需要手动绑定

#### 规则 6：UI 反馈声音
- 按钮悬停 = 轻 "ding"
- 按钮点击 = 中 "叮"
- 弹窗打开 = 低沉 "咚"
- 弹窗关闭 = 反向 "咚"
- 玩家**可**在设置关闭 UI 音

#### 规则 7：Audio 可在设置调整
- 主音量（0-100%）
- 音乐音量（0-100%）
- 环境音量（0-100%）
- SFX 音量（0-100%）
- UI 音音量（0-100%）
- 字幕（v1.1 决策）

#### 规则 8：Audio 在 UI 主题切换时调整
- 白天 = 音乐 80%（低）
- 夜晚 = 音乐 100%（高）
- 战斗 = 音乐 120%（峰值）
- 仪式 = 音乐 90%（平衡）

---

### C.2 配乐（Music）

#### 5 类配乐

| 配乐 | 触发 | 时长 | 风格 |
|---|---|---|---|
| **主菜单** | 主菜单 | 2 min 循环 | norse folk 弦乐 |
| **游戏探索** | 游戏中（非战斗）| 5 min 循环 | ambient + 风声 |
| **聚落** | 玩家在聚落 | 3 min 循环 | 篝火 + 弦乐 |
| **战斗** | 进入战斗 | 2 min 循环 | 紧张弦乐 + 鼓点 |
| **仪式** | 编织/送别/誓言 | 1-3 min 循环 | norse 号角 + 神圣 |
| **奥丁审判** | 终局 | 3 min 一次性 | 独白 + 神圣音乐 |

#### 配乐切换
- 玩家进入不同场景 → 配乐**淡入淡出** 2 秒
- 同时**只**有 1 个配乐播放
- v1.0 决策：不做"配乐层叠"（避免 8 个 audio source 超限）

---

### C.3 环境音（Ambient）

#### 6 群系环境音

| 群系 | 声音 | 音量 |
|---|---|---|
| **白桦林** | 鸟鸣 + 风声 | 50% |
| **白骨原** | 风声 + 骨粉沙沙 | 40% |
| **深渊沼** | 沼泽 + 虫鸣 | 60% |
| **永冻崖** | 暴风雪 + 寒风 | 70% |
| **奥丁圣所** | 神圣 + 卢恩 | 30% |
| **深渊之心** | 暗物质 + 奥丁呼吸 | 50% |

#### 群系过渡环境音
- 群系 A + 群系 B 环境音**同时**播放，音量各 50%
- 1 秒渐变

#### 天气环境音
- **晴朗**：无
- **多云**：风声 +20%
- **雨**：雨声
- **雪**：雪声
- **雾**：低沉嗡鸣
- **暴风雪**：暴风 + 雪
- **神显**：norse 号角（满月）

---

### C.4 游戏音效（SFX）

#### 攻击音效
- **轻击**：剑光（0.2s）
- **重击**：重砍（0.4s）
- **Fate-Thread Bind**：cyan 缠绕（1s）
- **Boss 攻击**：boss 咆哮（0.5s）

#### 受击音效
- **玩家受击**：打击（0.2s）
- **敌人受击**：打击（0.1s）
- **Boss 受击**：重击（0.3s）

#### 死亡音效
- **普通敌人**：消散（1s）
- **精英**：金色消散（1.5s）
- **Boss**：boss 死亡吼（5s）

#### 状态音效
- **燃烧**：火焰（持续）
- **冰冻**：冰晶（持续）
- **中毒**：气泡（持续）
- **祝福**：norse 号角（持续）

#### 资源 / 拾取
- **拾取资源**：轻 "叮"
- **拾取 Tier 4+**：金 "叮"
- **拾取 Tier 5 传说**：神圣号角

#### 编织音效
- **编织中**：norse 符文声（持续 3s）
- **编织成功**：金 "叮"
- **编织失败**：灰 "咚"

#### 送别音效
- **送英灵殿**：奥丁号角 + 挽歌（10s）
- **战斗葬礼**：野兽低吼 + 战斗号角（5s）
- **简单葬礼**：挖土声 + 吟游诗人吟唱（5s）
- **强留**：沉默 + 炉火减弱（3s）
- **让其安息**：无（0s）

#### 誓言 / 奥丁
- **誓言完成**：norse 号角（5s）
- **奥丁审判**：奥丁独白（3 min 配音）

---

### C.5 UI 反馈（UI Sound）

#### 按钮
- **悬停**：轻 "ding"（0.1s）
- **点击**：中 "叮"（0.1s）
- **禁用**：低 "咚"（0.1s）

#### 弹窗
- **打开**：低 "咚"（0.3s）
- **关闭**：反向 "咚"（0.3s）

#### Toast
- **资源拾取**：轻 "叮"（0.2s）
- **任务完成**：金 "叮"（0.5s）
- **教程提示**：米色 "ding"（0.3s）
- **错误提示**：红 "咚"（0.2s）

#### 死亡 / 复活
- **玩家死亡**：低沉 "咚"（1s）
- **玩家复活**：轻 "ding"（0.5s）

---

### C.6 动态混音

#### 混音规则
- **主音量 × 各通道音量** = 实际播放音量
- 例：主音量 80% × 音乐 50% = 40% 实际音量
- 各通道独立调整

#### 实时调整
- 战斗进入：音乐 × 1.5 / 环境 × 0.5
- 仪式：音乐 × 1.2 / 环境 × 0.3
- 夜晚：环境 × 1.2（增强神秘感）
- 死亡：音乐 × 0（静音）

#### 混音淡变
- 所有音量调整 1 秒淡变（避免突兀）
- 跨场景音乐切换 2 秒淡入淡出

---

### C.7 Audio 性能预算

#### AudioSource 分配
| 类型 | 数量 | 优先级 |
|---|---|---|
| 配乐 | 1 | 最高 |
| 环境音 | 2-3 | 高 |
| 战斗 SFX | 1-2 | 高 |
| 仪式 SFX | 1-2 | 中 |
| UI 反馈 | 1 | 低 |
| 状态 SFX | 1 | 低 |
| **总计** | **< 8** | - |

#### 总预算
- 8 个 audio source
- < 4ms / 帧
- 60 FPS 目标

---

### C.8 与其他系统的协同

| 系统 | 协同 Audio |
|---|---|
| **Day-Night** | 环境音昼夜差异 + 配乐音量调整 |
| **Fate-Thread** | 编织音效（3s）|
| **Death-Send-off** | 5 种送别音效 |
| **Oath** | 誓言完成音效 + 奥丁独白 |
| **Einherjar** | 招募音效（5s）|
| **Settlement** | 聚落篝火音 + 升级音 |
| **Combat** | 攻击/受击/死亡/状态音 |
| **Weather** | 6 天气环境音 |
| **UI/HUD** | UI 反馈音 |

---

## D. Formulas

### D.1 动态混音
```csharp
float GetMusicVolume() {
  float base = 1.0f;
  if (inCombat) base *= 1.5f;
  if (inRitual) base *= 1.2f;
  if (isNight) base *= 1.0f;
  return Mathf.Clamp(base, 0f, 1.5f);
}
```

### D.2 Audio LOD（距离）
```csharp
bool IsAudioAudible(Vector3 pos, float maxDistance) {
  float dist = Vector3.Distance(player.position, pos);
  return dist < maxDistance;
}
```

### D.3 Audio 优先级
```csharp
int GetAudioPriority(AudioType type) {
  return type switch {
    Music => 0,        // 最高
    Ambient => 32,
    CombatSFX => 64,
    RitualSFX => 96,
    StatusSFX => 128,
    UISound => 160,    // 最低
    _ => 128
  };
}
```

### D.4 混音淡变
```csharp
IEnumerator FadeVolume(AudioSource source, float from, float to, float duration) {
  for (float t = 0; t < duration; t += Time.unscaledDeltaTime) {
    source.volume = Mathf.Lerp(from, to, t / duration);
    yield return null;
  }
  source.volume = to;
}
```

---

## E. Edge Cases

| 情况 | 处理 |
|---|---|
| 8 个 audio source 全占用 | 优先级最低的**停止** |
| 玩家在聚落内战斗 | 配乐**不**切战斗（聚落是庇护所）|
| 多个 SFX 同时触发 | 按优先级 + 时间差 |
| 玩家设备音频延迟 | 0.1s 缓冲 |
| 玩家色弱 / 听障 | UI 提示音 + 视觉反馈 |
| 奥丁独白时退出 | 独白进度**保留** |
| 玩家静音 | 配乐 + SFX 静音，**保留** UI 音（默认）|
| 战斗 → 仪式 切换 | 战斗音 1s 淡出 + 仪式音 0.5s 淡入 |
| 群系过渡音爆 | 各 50% 音量混合 |

---

## F. Dependencies

### 上游（这个系统依赖谁）
- **Style-bible** —— Norse 风格锚定
- **VFX** —— VFX 触发联动 Audio
- **Data Config** —— `AudioPresetSO` 是新类型 16

### 下游（谁依赖这个系统）
- **所有 14 个 GDD** —— Audio 是它们的"声音层"
- **UI/HUD** —— UI 反馈音

---

## G. Tuning Knobs（12 字段）

| 旋钮 | 默认值 | 范围 | 决策编号 | 影响 |
|---|---|---|---|---|
| `audioMaxSources` | 8 | 4-16 | #1 | AudioSource 上限 |
| `audioMasterVolume` | 0.8f | 0-1 | #2 | 主音量 |
| `audioMusicVolume` | 0.5f | 0-1 | #3 | 音乐音量 |
| `audioAmbientVolume` | 0.7f | 0-1 | #3 | 环境音量 |
| `audioSfxVolume` | 0.8f | 0-1 | #3 | SFX 音量 |
| `audioUiVolume` | 0.6f | 0-1 | #3 | UI 音量 |
| `audioDayNightMult` | 1.0f | 0.5-1.5 | #4 | 夜晚环境音量倍率 |
| `audioCombatMusicMult` | 1.5f | 1-2 | #4 | 战斗音乐音量倍率 |
| `audioRitualMusicMult` | 1.2f | 1-1.5 | #4 | 仪式音乐音量倍率 |
| `audioFadeSec` | 1.0f | 0-3 | #5 | 混音淡变时长 |
| `audioMusicFadeSec` | 2.0f | 1-5 | #5 | 跨场景音乐切换时长 |
| `audioOdinTrialVolume` | 1.0f | 0-1 | #6 | 奥丁审判音量 |

---

## H. Acceptance Criteria

### AC-1: 配乐自动切换
- **条件**：玩家进入不同场景
- **结果**：配乐 2 秒淡入淡出

### AC-2: 群系环境音
- **条件**：玩家进入任一群系
- **结果**：群系环境音自动播放

### AC-3: 战斗音乐
- **条件**：玩家进入战斗
- **结果**：紧张弦乐 + 鼓点自动播放

### AC-4: 仪式音效
- **条件**：玩家编织 / 送别 / 完成誓言
- **结果**：仪式音效自动触发

### AC-5: UI 反馈
- **条件**：玩家点击按钮
- **结果**：UI 反馈音 0.1s

### AC-6: 动态混音
- **条件**：玩家进入战斗
- **结果**：音乐 × 1.5 / 环境 × 0.5

### AC-7: 音量调整
- **条件**：玩家在设置调整音量
- **结果**：实时生效

### AC-8: AudioSource 限制
- **条件**：8 个 audio source 全占用
- **结果**：优先级最低的停止

### AC-9: 奥丁独白
- **条件**：玩家进入深渊之心
- **结果**：3 分钟奥丁独白

### AC-10: VFX 联动
- **条件**：VFX 触发
- **结果**：同步 SFX 自动播放

### AC-11: 群系过渡音
- **条件**：玩家在过渡带
- **结果**：群系音 1 秒渐变

### AC-12: 静音设置
- **条件**：玩家主音量 = 0
- **结果**：所有通道静音

---

## 10. 已锁定决策（Locked Decisions）

> 2026-07-27 用户拍板，12 个开放问题全部锁定。已落地为 §G 旋钮 + data-config v2.3。

| # | 决策点 | 锁定值 | 落地位置 |
|---|---|---|---|
| 1 | **AudioSource 上限** | **8**（性能平衡） | §C.1 规则 1 + §G |
| 2 | **主音量** | **0.8** | §C.7 + §G |
| 3 | **音乐/环境/SFX/UI 音量** | **0.5/0.7/0.8/0.6** | §C.7 + §G |
| 4 | **夜晚环境音量倍率** | **×1.0** | §C.3 + §G |
| 5 | **战斗音乐音量倍率** | **×1.5** | §C.6 + §G |
| 6 | **仪式音乐音量倍率** | **×1.2** | §C.6 + §G |
| 7 | **混音淡变时长** | **1 秒** | §C.6 + §G |
| 8 | **跨场景音乐切换时长** | **2 秒** | §C.2 + §G |
| 9 | **奥丁审判音量** | **1.0** | §C.2 + §G |
| 10 | **VFX → SFX 联动** | **v1.0 完整实现** | §C.1 规则 5 + §G |
| 11 | **语音对话** | **v1.0 不实现**（仅吟唱/哼唱） | §C.1 + §C.4 |
| 12 | **Audio 调试模式** | **v1.0 不实现**（v1.1） | §C.1 规则 8 |

### 决策之间的协同

- **#1 + #2-#6 + #7**：8 source + 多通道音量 + 1s 淡变 = **"动态混音"**——vfx 联动 + Day-Night 自动
- **#8 + #9 + #10**：2s 跨场景 + 奥丁 1.0 + VFX→SFX = **"仪式感"**——奥丁审判有专属音量
- **#11 + #12**：无语音对话 + 无调试 = **"MVP 范围控制"**——v1.0 聚焦核心

### 仍待 playtest 调参

| 待调项 | 候选范围 | 调参位置 |
|---|---|---|
| 6 群系环境音平衡 | 当前 vs ×1.5 | `AudioPresetSO.volume` |
| 战斗音峰值 | ×1.5 vs ×2 | `GameConfigSO.audioCombatMusicMult` |
| 8 source 限制 | 8 vs 12 | `GameConfigSO.audioMaxSources` |
| 奥丁独白 | 配音 vs 文字 | 资产决策 |
| 配乐层叠（v1.1）| 单音 vs 多层 | v1.1 设计 |

→ 这些都是 Prototype 阶段调参，不阻塞任何 GDD。

---

> 12 个开放问题待用户拍板。

1. **AudioSource 上限**
   - 我的推荐：**8**（性能平衡）
2. **主音量默认**
   - 我的推荐：**0.8**（80%）
3. **音乐 / 环境 / SFX / UI 音量默认**
   - 我的推荐：**0.5 / 0.7 / 0.8 / 0.6**（音乐轻 / SFX 重）
4. **夜晚环境音量倍率**
   - 我的推荐：**×1.0**（不特别加强）
5. **战斗音乐音量倍率**
   - 我的推荐：**×1.5**（明显但不过分）
6. **仪式音乐音量倍率**
   - 我的推荐：**×1.2**（平衡）
7. **混音淡变时长**
   - 我的推荐：**1 秒**
8. **跨场景音乐切换时长**
   - 我的推荐：**2 秒**
9. **奥丁审判音量**
   - 我的推荐：**1.0**（满音量）
10. **VFX → SFX 联动**
    - 我的推荐：**v1.0 完整实现**（v1.1 决策：vfx-audio）
11. **语音对话**
    - 我的推荐：**v1.0 不实现**（仅吟唱 / 哼唱）
12. **Audio 调试模式**
    - 我的推荐：**v1.0 不实现**（v1.1）

---

## Unity Implementation Notes

### 核心脚本（`Assets/Scripts/Audio/`）
- `AudioManager.cs` —— Audio 管理（8 source 限制 + 优先级）
- `MusicPlayer.cs` —— 配乐（5 类 + 跨场景切换）
- `AmbientPlayer.cs` —— 环境音（6 群系 + 6 天气）
- `SFXPool.cs` —— SFX 池（按需播放）
- `DynamicMixer.cs` —— 动态混音（战斗/仪式/Day-Night）
- `AudioPresetCache.cs` —— AudioPresetSO 缓存

### 数据结构
```csharp
public class AudioPreset {
  public AudioPresetSO data;             // Audio 预设 SO
  public AudioType type;                 // 4 类
  public AudioClip clip;                 // 音频片段
  public float volume;                   // 音量
  public float pitch;                    // 音调
  public bool loop;                      // 循环
}

public enum AudioType {
  Music,        // 配乐
  Ambient,      // 环境音
  SFX,          // 游戏音效
  UI,           // UI 反馈
}
```

### 状态机
```csharp
public enum AudioState {
  Idle,
  Playing,
  Fading,
  Paused,
  Stopped,
}
```

### 事件订阅
```csharp
public class AudioManager : MonoBehaviour {
  public static event Action<AudioPreset> OnAudioTriggered;
  public static event Action<AudioType> OnVolumeChanged;
  public static event Action<MusicType> OnMusicChanged;
}
```

### 性能预算
- Audio Manager：< 1ms / 帧
- 8 个 audio source
- < 4ms / 帧（音频处理）
- 60 FPS 目标

### 资产制作（v1.0 关键 Audio）
- **6 配乐**：主菜单 / 探索 / 聚落 / 战斗 / 仪式 / 奥丁审判
- **6 群系环境音**：每群系 1 套（5-10 个 audio clip）
- **6 天气环境音**：晴 / 多云 / 雨 / 雪 / 雾 / 暴风雪
- **~30 SFX**：攻击 / 受击 / 死亡 / 状态 / 资源 / 编织 / 送别
- **~10 UI 音**：按钮 / 弹窗 / Toast
- **总计：~60 个 audio clip**

---

## 进度

| 章节 | 状态 |
|---|---|
| A. Overview | ✅ |
| B. Player Fantasy | ✅ |
| C. Detailed Design (8 小节) | ✅ |
| D. Formulas (4 个) | ✅ |
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
| 2026-07-27 | v1.0 LOCKED | 12 开放问题用户拍板全部锁定；data-config v2.3 同步升级 + 新增 `AudioPresetSO`（类型 16） | Mavis + 用户 |
