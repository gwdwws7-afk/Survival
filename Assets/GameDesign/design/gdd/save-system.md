# Save System — System GDD

> **Status**: 🔒 LOCKED v1.0
> **Author**: Mavis + 用户
> **Last Updated**: 2026-07-27
> **Implements Pillar**: **Foundation**（支持全部 4 根支柱）

---

## A. Overview

**Save System 是 Ravensong 的"记忆中枢"——让玩家的每个选择、每次编织、每位英灵都被记住。** 所有运行时状态（玩家位置、库存、英灵、誓言、群系进度、时间）持久化到本地存档，玩家关闭游戏后下次进入可继续。

存档分 3 类：**5 个手动槽位 + 1 个自动存档槽 + 1 个快速存档槽**。存档格式为 JSON（可读 + 易调试），v1.0 不加密（playtest 友好），release 加密。

**关键约束**：所有静态数据（`ItemSO` / `RecipeSO` / `BiomeSO` 等）**不**进存档——这些是设计师编辑的资产，玩家存档只记录**运行时状态**（哪几件物品在包里、哪位英灵在工作、现在是第几天 17:30）。

---

## B. Player Fantasy / User Experience

### 关键体验时刻
- **第一次**手动存档：UI 显示缩略图（角色 + 当前位置 + 时间），玩家感到"这一刻被记住了"
- **游戏崩溃**后重启：自动存档恢复，玩家损失 < 30 分钟
- **死亡**时：自动存档"紧急模式"被加载，玩家从上一次安全点继续
- **第 1 周目完成**：存档可标记"完成"，未来"新周目+"可继承部分内容（v1.1 决策）
- **跨设备**（v1.1 不支持，但设计预留）：存档格式标准化，未来可云同步

### 关键体验目标
- "**游戏永远不会真的失去任何东西**"—— 这是 Save System 的最高准则
- 任何"存档丢失"或"存档损坏"都是 Save 系统的 bug

---

## C. Detailed Design

### C.1 核心规则（Core Rules）

#### 规则 1：存档分 3 类
- **5 个手动槽位**（SaveSlot 1-5）：玩家主动控制
- **1 个自动存档槽**（AutoSave）：系统自动覆盖
- **1 个快速存档槽**（QuickSave）：F5 / 特定键，玩家主动触发
- **总存档数：5 + 1 + 1 = 7 个文件**

#### 规则 2：静态数据不入存档
- `ItemSO` / `RecipeSO` / `BiomeSO` / `BossSO` 等**不**入存档
- 存档只记录**运行时状态**（id 引用 + 当前值）
- 例：存档说"背包有 1 个 `item_ash_branch`"（不是"背包有 1 个 Y 像素的 sprite"）

#### 规则 3：所有存档都有 schema 版本
- 每个存档文件包含 `saveVersion` 字段
- 加载时检查版本，不匹配则跑迁移
- 迁移失败 → 显示错误，提示玩家

#### 规则 4：存档可写时机
- **可写**：Gameplay / UI / Menu / Pause / Dawn（5:00 之后） / Death
- **不可写**：Boss 战 / 对话 / Cutscene（避免破坏原子性）
- 进入不可写场景时，**先**完成当前操作 → 玩家主动离开 → 才允许存档

#### 规则 5：存档原子写入
- 写入过程 = "先写临时文件 → rename 覆盖原文件"
- 防止写入过程中崩溃导致原文件损坏

#### 规则 6：存档可恢复
- 每个手动存档保留**最近 3 次写入**作为备份（`save_X.bak.1`, `.bak.2`, `.bak.3`）
- 自动存档保留**最近 3 次**备份
- 快速存档保留**最近 1 次**备份

#### 规则 7：存档大小限制
- 单个存档 ≤ 5 MB（足够 8 英灵 + 完整库存 + 群系数据）
- 超过 5 MB → 警告玩家，并强制精简数据

### C.2 Save Slot Structure

```
%APPDATA%/Ravensong/saves/   (Windows)
~/.local/share/Ravensong/saves/  (Linux/Mac)
├── slot_1.save              # 手动槽位 1
├── slot_1.bak.1            # 备份
├── slot_1.bak.2
├── slot_1.bak.3
├── slot_2.save
├── ...
├── slot_5.save
├── auto.save               # 自动存档（覆盖）
├── auto.bak.1
├── auto.bak.2
├── auto.bak.3
├── quick.save              # 快速存档（覆盖）
├── quick.bak.1
└── meta.json               # 所有槽位的元信息（用于 UI 显示）
```

