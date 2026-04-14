using Unity.Burst;
using Unity.Entities;
using vikwhite.ECS;

namespace vikwhite
{
    [BurstCompile]
    [UpdateInGroup(typeof(StatusesSystemGroup), OrderFirst = true)]
    public partial struct ApplyStatusesOnTargetsSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) 
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (collisionTargets, statuses, provider) in SystemAPI.Query<DynamicBuffer<CollisionTarget>, RefRO<Statuses>, RefRO<Provider>>()) 
            {
                for (int i = 0; i < collisionTargets.Length; i++) 
                {
                    foreach (var status in statuses.ValueRO.Array) 
                    {
                        ecb.CreateFrameEntity(new CreateStatus {
                            Ability = statuses.ValueRO.Ability,
                            Provider = provider.ValueRO.Value,
                            Target = collisionTargets[i].Value, 
                            Data = status, 
                        });
                    }
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}