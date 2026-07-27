using System.Collections.Generic;
using UnityEngine;

namespace Ravensong.Data
{
    /// <summary>
    /// 编织配方（game-concept §3.1 Woven Power 核心 + data-config §C.2 类型 1）。
    /// 锁定：永远开放，不靠解锁（Anti-1 NOT 科技树）。
    /// </summary>
    [CreateAssetMenu(menuName = "Ravensong/Recipe", fileName = "Recipe")]
    public class RecipeSO : ScriptableObject, IDataValidatable
    {
        public string SchemaVersion => "1.0";

        [Header("Identity")]
        [Tooltip("唯一 ID，格式 recipe_<name>，永久不可改")]
        public string id;
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;

        [Header("Crafting")]
        [Tooltip("1-3 个输入物品")]
        public ItemStack[] inputs;
        [Tooltip("1 个输出物品")]
        public ItemStack output;
        public RecipeTier tier = RecipeTier.Common;
        [Tooltip("消耗的神力余烬数（5-25）")]
        [Range(0, 100)] public int godEmberCost = 5;

        [Header("Requirements")]
        public DayNightRequirement dayNight = DayNightRequirement.Any;
        [Tooltip("可选：需要先解锁的誓言")]
        public OathType? requiredOath;
        public string requiredOathId;

        [Header("Discovery")]
        [Tooltip("未发现前玩家看不到（不依赖 Tier 锁定）")]
        public bool isHidden;
        [TextArea] public string discoveryHint;

        public List<string> Validate()
        {
            var errs = new List<string>();
            if (string.IsNullOrEmpty(id)) errs.Add("RecipeSO.id is required");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(id, @"^recipe_[a-z0-9_]+$"))
                errs.Add($"RecipeSO.id '{id}' must match pattern recipe_<name> (lowercase snake_case)");

            if (string.IsNullOrEmpty(displayName)) errs.Add("RecipeSO.displayName is required");
            if (inputs == null || inputs.Length < 1 || inputs.Length > 3)
                errs.Add($"RecipeSO.inputs must be 1-3 items, got {(inputs?.Length ?? 0)}");
            if (output == null || output.IsEmpty)
                errs.Add("RecipeSO.output is required");
            if (godEmberCost < 0)
                errs.Add("RecipeSO.godEmberCost must be >= 0");
            return errs;
        }
    }
}
