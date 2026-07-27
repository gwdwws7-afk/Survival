using System.Collections.Generic;
using UnityEngine;

namespace Ravensong.Data
{
    /// <summary>
    /// 物品基础类型（data-config §C.2 类型 2）。
    /// ToolSO 继承扩展（gathering GDD 锁定）。
    /// </summary>
    [CreateAssetMenu(menuName = "Ravensong/Item", fileName = "Item")]
    public class ItemSO : ScriptableObject, IDataValidatable
    {
        public string SchemaVersion => "1.0";

        [Header("Identity")]
        [Tooltip("唯一 ID，格式 item_<name>")]
        public string id;
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;

        [Header("Category")]
        public ItemCategory category = ItemCategory.Resource;
        [Tooltip("装备槽（仅当 category=Equipment）")]
        public EquipmentSlot equipSlot = EquipmentSlot.None;

        [Header("Stacking")]
        public bool stackable = true;
        [Min(1)] public int maxStack = 1;

        [Header("Economy")]
        [Tooltip("基础价值（用于交易/估价）")]
        public int value = 1;

        [Header("Combat (if equipment)")]
        public StatBlock stats;
        public StatusEffect onHit;

        [Header("Consumable (if consumable)")]
        public ConsumableEffect effect;

        [Header("Day-Night Bonus (v1.2 锁定)")]
        public DayNightItemBonus dayBonus;
        public DayNightItemBonus nightBonus;

        public List<string> Validate()
        {
            var errs = new List<string>();
            if (string.IsNullOrEmpty(id)) errs.Add("ItemSO.id is required");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(id, @"^item_[a-z0-9_]+$"))
                errs.Add($"ItemSO.id '{id}' must match pattern item_<name>");

            if (string.IsNullOrEmpty(displayName)) errs.Add("ItemSO.displayName is required");
            if (category == ItemCategory.Equipment && equipSlot == EquipmentSlot.None)
                errs.Add("Equipment items must have a valid equipSlot");
            if (maxStack < 1) errs.Add("ItemSO.maxStack must be >= 1");
            return errs;
        }
    }

    /// <summary>
    /// 工具扩展（gathering GDD 锁定）。
    /// </summary>
    [CreateAssetMenu(menuName = "Ravensong/Tool", fileName = "Tool")]
    public class ToolSO : ItemSO
    {
        [Header("Tool")]
        public ToolType toolType = ToolType.Axe;
        [Range(1, 5)] public int tier = 1;
        public bool isTwoHanded;

        [Header("Gather Modifiers")]
        [Range(1f, 3f)] public float gatherSpeedMult = 1f;
        [Range(1f, 2f)] public float gatherYieldMult = 1f;
        [Tooltip("基础耐久 50-500（实际值从 GameConfigSO 读）")]
        [Range(50, 500)] public int baseDurability = 100;
    }
}
