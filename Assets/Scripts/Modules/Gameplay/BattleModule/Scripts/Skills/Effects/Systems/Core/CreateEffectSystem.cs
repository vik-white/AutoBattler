using Unity.Entities;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EffectsSystemGroup))]
    [UpdateAfter(typeof(CreateEffectImpulseSystem))]
    public partial struct CreateEffectSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var levelUpConfigs = SystemAPI.GetSingleton<LevelUpConfigsBlob>().Value;
            var critCounters = SystemAPI.GetComponentLookup<CritCounter>();
            var characters = SystemAPI.GetComponentLookup<Character>(true);
            foreach (var request in SystemAPI.Query<RefRO<CreateEffect>>()) {
                var type = request.ValueRO.Data.Type;
                var provider = request.ValueRO.Provider;
                var abilityID = request.ValueRO.Skill.IsCreated ? request.ValueRO.Skill.Value.ID : 0;
                var value = GetEffectValue(ref state, levelUpConfigs, request.ValueRO.Data, provider, abilityID);
                var isCrit = false;
                if (type == EffectType.Damage)
                    value = TryApplyCrit(ref characters, ref critCounters, levelUpConfigs, provider, value, out isCrit);

                var effect = ecb.CreateEntity();
                ecb.AddComponent(effect, new Effect
                {
                    Ability = request.ValueRO.Skill,
                    Value = value,
                    IsCrit = isCrit
                });
                ecb.AddComponent(effect, new Target{ Value = request.ValueRO.Target });
                ecb.AddComponent(effect, new Provider{ Value = request.ValueRO.Provider });
                
                if (type == EffectType.Damage) ecb.AddComponent<DamageEffect>(effect);
                if (type == EffectType.Heal) ecb.AddComponent<HealEffect>(effect);
                if (type == EffectType.Shield) ecb.AddComponent<ShieldEffect>(effect);
                if (type == EffectType.Spawn) ecb.AddComponent<SpawnEffect>(effect);
                if (type == EffectType.Aggro) ecb.AddComponent<AggroEffect>(effect);
            }
            ecb.Playback(state.EntityManager);
        }

        public float GetEffectValue(ref SystemState state, BlobAssetReference<BlobArrayContainer<UpgradeConfig>> levelUpConfigs, EffectData effect, Entity entity, uint abilityID)
        {
            var character = SystemAPI.GetComponent<Character>(entity);
            var config = character.GetConfig();

            var levelUpConfig = levelUpConfigs.Get(config.LevelUpgrade);
            var starsLevelUpConfig = levelUpConfigs.Get(config.StarUpgrade);
            var skillLevelUpConfig = levelUpConfigs.Get(config.SkillUpgrade);
            var level = character.Level - 1;
            var stars = character.Stars;
            var skillLevel = character.SkillLevel - 1;

            var skillMultiplier = GetSkillMultiplier(config, abilityID, level, stars, skillLevel, levelUpConfig, starsLevelUpConfig, skillLevelUpConfig);
            var effectValue = effect.Value * skillMultiplier;

            var value = effect.Stat switch
            {
                StatType.Attack => (config.Attack * GetLevelUpMultiply(level, stars, skillLevel, levelUpConfig.Attack, starsLevelUpConfig.Attack, skillLevelUpConfig.Attack)) * effectValue,
                StatType.Defense => (config.Defense * GetLevelUpMultiply(level, stars, skillLevel, levelUpConfig.Defense, starsLevelUpConfig.Defense, skillLevelUpConfig.Defense)) * effectValue,
                StatType.Health => (config.Health * GetLevelUpMultiply(level, stars, skillLevel, levelUpConfig.Health, starsLevelUpConfig.Health, skillLevelUpConfig.Health)) * effectValue,
                StatType.CritChance => (config.CritChance * GetLevelUpMultiply(level, stars, skillLevel, levelUpConfig.CritChance, starsLevelUpConfig.CritChance, skillLevelUpConfig.CritChance)) * effectValue,
                StatType.CritValue => (config.CritValue * GetLevelUpMultiply(level, stars, skillLevel, levelUpConfig.CritValue, starsLevelUpConfig.CritValue, skillLevelUpConfig.CritValue)) * effectValue,
                _ => effectValue,
            };

            return value;
        }

        private float GetSkillMultiplier(in CharacterConfigData config, uint abilityID, int level, int stars, int skillLevel, in UpgradeConfig upgradeConfig, in UpgradeConfig starsUpgradeConfig, in UpgradeConfig skillUpgradeConfig)
        {
            if (abilityID == 0) return 1f;
            if (abilityID == config.SkillActive) return GetLevelUpMultiply(level, stars, skillLevel, upgradeConfig.SkillActive, starsUpgradeConfig.SkillActive, skillUpgradeConfig.SkillActive);
            if (abilityID == config.SkillPassive1) return GetLevelUpMultiply(level, stars, skillLevel, upgradeConfig.SkillPassive1, starsUpgradeConfig.SkillPassive1, skillUpgradeConfig.SkillPassive1);
            if (abilityID == config.SkillPassive2) return GetLevelUpMultiply(level, stars, skillLevel, upgradeConfig.SkillPassive2, starsUpgradeConfig.SkillPassive2, skillUpgradeConfig.SkillPassive2);
            if (abilityID == config.SkillMeta1) return GetLevelUpMultiply(level, stars, skillLevel, upgradeConfig.SkillMeta1, starsUpgradeConfig.SkillMeta1, skillUpgradeConfig.SkillMeta1);
            if (abilityID == config.SkillMeta2) return GetLevelUpMultiply(level, stars, skillLevel, upgradeConfig.SkillMeta2, starsUpgradeConfig.SkillMeta2, skillUpgradeConfig.SkillMeta2);
            if (abilityID == config.SkillMeta3) return GetLevelUpMultiply(level, stars, skillLevel, upgradeConfig.SkillMeta3, starsUpgradeConfig.SkillMeta3, skillUpgradeConfig.SkillMeta3);
            return 1f;
        }

        private float GetLevelUpMultiply(int levelIndex, int starsIndex, int skillIndex, float levelMultiply, float starsMultiply, float skillMultiply)
        {
            return 
                CharacterHandler.GetLevelMultiplier(levelIndex, levelMultiply) *
                CharacterHandler.GetLevelMultiplier(starsIndex, starsMultiply) *
                CharacterHandler.GetLevelMultiplier(skillIndex, skillMultiply);
        }

        private float TryApplyCrit(ref ComponentLookup<Character> characters, ref ComponentLookup<CritCounter> critCounters, BlobAssetReference<BlobArrayContainer<UpgradeConfig>> levelUpConfigs, Entity provider, float value, out bool isCrit)
        {
            isCrit = false;

            var character = characters[provider];
            var config = character.GetConfig();
            var levelUpgradeConfig = levelUpConfigs.Get(config.LevelUpgrade);
            var starUpgradeConfig = levelUpConfigs.Get(config.StarUpgrade);
            var skillUpgradeConfig = levelUpConfigs.Get(config.SkillUpgrade);
            var level = character.Level - 1;
            var stars = character.Stars;
            var skillLevel = character.SkillLevel - 1;

            var chance = config.CritChance * GetLevelUpMultiply(level, stars, skillLevel, levelUpgradeConfig.CritChance, starUpgradeConfig.CritChance, skillUpgradeConfig.CritChance);
            if (chance <= 0f) return value;

            var counter = critCounters.HasComponent(provider) ? critCounters[provider].Value : 0;
            counter += 1;

            var guaranteed = counter * chance + 1e-4f >= 1f;
            isCrit = guaranteed || Random.value < chance;
            critCounters[provider] = new CritCounter { Value = isCrit ? 0 : counter };

            if (isCrit)
            {
                var critValue = config.CritValue * GetLevelUpMultiply(level, stars, skillLevel, levelUpgradeConfig.CritValue, starUpgradeConfig.CritValue, skillUpgradeConfig.CritValue);
                var multiplier = critValue > 0f ? critValue : 1f;
                value *= multiplier;
            }

            return value;
        }
    }
}
