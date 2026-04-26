using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(CleanupSystemGroup))]
    public partial struct DestroyTimeSystem: ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (timer, entity) in SystemAPI.Query<RefRW<DestroyTimer>>().WithEntityAccess()) {
                timer.ValueRW.Time -= SystemAPI.Time.DeltaTime;
                if (timer.ValueRO.Time <= 0)
                    ecb.DestroyEntityAndPhysics(state.EntityManager, entity);
            }
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
