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
            var transforms = SystemAPI.GetComponentLookup<LocalTransform>(true);
            foreach (var skillActivatedEvent in SystemAPI.Query<RefRO<SkillActivatedEvent>>()) {
                var skill = skillActivatedEvent.ValueRO.Skill;
                var config = skill.Value;
                var entity = skillActivatedEvent.ValueRO.Character;
                if (config.Type != SkillType.OrbitAttack) continue;
                if (!transforms.HasComponent(entity)) continue;

                var count = config.Projectile.Count;
                for (int i = 0; i < count; i++)
                {
                    var phase = (2 * Mathf.PI * i) / count;
                    ecb.CreateFrameEntity(new CreateOrbitProjectile
                    {
                        Skill = skill,
                        Provider = entity,
                        Position = transforms[entity].Position,
                        Phase = phase,
                    });
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
