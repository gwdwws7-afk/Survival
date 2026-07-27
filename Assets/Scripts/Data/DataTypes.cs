using System;
using UnityEngine;

namespace Ravensong.Data
{
    // ============================================================
    // Enums（game-concept §6/§7 + data-config §C.2 推导出）
    // ============================================================

    /// <summary>编织配方 Tier（data-config §C.2 RecipeSO）</summary>
    public enum RecipeTier
    {
        Common = 1,    // 基础
        Uncommon = 2,  // 常见
        Rare = 3,      // 稀有
        Epic = 4,      // 史诗
        Legendary = 5  // 传说
    }

    /// <summary>日夜约束（编织配方只能在某时段用）</summary>
    public enum DayNightRequirement
    {
        Any = 0,
        Day = 1,
        Night = 2
    }

    /// <summary>物品大类（data-config §C.2 ItemSO）</summary>
    public enum ItemCategory
    {
        Resource = 0,    // 资源
        Equipment = 1,   // 装备
        Consumable = 2,  // 消耗品
        Quest = 3,       // 任务品
        Token = 4        // 代币（god-ember 等）
    }

    /// <summary>装备槽位</summary>
    public enum EquipmentSlot
    {
        None = 0,
        Main = 1,     // 主手武器
        Off = 2,      // 副手
        Body = 3,     // 身体甲
        Head = 4,     // 头部
        Accessory = 5 // 饰品
    }

    /// <summary>英灵职业（data-config §C.2 EinherjarSO）</summary>
    public enum Profession
    {
        Blacksmith = 0, // 铁匠 → 锻冶产能
        Hunter = 1,     // 猎人 → 食物产能
        Skald = 2,      // 吟游诗人 → 编织加成
        Farmer = 3,     // 农夫 → 木材/草产能
        Warrior = 4,    // 战士 → 战斗产能
        Healer = 5      // 治疗 → 伤病恢复
    }

    /// <summary>基础资源类型（4 种 + god-ember）</summary>
    public enum ResourceType
    {
        Iron = 0,
        Food = 1,
        Wood = 2,
        Grass = 3,
        GodEmber = 4
    }

    /// <summary>英灵性格特征（占位；具体在 SO 中以 string 列表维护）</summary>
    public enum TraitCategory
    {
        Positive = 0,
        Neutral = 1,
        Negative = 2
    }

    /// <summary>5 誓言类型（game-concept §4 + oath-system.md）</summary>
    public enum OathType
    {
        Forge = 0,     // 锻冶之誓
        Hearth = 1,    // 炉火之誓
        Wild = 2,      // 荒野之誓
        Death = 3,     // 亡者之誓
        Canopy = 4     // 苍穹之誓（终局）
    }

    /// <summary>6 种生物群系（world-exploration.md）</summary>
    public enum BiomeId
    {
        BirchGrove = 0,    // 白桦林（首推 D4）
        Bonefield = 1,     // 白骨原
        AbyssMarsh = 2,    // 深渊沼
        FrozenCliff = 3,   // 永冻崖
        AshenReach = 4,    // 灰烬之原
        ValhallaGate = 5   // 英灵殿门（终局）
    }

    /// <summary>工具类型（gathering.md）</summary>
    public enum ToolType
    {
        None = 0,
        Axe = 1,           // 斧
        Pick = 2,          // 镐
        Bow = 3,           // 弓
        FishingRod = 4,    // 钓竿
        Sickle = 5,        // 镰
        Hammer = 6         // 锤
    }

    /// <summary>送别方式（death-sendoff.md）</summary>
    public enum SendoffType
    {
        Valhalla = 0,   // 送走（buff 换）
        KeepBody = 1    // 强留（3-5 天腐化警告）
    }

    // ============================================================
    // Helper Classes（SO 字段中复用的结构）
    // ============================================================

    /// <summary>状态块（攻击/防御/特殊值）</summary>
    [Serializable]
    public class StatBlock
    {
        public float attackDamage = 0f;
        public float defense = 0f;
        public float moveSpeed = 0f;
        public float gatherSpeed = 0f;
        public float gatherYield = 0f;
        public float critChance = 0f;
        public float lifeSteal = 0f;

        public static StatBlock Zero => new StatBlock();

        public StatBlock Add(StatBlock other)
        {
            if (other == null) return this;
            attackDamage += other.attackDamage;
            defense += other.defense;
            moveSpeed += other.moveSpeed;
            gatherSpeed += other.gatherSpeed;
            gatherYield += other.gatherYield;
            critChance += other.critChance;
            lifeSteal += other.lifeSteal;
            return this;
        }
    }

    /// <summary>负面状态效果（攻击附带）</summary>
    [Serializable]
    public class StatusEffect
    {
        public string effectId;          // "burn" / "freeze" / "curse" 等
        public float duration = 0f;      // 秒
        public float potency = 0f;       // 强度（按 effectId 解释）
    }

    /// <summary>消耗品效果</summary>
    [Serializable]
    public class ConsumableEffect
    {
        public string effectId;          // "heal_hp" / "restore_godember" 等
        public float amount = 0f;        // 数值
        public float duration = 0f;      // 持续时间（0 = 瞬时）
    }

    /// <summary>日夜装备加成（v1.2 inventory GDD #3 锁定）</summary>
    [Serializable]
    public class DayNightItemBonus
    {
        public StatBlock stats;
        [TextArea] public string description;
    }

    /// <summary>英灵性格特征条目（不用 enum，因为特性名是开放式的）</summary>
    [Serializable]
    public class TraitEntry
    {
        public string traitId;       // "strong" / "sickly" / "lucky" 等
        [TextArea] public string description;
        public TraitCategory category = TraitCategory.Positive;
        public float potency = 1.0f; // 0.5 - 2.0
    }
}
