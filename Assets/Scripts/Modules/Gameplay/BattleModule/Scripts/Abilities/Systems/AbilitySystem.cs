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
                    var activeAbility = SystemAPI.HasComponent<ActiveAbility>(entity) ? SystemAPI.GetComponent<ActiveAbility>(entity).Value : 0;
                    
                    var cooldownMultiply = SystemAPI.GetBuffer<StatMultiply>(entity)[(int)StatType.CooldownMultiply].Value;
                    if (activeAbility == ability.Config.ID) cooldownMultiply = 1;
                    ability.Cooldown += dt * cooldownMultiply;
                    
                    if (ability.Cooldown > ability.Config.Cooldown)
                    {
                        if (ability.Config.ID != activeAbility)
                        {
                            var distance = float.MaxValue;
                            var characterConfig = SystemAPI.GetSingletonBuffer<CharacterConfig>().Get(character.ValueRO.ID);
                            if (SystemAPI.HasComponent<Target>(entity))
                            {
                                var target = SystemAPI.GetComponent<Target>(entity);
                                var targetTransform = SystemAPI.GetComponent<LocalTransform>(target.Value);
                                var direction = targetTransform.Position - transform.ValueRO.Position;
                                distance = math.length(direction);
                            }
                            if (distance <= ability.Config.Radius + characterConfig.ColliderRadius || ability.Config.Radius == 0)
                            {
                                ability.IsActivate = true;
                                ability.Cooldown = 0;
                            }
                        }
                        else
                        {
                            var useAbility = SystemAPI.HasComponent<UseAbility>(entity) ? activeAbility : 0;
                            if (ability.Config.ID == useAbility)
                            {
                                ecb.RemoveComponent<UseAbility>(entity);
                                ability.IsActivate = true;
                                ability.Cooldown = 0;
                            }
                        }
                    }
                }
            }
            ecb.Playback(state.EntityManager);
        }
    }
}