# Ravensong — Visual Style Bible

> **Status**: 🔒 LOCKED
> **锁定日期**: 2026-07-27
> **锁定方式**: 4 张锚点样图（8/7/7/7）+ 1 张主视觉 + 用户拍板
> **变更规则**: 见第 9 节。任何 Ravensong 资产的 AI 出图都必须遵守本文档。

---

## 1. 核心一句话

**Ravensong 的视觉 = 黑暗奇幻厚涂油画 + 北欧图形装饰 + 亲密仪式镜头。**

不是平面插画，不是 3D 渲染，不是卡通风。**永远是油画质感、永远是亲密镜头、永远是仪式感压过英雄感。**

---

## 2. 锚点图（Visual Anchors）

> **这 5 张图是 Ravensong 视觉的宪法。任何新出图都必须向它们看齐。**

| # | 文件路径 | 评分 | 角色 |
|---|---|---|---|
| 🦸 | `art-exploration/v3/01-valkyrie.png` | 8 | 角色锚点 |
| 🏛️ | `art-exploration/v3/02b-settlement-intimate.png` | 7 | 场景锚点 |
| 🧵 | `art-exploration/v3/03-thread.png` | 7 | 核心动词可视化 |
| 💀 | `art-exploration/v3/04-draugr.png` | 7 | 敌对生物锚点 |
| 🎬 | `art-exploration/hero-main.png` | 主视觉 | Steam 主页 / 封面 / 营销 |

> ⚠️ **不得删除这 5 张文件**。它们是"风格宪法"，新图气质漂移时的对照基准。

---

## 3. 调色板（Color Palette）

### 主色（必用，按比例分配）

| 色 | 名称 | Hex | 单图占比 | 用途 |
|---|---|---|---|---|
| 🔵 | Deep Midnight Blue | `#0A1A2F` | **60%** | 主导色：背景、阴影、负面空间、夜间 |
| 🩵 | Cyan Magic Light | `#4DD8E6` | **15%** | 命运丝线、神力、技能特效、眼睛高光 |
| 🟡 | Warm Gold | `#C9A567` | **10%** | 卢恩字符、装备金属、神器细节 |

### 辅色（氛围支撑，≤ 15%）

| 色 | 名称 | Hex | 用途 |
|---|---|---|---|
| ⚪ | Frost White | `#E8E8E8` | 雪、霜、星、神秘光点 |
| ⚫ | Charcoal Black | `#1A1A1A` | 最深阴影、死亡、虚无 |
| 🟦 | Ocean Blue | `#1B3A5C` | 中间过渡、远景大气 |

### 比例铁律
**任何单张图**: 60% Deep Blue / 15% Cyan / 10% Gold / 15% 辅色
- 偏离 ±10% 需返工
- 绝对禁止出现"无主导色"的灰色调画面

---

## 4. 必带关键词（AI Prompt 必出）

### 核心 5 个（每次必出，少一个不算 Ravensong）
```
oil painting
heavy visible brushstrokes
deep midnight blue
cyan magical light
chiaroscuro
```

### 完整关键词包（推荐全部使用）
```
dark fantasy oil painting
heavy visible brushstrokes
rich painterly texture
dramatic chiaroscuro lighting
deep midnight blue dominant
cyan magical light source
warm gold accents
intimate cinematic framing
mythological ritualistic atmosphere
Bastien Lecouffe-Deharme dark fantasy style
```

### 收尾约束（永远加在末尾）
```
NOT flat, NOT vector, NOT graphic stylized, NOT anime
```

---

## 5. 禁用关键词（明令禁止）

| 词 | 原因 |
|---|---|
| `flat`, `vector` | 失去油画质感 |
| `graphic stylized`, `anime`, `cartoon`, `chibi` | 错位风格 |
| `photorealistic`, `3D render`, `unreal engine` | 不像画 |
| `neon`, `vibrant`, `high saturation`, `bright cheerful` | 破坏暗调灵魂 |
| `cute`, `kawaii`, `minimalist`, `simple`, `low detail` | 失去厚涂 |
| `wide establishing shot`, `epic panorama` | 用户明确反对 |
| `low poly`, `voxel`, `pixel art` | 与高精 2D 路线冲突 |

