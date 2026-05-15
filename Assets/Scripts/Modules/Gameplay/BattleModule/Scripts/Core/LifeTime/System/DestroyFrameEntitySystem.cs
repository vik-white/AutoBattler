using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(FrameCleanupSystemGroup), OrderLast = true)]
    public partial struct DestroyFrameEntitySystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            foreach (var (_, entity) in SystemAPI.Query<RefRO<Destroy>>().WithNone<SceneEntity>().WithEntityAccess())
                ecb.DestroyEntityAndPhysics(state.EntityManager, entity);
            ecb.Playback(state.EntityManager);
        }
    }
}
