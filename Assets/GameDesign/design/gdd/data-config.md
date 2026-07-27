# Data Config — System GDD

> **Status**: 🔒 LOCKED v2.5
> **Author**: Mavis + 用户
> **Last Updated**: 2026-07-27
> **Implements Pillar**: **Foundation**（支持全部 4 根支柱）

---

## A. Overview（系统一句话）

**Data Config 是 Ravensong 全部数据驱动内容的中央注册表与版本管理。** 所有"非代码"内容——编织配方、英灵档案、物品、生物群系、誓言里程碑、世界 boss、世界事件、对话——都以 **ScriptableObject (Unity) 为唯一格式**（决策 #1 锁定：全 `.asset`，不导出 JSON 副本），存放在 `Assets/Data/` 下，由统一的 `DataRegistry` 暴露给运行时。

**全局配置**（如英灵上限 8、日夜数值）集中在**一个** `GameConfigSO` 资产里（决策 #2 锁定），不在 8 个核心数据里分散。

**数据流向**：
```
设计师 ScriptableObject 编辑
        ↓ 自动
DataRegistry 索引 (内存)
        ↓ 引用
Game Systems（编织/英灵/战斗/聚落…）
        ↓ 玩家进度
JSON Save（仅玩家数据，不是静态数据）
```

**关键属性**：
- **设计师友好**：用 Unity Inspector 编辑，**不需要写代码**就能加一条编织配方
- **强类型**：每个数据都有明确的 schema，新人也能照葫芦画瓢
- **可验证**：编辑时 + 启动时 + 加载时三道关卡抓错
- **可热重载**：play mode 改数据 → 立即生效（不用重启）
- **可版本化**：schema 升级有迁移脚本，不会因为加字段就崩存档

---

## B. Designer Experience（开发者体验）

> Data Config 不是 player-facing 系统，所以这节是给"未来要加新配方/英灵/群系的开发者"写的——也就是你（单人项目）。

**理想体验**：
> "我想加一个英灵叫 Eirik，他是猎人，会翻译卢恩，希望他在 3 天后会死。"
> → 在 Unity Project 窗口右键 → Create → Ravensong → Einherjar
> → 填 5 个字段（id / name / profession / traits / deathDay）
> → 拖一张肖像图
> → **保存，立刻在游戏里看到 Eirik 出现在战区等着被招募**

**对比"反例"**：
- ❌ 写一段 C# 代码注册新英灵（违反"设计师友好"）
- ❌ 改一个 prefab（违反"强类型"——出错了只在运行时发现）
- ❌ 改一个 Excel 表格然后跑导入脚本（违反"可热重载"——不能 play mode 实时生效）

---

## C. Detailed Design

### C.1 核心规则（Core Rules）

#### 规则 1：所有数据资产必须在 `Assets/Data/` 下
- 子目录按类型分：`Recipes/`, `Items/`, `Einherjars/`, `Biomes/`, `Oaths/`, `Bosses/`, `WorldEvents/`
- 任何不在 `Assets/Data/` 下的 ScriptableObject **不被注册**

#### 规则 2：每个资产必须有唯一 string ID
- 格式：`{type}_{name}`，全小写，snake_case
- 例：`recipe_iron_sword`, `item_ash_branch`, `einherjar_eirik_hunter`
- ID 不可修改（一旦定下就是永久标识，存档里也用它）

#### 规则 3：所有跨资产引用都用 ID，不用 Unity 引用
- 配方中的输入物品 → `itemId: "ash_branch"`
- 不用 `ItemSO` 引用，避免重新导入时 GUID 变化
- 运行时由 `DataRegistry.GetItem("ash_branch")` 解析

#### 规则 4：schema 必带版本号
- 每个数据类型有 `SchemaVersion` 字段（"1.0", "1.1", "2.0"）
- 加载时检查版本，不匹配则跑迁移
- **永远不删除旧字段**——加新字段标 `[Obsolete]`，下个版本才删

#### 规则 5：启动时三道关卡验证
- **Edit-time**：自定义 Inspector 实时检查（红框警告）
- **Play-mode start**：`DataRegistry.ValidateAll()` 全量扫描，错误入 Console
- **Runtime per-access**：懒加载 + 缺失检测（找不到的 ID 立即报警而不是返回 null）

### C.2 数据类型（16 个核心 + 1 个配置 + 1 个容器）

#### 类型 1: `RecipeSO`（编织配方）—— **签名数据**
```csharp
[CreateAssetMenu(menuName = "Ravensong/Recipe")]
public class RecipeSO : ScriptableObject, IDataValidatable {
  [Header("Identity")]
  public string id;                  // "recipe_famine_bow"
  public string displayName;         // "饥荒之弓"
  public string description;         // 玩家看到的描述
  public Sprite icon;

  [Header("Crafting")]
  public ItemStack[] inputs;         // 1-3 个输入物品
  public ItemStack output;           // 1 个输出
  public RecipeTier tier;            // 1-5（基础/常见/稀有/史诗/传说）
  public int godEmberCost;           // 消耗神力余烬数

  [Header("Requirements")]
  public DayNightRequirement dayNight;  // Any/Day/Night
  public OathType? requiredOath;        // 可选：需要先解锁某条誓言
  public string requiredOathId;         // 可选：具体里程碑

  [Header("Discovery")]
  public bool isHidden;              // 未发现前玩家看不到
  public string discoveryHint;       // "在...附近尝试编织"
}
```

**用在哪**：Fate-Thread 系统、Inventory、UI 配方列表

#### 类型 2: `ItemSO`（物品）
```csharp
[CreateAssetMenu(menuName = "Ravensong/Item")]
public class ItemSO : ScriptableObject, IDataValidatable {
  [Header("Identity")]
  public string id;
  public string displayName;
  public string description;
  public Sprite icon;

  [Header("Category")]
  public ItemCategory category;      // Resource/Equipment/Consumable/Quest/Token
  public EquipmentSlot? equipSlot;   // 可选：装备槽

  [Header("Stacking")]
  public bool stackable;
  public int maxStack = 1;

  [Header("Economy")]
  public int value;                  // 基础价值（用于交易/估值）

  [Header("Combat (if equipment)")]
  public StatBlock stats;            // 可选：攻击力/防御/特殊
  public StatusEffect? onHit;        // 可选：命中时附带的负面效果

  [Header("Consumable (if consumable)")]
  public ConsumableEffect effect;    // 可选：使用效果

  [Header("Day-Night Bonus (v1.2 新增，inventory GDD #3 锁定)")]
  public DayNightItemBonus? dayBonus;     // 装备时白天附加
  public DayNightItemBonus? nightBonus;   // 装备时夜间附加
}

[Serializable]
public class DayNightItemBonus {
  public StatBlock stats;                  // 装备时附加的属性
  [TextArea] public string description;    // UI 提示
}
```

**v1.3 新增：`ToolSO` 扩展 `ItemSO`**（gathering GDD 锁定）
```csharp
[CreateAssetMenu(menuName = "Ravensong/Tool")]
public class ToolSO : ItemSO {
  [Header("Tool Type")]
  public ToolType toolType;          // Axe/Pick/Bow/FishingRod
  public int tier;                    // 1-5
  public bool isTwoHanded;            // 双手武器（占用 Main + 禁止 Off）

  [Header("Gather Modifiers")]
  public float gatherSpeedMult;       // 1.0 - 3.0
  public float gatherYieldMult;       // 1.0 - 2.0
  public int baseDurability;          // 50-500（实际值从 GameConfigSO 读）
}
```
**用在哪**：Gathering 系统、Inventory（装备栏）、UI

**用在哪**：Inventory、Equipment、Gathering、Weaving

#### 类型 3: `EinherjarSO`（英灵档案）—— **签名数据**
```csharp
[CreateAssetMenu(menuName = "Ravensong/Einherjar")]
public class EinherjarSO : ScriptableObject, IDataValidatable {
  [Header("Identity")]
  public string id;
  public string displayName;
  public Sprite portrait;
  public string backstory;           // 招募前显示的背景故事

  [Header("Vitals")]
  public Profession profession;      // Blacksmith/Hunter/Skald/Farmer/Warrior
  public int ageAtRecruitment;       // 招募时年龄（决定死亡时间）
  public Trait[] traits;             // 1-3 个性格特征

  [Header("Work")]
  public ResourceType workType;      // 主要产出什么资源
  public float workEfficiency;       // 0.5 - 2.0

  [Header("Death (signature)")]
  public bool willDie;               // 是否注定死亡（true 给"告别"剧情）
  public int daysToDeath;            // 招募后几天会死（用于"缓慢腐化"机制）
  public string deathQuote;          // 死前最后一句话
  public string valhallaReward;      // 送走后给的永久 buff 描述
  public StatBlock valhallaBuff;     // 数值化的 buff

  [Header("Voice")]
  public AudioClip greetingLine;     // 招募时的对话
  public AudioClip workLine;         // 工作时的喃喃
  public AudioClip dyingLine;        // 临终的告别
}
```

**用在哪**：Einherjar Management、Death & Send-off、UI Portrait、Settlement

#### 类型 4: `BiomeSO`（生物群系）
```csharp
[CreateAssetMenu(menuName = "Ravensong/Biome")]
public class BiomeSO : ScriptableObject, IDataValidatable {
  [Header("Identity")]
  public string id;
  public string displayName;
  public Sprite mapIcon;
  public Color ambientColor;          // 雾/天空主色

  [Header("Atmosphere")]
  public AudioClip ambientLoop;
  public AudioClip musicTrack;
  public LightingPreset lighting;    // 引用 Unity 2D Light 预设

  [Header("Resources")]
  public ResourceSpawn[] resources;  // 该群系可采集什么
  public EnemySpawn[] enemies;       // 该群系刷什么敌人

  [Header("Unlock")]
  public UnlockCondition[] unlocks;  // 如何解锁进入（誓言 + 英灵任务）

  [Header("Boss")]
  public string bossId;              // 该群系的世界 boss（如果有）
}
```

**用在哪**：World Exploration、Day-Night（光照）、Audio

#### 类型 5: `OathSO`（誓言）
```csharp
[CreateAssetMenu(menuName = "Ravensong/Oath")]
public class OathSO : ScriptableObject, IDataValidatable {
  [Header("Identity")]
  public string id;                  // "oath_smithing", "oath_hearth", etc.
  public string displayName;         // "锻冶之誓"
  public string description;
  public Sprite icon;

  [Header("Theme")]
  public OathTheme theme;            // Wyrd/Woven/Hearth/Land/Divine

  [Header("Milestones (5)")]
  public Milestone[] milestones;     // 固定 5 个，按 systems-index §10.3 锁定

  [Header("Final Reward")]
  public StatBlock completionBuff;   // 完成全部 5 里程碑后给的永久 buff
  public string finalQuote;          // 奥丁审判时的对话
}
```

**用在哪**：Oath System、UI 誓言页、Quest-Event（誓言作为软主线）

#### 类型 6: `BossSO`（世界 boss）
```csharp
[CreateAssetMenu(menuName = "Ravensong/Boss")]
public class BossSO : ScriptableObject, IDataValidatable {
  [Header("Identity")]
  public string id;
  public string displayName;
  public Sprite portrait;
  public AudioClip encounterMusic;

  [Header("Phases (3)")]
  public BossPhase[] phases;         // 固定 3 阶段

  [Header("Combat")]
  public float baseHealth;
  public DamageType[] weaknesses;    // 编织哪种武器伤害 +X
  public StatBlock stats;

  [Header("Fate-Thread Interaction (signature)")]
  public string[] threadBindableAttacks;  // 哪些招式可以被织线绑定
  public float threadBindDuration;        // 绑住后 boss 硬直多久

  [Header("Rewards")]
  public ItemStack[] guaranteedDrops;
  public LootTable lootTable;
}
```

**用在哪**：Combat、World Exploration、Quest-Event

