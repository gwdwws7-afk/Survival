#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using Ravensong.Data;

namespace Ravensong.EditorTools
{
    /// <summary>
    /// Ravensong 一键数据初始化（dev-plan §4.5）。
    /// 菜单 Tools → Ravensong → Bootstrap Data
    /// 一次性创建 1 个 GameConfig.asset + 17 个占位 SO（每个类型各 1 个，方便 DataRegistry 启动不报缺）。
    /// </summary>
    public static class RavensongSetupWizard
    {
        private const string ROOT = "Assets/Data";

        [MenuItem("Tools/Ravensong/Bootstrap Data", priority = 100)]
        public static void Bootstrap()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Data"))
                AssetDatabase.CreateFolder("Assets", "Data");

            EnsureFolder("Assets/Data/Recipes");
            EnsureFolder("Assets/Data/Items");
            EnsureFolder("Assets/Data/Einherjars");
            EnsureFolder("Assets/Data/Biomes");
            EnsureFolder("Assets/Data/Oaths");
            EnsureFolder("Assets/Data/Bosses");
            EnsureFolder("Assets/Data/WorldEvents");
            EnsureFolder("Assets/Data/Dialogues");
            EnsureFolder("Assets/Data/Sendoffs");
            EnsureFolder("Assets/Data/UIStyles");
            EnsureFolder("Assets/Data/Expeditions");
            EnsureFolder("Assets/Data/Quests");
            EnsureFolder("Assets/Data/VFX");
            EnsureFolder("Assets/Data/Audio");

            // 各类型 1 个占位
            CreateOrUpdate<RecipeSO>("Assets/Data/Recipes/Recipe_placeholder", "recipe_placeholder", "占位配方", r => {
                r.inputs = new[] { new ItemStack("item_ash_branch", 2) };
                r.output = new ItemStack("item_famine_bow", 1);
                r.tier = RecipeTier.Common;
                r.godEmberCost = 5;
            });

            CreateOrUpdate<ItemSO>("Assets/Data/Items/Item_ash_branch", "item_ash_branch", "白桦枝", i => {
                i.category = ItemCategory.Resource;
                i.stackable = true;
                i.maxStack = 99;
                i.value = 1;
            });

            CreateOrUpdate<EinherjarSO>("Assets/Data/Einherjars/Einherjar_eirik", "einherjar_eirik_blacksmith", "Eirik 铁匠", e => {
                e.profession = Profession.Blacksmith;
                e.workType = ResourceType.Iron;
                e.workEfficiency = 1.2f;
                e.willDie = true;
                e.daysToDeath = 7;
            });

            CreateOrUpdate<BiomeSO>("Assets/Data/Biomes/Biome_birch_grove", "biome_birch_grove", "白桦林", b => {
                b.biomeId = BiomeId.BirchGrove;
                b.difficultyFactor = 0.8f;
            });

            CreateOrUpdate<OathSO>("Assets/Data/Oaths/Oath_forge", "oath_forge", "锻冶之誓", o => {
                o.oathType = OathType.Forge;
                o.milestones = new OathMilestone[4];
                for (int i = 0; i < 4; i++)
                    o.milestones[i] = new OathMilestone
                    {
                        milestoneId = $"weave_t{i + 1}_count",
                        displayName = $"编织 T{i + 1} 配方 10 次",
                        target = 10
                    };
            });

            CreateOrUpdate<BossSO>("Assets/Data/Bosses/Boss_draugrlord", "boss_draugrlord", "白桦林古龙 Draugrlord", b => {
                b.biomeId = "biome_birch_grove";
                b.biomeHint = BiomeId.BirchGrove;
                b.maxHP = 2000;
            });

            CreateOrUpdate<WorldEventSO>("Assets/Data/WorldEvents/Event_raven_omen", "event_raven_omen", "乌鸦占卜", e => {
                e.triggerCondition = "随机 0.05/小时";
                e.triggerChance = 0.05f;
                e.durationHours = 1;
            });

            CreateOrUpdate<DialogueSO>("Assets/Data/Dialogues/Dialogue_eirik_greeting", "dialogue_eirik_greeting", "Eirik 招募对白", d => {
                d.speakerId = "einherjar_eirik_blacksmith";
                d.lines = new[] { new DialogueLine { text = "我闻到了锻炉的味道…你是女武神？", displayDuration = 4f } };
            });

            CreateOrUpdate<SendoffSO>("Assets/Data/Sendoffs/Sendoff_valhalla", "sendoff_valhalla", "送往英灵殿", s => {
                s.sendoffType = SendoffType.Valhalla;
                s.buffId = "buff_valhalla_blessing";
                s.settlementMoralePenalty = 0.2f;
                s.moralePenaltyHours = 24;
            });

            CreateOrUpdate<UIStyleSO>("Assets/Data/UIStyles/UIStyle_ravensong", "uistyle_ravensong", "Ravensong 主 UI 风格", u => {
                // color 已用 SO 默认（style-bible 锁定值）
            });

            CreateOrUpdate<ExpeditionSO>("Assets/Data/Expeditions/Expedition_patrol", "expedition_patrol", "夜间巡逻", e => {
                e.expeditionType = "patrol";
                e.biomeId = "biome_birch_grove";
                e.baseRisk = 0.3f;
                e.einherjarRecruitChance = 0.2f;
                e.durationHours = 2f;
            });

            CreateOrUpdate<QuestSO>("Assets/Data/Quests/Quest_first_weave", "quest_first_weave", "第一次编织", q => {
                q.questType = "main";
                q.objectives = new[] { new QuestObjective { objectiveId = "weave", targetId = "recipe_placeholder", requiredCount = 1 } };
            });

