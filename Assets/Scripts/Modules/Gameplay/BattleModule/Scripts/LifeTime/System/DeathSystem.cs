using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(DeadSystemGroup), OrderFirst = true)]
    public partial struct DeathSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            foreach (var (health, entity) in SystemAPI.Query<RefRO<Health>>().WithEntityAccess()) {
                if(health.ValueRO.Value <= 0) ecb.AddComponent<Dead>(entity);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}