using System;
using System.Collections.Generic;
using UnityEngine;
using Ravensong.Data;
using Ravensong.Core;

namespace Ravensong.Oath
{
    /// <summary>
    /// 5 签名系统 #4：Oath System（oath-system.md + game-concept §4）。
    /// 5 誓言 + milestone tracking + 4-oath → 苍穹解锁 → 5 → 奥丁审判（终局）。
    /// </summary>
    public class OathManager : MonoBehaviour
    {
        public static OathManager Instance { get; private set; }

        [Header("Runtime")]
        public List<OathProgress> progress = new();

        public event Action<OathSO, OathMilestone> OnMilestoneCompleted;
        public event Action<OathSO> OnOathCompleted;
        public event Action OnCanopyUnlocked;     // 4 誓言完成
        public event Action OnOdinJudgmentTriggered;   // 终局

        private DataRegistry _registry;
        private SettlementManager _settlement;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start()
        {
            _registry = DataRegistry.Instance;
            _settlement = SettlementManager.Instance;
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        // ============================================================
        // 初始化（载入全部誓言 + 创建 progress）
        // ============================================================

        public void InitializeFromData()
        {
            progress.Clear();
            if (_registry == null) return;
            // 反射访问 _oaths（类似 WeavingSystem）
            var oaths = _registry.GetType()
                .GetField("_oaths", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(_registry) as Dictionary<string, OathSO>;
            if (oaths == null) return;
            foreach (var o in oaths.Values)
            {
                progress.Add(new OathProgress
                {
                    oathId = o.id,
                    oathType = o.oathType,
                    milestoneProgress = new int[o.milestones?.Length ?? 0]
                });
            }
            Debug.Log($"[Oath] Initialized {progress.Count} oaths");
        }

        // ============================================================
        // 进度更新（由 WeavingSystem / EinherjarManager / SettlementManager 等调用）
        // ============================================================

        /// <summary>更新某个 milestone 的进度（+1）。</summary>
        public void AddMilestoneProgress(string oathId, int milestoneIndex, int delta = 1)
        {
            var p = progress.Find(x => x.oathId == oathId);
            if (p == null || p.completed) return;
            if (milestoneIndex < 0 || milestoneIndex >= p.milestoneProgress.Length) return;
            p.milestoneProgress[milestoneIndex] += delta;

            var oath = _registry.GetOath(oathId);
            if (oath == null) return;
            int target = oath.milestones[milestoneIndex].target;
            if (p.milestoneProgress[milestoneIndex] >= target)
            {
                p.milestoneProgress[milestoneIndex] = target;
                OnMilestoneCompleted?.Invoke(oath, oath.milestones[milestoneIndex]);
                Debug.Log($"[Oath] ✓ '{oath.displayName}' milestone '{oath.milestones[milestoneIndex].displayName}' completed");

                // 全部 milestone 完成？
                if (IsAllMilestoneDone(p))
                {
                    p.completed = true;
                    OnOathCompleted?.Invoke(oath);
                    Debug.Log($"[Oath] ✓ '{oath.displayName}' fully completed");
                    CheckCanopyUnlock();
                }
            }
        }

        public void AddMilestoneProgress(OathType type, int milestoneIndex, int delta = 1)
        {
            var p = progress.Find(x => x.oathType == type);
            if (p != null) AddMilestoneProgress(p.oathId, milestoneIndex, delta);
        }

        private bool IsAllMilestoneDone(OathProgress p)
        {
            for (int i = 0; i < p.milestoneProgress.Length; i++)
                if (p.milestoneProgress[i] < GetMilestoneTarget(p.oathId, i))
                    return false;
            return true;
        }

        private int GetMilestoneTarget(string oathId, int idx)
        {
            var oath = _registry.GetOath(oathId);
            if (oath == null || oath.milestones == null) return int.MaxValue;
            if (idx < 0 || idx >= oath.milestones.Length) return int.MaxValue;
            return oath.milestones[idx].target;
        }

        // ============================================================
        // 苍穹之誓 / 奥丁审判
        // ============================================================

        private void CheckCanopyUnlock()
        {
            int completed = progress.FindAll(p => p.completed && p.oathType != OathType.Canopy).Count;
            if (completed >= 4)
            {
                // 解锁苍穹
                var canopy = progress.Find(p => p.oathType == OathType.Canopy);
                if (canopy != null && !canopy.unlocked)
                {
                    canopy.unlocked = true;
                    OnCanopyUnlocked?.Invoke();
                    Debug.Log("[Oath] ⭐ Canopy (苍穹之誓) UNLOCKED! Complete its milestones to face Odin.");
                }
            }
        }

        /// <summary>苍穹之誓完成时触发奥丁审判（终局）</summary>
        public void TriggerOdinJudgment()
        {
            var canopy = progress.Find(p => p.oathType == OathType.Canopy);
            if (canopy == null || !canopy.unlocked || !canopy.completed) return;
            OnOdinJudgmentTriggered?.Invoke();
            Debug.Log("[Oath] ⚔ 奥丁审判触发（终局）");
        }

        // ============================================================
        // 状态查询
        // ============================================================

        public OathProgress GetProgress(string oathId) => progress.Find(p => p.oathId == oathId);
        public OathProgress GetProgress(OathType type) => progress.Find(p => p.oathType == type);

        public int CompletedOathCount() => progress.FindAll(p => p.completed).Count;
    }

    [Serializable]
    public class OathProgress
    {
        public string oathId;
        public OathType oathType;
        public int[] milestoneProgress;
        public bool completed;
        public bool unlocked;   // 苍穹之誓专用
    }
}
