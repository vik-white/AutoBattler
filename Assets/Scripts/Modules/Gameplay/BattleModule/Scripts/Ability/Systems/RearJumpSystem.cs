using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct RearJumpSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var enemies = SystemAPI.GetComponentLookup<Enemy>(true);
            var dead = SystemAPI.GetComponentLookup<Dead>(true);
            var masses = SystemAPI.GetComponentLookup<PhysicsMass>(true);
            var velocities = SystemAPI.GetComponentLookup<PhysicsVelocity>(true);
            var colliders = SystemAPI.GetComponentLookup<PhysicsCollider>(true);

            foreach (var (abilities, transform, entity) in SystemAPI.Query<DynamicBuffer<Ability>, RefRO<LocalTransform>>().WithAll<Character>().WithNone<Dead>().WithEntityAccess())
            {
                foreach (var ability in abilities)
                {
                    if (ability.GetConfig().Type != AbilityType.RearJump || !ability.IsActivate) continue;
                    if (SystemAPI.HasComponent<Jump>(entity)) continue;

                    var target = Entity.Null;
                    var maxDistanceSq = float.MinValue;
                    var isEnemy = enemies.HasComponent(entity);
                    var position = transform.ValueRO.Position;

                    foreach (var (otherTransform, otherEntity) in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Character>().WithNone<Dead>().WithEntityAccess())
                    {
                        if (otherEntity == entity || dead.HasComponent(otherEntity)) continue;
                        if (isEnemy == enemies.HasComponent(otherEntity)) continue;

                        float distanceSq = math.distancesq(position, otherTransform.ValueRO.Position);
                        if (distanceSq <= maxDistanceSq) continue;

                        maxDistanceSq = distanceSq;
                        target = otherEntity;
                    }

                    if (target == Entity.Null) continue;

                    float distance = math.distance(position, SystemAPI.GetComponent<LocalTransform>(target).Position);
                    ecb.AddComponent(entity, new Jump
                    {
                        Value = target,
                        StartPosition = position,
                        Progress = 0f,
                        Duration = math.max(distance / 8f, 0.2f),
                        Height = math.max(1.5f, distance * 0.2f)
                    });

                    ecb.AddComponent(entity, new JumpPhysicsState
                    {
                        Mass = masses[entity],
                        Velocity = velocities[entity],
                        Collider = colliders[entity]
                    });

                    ecb.RemoveComponent<PhysicsMass>(entity);
                    ecb.RemoveComponent<PhysicsCollider>(entity);
                    ecb.RemoveComponent<PhysicsVelocity>(entity);
                }
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
