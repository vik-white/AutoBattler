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
            var statBuffers = SystemAPI.GetBufferLookup<StatMultiply>(true);
            foreach (var request in SystemAPI.Query<RefRO<CreateEffect>>()) {
                var type = request.ValueRO.Data.Type;
                var provider = request.ValueRO.Provider;
                var skillID = request.ValueRO.Skill.IsCreated ? request.ValueRO.Skill.Value.ID : 0;
                var value = GetEffectValue(ref state, request.ValueRO.Data, provider, skillID);
                var isCrit = false;
                if (type == EffectType.Damage)
                    value = TryApplyCrit(ref characters, ref statBuffers, ref critCounters, provider, value, out isCrit);

                var effect = ecb.CreateEntity();
                ecb.AddComponent(effect, new Effect
                {
                    Skill = request.ValueRO.Skill,
                    Value = value,
                    IsCrit = isCrit
                });
                ecb.AddComponent(effect, new Target{ Value = request.ValueRO.Target });
                ecb.AddComponent(effect, new Provider{ Value = request.ValueRO.Provider });
                ecb.CreateEffectEvent(request.ValueRO.Skill, request.ValueRO.Target, request.ValueRO.Provider);

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

            var effectValue = effect.Value * upgrade.GetSkillMultiplier(config, skillID);

            if (!config.TryGetStat(effect.Stat, out var baseStat)) return effectValue;
            var statBuffer = SystemAPI.GetBuffer<StatMultiply>(entity);
            return baseStat * statBuffer[(int)effect.Stat].Value * effectValue;
        }

        private float TryApplyCrit(ref ComponentLookup<Character> characters, ref BufferLookup<StatMultiply> statBuffers, ref ComponentLookup<CritCounter> critCounters, Entity provider, float value, out bool isCrit)
        {
            isCrit = false;

            var config = characters[provider].GetConfig();
            var statBuffer = statBuffers[provider];

            var chance = config.CritChance * statBuffer[(int)StatType.CritChance].Value;
            if (chance <= 0f) return value;

            var counter = critCounters.HasComponent(provider) ? critCounters[provider].Value : 0;
            counter += 1;

            var guaranteed = counter * chance + 1e-4f >= 1f;
            isCrit = guaranteed || Random.value < chance;
            critCounters[provider] = new CritCounter { Value = isCrit ? 0 : counter };

            if (isCrit)
            {
                var critValue = config.CritValue * statBuffer[(int)StatType.CritValue].Value;
                var multiplier = critValue > 0f ? critValue : 1f;
                value *= multiplier;
            }

            return value;
        }
    }
}