#### 类型 7: `WorldEventSO`（世界事件）
```csharp
[CreateAssetMenu(menuName = "Ravensong/World Event")]
public class WorldEventSO : ScriptableObject, IDataValidatable {
  [Header("Identity")]
  public string id;
  public string displayName;
  public string description;
  public Sprite icon;

  [Header("Trigger")]
  public EventTrigger[] triggers;    // 时间/地点/玩家状态 触发
  public float weight;               // 同一时间多个事件被选中的权重

  [Header("Duration")]
  public float durationHours;        // 持续多少游戏内小时

  [Header("Effects")]
  public WorldModifier[] modifiers;  // 影响世界（如"渡鸦带来奥丁诏令"）
  public ItemStack[] rewards;
  public DialogueLine[] dialogue;    // 事件相关对话

  [Header("Recurrence")]
  public bool oneTime;               // 一次性还是可重复
  public float cooldownHours;        // 重复间隔
}
```

**用在哪**：Quest-Event、World Exploration、UI 通知

#### 类型 8: `DialogueSO`（对话）—— **决策 #5 锁定**
```csharp
[CreateAssetMenu(menuName = "Ravensong/Dialogue")]
public class DialogueSO : ScriptableObject, IDataValidatable {
  [Header("Identity")]
  public string id;                  // "dialogue_eirik_recruit"
  public string displayName;         // 调试用，玩家不直接看到

  [Header("Lines (MVP: 线性)")]
  public DialogueLine[] lines;       // 顺序播放；未来扩展为树
}

[Serializable]
public struct DialogueLine {
  public string speakerId;           // 说话人（"odin" / "eirik_hunter" / "narrator"）
  public string portraitId;          // 头像 item id（指向 ItemSO category=Portrait）
  [TextArea(2, 5)]
  public string text;                // 玩家看到的对话文本
  public string nextDialogueId;      // 可选：链向下一段对话（null = 对话结束）
  public Condition[] conditions;     // 可选：每行的触发条件（MVP 可空）
}
```
**MVP 范围**：线性展示（按 lines 顺序播），不支持分支
**未来扩展**：通过 `nextDialogueId` 链成树，可演化为 Yarn Spinner / Ink 集成
**用在哪**：WorldEvent（事件触发对话）、Einherjar 招募/死亡对话、Oath 仪式对话

