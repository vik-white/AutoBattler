using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(CollisionSystemGroup))]
    public partial struct AuraCollisionSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<Time>()) return;

            var entityManager = state.EntityManager;
            var dt = SystemAPI.GetSingleton<Time>().DeltaTime;
            var enemies = SystemAPI.GetComponentLookup<Enemy>(true);
            var dead = SystemAPI.GetComponentLookup<Dead>(true);

            foreach (var (aura, auraTransform, radius, provider, collisionTargets) in SystemAPI
                         .Query<RefRW<Aura>, RefRO<LocalTransform>, RefRO<CollisionRadius>, RefRO<Provider>, DynamicBuffer<CollisionTarget>>())
            {
                if (aura.ValueRO.IntervalTimeLeft >= 0)
                {
                    aura.ValueRW.IntervalTimeLeft -= dt;
                    continue;
                }

                aura.ValueRW.IntervalTimeLeft = aura.ValueRO.Interval;
                if (!entityManager.Exists(provider.ValueRO.Value) || dead.HasComponent(provider.ValueRO.Value)) continue;

                var providerIsEnemy = enemies.HasComponent(provider.ValueRO.Value);
                var auraPosition = auraTransform.ValueRO.Position;
                var auraRadius = radius.ValueRO.Value;

                foreach (var (characterTransform, character, characterEntity) in SystemAPI
                             .Query<RefRO<LocalTransform>, RefRO<Character>>()
                             .WithNone<Dead>()
                             .WithEntityAccess())
                {
                    if (characterEntity == provider.ValueRO.Value) continue;
                    if (providerIsEnemy == enemies.HasComponent(characterEntity)) continue;

                    var characterConfig = character.ValueRO.GetConfig();
                    var characterCenter = characterTransform.ValueRO.Position + new float3(0f, characterConfig.ColliderHeight * 0.5f, 0f);
                    var horizontalDistance = math.distance(auraPosition.xz, characterCenter.xz);
                    var verticalDistance = math.abs(auraPosition.y - characterCenter.y);
                    var horizontalHitDistance = auraRadius + characterConfig.ColliderRadius;
                    var verticalHitDistance = auraRadius + characterConfig.ColliderHeight * 0.5f;

                    if (horizontalDistance > horizontalHitDistance || verticalDistance > verticalHitDistance) continue;

                    collisionTargets.Add(new CollisionTarget { Value = characterEntity });
                }
            }
        }
    }
}
