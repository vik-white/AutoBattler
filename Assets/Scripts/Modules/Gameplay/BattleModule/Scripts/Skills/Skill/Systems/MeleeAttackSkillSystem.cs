using Unity.Entities;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct MeleeAttackSkillSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (skills, target, entity) in SystemAPI.Query<DynamicBuffer<Skill>, RefRO<Target>>().WithAll<Character>().WithEntityAccess()) {
                foreach (var skill in skills) {
                    if (!skill.TryGetActivatedConfig(SkillType.MeleeAttack, out var config)) continue;
                    
                    foreach (var status in config.Statuses) {
                        ecb.CreateFrameEntity(new CreateStatus
                        {
                            Skill = skill.Config,
                            Provider = entity,
                            Target = target.ValueRO.Value, 
                            Data = status, 
                        });
                    }
                    
                    foreach (var effect in config.Effects) {
                        ecb.CreateFrameEntity(new CreateEffect 
                        {
                            Skill = skill.Config,
                            Provider = entity,
                            Target = target.ValueRO.Value, 
                            Data = effect, 
                        });
                    }
                    
                    foreach (var stat in config.Stats) {
                        ecb.CreateFrameEntity(new CreateStatChange 
                        {
                            Skill = skill.Config,
                            Provider = entity,
                            Target = target.ValueRO.Value, 
                            Data = stat, 
                        });
                    }
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