#### 类型 9: `GameConfigSO`（全局配置）—— **决策 #2 锁定**
```csharp
[CreateAssetMenu(menuName = "Ravensong/Game Config")]
public class GameConfigSO : ScriptableObject, IDataValidatable {
  [Header("Game Balance")]
  public int maxEinherjarInSettlement = 8;     // 锁定决策 #4
  public int maxOathMilestones = 5;             // 锁定决策 #3
  public int minRecipesPerOath = 5;

  [Header("Day-Night (锁定决策 #1)")]
  // v1.0 基础
  public float dayDurationHours = 12f;
  public float nightDurationHours = 12f;
  public float daySpeedFactor = 0.5f;           // 白天 -50%
  public float nightSpeedFactor = 1.6f;          // 夜晚 +60%
  public float dayVisionFactor = 0.5f;          // 视野 -50%
  public float nightVisionFactor = 1.6f;         // 视野 +60%
  public float dayWeavingTimeFactor = 1.5f;     // 白天慢 50%
  public float nightWeavingTimeFactor = 0.5f;    // 夜晚快 50%
  // v1.1 新增（day-night-cycle GDD 锁定后）
  public float realSecondsPerGameHour = 30f;     // 1 游戏内小时 = 30 真实秒
  public float dayCombatDamageFactor = 0.75f;    // 白天战斗伤害
  public float nightCombatDamageFactor = 1.2f;   // 夜晚战斗伤害
  public float dawnDurationHours = 1f;           // 黎明转换时长
  public float duskDurationHours = 1f;           // 暮色转换时长
  public float transitionVisualDurationSec = 60f;// 视觉转换真实时长
  public float odinEyeScanChance = 0.3f;         // 奥丁之眼扫描成功率
  public float odinEyeScanIntervalHours = 0.5f;  // 扫描判定间隔（游戏内小时）
  public float moonPhaseCycleDays = 8f;          // 月相完整循环天数（决策 #3）

  [Header("Enemy Alert (day-night-cycle #5 锁定)")]
  public bool showEnemyAlertRange = true;        // 是否显示敌人警觉光圈
  public float enemyAlertRangeBase = 5f;         // 基础警觉范围（网格）
  public float enemyAlertRangeDayMultiplier = 1.3f; // 白天警觉范围乘数

  [Header("Inventory (inventory GDD 锁定)")]
  public int inventoryMaxSlots = 24;             // 决策 #1
  public float inventoryGroundItemLifetimeHours = 0.5f; // 决策 #5（30 真实秒）
  public int quickbarSlotCount = 4;              // 决策 #6
  public float quickbarSwitchCooldownSec = 0.3f;
  public float pickupRange = 1.0f;               // 拾取近距离（格）
  public float pickupLongPressRange = 5.0f;      // 长按拾取距离（格）
  public float pickupLongPressDurationSec = 0.5f;
  public int defaultMaxStackResource = 99;
  public int defaultMaxStackConsumable = 20;
  public float dayNightBonusMaxRatio = 0.5f;     // Bonus 不超过基础 50%（决策 #3）

  [Header("Resources (锁定决策 #2)")]
  public float godEmberPassivePerHour = 1f;     // 被动基底
  public float godEmberFromCombatMultiplier = 0.5f;  // 杀敌奖励乘数
  public float godEmberFromWeavingRefundRatio = 0.3f;  // 织出物品返还 30%

  [Header("Death (锁定决策 #6)")]
  public int corpseDecayMinDays = 3;            // 强留尸鬼腐化时间
  public int corpseDecayMaxDays = 5;

  [Header("Performance (锁定决策 #3 + #4)")]
  public int maxValidationTimeMs = 200;
  public bool hotReloadEnabled = true;
  public int hotReloadDebounceMs = 500;
  public bool loadAsync = false;                // MVP 同步；EA 转 true

  [Header("Gathering (gathering GDD 锁定)")]
  public float gatherDayEfficiencyMult = 0.7f;   // 决策 #1（白天 -30%）
  public float gatherNightEfficiencyMult = 1.5f; // 决策 #1（夜晚 +50%）
  public float gatherBaseTimeChop = 3.0f;
  public float gatherBaseTimeMine = 4.0f;
  public float gatherBaseTimePick = 1.0f;
  public float gatherBaseTimeHunt = 5.0f;
  public float gatherBaseTimeFish = 4.0f;
  public int toolDurabilityTier1 = 50;            // 决策 #1（做耐久）
  public int toolDurabilityTier2 = 100;
  public int toolDurabilityTier3 = 200;
  public int toolDurabilityTier4 = 350;
  public int toolDurabilityTier5 = 500;
  public float biomeRegrowIntervalHours = 24f;   // 决策 #5
  public float biomeRegrowChance = 0.3f;
  public float biomeRegrowRatio = 0.3f;
  public float nodeRenewableRespawnHours = 24f;
  public float specialResourceNightBonus = 1.5f;  // 决策 #7

  [Header("Input (input-system GDD 锁定)")]
  public float inputBufferWindowSec = 0.2f;       // 决策 #5
  public float inputLeftStickDeadzone = 0.2f;     // 决策：避免漂移
  public float inputRightStickDeadzone = 0.15f;
  public float inputTriggerDeadzone = 0.1f;
  public bool enableInputRebinding = false;       // 决策 #1（v1.0 false，v1.1 true）
  public float mouseAimSensitivity = 1.0f;        // 决策 #2（v1.0 不需要）
  public float gamepadAimSensitivity = 1.0f;
  public float minCutsceneSkipDurationSec = 5f;   // 决策 #6
  public bool pauseFreezeTime = true;

  [Header("Save (save-system GDD 锁定)")]
  public int saveMaxManualSlots = 5;              // 决策 #1
  public int saveMaxBackupsPerSlot = 3;
  public int saveMaxSizeMB = 5;
  public int autoSaveIntervalSec = 1800;          // 决策 #2（30 分钟）
  public int autoSaveDebounceSec = 300;            // 5 分钟防抖
  public bool autoSaveOnDawn = true;               // 决策 #4
  public bool autoSaveOnBossKill = true;
  public bool autoSaveOnOathMilestone = true;
  public bool autoSaveOnDeath = true;
  public bool saveEncryptionEnabled = false;      // 决策 #3（v1.0 false）
  public bool showAutoSaveNotification = true;
  public string quickSaveKey = "F5";

  [Header("Combat (combat GDD 锁定)")]
  public int combatPlayerMaxHP = 100;              // 玩家血量上限
  public int combatPlayerMaxStamina = 100;         // 玩家 Stamina 上限
  public float combatStaminaRegenPerSec = 15f;
  public float combatStaminaRegenIdleMultiplier = 2.0f;
  public float combatLightAttackDamageMult = 1.0f;
  public float combatHeavyAttackDamageMult = 1.8f;
  public float combatComboBonusMult = 1.3f;        // 3 段 combo 第 3 击
  public float combatBlockDamageReduction = 0.7f;  // 决策 #3
  public float combatPerfectBlockWindowSec = 0.1f; // 决策 #8
  public float combatDodgeIFrameSec = 0.3f;
  public float combatDodgeDistance = 4f;
  public float combatThreadBindCooldownSec = 5f;   // 决策 #6
  public float combatThreadBindStunSec = 2f;        // boss 硬直
  public float combatThreadBindStunSecNormal = 1f;  // 普通敌人硬直
  public int combatThreadBindGodEmberCost = 15;
  public int combatThreadBindStaminaCost = 30;
  public float combatThreadBindRange = 8f;
  public float combatComboWindowSec = 0.5f;
  public float combatEnemyAlertRangeDayMult = 1.3f; // 决策 #4 + Day-Night
  public float combatEnemyAlertRangeNightMult = 1.0f;
  public int combatBossPhasesCount = 3;             // 决策：永远 3
  public bool combatBossTimeFreeze = true;
  public float combatDropOnGroundLifetimeSec = 15f;

  [Header("Fate-Thread (fate-thread GDD 锁定)")]
  public float weaveBaseTimeSec = 3.0f;
  public float weaveDayTimeMult = 1.5f;             // 决策 #5
  public float weaveNightTimeMult = 0.5f;
  public float weaveDayEffectMult = 0.7f;
  public float weaveNightEffectMult = 1.5f;
  public float weaveFailureRateTier3 = 0.05f;
  public float weaveFailureRateTier4 = 0.10f;
  public float weaveFailureRateTier5 = 0.15f;
  public float weaveFullMoonTier5Bonus = 1.5f;      // 决策 #3
  public int weaveMaxInputs = 3;                    // 决策 #2
  public int weaveGodEmberBaseCost = 5;             // Tier 1 cost
  public int weaveGodEmberPerTier = 5;              // 每 Tier +5
  public float weaveInterruptGodEmberRatio = 0.5f;  // 决策 #5
  public int weaveHintCooldown = 30;
  public int weaveRecipeDiscoveryToastSec = 3;

  [Header("Einherjar (einherjar GDD 锁定)")]
  public int professionMaxCount = 2;
  public int professionLevelUpDays = 5;
  public int professionLevelMax = 3;
  public float relationshipFriendlyMult = 1.2f;    // 决策 #2
  public float relationshipHostileMult = 0.8f;
  public int mourningPeriodHours = 24;
  public float mourningEfficiencyMult = 0.8f;
  public int decayLevelMax = 5;
  public float einherjarDayEfficiencyMult = 0.5f;
  public float einherjarNightEfficiencyMult = 1.5f;
  public int dyingDecisionWindowHours = 24;          // 决策 #5
  public int professionSwitchCooldownHours = 24;    // 决策 #8
  public bool valhallaBuffApplication = true;        // 决策 #7
  public int einherjarRecruitedVoicelineCount = 3;   // 招募时播放几条
  public float corpseDecayVisualStep = 1f;          // 腐化等级视觉步进秒数

  [Header("Oath (oath-system GDD 锁定 — v1.6 补 schema)")]
  public int oathMilestonesPerOath = 5;              // 决策 #11
  public int oathCount = 5;                          // 决策 #1
  public int oathRequiredForSkyOath = 4;             // 决策 #7
  public float oathCompleteAnimationSec = 5f;        // 决策 #10
  public bool oathMilestoneProgressBar = true;
  public bool oathAutoApplyBuff = true;              // 决策 #3
  public float odinTrialDurationSec = 180f;          // 决策 #5（3 min 独白）
  public float odinTrialMinSecondsBeforeChoice = 10f; // 决策 #6（前 10s 不可跳）
  public bool endgameResetAfterChoice = false;       // 结局 1 回主菜单 / 结局 2 继续
  public bool newGamePlusEnabled = false;            // 决策 #4（v1.0 无，v1.1 有）
  public float endgameCreditsRollSec = 90f;          // 演职员表 90s
  public float skyOathUnlockVisualDuration = 8f;     // 苍穹解锁仪式

  [Header("Death & Send-off (death-sendoff GDD 锁定 — v1.7 新增)")]
  public SendoffDefaultChoice dyingDefaultChoice = SendoffDefaultChoice.Refuse; // 决策 #1（24h 后默认强留）
  public bool sendOffCinematicSkipEnabled = false;   // 决策 #2（不可跳）
  public int battleSendoffGodEmberCost = 0;          // 决策 #3（v1.0 免费）
  public float memorialMoodBonusPerMemorial = 0.01f; // 决策 #4（每个 +1% 士气）
  public int memorialMaxCount = 8;                   // 决策 #4（=英灵上限）
  public float wyrdAnchorEfficiencyBonus = 0.1f;     // 决策 #6（Wyrd 锚点 +10%）
  public bool wyrdAnchorStopsDecay = true;           // 决策 #7（阻止强留腐化）
  public float letRestDurationDays = 6f;             // 决策 #8（5-7 天中位数）
  public bool deathOathMilestoneProgressBar = true;  // 决策 #9（亡者之誓进度条可见）

  [Header("UI / HUD (ui-hud GDD 锁定 — v1.8 新增)")]
  public float uiFrameBudgetMs = 16f;                // 决策：60 FPS 预算
  public float uiFadeInSec = 0.5f;                   // 决策 #2（淡入时长）
  public float uiFadeOutSec = 0.3f;                  // 决策 #2（淡出时长）
  public bool uiThemeAutoDayNight = true;            // 决策 #7（自动跟随 Day-Night）
  public bool uiSoundEnabled = true;                 // 决策 #11（UI 声音全开可关）
  public float uiToastDefaultSec = 2f;               // 决策 #6（Toast 时长）
  public int uiToastMaxStack = 3;                    // 堆叠上限
  public bool uiTutorialEnabled = true;              // 决策 #5（教程启用）
  public float uiDialogueAutoPlaySec = 3f;           // 对话自动播放间隔
  public bool uiModalPauseGame = true;               // 决策 #3（弹窗暂停）
  public float uiHealthBarLength = 200f;             // 血量条长度（像素）
  public float uiCompassShowDistanceMax = 200f;      // 罗盘显示最大距离

  [Header("Settlement (settlement GDD 锁定 — v1.9 新增)")]
  public int longhouseCapacityLevel1 = 4;            // 决策 #1
  public int longhouseCapacityLevel2 = 8;            // 决策 #2（=v1.0 上限）
  public int longhouseCapacityLevel3 = 12;           // v1.1 决策
  public int forgeMaxTierLevel1 = 3;                 // 决策 #3
  public int forgeMaxTierLevel2 = 4;                 // 决策 #4
  public float hearthMoraleBonusLevel1 = 0.15f;      // 决策 #5
  public float hearthMoraleBonusLevel2 = 0.30f;      // 决策 #6
  public int storageSlotsLevel1 = 24;                // 决策 #7
  public int storageSlotsLevel2 = 48;                // 决策 #8
  public float shrineGodEmberMultiplierLevel2 = 1.5f; // 决策 #9
  public int shrineMourningAccelerateCost = 20;      // 决策 #10
  public float buildingUpgradeVfxHours = 24f;        // 决策 #11

  [Header("World Exploration (world-exploration GDD 锁定 — v2.0 新增)")]
  public int biomeCount = 6;                         // 决策 #1
  public int poiTotalCount = 70;                     // 决策 #2
  public float hiddenRecipePercentage = 0.20f;       // 决策 #3
  public int hiddenRecipeCount = 6;                  // 决策 #3
  public int expeditionMaxDurationDays = 7;          // 决策 #4
  public float expeditionRewardLegendaryChance = 0.01f; // 决策 #5
  public float expeditionRewardRareChance = 0.04f;  // 决策 #6
  public float biomeNightDifficultyBonus = 1.3f;     // 决策 #7
  public float biomeTransitionMeters = 30f;          // 决策 #8
  public ExpeditionType expeditionType = ExpeditionType.Player; // 决策 #9
  public float compassMaxDistance = 200f;            // 决策 #10（与 ui-hud 同步）
  public int frostCliffColdDamagePerMin = 1;         // 决策 #11

  [Header("Quest & Event (quest-event GDD 锁定 — v2.1 新增)")]
  public int mainQuestCount = 10;                    // 决策 #1（=5 誓言 × 2）
  public int sideQuestCount = 15;                    // 决策 #2
  public int dailyQuestCount = 10;                   // 决策 #3
  public float dailyQuestRefreshHours = 24f;         // 决策 #4
  public int worldEventMaxActive = 3;                // 决策 #5
  public float worldEventMinIntervalHours = 12f;     // 决策 #6
  public int worldEventMinTriggerPerPlaythrough = 5; // 决策 #7
  public bool questAbandonAllowed = false;           // 决策 #8（v1.0 锁定）
  public bool mainQuestSequential = true;            // 决策 #9
  public int questLogMaxActive = 30;                 // 决策 #10
  public float questCompleteToastSec = 4f;           // 决策 #11
  public float questRewardMultiplier = 1.0f;         // 决策 #12（v1.0 基准）

  [Header("VFX (vfx GDD 锁定 — v2.2 新增)")]
  public int vfxMaxParticlesPerFrame = 500;          // 决策 #1（性能+视觉平衡）
  public int vfxFpsTarget = 60;                      // 决策 #2（目标帧率）
  public float vfxFadeInSec = 0.5f;                  // 决策 #3（淡入）
  public float vfxFadeOutSec = 0.3f;                 // 决策 #4（淡出）
  public float vfxLodNearMeters = 20f;               // 决策 #5（LOD 近）
  public float vfxLodMidMeters = 50f;                // 决策 #6（LOD 中）
  public float vfxLodFarMeters = 100f;               // 决策 #7（LOD 远=不渲染）
  public float vfxReducedMultiplier = 0.5f;          // 决策 #8（减少模式乘数）
  public float weatherChangeIntervalHours = 6f;      // 决策 #9（天气最小间隔）
  public float fullMoonGodsightChance = 0.5f;        // 决策 #10（满月神显 Tier 5 概率）
  public int vfxOdinTrialParticles = 200;            // 决策 #12（奥丁审判粒子数）

  [Header("Audio (vfx-audio GDD 锁定 — v2.3 新增)")]
  public int audioMaxSources = 8;                     // 决策 #1（AudioSource 上限）
  public float audioMasterVolume = 0.8f;             // 决策 #2（主音量）
  public float audioMusicVolume = 0.5f;              // 决策 #3（音乐）
  public float audioAmbientVolume = 0.7f;            // 决策 #3（环境）
  public float audioSfxVolume = 0.8f;                // 决策 #3（SFX）
  public float audioUiVolume = 0.6f;                 // 决策 #3（UI）
  public float audioDayNightMult = 1.0f;             // 决策 #4（夜晚音量倍率）
  public float audioCombatMusicMult = 1.5f;          // 决策 #5（战斗音量倍率）
  public float audioRitualMusicMult = 1.2f;          // 决策 #6（仪式音量倍率）
  public float audioFadeSec = 1.0f;                  // 决策 #7（混音淡变）
  public float audioMusicFadeSec = 2.0f;             // 决策 #8（跨场景切换）
  public float audioOdinTrialVolume = 1.0f;          // 决策 #9（奥丁审判）

  [Header("Camera (camera GDD 锁定 — v2.4 新增)")]
  public float cameraFollowLerpSpeed = 5f;           // 决策 #1（跟随速度，0.1s 延迟）
  public float cameraCombatOffsetMultiplier = 1.2f;  // 决策 #2（战斗偏移）
  public float cameraCombatLerpSpeed = 3f;           // 决策 #3（战斗速度）
  public float cameraRitualZoom = 0.8f;              // 决策 #4（仪式缩放=拉远）
  public float cameraRitualLerpSpeed = 2f;           // 决策 #5（仪式速度）
  public float cameraTransitionZoom = 1.2f;          // 决策 #6（过渡缩放=拉近）
  public float cameraTransitionDurationSec = 1f;     // 决策 #7（过渡时长）
  public float cameraDeathFadeSec = 0.5f;            // 决策 #8（死亡淡黑）
  public float cameraDeathZoom = 0.7f;               // 决策 #9（死亡缩放）
  public float cameraFollowOffsetZ = -10f;           // 决策 #10（Z 偏移）
  public float cameraMaxZoom = 2.0f;                 // 决策 #11（v1.1 最大缩放）
  public float cameraMinZoom = 0.5f;                 // 决策 #12（v1.1 最小缩放）

  [Header("Boss (boss-design GDD 锁定 — v2.5 新增，修复 P0 缺口)")]
  public int bossPhaseCount = 3;                      // 决策 #1（与 combat 锁定一致）
  public float bossBaseHP = 1000f;                   // 决策 #2（骨王基础血量）
  public float bossMoveWarningSec = 0.8f;            // 决策 #3（招式预警）
  public float bossPhaseTransitionSec = 1.0f;        // 决策 #4（阶段转换黑屏）
  public float bossCooldownHours = 24f;              // 决策 #5（失败冷却）
  public float bossDeathCinematicSec = 5f;           // 决策 #6（死亡演出）
  public float bossEntryCinematicSec = 180f;          // 决策 #7（房演出 3min）
  public int bossGodEmberHealingLimit = 1;           // 决策 #8（治疗上限/分钟）
  public float bossHealingShrineRange = 30f;         // 决策 #9（神龛治疗范围）
  public bool bossNightOnly = true;                  // 决策 #10（前 3 boss 必夜晚）
  public float bossMoraleBonusOnKill = 0.05f;        // 决策 #11（聚落士气加成）
  public float bossDifficultyMultiplierBase = 1.0f;  // 决策 #12（骨王基准系数）

  [Header("Build")]
  public bool logDataAccess = false;            // dev = true
  public float validationStrictness = 0.8f;     // dev = 1.0
}
```
**用在哪**：所有需要平衡数值的系统（Day-Night、Inventory、Combat、Death 等）。**它是"所有锁定决策的数值执行点"**——修改它即可整体调参，无需重编译。

#### 类型 10: `SendoffSO`（送别方式）—— **death-sendoff GDD 锁定**
```csharp
[CreateAssetMenu(menuName = "Ravensong/Sendoff")]
public class SendoffSO : ScriptableObject, IDataValidatable {
  [Header("Identity")]
  public string id;                       // "sendoff_valhalla" / "sendoff_battle" / "sendoff_burial" / "sendoff_refuse" / "sendoff_letrest"
  public string displayName;              // 调试用

  [Header("Type & Cost")]
  public SendoffType type;                // Valhalla / Battle / Burial / Refuse / LetRest
  public int godEmberCost;                // 0=免费（如战斗葬礼 v1.0）
  public float durationSec;               // 演出时长（0-10s）

  [Header("Effects")]
  public StatBlock permanentBuff;         // 送英灵殿/战斗葬礼的永久 buff
  public bool createsMemorial;            // 是否立纪念碑
  public bool triggersMourningPeriod;     // 是否触发 24h 衰悼期
  public bool countsAsDeathOathMilestone41; // 亡者之誓 4.1 计数
  public bool countsAsDeathOathMilestone45; // 亡者之誓 4.5 计数（仅送英灵殿 = true）
}
```

