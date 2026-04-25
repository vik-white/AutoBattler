using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

namespace vikwhite.ECS
{
    [BurstCompile]
    [UpdateInGroup(typeof(CleanupSystemGroup))]
    public partial struct AggroCleanupSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<Time>()) return;

            var dt = SystemAPI.GetSingleton<Time>().DeltaTime;
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (aggro, entity) in SystemAPI.Query<RefRW<Aggro>>().WithEntityAccess())
            {
                if (aggro.ValueRO.TimeLeft >= 0)
                    aggro.ValueRW.TimeLeft -= dt;
                else
                {
                    ecb.RemoveComponent<Aggro>(entity);
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
