using System.Collections.Generic;
using UnityEngine;

namespace Ravensong.Data
{
    /// <summary>
    /// 英灵档案（game-concept §3.3 Living Hearth 核心 + data-config §C.2 类型 3）。
    /// 锁定：会衰老、生病、被狼咬会死。
    /// </summary>
    [CreateAssetMenu(menuName = "Ravensong/Einherjar", fileName = "Einherjar")]
    public class EinherjarSO : ScriptableObject, IDataValidatable
    {
        public string SchemaVersion => "1.0";

        [Header("Identity")]
        [Tooltip("唯一 ID，格式 einherjar_<name>")]
        public string id;
        public string displayName;
        public Sprite portrait;
        [TextArea(3, 6)] public string backstory;

        [Header("Vitals")]
        public Profession profession = Profession.Hunter;
        [Tooltip("招募时年龄（决定死亡时间窗口）")]
        [Range(18, 80)] public int ageAtRecruitment = 25;
        public TraitEntry[] traits;

        [Header("Work")]
        public ResourceType workType = ResourceType.Food;
        [Range(0.5f, 2f)] public float workEfficiency = 1f;

        [Header("Death (signature — Death & Send-off)")]
        [Tooltip("true = 注定死亡（玩家会被预告）")]
        public bool willDie = true;
        [Tooltip("招募后几天会死（"缓慢恶化"机制）")]
        [Range(1, 60)] public int daysToDeath = 7;
        [TextArea(2, 4)] public string deathQuote;
        [Tooltip("送走后给的永久 buff 描述")]
        [TextArea(2, 4)] public string valhallaReward;
        public StatBlock valhallaBuff;

        [Header("Voice (后续接入 FMOD)")]
        public AudioClip greetingLine;
        public AudioClip workLine;

        public List<string> Validate()
        {
            var errs = new List<string>();
            if (string.IsNullOrEmpty(id)) errs.Add("EinherjarSO.id is required");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(id, @"^einherjar_[a-z0-9_]+$"))
                errs.Add($"EinherjarSO.id '{id}' must match pattern einherjar_<name>");

            if (string.IsNullOrEmpty(displayName)) errs.Add("EinherjarSO.displayName is required");
            if (willDie && daysToDeath < 1)
                errs.Add("EinherjarSO.daysToDeath must be >= 1 when willDie=true");
            return errs;
        }
    }
}
