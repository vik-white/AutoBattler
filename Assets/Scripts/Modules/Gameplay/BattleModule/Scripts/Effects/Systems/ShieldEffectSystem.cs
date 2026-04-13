using Unity.Entities;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(EffectsSystemGroup))]
    [UpdateAfter(typeof(CreateEffectSystem))]
    public partial struct ShieldEffectSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var shields = SystemAPI.GetComponentLookup<Shield>();
            var shieldMaxes = SystemAPI.GetComponentLookup<ShieldMax>();
            foreach (var (effect, target) in SystemAPI.Query<RefRO<Effect>, RefRO<Target>>().WithAny<ShieldEffect>())
            {
                var character = target.ValueRO.Value;
                var shield = shields[character].Value + effect.ValueRO.Value;
                
                if(shield > shieldMaxes[character].Value) 
                    shieldMaxes[character] = new ShieldMax { Value = shield };
                
                shields[character] = new Shield { Value = shield };
            }
        }
    }
}