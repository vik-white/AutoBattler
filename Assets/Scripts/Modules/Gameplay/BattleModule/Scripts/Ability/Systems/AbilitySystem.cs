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
                    var abilityConfig = ability.GetConfig();
                    ability.IsActivate = false;

                    if (ability.IsChild) continue;
                    if (!SystemAPI.HasComponent<Target>(entity)) continue;

                    var activeAbility = SystemAPI.HasComponent<ActiveAbility>(entity) ? SystemAPI.GetComponent<ActiveAbility>(entity).Value : 0;
                    var statBuffer = SystemAPI.GetBuffer<StatMultiply>(entity);
                    var cooldownMultiply = activeAbility == abilityConfig.ID ? statBuffer[(int)StatType.ActiveAbilityCooldownMultiply].Value : statBuffer[(int)StatType.CooldownMultiply].Value;
                    ability.Cooldown += dt * cooldownMultiply;

                    if (ability.Cooldown > abilityConfig.Cooldown)
                    {
                        if (abilityConfig.ID != activeAbility)
                        {
                            var distance = float.MaxValue;
                            var target = SystemAPI.GetComponent<Target>(entity).Value;
                            var characterConfig = character.ValueRO.GetConfig();
                            var targetConfig = SystemAPI.GetComponent<Character>(target).GetConfig();
                            var baseDistance = abilityConfig.Radius + characterConfig.ColliderRadius + targetConfig.ColliderRadius;
                            if (SystemAPI.HasComponent<Target>(entity))
                            {
                                var targetTransform = SystemAPI.GetComponent<LocalTransform>(target);
                                var direction = targetTransform.Position - transform.ValueRO.Position;
                                distance = math.length(direction);
                            }
                            if (distance <= baseDistance || abilityConfig.Radius == 0)
                            {
                                ability.Cooldown = 0;

                                if (abilityConfig.Type != AbilityType.Abilities)
                                {
                                    ability.IsAnimation = true;
                                    var speedMultiply = SystemAPI.GetBuffer<StatMultiply>(entity)[(int)StatType.CooldownMultiply].Value;
                                    ecb.CreateFrameEntity(new Animation { Character = entity, Type = abilityConfig.Animation, Speed = speedMultiply });
                                    if (abilityConfig.CastVFXPrefab != 0) ecb.CreateFrameEntity(new CreatePrefabEvent { ID = abilityConfig.CastVFXPrefab, Position = transform.ValueRO.Position });
                                }
                                else
                                {
                                    for (int j = 0; j < abilities.Length; j++)
                                    {
                                        ref var abilityChild = ref abilities.ElementAt(j);
                                        if (abilityChild.IsChild)
                                        {
                                            var abilityChildConfig = abilityChild.GetConfig();
                                            abilityChild.IsActivate = false;
                                            abilityChild.IsAnimation = true;
                                            var speedMultiply = SystemAPI.GetBuffer<StatMultiply>(entity)[(int)StatType.CooldownMultiply].Value;
                                            ecb.CreateFrameEntity(new Animation { Character = entity, Type = abilityChildConfig.Animation, Speed = speedMultiply });
                                            if (abilityChildConfig.CastVFXPrefab != 0) ecb.CreateFrameEntity(new CreatePrefabEvent { ID = abilityChildConfig.CastVFXPrefab, Position = transform.ValueRO.Position });
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            var useAbility = SystemAPI.HasComponent<UseAbility>(entity) ? activeAbility : 0;
                            if (abilityConfig.ID == useAbility)
                            {
                                ecb.RemoveComponent<UseAbility>(entity);
                                ability.Cooldown = 0;

                                if (abilityConfig.CastVFXPrefab != 0) ecb.CreateFrameEntity(new CreatePrefabEvent { ID = abilityConfig.CastVFXPrefab, Position = transform.ValueRO.Position });

                                if (abilityConfig.Type != AbilityType.Abilities)
                                {
                                    ability.IsAnimation = true;
                                    var speedMultiply = SystemAPI.GetBuffer<StatMultiply>(entity)[(int)StatType.CooldownMultiply].Value;
                                    ecb.CreateFrameEntity(new Animation { Character = entity, Type = abilityConfig.Animation, Speed = speedMultiply });
                                }
                                else
                                {
                                    for (int j = 0; j < abilities.Length; j++)
                                    {
                                        ref var abilityChild = ref abilities.ElementAt(j);
                                        if (abilityChild.IsChild)
                                        {
                                            var abilityChildConfig = abilityChild.GetConfig();
                                            abilityChild.IsActivate = false;
                                            abilityChild.IsAnimation = true;
                                            var speedMultiply = SystemAPI.GetBuffer<StatMultiply>(entity)[(int)StatType.CooldownMultiply].Value;
                                            ecb.CreateFrameEntity(new Animation { Character = entity, Type = abilityChildConfig.Animation, Speed = speedMultiply });
                                            if (abilityChildConfig.CastVFXPrefab != 0) ecb.CreateFrameEntity(new CreatePrefabEvent { ID = abilityChildConfig.CastVFXPrefab, Position = transform.ValueRO.Position });
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