**meta.json** 包含每个槽位的：
- 缩略图（PNG 256x144，~50KB）
- 玩家名（默认 "Ravensong"）
- 存档时间戳
- 游戏内日期 + 时间
- 当前生物群系
- 已玩时长
- 完成度（%）

### C.3 Save Data Schema

```json
{
  "saveVersion": "1.0",
  "saveTimestamp": "2026-07-27T10:30:00Z",
  "playtimeSeconds": 16200,
  
  "metadata": {
    "characterName": "Ravensong",
    "currentBiomeId": "biome_deep_forest",
    "gameDay": 12,
    "currentTimeOfDay": 17.5,
    "einherjarCount": 4,
    "oathMilestonesCompleted": 6,
    "isCompleted": false
  },
  
  "player": {
    "position": {"x": 12.3, "y": 5.6},
    "health": 80,
    "godEmber": 50,
    "stats": {
      "damage": 10,
      "defense": 5,
      "speed": 1.6
    }
  },
  
  "inventory": {
    "stacks": [
      {"itemId": "item_ash_branch", "quantity": 12},
      {"itemId": "item_iron_ore", "quantity": 3},
      ...
    ],
    "equipment": {
      "mainHand": "item_silver_spear",
      "offHand": null,
      "armor": "item_bronze_chest",
      "accessory": "item_moonlit_charm"
    },
    "quickbar": [
      "item_minor_healing_potion",
      null,
      null,
      null
    ]
  },
  
  "einherjars": [
    {
      "einherjarId": "einherjar_eirik_hunter",
      "name": "Eirik",
      "profession": "Hunter",
      "currentHealth": 100,
      "maxHealth": 100,
      "daysInSettlement": 5,
      "daysToDeath": 7,
      "isDead": false,
      "isValhallaBlessed": false,
      "isWight": false,
      "currentWork": "patrol",
      "relationshipState": "friendly"
    },
    ...
  ],
  
  "oaths": {
    "oath_smithing": {
      "milestonesCompleted": [0, 1, 2],  // 索引
      "isComplete": false
    },
    "oath_hearth": {
      "milestonesCompleted": [0, 1],
      "isComplete": false
    }
  },
  
  "world": {
    "visitedBiomes": ["biome_deep_forest", "biome_frozen_tundra"],
    "depletedNodes": [
      {"biomeId": "biome_deep_forest", "nodeId": "tree_001", "depletedTime": 280.5}
    ],
    "regrowableNodes": [
      {"biomeId": "biome_deep_forest", "nodeId": "plant_005", "regrowAt": 304.5}
    ],
    "activeEvents": []
  },
  
  "time": {
    "totalGameHours": 280.5,
    "currentMoonPhase": 3,
    "moonPhaseCycle": 8
  },
  
  "settings": {
    "audioVolume": 0.8,
    "musicVolume": 0.6,
    "showEnemyAlertRange": true,
    "inputRebindings": {}  // v1.1 启用
  }
}
```

### C.4 Save Triggers

#### 手动存档
- 玩家在菜单中选择"Save Game" → 选槽位 → 确认
- 触发：写入指定 `slot_X.save`

#### 自动存档（AutoSave）
- 触发时机：
  - **每日 06:00（Dawn 警告之后）** —— "新的一天开始时存档"
  - **Boss 战胜利** —— 重要成就
  - **Oath 里程碑完成** —— 重要成就
  - **每 30 真实分钟** —— 兜底
  - **死亡前紧急存档** —— "死亡前的最后快照"
- 触发：覆盖 `auto.save`

#### 快速存档（QuickSave）
- 玩家按 F5 / 手柄 LB+Start 组合
- 触发：覆盖 `quick.save`
- 只能有 1 个（不累计）

### C.5 Save Format & Encryption

#### MVP（v1.0）
- **格式**：JSON（明文），UTF-8 编码
- **不加密**（playtest 友好，可手动编辑调试）
- 路径：标准 Unity `Application.persistentDataPath`

#### Release
- **格式**：JSON → **AES-256 加密**
- **密钥**：从 player 平台 ID 生成（Steam ID 或设备 ID）
- **不**回写（玩家不能从客户端"编辑"存档）

**MVP → Release 切换**：
- 存档文件加 `encrypted: bool` 字段
- 加载时自动判断明文 vs 加密
- 加密失败 → 报错（不静默回退到明文）

### C.6 Save Versioning

```json
{
  "saveVersion": "1.0"
}
```

