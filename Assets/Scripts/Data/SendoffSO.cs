using System.Collections.Generic;
using UnityEngine;

namespace Ravensong.Data
{
    /// <summary>
    /// 送别方式（death-sendoff.md + data-config §C.2 类型 10）。
    /// 锁定的 Wyrd 支柱：选择有重量。
    /// </summary>
    [CreateAssetMenu(menuName = "Ravensong/Sendoff", fileName = "Sendoff")]
    public class SendoffSO : ScriptableObject, IDataValidatable
    {
        public string SchemaVersion => "1.0";

        [Header("Identity")]
        [Tooltip("唯一 ID，格式 sendoff_<name>")]
        public string id;
        public string displayName;
        [TextArea(3, 6)] public string description;

        [Header("Type")]
        public SendoffType sendoffType = SendoffType.Valhalla;

        [Header("Valhalla Buff (送走获得)")]
        [Tooltip("送走后给的永久 buff 类型")]
        public string buffId;
        public StatBlock buffStats;
        [TextArea] public string buffDescription;

        [Header("Keep Cost (强留代价)")]
        [Tooltip("强留 3-5 天后变成尸鬼反噬的概率")]
        [Range(0f, 1f)] public float corruptionChance = 0.5f;
        [Range(0, 100)] public int daysBeforeCorruption = 4;
        [TextArea] public string corruptionDescription;

        [Header("Settlement Effect")]
        [Tooltip("聚落 -20% 士气持续 24h（systems-index §3.2 锁定）")]
        [Range(0f, 1f)] public float settlementMoralePenalty = 0.2f;
        [Range(0, 48)] public int moralePenaltyHours = 24;

        public List<string> Validate()
        {
            var errs = new List<string>();
            if (string.IsNullOrEmpty(id)) errs.Add("SendoffSO.id is required");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(id, @"^sendoff_[a-z0-9_]+$"))
                errs.Add($"SendoffSO.id '{id}' must match pattern sendoff_<name>");

            if (string.IsNullOrEmpty(displayName)) errs.Add("SendoffSO.displayName is required");
            return errs;
        }
    }
}
