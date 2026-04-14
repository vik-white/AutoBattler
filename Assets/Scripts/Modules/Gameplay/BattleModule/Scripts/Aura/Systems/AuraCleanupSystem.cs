using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using vikwhite.ECS;

namespace vikwhite
{
    [UpdateInGroup(typeof(CreateSystemGroup))]
    public partial struct AuraCleanupSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var dt = SystemAPI.GetSingleton<Time>().DeltaTime;
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (aura, entity) in SystemAPI.Query<RefRW<Aura>>().WithEntityAccess()) {
                if (aura.ValueRO.TimeLeft >= 0)
                    aura.ValueRW.TimeLeft -= dt;
                else
                    ecb.DestroyEntity(entity);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}