**迁移规则**：
- `Migrate(oldVersion, newVersion)` 静态函数
- 链式迁移：v1.0 → v1.1 → v1.2
- 每次 GameConfigSO/DataConfig schema 升级，**同步**升级 saveVersion
- 加载时按版本链跑迁移

**当前 v1.0**（MVP）：
- 不需要迁移（初始版本）

**v1.1 预期**：
- 增加 `inputRebindings` 字段（Input GDD #1 决策）
- 迁移：v1.0 → v1.1 直接拷贝 + 加新字段

### C.7 Load Behavior

#### 加载流程
```
读取文件
    ↓
JSON 解析
    ↓
验证 schemaVersion
    ↓ (版本匹配)
加载数据到运行时
    ↓
触发 OnSaveLoaded 事件
    ↓
游戏继续
    ↓ (版本不匹配)
跑迁移 → 加载 → 触发 OnSaveLoaded
    ↓ (迁移失败)
显示错误，提示玩家
```

#### 加载前置条件
- **不能在 boss 战中加载**（先退出 boss）
- **不能在 cutscene 中加载**（先等 cutscene 结束）
- **不能在对话中加载**（先关闭对话）

#### 加载失败处理
| 失败原因 | 处理 |
|---|---|
| 文件不存在 | 提示"无存档"，返回主菜单 |
| JSON 解析失败 | 尝试 `.bak.1` → `.bak.2` → `.bak.3` |
| 备份也失败 | 显示"存档损坏"，提示从其他槽位恢复 |
| 版本不匹配 + 迁移失败 | 备份原文件，显示"版本过旧" |
| 校验失败（数据不一致） | 警告玩家，但**允许加载**（v1.0 不阻塞） |

### C.8 Auto-Save Details

#### 触发频率
- **每 30 真实分钟** 一次（兜底）
- **每日 Dawn** 一次（标志新一天）
- **Boss / Oath 完成** 立即一次
- **死亡前** 紧急一次

#### 防抖
- 两次 auto-save 至少间隔 **5 真实分钟**（防疯狂存档）
- 如果 trigger 距上次 < 5 min，跳过

#### UI 提示
- 自动存档时屏幕顶部弹"Auto-saved"提示 1 秒
- 不阻塞游戏
- 玩家可关闭此提示（设置）

### C.9 Save UI

#### 主菜单 → Load
- 显示 5 个手动槽位 + Auto + Quick（如果有）
- 每个槽位显示：
  - 缩略图（256x144 PNG）
  - 角色名 + 当前位置
  - 存档时间（"今天 14:32" / "昨天 22:15"）
  - 游戏内日期 + 时间（"Day 12, 17:30"）
  - 已玩时长（"3h 32m"）
  - 完成度（"24%"）
- 槽位空时显示"空"
- 选中槽位 → 显示"加载" / "删除" 按钮

#### 游戏中 → 暂停 → Save
- 5 个手动槽位列表
- 当前存档信息（用 thumbnail）
- 选中 → "确认保存" 按钮
- 覆盖时显示"将覆盖 slot X，确定吗？"

#### 缩略图
- **MVP**：256x144 PNG 截图（从 SaveManager 截当前游戏画面）
- **存储**：与 .save 文件同目录
- **生成时机**：每次手动/自动存档时

### C.10 Death & Recovery

#### 死亡时
1. 玩家 HP 归 0 → 触发死亡动画
2. **死亡前紧急存档**（autosave）
3. 显示"你死了"画面
4. 选项：
   - "复活于最近聚落"（消耗 token）
   - "加载最近自动存档"（默认选项）
5. 加载 autosave → 玩家从上次安全点继续

#### 数据保护
- 死亡时**不**清空 inventory
- 英灵**不**变 wight（除非强留 + 3-5 天腐化触发）
- 玩家只是回到上一个时间点

### C.11 与其他系统的交互

| 系统 | 怎么用 Save |
|---|---|
| **Player** | 位置 + 血量 + god-ember + stats |
| **Inventory** | stacks + equipment + quickbar |
| **Einherjar** | 8 个英灵的完整状态（health, daysToDeath, work, etc.） |
| **Oath** | 5 条誓言的 milestonesCompleted |
| **World** | 已访问群系 + 节点状态 + 活动事件 |
| **Time** | totalGameHours + 月相 |
| **Day-Night** | currentTimeOfDay 决定加载后的时间 |
| **Settings** | audio + 显示 + 输入重映射（v1.1） |
| **Quest-Event** | activeEvents 列表 |

