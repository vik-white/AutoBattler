using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(CollisionSystemGroup))]
    public partial struct ProjectileCollisionSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var entityManager = state.EntityManager;
            var enemies = SystemAPI.GetComponentLookup<Enemy>(true);
            var dead = SystemAPI.GetComponentLookup<Dead>(true);
            var limits = SystemAPI.GetComponentLookup<CollisionTargetLimit>();

            foreach (var (projectileTransform, radius, provider, collisionTargets, collisionBuffer, projectile) in SystemAPI
                         .Query<RefRO<LocalTransform>, RefRO<CollisionRadius>, RefRO<Provider>, DynamicBuffer<CollisionTarget>, DynamicBuffer<CollisionBuffer>>()
                         .WithAll<Projectile>()
                         .WithEntityAccess())
            {
                if (!entityManager.Exists(provider.ValueRO.Value) || dead.HasComponent(provider.ValueRO.Value)) continue;

                var hasTargetLimit = limits.HasComponent(projectile);
                if (hasTargetLimit && limits[projectile].Value <= 0) continue;

                var providerIsEnemy = enemies.HasComponent(provider.ValueRO.Value);
                var projectilePosition = projectileTransform.ValueRO.Position;
                var projectileRadius = radius.ValueRO.Value;

                foreach (var (characterTransform, character, characterEntity) in SystemAPI
                             .Query<RefRO<LocalTransform>, RefRO<Character>>()
                             .WithNone<Dead>()
                             .WithEntityAccess())
                {
                    if (characterEntity == provider.ValueRO.Value) continue;
                    if (providerIsEnemy == enemies.HasComponent(characterEntity)) continue;

                    var characterConfig = character.ValueRO.GetConfig();
                    var characterCenter = characterTransform.ValueRO.Position + new float3(0f, characterConfig.ColliderHeight * 0.5f, 0f);
                    var horizontalDistance = math.distance(projectilePosition.xz, characterCenter.xz);
                    var verticalDistance = math.abs(projectilePosition.y - characterCenter.y);
                    var horizontalHitDistance = projectileRadius + characterConfig.ColliderRadius;
                    var verticalHitDistance = projectileRadius + characterConfig.ColliderHeight * 0.5f;

                    if (horizontalDistance > horizontalHitDistance || verticalDistance > verticalHitDistance) continue;

                    var isContains = false;
                    foreach (var collision in collisionBuffer)
                    {
                        if (collision.Value == characterEntity)
                        {
                            isContains = true;
                            break;
                        }
                    }

                    if (isContains) break;

                    collisionBuffer.Clear();
                    collisionBuffer.Add(new CollisionBuffer { Value = characterEntity });
                    collisionTargets.Add(new CollisionTarget { Value = characterEntity });
                    if (hasTargetLimit)
                        limits.GetRefRW(projectile).ValueRW.Value--;
                    break;
                }
            }
        }
    }
}
