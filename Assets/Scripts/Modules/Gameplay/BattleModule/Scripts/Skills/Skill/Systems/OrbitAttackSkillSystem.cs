using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct OrbitAttackSkillSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (skills, transform, entity) in SystemAPI.Query<DynamicBuffer<Skill>, RefRO<LocalTransform>>().WithAll<Character>().WithEntityAccess()) {
                foreach (var skill in skills) {
                    if (!skill.TryGetActivatedConfig(SkillType.OrbitAttack, out var config)) continue;
                    
                    var count = config.Projectile.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var phase = (2 * Mathf.PI * i) / count;
                        ecb.CreateFrameEntity(new CreateOrbitProjectile
                        {
                            Skill = skill.Config,
                            Provider = entity,
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
