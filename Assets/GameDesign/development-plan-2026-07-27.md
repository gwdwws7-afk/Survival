# Ravensong — 完整开发计划

> **Status**: 🟡 DRAFT v1.0
> **日期**: 2026-07-27
> **作者**: Mavis + 用户
> **关联**: `handover-2026-07-27.md` (起点状态) · `game-concept.md` §11.3 (三档规模) · `data-config.md` §C.2 (17 SO schema) · `systems-index.md` §3 (资源流图)
> **变更协议**: 修改任一阶段须更新本文件 + 写变更日志（§11）

---

## 0. TL;DR（30 秒）

- **起点**：设计 100% 锁（19 GDD / 17 SO schema / 5 签名系统 / 4 支柱）— Unity 实现 0%
- **目标**：3 档规模（Prototype 4-6 月 / EA 10-12 月 / Full 18-24 月）
- **P0（必须先做）**：拍板 3 个不一致（Unity 版本 / git / mcp 通道）→ 装 8 个工具链包 → 写 30 个 .cs
- **MVP 红线 7 项**（game-concept §12）— Prototype 阶段必须全部跑通
- **P0 风险**：美术瓶颈（高 / 致命）— 已用 AI 2D 序列帧 + 女武神 10 帧参考图缓解
- **下一步**：拍板 §11 决策点 → 进 P0 第一步（基础设施）

---

## 1. 制定依据

| 维度 | 已锁内容 | 来源 |
|---|---|---|
| 核心机制 | 4 支柱 + 5 签名系统 + 5 Anti-Pillars | `game-concept.md` §3, §6, §7 |
| 数据 schema | 17 SO + 254 GameConfigSO 字段（v2.5） | `data-config.md` §C.2 |
| 系统间经济 | 4 主反馈 + 4 负反馈 + 5 瓶颈 | `systems-index.md` §3 |
| 美术宪法 | 5 锚点图 + 配色 60/15/10/15 + 必带 5 关键词 | `style-bible.md` v1.0 |
| 资产 | 女武神 10 帧已出（4 idle + 6 run + 1 reference） | `character-valkyrie/` |
| 框架审查 | 8 框架 144 检查点全部优秀 | `01-23_xxx.md` |
| 缺口修复 | P0×1 / P1×3 / P2×4 全部完成 | `handover-2026-07-27.md` §5 |

**判断**：设计层是可信输入，可以直接据此制定实现路径。

---

## 2. 关键决策（必须先拍板，见 §11）

| # | 决策 | 候选 | 推荐 | 影响 |
|---|---|---|---|---|
| D1 | **Unity 版本** | Unity 2022.3 LTS（当前） / Unity 6（文档说） | **升 Unity 6 LTS** | 影响 8 个工具链包的选择、URP 配置、性能预算 |
| D2 | **Git 仓库** | 不 init / init 但不 commit / init + 首次 commit | **init + 首次 commit** | 保护已有工作 + 后续 .cs 改动可追踪 |
| D3 | **MCP 通道** | 保持当前（v9.7.2-beta.9，回滚过）/ 升 v10.1.0 | **保持 v9.7.2-beta.9** | MCP for Unity v10.1.0 验证不够，回滚后稳定 |
| D4 | **MVP 群系** | 白桦林 / 白骨原 / 深渊沼（3 选 1） | **白桦林** | 与女武神视觉最搭、最易做精 |
| D5 | **第一英灵** | 6 职业（铁匠/猎人/吟游诗人/农夫/战士/治疗）× 性别 | **铁匠 Eirik**（参考 handover）| 测试 Death & Send-off 签名系统最佳（职业高、代入感强）|
| D6 | **第一 boss** | 4 设计 boss 之一 | **白桦林古龙 Draugrlord** | 与白桦林群系配、与女武神背景呼应（亡灵主题）|
| D7 | **第一誓言** | 5 誓言之一（锻冶/炉火/荒野/亡者/苍穹） | **锻冶之誓** | 编织 Tier 升级直接可见、节奏短 |
| D8 | **音频方案** | FMOD（计划）/ Unity Audio（简单） | **FMOD** | 动态 BGM 必走 FMOD，否则放弃"白天压抑/夜晚史诗"效果 |

