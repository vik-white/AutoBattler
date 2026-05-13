using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace vikwhite.ECS
{
    [BurstCompile]
    [UpdateInGroup(typeof(EffectsSystemGroup))]
    [UpdateAfter(typeof(CreateEffectSystem))]
    public partial struct AggroSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (effect, target, provider) in SystemAPI.Query<RefRO<Effect>, RefRO<Target>, RefRO<Provider>>().WithAny<AggroEffect>())
            {
                if (SystemAPI.HasComponent<Aggro>(target.ValueRO.Value))
                    ecb.SetComponent(target.ValueRO.Value, new Aggro{ Provider = provider.ValueRO.Value, TimeLeft = effect.ValueRO.Value });
                else
                    ecb.AddComponent(target.ValueRO.Value, new Aggro{ Provider = provider.ValueRO.Value, TimeLeft = effect.ValueRO.Value });
            }
            ecb.Playback(state.EntityManager);
        }
    }
}