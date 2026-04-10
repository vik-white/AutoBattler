using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct OrbitingFireBollSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (abilities, transform, entity) in SystemAPI.Query<DynamicBuffer<Ability>, RefRO<LocalTransform>>().WithAll<Character>().WithEntityAccess()) {
                for (int i = 0; i < abilities.Length; i++)
                {
                    ref var ability = ref abilities.ElementAt(i);
                    if (ability.Config.ID != AbilityID.OrbitingFireBoll || !ability.IsReady) continue;
                    ability.Cooldown = 0;
                    
                    var count = ability.Config.Projectile.Count;
                    for (int j = 0; j < count; j++)
                    {
                        var phase = (2 * Mathf.PI * j) / count;
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