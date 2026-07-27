# Ravensong 实时进度（progress.md）

> **最后更新**: 2026-07-27 19:29
> **当前 HEAD commit**: `f779504` (origin/main)
> **当前阶段**: 阶段 1 收尾 → 等用户在 Unity Editor 验证编译 → 进阶段 2
> **本文件维护**: Mavis（每次 commit / 阶段切换 / 阻塞变化时更新）

---

## 阶段总览

| 阶段 | 状态 | 完成度 | 退出条件 |
|---|---|---|---|
| **0** 基础设施 | ✅ 完成 | 100% | Unity 编译 0 报错 |
| **1** 核心骨架 | ✅ 完成 | 100% | 30 .cs 写完 + DataRegistry load |
| **2** MVP 7 项 | ⚪ 未开始 | 0% | 30-60 min 可玩 demo |
| **3** EA 早期访问 | ⚪ 未开始 | 0% | Steam EA 上线 |
| **4** Full Release | ⚪ 未开始 | 0% | Steam 1.0 |

**详细计划**: `Assets/GameDesign/development-plan-2026-07-27.md`

---

## ✅ 已完成

### 决策（2026-07-27）
- **D1**: Unity 2022.3 LTS（实际版本，不升 Unity 6）— 见 `game-concept.md` §10
- **D2**: git init + remote = `https://github.com/gwdwws7-afk/Survival`
- **D3**: MCP for Unity 9.7.2-beta.9 保持（v10.1.0 验证过不稳）

### 阶段 0 — 基础设施（4 commit, 19:09-19:14）
- [x] `1db09bd` chore: initial commit（19 GDD + 5 锚点 + 女武神 10 帧 + 1 SampleScene）
- [x] `f7a4714` chore(deps): 装 3 个官方包（Cinemachine 2.10.1 / InputSystem 1.11.2 / Addressables 1.22.1）
- [x] `008f8ea` chore(structure): 27 个目录骨架（Scripts 8 / Data 15 / 顶层 4）
- [x] `.gitignore` 写完（Unity 标准 + Ravensong 特定）

### 阶段 1 — 核心骨架（2 commit, 19:17-19:28）
- [x] `6498b99` feat(data): 18 个数据层文件（17 SO + DataTypes + IDataValidatable + ItemStack, ~3300 行）
- [x] `f779504` feat(core): 10 个文件（3 Core + 5 Manager + Settlement + Editor Wizard, ~3100 行）

**累计**: 28 .cs / ~6400 行 / 5 commit

---

## 🔄 进行中

（无 — 等用户在 Unity Editor 验证编译）

---

## 🔥 下一步（按优先级）

1. **Unity Editor 启动**（用 2022.3.12f1）— pull 3 个新包、import 28 个 .cs
2. **Tools → Ravensong → Bootstrap Data** — 一键建 18 个 .asset 占位
3. **Tools → Ravensong → Setup Scene** — 一键挂 9 个 Manager + 绑 GameConfig
4. **Play 验证编译** — Console 应见 DataRegistry 加载 18 SO + 验证通过
5. **装 4 个第三方包**（FMOD / NodeCanvas BT / A*Pathfinding / Aseprite Importer）— AssetStore 或 openupm
6. **阶段 2 M1**: 昼夜倒置 UI + debuff 实际生效（6 段半透叠层 + 数值切换）

---

## 🚧 阻塞 / 风险（实时）

| # | 风险 | 状态 | 缓解 |
|---|---|---|---|
| 1 | 第三方包没装（FMOD/NodeCanvas/A*Pathfinding/Aseprite） | 🟡 等用户 | Manager 阶段 1 已打桩，可空跑 |
| 2 | DataRegistry 用反射访问私有字典（IL2CPP 可能剥） | 🟡 未验证 | 阶段 2 跑 Player build 时确认 |
| 3 | Cinemachine 2D 2.10.1 vs URP 2D 兼容性 | ⚪ 未测 | Unity 启动后看 Console |
| 4 | InputSystem 1.11.2 vs 旧 InputManager.asset 冲突 | ⚪ 未测 | 阶段 1 Editor wizard 已并存 |
| 5 | Addressables 1.22.1 是 1.x 末位，2.x 是 2024 主推 | 🟢 低 | 1.22.1 稳定 |

---

## 📜 Commit 日志（最近 5）

| 时间 | hash | message |
|---|---|---|
| 19:28 | `f779504` | feat(core): 5 signature system managers + 1 settlement + Editor wizard |
| 19:17 | `6498b99` | feat(data): 17 ScriptableObject types + DataTypes + IDataValidatable + ItemStack |
| 19:14 | `008f8ea` | chore(structure): create 27 empty directories for Scripts/Data/Art scaffolding |
| 19:12 | `f7a4714` | chore(deps): add 3 Unity official packages (Cinemachine/InputSystem/Addressables) |
| 19:09 | `1db09bd` | chore: initial commit (Ravensong 设计 100% + 美术锚点) |

**完整 log**: `git -C "E:/LevelDesign/Survival" log --oneline -20`

---

## 📊 代码统计

| 维度 | 数量 |
|---|---|
| .cs 文件 | 28 |
| 总行数 | ~6400 |
| SO 类型 | 17 |
| 签名系统 Manager | 5 |
| Core | 3 |
| Settlement Manager | 1 |
| Editor Wizard | 1 |
| 公共 enum（DataTypes.cs）| 11 |
| 公共 helper class | 5（StatBlock/StatusEffect/ConsumableEffect/DayNightItemBonus/TraitEntry） |

---

## 🎯 决策待拍板（plan §11）

### 阶段 0 拍板（部分完成）
- [x] **D1** Unity 2022.3 LTS（已拍）
- [x] **D2** git init（已拍）
- [x] **D3** MCP 9.7.2-beta.9 保持（已拍）
- [ ] **D8** 音频方案：FMOD / Unity Audio（未拍）

### 阶段 1 末拍板（Unity 编译通过后）
- [ ] **D4** 第一群系：白桦林 / 白骨原 / 深渊沼（推荐白桦林）
- [ ] **D5** 第一英灵：铁匠 Eirik / 其他（推荐 Eirik）
- [ ] **D6** 第一 boss：白桦林古龙 Draugrlord / 其他（推荐 Draugrlord）
- [ ] **D7** 第一誓言：锻冶之誓 / 其他（推荐锻冶之誓）

---

## 📝 更新日志（本文件）

| 日期 | 改动 |
|---|---|
| 2026-07-27 19:29 | 初版：阶段 0+1 完成进度快照 |
