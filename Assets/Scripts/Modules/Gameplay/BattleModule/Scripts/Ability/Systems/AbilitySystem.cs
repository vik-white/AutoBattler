using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup))]
    public partial struct AbilitySystem : ISystem
    {
        public void OnUpdate(ref SystemState state) 
        {
            var dt = SystemAPI.GetSingleton<Time>().DeltaTime;
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            foreach (var (abilities, transform, character, entity) in SystemAPI.Query<DynamicBuffer<Ability>, RefRO<LocalTransform>, RefRO<Character>>().WithEntityAccess()) 
            {
                for (int i = 0; i < abilities.Length; i++)
                {
                    ref var ability = ref abilities.ElementAt(i);
                    ability.IsActivate = false;
                    
                    if(ability.IsChild) continue;
                    if(!SystemAPI.HasComponent<Target>(entity)) continue;
                    
                    var activeAbility = SystemAPI.HasComponent<ActiveAbility>(entity) ? SystemAPI.GetComponent<ActiveAbility>(entity).Value : 0;
                    var statBuffer = SystemAPI.GetBuffer<StatMultiply>(entity);
                    var cooldownMultiply = activeAbility == ability.Config.ID ? statBuffer[(int)StatType.ActiveAbilityCooldownMultiply].Value : statBuffer[(int)StatType.CooldownMultiply].Value;
                    ability.Cooldown += dt * cooldownMultiply;
                    
                    if (ability.Cooldown > ability.Config.Cooldown)
                    {
                        if (ability.Config.ID != activeAbility)
                        {
                            var distance = float.MaxValue;
                            var target = SystemAPI.GetComponent<Target>(entity).Value;
                            var characterConfig = SystemAPI.GetSingletonBuffer<CharacterConfig>().Get(character.ValueRO.ID);
                            var targetID = SystemAPI.GetComponent<Character>(target).ID;
                            var targetConfig = SystemAPI.GetSingletonBuffer<CharacterConfig>().Get(targetID);
                            var baseDistance = ability.Config.Radius + characterConfig.ColliderRadius + targetConfig.ColliderRadius;
                            if (SystemAPI.HasComponent<Target>(entity))
                            {
                                var targetTransform = SystemAPI.GetComponent<LocalTransform>(target);
                                var direction = targetTransform.Position - transform.ValueRO.Position;
                                distance = math.length(direction);
                            }
                            if (distance <= baseDistance || ability.Config.Radius == 0)
                            {
                                ability.Cooldown = 0;
                                
                                if (ability.Config.Type != AbilityType.Abilities)
                                {
                                    ability.IsAnimation = true;
                                    var speedMultiply = SystemAPI.GetBuffer<StatMultiply>(entity)[(int)StatType.CooldownMultiply].Value;
                                    ecb.CreateFrameEntity(new Animation { Character = entity, Type = ability.Config.Animation, Speed = speedMultiply });
                                }
                                else
                                {
                                    for (int j = 0; j < abilities.Length; j++)
                                    {
                                        ref var abilityChild = ref abilities.ElementAt(j);
                                        if (abilityChild.IsChild)
                                        {
                                            abilityChild.IsActivate = false;
                                            abilityChild.IsAnimation = true;
                                            var speedMultiply = SystemAPI.GetBuffer<StatMultiply>(entity)[(int)StatType.CooldownMultiply].Value;
                                            ecb.CreateFrameEntity(new Animation { Character = entity, Type = abilityChild.Config.Animation, Speed = speedMultiply });
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            var useAbility = SystemAPI.HasComponent<UseAbility>(entity) ? activeAbility : 0;
                            if (ability.Config.ID == useAbility)
                            {
                                ecb.RemoveComponent<UseAbility>(entity);
                                ability.Cooldown = 0;

                                if (ability.Config.Type != AbilityType.Abilities)
                                {
                                    ability.IsAnimation = true;
                                    var speedMultiply = SystemAPI.GetBuffer<StatMultiply>(entity)[(int)StatType.CooldownMultiply].Value;
                                    ecb.CreateFrameEntity(new Animation { Character = entity, Type = ability.Config.Animation, Speed = speedMultiply });
                                }
                                else
                                {
                                    for (int j = 0; j < abilities.Length; j++)
                                    {
                                        ref var abilityChild = ref abilities.ElementAt(j);
                                        if (abilityChild.IsChild)
                                        {
                                            abilityChild.IsActivate = false;
                                            abilityChild.IsAnimation = true;
                                            var speedMultiply = SystemAPI.GetBuffer<StatMultiply>(entity)[(int)StatType.CooldownMultiply].Value;
                                            ecb.CreateFrameEntity(new Animation { Character = entity, Type = abilityChild.Config.Animation, Speed = speedMultiply });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}