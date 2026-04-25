using Unity.Burst;
using Unity.Entities;
using vikwhite.ECS;

namespace vikwhite
{
    [BurstCompile]
    [UpdateInGroup(typeof(EffectsSystemGroup), OrderFirst = true)]
    public partial struct ApplyStatOnTargetsSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) 
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (collisionTargets, stats, provider) in SystemAPI.Query<DynamicBuffer<CollisionTarget>, RefRO<Stats>, RefRO<Provider>>()) 
            {
                var ability = stats.ValueRO.Ability.Value;
                for (int i = 0; i < collisionTargets.Length; i++) 
                {
                    foreach (var stat in ability.Stats) 
                    {
                        ecb.CreateFrameEntity(new CreateStatChange {
                            Ability = stats.ValueRO.Ability,
                            Target = collisionTargets[i].Value, 
                            Data = stat, 
                            Provider = provider.ValueRO.Value
                        });
                    }
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
