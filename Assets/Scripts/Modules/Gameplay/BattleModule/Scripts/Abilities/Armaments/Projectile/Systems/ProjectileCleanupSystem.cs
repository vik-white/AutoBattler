using Unity.Entities;
using vikwhite.ECS;

namespace vikwhite
{
    [UpdateInGroup(typeof(CleanupSystemGroup))]
    public partial struct ProjectileCleanupSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            foreach (var (limit, projectile) in SystemAPI.Query<RefRO<CollisionTargetLimit>>().WithEntityAccess()) {
                if(limit.ValueRO.Value <= 0)
                {
                    PhysicsDisposeHandler.Dispose(state.EntityManager, projectile);
                    ecb.DestroyEntity(projectile);
                }
            }
        }
    }
}
