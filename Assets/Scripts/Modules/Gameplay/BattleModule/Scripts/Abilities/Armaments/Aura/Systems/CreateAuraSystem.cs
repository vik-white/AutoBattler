using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using vikwhite.ECS;
using SphereCollider = Unity.Physics.SphereCollider;

namespace vikwhite
{
    [UpdateInGroup(typeof(CreateSystemGroup))]
    [UpdateAfter(typeof(AuraCleanupSystem))]
    public partial struct CreateAuraSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var request in SystemAPI.Query<RefRO<CreateAura>>()) {
                var ability = request.ValueRO.Ability.Value;
                var providerPosition = SystemAPI.GetComponent<LocalTransform>(request.ValueRO.Provider).Position;
                var aura = ecb.CreateEntity();

                ecb.AddComponent<SceneEntity>(aura);
                ecb.AddComponent(aura, new Aura
                {
                    Interval = ability.AuraInterval,
                    TimeLeft = ability.AuraLifetime
                });
                ecb.AddComponent(aura, new LocalTransform {
                    Position = providerPosition,
                    Rotation = quaternion.identity,
                    Scale = 1f
                });
                var collider = SphereCollider.Create(
                    new SphereGeometry
                    {
                        Center = float3.zero,
                        Radius = ability.AuraRadius
                    },
                    CollisionFilter.Default,
                    new Material
                    {
                        CollisionResponse = CollisionResponsePolicy.RaiseTriggerEvents
                    });
                ecb.AddComponent(aura, new PhysicsCollider { Value = collider });
                ecb.AddComponent(aura, PhysicsMass.CreateKinematic(collider.Value.MassProperties));
                ecb.AddSharedComponentManaged(aura, new PhysicsWorldIndex { Value = 0 });
                ecb.AddComponent(aura, new Provider{ Value = request.ValueRO.Provider });
                ecb.AddComponent(aura, new CollisionRadius { Value = ability.AuraRadius });
                ecb.AddComponent(aura, new Effects{ Ability = request.ValueRO.Ability });
                ecb.AddComponent(aura, new Statuses{ Ability = request.ValueRO.Ability });
                ecb.AddComponent(aura, new Stats{ Ability = request.ValueRO.Ability });
                ecb.AddComponent<FollowingProvider>(aura);
                ecb.AddBuffer<CollisionTarget>(aura);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