> D1-D3 必须今天拍板；D4-D7 阶段 1 末拍板；D8 阶段 0 拍板。

---

## 3. 阶段 0：基础设施（Week 0，1-2 天）

### 目标
让 Unity 工程具备后续开发的最小条件：版本对齐 + 工具链齐全 + 数据隔离 + 版本控制。

### 任务清单

#### 3.1 版本对齐（D1）
- [ ] **D1 拍板**（Unity 6 vs 2022.3）
- [ ] 如果升 6：备份 `ProjectSettings/ProjectVersion.txt` + 重新打开 → Unity Hub 下载 Unity 6 LTS
- [ ] 如果留 2022.3：更新 `game-concept.md` §10 技术栈表（标记 "实际 = 2022.3 LTS"）

#### 3.2 Git init（D2）
- [ ] `git init` 在 `E:/LevelDesign/Survival/`
- [ ] `.gitignore`：Library/ Temp/ Logs/ UserSettings/obj/ *.csproj *.sln
- [ ] 首次 commit：`chore: initial commit (设计文档 19 GDD + 美术锚点 5 张 + 女武神 10 帧)`

#### 3.3 工具链包补全
- [ ] Cinemachine 2D
- [ ] Input System
- [ ] FMOD Unity Integration
- [ ] Addressables
- [ ] NodeCanvas（行为树）
- [ ] A* Pathfinding Project
- [ ] Aseprite Importer（可选）
- [ ] PSD Importer（可选）
- [ ] 写一个 `manifest.json` patch 脚本，避免手工加错格式

#### 3.4 Unity Editor 设置
- [ ] Project Settings → Player → 2D 模板
- [ ] Quality Settings → 60 FPS 目标（v-sync off + targetFrameRate=60）
- [ ] Color Space → Linear
- [ ] Graphics → URP 2D Renderer（如果升 6）
- [ ] Test Framework → EditMode + PlayMode assembly

#### 3.5 目录骨架（仅创建空目录 + `.gitkeep`）
```
Assets/
├── Scripts/        # 阶段 1 填
│   ├── Data/
│   ├── Core/
│   ├── DayNight/
│   ├── FateThread/
│   ├── Einherjar/
│   ├── Oath/
│   ├── DeathSendoff/
│   └── Settlement/
├── Data/           # 阶段 1 末尾填（SO .asset 落地）
│   ├── Recipes/
│   ├── Items/
│   ├── Einherjars/
│   ├── Biomes/
│   ├── Oaths/
│   ├── Bosses/
│   ├── WorldEvents/
│   ├── Dialogues/
│   ├── Sendoffs/
│   ├── Expeditions/
│   ├── Quests/
│   ├── UIStyles/
│   ├── VFX/
│   ├── Audio/
│   └── GameConfig/
├── Art/            # 阶段 2 填
├── Audio/          # 阶段 2 填
├── Prefabs/        # 阶段 2 填
├── Scenes/         # 阶段 2 填
└── Resources/      # 阶段 1 末填（DataRegistry 入口）
```

### 验收
- [ ] Unity Editor 启动不报错
- [ ] Console 无 warning（package 兼容）
- [ ] git status 显示首次 commit 成功
- [ ] 8 个目录骨架存在

### 退出条件
✅ 全部 ✅ → 进阶段 1

---

## 4. 阶段 1：核心代码骨架（Week 1-2）

### 目标
写完 30 个 .cs（~5000 行）— Unity 编译通过，DataRegistry 能 load，能跑空核心循环。

### 4.1 17 个 SO 脚本（基于 `data-config.md` §C.2 已有 schema）