**Save 是 Ravensong 的"全状态快照"** —— 任何运行时变化都进存档。

---

## D. Formulas

### D.1 存档大小估算
```csharp
int EstimateSaveSize() {
  int size = 0;
  size += 200;  // metadata
  size += 50;  // player
  size += inventory.stacks.Count * 30;  // 30 bytes / stack
  size += einherjars.Count * 200;  // 200 bytes / einherjar
  size += oaths.Count * 50;  // 50 bytes / oath
  size += 1000;  // world
  size += 100;  // time
  size += 200;  // settings
  return size;  // 估算 ~ 1-3 KB（压缩后）
}
```

### D.2 自动存档时间估算
```csharp
float EstimateSaveTime() {
  // 序列化时间：< 50ms（数据量小）
  // 写入磁盘：< 100ms（SSD）
  // 总计：< 200ms（玩家无感）
  return 0.2f;  // 秒
}
```

### D.3 防抖时间
```csharp
bool ShouldAutoSave(float lastAutoSaveTime) {
  return Time.realtimeSinceStartup - lastAutoSaveTime > 300f;  // 5 分钟
}
```

### D.4 缩略图生成
```csharp
Texture2D GenerateThumbnail(Camera cam, int width = 256, int height = 144) {
  RenderTexture rt = new RenderTexture(width, height, 24);
  cam.targetTexture = rt;
  cam.Render();
  RenderTexture.active = rt;
  Texture2D tex = new Texture2D(width, height);
  tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
  tex.Apply();
  cam.targetTexture = null;
  RenderTexture.active = null;
  return tex;
}
```

---

## E. Edge Cases

| 情况 | 处理 |
|---|---|
| 存档中引用了已删除的 ItemSO id | 加载时警告，该物品显示为"未知物品"，玩家可丢弃 |
| 存档中 8 个英灵全部死亡 | 加载成功，提示"全部英灵已逝去，是否开始新游戏" |
| 存档中玩家位置在群系外 | 加载后**传送到最近群系中心** |
| 存档在 boss 战中被触发 | 先等 boss 战结束，存到最近 slot |
| 存档在 cutscene 中 | 禁止存，显示"演出中无法保存" |
| 存档在对话中 | 关闭对话 → 允许存 |
| 存档文件损坏 | 尝试 .bak.1/.bak.2/.bak.3 → 都失败则报错 |
| 存档文件 0 字节 | 同上 |
| 存档超过 5 MB | 警告，强制精简（删除过期数据） |
| 磁盘满 | 显示错误，**不**覆盖现有存档 |
| 玩家在存档写入过程中退出 | 临时文件已 rename → 主文件完整；崩溃 → 临时文件未 rename → 主文件仍是旧版（无损） |
| 跨版本加载（v1.0 存档 → v1.1 客户端） | 跑迁移，加载成功 |
| 反向（v1.1 存档 → v1.0 客户端） | 不支持，提示"版本不兼容" |
| 玩家修改存档文件（明文 MVP） | 风险已知（playtest 接受），v1.1 加密 |
| 多个 auto-save 同时触发 | 排队执行，避免写冲突 |
| Save UI 加载中关闭 | 取消加载，不写文件 |
| 月相在存档中过期（玩家在 v1.0 加载 v0.9 存档） | 跑迁移，moonPhase 重算 |
| 英灵 daysToDeath 已到 0 | 加载后立即触发"腐化"事件 |

---

## F. Dependencies

### 上游（Save 依赖谁）

- **Data Config** —— schemaVersion 同步；ItemSO/RecipeSO 等 schema 变化时升 version
- **Day-Night** —— 读取 currentTimeOfDay
- **Inventory** —— 序列化 stacks/equipment
- **Einherjar** —— 序列化 8 个英灵
- **Oath** —— 序列化 5 条誓言
- **World Exploration** —— 序列化节点状态
- **Quest-Event** —— 序列化活动事件

### 下游（谁依赖 Save）

- **所有有运行时状态的系统**（几乎全部）

**Save 是 Ravensong 的"完整状态快照"** —— 任何有状态变化的系统都必须支持 Save/Load。

---

## G. Tuning Knobs

> 调参字段建议加到 `GameConfigSO`（data-config v1.3 阶段）

