using Unity.Entities;
using vikwhite.ECS;

namespace vikwhite
{
    [UpdateInGroup(typeof(CleanupSystemGroup))]
    public partial struct ProjectileCleanupSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (limit, projectile) in SystemAPI.Query<RefRO<CollisionTargetLimit>>().WithEntityAccess()) {
                if(limit.ValueRO.Value <= 0) ecb.DestroyEntity(projectile);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}