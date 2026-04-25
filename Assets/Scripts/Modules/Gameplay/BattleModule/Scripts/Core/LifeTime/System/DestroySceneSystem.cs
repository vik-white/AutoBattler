using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    public partial struct DestroySceneSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            if(!SystemAPI.HasSingleton<DestroyScene>()) return;
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            foreach (var (_, entity) in SystemAPI.Query<SceneEntity>().WithEntityAccess())
            {
                PhysicsDisposeHandler.Dispose(state.EntityManager, entity);
                ecb.DestroyEntity(entity);
            }
            ecb.DestroyEntity(SystemAPI.GetSingletonEntity<DestroyScene>());
            ecb.Playback(state.EntityManager);
        }
    }
}