**MVP 范围**：5 种送别方式（Valhalla / Battle / Burial / Refuse / LetRest），每种 1 个 `SendoffSO` 资产
**用在哪**：Einherjar 死亡、远征兵 v1.1 死亡、boss 击杀、驯服野兽老死

#### 类型 11: `UIStyleSO`（UI 风格主题）—— **ui-hud GDD 锁定**
```csharp
[CreateAssetMenu(menuName = "Ravensong/UI Style")]
public class UIStyleSO : ScriptableObject, IDataValidatable {
  [Header("Identity")]
  public string id;                       // "style_day_dark_painterly" / "style_night_dark_painterly"
  public string displayName;              // 调试用

  [Header("Colors (Color4 with alpha)")]
  public Color4 backgroundColor;          // 半透明深色画布 (0,0,0,0.7)
  public Color4 borderColor;              // 金色卢恩符文
  public Color4 textColor;                // 米色
  public Color4 textAccentColor;          // cyan 强调
  public Color4 buttonColor;              // cyan 描边

  [Header("Visual")]
  public Sprite borderSprite;             // 卢恩符文 9-slice
  public Font fontAsset;                  // 衬线体（中英混合）
  public float cornerRadius;              // 4px
  public float painterlyNoiseStrength;    // 5% 油画纹理强度

  [Header("Animation")]
  public float fadeInDurationSec;         // 0.5s
  public float fadeOutDurationSec;        // 0.3s
  public bool autoDayNight;               // true=自动跟随 Day-Night
}
```

**MVP 范围**：2 套主题（白天暖色 / 夜晚冷色）+ 1 套衬线字体 + 1 套 9-slice 边框
**用在哪**：HUD / 弹窗 / 菜单 / Toast / 对话 / 教程（所有 UI 元素）
**v1.0 锁定决策**：深色油画风格（与 `style-bible.md` 一致）

#### 类型 12: `SettlementSO`（聚落状态）—— **settlement GDD 锁定**
```csharp
[CreateAssetMenu(menuName = "Ravensong/Settlement")]
public class SettlementSO : ScriptableObject, IDataValidatable {
  [Header("Identity")]
  public string id;                       // "settlement_main"（v1.0 玩家只 1 个聚落）
  public string displayName;              // "Ravensong Settlement"

  [Header("Buildings (5 基础)")]
  public BuildingState longhouse;         // type=Longhouse, level 1-2
  public BuildingState forge;             // type=Forge, level 1-2
  public BuildingState hearth;            // type=Hearth, level 1-2 (Living Hearth 核心)
  public BuildingState storage;           // type=Storage, level 1-2
  public BuildingState shrine;            // type=Shrine, level 1-2

  [Header("Oath Buildings (0-5 誓言)")]
  public List<OathBuilding> oathBuildings; // 由誓言 5/5 触发，自动出现

  [Header("Memorials (与 death-sendoff 协同)")]
  public List<string> memorialEinherjarIds; // 立碑的英灵 ID

  [Header("State")]
  public float mourningHoursRemaining;    // 衰悼期倒计时
  public ResourcePool resources;          // 4 种资源池
}

[Serializable]
public class BuildingState {
  public BuildingType type;               // Longhouse / Forge / Hearth / Storage / Shrine
  public int level;                       // 1-2 (3 in v1.1)
  public float upgradeProgressHours;      // 0-24h 升级中
  public Vector2Int position;             // 聚落内位置
}

[Serializable]
public class OathBuilding {
  public OathType oath;                   // Smithing / Hearth / Wild / Death / Sky
  public Vector2Int position;             // 固定位置
  public bool active;                     // 是否生效
}

[Serializable]
public class ResourcePool {
  public float wood;                      // 木材
  public float iron;                      // 铁
  public float food;                      // 食物
  public float herbs;                     // 草
}
```

**MVP 范围**：1 个聚落（玩家只 1 个）；5 基础建筑初始 Level 1；0-5 誓言建筑由誓言触发
**用在哪**：聚落菜单（按 C）/ 状态条（HUD）/ 编织（工坊 Level）/ 招募（长屋容量）/ 衰悼期（神龛）
**v1.0 锁定决策**：固定位置、5 基础 + 5 誓言 = 10 建筑；不开放自由布局

#### 类型 13: `ExpeditionSO`（远征任务）—— **world-exploration GDD 锁定**
```csharp
[CreateAssetMenu(menuName = "Ravensong/Expedition")]
public class ExpeditionSO : ScriptableObject, IDataValidatable {
  [Header("Identity")]
  public string id;                       // "expedition_frost_cliff_long_01"
  public string displayName;              // "永冻崖长途远征"
  public string description;              // 远征简介

  [Header("Route")]
  public BiomeSO fromBiome;               // 出发群系
  public BiomeSO toBiome;                 // 目标群系
  public string targetPOI;                // 远征目标 POI ID

  [Header("Type & Duration")]
  public ExpeditionType type;             // Player / Expeditioner (v1.1)
  public ExpeditionRisk risk;             // Low / Medium / High
  public float baseDurationDays;          // 基础时长（按 risk 乘 1.0/1.5/2.0）
  public float maxDurationDays;           // 决策 #4：v1.0 = 7

  [Header("Rewards")]
  public ExpeditionReward rewardTemplate; // 奖励模板
  public float legendaryChance;           // 决策 #5：1%
  public float rareChance;                // 决策 #6：4%
  public float uncommonChance;            // 15%
  public float commonChance;              // 80%

  [Header("v1.1 Hooks")]
  public List<string> requiredProfessions; // 远征兵 v1.1 需要的 Profession
  public float deathChance;                // 远征兵 v1.1 死亡概率（v1.0 = 0）
}

[Serializable]
public class ExpeditionReward {
  public ResourceType resourceType;       // 资源类型
  public float minAmount;                 // 最小数量
  public float maxAmount;                 // 最大数量
  public string hiddenRecipeHintId;       // 隐藏配方线索（可选）
  public string einherjarRecruitId;       // 招募英灵 ID（可选）
}
```

**MVP 范围**：v1.0 玩家亲自远行；约 18 个远征任务（6 基地 × 3 难度）
**v1.1 扩展**：远征兵系统（派英灵远征，可死亡触发 Death-Send-off 类型 4）
**用在哪**：远征基地（聚落内）/ 任务日志 / 探索奖励

#### 类型 14: `QuestSO`（任务）—— **quest-event GDD 锁定**
```csharp
[CreateAssetMenu(menuName = "Ravensong/Quest")]
public class QuestSO : ScriptableObject, IDataValidatable {
  [Header("Identity")]
  public string id;                       // "quest_main_01_hearth_ignite"
  public string displayName;              // "炉火初燃"
  public string description;              // 任务描述（叙事钩子）
  public Sprite icon;

  [Header("Type")]
  public QuestType type;                  // Main / Side / Expedition / Daily
  public int orderIndex;                  // 主线顺序（=orderIndex 升序触发）

  [Header("Trigger")]
  public QuestTrigger[] triggers;         // 触发条件（自动/剧情/远征/周期）

  [Header("Conditions (1-6)")]
  public QuestCondition[] conditions;      // Kill / Gather / Craft / Explore / SendOff / Recruit

  [Header("Rewards")]
  public QuestReward[] rewards;           // Oath 进度 / 永久 buff / 资源 / 物品 / god-ember / 隐藏配方

  [Header("Failure")]
  public bool canFail;                    // 任务可失败（远征 = true，主线/支线 = false）
  public float timeoutRealHours;          // 超时（仅日常 = 24）
}

[Serializable]
public class QuestCondition {
  public ConditionType type;              // Kill / Gather / Craft / Explore / SendOff / Recruit
  public string targetId;                 // 目标 ID（敌人/资源/配方/群系/送别方式/英灵）
  public int required;                    // 需要数量
  public string currentProgress;          // 当前进度（运行时填充）
}

[Serializable]
public class QuestReward {
  public RewardType type;                 // OathMilestone / StatBlock / Resource / Item / GodEmber / HiddenRecipe
  public string targetId;                 // 目标 ID（誓言索引/资源/物品/配方）
  public float amount;                    // 数量
  public StatBlock statBlock;             // 永久 buff（StatBlock）
}
```

**MVP 范围**：10 主线 + 15 支线 + 18 远征（共享 world-exploration）+ 10 日常 = 53 任务
**v1.0 锁定决策**：任务不可放弃（承诺有重量）；主线顺序触发；日常 24h 周期
**用在哪**：任务日志（按 J）/ 完成 Toast / Oath 进度推进 / 世界事件联动

#### 类型 15: `VFXPresetSO`（VFX 预设）—— **vfx GDD 锁定**
```csharp
[CreateAssetMenu(menuName = "Ravensong/VFX Preset")]
public class VFXPresetSO : ScriptableObject, IDataValidatable {
  [Header("Identity")]
  public string id;                       // "vfx_weave_success" / "vfx_valhalla_send_off" / "vfx_biome_birch_atmosphere"
  public string displayName;              // 调试用
  public VFXType type;                    // Atmosphere / Ritual / Combat / Status / Weather / UI / Theme

  [Header("Visuals")]
  public ParticleSystem particles;        // 粒子系统引用
  public Material material;               // 油画风材质（锚定 style-bible）
  public float particleCount;             // 基础粒子数（LOD 起点）
  public Color4 primaryColor;             // 主色（navy/cyan/gold）
  public Color4 secondaryColor;           // 辅色（米色/灰）

  [Header("Animation")]
  public float fadeInDurationSec;         // 0.5s
  public float fadeOutDurationSec;        // 0.3s
  public float totalDurationSec;          // 0=持续 / 3=仪式 / 180=奥丁审判

  [Header("Performance")]
  public int priority;                    // 0=氛围 / 1=状态天气 / 2=战斗 / 3=Boss / 4=仪式
  public bool lodEnabled;                 // 是否启用 LOD（距离衰减）
}
```

**MVP 范围**：30 套 VFX（6 群系氛围 + 7 仪式 + 6 战斗 + 6 天气 + 1 奥丁 + 4 UI）
**v1.0 锁定决策**：60 FPS 目标 + 500 粒子上限 + LOD 3 档（20/50/100m）
**用在哪**：所有 14 个 GDD 的"叙事时刻"

#### 类型 16: `AudioPresetSO`（Audio 预设）—— **vfx-audio GDD 锁定**
```csharp
[CreateAssetMenu(menuName = "Ravensong/Audio Preset")]
public class AudioPresetSO : ScriptableObject, IDataValidatable {
  [Header("Identity")]
  public string id;                       // "audio_music_combat" / "audio_sfx_weave_success"
  public string displayName;              // 调试用
  public AudioType type;                  // Music / Ambient / SFX / UI

  [Header("Clip")]
  public AudioClip clip;                  // 音频片段
  public float baseVolume;                // 基础音量
  public float basePitch;                 // 基础音调
  public bool loop;                       // 循环

  [Header("Dynamic")]
  public bool dayNightAffected;           // 受 Day-Night 影响
  public bool combatBoost;                // 战斗时音量提升
  public bool ritualBoost;                // 仪式时音量提升
}
```

**MVP 范围**：~60 个 audio clip（6 配乐 + 6 群系环境音 + 6 天气 + ~30 SFX + ~10 UI 音）
**v1.0 锁定决策**：8 AudioSource 上限 + 4 通道音量 + 动态混音 + VFX 联动
**用在哪**：所有 14 个 GDD 的"声音层"

