using System.Collections.Generic;
using UnityEngine;

namespace Ravensong.Data
{
    /// <summary>
    /// 全局配置（data-config §C.2 类型 9 + handover §9 锁定：所有数值决策的执行点）。
    /// 调参改这里即可，不在 8 个核心 SO 里分散。
    /// SchemaVersion 2.5（data-config v2.5）。
    /// </summary>
    [CreateAssetMenu(menuName = "Ravensong/GameConfig", fileName = "GameConfig")]
    public class GameConfigSO : ScriptableObject, IDataValidatable
    {
        public string SchemaVersion => "2.5";

        // ============================================================
        // Day-Night Cycle（day-night-cycle.md + game-concept §3.2）
        // ============================================================
        [Header("=== Day-Night Cycle ===")]

        [Tooltip("6 段时长占比（黎明/日/暮/夜/午夜/深宵）")]
        [Range(0f, 1f)] public float dayDawnRatio = 0.10f;
        [Range(0f, 1f)] public float dayRatio = 0.30f;
        [Range(0f, 1f)] public float dayDuskRatio = 0.10f;
        [Range(0f, 1f)] public float nightRatio = 0.25f;
        [Range(0f, 1f)] public float midnightRatio = 0.15f;
        [Range(0f, 1f)] public float deepNightRatio = 0.10f;

        [Tooltip("完整一天游戏时间（秒，60 = 1 游戏分钟 = 1 现实秒）")]
        [Range(60f, 600f)] public float dayLengthSeconds = 360f;

        [Tooltip("白天 debuff（game-concept §3.2 锁定）")]
        [Range(0f, 1f)] public float dayVisionPenalty = 0.30f;
        [Range(0f, 1f)] public float dayMoveSpeedPenalty = 0.20f;
        [Range(0f, 1f)] public float dayWeavePenalty = 0.30f;

        [Tooltip("夜晚 buff（game-concept §3.2 锁定）")]
        [Range(0f, 1f)] public float nightMoveSpeedBonus = 0.40f;
        [Range(0f, 1f)] public float nightWeaveTimeReduction = 0.50f;
        [Range(0f, 1f)] public float nightGodEmberRegenPerHour = 1f;

        [Tooltip("Waxing Moon 月相（8 段新月→满月）")]
        public float[] moonPhaseDurations = { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };  // 8 段

        // ============================================================
        // Fate-Thread / Weaving（fate-thread.md）
        // ============================================================
        [Header("=== Weaving ===")]

        [Tooltip("编织配方 80-120（game-concept §11.1）")]
        [Range(20, 200)] public int totalRecipes = 80;
        [Range(0, 30)] public int hiddenRecipes = 6;

        [Tooltip("成功率（data-config §C.2 systems-index §3.2 锁定）")]
        [Range(0f, 1f)] public float tier1SuccessRate = 1.00f;
        [Range(0f, 1f)] public float tier2SuccessRate = 0.97f;
        [Range(0f, 1f)] public float tier3SuccessRate = 0.93f;
        [Range(0f, 1f)] public float tier4SuccessRate = 0.90f;
        [Range(0f, 1f)] public float tier5SuccessRate = 0.85f;

        [Tooltip("god-ember 返还（systems-index §3.6 锁定：30%）")]
        [Range(0f, 1f)] public float godEmberRefundRate = 0.30f;

        // ============================================================
        // Economy（systems-index §3.2 + data-config §C.2）
        // ============================================================
        [Header("=== Economy ===")]

        [Tooltip("资源池（v1.0 无上限，v1.1 = 1000）")]
        [Range(0, 10000)] public int ironPoolMax = 0;       // 0 = 无上限
        [Range(0, 10000)] public int foodPoolMax = 0;
        [Range(0, 10000)] public int woodPoolMax = 0;
        [Range(0, 10000)] public int grassPoolMax = 0;
        [Range(0, 999)] public int godEmberMax = 999;        // 锁定 999

        [Tooltip("物品池（仓储）")]
        [Range(8, 96)] public int warehouseL1Slots = 24;
        [Range(8, 192)] public int warehouseL2Slots = 48;

        // ============================================================
        // Einherjar（einherjar.md + systems-index §3.5）
        // ============================================================
        [Header("=== Einherjar ===")]

