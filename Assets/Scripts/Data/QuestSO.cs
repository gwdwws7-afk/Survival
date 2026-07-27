using System.Collections.Generic;
using UnityEngine;

namespace Ravensong.Data
{
    /// <summary>
    /// 任务（data-config §C.2 类型 14 + quest-event.md）。
    /// </summary>
    [CreateAssetMenu(menuName = "Ravensong/Quest", fileName = "Quest")]
    public class QuestSO : ScriptableObject, IDataValidatable
    {
        public string SchemaVersion => "1.0";

        [Header("Identity")]
        [Tooltip("唯一 ID，格式 quest_<name>")]
        public string id;
        public string displayName;
        [TextArea(3, 6)] public string description;
        public Sprite icon;

        [Header("Type")]
        [Tooltip("main / side / hidden / oath_milestone")]
        public string questType = "side";

        [Header("Objectives")]
        public QuestObjective[] objectives;

        [Header("Rewards")]
        public ItemStack[] itemRewards;
        [Range(0, 999)] public int godEmberReward = 0;
        [Tooltip("解锁的誓言 milestone（可选）")]
        public string triggersOathMilestoneId;

        [Header("Trigger")]
        [Tooltip("触发条件（"自动" / "进入 X 群系" / "杀死 Y"）")]
        public string triggerCondition;

        public List<string> Validate()
        {
            var errs = new List<string>();
            if (string.IsNullOrEmpty(id)) errs.Add("QuestSO.id is required");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(id, @"^quest_[a-z0-9_]+$"))
                errs.Add($"QuestSO.id '{id}' must match pattern quest_<name>");

            if (objectives == null || objectives.Length < 1) errs.Add("QuestSO.objectives must be >= 1");
            return errs;
        }
    }

    [System.Serializable]
    public class QuestObjective
    {
        public string objectiveId;        // "collect" / "kill" / "weave" / "send" 等
        [TextArea] public string description;
        [Tooltip("目标 ID（Item/Boss/Recipe/...）")]
        public string targetId;
        [Min(1)] public int requiredCount = 1;
    }
}