| # | 脚本 | 行数估算 | 依赖 |
|---|---|---|---|
| 1 | `IDataValidatable.cs`（接口） | 20 | — |
| 2 | `GameConfigSO.cs`（~254 字段，核心） | 800 | IDataValidatable |
| 3 | `RecipeSO.cs` | 60 | IDataValidatable |
| 4 | `ItemSO.cs` + `ToolSO.cs`（v1.3 扩展） | 150 | IDataValidatable |
| 5 | `EinherjarSO.cs` | 100 | IDataValidatable |
| 6 | `BiomeSO.cs` | 80 | IDataValidatable |
| 7 | `OathSO.cs` | 70 | IDataValidatable |
| 8 | `BossSO.cs` + `BossDetailSO.cs` | 120 | IDataValidatable |
| 9 | `WorldEventSO.cs` | 60 | IDataValidatable |
| 10 | `DialogueSO.cs` | 50 | IDataValidatable |
| 11 | `SendoffSO.cs` | 60 | IDataValidatable |
| 12 | `UIStyleSO.cs` | 70 | IDataValidatable |
| 13 | `SettlementSO.cs` | 80 | IDataValidatable |
| 14 | `ExpeditionSO.cs` | 70 | IDataValidatable |
| 15 | `QuestSO.cs` | 70 | IDataValidatable |
| 16 | `VFXPresetSO.cs` | 50 | IDataValidatable |
| 17 | `AudioPresetSO.cs` | 50 | IDataValidatable |
| 18 | `ItemStack.cs`（容器） | 30 | — |

**SO 合计 ~1990 行**

### 4.2 Core（3 个）

| # | 脚本 | 行数估算 | 职责 |
|---|---|---|---|
| 19 | `DataRegistry.cs` | 400 | SO 索引、按 ID 解析、热重载、版本检查 |
| 20 | `GameManager.cs` | 200 | 主控 MonoBehaviour、单例、scene 切换、save/load hook |
| 21 | `TimeManager.cs` | 150 | 日夜时钟、timeScale、debounce 触发器 |

**Core 合计 ~750 行**

### 4.3 5 个签名系统 Manager（来自 `systems-index.md`）

| # | 脚本 | 行数估算 | 职责 |
|---|---|---|---|
| 22 | `DayNightCycle.cs` | 300 | Waxing Moon 月相、6 段（黎明/日/暮/夜/午夜/深宵）、debuff 计算 |
| 23 | `WeavingSystem.cs` | 400 | 编织配方匹配、成功率、god-ember 消耗、Tier 1-5 路径 |
| 24 | `EinherjarManager.cs` | 500 | 招募、衰老、生病、死亡、Work productivity 调参 |
| 25 | `OathManager.cs` | 300 | 5 誓言 + milestone tracking + 苍穹解锁 |
| 26 | `SendoffManager.cs` | 400 | 强留 vs 送别分支、衰悼期、Valhalla buff |

**5 Manager 合计 ~1900 行**

### 4.4 辅助系统（1 个）

| # | 脚本 | 行数估算 | 职责 |
|---|---|---|---|
| 27 | `SettlementManager.cs` | 400 | 长屋容量、英灵岗位、士气、+8 产能 / 英灵 |

### 4.5 1 个 GameConfig .asset 占位

- [ ] 阶段 1 末尾：1 个 `GameConfig.asset`（填最低必需的 50 字段，让 DataRegistry 启动不报缺字段）

### 验收
- [ ] Unity 编译 0 error 0 warning
- [ ] `DataRegistry.GetItem("any_test_id")` 在 EditMode test 中返回 SO
- [ ] PlayMode 启动 `DayNightCycle` 跑 1 个虚拟日（10 分钟游戏时间）
- [ ] `WeavingSystem.TryWeave(recipe, inputs)` 单元测试通过

### 退出条件
✅ 30 .cs 编译通过 + 1 个空 `GameConfig.asset` 能 load → 进阶段 2

### 风险
- 🟡 254 GameConfigSO 字段太大 — **缓解**：v2.5 schema 已有，可分 3 批写（核心 50 字段 → 战斗 80 → 其他 124）
- 🟡 行为树 / 寻路包 API 复杂 — **缓解**：本阶段 Manager 不直接调，先打桩；阶段 2 接入

---

## 5. 阶段 2：MVP 7 项（Week 3-8，6 周）— Prototype 规模

### 目标
跑通 `game-concept.md` §12 的 7 项 MVP 红线清单 — 可玩的内部 demo。

### 5.1 Milestone 1：昼夜倒置（W3-W4，2 周）
**对应 4 支柱 #2 Waxing Moon**

