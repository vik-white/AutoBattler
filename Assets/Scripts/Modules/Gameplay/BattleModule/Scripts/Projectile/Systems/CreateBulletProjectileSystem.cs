using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(CreateSystemGroup))]
    public partial struct CreateBulletProjectileSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var request in SystemAPI.Query<RefRO<CreateBulletProjectile>>()) {
                var ability = request.ValueRO.Ability;
                var projectile = ecb.Instantiate(SystemAPI.GetSingletonBuffer<Prefab>()[ability.Prefab].Value);
                ecb.AddComponent<SceneEntity>(projectile);
                ecb.AddComponent<Projectile>(projectile);
                ecb.SetComponent(projectile, new LocalTransform {
                    Position = request.ValueRO.Position + new float3(0, 0.5f, 0),
                    Rotation = request.ValueRO.Rotation,
                    Scale = ability.Projectile.Scale
                });
                ecb.AddComponent(projectile, new Provider{ Value = request.ValueRO.Provider });
                ecb.AddComponent(projectile, new Speed{ Value = ability.Projectile.Speed });
                ecb.AddComponent(projectile, new DirectionMovement{ Direction = math.forward(request.ValueRO.Rotation) });
                ecb.AddComponent(projectile, new CollisionTargetLimit{ Value = ability.Projectile.Pierce });
                ecb.AddComponent(projectile, new Effects{ Array = ability.Effects, Ability = new AbilityLevelData{ ID = ability.ID, Level = ability.Level } });
                ecb.AddComponent(projectile, new Statuses{ Array = ability.Statuses, Ability = new AbilityLevelData{ ID = ability.ID, Level = ability.Level } });
                ecb.AddComponent(projectile, new Stats{ Array = ability.Stats });
                ecb.AddBuffer<CollisionTarget>(projectile);
                ecb.AddBuffer<CollisionBuffer>(projectile);
                ecb.AddComponent<DestroyOutsideScene>(projectile);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}