---

## 6. 参考艺术家（气质锚点）

| 艺术家 / 来源 | 锁定角度 |
|---|---|
| **Bastien Lecouffe-Deharme** | 主参考 — MtG Innistrad / Dark Ascension 黑暗系列的油画质感 |
| **Brom** | 黑暗奇幻插画 — 厚重笔触与神秘氛围 |
| **Jason Benjamin** | 戏剧性光影与情绪渲染 |
| **Banner Saga 角色设计**（结构层参考） | 北欧图形 + 厚涂执行 |
| **Sable 游戏艺术**（克制感参考） | 调色板克制与构图 |

> 用词：`Bastien Lecouffe-Deharme dark fantasy style` 比 "Hades style" 准确得多（v1 教训）。

---

## 7. 构图规则（Composition Rules）

### 镜头选择

| 等级 | 镜头类型 | 占比 | 适用 |
|---|---|---|---|
| ✅ 优先 | **亲密 / 近景 / 聚焦主体** | 80% | 角色、关键道具、仪式时刻 |
| ✅ 推荐 | **3/4 角度俯视** | 15% | 场景图（避免纯 top-down 失温） |
| ⚠️ 慎用 | 半身像、特写 | 5% | UI 元素、图标 |
| ❌ 避免 | 远景建立镜头 | 0% | AI 表现差，用户明确反对 |

### 光源规则

- ✅ **画内光源**（in-frame light source）— 丝线、炉火、鬼火、月光，**画面中必须能看到光从哪来**
- ✅ 单一主光源 + 微环境补光
- ❌ 全局均匀光（无戏剧性）
- ❌ 多光源混战（失焦）

### 情绪基调

- ✅ **仪式感、亲密、神秘、忧郁**（首选）
- ✅ 战斗感、英雄感（次选，**且要克制**）
- ❌ 动作场面、广告感、夸张

---

## 8. 应用规则（按资产类型）

### 角色生成流程
1. **参考图**：每个主要角色先出 1 张中性姿态半身像
2. **风格锁定**：通过用户评分（≥ 7）后，参考图作为后续锚点
3. **动作序列帧**：每个动作 6-12 帧
   - run: 8 帧
   - attack: 8 帧
   - cast: 12 帧
   - hit / stagger: 4 帧
   - death: 6 帧
   - idle: 4 帧
4. **每 30 帧自检**：与参考图对比，调色板与笔触是否漂

### 场景生成流程
1. 优先做**亲密场景**（一个建筑 + 一个角色 + 一个光源）
2. 大场景拆解为 4-6 个亲密场景的**组合**
3. 俯视用 **3/4 角度**而非纯 top-down
4. 每个生物群系 1 张主场景 + 4-6 个局部特写

### UI / VFX
- 提取主色板三色，**不要引入新颜色**
- 神力效果统一用 Cyan `#4DD8E6`
- 危险/警告：Cyan + 极少橙红
- 卢恩字符**画**出来，不引用字库
- 字体（如果有）：手写感、衬线、北欧风

### 动画序列帧
- 关键帧与中间帧用**同一 prompt 模板**，只改姿态描述
- 例：
  ```
  [完整关键词包] + "a Norse valkyrie in [动作描述],
  same character, same color palette, same painterly style as reference"
  ```
- 锁参考图：把参考图作为 `input_file_paths` 喂给每张生成
- **背景**：所有动画帧必须用纯亮绿 #00FF00 背景（见第 8.5 节），不允许任何其他背景

---

## 8.5 动画帧背景规则（Animation Frame Backgrounds）🔒

> **2026-07-27 新增：源于实际生产需求——所有角色/敌人动画帧必须使用纯亮绿（#00FF00）背景。**

**为什么是亮绿而不是暗色背景**：
- 亮绿 `#00FF00` 与人物调色板（navy `#0A1A2F` / cyan `#4DD8E6` / gold `#C9A567`）**零交集**，不会污染角色边缘
- Unity 可直接走 chroma key shader 或 sprite import 抠图
- 比"暗色背景 + 边缘羽化"干净 100 倍——后者笔触会渗入背景，抠不干净