#### 类型 17: `BossDetailSO`（Boss 详细设计）—— **boss-design GDD 锁定**
```csharp
[CreateAssetMenu(menuName = "Ravensong/Boss Detail")]
public class BossDetailSO : ScriptableObject, IDataValidatable {
  [Header("Identity")]
  public string id;                       // "boss_bone_king" / "boss_abyss_lord" / "boss_frost_giant" / "boss_mythic_guardian"
  public string displayName;              // 调试用
  public BossSO baseData;                 // 引用 BossSO（已有类型 6）

  [Header("Stats")]
  public float baseHP;                    // 骨王 = 1000，其他 = baseHP * difficultyMultiplier
  public float difficultyMultiplier;      // 骨王 1.0 / 深渊 1.3 / 寒霜 1.6 / 神话 2.0
  public bool nightOnly;                  // 必在夜晚（前 3 个 = true；神话守卫者 = false）

  [Header("Phase 1 (100-60%)")]
  public BossMoveSO[] phase1Moves;        // 教学期招式
  public float phase1CinematicSec;        // 阶段转换演出

  [Header("Phase 2 (60-30%)")]
  public BossMoveSO[] phase2Moves;        // 强化期招式
  public float phase2CinematicSec;

  [Header("Phase 3 (30-0%)")]
  public BossMoveSO[] phase3Moves;        // 终局期招式
  public float phase3CinematicSec;

  [Header("Cinematic")]
  public float entryCinematicSec;         // 进入 boss 房演出（180s = 3min）
  public float deathCinematicSec;         // 死亡演出（5s）
  public string entryDialogueId;          // boss 介绍对话 ID
  public string deathDialogueId;          // boss 死亡对话 ID

  [Header("Loot & Reward")]
  public ItemStack[] lootTable;           // 死亡掉落
  public float moraleBonus;               // 战胜后聚落士气 +X%
  public string oathMilestoneId;          // 触发的誓言里程碑
}

[Serializable]
public class BossMoveSO {
  public string id;
  public string displayName;
  public float warningDurationSec;        // 0.5-1s
  public float executionDurationSec;
  public string visualWarning;             // 视觉预警（红光/蓝光/cyan 光圈）
  public string audioWarning;              // 声音预警
  public string testedSkill;               // 测试技能（"闪避"/"视野"/"完美闪避"等）
  public int phase;                        // 1 / 2 / 3
  public float damage;                     // 攻击伤害
  public float aoeRadius;                  // AOE 半径
}
```

**MVP 范围**：4 boss 资产 + ~16 招式资产（4 boss × 4 招式平均）
**v1.0 锁定决策**：3 阶段结构 + 0.8s 预警 + 1.0× 难度系数（骨王为基准）
**用在哪**：combat 3 阶段 boss / world-exploration 4 boss 房 / oath 3.4 + 5.2 / quest 主线 7 + 支线 12

#### 容器类型: `ItemStack`（物品堆叠）
```csharp
[Serializable]
public struct ItemStack {
  public string itemId;              // 引用 ItemSO
  public int quantity;
}
```

被 RecipeSO / Inventory / EinherjarSO / 各种奖励引用。

### C.3 Schema 版本管理

```csharp
public interface IDataValidatable {
  string SchemaVersion { get; }      // "1.0"
  ValidationResult Validate();       // 返回错误列表
}

public class ValidationResult {
  public bool isValid;
  public List<ValidationError> errors;  // 不阻塞但要报告
}
```

**版本升级流程**：
1. 数据有 `Migrate(oldVersion, newVersion)` 函数（在 `DataMigrations` 静态类里）
2. `DataRegistry` 加载时按 `oldVersion → newVersion` 链式迁移
3. 迁移失败则该资产**标红禁用**，console 报错

### C.4 验证规则

| 检查 | Edit-time | 启动时 | Runtime |
|---|---|---|---|
| ID 唯一性 | ✓ Inspector 标红 | ✓ 全局扫描 | ✓ 拒绝冲突 |
| ID 格式 (snake_case) | ✓ | ✓ | — |
| 引用 ID 存在 | ✓ 显示 "missing" | ✓ 全局扫描 | ✓ 立即报警 |
| 必填字段 | ✓ | ✓ | ✓ |
| 数值范围 | ✓ | ✓ | ✓ |
| 循环依赖 | — | ✓ | — |

### C.5 热重载（Hot-reload）

- Play mode 中编辑 ScriptableObject → **debounce 500ms**（决策 #3 锁定）→ `DataRegistry.NotifyChanged(id)` → 触发依赖系统重读
- **debounce 目的**：连续编辑时（如改数值时 Unity 自动保存多次）合并为单次广播，避免广播风暴卡 game thread
- 受影响系统需实现 `IDataObserver` 接口：
```csharp
public interface IDataObserver {
  void OnDataChanged(string dataId);
}
```
- 适用：配方调试、英灵数值微调、群系参数

### C.6 状态与转换（States and Transitions）

数据资产生命周期：

```
[Disk] → [Edit-time] → [Build] → [Runtime]
            ↓
      [Invalid] → (validation fails)
            ↓
      [Quarantined] → (不进入游戏，但保留供修复)
```

```
设计师保存 → DataRegistry 重读 → OnDataChanged 广播
                                          ↓
                              依赖系统重读或重渲染
```

### C.7 与其他系统的交互

| 系统 | 怎么用 Data Config |
|---|---|
| **Fate-Thread** | 读 `RecipeSO[]` 决定可织什么 |
| **Inventory** | 读 `ItemSO` 决定物品属性 |
| **Einherjar** | 读 `EinherjarSO[]` 决定居民；写 `EinherjarState`（运行时数据，不是 SO） |
| **Combat** | 读 `RecipeSO` 决定可用武器；读 `BossSO` 决定当前 boss |
| **Settlement** | 读 `BuildingSO`（Data Config 扩展）决定可建建筑 |
| **World Exploration** | 读 `BiomeSO[]` 决定可见群系 |
| **Oath** | 读 `OathSO[]` 决定可见誓言；写 `OathProgress`（运行时） |
| **Quest-Event** | 读 `WorldEventSO[]` 决定可能发生事件 |
| **Day-Night** | 读 `BiomeSO.ambientColor` 决定光照 |
| **UI/HUD** | 读所有 SO 渲染 UI |

---

## D. Formulas

### F.1 ID 验证公式

```
正则：^[a-z][a-z0-9_]*[a-z0-9]$
长度：3 - 64 字符
首字符：小写字母
末字符：小写字母或数字
中间：仅小写字母、数字、下划线
```

### F.2 配方价值计算（用于 AI 决策 / 交易估值）

```
recipeValue = sum(inputItem.value × inputQuantity) × (1 + tier × 0.5)
```
- 例：饥荒之弓 = (10 × 1) × (1 + 2 × 0.5) = 30 价值
- 用途：NPC 交易时估值、玩家对照"我值不值得织这个"

### F.3 配方解锁时间（玩家体验期望）

```
estimatedHoursToDiscover = 0.5 × tier^1.5
```
- tier 1 = 0.5h
- tier 3 = ~2.6h
- tier 5 = ~5.6h
- 用途：playtest 调参，确保每条誓言的体验节奏

### F.4 数据资产加载优先级

```
loadOrder = 0:        Items, Biomes (被 Recipe 引用)
loadOrder = 1:        Recipes, Bosses (引用 Items)
loadOrder = 2:        Einherjars, Oaths (引用 Items, Recipes)
loadOrder = 3:        WorldEvents (引用以上全部)
```
加载时按此顺序，未加载的引用返回 `null` + 警告。

### F.5 启动时数据校验时间预算

```
maxValidationTime = 200ms
```
超过则拆成延迟校验（先启动游戏，校验在后台跑）。

---

## E. Edge Cases

| 情况 | 怎么处理 |
|---|---|
| **重复 ID**（两个 SO 同 id） | 启动时报错，标红，全部禁用 |
| **缺失引用**（recipe 输入物品不存在） | 启动时报错，该 recipe 标"未实现" |
| **循环引用**（A 引用 B，B 引用 A） | 启动时检测到则两个都禁用 |
| **schema 版本太旧**（< 1.0） | 跑迁移脚本，失败则禁用 |
| **磁盘损坏 / 解析失败** | 该资产 quarantined，列出错误，不影响其他 |
| **设计师留空 ID** | Inspector 红框 + 不允许保存 |
| **运行时改 SO 引用** | 通过 OnDataChanged 广播，所有依赖重读 |
| **存档中的 ID 在新版数据中不存在** | 保存时记录 schema 兼容性，缺失则降级为"未识别物品" |
| **热重载时玩家正在用这个数据** | 标记"延迟更新"，等当前操作完成再切换 |

---

## F. Dependencies

### 上游（这个系统依赖谁）

**无** —— Data Config 是 L1 Foundation，不依赖任何其他 gameplay 系统。

### 下游（谁依赖这个系统）

**几乎所有系统**：
- Input / Save（间接）
- Day-Night, Gathering, Inventory, Combat, VFX-Audio（L2）
- Fate-Thread, Einherjar, Settlement, World Exploration, Death-Send-off（L3）
- Oath, Quest-Event（L4）
- UI/HUD, VFX, Camera（L5）

**Data Config 是 Ravensong 的"唯一真实源"**：其他系统的实现必须从 DataRegistry 拿数据，**不允许 hardcode**。

---

## G. Tuning Knobs

| 旋钮 | 默认值 | 范围 | 影响 |
|---|---|---|---|
| `validationStrictness` | 0.8 | 0-1 | 0 = 编辑时只警告；1 = 任何 error 都阻止保存 |
| `maxValidationTimeMs` | 200 | 50-1000 | 启动校验超时阈值 |
| `hotReloadEnabled` | true | bool | play mode 是否启用热重载 |
| `hotReloadDebounceMs` | 500 | 100-2000 | 热重载广播 debounce 时间（决策 #3）|
| `quarantineOnError` | true | bool | 损坏的资产是否隔离 vs 删除 |
| `autoMigrate` | true | bool | 是否自动跑迁移脚本 |
| `logDataAccess` | false | bool | 调试时打开，记录所有数据访问 |

**调整建议**：
- Development build：`validationStrictness=1.0`, `logDataAccess=true`
- Release build：`validationStrictness=0.8`, `logDataAccess=false`

---

## H. Acceptance Criteria

### AC-1: 设计师加新配方 < 5 分钟
**测试步骤**：
1. 打开 Unity
2. 右键 → Create → Ravensong → Recipe
3. 填 id / name / inputs / output
4. 拖入 play mode
5. 验证：Fate-Thread 系统的可用配方列表立即包含新配方
6. **耗时 < 5 分钟**

### AC-2: ID 冲突立即报错
**测试步骤**：
1. 复制一个 recipe，修改名字但保留 id
2. 进入 play mode
3. **期望**：Console 立即出现 "Duplicate ID: recipe_famine_bow" 错误
4. **期望**：两个 recipe 都被 quarantined

### AC-3: 缺失引用立即报警
**测试步骤**：
1. 创建一个 recipe，输入 itemId = "item_nonexistent"
2. 进入 play mode
3. **期望**：Console 出现 "Missing reference: item_nonexistent" 错误
4. **期望**：该 recipe 标"未实现"，玩家 UI 不显示

### AC-4: 热重载生效 < 1 秒
**测试步骤**：
1. play mode 中，编辑 recipe 的 `godEmberCost: 10 → 20`
2. 不重启 play mode
3. **期望**：1 秒内，Fate-Thread 系统下次织这个物品消耗 20 余烬

### AC-5: Schema 升级不破坏存档
**测试步骤**：
1. 在 v1.0 数据上做一次完整游戏进度
2. 把 schema 升到 v1.1（加一个新字段）
3. 写迁移脚本
4. 加载旧存档
5. **期望**：存档正常加载，新字段用默认值

### AC-6: 数据校验性能
**测试步骤**：
1. 准备 200 个 recipe + 50 个 einherjar + 20 个 biome + 5 个 oath
2. 进入 play mode
3. **期望**：启动校验总时间 < 200ms
4. **期望**：Console 无性能警告