- [ ] 6 段日相 UI（半透叠层 + 文字）
- [ ] 白天 debuff 实现：视野 -30% / 移速 -20% / 编织 -30%
- [ ] 夜晚 buff 实现：移速 +40% / 编织 -50% 时间
- [ ] 1 个 test scene：玩家放个 dummy，能看到 debuff/buff 切换
- [ ] **D4 拍板**：选 1 个群系（推荐白桦林）— 选完开始做地形

**验收**：玩家从白天走到夜晚，状态数值实时变化，UI 半透层变

### 5.2 Milestone 2：命运丝线（W4-W5，2 周）
**对应 4 支柱 #4 Woven Power**

- [ ] 编织 UI（左 A + 右 B → 拖丝线 → 符文浮现 → 出生 C）
- [ ] 25-30 个 EA 配方（`RecipeSO` 首批）— 全开放，无科技树
- [ ] god-ember 消耗 + 上限 999
- [ ] 视觉：丝线震颤 / 0.1s 慢动作 / 光晕扩散
- [ ] **D7 拍板**：选 1 誓言（推荐锻冶之誓）— 选完开始做 milestone tracking

**验收**：白桦林收集 2 资源 → 编织 UI → 出现 1 个 Tier 1 物品

### 5.3 Milestone 3：1 个可培养英灵（W5-W6，2 周）
**对应 4 支柱 #3 Living Hearth**

- [ ] **D5 拍板**：选 1 英灵（推荐铁匠 Eirik）
- [ ] EinherjarSO 数据 + portrait + 10 帧动画
- [ ] 招募流程（从战场找回 → 长屋入住）
- [ ] 衰老 / 生病 / 死亡机制
- [ ] **送别 UI**：选择送 Valhalla（buff 获得）或强留（3-5 天腐化警告）
- [ ] 衰悼期 -20% 士气 / 24h

**验收**：Eirik 招募 → 工作中 → 死亡 → 玩家选送别 → 衰悼期触发

### 5.4 Milestone 4：1 条完整誓言（W6-W7，2 周）
**对应终局汇聚 #1**

- [ ] 4-5 个 milestone（按 D7 选的那条誓言）
- [ ] OathSO 数据 + UI milestone tracker
- [ ] 完成后解锁对应内容（苍穹之誓暂留 Full 阶段）

**验收**：1 条誓言 4 个 milestone 全完成，UI 出现"已达成"

### 5.5 Milestone 5：1 个可玩 boss（W7-W8，2 周）
**对应战斗签名系统**

- [ ] **D6 拍板**：选 1 boss（推荐白桦林古龙 Draugrlord）
- [ ] BossSO + BossDetailSO 数据
- [ ] Boss 房 prefab + AI（用 NodeCanvas BT）
- [ ] 战斗系统：编织武器攻击 + 武器耐久 -5/次
- [ ] 击败后 24h 冷却（避免刷）
- [ ] 战利品：1 个 Tier 4 装备

**验收**：进入 boss 房 → 战斗 → 死/胜 → 战利品入背包

### 5.6 Milestone 6：1 个生物群系做精（W8，1 周）
**对应美术瓶颈缓解**

- [ ] **D4 已选**：白桦林
- [ ] 1 张主场景图 + 4-6 个局部特写
- [ ] 1 套分层视差背景（用 Aseprite 拆分）
- [ ] 1 套 6 群系资源节点 prefab
- [ ] Cinemachine 2D camera 配置（战斗 zoom / 跟随）

**验收**：玩家能"自由跑"白桦林，看到分层视差 + 6 资源节点

### 5.7 Milestone 7：结束/再开循环（W8，1 周）
**对应 session 闭环**

- [ ] 黎明收束（夜 → 晨的过渡）
- [ ] Save System（JSON save，仅玩家数据）
- [ ] Load Game + 死亡 + 重启循环
- [ ] 0.5s 不可跳的淡入（奥丁审判独白前 10 秒）

**验收**：玩到 30 分钟 → 黎明回聚落 → 主动 save → 退出 → load 继续

### 5.8 阶段 2 整体验收
- [ ] 7 项 MVP 全跑通
- [ ] 1 完整 30-60 分钟 session（黎明 → 战斗 → 编织 → 招募 → 死亡 → 送别 → 黎明）
- [ ] 内部 demo 可玩

