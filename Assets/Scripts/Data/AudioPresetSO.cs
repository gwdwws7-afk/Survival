using System.Collections.Generic;
using UnityEngine;

namespace Ravensong.Data
{
    /// <summary>
    /// Audio 预设（data-config §C.2 类型 16 + vfx-audio.md）。
    /// FMOD 集成在 FMOD package 装上后接入。
    /// </summary>
    [CreateAssetMenu(menuName = "Ravensong/Audio", fileName = "AudioPreset")]
    public class AudioPresetSO : ScriptableObject, IDataValidatable
    {
        public string SchemaVersion => "1.0";

        [Header("Identity")]
        [Tooltip("唯一 ID，格式 audio_<name>")]
        public string id;
        public string displayName;
        [Tooltip("分类：bgm_day / bgm_night / sfx_weave / sfx_combat / ambience / voice")]
        public string category = "sfx";

        [Header("Clips")]
        [Tooltip("FMOD event 路径或 Unity AudioClip（fallback）")]
        public string fmodEventPath;
        public AudioClip fallbackClip;

        [Header("Variation")]
        [Tooltip("随机播放变体（避免重复感）")]
        public AudioClip[] variationClips;

        [Header("Mix")]
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitchMin = 0.95f;
        [Range(0.1f, 3f)] public float pitchMax = 1.05f;
        [Tooltip("空间音效 3D 距离")]
        [Range(1f, 100f)] public float maxDistance = 50f;

        [Header("Performance（handover 锁定：8 AudioSource 上限）")]
        public bool allowConcurrent = true;

        public List<string> Validate()
        {
            var errs = new List<string>();
            if (string.IsNullOrEmpty(id)) errs.Add("AudioPresetSO.id is required");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(id, @"^audio_[a-z0-9_]+$"))
                errs.Add($"AudioPresetSO.id '{id}' must match pattern audio_<name>");

            if (string.IsNullOrEmpty(fmodEventPath) && fallbackClip == null && (variationClips == null || variationClips.Length == 0))
                errs.Add("AudioPresetSO must have at least one audio source (fmod path / fallback / variation)");
            return errs;
        }
    }
}
