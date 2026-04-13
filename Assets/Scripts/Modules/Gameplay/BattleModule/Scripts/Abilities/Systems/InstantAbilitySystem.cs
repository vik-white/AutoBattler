using System.Collections.Generic;
using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct InstantAbilitySystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (abilities, entity) in SystemAPI.Query<DynamicBuffer<Ability>>().WithAll<Character>().WithEntityAccess()) {
                foreach (var ability in abilities) {
                    if (ability.Config.Type != AbilityType.Instant || !ability.IsActivate) continue;
                    
                    var targets = new List<Entity>();
                    if (ability.Config.Targets.Length == 0) continue;
                    if (ability.Config.Targets.Contains(TargetType.Self)) targets.Add(entity);
                    if (ability.Config.Targets.Contains(TargetType.Allies))
                    {
                        foreach (var (_, ally) in SystemAPI.Query<RefRO<Character>>().WithNone<Enemy>().WithEntityAccess()) 
                            targets.Add(ally);
                    }
                    if (ability.Config.Targets.Contains(TargetType.Enemies))
                    {
                        foreach (var (_, enemy) in SystemAPI.Query<RefRO<Character>>().WithAll<Enemy>().WithEntityAccess()) 
                            targets.Add(enemy);
                    }
                    
                    foreach (var status in ability.Config.Statuses) {
                        foreach (var target in targets)
                        {
                            ecb.CreateFrameEntity(new CreateStatus
                            {
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
                                Target = target, 
                                Data = effect, 
                            });
                        }
                    }
                    
                    ecb.CreateFrameEntity(new Animation { Character = entity, ID = AnimationID.Attack });
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}