using System.Collections.Generic;
using UnityEngine;

namespace Ravensong.Data
{
    /// <summary>
    /// UI 风格主题（data-config §C.2 类型 11）。
    /// 锁定配色：navy #0A1A2F / cyan #4DD8E6 / gold #C9A567（style-bible §3）。
    /// </summary>
    [CreateAssetMenu(menuName = "Ravensong/UIStyle", fileName = "UIStyle")]
    public class UIStyleSO : ScriptableObject, IDataValidatable
    {
        public string SchemaVersion => "1.0";

        [Header("Identity")]
        [Tooltip("唯一 ID，格式 uistyle_<name>")]
        public string id;
        public string displayName;

        [Header("Palette（style-bible §3 铁律）")]
        [Tooltip("Deep Midnight Blue，60% 占比")]
        public Color deepBlue = new Color(0x0A / 255f, 0x1A / 255f, 0x2F / 255f, 1f);
        [Tooltip("Cyan Magic Light，15% 占比")]
        public Color cyan = new Color(0x4D / 255f, 0xD8 / 255f, 0xE6 / 255f, 1f);
        [Tooltip("Warm Gold，10% 占比")]
        public Color warmGold = new Color(0xC9 / 255f, 0xA5 / 255f, 0x67 / 255f, 1f);
        [Tooltip("辅色（≤15%），可多个")]
        public Color[] secondaryColors;

        [Header("Typography")]
        public Font primaryFont;             // 北欧手写感衬线（待选）
        public Font accentFont;              // 数字/计时器
        [Range(8, 64)] public int baseFontSize = 18;

        [Header("Animation")]
        [Tooltip("淡入 0.5s / 淡出 0.3s（handover 实现层锁定）")]
        [Range(0f, 2f)] public float fadeInDuration = 0.5f;
        [Range(0f, 2f)] public float fadeOutDuration = 0.3f;

        [Header("Rune Icons（style-bible §7 锁定"画"出来）")]
        [Tooltip("手绘的卢恩符号 sprite 集")]
        public Sprite[] runeIcons;

        public List<string> Validate()
        {
            var errs = new List<string>();
            if (string.IsNullOrEmpty(id)) errs.Add("UIStyleSO.id is required");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(id, @"^uistyle_[a-z0-9_]+$"))
                errs.Add($"UIStyleSO.id '{id}' must match pattern uistyle_<name>");

            if (primaryFont == null) errs.Add("UIStyleSO.primaryFont is required");
            return errs;
        }
    }
}