### 退出条件
✅ 7 项 MVP 全通过 + 内部 demo 可玩 → 进阶段 3（EA 准备）

### 总时间估算
- **乐观**：4-6 个月（game-concept §11.3）— solo 全职密集
- **现实**：6-8 个月（含美术 + 平衡 + debug）

---

## 6. 阶段 3：EA 早期访问（Week 9-24，约 4-5 月）

### 目标
扩量到 EA 规模：2 誓言 / 2 群系 / 8 英灵 / 2 boss，Steam EA 上线，边卖边补。

### 任务清单
- [ ] 第 2 誓言（亡者之誓或荒野之誓）
- [ ] 第 2 群系（白骨原）
- [ ] 8 个英灵（6 职业 + 2 重复性别）
- [ ] 4 boss → 2 boss 完整（剩 2 留 Full）
- [ ] 30+ audio clip（白天压抑 / 夜晚史诗 BGM + 战斗音）
- [ ] 25+ VFX 预设（编织丝线 / 月相 / 神力爆发）
- [ ] 25 milestone 完整分布
- [ ] 6 群系难度平衡（用 `systems-index.md` §3.6 风险表）
- [ ] Steam EA 页面 + Next Fest 申请
- [ ] 易难度 / 平衡昼夜选项（"昼夜倒置被讨厌"风险缓解 — `game-concept.md` §13）

### 退出条件
✅ Steam EA 上线 + 100+ 评测 + 0.85+ 好评率 → 进阶段 4

---

## 7. 阶段 4：Full Release（Week 25+，约 4-6 月）

### 目标
完成全愿景：5 誓言 / 4-5 群系 / 12+ 英灵 / 4-5 boss / 真结局。

### 任务清单
- [ ] 3-5 群系全部做精
- [ ] 5 誓言全完成 + 苍穹之誓 + 奥丁审判
- [ ] 2 真结局（回阿斯加德 / 留中庭）
- [ ] 4-5 boss 完整
- [ ] 60+ audio clip + 30+ VFX
- [ ] 800-1500 美术资产
- [ ] NG+ 循环（`systems-index.md` §3.6 终局风险缓解）
- [ ] Steam 1.0 发布
- [ ] 媒体 Kit（hero 图 / 4 主题图 / Gif 演示 / 30s trailer）

### 退出条件
✅ Steam 1.0 发布 + 1.0 评测 + 长尾销售曲线建立 → 项目交付

---

## 8. 跨阶段关注

### 8.1 美术（持续）
- 锚定 5 张已锁，不动
- 继续按 `style-bible.md` 做：
  - 4 boss 参考 + 动画（阶段 2 M5 + 阶段 3/4）
  - 6 群系主场景 + 4-6 局部特写（阶段 2 M6 + 阶段 3/4）
  - 8+ 英灵肖像（阶段 3）
  - 6 隐藏配方视觉（阶段 3）
- **每 30 帧自检**：与 5 锚点图对比
- **背景铁律**：所有动画帧纯亮绿 #00FF00 背景（chroma key）

### 8.2 音频（阶段 3 集中做）
- D8 拍板 FMOD
- 白天压抑 BGM（北欧民谣 + 低频 + 弦乐 solo）
- 夜晚史诗 BGM（合唱 + 全弦乐 + 鼓）
- 编织音、清亮"叮"+ 符文回音
- 60+ clip 总量

### 8.3 测试（阶段 2 起持续）
- EditMode：DataRegistry / WeavingSystem 单元测试
- PlayMode：DayNight 周期、boss 战斗、编织成功率
- 平衡测试：用 `systems-index.md` §3.6 风险表做 §G 旋钮调参

### 8.4 平衡（用 Machinations 资源流图）
- 阶段 1：DataRegistry + 资源池计算 unit test
- 阶段 2：M1-M7 内部 playtest（你自己）
- 阶段 3：EA 玩家数据回流 + 调参
- 阶段 4：NG+ 难度曲线调参

---

## 9. 风险与退路