**Prompt 必带措辞**（任何动画帧都加）：
```
ISOLATED on solid pure bright green chroma key background #00FF00,
no scenery, no environment, no other elements, no background bleed,
character only, designed for game engine sprite compositing
```

**反例（禁止使用）**：
- ❌ `simple dark gradient background`（笔触渗入背景）
- ❌ `dark moody background`（与角色 navy 主体冲突）
- ❌ `atmospheric background`（无明确色键边界）
- ❌ 任何非 `#00FF00` 的复杂背景

**例外**：参考图、英雄图、Steam 营销图 — 这些**保留**第 7 节的暗调氛围，因为它们不进游戏 runtime。

**Unity 端 3 种扣图方案**：

| 方案 | 适用 | 操作 |
|---|---|---|
| **A. Sprite Import** | 简单场景 | Sprite Editor → 手动描 alpha，或写自动脚本批量按颜色 key |
| **B. Chroma Key Shader** | 性能要求高 | 写一个 Shader Graph，把 `dot(color, greenMask) > threshold` 的像素 alpha 设为 0 |
| **C. ImageMagick 批处理** | 大量资产入库前 | `magick input.png -fuzz 25% -transparent "#00FF00" output.png` |

推荐 B 用于运行时（性能最好），C 用于批量预处理。

---

## 9. 验证清单（每张图生成后跑）

```
□ 5 个核心关键词是否都出现？
□ 调色板比例是否接近 60/15/10/15？
□ 是否有可见的画内光源？
□ 镜头是否亲密（不是 wide establishing）？
□ 卢恩/装饰是"画"出来的，不是"贴"上去的？
□ 与 5 张锚点图放一起像同一个游戏？
□ 没有禁用关键词？
□ 动画帧是否是纯亮绿 #00FF00 背景（无则重做）？
```

任何一项不过 → 改 prompt 重做（动画帧无亮绿背景是**返工**级问题）。

---

## 10. 变更流程（Change Protocol）

> **改风格必须走以下流程。** 禁止单独改 prompt、单独调色、单独换参考艺术家。

1. **用户提出改动**（附具体需求 + 例子）
2. **生成对比样图**（4-6 张，覆盖 2-3 个改动方向）
3. **用户打分**（1-10），挑出新高分图
4. **同步更新本文档**：
   - 锚点图（如有替换，旧的移到"历史锚点"）
   - 调色板
   - 必带/禁用关键词
   - 参考艺术家
5. **写变更日志**（第 11 节）

---

## 11. 变更日志

| 日期 | 版本 | 改动 | 责任人 |
|---|---|---|---|
| 2026-07-27 | v1.0 | 初版锁定 | Mavis + 用户 |

---

## 12. 附：完整 Prompt 模板（推荐复制使用）

### 角色模板
```
[核心 5 关键词] + dark fantasy oil painting, heavy visible brushstrokes,
rich painterly texture, dramatic chiaroscuro lighting, deep midnight blue
dominant, cyan magical light source, warm gold accents, intimate cinematic
framing, mythological ritualistic atmosphere, Bastien Lecouffe-Deharme dark
fantasy style,

[角色描述：性别、姿态、装备、表情、动作] +

NOT flat, NOT vector, NOT graphic stylized, NOT anime
```

### 场景模板
```
[核心 5 关键词] + dark fantasy oil painting, heavy visible brushstrokes,
rich painterly texture, dramatic chiaroscuro from [光源] as primary light
source, deep midnight blue dominant, warm gold accents, cool moonlight cyan
shadows, intimate cinematic framing NOT a wide establishing shot,
Bastien Lecouffe-Deharme dark fantasy style,

[场景描述：建筑、人物、天气、时间、关键道具] +

NOT flat, NOT vector, NOT graphic stylized, NOT a wide panorama
```

---

**🔒 本文档从 2026-07-27 起生效，所有 Ravensong 视觉资产必须遵守。**
