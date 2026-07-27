using System.Collections.Generic;
using UnityEngine;

namespace Ravensong.Data
{
    /// <summary>
    /// VFX 预设（data-config §C.2 类型 15 + vfx.md + vfx-audio.md）。
    /// 编织丝线 / 月相 / 神力爆发 / 死亡消散 等。
    /// </summary>
    [CreateAssetMenu(menuName = "Ravensong/VFX", fileName = "VFXPreset")]
    public class VFXPresetSO : ScriptableObject, IDataValidatable
    {
        public string SchemaVersion => "1.0";

        [Header("Identity")]
        [Tooltip("唯一 ID，格式 vfx_<name>")]
        public string id;
        public string displayName;

        [Header("Visual")]
        [Tooltip("prefab（用 Unity VFX Graph + Shader Graph 组合）")]
        public GameObject prefab;
        [Tooltip("配色（style-bible 锁定：cyan 4DD8E6 / gold C9A567）")]
        public Color primaryColor = new Color(0x4D / 255f, 0xD8 / 255f, 0xE6 / 255f, 1f);
        public Color secondaryColor = new Color(0xC9 / 255f, 0xA5 / 255f, 0x67 / 255f, 1f);

        [Header("Timing")]
        [Range(0.05f, 5f)] public float duration = 1f;
        [Range(0f, 2f)] public float fadeIn = 0.1f;
        [Range(0f, 2f)] public float fadeOut = 0.3f;

        [Header("Performance（handover 锁定：500 粒子/帧上限）")]
        [Range(1, 500)] public int maxParticles = 100;

        public List<string> Validate()
        {
            var errs = new List<string>();
            if (string.IsNullOrEmpty(id)) errs.Add("VFXPresetSO.id is required");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(id, @"^vfx_[a-z0-9_]+$"))
                errs.Add($"VFXPresetSO.id '{id}' must match pattern vfx_<name>");

            if (prefab == null) errs.Add("VFXPresetSO.prefab is required (can be assigned later)");
            return errs;
        }
    }
}
