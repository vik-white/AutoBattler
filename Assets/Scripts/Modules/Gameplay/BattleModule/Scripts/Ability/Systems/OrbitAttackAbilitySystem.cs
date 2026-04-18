using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct OrbitAttackAbilitySystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (abilities, transform, entity) in SystemAPI.Query<DynamicBuffer<Ability>, RefRO<LocalTransform>>().WithAll<Character>().WithEntityAccess()) {
                foreach (var ability in abilities) {
                    if (ability.Config.Type != AbilityType.OrbitAttack || !ability.IsActivate) continue;
                    
                    var count = ability.Config.Projectile.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var phase = (2 * Mathf.PI * i) / count;
                        ecb.CreateFrameEntity(new CreateOrbitProjectile
                        {
                            Provider = entity,
                            Ability = ability.Config,
                            Position = transform.ValueRO.Position,
                            Phase = phase,
                        });
                    }
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}