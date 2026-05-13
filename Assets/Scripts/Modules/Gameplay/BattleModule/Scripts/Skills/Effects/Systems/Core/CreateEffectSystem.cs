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
            var critCounters = SystemAPI.GetComponentLookup<CritCounter>();
            var characters = SystemAPI.GetComponentLookup<Character>(true);
            var upgrades = SystemAPI.GetComponentLookup<CharacterUpgrade>(true);
            foreach (var request in SystemAPI.Query<RefRO<CreateEffect>>()) {
                var type = request.ValueRO.Data.Type;
                var provider = request.ValueRO.Provider;
                var skillID = request.ValueRO.Skill.IsCreated ? request.ValueRO.Skill.Value.ID : 0;
                var value = GetEffectValue(ref state, request.ValueRO.Data, provider, skillID);
                var isCrit = false;
                if (type == EffectType.Damage)
                    value = TryApplyCrit(ref characters, ref upgrades, ref critCounters, provider, value, out isCrit);

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

        public float GetEffectValue(ref SystemState state, EffectData effect, Entity entity, uint skillID)
        {
            var config = SystemAPI.GetComponent<Character>(entity).GetConfig();
            var upgrade = SystemAPI.GetComponent<CharacterUpgrade>(entity);

            var effectValue = effect.Value * upgrade.GetEffectMultiplier(config, skillID);

            if (!config.TryGetStat(effect.Stat, out var baseStat)) return effectValue;
            return baseStat * upgrade.GetStatMultiplier(effect.Stat) * effectValue;
        }

        private float TryApplyCrit(ref ComponentLookup<Character> characters, ref ComponentLookup<CharacterUpgrade> upgrades, ref ComponentLookup<CritCounter> critCounters, Entity provider, float value, out bool isCrit)
        {
            isCrit = false;

            var config = characters[provider].GetConfig();
            var upgrade = upgrades[provider];

            var chance = config.CritChance * upgrade.GetStatMultiplier(StatType.CritChance);
            if (chance <= 0f) return value;

            var counter = critCounters.HasComponent(provider) ? critCounters[provider].Value : 0;
            counter += 1;

            var guaranteed = counter * chance + 1e-4f >= 1f;
            isCrit = guaranteed || Random.value < chance;
            critCounters[provider] = new CritCounter { Value = isCrit ? 0 : counter };

            if (isCrit)
            {
                var critValue = config.CritValue * upgrade.GetStatMultiplier(StatType.CritValue);
                var multiplier = critValue > 0f ? critValue : 1f;
                value *= multiplier;
            }

            return value;
        }
    }
}
