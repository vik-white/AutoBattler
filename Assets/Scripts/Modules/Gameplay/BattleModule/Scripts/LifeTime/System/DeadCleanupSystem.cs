using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(CleanupSystemGroup))]
    public partial struct DeadCleanupSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            foreach (var (_, entity) in SystemAPI.Query<Dead>().WithEntityAccess()) {
                ecb.DestroyEntity(entity);
            }
        }
    }
}