using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(CleanupSystemGroup))]
    public partial struct PhysicsImpulseCleanupSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (impulse, entity) in SystemAPI.Query<RefRO<Impulse>>().WithEntityAccess()) {
                ecb.RemoveComponent<Impulse>(entity);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}