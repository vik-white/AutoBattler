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
                var abilityID = request.ValueRO.Ability.IsCreated ? request.ValueRO.Ability.Value.ID : 0;
                var value = GetEffectValue(ref state, levelUpConfigs, request.ValueRO.Data, provider, abilityID);
                var isCrit = false;
                if (type == EffectType.Damage)
                    value = TryApplyCrit(ref characters, ref critCounters, levelUpConfigs, provider, value, out isCrit);

                var effect = ecb.CreateEntity();
                ecb.AddComponent(effect, new Effect
                {
                    Ability = request.ValueRO.Ability,
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

        public float GetEffectValue(ref SystemState state, BlobAssetReference<BlobArrayContainer<LevelUpConfig>> levelUpConfigs, EffectData effect, Entity entity, uint abilityID)
        {
            var character = SystemAPI.GetComponent<Character>(entity);
            var config = character.GetConfig();

            var levelUpConfig = levelUpConfigs.Get(config.LevelUp);
            var starsLevelUpConfig = levelUpConfigs.Get(config.StarLevelUp);
            var skillLevelUpConfig = levelUpConfigs.Get(config.SkillLevelUp);
            var level = character.Level - 1;
            var stars = character.Stars;
            var skillLevel = character.SkillLevel - 1;

            var skillMultiplier = GetSkillMultiplier(config, abilityID, level, stars, skillLevel, levelUpConfig, starsLevelUpConfig, skillLevelUpConfig);
            var effectValue = effect.Value * skillMultiplier;

            var value = effect.Dependence switch
            {
                EffectDependenceType.Attack => (config.Attack * GetLevelUpMultiply(level, stars, skillLevel, levelUpConfig.Attack, starsLevelUpConfig.Attack, skillLevelUpConfig.Attack)) * effectValue,
                EffectDependenceType.Defense => (config.Defense * GetLevelUpMultiply(level, stars, skillLevel, levelUpConfig.Defense, starsLevelUpConfig.Defense, skillLevelUpConfig.Defense)) * effectValue,
                EffectDependenceType.Health => (config.Health * GetLevelUpMultiply(level, stars, skillLevel, levelUpConfig.Health, starsLevelUpConfig.Health, skillLevelUpConfig.Health)) * effectValue,
                EffectDependenceType.CritChance => (config.CritChance * GetLevelUpMultiply(level, stars, skillLevel, levelUpConfig.CritChance, starsLevelUpConfig.CritChance, skillLevelUpConfig.CritChance)) * effectValue,
                EffectDependenceType.CritValue => (config.CritValue * GetLevelUpMultiply(level, stars, skillLevel, levelUpConfig.CritValue, starsLevelUpConfig.CritValue, skillLevelUpConfig.CritValue)) * effectValue,
                _ => effectValue,
            };

            if (effect.Type == EffectType.Damage)
                value *= SystemAPI.GetBuffer<StatMultiply>(entity)[(int)StatType.DamageMultiply].Value;

            return value;
        }

        private float GetSkillMultiplier(in CharacterConfigData config, uint abilityID, int level, int stars, int skillLevel, in LevelUpConfig levelUpConfig, in LevelUpConfig starsLevelUpConfig, in LevelUpConfig skillLevelUpConfig)
        {
            if (abilityID == 0) return 1f;
            if (abilityID == config.SkillActive) return GetLevelUpMultiply(level, stars, skillLevel, levelUpConfig.SkillActive, starsLevelUpConfig.SkillActive, skillLevelUpConfig.SkillActive);
            if (abilityID == config.SkillPassive1) return GetLevelUpMultiply(level, stars, skillLevel, levelUpConfig.SkillPassive1, starsLevelUpConfig.SkillPassive1, skillLevelUpConfig.SkillPassive1);
            if (abilityID == config.SkillPassive2) return GetLevelUpMultiply(level, stars, skillLevel, levelUpConfig.SkillPassive2, starsLevelUpConfig.SkillPassive2, skillLevelUpConfig.SkillPassive2);
            if (abilityID == config.SkillMeta1) return GetLevelUpMultiply(level, stars, skillLevel, levelUpConfig.SkillMeta1, starsLevelUpConfig.SkillMeta1, skillLevelUpConfig.SkillMeta1);
            if (abilityID == config.SkillMeta2) return GetLevelUpMultiply(level, stars, skillLevel, levelUpConfig.SkillMeta2, starsLevelUpConfig.SkillMeta2, skillLevelUpConfig.SkillMeta2);
            if (abilityID == config.SkillMeta3) return GetLevelUpMultiply(level, stars, skillLevel, levelUpConfig.SkillMeta3, starsLevelUpConfig.SkillMeta3, skillLevelUpConfig.SkillMeta3);
            return 1f;
        }

        private float GetLevelUpMultiply(int levelIndex, int starsIndex, int skillIndex, float levelMultiply, float starsMultiply, float skillMultiply)
        {
            return 
                CharacterHandler.GetLevelMultiplier(levelIndex, levelMultiply) *
                CharacterHandler.GetLevelMultiplier(starsIndex, starsMultiply) *
                CharacterHandler.GetLevelMultiplier(skillIndex, skillMultiply);
        }

        private float TryApplyCrit(ref ComponentLookup<Character> characters, ref ComponentLookup<CritCounter> critCounters, BlobAssetReference<BlobArrayContainer<LevelUpConfig>> levelUpConfigs, Entity provider, float value, out bool isCrit)
        {
            isCrit = false;

            var character = characters[provider];
            var config = character.GetConfig();
            var levelUpConfig = levelUpConfigs.Get(config.LevelUp);
            var starsLevelUpConfig = levelUpConfigs.Get(config.StarLevelUp);
            var skillLevelUpConfig = levelUpConfigs.Get(config.SkillLevelUp);
            var level = character.Level - 1;
            var stars = character.Stars;
            var skillLevel = character.SkillLevel - 1;

            var chance = config.CritChance * GetLevelUpMultiply(level, stars, skillLevel, levelUpConfig.CritChance, starsLevelUpConfig.CritChance, skillLevelUpConfig.CritChance);
            if (chance <= 0f) return value;

            var counter = critCounters.HasComponent(provider) ? critCounters[provider].Value : 0;
            counter += 1;

            var guaranteed = counter * chance + 1e-4f >= 1f;
            isCrit = guaranteed || Random.value < chance;
            critCounters[provider] = new CritCounter { Value = isCrit ? 0 : counter };

            if (isCrit)
            {
                var critValue = config.CritValue * GetLevelUpMultiply(level, stars, skillLevel, levelUpConfig.CritValue, starsLevelUpConfig.CritValue, skillLevelUpConfig.CritValue);
                var multiplier = critValue > 0f ? critValue : 1f;
                value *= multiplier;
            }

            return value;
        }
    }
}
