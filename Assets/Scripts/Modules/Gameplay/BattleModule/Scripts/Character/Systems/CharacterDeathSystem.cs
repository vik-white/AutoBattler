using Unity.Entities;
using Unity.Physics;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(DeadSystemGroup), OrderFirst = true)]
    public partial struct CharacterDeathSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            foreach (var (health, abilities, entity) in SystemAPI.Query<RefRO<Health>, DynamicBuffer<Ability>>().WithNone<Dead>().WithEntityAccess()) {
                if (health.ValueRO.Value <= 0)
                {
                    abilities.Clear();
                    ecb.AddComponent<Dead>(entity);
                    ecb.RemoveComponent<PhysicsMass>(entity);
                    ecb.RemoveComponent<PhysicsCollider>(entity);
                    ecb.RemoveComponent<PhysicsVelocity>(entity);
                    ecb.CreateFrameEntity(new DeadCharacterEvent { Character = entity });
                    ecb.CreateFrameEntity(new Animation { Character = entity, ID = AnimationID.Dead, Speed = 1 });
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}