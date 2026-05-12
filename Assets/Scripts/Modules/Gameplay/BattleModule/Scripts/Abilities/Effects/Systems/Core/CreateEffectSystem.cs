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
                var value = GetEffectValue(ref state, levelUpConfigs, request.ValueRO.Data, provider, request.ValueRO.Ability.Value.Skill);
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

        public float GetEffectValue(ref SystemState state, BlobAssetReference<BlobArrayContainer<LevelUpConfig>> levelUpConfigs, EffectData effect, Entity entity, bool isSkill)
        {
            var character = SystemAPI.GetComponent<Character>(entity);
            var config = character.GetConfig();
            var value = effect.Dependence switch
            {
                EffectDependenceType.Attack => config.Attack * effect.Value,
                EffectDependenceType.Defense => config.Defense * effect.Value,
                _ => effect.Value,
            };

            value *= isSkill
                ? GetSkillScale(character.SkillLevel - 1, effect.Type, levelUpConfigs.Get(config.SkillLevelUp))
                : GetLevelStarsScale(character.Level - 1, character.Stars, effect.Type, levelUpConfigs.Get(config.LevelUp), levelUpConfigs.Get(config.StarLevelUp));

            if (effect.Type == EffectType.Damage)
                value *= SystemAPI.GetBuffer<StatMultiply>(entity)[(int)StatType.DamageMultiply].Value;

            return value;
        }

        private static float TryApplyCrit(ref ComponentLookup<Character> characters, ref ComponentLookup<CritCounter> critCounters, Entity provider, float value, out bool isCrit)
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

        private static float GetSkillScale(int skillIndex, EffectType type, LevelUpConfig skill)
        {
            switch (type)
            {
                case EffectType.Damage: return CharacterHandler.GetLevelMultiplier(skillIndex, skill.Damage);
                case EffectType.Heal:   return CharacterHandler.GetLevelMultiplier(skillIndex, skill.Heal);
                case EffectType.Shield: return CharacterHandler.GetLevelMultiplier(skillIndex, skill.Shield);
                default: return 1f;
            }
        }

        private static float GetLevelStarsScale(int levelIndex, int starsIndex, EffectType type, LevelUpConfig level, LevelUpConfig stars)
        {
            switch (type)
            {
                case EffectType.Damage: return CharacterHandler.GetLevelMultiplier(levelIndex, level.Damage) * CharacterHandler.GetLevelMultiplier(starsIndex, stars.Damage);
                case EffectType.Heal:   return CharacterHandler.GetLevelMultiplier(levelIndex, level.Heal)   * CharacterHandler.GetLevelMultiplier(starsIndex, stars.Heal);
                case EffectType.Shield: return CharacterHandler.GetLevelMultiplier(levelIndex, level.Shield) * CharacterHandler.GetLevelMultiplier(starsIndex, stars.Shield);
                default: return 1f;
            }
        }
    }
}
