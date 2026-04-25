using Unity.Entities;
using UnityEngine;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(GameplaySystemGroup))]
    public partial struct MeleeAttackAbilitySystem : ISystem
    {
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (abilities, target, entity) in SystemAPI.Query<DynamicBuffer<Ability>, RefRO<Target>>().WithAll<Character>().WithEntityAccess()) {
                foreach (var ability in abilities) {
                    if (!ability.TryGetActivatedConfig(AbilityType.MeleeAttack, out var config)) continue;
                    
                    foreach (var status in config.Statuses) {
                        ecb.CreateFrameEntity(new CreateStatus
                        {
                            Ability = ability.Config,
                            Provider = entity,
                            Target = target.ValueRO.Value, 
                            Data = status, 
                        });
                    }
                    
                    foreach (var effect in config.Effects) {
                        ecb.CreateFrameEntity(new CreateEffect 
                        {
                            Ability = ability.Config,
                            Provider = entity,
                            Target = target.ValueRO.Value, 
                            Data = effect, 
                        });
                    }
                    
                    foreach (var stat in config.Stats) {
                        ecb.CreateFrameEntity(new CreateStatChange 
                        {
                            Ability = ability.Config,
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
