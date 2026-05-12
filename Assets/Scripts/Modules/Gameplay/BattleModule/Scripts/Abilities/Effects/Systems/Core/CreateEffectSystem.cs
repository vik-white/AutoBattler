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
                var value = GetEffectValue(ref state, levelUpConfigs, request.ValueRO.Data, provider);
                var isCrit = false;
                if (type == EffectType.Damage)
                    value = TryApplyCrit(ref characters, ref critCounters, provider, value, out isCrit);

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

        public float GetEffectValue(ref SystemState state, BlobAssetReference<BlobArrayContainer<LevelUpConfig>> levelUpConfigs, EffectData effect, Entity entity)
        {
            var character = SystemAPI.GetComponent<Character>(entity);
            var config = character.GetConfig();

            var levelUpConfig = levelUpConfigs.Get(config.LevelUp);
            var starsLevelUpConfig = levelUpConfigs.Get(config.StarLevelUp);
            var skillLevelUpConfig = levelUpConfigs.Get(config.SkillLevelUp);
            var level = character.Level - 1;
            var stars = character.Stars;
            var skillLevel = character.SkillLevel - 1;
            
            var value = effect.Dependence switch
            {
                EffectDependenceType.Attack => (config.Attack * GetLevelUpMultiply(level, stars, skillLevel, levelUpConfig.Attack, starsLevelUpConfig.Attack, skillLevelUpConfig.Attack)) * effect.Value,
                EffectDependenceType.Defense => (config.Defense * GetLevelUpMultiply(level, stars, skillLevel, levelUpConfig.Defense, starsLevelUpConfig.Defense, skillLevelUpConfig.Defense)) * effect.Value,
                EffectDependenceType.Health => (config.Health * GetLevelUpMultiply(level, stars, skillLevel, levelUpConfig.Health, starsLevelUpConfig.Health, skillLevelUpConfig.Health)) * effect.Value,
                EffectDependenceType.CritChance => (config.CritChance * GetLevelUpMultiply(level, stars, skillLevel, levelUpConfig.CritChance, starsLevelUpConfig.CritChance, skillLevelUpConfig.CritChance)) * effect.Value,
                EffectDependenceType.CritValue => (config.CritValue * GetLevelUpMultiply(level, stars, skillLevel, levelUpConfig.CritValue, starsLevelUpConfig.CritValue, skillLevelUpConfig.CritValue)) * effect.Value,
                _ => effect.Value,
            };

            if (effect.Type == EffectType.Damage)
                value *= SystemAPI.GetBuffer<StatMultiply>(entity)[(int)StatType.DamageMultiply].Value;

            return value;
        }

        private float GetLevelUpMultiply(int levelIndex, int starsIndex, int skillIndex, float levelMultiply, float starsMultiply, float skillMultiply)
        {
            return 
                CharacterHandler.GetLevelMultiplier(levelIndex, levelMultiply) *
                CharacterHandler.GetLevelMultiplier(starsIndex, starsMultiply) *
                CharacterHandler.GetLevelMultiplier(skillIndex, skillMultiply);
        }

        private float TryApplyCrit(ref ComponentLookup<Character> characters, ref ComponentLookup<CritCounter> critCounters, Entity provider, float value, out bool isCrit)
        {
            isCrit = false;
            
            var config = characters[provider].GetConfig();
            var chance = config.CritChance;
            if (chance <= 0f) return value;

            var counter = critCounters.HasComponent(provider) ? critCounters[provider].Value : 0;
            counter += 1;

            var guaranteed = counter * chance + 1e-4f >= 1f;
            isCrit = guaranteed || Random.value < chance;
            critCounters[provider] = new CritCounter { Value = isCrit ? 0 : counter };

            if (isCrit)
            {
                var multiplier = config.CritValue > 0f ? config.CritValue : 1f;
                value *= multiplier;
            }

            return value;
        }
    }
}
