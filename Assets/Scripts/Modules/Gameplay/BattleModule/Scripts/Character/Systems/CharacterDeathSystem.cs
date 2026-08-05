using Unity.Entities;
using Unity.Physics;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(DeadSystemGroup), OrderFirst = true)]
    public partial struct CharacterDeathSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            var colliders = SystemAPI.GetComponentLookup<PhysicsCollider>(true);
            foreach (var (health, entity) in SystemAPI.Query<RefRO<Health>>().WithAll<Character>().WithNone<Dead>().WithEntityAccess()) {
                if (health.ValueRO.Value <= 0)
                {
                    var dead = new Dead();
                    if (colliders.HasComponent(entity))
                    {
                        dead.Collider = colliders[entity];
                        ecb.RemoveComponent<PhysicsCollider>(entity);
                    }

                    ecb.AddComponent(entity, dead);
                    ecb.CreateFrameEntity(new DeadCharacterEvent { Character = entity });
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
