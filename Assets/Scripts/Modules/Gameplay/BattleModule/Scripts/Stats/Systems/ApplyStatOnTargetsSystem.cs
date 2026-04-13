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
            foreach (var (collisionTargets, stats) in SystemAPI.Query<DynamicBuffer<CollisionTarget>, RefRO<Stats>>()) 
            {
                for (int i = 0; i < collisionTargets.Length; i++) 
                {
                    foreach (var stat in stats.ValueRO.Array) 
                    {
                        ecb.CreateFrameEntity(new CreateStatChange {
                            Target = collisionTargets[i].Value, 
                            Data = stat, 
                        });
                    }
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}