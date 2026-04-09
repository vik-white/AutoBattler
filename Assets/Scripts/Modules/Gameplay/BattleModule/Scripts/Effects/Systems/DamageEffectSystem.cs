using Unity.Entities;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EffectsSystemGroup))]
    [UpdateAfter(typeof(CreateEffectSystem))]
    public partial struct DamageEffectSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var healths = SystemAPI.GetComponentLookup<Health>();
            foreach (var (effect, target) in SystemAPI.Query<RefRO<EffectValue>, RefRO<Target>>().WithAny<EffectDamage>()) {
                healths[target.ValueRO.Value] = new Health { Value = healths[target.ValueRO.Value].Value - effect.ValueRO.Value };
            }
        }
    }
}