### AC-7: 所有数据资产可被 Git 追踪
**测试步骤**：
1. 修改一个 SO，保存
2. `git status` 查看
3. **期望**：该 .asset 文件被检测到变更
4. **期望**：JSON diff 友好（不容易 merge conflict）

### AC-8: 校验错误信息可被设计师理解
**测试步骤**：
1. 故意创建一个无效 recipe
2. 查看 Inspector 红框错误
3. **期望**：错误信息包含：哪个字段、什么问题、怎么修
4. **反例**："Validation failed"（太模糊，失败）

---

## 10. 已锁定决策（Locked Decisions）

> 2026-07-27 用户拍板，5 个开放问题全部锁定。已落地为 C.2 类型 8/9 与 G 旋钮。
> **v1.1 升级（2026-07-27）**：由 day-night-cycle GDD 锁定驱动，新增 12 个 Day-Night / Enemy Alert 字段到 `GameConfigSO`。
> **v1.2 升级（2026-07-27）**：由 inventory GDD 锁定驱动，新增 10 个 Inventory 调参字段到 `GameConfigSO` + ItemSO 增 `dayBonus` / `nightBonus` / `DayNightItemBonus` 字段。
> **v1.3 升级（2026-07-27）**：由 gathering / input-system / save-system 3 个 GDD 锁定驱动，**新增 38 个字段到 `GameConfigSO` + 新增 `ToolSO` 数据类型**。
> **v1.4 升级（2026-07-27）**：由 combat GDD 锁定驱动，**新增 23 个 Combat 调参字段到 `GameConfigSO`**。
> **v1.5 升级（2026-07-27）**：由 fate-thread + einherjar 2 个 GDD 锁定驱动，**新增 31 个字段到 `GameConfigSO`（Fate-Thread 15 + Einherjar 16）**。
> **v1.6 升级（2026-07-27）**：由 oath-system GDD 锁定驱动，**新增 12 个字段到 `GameConfigSO`（Oath / 终局 / 奥丁审判）**。覆盖 5 誓言计数、苍穹解锁、3 min 奥丁独白、2 选 1 结局、25 里程碑等关键参数。
> **v1.6 schema 补全（2026-07-27 v1.7 锁定时）**：v1.6 LOCKED 时只更新了 §10 决策表，未把 12 个 Oath 字段加进 `GameConfigSO` schema。v1.7 锁定时已补：在 §C.2 类型 9 中加 `[Header("Oath (oath-system GDD 锁定 — v1.6 补 schema)")]` 块（12 字段）。
> **v1.7 升级（2026-07-27）**：由 death-sendoff GDD 锁定驱动，**新增 9 个字段到 `GameConfigSO`（死亡默认/送别/纪念碑/Wyrd 锚点）+ 新增 `SendoffSO`（类型 10）**。覆盖 24h 强留默认、送别不可跳、纪念碑 8 块上限、Wyrd 锚点 +10%、5-7 天自然腐化等关键参数。
> **v1.8 升级（2026-07-27）**：由 ui-hud GDD 锁定驱动，**新增 12 个字段到 `GameConfigSO`（UI 框架/HUD/弹窗/Toast/对话/罗盘）+ 新增 `UIStyleSO`（类型 11）**。覆盖 16ms 帧预算、0.5s 淡入、2s Toast、自动昼夜主题、教程启用、衬线字体、3s 对话等关键参数。
> **v1.9 升级（2026-07-27）**：由 settlement GDD 锁定驱动，**新增 12 个字段到 `GameConfigSO`（5 基础建筑等级 1-2/3 容量/士气/效率）+ 新增 `SettlementSO`（类型 12）**。覆盖长屋 4→8→12、工坊 Tier 3→4、篝火 +15%→+30%、仓库 24→48 槽、神龛 god-ember ×1.5、24h 升级 VFX 等关键参数。
> **v2.0 升级（2026-07-27）**：由 world-exploration GDD 锁定驱动，**新增 12 个字段到 `GameConfigSO`（6 群系/70 POI/隐藏配方/远征奖励概率/寒冷 debuff）+ 新增 `ExpeditionSO`（类型 13）**。覆盖 6 群系架构、70 POI 分布、20% 隐藏配方（6 个）、7 天远征、1% Legendary + 4% Rare、群系夜晚 +30%、30m 过渡带、-1 HP/分钟寒冷等关键参数。
> **v2.1 升级（2026-07-27）**：由 quest-event GDD 锁定驱动，**新增 12 个字段到 `GameConfigSO`（4 类任务数/24h 周期/3 世界事件/任务不可放弃/主线顺序）+ 新增 `QuestSO`（类型 14）**。覆盖 10 主线（=5 誓言×2）、15 支线、10 日常、24h 刷新、3 世界事件同时存在、12h 触发间隔、1 周目 ≥5 事件、任务不可放弃、主线顺序触发、30 任务日志上限、4s Toast、×1.0 奖励乘数等关键参数。
> **v2.2 升级（2026-07-27）**：由 vfx GDD 锁定驱动，**新增 11 个字段到 `GameConfigSO`（粒子/LOD/淡入淡出/天气/满月神显）+ 新增 `VFXPresetSO`（类型 15）**。覆盖 500 粒子上限、60 FPS 目标、0.5s 淡入 + 0.3s 淡出、LOD 3 档（20/50/100m）、0.5× 减少模式、6h 天气变化、满月 Tier 5 +50%、奥丁审判 200 粒子等关键参数。
> **v2.3 升级（2026-07-27）**：由 vfx-audio GDD 锁定驱动，**新增 12 个字段到 `GameConfigSO`（AudioSource 上限/4 通道音量/动态混音/昼夜倍率）+ 新增 `AudioPresetSO`（类型 16）**。覆盖 8 AudioSource 上限、主音量 0.8、音乐/环境/SFX/UI = 0.5/0.7/0.8/0.6、战斗 ×1.5、仪式 ×1.2、1s 混音淡变、2s 跨场景切换、1.0 奥丁审判音量等关键参数。
> **v2.4 升级（2026-07-27）**：由 camera GDD 锁定驱动，**新增 12 个字段到 `GameConfigSO`（5 模式相机：Follow/Combat/Ritual/Transition/Death）**。Camera 行为简单，**不**新增 SO（参数都在 GameConfigSO）。覆盖 5 跟随速度、1.2 战斗偏移、3 战斗速度、0.8 仪式缩放（拉远）、2 仪式速度、1.2 过渡缩放（拉近）、1s 过渡时长、0.5s 死亡淡黑、0.7 死亡缩放、-10 Z 偏移、v1.1 缩放范围 0.5-2.0 等关键参数。
> **v2.5 升级（2026-07-27，⭐ 修复 P0 缺口）**：由 boss-design GDD 锁定驱动，**新增 12 个字段到 `GameConfigSO`（3 阶段 boss 框架/4 boss 难度阶梯/招式预警/治疗限制）+ 新增 `BossDetailSO`（类型 17）**。覆盖 3 阶段结构、1000 基础血、0.8s 招式预警、1s 阶段转换、24h 失败冷却、5s 死亡演出、180s 房演出、1/min 治疗上限、30m 神龛范围、夜晚必触发、+5% 士气加成、1.0× 难度系数等关键参数。**修复 P0 缺口：4 boss（骨王 1.0× / 深渊之主 1.3× / 寒霜巨人 1.6× / 神话守卫者 2.0×）全部有详细阶段设计**。

| # | 决策点 | 锁定值 | 落地位置 |
|---|---|---|---|
| 1 | **保存格式** | **全 `.asset`**（Unity native），不导出 JSON 副本 | A. Overview + C.1 规则 1 |
| 2 | **英灵上限等全局配置** | 集中在**一个** `GameConfigSO`（不在 8 个核心里分散） | C.2 类型 9 + G 旋钮 |
| 3 | **热重载 debounce** | **500ms**（避免连续编辑风暴） | C.5 + G 旋钮 `hotReloadDebounceMs` |
| 4 | **加载策略** | **MVP 同步，EA 转异步**（`GameConfigSO.loadAsync = false → true`） | C.2 类型 9 + 实施备注 |
| 5 | **数据驱动对话** | **MVP 就定义 `DialogueSO`**（线性展示，为未来分支留接口） | C.2 类型 8 |

### v1.1 新增字段（由 day-night-cycle GDD 驱动）

| 字段 | 默认值 | 来源 |
|---|---|---|
| `realSecondsPerGameHour` | 30f | day-night-cycle §C.1 规则 1 |
| `dayCombatDamageFactor` | 0.75f | day-night-cycle §C.2 |
| `nightCombatDamageFactor` | 1.2f | day-night-cycle §C.3 |
| `dawnDurationHours` | 1f | day-night-cycle §C.4 |
| `duskDurationHours` | 1f | day-night-cycle §C.4 |
| `transitionVisualDurationSec` | 60f | day-night-cycle §C.4 |
| `odinEyeScanChance` | 0.3f | day-night-cycle §C.5 决策 #2 |
| `odinEyeScanIntervalHours` | 0.5f | day-night-cycle §C.5 决策 #2 |
| `moonPhaseCycleDays` | 8f | day-night-cycle §C.6 决策 #3 |
| `showEnemyAlertRange` | true | day-night-cycle §C.10 决策 #5 |
| `enemyAlertRangeBase` | 5f | day-night-cycle §C.10 |
| `enemyAlertRangeDayMultiplier` | 1.3f | day-night-cycle §C.10 |

**v1.1 总字段数：12**（原 8 个 v1.0 字段保留，全部在 C.2 类型 9 schema 中）

### v1.6 新增字段（由 oath-system GDD 驱动）

| 字段 | 默认值 | 来源 |
|---|---|---|
| `oathMilestonesPerOath` | 5 | oath-system §C.1 规则 2（决策 #11）|
| `oathCount` | 5 | oath-system §C.1 规则 1（决策 #1）|
| `oathRequiredForSkyOath` | 4 | oath-system §C.6（决策 #7）|
| `oathCompleteAnimationSec` | 5f | oath-system §C.4（决策 #10）|
| `oathMilestoneProgressBar` | true | oath-system §C.8 UI |
| `oathAutoApplyBuff` | true | oath-system §C.5（决策 #3）|
| `odinTrialDurationSec` | 180f | oath-system §C.7（决策 #5）|
| `odinTrialMinSecondsBeforeChoice` | 10f | oath-system §C.7（决策 #6）|
| `endgameResetAfterChoice` | false | oath-system §C.7（结局 1/2 控制）|
| `newGamePlusEnabled` | false | oath-system §C.7（决策 #4）|
| `endgameCreditsRollSec` | 90f | oath-system §C.7 演职员表 |
| `skyOathUnlockVisualDuration` | 8f | oath-system §C.6 解锁仪式 |

**v1.6 总字段数：12**（与 oath-system GDD §G 旋钮一一对应，全部在 C.2 类型 9 schema 中）

### v1.7 新增字段（由 death-sendoff GDD 驱动）

| 字段 | 默认值 | 来源 | 状态 |
|---|---|---|---|
| `dyingDefaultChoice` | Refuse | death-sendoff §C.1 规则 1（决策 #1）| **新加** |
| `sendOffCinematicSkipEnabled` | false | death-sendoff §C.7（决策 #2）| **新加** |
| `battleSendoffGodEmberCost` | 0 | death-sendoff §C.4 方式 2（决策 #3）| **新加** |
| `memorialMoodBonusPerMemorial` | 0.01f | death-sendoff §C.6（决策 #4）| **新加** |
| `memorialMaxCount` | 8 | death-sendoff §C.6（决策 #4）| **新加** |
| `wyrdAnchorEfficiencyBonus` | 0.1f | death-sendoff §C.1 规则 7（决策 #6）| **新加** |
| `wyrdAnchorStopsDecay` | true | death-sendoff §C.1 规则 7（决策 #7）| **新加** |
| `letRestDurationDays` | 6f | death-sendoff §C.4 方式 5（决策 #8）| **新加** |
| `deathOathMilestoneProgressBar` | true | death-sendoff §C.5（决策 #9）| **新加** |

