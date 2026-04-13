using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(DeadSystemGroup))]
    [UpdateAfter(typeof(CharacterDeathSystem))]
    public partial struct UnapplyStatusesOfDeadTargetSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            foreach (var (_, entity) in SystemAPI.Query<RefRO<Dead>>().WithEntityAccess()) {
                foreach (var (_, target, statusEntity) in SystemAPI.Query<RefRO<Status>, RefRO<Target>>().WithEntityAccess()) {
                    if(target.ValueRO.Value == entity)
                        ecb.AddComponent<Unapplied>(statusEntity);
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}