| 参数 | 默认值 | 调参影响 |
|---|---|---|
| `saveMaxManualSlots` | 5 | 手动存档槽位数 |
| `saveMaxBackupsPerSlot` | 3 | 每个槽位备份数 |
| `saveMaxSizeMB` | 5 | 存档大小限制（MB） |
| `autoSaveIntervalSec` | 1800 | 自动存档兜底间隔（30 分钟） |
| `autoSaveDebounceSec` | 300 | 自动存档防抖（5 分钟） |
| `autoSaveOnDawn` | true | Dawn 时自动存档 |
| `autoSaveOnBossKill` | true | Boss 战胜利时自动存档 |
| `autoSaveOnOathMilestone` | true | Oath 里程碑完成时自动存档 |
| `autoSaveOnDeath` | true | 死亡前紧急存档 |
| `saveEncryptionEnabled` | false | v1.0 false，release true |
| `showAutoSaveNotification` | true | 自动存档时显示提示 |
| `quickSaveKey` | F5 | 快速存档键（手柄 LB+Start） |

---

## H. Acceptance Criteria

### AC-1: 5 个手动存档槽位可用
**测试**：
1. 主菜单 → Save Game
2. 选 slot 1 → 确认 → 看到"已保存"
3. 退出游戏
4. 重新进入 → Load Game → slot 1 → 加载成功
5. **期望**：所有数据（位置、库存、英灵、誓言）完全恢复

### AC-2: Auto-save 在 Dawn 触发
**测试**：
1. 玩到 17:30 → 等待
2. 玩到 05:00（Dawn 警告）→ 06:00（Day 开始）
3. **期望**：UI 弹"Auto-saved"提示
4. **期望**：`auto.save` 文件已更新

### AC-3: Auto-save 频率兜底
**测试**：
1. 玩 30 真实分钟（不进入 Dawn / Boss / Oath 完成）
2. **期望**：触发一次 auto-save

### AC-4: 死亡前紧急存档
**测试**：
1. 玩家血量 → 0
2. 触发死亡
3. **期望**：死亡**前** auto-save 触发（"紧急快照"）
4. 加载 autosave → 玩家从死亡前 1 帧恢复

### AC-5: Quick-save / Quick-load
**测试**：
1. 游戏中按 F5 → 快速存档
2. 移动 10 秒后
3. 按 F9（quick load）→ 玩家回到 F5 时的位置
4. **期望**：quick-save/load 配对工作

### AC-6: 存档原子性
**测试**：
1. 触发 save 写入
2. 在写入过程中**强制** kill 进程（模拟崩溃）
3. 重启游戏
4. 加载存档
5. **期望**：要么旧版存档（rename 失败），要么新版存档（rename 成功）—— **不**会出现半写状态

### AC-7: 备份机制
**测试**：
1. 写 slot 1 第一次
2. 写 slot 1 第二次（覆盖）
3. 写 slot 1 第三次
4. **期望**：slot 1.save = 第三次；slot 1.bak.1 = 第二次；slot 1.bak.2 = 第一次

### AC-8: 存档大小 < 5 MB
**测试**：
1. 玩 1 真实小时（8 英灵全招、库存满、誓言完成 3 条）
2. 触发 save
3. **期望**：.save 文件 < 5 MB
4. **期望**：meta.json < 100 KB

### AC-9: Save 时间 < 200ms
**测试**：
1. 触发 save
2. 测量从开始到结束
3. **期望**：< 200ms（玩家无感）

### AC-10: 跨版本迁移
**测试**：
1. 写 v1.0 存档
2. 升级客户端到 v1.1（schema 变了）
3. 加载 v1.0 存档
4. **期望**：跑迁移成功，加载 v1.1 状态
5. **期望**：玩家数据保留 + 新字段有默认值

---

## 10. 已锁定决策（Locked Decisions）

> 2026-07-27 用户拍板，6 个开放问题全部锁定。已落地为 G 旋钮 + data-config v1.3。

| # | 决策点 | 锁定值 | 落地位置 |
|---|---|---|---|
| 1 | **存档槽位数** | **5**（手动）+ 1 auto + 1 quick | §C.1 + §C.2 |
| 2 | **自动存档频率** | **每 30 真实分钟**（保证丢失 < 30 分钟进度） | §C.4 + G 旋钮 |
| 3 | **MVP 加密** | **不加密**（playtest 友好，release 加密） | §C.5 |
| 4 | **存档触发时机** | **5 个**：dawn / boss / oath / 30min / death | §C.4 |
| 5 | **缩略图** | **截图**（256x144 PNG，< 50KB） | §C.9 |
| 6 | **多存档** | **不混乱**（UI 缩略图 + meta.json） | §C.9 |

### 决策之间的协同

