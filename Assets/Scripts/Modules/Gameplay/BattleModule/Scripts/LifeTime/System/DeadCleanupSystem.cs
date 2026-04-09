using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(CleanupSystemGroup))]
    public partial struct DeadCleanupSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            foreach (var (_, entity) in SystemAPI.Query<Dead>().WithEntityAccess()) {
                ecb.DestroyEntity(entity);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}