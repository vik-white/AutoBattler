using Unity.Burst;
using Unity.Entities;
using vikwhite.ECS;

namespace vikwhite
{
    [BurstCompile]
    [UpdateInGroup(typeof(StatusesSystemGroup))]
    [UpdateAfter(typeof(CreateStatusSystem))]
    public partial struct StatusDurationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<Time>()) return;

            var dt = SystemAPI.GetSingleton<Time>().DeltaTime;
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (status, entity) in SystemAPI.Query<RefRW<Status>>().WithEntityAccess())
            {
                if(status.ValueRO.TileLeft >= 0)
                    status.ValueRW.TileLeft -= dt;
                else
                    ecb.AddComponent<Unapplied>(entity);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
