using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    [UpdateAfter(typeof(RearJumpSystem))]
    public partial struct RestoreJumpPhysicsSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (physicsState, entity) in SystemAPI.Query<RefRO<JumpPhysicsState>>().WithNone<Jump>().WithEntityAccess())
            {
                ecb.AddComponent(entity, physicsState.ValueRO.Mass);
                ecb.AddComponent(entity, physicsState.ValueRO.Collider);
                ecb.AddComponent(entity, physicsState.ValueRO.Velocity);
                ecb.RemoveComponent<JumpPhysicsState>(entity);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}