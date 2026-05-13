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
                var skillID = request.ValueRO.Skill.IsCreated ? request.ValueRO.Skill.Value.ID : 0;
                var value = GetEffectValue(ref state, levelUpConfigs, request.ValueRO.Data, provider, skillID);
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

        public float GetEffectValue(ref SystemState state, BlobAssetReference<BlobArrayContainer<UpgradeConfig>> levelUpConfigs, EffectData effect, Entity entity, uint skillID)
        {
            var character = SystemAPI.GetComponent<Character>(entity);
            var config = character.GetConfig();

            var levelUpConfig = levelUpConfigs.Get(config.LevelUpgrade);
            var starsLevelUpConfig = levelUpConfigs.Get(config.StarUpgrade);
            var skillLevelUpConfig = levelUpConfigs.Get(config.SkillUpgrade);
            var level = character.Level - 1;
            var stars = character.Stars;
            var skillLevel = character.SkillLevel - 1;

            var skillMultiplier = GetSkillMultiplier(config, skillID, level, stars, skillLevel, levelUpConfig, starsLevelUpConfig, skillLevelUpConfig);
            var effectValue = effect.Value * skillMultiplier;

            if (!config.TryGetStat(effect.Stat, out var baseStat)) return effectValue;

            var statMultiplier = CharacterUpgradeExtensions.GetStatMultiplier(level, stars, skillLevel, effect.Stat,
                levelUpConfig, starsLevelUpConfig, skillLevelUpConfig);
            return baseStat * statMultiplier * effectValue;
        }

        private float GetSkillMultiplier(in CharacterConfigData config, uint skillID, int level, int stars, int skillLevel, in UpgradeConfig levelUp, in UpgradeConfig starUp, in UpgradeConfig skillUp)
        {
            if (skillID == 0) return 1f;
            if (!config.TryFindSlot(skillID, out var slot) || slot == SkillSlotType.Attack) return 1f;
            return CharacterUpgradeExtensions.GetSkillMultiplier(level, stars, skillLevel, slot, levelUp, starUp, skillUp);
        }

        private float TryApplyCrit(ref ComponentLookup<Character> characters, ref ComponentLookup<CritCounter> critCounters, BlobAssetReference<BlobArrayContainer<UpgradeConfig>> levelUpConfigs, Entity provider, float value, out bool isCrit)
        {
            isCrit = false;

            var character = characters[provider];
            var config = character.GetConfig();
            var levelUpConfig = levelUpConfigs.Get(config.LevelUpgrade);
            var starsLevelUpConfig = levelUpConfigs.Get(config.StarUpgrade);
            var skillLevelUpConfig = levelUpConfigs.Get(config.SkillUpgrade);
            var level = character.Level - 1;
            var stars = character.Stars;
            var skillLevel = character.SkillLevel - 1;

            var chance = config.CritChance * CharacterUpgradeExtensions.GetStatMultiplier(level, stars, skillLevel, StatType.CritChance,
                levelUpConfig, starsLevelUpConfig, skillLevelUpConfig);
            if (chance <= 0f) return value;

            var counter = critCounters.HasComponent(provider) ? critCounters[provider].Value : 0;
            counter += 1;

            var guaranteed = counter * chance + 1e-4f >= 1f;
            isCrit = guaranteed || Random.value < chance;
            critCounters[provider] = new CritCounter { Value = isCrit ? 0 : counter };

            if (isCrit)
            {
                var critValue = config.CritValue * CharacterUpgradeExtensions.GetStatMultiplier(level, stars, skillLevel, StatType.CritValue,
                    levelUpConfig, starsLevelUpConfig, skillLevelUpConfig);
                var multiplier = critValue > 0f ? critValue : 1f;
                value *= multiplier;
            }

            return value;
        }
    }
}