            CreateOrUpdate<VFXPresetSO>("Assets/Data/VFX/VFX_weave_burst", "vfx_weave_burst", "编织爆发", v => {
                v.duration = 0.5f;
                v.maxParticles = 80;
            });

            CreateOrUpdate<AudioPresetSO>("Assets/Data/Audio/Audio_weave_ding", "audio_weave_ding", "编织叮", a => {
                a.category = "sfx_weave";
                a.volume = 0.6f;
            });

            // GameConfig（核心）
            CreateOrUpdate<GameConfigSO>("Assets/Data/GameConfig/GameConfig", "gameconfig_main", "Ravensong 全局配置", g => {
                // 全部使用 SO 默认值（data-config v2.5）
            });

            // Settlement
            CreateOrUpdate<SettlementSO>("Assets/Data/Settlement_main", "settlement_main", "主聚落（家）", s => {
                s.maxEinherjarCapacity = 4;
                s.currentLevel = 1;
                s.buildings = new[] { new SettlementBuilding { buildingId = "long_house", displayName = "长屋", level = 1, gridPosition = new Vector2(0, 0) } };
            });

            // BossDetail（按 boss.id 索引）
            CreateOrUpdate<BossDetailSO>("Assets/Data/Bosses/BossDetail_draugrlord", "boss_draugrlord", "Draugrlord 阶段", d => {
                d.phases = new[] {
                    new BossPhase { phaseName = "Phase 1", damageMultiplier = 1f, speedMultiplier = 1f },
                    new BossPhase { phaseName = "Phase 2 (< 66% HP)", damageMultiplier = 1.2f, speedMultiplier = 1.1f },
                    new BossPhase { phaseName = "Phase 3 (< 33% HP)", damageMultiplier = 1.5f, speedMultiplier = 1.3f }
                };
            });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Ravensong Wizard] ✓ Bootstrap done. 18 .asset created.");
            EditorUtility.DisplayDialog("Ravensong Setup",
                "✓ 已创建 18 个占位 .asset。\n\n下一步：\n1. 创建一个空 GameObject 挂 DataRegistry + GameManager + TimeManager\n2. 拖 GameConfig 资产到 DataRegistry 的 GameConfig 字段\n3. Play 验证编译", "OK");
        }

        // ============================================================
        // Helpers
        // ============================================================

        private static void EnsureFolder(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                var parent = Path.GetDirectoryName(path).Replace('\\', '/');
                var leaf = Path.GetFileName(path);
                AssetDatabase.CreateFolder(parent, leaf);
            }
        }

        private static T CreateOrUpdate<T>(string path, string id, string displayName, System.Action<T> customize) where T : ScriptableObject
        {
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null)
            {
                Debug.Log($"[Ravensong Wizard] {path} already exists, skip");
                return existing;
            }
            var asset = ScriptableObject.CreateInstance<T>();
            customize?.Invoke(asset);
            // 反射设 id / displayName（用统一接口 IDataValidatable）
            var idField = typeof(T).GetField("id");
            idField?.SetValue(asset, id);
            var nameField = typeof(T).GetField("displayName");
            nameField?.SetValue(asset, displayName);
            AssetDatabase.CreateAsset(asset, path + ".asset");
            Debug.Log($"[Ravensong Wizard] ✓ Created {path}.asset (id={id})");
            return asset;
        }

        [MenuItem("Tools/Ravensong/Setup Scene (Managers + DataRegistry)", priority = 110)]
        public static void SetupScene()
        {
            // 找现有或新建 GameObject
            var go = GameObject.Find("[RavensongCore]");
            if (go == null) go = new GameObject("[RavensongCore]");

            if (go.GetComponent<DataRegistry>() == null) go.AddComponent<DataRegistry>();
            if (go.GetComponent<GameManager>() == null) go.AddComponent<GameManager>();
            if (go.GetComponent<TimeManager>() == null) go.AddComponent<TimeManager>();
            if (go.GetComponent<DayNight.DayNightCycle>() == null) go.AddComponent<DayNight.DayNightCycle>();
            if (go.GetComponent<FateThread.WeavingSystem>() == null) go.AddComponent<FateThread.WeavingSystem>();
            if (go.GetComponent<Einherjar.EinherjarManager>() == null) go.AddComponent<Einherjar.EinherjarManager>();
            if (go.GetComponent<Oath.OathManager>() == null) go.AddComponent<Oath.OathManager>();
            if (go.GetComponent<DeathSendoff.SendoffManager>() == null) go.AddComponent<DeathSendoff.SendoffManager>();
            if (go.GetComponent<Settlement.SettlementManager>() == null) go.AddComponent<Settlement.SettlementManager>();

            // 自动绑 GameConfig
            var dr = go.GetComponent<DataRegistry>();
            if (dr.gameConfig == null)
            {
                var gc = AssetDatabase.LoadAssetAtPath<GameConfigSO>("Assets/Data/GameConfig/GameConfig.asset");
                if (gc != null)
                {
                    dr.gameConfig = gc;
                    EditorUtility.SetDirty(dr);
                    Debug.Log("[Ravensong Wizard] ✓ Auto-bound GameConfig to DataRegistry");
                }
                else
                {
                    Debug.LogWarning("[Ravensong Wizard] GameConfig.asset not found, run Bootstrap first");
                }
            }

            Selection.activeGameObject = go;
            EditorUtility.DisplayDialog("Ravensong Setup",
                "✓ [RavensongCore] GameObject 配置完成。\n\n包含 9 个 Manager 组件。", "OK");
        }
    }
}
#endif
