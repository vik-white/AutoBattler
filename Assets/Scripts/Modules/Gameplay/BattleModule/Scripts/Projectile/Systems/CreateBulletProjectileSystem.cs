using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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
                ecb.SetComponent(projectile, new LocalTransform {
                    Position = request.ValueRO.Position + new float3(0, 0.5f, 0),
                    Rotation = request.ValueRO.Rotation,
                    Scale = ability.Projectile.Radius
                });
                ecb.AddComponent(projectile, new Provider{ Value = request.ValueRO.Provider });
                ecb.AddComponent(projectile, new Speed{ Value = ability.Projectile.Speed });
                ecb.AddComponent(projectile, new DirectionMovement{ Direction = math.forward(request.ValueRO.Rotation) });
                ecb.AddComponent(projectile, new PreviousPosition{ Value = request.ValueRO.Position });
                ecb.AddComponent(projectile, new CollisionTargetLimit{ Value = ability.Projectile.Pierce });
                ecb.AddBuffer<CollisionTarget>(projectile);
                ecb.AddBuffer<CollisionBuffer>(projectile);
                ecb.AddComponent<DestroyOutsideScene>(projectile);
                var effects = ecb.AddBuffer<EffectData>(projectile);
                for (int i = 0; i < ability.Effects.Length; i++) effects.Add(ability.Effects[i]);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}