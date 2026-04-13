using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(CreateSystemGroup))]
    public partial struct CreateOrbitProjectileSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var request in SystemAPI.Query<RefRO<CreateOrbitProjectile>>()) {
                var ability = request.ValueRO.Ability;
                var projectile = ecb.Instantiate(SystemAPI.GetSingletonBuffer<Prefab>()[ability.Prefab].Value);
                ecb.AddComponent<SceneEntity>(projectile);
                ecb.AddComponent<Projectile>(projectile);
                ecb.SetComponent(projectile, new LocalTransform {
                    Position = request.ValueRO.Position,
                    Rotation = quaternion.identity,
                    Scale = ability.Projectile.Scale
                });
                ecb.AddComponent(projectile, new Target{ Value = request.ValueRO.Provider });
                ecb.AddComponent(projectile, new Provider{ Value = request.ValueRO.Provider });
                ecb.AddComponent(projectile, new Speed{ Value = ability.Projectile.Speed });
                ecb.AddComponent(projectile, new DestroyTimer{ Time = ability.Projectile.Lifetime });
                ecb.AddComponent(projectile, new OrbitMovement{ Radius = ability.Projectile.OrbitRadius, Phase = request.ValueRO.Phase });
                ecb.AddComponent(projectile, new Effects{ Array = ability.Effects });
                ecb.AddComponent(projectile, new Statuses{ Array = ability.Statuses });
                ecb.AddBuffer<CollisionTarget>(projectile);
                ecb.AddBuffer<CollisionBuffer>(projectile);
                ecb.AddComponent<DestroyOutsideScene>(projectile);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}