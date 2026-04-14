using System.Collections.Generic;
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
                    if (ability.Config.Type != AbilityType.Buff || !ability.IsActivate) continue;
                    if (ability.Config.Targets.Length == 0) continue;

                    var targets = GetTargets(ref state, ability, entity);
                    
                    foreach (var status in ability.Config.Statuses) {
                        foreach (var target in targets)
                        {
                            ecb.CreateFrameEntity(new CreateStatus
                            {
                                Ability = new AbilityLevelData{ ID = ability.Config.ID, Level = ability.Config.Level },
                                Provider = entity,
                                Target = target, 
                                Data = status, 
                            });
                        }
                    }
                    
                    foreach (var effect in ability.Config.Effects) {
                        foreach (var target in targets)
                        {
                            ecb.CreateFrameEntity(new CreateEffect 
                            {
                                Ability = new AbilityLevelData{ ID = ability.Config.ID, Level = ability.Config.Level },
                                Provider = entity,
                                Target = target, 
                                Data = effect, 
                            });
                        }
                    }
                    
                    foreach (var stat in ability.Config.Stats) {
                        foreach (var target in targets)
                        {
                            ecb.CreateFrameEntity(new CreateStatChange 
                            {
                                Target = target, 
                                Data = stat, 
                            });
                        }
                    }
                    
                    ecb.CreateFrameEntity(new Animation { Character = entity, ID = AnimationID.Attack });
                }
            }
            ecb.Playback(state.EntityManager);
        }

        private List<Entity> GetTargets(ref SystemState state, Ability ability, Entity entity)
        {
            var targets = new List<Entity>();
            
            if (ability.Config.Targets.Contains(TargetType.Self)) 
                targets.Add(entity);
            
            var isEnemy = SystemAPI.HasComponent<Enemy>(entity);
            if (ability.Config.Targets.Contains(TargetType.Allies))
            {
                if (!isEnemy)
                {
                    foreach (var (_, ally) in SystemAPI.Query<RefRO<Character>>().WithNone<Enemy>().WithEntityAccess())
                    {
                        if(ally != entity) targets.Add(ally);
                    }
                }
                else
                {
                    foreach (var (_, ally) in SystemAPI.Query<RefRO<Character>>().WithAll<Enemy>().WithEntityAccess())
                    {
                        if(ally != entity) targets.Add(ally);
                    }
                }
            }
            
            if (ability.Config.Targets.Contains(TargetType.Enemies))
            {
                if (!isEnemy)
                {
                    foreach (var (_, enemy) in SystemAPI.Query<RefRO<Character>>().WithAll<Enemy>().WithEntityAccess())
                    {
                        if(enemy != entity) targets.Add(enemy);
                    }
                }
                else
                {
                    foreach (var (_, enemy) in SystemAPI.Query<RefRO<Character>>().WithNone<Enemy>().WithEntityAccess())
                    {
                        if(enemy != entity) targets.Add(enemy);
                    }
                }
            }
            return targets;
        }
    }
}