**v1.7 总字段数：9 新加 + 3 已在（dyingDecisionWindowHours / mourningPeriodHours / mourningEfficiencyMult 在 Einherjar 块）**。
**v1.7 新增类型：`SendoffSO`（类型 10）**——5 种送别方式 SO 资产（Valhalla / Battle / Burial / Refuse / LetRest）

### v1.8 新增字段（由 ui-hud GDD 驱动）

| 字段 | 默认值 | 来源 |
|---|---|---|
| `uiFrameBudgetMs` | 16f | ui-hud §G 决策 #1（60 FPS 预算）|
| `uiFadeInSec` | 0.5f | ui-hud §G 决策 #2（淡入）|
| `uiFadeOutSec` | 0.3f | ui-hud §G 决策 #2（淡出）|
| `uiThemeAutoDayNight` | true | ui-hud §C.10（决策 #7）|
| `uiSoundEnabled` | true | ui-hud §C.1 规则 6（决策 #11）|
| `uiToastDefaultSec` | 2f | ui-hud §C.5（决策 #6）|
| `uiToastMaxStack` | 3 | ui-hud §C.5（堆叠上限）|
| `uiTutorialEnabled` | true | ui-hud §C.9（决策 #5）|
| `uiDialogueAutoPlaySec` | 3f | ui-hud §C.6（自动播放）|
| `uiModalPauseGame` | true | ui-hud §C.3（决策 #3）|
| `uiHealthBarLength` | 200f | ui-hud §C.2（血量条）|
| `uiCompassShowDistanceMax` | 200f | ui-hud §C.8（罗盘距离）|

**v1.8 总字段数：12**（与 ui-hud GDD §G 旋钮一一对应，全部在 C.2 类型 9 schema 中）
**v1.8 新增类型：`UIStyleSO`（类型 11）**——2 套主题（白天暖 / 夜晚冷）+ 1 套衬线字体 + 1 套 9-slice 边框

### v1.9 新增字段（由 settlement GDD 驱动）

| 字段 | 默认值 | 来源 |
|---|---|---|
| `longhouseCapacityLevel1` | 4 | settlement §C.2 建筑 1（决策 #1）|
| `longhouseCapacityLevel2` | 8 | settlement §C.2 建筑 1（决策 #2，=v1.0 上限）|
| `longhouseCapacityLevel3` | 12 | settlement §C.2 建筑 1（v1.1 决策）|
| `forgeMaxTierLevel1` | 3 | settlement §C.2 建筑 2（决策 #3）|
| `forgeMaxTierLevel2` | 4 | settlement §C.2 建筑 2（决策 #4）|
| `hearthMoraleBonusLevel1` | 0.15f | settlement §C.2 建筑 3（决策 #5）|
| `hearthMoraleBonusLevel2` | 0.30f | settlement §C.2 建筑 3（决策 #6）|
| `storageSlotsLevel1` | 24 | settlement §C.2 建筑 4（决策 #7）|
| `storageSlotsLevel2` | 48 | settlement §C.2 建筑 4（决策 #8）|
| `shrineGodEmberMultiplierLevel2` | 1.5f | settlement §C.2 建筑 5（决策 #9）|
| `shrineMourningAccelerateCost` | 20 | settlement §C.7（决策 #10）|
| `buildingUpgradeVfxHours` | 24f | settlement §C.5（决策 #11）|

**v1.9 总字段数：12**（与 settlement GDD §G 旋钮一一对应，全部在 C.2 类型 9 schema 中）
**v1.9 新增类型：`SettlementSO`（类型 12）**——5 基础建筑 + 0-5 誓言建筑 + 4 资源池 + 衰悼期倒计时

### v2.0 新增字段（由 world-exploration GDD 驱动）

| 字段 | 默认值 | 来源 |
|---|---|---|
| `biomeCount` | 6 | world-exploration §C.1 规则 1（决策 #1）|
| `poiTotalCount` | 70 | world-exploration §C.3（决策 #2）|
| `hiddenRecipePercentage` | 0.20f | world-exploration §C.5（决策 #3）|
| `hiddenRecipeCount` | 6 | world-exploration §C.5（决策 #3）|
| `expeditionMaxDurationDays` | 7 | world-exploration §C.4（决策 #4）|
| `expeditionRewardLegendaryChance` | 0.01f | world-exploration §C.4（决策 #5）|
| `expeditionRewardRareChance` | 0.04f | world-exploration §C.4（决策 #6）|
| `biomeNightDifficultyBonus` | 1.3f | world-exploration §C.1 规则 3（决策 #7）|
| `biomeTransitionMeters` | 30f | world-exploration §C.6（决策 #8）|
| `expeditionType` | Player | world-exploration §C.4（决策 #9）|
| `compassMaxDistance` | 200f | world-exploration §C.3（决策 #10）|
| `frostCliffColdDamagePerMin` | 1 | world-exploration §C.2 群系 4（决策 #11）|

**v2.0 总字段数：12**（与 world-exploration GDD §G 旋钮一一对应，全部在 C.2 类型 9 schema 中）
**v2.0 新增类型：`ExpeditionSO`（类型 13）**——远征任务定义（v1.0 玩家亲自 / v1.1 远征兵）

### v2.1 新增字段（由 quest-event GDD 驱动）

| 字段 | 默认值 | 来源 |
|---|---|---|
| `mainQuestCount` | 10 | quest-event §C.2 类型 1（决策 #1）|
| `sideQuestCount` | 15 | quest-event §C.2 类型 2（决策 #2）|
| `dailyQuestCount` | 10 | quest-event §C.2 类型 4（决策 #3）|
| `dailyQuestRefreshHours` | 24f | quest-event §C.2 类型 4（决策 #4）|
| `worldEventMaxActive` | 3 | quest-event §C.3（决策 #5）|
| `worldEventMinIntervalHours` | 12f | quest-event §C.3（决策 #6）|
| `worldEventMinTriggerPerPlaythrough` | 5 | quest-event §C.3（决策 #7）|
| `questAbandonAllowed` | false | quest-event §C.7（决策 #8）|
| `mainQuestSequential` | true | quest-event §C.4（决策 #9）|
| `questLogMaxActive` | 30 | quest-event §C.6（决策 #10）|
| `questCompleteToastSec` | 4f | quest-event §C.6（决策 #11）|
| `questRewardMultiplier` | 1.0f | quest-event §C.5（决策 #12）|

**v2.1 总字段数：12**（与 quest-event GDD §G 旋钮一一对应，全部在 C.2 类型 9 schema 中）
**v2.1 新增类型：`QuestSO`（类型 14）**——任务定义（Main/Side/Expedition/Daily 4 类），含 6 种条件类型（Kill/Gather/Craft/Explore/SendOff/Recruit）+ 6 种奖励类型（Oath 进度/永久 buff/资源/物品/god-ember/隐藏配方）

### v2.2 新增字段（由 vfx GDD 驱动）

| 字段 | 默认值 | 来源 |
|---|---|---|
| `vfxMaxParticlesPerFrame` | 500 | vfx §C.1 规则 1（决策 #1）|
| `vfxFpsTarget` | 60 | vfx §C.1 规则 4（决策 #2）|
| `vfxFadeInSec` | 0.5f | vfx §C.1 规则 3（决策 #3）|
| `vfxFadeOutSec` | 0.3f | vfx §C.1 规则 3（决策 #4）|
| `vfxLodNearMeters` | 20f | vfx §C.1 规则 6（决策 #5）|
| `vfxLodMidMeters` | 50f | vfx §C.1 规则 6（决策 #6）|
| `vfxLodFarMeters` | 100f | vfx §C.1 规则 6（决策 #7）|
| `vfxReducedMultiplier` | 0.5f | vfx §C.1 规则 8（决策 #8）|
| `weatherChangeIntervalHours` | 6f | vfx §C.6（决策 #9）|
| `fullMoonGodsightChance` | 0.5f | vfx §C.6（决策 #10）|
| `vfxOdinTrialParticles` | 200 | vfx §C.3 仪式 6（决策 #12）|

**v2.2 总字段数：11**（v1.0 决策 #11 vfx 调试模式不实现 v1.1 决策；与 vfx GDD §G 11 字段对应，全部在 C.2 类型 9 schema 中）
**v2.2 新增类型：`VFXPresetSO`（类型 15）**——VFX 预设（7 类：Atmosphere/Ritual/Combat/Status/Weather/UI/Theme），30 套 MVP 资产（6 群系 + 7 仪式 + 6 战斗 + 6 天气 + 1 奥丁 + 4 UI）

### v2.3 新增字段（由 vfx-audio GDD 驱动）

| 字段 | 默认值 | 来源 |
|---|---|---|
| `audioMaxSources` | 8 | vfx-audio §C.1 规则 1（决策 #1）|
| `audioMasterVolume` | 0.8f | vfx-audio §C.7（决策 #2）|
| `audioMusicVolume` | 0.5f | vfx-audio §C.7（决策 #3）|
| `audioAmbientVolume` | 0.7f | vfx-audio §C.7（决策 #3）|
| `audioSfxVolume` | 0.8f | vfx-audio §C.7（决策 #3）|
| `audioUiVolume` | 0.6f | vfx-audio §C.7（决策 #3）|
| `audioDayNightMult` | 1.0f | vfx-audio §C.3（决策 #4）|
| `audioCombatMusicMult` | 1.5f | vfx-audio §C.6（决策 #5）|
| `audioRitualMusicMult` | 1.2f | vfx-audio §C.6（决策 #6）|
| `audioFadeSec` | 1.0f | vfx-audio §C.6（决策 #7）|
| `audioMusicFadeSec` | 2.0f | vfx-audio §C.2（决策 #8）|
| `audioOdinTrialVolume` | 1.0f | vfx-audio §C.2（决策 #9）|

**v2.3 总字段数：12**（与 vfx-audio GDD §G 旋钮一一对应，全部在 C.2 类型 9 schema 中）
**v2.3 新增类型：`AudioPresetSO`（类型 16）**——Audio 预设（4 类：Music/Ambient/SFX/UI），~60 个 audio clip MVP 资产

### v2.4 新增字段（由 camera GDD 驱动）

| 字段 | 默认值 | 来源 |
|---|---|---|
| `cameraFollowLerpSpeed` | 5f | camera §C.2 模式 1（决策 #1）|
| `cameraCombatOffsetMultiplier` | 1.2f | camera §C.2 模式 2（决策 #2）|
| `cameraCombatLerpSpeed` | 3f | camera §C.2 模式 2（决策 #3）|
| `cameraRitualZoom` | 0.8f | camera §C.2 模式 3（决策 #4）|
| `cameraRitualLerpSpeed` | 2f | camera §C.2 模式 3（决策 #5）|
| `cameraTransitionZoom` | 1.2f | camera §C.2 模式 4（决策 #6）|
| `cameraTransitionDurationSec` | 1f | camera §C.2 模式 4（决策 #7）|
| `cameraDeathFadeSec` | 0.5f | camera §C.2 模式 5（决策 #8）|
| `cameraDeathZoom` | 0.7f | camera §C.2 模式 5（决策 #9）|
| `cameraFollowOffsetZ` | -10f | camera §C.2 模式 1（决策 #10）|
| `cameraMaxZoom` | 2.0f | camera §C.7（决策 #11，v1.1 玩家控制）|
| `cameraMinZoom` | 0.5f | camera §C.7（决策 #12，v1.1 玩家控制）|

**v2.4 总字段数：12**（与 camera GDD §G 旋钮一一对应，全部在 C.2 类型 9 schema 中）
**v2.4 不新增 SO**——Camera 行为简单，5 模式参数都在 `GameConfigSO` 中

### v2.5 新增字段（由 boss-design GDD 驱动，⭐ 修复 P0 缺口）

