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
            var transforms = SystemAPI.GetComponentLookup<LocalTransform>(true);
            var characters = SystemAPI.GetComponentLookup<Character>(true);
            foreach (var (abilities, transform, character, entity) in SystemAPI.Query<DynamicBuffer<Ability>, RefRO<LocalTransform>, RefRO<Character>>().WithEntityAccess())
            {
                bool hasTarget = SystemAPI.HasComponent<Target>(entity);
                uint activeAbility = SystemAPI.HasComponent<ActiveAbility>(entity) ? SystemAPI.GetComponent<ActiveAbility>(entity).Value : 0;
                bool useActiveAbility = SystemAPI.HasComponent<UseAbility>(entity);
                var statBuffer = SystemAPI.GetBuffer<StatMultiply>(entity);
                float activeCooldownMultiply = statBuffer[(int)StatType.ActiveAbilityCooldownMultiply].Value;
                float cooldownMultiply = statBuffer[(int)StatType.CooldownMultiply].Value;
                var characterConfig = character.ValueRO.GetConfig();
                Entity target = hasTarget ? SystemAPI.GetComponent<Target>(entity).Value : Entity.Null;
                bool targetIsValid = hasTarget
                    && target != Entity.Null
                    && transforms.HasComponent(target)
                    && characters.HasComponent(target);

                if (hasTarget && !targetIsValid)
                {
                    ecb.RemoveComponent<Target>(entity);
                    hasTarget = false;
                    target = Entity.Null;
                }

                var targetTransform = targetIsValid ? transforms[target] : default;
                var targetConfig = targetIsValid ? characters[target].GetConfig() : default;

                for (int i = 0; i < abilities.Length; i++)
                {
                    ref var ability = ref abilities.ElementAt(i);
                    var abilityConfig = ability.GetConfig();
                    ability.IsActivate = false;

                    if (ability.IsChild || !hasTarget) continue;

                    ability.Cooldown += dt * (activeAbility == abilityConfig.ID ? activeCooldownMultiply : cooldownMultiply);
                    if (ability.Cooldown <= abilityConfig.Cooldown) continue;

                    bool isActiveAbility = abilityConfig.ID == activeAbility;
                    if (isActiveAbility)
                    {
                        if (!useActiveAbility) continue;
                        ecb.RemoveComponent<UseAbility>(entity);
                    }
                    else if (!CanUseOnTarget(transform.ValueRO, targetTransform, abilityConfig, characterConfig, targetConfig)) continue;

                    ability.Cooldown = 0;
                    TriggerAbility(ref state, ecb, abilities, entity, transform.ValueRO.Position, abilityConfig, cooldownMultiply, ref ability);
                }
            }
            ecb.Playback(state.EntityManager);
        }

        private static bool CanUseOnTarget(in LocalTransform transform, in LocalTransform targetTransform, in AbilityConfig abilityConfig, in CharacterConfigData characterConfig, in CharacterConfigData targetConfig)
        {
            if (abilityConfig.Radius == 0) return true;

            var distance = math.distance(transform.Position, targetTransform.Position);
            var maxDistance = abilityConfig.Radius + characterConfig.ColliderRadius + targetConfig.ColliderRadius;
            return distance <= maxDistance;
        }

        private static void TriggerAbility(ref SystemState state, EntityCommandBuffer ecb, DynamicBuffer<Ability> abilities, Entity entity, float3 position, in AbilityConfig abilityConfig, float speedMultiply, ref Ability ability)
        {
            if (abilityConfig.Type != AbilityType.Abilities)
            {
                ability.IsAnimation = true;
                PlayAbility(ref state, ecb, entity, position, abilityConfig, speedMultiply);
                return;
            }

            for (int i = 0; i < abilities.Length; i++)
            {
                ref var childAbility = ref abilities.ElementAt(i);
                if (!childAbility.IsChild) continue;

                var childConfig = childAbility.GetConfig();
                childAbility.IsActivate = false;
                childAbility.IsAnimation = true;
                PlayAbility(ref state, ecb, entity, position, childConfig, speedMultiply);
            }
        }

        private static void PlayAbility(ref SystemState state, EntityCommandBuffer ecb, Entity entity, float3 position, in AbilityConfig abilityConfig, float speedMultiply)
        {
            if (abilityConfig.Animation == AnimationType.Attack || abilityConfig.Animation == AnimationType.Ability)
            {
                if (!state.EntityManager.HasComponent<MovementLock>(entity))
                    ecb.AddComponent<MovementLock>(entity);
            }

            ecb.CreateFrameEntity(new Animation { Character = entity, Type = abilityConfig.Animation, Speed = speedMultiply });
            if (abilityConfig.CastVFXPrefab != 0) ecb.CreateFrameEntity(new CreatePrefabEvent { ID = abilityConfig.CastVFXPrefab, Position = position });
        }
    }
}
