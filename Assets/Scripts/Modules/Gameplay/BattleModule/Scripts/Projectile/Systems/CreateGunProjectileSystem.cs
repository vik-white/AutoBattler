using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(CreateSystemGroup))]
    public partial struct CreateGunProjectileSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var request in SystemAPI.Query<RefRO<CreateGunProjectile>>()) {
                var ability = Ability();
                var projectile = ecb.Instantiate(SystemAPI.GetSingletonBuffer<Prefab>()[ability.Prefab].Value);
                ecb.SetComponent(projectile, new LocalTransform {
                    Position = request.ValueRO.Position,
                    Rotation = request.ValueRO.Rotation,
                    Scale = 0.02f
                });
                ecb.AddComponent(projectile, new Speed{ Value = ability.Projectile.Speed});
                ecb.AddComponent(projectile, new DirectionMovement{ Direction = math.forward(request.ValueRO.Rotation) });
                ecb.AddComponent(projectile, new PreviousPosition{ Value = request.ValueRO.Position });
                ecb.AddComponent(projectile, new CollisionTargetLimit{ Value = ability.Projectile.Pierce });
                ecb.AddComponent<DestroyOutsideScene>(projectile);
                ecb.AddBuffer<CollisionTarget>(projectile);
                var effects = ecb.AddBuffer<EffectData>(projectile);
                for (int i = 0; i < ability.Effects.Length; i++) effects.Add(ability.Effects[i]);
            }
            ecb.Playback(state.EntityManager);
        }
        
        private AbilityLevelConfig Ability(AbilityID id = AbilityID.Gun) {
            var level = SystemAPI.GetSingletonBuffer<AbilityLevel>()[(int)id].Value;
            return SystemAPI.GetSingletonBuffer<AbilityConfig>()[(int)id].Levels[level];
        }
    }
}