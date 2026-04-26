using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(CleanupSystemGroup), OrderLast = true)]
    public partial struct DestroySystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            foreach (var (_, entity) in SystemAPI.Query<Destroy>().WithEntityAccess())
                ecb.DestroyEntityAndPhysics(state.EntityManager, entity);
            ecb.Playback(state.EntityManager);
        }
    }
}
