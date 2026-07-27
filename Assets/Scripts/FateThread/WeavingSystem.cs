using System.Collections.Generic;
using UnityEngine;
using Ravensong.Data;
using Ravensong.Core;

namespace Ravensong.FateThread
{
    /// <summary>
    /// 5 签名系统 #2：Fate-Thread / Weaving（fate-thread.md + game-concept §3.1）。
    /// 核心动词：左手 A + 右手 B → 编织出 C。永远开放，无科技树（Anti-1 锁定）。
    /// </summary>
    public class WeavingSystem : MonoBehaviour
    {
        public static WeavingSystem Instance { get; private set; }

        [Header("Runtime")]
        public int playerGodEmber = 50;

        public event System.Action<RecipeSO, bool> OnWeaveAttempted;   // (recipe, success)
        public event System.Action<RecipeSO> OnRecipeDiscovered;

        private DataRegistry _registry;
        private GameConfigSO _config;
        private HashSet<string> _discoveredRecipeIds = new();

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start()
        {
            _registry = DataRegistry.Instance;
            if (_registry != null) _config = _registry.gameConfig;
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        // ============================================================
        // 编织入口
        // ============================================================

        /// <summary>尝试编织。结果：true=成功，false=失败（god-ember 仍消耗 + refund 30%）</summary>
        public bool TryWeave(RecipeSO recipe, List<ItemStack> playerInputs)
        {
            if (recipe == null)
            {
                Debug.LogWarning("[Weaving] recipe is null");
                return false;
            }
            if (_config == null)
            {
                Debug.LogError("[Weaving] GameConfig not ready");
                return false;
            }

            // 0. 日夜约束
            if (!CheckDayNightRequirement(recipe))
            {
                Debug.Log($"[Weaving] '{recipe.id}' requires {recipe.dayNight}, current phase={TimeManager.Instance?.CurrentPhase}");
                OnWeaveAttempted?.Invoke(recipe, false);
                return false;
            }

            // 1. god-ember 够吗
            if (playerGodEmber < recipe.godEmberCost)
            {
                Debug.Log($"[Weaving] Not enough god-ember: have {playerGodEmber}, need {recipe.godEmberCost}");
                OnWeaveAttempted?.Invoke(recipe, false);
                return false;
            }

            // 2. 输入够吗（按 ID + count 匹配）
            if (!CheckInputs(recipe.inputs, playerInputs))
            {
                Debug.Log($"[Weaving] Missing inputs for '{recipe.id}'");
                OnWeaveAttempted?.Invoke(recipe, false);
                return false;
            }

            // 3. 消耗 god-ember（先扣）
            playerGodEmber -= recipe.godEmberCost;

            // 4. 成功率（data-config §C.2 + systems-index §3.2 锁定 T1=100% / T5=85%）
            float successRate = GetSuccessRateForTier(recipe.tier);
            float roll = Random.value;
            bool success = roll < successRate;

            if (success)
            {
                // 5a. 成功：消耗输入
                ConsumeInputs(recipe.inputs, playerInputs);
                Debug.Log($"[Weaving] ✓ '{recipe.displayName}' (T{(int)recipe.tier}, success rate {successRate:P0}, roll {roll:F2})");
            }
            else
            {
                // 5b. 失败：god-ember 退 30%（systems-index §3.6 锁定）
                int refund = Mathf.RoundToInt(recipe.godEmberCost * _config.godEmberRefundRate);
                playerGodEmber += refund;
                Debug.Log($"[Weaving] ✗ '{recipe.displayName}' failed (roll {roll:F2} >= {successRate:P0}), refunded {refund} god-ember");
            }

            // 6. 发现（不管成败，hidden 配方被发现）
            if (recipe.isHidden && !_discoveredRecipeIds.Contains(recipe.id))
            {
                _discoveredRecipeIds.Add(recipe.id);
                OnRecipeDiscovered?.Invoke(recipe);
                Debug.Log($"[Weaving] 🔍 Hidden recipe discovered: '{recipe.displayName}'");
            }

            OnWeaveAttempted?.Invoke(recipe, success);
            return success;
        }

        // ============================================================
        // 工具方法
        // ============================================================

        public bool IsRecipeDiscovered(RecipeSO recipe)
        {
            if (recipe == null) return false;
            if (!recipe.isHidden) return true;  // 非隐藏总是"已发现"
            return _discoveredRecipeIds.Contains(recipe.id);
        }

        /// <summary>列出玩家当前可用的输入（用于 UI 显示可编织什么）</summary>
        public List<RecipeSO> GetAvailableRecipes(List<ItemStack> playerInputs, bool includeHidden = false)
        {
            var result = new List<RecipeSO>();
            if (_registry == null) return result;
            // 这里简化处理：DataRegistry 暴露全部 RecipeSO
            // 实际应通过 _registry 反射 getRecipes，但为了避免反射开销
            // 直接 for 循环 DataRegistry 的 _recipes 字段（Reflection 一次后 cache）
            var recipes = _registry.GetType()
                .GetField("_recipes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(_registry) as Dictionary<string, RecipeSO>;
            if (recipes == null) return result;

            foreach (var r in recipes.Values)
            {
                if (r == null) continue;
                if (!includeHidden && !IsRecipeDiscovered(r)) continue;
                if (r.godEmberCost > playerGodEmber) continue;
                if (!CheckInputs(r.inputs, playerInputs)) continue;
                result.Add(r);
            }
            return result;
        }

        // ============================================================
        // 内部 helper
        // ============================================================

        private bool CheckDayNightRequirement(RecipeSO recipe)
        {
            if (recipe.dayNight == DayNightRequirement.Any) return true;
            if (TimeManager.Instance == null) return true;  // 兜底
            return recipe.dayNight switch
            {
                DayNightRequirement.Day => TimeManager.Instance.IsDaytime,
                DayNightRequirement.Night => TimeManager.Instance.IsNighttime,
                _ => true
            };
        }

        private bool CheckInputs(ItemStack[] required, List<ItemStack> available)
        {
            if (required == null) return true;
            foreach (var r in required)
            {
                if (r == null || r.IsEmpty) continue;
                int have = available.Find(s => s.itemId == r.itemId)?.count ?? 0;
                if (have < r.count) return false;
            }
            return true;
        }

        private void ConsumeInputs(ItemStack[] required, List<ItemStack> available)
        {
            if (required == null) return;
            foreach (var r in required)
            {
                if (r == null || r.IsEmpty) continue;
                var s = available.Find(x => x.itemId == r.itemId);
                if (s != null) s.count -= r.count;
            }
        }

        private float GetSuccessRateForTier(RecipeTier tier)
        {
            if (_config == null) return 1f;
            return tier switch
            {
                RecipeTier.Common => _config.tier1SuccessRate,
                RecipeTier.Uncommon => _config.tier2SuccessRate,
                RecipeTier.Rare => _config.tier3SuccessRate,
                RecipeTier.Epic => _config.tier4SuccessRate,
                RecipeTier.Legendary => _config.tier5SuccessRate,
                _ => 1f
            };
        }

        /// <summary>debug：注入 god-ember</summary>
        public void AddGodEmber(int amount) => playerGodEmber = Mathf.Clamp(playerGodEmber + amount, 0, 999);
    }
}
