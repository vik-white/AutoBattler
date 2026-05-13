using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct BuffSkillSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (skills, entity) in SystemAPI.Query<DynamicBuffer<Skill>>().WithAll<Character>().WithEntityAccess()) {
                foreach (var skill in skills) {
                    if (!skill.TryGetActivatedConfig(SkillType.Buff, out var config)) continue;
                    if (config.Targets.Length == 0) continue;

                    NativeArray<Entity> enemies = SystemAPI.QueryBuilder().WithAll<Character>().WithAny<Enemy>().Build().ToEntityArray(Allocator.Temp);
                    NativeArray<Entity> allies = SystemAPI.QueryBuilder().WithAll<Character>().WithNone<Enemy>().Build().ToEntityArray(Allocator.Temp);
                    var targets = SkillHandler.GetTargets(skill, entity, SystemAPI.HasComponent<Enemy>(entity), enemies, allies);
                    
                    foreach (var status in config.Statuses) {
                        foreach (var target in targets)
                        {
                            ecb.CreateFrameEntity(new CreateStatus
                            {
                                Skill = skill.Config,
                                Provider = entity,
                                Target = target, 
                                Data = status, 
                            });
                        }
                    }
                    
                    foreach (var effect in config.Effects) {
                        foreach (var target in targets)
                        {
                            ecb.CreateFrameEntity(new CreateEffect 
                            {
                                Skill = skill.Config,
                                Provider = entity,
                                Target = target, 
                                Data = effect, 
                            });
                        }
                    }
                    
                    foreach (var stat in config.Stats) {
                        foreach (var target in targets)
                        {
                            ecb.CreateFrameEntity(new CreateStatChange 
                            {
                                Skill = skill.Config,
                                Provider = entity,
                                Target = target, 
                                Data = stat, 
                            });
                        }
                    }
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}