        [Tooltip("聚落容量（systems-index §3.5 锁定）")]
        [Range(1, 16)] public int longHouseL1Capacity = 4;
        [Range(1, 16)] public int longHouseL2Capacity = 8;

        [Tooltip("英灵工作 1-3 单位/小时（systems-index §3.2）")]
        [Range(0.5f, 5f)] public float einherjarWorkMinUnitsPerHour = 1f;
        [Range(0.5f, 5f)] public float einherjarWorkMaxUnitsPerHour = 3f;

        [Tooltip("英灵死亡预告时间范围")]
        [Range(1, 60)] public int daysToDeathMin = 1;
        [Range(1, 120)] public int daysToDeathMax = 30;

        [Tooltip("强留腐化 3-5 天（death-sendoff.md）")]
        [Range(1, 10)] public int keepCorruptionDaysMin = 3;
        [Range(1, 10)] public int keepCorruptionDaysMax = 5;

        [Tooltip("衰悼期（systems-index §3.2 锁定：24h, -20%）")]
        [Range(0, 48)] public int mourningHours = 24;
        [Range(0f, 1f)] public float mourningMoralePenalty = 0.20f;

        // ============================================================
        // Settlement（settlement.md）
        // ============================================================
        [Header("=== Settlement ===")]

        [Tooltip("士气上限 0-1")]
        [Range(0f, 1f)] public float moraleMax = 1f;
        [Tooltip("士气自然恢复速率（/小时）")]
        [Range(0f, 0.1f)] public float moraleRegenPerHour = 0.02f;

        [Tooltip("建筑升级基础材料消耗（systems-index §3.2）")]
        [Range(10, 200)] public int upgradeL1ToL2Wood = 50;
        [Range(10, 200)] public int upgradeL1ToL2Iron = 20;

        // ============================================================
        // Combat（combat.md + systems-index §3.2）
        // ============================================================
        [Header("=== Combat ===")]

        [Tooltip("基础玩家 HP")]
        [Range(50, 500)] public int playerBaseHP = 100;
        [Tooltip("战斗消耗 1-3 HP/次（systems-index §3.2）")]
        [Range(0f, 10f)] public float combatHPLossMin = 1f;
        [Range(0f, 10f)] public float combatHPLossMax = 3f;
        [Tooltip("武器耐久 -5/次（systems-index §3.2）")]
        [Range(1, 50)] public int weaponDurabilityLossPerHit = 5;

        [Tooltip("工具耐久范围（data-config ItemSO.ToolSO 锁定）")]
        [Range(50, 500)] public int toolDurabilityMin = 50;
        [Range(50, 500)] public int toolDurabilityMax = 500;

        // ============================================================
        // Boss（boss-design.md + systems-index §3.5）
        // ============================================================
        [Header("=== Boss ===")]

        [Tooltip("Boss 死亡冷却（systems-index §3.5 锁定：24h 避免刷）")]
        [Range(0, 168)] public int bossRespawnCooldownHours = 24;
        [Tooltip("Boss 数量（game-concept §11.1）")]
        [Range(1, 8)] public int totalBosses = 4;

        [Tooltip("Boss HP 范围（data-config §C.2 类型 6 锁定）")]
        [Range(100, 10000)] public int bossHPMin = 500;
        [Range(100, 10000)] public int bossHPMax = 5000;

        // ============================================================
        // World Exploration（world-exploration.md）
        // ============================================================
        [Header("=== World Exploration ===")]

        [Tooltip("6 群系难度系数范围（lenses P2 修复）")]
        [Range(0.5f, 2f)] public float biomeDifficultyMin = 0.7f;
        [Range(0.5f, 2f)] public float biomeDifficultyMax = 1.5f;
        [Tooltip("寒冷 debuff（systems-index §3.4：永冻崖 -1 HP/分钟）")]
        [Range(0f, 5f)] public float coldDPS = 1f;

        [Tooltip("远征 5 分钟循环（game-concept §4）")]
        [Range(0.25f, 12f)] public float expeditionMinHours = 0.5f;
        [Range(0.25f, 12f)] public float expeditionMaxHours = 4f;

