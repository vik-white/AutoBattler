using Unity.Entities;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct MeleeAttackAbilitySystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (abilities, target, entity) in SystemAPI.Query<DynamicBuffer<Ability>, RefRO<Target>>().WithAll<Character>().WithEntityAccess()) {
                foreach (var ability in abilities) {
                    if (ability.Config.Type != AbilityType.MeleeAttack || !ability.IsActivate) continue;
                    
                    foreach (var status in ability.Config.Statuses) {
                        ecb.CreateFrameEntity(new CreateStatus
                        {
                            Ability = new AbilityLevelData{ ID = ability.Config.ID, Level = ability.Config.Level },
                            Provider = entity,
                            Target = target.ValueRO.Value, 
                            Data = status, 
                        });
                    }
                    
                    foreach (var effect in ability.Config.Effects) {
                        ecb.CreateFrameEntity(new CreateEffect 
                        {
                            Ability = new AbilityLevelData{ ID = ability.Config.ID, Level = ability.Config.Level },
                            Provider = entity,
                            Target = target.ValueRO.Value, 
                            Data = effect, 
                        });
                    }
                    
                    foreach (var stat in ability.Config.Stats) {
                        ecb.CreateFrameEntity(new CreateStatChange 
                        {
                            Target = target.ValueRO.Value, 
                            Data = stat, 
                        });
                    }
                    
                    ecb.CreateFrameEntity(new Animation { Character = entity, ID = AnimationID.Attack });
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}