| 字段 | 默认值 | 来源 |
|---|---|---|
| `bossPhaseCount` | 3 | boss-design §C.1 规则 1（决策 #1）|
| `bossBaseHP` | 1000f | boss-design §C.2（决策 #2）|
| `bossMoveWarningSec` | 0.8f | boss-design §C.1 规则 4（决策 #3）|
| `bossPhaseTransitionSec` | 1.0f | boss-design §C.1（决策 #4）|
| `bossCooldownHours` | 24f | boss-design §C.1 规则 5（决策 #5）|
| `bossDeathCinematicSec` | 5f | boss-design §C.1 规则 7（决策 #6）|
| `bossEntryCinematicSec` | 180f | boss-design §C.7（决策 #7）|
| `bossGodEmberHealingLimit` | 1 | boss-design §C.6（决策 #8）|
| `bossHealingShrineRange` | 30f | boss-design §C.6（决策 #9）|
| `bossNightOnly` | true | boss-design §C.1 规则 2（决策 #10）|
| `bossMoraleBonusOnKill` | 0.05f | boss-design §C.8（决策 #11）|
| `bossDifficultyMultiplierBase` | 1.0f | boss-design §C.2（决策 #12）|

**v2.5 总字段数：12**（与 boss-design GDD §G 旋钮一一对应，全部在 C.2 类型 9 schema 中）
**v2.5 新增类型：`BossDetailSO`（类型 17）**——4 boss 详细阶段设计（骨王 / 深渊之主 / 寒霜巨人 / 神话守卫者），每 boss ~4 招式（3 阶段），覆盖 3 阶段结构、0.8s 预警、失败归因、士气加成

**⭐ 修复 P0 缺口**：4 boss 不再是'血牛'，每个 boss 测试不同战斗技能（记忆型/适应型/耐久型/综合型），严格遵循 Boss 框架（Skill exam / Move readability / Phase structure / Spectacle vs clarity / True reason for difficulty）

### 决策之间的协同

- **#1 + #5**：保存格式选 `.asset` 让 `DialogueSO` 用纯 SO 实现即可，**不用为对话单独搞 JSON 引擎**。
- **#2 + 之前的 Day-Night 锁定决策**：`GameConfigSO` 是**所有锁定决策的数值执行点**——调整它即可整体调参，不用碰任何 GDD。这是 §10 锁定决策的"可调性保险"。
- **#3 + #4**：debounce 500ms + 同步加载（MVP），性能预算 200ms 完全够用，等数据量到 200+ 再考虑异步 + 调整 debounce。
- **v1.1 升级原因**：day-night-cycle GDD 锁定决策 #4 要求"GameConfigSO 字段同步"——这是**GDD 之间通过 `GameConfigSO` 形成依赖链**的第一个具体案例。后面 Combat GDD 也会加新字段，data-config 会继续升 v1.2, v1.3...

### 仍待 playtest 调参（不阻塞 GDD 锁定）

| 待调项 | 候选范围 | 调参位置 |
|---|---|---|
| Day-Night 数值精度 | -50% vs -45% vs -55% | `GameConfigSO.daySpeedFactor` |
| 神力余烬基础速率 | 1/小时 vs 0.8/小时 | `GameConfigSO.godEmberPassivePerHour` |
| 永久 buff 的具体数值 | 产出+15% vs +20% | `EinherjarSO.valhallaBuff` |
| 尸鬼腐化时间 | 3 天 vs 4 天 vs 5 天 | `GameConfigSO.corpseDecayMinDays/MaxDays` |
| 奥丁之眼扫描频率 | 30% / 0.5h vs 25% / 0.5h | `GameConfigSO.odinEyeScanChance` |
| 月相 8 天 vs 12 天 | 后续 playtest 决定 | `GameConfigSO.moonPhaseCycleDays` |

→ 这些都是 Prototype 阶段的**数值调参工作**，通过 `GameConfigSO` / `EinherjarSO` 直接改即可，不阻塞任何 GDD。

---

## Unity Implementation Notes

### 目录结构
```
Assets/Data/
├── Items/              # ItemSO 资产
├── Recipes/            # RecipeSO 资产
├── Einherjars/         # EinherjarSO 资产
├── Biomes/             # BiomeSO 资产
├── Oaths/              # OathSO 资产
├── Bosses/             # BossSO 资产
├── WorldEvents/        # WorldEventSO 资产
├── GameConfig.asset    # 全局配置（上限、默认值）
└── DataRegistry.cs     # 单例运行时索引
```

### 核心脚本（位于 `Assets/Scripts/Data/`）
- `DataRegistry.cs` - 单例，加载 + 索引 + 广播
- `IDataValidatable.cs` - 验证接口
- `IDataObserver.cs` - 观察者接口
- `DataMigrations.cs` - 版本迁移
- `DataInspector/` - 自定义 Inspector（Edit-time 验证 UI）

### 性能预算
- 启动加载：< 50ms
- 单次访问：< 0.1ms（O(1) 字典查找）
- 热重载广播：< 5ms / 资产

---

## 进度

| 章节 | 状态 |
|---|---|
| A. Overview | ✅ |
| B. Designer Experience | ✅ |
| C. Detailed Design | ✅ |
| D. Formulas | ✅ |
| E. Edge Cases | ✅ |
| F. Dependencies | ✅ |
| G. Tuning Knobs | ✅ |
| H. Acceptance Criteria | ✅ |
| Locked Decisions (§10) | ✅ |
| Unity Implementation Notes | ✅ |

**总进度**: 10/10 (100%) 🔒

**🔒 已锁定 v2.5 ⭐ 全部 GDD 完成 + P0 修复** — 8 段全填 + 5 开放问题已落地 + **18 个 GDD 全部锁定**（data-config / day-night-cycle / inventory / gathering / input-system / save-system / combat / fate-thread / einherjar / oath-system / death-sendoff / ui-hud / settlement / world-exploration / quest-event / vfx / vfx-audio / camera / **boss-design**）通过 GameConfigSO / ItemSO / `SendoffSO` / `UIStyleSO` / `SettlementSO` / `ExpeditionSO` / `QuestSO` / `VFXPresetSO` / `AudioPresetSO` / **`BossDetailSO`** 持续进化。**⭐ 修复 P0 缺口**：4 boss 详细阶段设计已落地。

---

## 变更日志

| 日期 | 版本 | 改动 | 责任人 |
|---|---|---|---|
| 2026-07-27 | v1.0 draft | 初版生成：8 段 + Unity 实施备注 | Mavis |
| 2026-07-27 | v1.0 LOCKED | 5 个开放问题用户拍板全部锁定；新增 `DialogueSO`（类型 8）+ `GameConfigSO`（类型 9）；C.5/G 旋钮更新 | Mavis + 用户 |
| 2026-07-27 | v1.1 LOCKED | 由 day-night-cycle GDD 锁定驱动；`GameConfigSO` 新增 12 字段（Day-Night + Enemy Alert）；§10 锁定决策增加 v1.1 升级说明 | Mavis + 用户 |
| 2026-07-27 | v1.2 LOCKED | 由 inventory GDD 锁定驱动；`GameConfigSO` 新增 10 字段（Inventory 调参）；`ItemSO` 增 `dayBonus` / `nightBonus` / `DayNightItemBonus`（Ravensong 特色机制）；§10 增加 v1.2 升级说明 | Mavis + 用户 |
| 2026-07-27 | **v1.3 LOCKED** | 由 gathering / input-system / save-system 3 个 GDD 锁定驱动；`GameConfigSO` 新增 38 字段（Gathering 17 + Input 9 + Save 12）；新增 `ToolSO` 扩展 `ItemSO`；§10 增加 v1.3 升级说明 | Mavis + 用户 |
| 2026-07-27 | **v1.4 LOCKED** | 由 combat GDD 锁定驱动；`GameConfigSO` 新增 23 字段（Combat 调参）；§10 增加 v1.4 升级说明 | Mavis + 用户 |
| 2026-07-27 | **v1.5 LOCKED** | 由 fate-thread + einherjar 2 个 GDD 锁定驱动；`GameConfigSO` 新增 31 字段（Fate-Thread 15 + Einherjar 16）；§10 增加 v1.5 升级说明 | Mavis + 用户 |
| 2026-07-27 | **v1.6 LOCKED** | 由 oath-system GDD 锁定驱动；`GameConfigSO` 新增 12 字段（Oath / 苍穹 / 奥丁审判 / 2 结局）；§10 增加 v1.6 升级说明 + v1.6 字段表 | Mavis + 用户 |
| 2026-07-27 | **v1.7 LOCKED** | 由 death-sendoff GDD 锁定驱动；**新增 `SendoffSO`（类型 10）** + `GameConfigSO` 新增 9 字段（死亡默认/送别/纪念碑/Wyrd 锚点）；同步修补 v1.6 漏加的 12 个 Oath 字段到 `GameConfigSO` schema；§10 增加 v1.6 schema 补全说明 + v1.7 升级说明 + v1.7 字段表 | Mavis + 用户 |
| 2026-07-27 | **v1.8 LOCKED** | 由 ui-hud GDD 锁定驱动；**新增 `UIStyleSO`（类型 11）** + `GameConfigSO` 新增 12 字段（UI 框架/HUD/弹窗/Toast/对话/罗盘）；§10 增加 v1.8 升级说明 + v1.8 字段表 | Mavis + 用户 |
| 2026-07-27 | **v1.9 LOCKED** | 由 settlement GDD 锁定驱动；**新增 `SettlementSO`（类型 12）** + `GameConfigSO` 新增 12 字段（5 基础建筑容量/士气/效率/升级 VFX）；§10 增加 v1.9 升级说明 + v1.9 字段表 | Mavis + 用户 |
| 2026-07-27 | **v2.0 LOCKED** | 由 world-exploration GDD 锁定驱动；**新增 `ExpeditionSO`（类型 13）** + `GameConfigSO` 新增 12 字段（6 群系/70 POI/隐藏配方/远征奖励/寒冷 debuff）；§10 增加 v2.0 升级说明 + v2.0 字段表 | Mavis + 用户 |
| 2026-07-27 | **v2.1 LOCKED** | 由 quest-event GDD 锁定驱动；**新增 `QuestSO`（类型 14）** + `GameConfigSO` 新增 12 字段（4 类任务数/24h 周期/3 世界事件/任务不可放弃/主线顺序）；§10 增加 v2.1 升级说明 + v2.1 字段表 | Mavis + 用户 |
| 2026-07-27 | **v2.2 LOCKED** | 由 vfx GDD 锁定驱动；**新增 `VFXPresetSO`（类型 15）** + `GameConfigSO` 新增 11 字段（粒子/LOD/淡入淡出/天气/满月神显）；§10 增加 v2.2 升级说明 + v2.2 字段表 | Mavis + 用户 |
| 2026-07-27 | **v2.3 LOCKED** | 由 vfx-audio GDD 锁定驱动；**新增 `AudioPresetSO`（类型 16）** + `GameConfigSO` 新增 12 字段（AudioSource 上限/4 通道音量/动态混音/昼夜倍率）；§10 增加 v2.3 升级说明 + v2.3 字段表 | Mavis + 用户 |
| 2026-07-27 | **v2.4 LOCKED ⭐ 全部 GDD 完成** | 由 camera GDD 锁定驱动；`GameConfigSO` 新增 12 字段（5 模式相机：Follow/Combat/Ritual/Transition/Death）；**不**新增 SO（Camera 行为简单）；§10 增加 v2.4 升级说明 + v2.4 字段表；**18/18 GDD 全部锁定** | Mavis + 用户 |
| 2026-07-27 | **v2.5 LOCKED ⭐ 修复 P0 缺口** | 由 boss-design GDD 锁定驱动（修复 8 框架审查发现的 P0 缺口）；**新增 `BossDetailSO`（类型 17）** + `GameConfigSO` 新增 12 字段（3 阶段 boss / 4 boss 难度阶梯 1.0/1.3/1.6/2.0× / 0.8s 招式预警 / 24h 失败冷却 / 3min 房演出 / 5s 死亡演出）；§10 增加 v2.5 升级说明 + v2.5 字段表；**18/18 GDD 全部锁定** | Mavis + 用户 |