- **#1 + #5**：5 槽位 + 缩略图 = **存档有视觉区分**——玩家不会"哪个是最新？"
- **#2 + #4**：30 分钟兜底 + 5 个特定时机 = **重要事件立即存档**——boss 战胜利不会被 30 分钟规则"等太久"
- **#3 + release 路径**：MVP 明文 + release 加密 = **分层发布**——playtest 阶段玩家可手动编辑调试，发布后保护
- **#6 + meta.json**：meta.json 是**多存档的总览**——5 槽位的"哪个更新"一目了然

### 仍待 playtest 调参（不阻塞 GDD 锁定）

| 待调项 | 候选范围 | 调参位置 |
|---|---|---|
| 存档大小限制 | 5 MB vs 10 MB | `GameConfigSO.saveMaxSizeMB` |
| 备份数 | 3 vs 5 | `GameConfigSO.saveMaxBackupsPerSlot` |
| 自动存档防抖 | 5 min vs 3 min | `GameConfigSO.autoSaveDebounceSec` |
| 死亡恢复策略 | 紧急存档 vs 复活点 | GDD §C.10 |

→ 这些都是 Prototype 阶段的**数值调参工作**，通过 `GameConfigSO` 直接改即可，不阻塞任何 GDD。

---

## Unity Implementation Notes

### 核心脚本（`Assets/Scripts/Save/`）
- `SaveManager.cs` —— 存档入口单例
- `SaveData.cs` —— 完整 save 数据结构（与 C.3 schema 对应）
- `SaveSerializer.cs` —— JSON 序列化（JsonUtility）
- `SaveUI.cs` —— Save/Load 菜单 UI
- `SaveThumbnail.cs` —— 缩略图生成
- `SaveMigration.cs` —— 版本迁移
- `SaveValidator.cs` —— 校验一致性
- `AutoSaveManager.cs` —— 触发时机管理
- `SaveEncryption.cs` —— v1.0 不实现，release 加密

### 事件订阅
```csharp
public class SaveManager : MonoBehaviour {
  public static event Action OnSaveStarted;
  public static event Action OnSaveCompleted;
  public static event Action<SaveData> OnSaveLoaded;
  public static event Action<string> OnSaveError;
}
```

### 原子写入
```csharp
async Task<bool> WriteAtomic(string path, string content) {
  string tempPath = path + ".tmp";
  await File.WriteAllTextAsync(tempPath, content);
  File.Move(tempPath, path);  // atomic on most filesystems
  return true;
}
```

### 备份
```csharp
void RotateBackups(string slotPath) {
  // .bak.3 → 删除
  // .bak.2 → .bak.3
  // .bak.1 → .bak.2
  // .save → .bak.1
  // 新 .save
}
```

### 性能预算
- 序列化：< 50ms（数据量 < 1MB）
- 写入磁盘：< 100ms（SSD）
- 缩略图生成：< 50ms（256x144 截图）
- 加载：< 200ms（反序列化 + 写回运行时）
- 总 save：< 200ms
- 总 load：< 500ms（含资源预加载）

### 安全考虑
- MVP 阶段不加密，**但**写明"playtest only, do not modify" 在 meta.json
- release 加密时密钥从平台 ID 生成（不让玩家控制）
- 不可写场景：boss / cutscene — 在 SaveManager.CanSave() 阻塞

---

## 进度

| 章节 | 状态 |
|---|---|
| A. Overview | ✅ |
| B. Player Fantasy / User Experience | ✅ |
| C. Detailed Design (11 小节) | ✅ |
| D. Formulas (4 个) | ✅ |
| E. Edge Cases (20 种) | ✅ |
| F. Dependencies | ✅ |
| G. Tuning Knobs (12 字段已落 v1.3) | ✅ |
| H. Acceptance Criteria (10 条) | ✅ |
| **10. Locked Decisions (6 决策)** | ✅ |
| Unity Implementation Notes | ✅ |

**总进度**: 10/10 (100%) 🔒

**🔒 已锁定 v1.0** —— 8 段全填 + 6 开放问题全部锁定 + 12 调参字段落 data-config v1.3。

---

## 变更日志

| 日期 | 版本 | 改动 | 责任人 |
|---|---|---|---|
| 2026-07-27 | v1.0 draft | 初版生成：11 段 + 公式 + Unity 实施 | Mavis |
| 2026-07-27 | **v1.0 LOCKED** | 6 开放问题用户拍板全部锁定；data-config v1.3 同步升级 | Mavis + 用户 |
