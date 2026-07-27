using System.Collections.Generic;
using UnityEngine;

namespace Ravensong.Data
{
    /// <summary>
    /// 对话（data-config §C.2 类型 8）。
    /// 包含 NPC 招募对话、英灵生前身后对白、奥丁审判独白等。
    /// </summary>
    [CreateAssetMenu(menuName = "Ravensong/Dialogue", fileName = "Dialogue")]
    public class DialogueSO : ScriptableObject, IDataValidatable
    {
        public string SchemaVersion => "1.0";

        [Header("Identity")]
        [Tooltip("唯一 ID，格式 dialogue_<name>")]
        public string id;
        public string displayName;

        [Header("Content")]
        [Tooltip("关联角色 ID（EinherjarSO.id 或空 = 奥丁）")]
        public string speakerId;
        public DialogueLine[] lines;

        [Header("Branching (optional)")]
        [Tooltip("选项 → 下一个 dialogue ID")]
        public DialogueChoice[] choices;

        [Header("Audio (FMOD hook)")]
        public AudioClip voiceLine;
        [Tooltip("0.5s 不可跳的淡入（handover 锁定：奥丁审判独白前 10 秒）")]
        public bool requiresUnskippableFadeIn = false;

        public List<string> Validate()
        {
            var errs = new List<string>();
            if (string.IsNullOrEmpty(id)) errs.Add("DialogueSO.id is required");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(id, @"^dialogue_[a-z0-9_]+$"))
                errs.Add($"DialogueSO.id '{id}' must match pattern dialogue_<name>");

            if (lines == null || lines.Length < 1) errs.Add("DialogueSO.lines must be >= 1");
            return errs;
        }
    }

    [System.Serializable]
    public class DialogueLine
    {
        [TextArea(2, 5)] public string text;
        public float displayDuration = 3f;     // 自动下一行秒数（0 = 玩家点）
        public Sprite portraitOverride;        // 换表情（可选）
        public string sfxId;                   // 音效 ID
    }

    [System.Serializable]
    public class DialogueChoice
    {
        [TextArea(1, 2)] public string prompt;
        public string nextDialogueId;
    }
}
