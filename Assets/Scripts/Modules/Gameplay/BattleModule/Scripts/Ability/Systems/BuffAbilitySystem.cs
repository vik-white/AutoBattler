using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct BuffAbilitySystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (abilities, entity) in SystemAPI.Query<DynamicBuffer<Ability>>().WithAll<Character>().WithEntityAccess()) {
                foreach (var ability in abilities) {
                    var config = ability.GetConfig();
                    if (config.Type != AbilityType.Buff || !ability.IsActivate) continue;
                    if (config.Targets.Length == 0) continue;

                    NativeArray<Entity> enemies = SystemAPI.QueryBuilder().WithAll<Character>().WithAny<Enemy>().Build().ToEntityArray(Allocator.Temp);
                    NativeArray<Entity> allies = SystemAPI.QueryBuilder().WithAll<Character>().WithNone<Enemy>().Build().ToEntityArray(Allocator.Temp);
                    var targets = AbilityHandler.GetTargets(ability, entity, SystemAPI.HasComponent<Enemy>(entity), enemies, allies);
                    
                    foreach (var status in config.Statuses) {
                        foreach (var target in targets)
                        {
                            ecb.CreateFrameEntity(new CreateStatus
                            {
                                Ability = ability.Config,
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
                                Ability = ability.Config,
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
                                Ability = ability.Config,
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