| 风险 | 概率 | 严重度 | 缓解 | 退路 |
|---|---|---|---|---|
| **美术瓶颈** | 极高 | 致命 | AI 2D 序列帧 + 女武神 10 帧参考 | 外包（先 1 个完整角色做风格锚再做外包）|
| **英灵情感设计失效** | 高 | 高 | 阶段 2 M3 跑测 3 个月 | MVP 砍"送别 UI 情感深度"，保"机制" |
| **昼夜倒置被讨厌** | 中 | 致命 | EA A/B 测试 | 提供"易难度"或"平衡昼夜"选项（`game-concept.md` §13）|
| **行为树 / 寻路 API 复杂** | 中 | 中 | 阶段 1 打桩、阶段 2 接入 | 退化到简单 FSM |
| **FMOD 学习成本** | 中 | 中 | D8 拍板就开干 | 退化到 Unity Audio，砍动态 BGM |
| **Unity 6 升版踩坑** | 中 | 高 | 阶段 0 1 天升级 | 退回 2022.3 LTS（更新文档）|
| **CoplayDev MCP 升级风险** | 低 | 中 | D3 保持 v9.7.2-beta.9 | 后续 v10.x 验证充分再升 |
| **solo burnout** | 中 | 致命 | 4-5 小时 session 节奏 + 干净 stopping point | 砍范围到 MVP 即可上线（不达 Full）|

---

## 10. 总时间线

```
Week 0         W1-2         W3-8                  W9-24              W25+
│              │            │                     │                  │
├─ 阶段 0      ├─ 阶段 1    ├─ 阶段 2 (6 周)      ├─ 阶段 3 (16 周)  ├─ 阶段 4
│  基础设施    │  30 .cs    │  MVP 7 项           │  EA 规模         │  Full 规模
│  1-2 天      │  ~5000 行  │  30-60 min demo     │  Steam EA        │  Steam 1.0
                              │                     │                  │
                              ↓                     ↓                  ↓
                          Prototype 规模        EA 规模            Full 规模
                          4-6 月目标            10-12 月目标        18-24 月目标
```

**累计时间估算**（solo 全职）：
- **乐观**：4-6 月到 Prototype / 10-12 月到 EA / 18-24 月到 Full
- **现实**：6-8 月到 Prototype / 14-18 月到 EA / 24-30 月到 Full

---

## 11. 决策点（按时间排序）

### 今天必须拍板
- [ ] **D1**: Unity 6 vs 2022.3 LTS
- [ ] **D2**: Git init
- [ ] **D3**: MCP 保持 v9.7.2-beta.9

### 阶段 0 拍板
- [ ] **D8**: 音频方案（FMOD vs Unity Audio）

### 阶段 1 末拍板
- [ ] **D4**: 第一群系（白桦林 / 白骨原 / 深渊沼）
- [ ] **D5**: 第一英灵（铁匠 Eirik / 其他）
- [ ] **D6**: 第一 boss（白桦林古龙 Draugrlord / 其他）
- [ ] **D7**: 第一誓言（锻冶之誓 / 其他）

### 阶段 2 末拍板
- [ ] Prototype demo 反馈（自己 playtest）
- [ ] 是否进 EA 模式

---

## 12. 立即可做的第一步（在你拍板 D1-D3 之后）

如果你说"开干"，我下一步会：

1. **D1 拍板**（看你选哪个）
   - 如果升 Unity 6 → 写 manifest patch + 备份 ProjectVersion.txt
   - 如果留 2022.3 → 更新 game-concept.md §10 标记 "实际 = 2022.3"
2. **D2 git init** + `.gitignore` + 首次 commit（保护当前 19 GDD + 5 锚点 + 10 帧）
3. **D3 不动**（MCP 保持 9.7.2）
4. **装 8 个工具链包**（Cinemachine/Input/FMOD/Addressables/NodeCanvas/A*/Aseprite/PSD）
5. **创建 §3.5 目录骨架**（30 个空目录 + `.gitkeep`）
6. **写 30 .cs**（按 §4 顺序，分 5-6 批推 commit）

**预计**：阶段 0 = 1-2 天（如果你拍板快）；阶段 1 = 5-7 天（按 batch 推）

---

## 13. 变更日志

| 日期 | 版本 | 改动 | 责任人 |
|---|---|---|---|
| 2026-07-27 | v1.0 | 初版：基于 handover §7 + game-concept §11.3 + 17 SO schema | Mavis + 用户 |
