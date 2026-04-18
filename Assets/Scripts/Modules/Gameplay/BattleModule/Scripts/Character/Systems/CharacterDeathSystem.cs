using Rukhanka.Toolbox;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(DeadSystemGroup), OrderFirst = true)]
    public partial struct CharacterDeathSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            foreach (var (health, transform, abilities, entity) in SystemAPI.Query<RefRO<Health>, RefRO<LocalTransform>, DynamicBuffer<Ability>>().WithNone<Dead>().WithEntityAccess()) {
                if (health.ValueRO.Value <= 0)
                {
                    abilities.Clear();
                    ecb.AddComponent<Dead>(entity);
                    ecb.RemoveComponent<PhysicsMass>(entity);
                    ecb.RemoveComponent<PhysicsCollider>(entity);
                    ecb.RemoveComponent<PhysicsVelocity>(entity);
                    ecb.CreateFrameEntity(new DeadCharacterEvent { Character = entity });
                    ecb.CreateFrameEntity(new Animation { Character = entity, Type = AnimationType.Dead, Speed = 1 });
                    if (SystemAPI.HasComponent<Enemy>(entity))
                        ecb.CreateFrameEntity(new CreatePrefabEvent { ID = "DeadVFX".CalculateHash32(), Position = transform.ValueRO.Position });
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}