        // ============================================================
        // Performance Budgets（handover §9 实现层锁定）
        // ============================================================
        [Header("=== Performance ===")]

        [Range(30, 144)] public int targetFrameRate = 60;
        [Range(8, 33)] public int frameBudgetMs = 16;
        [Range(1, 32)] public int maxAudioSources = 8;
        [Range(100, 2000)] public int maxParticlesPerFrame = 500;
        [Range(0.1f, 1f)] public float stateMachineBudgetFraction = 0.5f;

        [Tooltip("UI 淡入淡出（handover 锁定：0.5s / 0.3s）")]
        [Range(0f, 2f)] public float uiFadeInSeconds = 0.5f;
        [Range(0f, 2f)] public float uiFadeOutSeconds = 0.3f;

        [Tooltip("0.5s 不可跳淡入（handover 锁定：奥丁审判独白前 10 秒）")]
        [Range(0f, 2f)] public float unskippableFadeInSeconds = 0.5f;
        [Range(0, 60)] public int unskippableTriggerSeconds = 10;

        [Tooltip("Save/Load 热重载 debounce（data-config §C.1 规则 5 锁定：500ms）")]
        [Range(100, 2000)] public int hotReloadDebounceMs = 500;
        [Tooltip("加载策略：true = 异步，false = MVP 同步（data-config §C.1 锁定 false）")]
        public bool loadAsync = false;

        // ============================================================
        // Difficulty / New Player（systems-index §3.6）
        // ============================================================
        [Header("=== Difficulty ===")]

        [Tooltip("新手期 daySpeedFactor（systems-index §3.6 锁定：0.5/0.7）")]
        [Range(0.1f, 1f)] public float newbieDaySpeedFactor1 = 0.5f;
        [Range(0.1f, 1f)] public float newbieDaySpeedFactor2 = 0.7f;
        [Range(0, 30)] public int newbieDurationDays = 30;

        [Tooltip("易难度 / 平衡昼夜选项（game-concept §13 退路）")]
        public bool allowEasyMode = true;
        public bool allowBalancedDayNight = true;

        // ============================================================
        // Save/Load（save-system.md）
        // ============================================================
        [Header("=== Save/Load ===")]

        [Tooltip("自动 save 间隔（分钟）")]
        [Range(1, 60)] public int autoSaveIntervalMinutes = 5;
        [Tooltip("最多 save 槽数")]
        [Range(1, 20)] public int maxSaveSlots = 5;

        // ============================================================
        // Anti-Pillar Guards（game-concept §7 + 验证用）
        // ============================================================
        [Header("=== Anti-Pillar Guards ===")]

        [Tooltip("true = 启用科技树检测（永远 false，Anti-1）")]
        public bool enableTechTreeGuard = false;
        [Tooltip("true = 启用 soulslike 死亡检测（永远 false，Anti-3）")]
        public bool enableSoulslikeGuard = false;

        // ============================================================
        // Validation
        // ============================================================
        public List<string> Validate()
        {
            var errs = new List<string>();
            // 概率性字段范围已经在 Range attribute 限制，Editor 端先报
            // 跨字段约束（最少检查几个关键不变量）
            float dayTotal = dayDawnRatio + dayRatio + dayDuskRatio + nightRatio + midnightRatio + deepNightRatio;
            if (Mathf.Abs(dayTotal - 1f) > 0.001f)
                errs.Add($"Day-Night 段比例总和必须 = 1.0，当前 {dayTotal:F3}");

            if (longHouseL1Capacity > longHouseL2Capacity)
                errs.Add("longHouseL1Capacity must be <= longHouseL2Capacity");

            if (daysToDeathMin > daysToDeathMax)
                errs.Add("daysToDeathMin must be <= daysToDeathMax");

            if (moonPhaseDurations == null || moonPhaseDurations.Length != 8)
                errs.Add("moonPhaseDurations must have exactly 8 phases (Waxing Moon)");

            if (enableTechTreeGuard)
                errs.Add("enableTechTreeGuard must be false (Anti-1 NOT 科技树)");
            if (enableSoulslikeGuard)
                errs.Add("enableSoulslikeGuard must be false (Anti-3 NOT soulslike)");

            return errs;
        }
    }
}
