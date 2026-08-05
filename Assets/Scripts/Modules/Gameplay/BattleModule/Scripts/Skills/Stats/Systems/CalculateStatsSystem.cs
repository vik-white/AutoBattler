using System;
using Unity.Collections;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup))]
    public partial struct CalculateStatsSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) 
        {
            foreach (var (character, upgrade, entity) in SystemAPI.Query<RefRO<Character>, RefRO<CharacterUpgrade>>().WithEntityAccess())
            {
                var stats = SystemAPI.GetBuffer<StatMultiply>(entity);
                var statBases = SystemAPI.GetBuffer<StatBase>(entity);
                var strongestPositive = new FixedList128Bytes<float>();
                var strongestNegative = new FixedList128Bytes<float>();
                for (int i = 0; i < stats.Length; i++)
                {
                    strongestPositive.Add(1f);
                    strongestNegative.Add(1f);
                }
                
                foreach (var change in SystemAPI.Query<RefRO<StatChange>>()) {
                    if (change.ValueRO.Target == entity)
                    {
                        var id = (int)change.ValueRO.Type;
                        var value = change.ValueRO.Value;
                        if (value > 1f && IsStrongerMultiplier(value, strongestPositive[id]))
                            strongestPositive[id] = value;
                        else if (value < 1f && IsStrongerMultiplier(value, strongestNegative[id]))
                            strongestNegative[id] = value;
                    }
                }

                for (int i = 0; i < stats.Length; i++)
                    stats[i] = new StatMultiply
                    {
                        Value = statBases[i].Value *
                                character.ValueRO.GetAppliedUpgradeMultiplier(upgrade.ValueRO, (StatType)i) *
                                GetCombinedMultiplier(strongestPositive[i], strongestNegative[i])
                    };
            }
        }

        private static bool IsStrongerMultiplier(float candidate, float current) =>
            Math.Abs(candidate - 1f) > Math.Abs(current - 1f);

        private static float GetCombinedMultiplier(float positive, float negative)
        {
            var hasPositive = positive > 1f;
            var hasNegative = negative < 1f;
            if (hasPositive && hasNegative)
                return (positive + negative) * 0.5f;

            return hasPositive ? positive : negative;
        }
    }
}
