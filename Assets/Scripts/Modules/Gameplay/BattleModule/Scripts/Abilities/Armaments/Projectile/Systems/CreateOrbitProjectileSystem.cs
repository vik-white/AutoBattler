using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using SphereCollider = Unity.Physics.SphereCollider;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(CreateSystemGroup))]
    public partial struct CreateOrbitProjectileSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var request in SystemAPI.Query<RefRO<CreateOrbitProjectile>>()) {
                var ability = request.ValueRO.Ability.Value;
                var projectile = ecb.CreateEntity();
                
                ecb.AddComponent<SceneEntity>(projectile);
                ecb.AddComponent<Projectile>(projectile);
                ecb.AddComponent(projectile, new LocalTransform {
                    Position = request.ValueRO.Position,
                    Rotation = quaternion.identity,
                    Scale = 1f
                });
                var collider = SphereCollider.Create(
                    new SphereGeometry
                    {
                        Center = float3.zero,
                        Radius = ability.Projectile.Scale
                    },
                    CollisionFilter.Default,
                    new Material
                    {
                        CollisionResponse = CollisionResponsePolicy.RaiseTriggerEvents
                    });
                ecb.AddComponent(projectile, new PhysicsCollider { Value = collider });
                ecb.AddComponent(projectile, PhysicsMass.CreateKinematic(collider.Value.MassProperties));
                ecb.AddSharedComponentManaged(projectile, new PhysicsWorldIndex { Value = 0 });
                ecb.AddComponent(projectile, new Target{ Value = request.ValueRO.Provider });
                ecb.AddComponent(projectile, new Provider{ Value = request.ValueRO.Provider });
                ecb.AddComponent(projectile, new Speed{ Value = ability.Projectile.Speed });
                ecb.AddComponent(projectile, new DestroyTimer{ Time = ability.Projectile.Lifetime });
                ecb.AddComponent(projectile, new OrbitMovement{ Radius = ability.Projectile.OrbitRadius, Phase = request.ValueRO.Phase });
                ecb.AddComponent(projectile, new CollisionRadius { Value = ability.Projectile.Scale });
                ecb.AddComponent(projectile, new Effects{ Ability = request.ValueRO.Ability });
                ecb.AddComponent(projectile, new Statuses{ Ability = request.ValueRO.Ability });
                ecb.AddComponent(projectile, new Stats{ Ability = request.ValueRO.Ability });
                ecb.AddBuffer<CollisionTarget>(projectile);
                ecb.AddBuffer<CollisionBuffer>(projectile);
                ecb.AddComponent<DestroyOutsideScene>(projectile);
                ecb.CreateFrameEntity(new CreateFollowPrefabEvent { ID = ability.ProjectilePrefab, Position = request.ValueRO.Position, Entity = projectile });
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
