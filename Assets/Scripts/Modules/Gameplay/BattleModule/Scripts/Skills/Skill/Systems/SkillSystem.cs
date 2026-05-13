using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    [UpdateInGroup(typeof(SetupSystemGroup))]
    public partial struct SkillSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var dt = SystemAPI.GetSingleton<Time>().DeltaTime;
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            var transforms = SystemAPI.GetComponentLookup<LocalTransform>(true);
            var characters = SystemAPI.GetComponentLookup<Character>(true);
            foreach (var (skills, transform, character, entity) in SystemAPI.Query<DynamicBuffer<Skill>, RefRO<LocalTransform>, RefRO<Character>>().WithEntityAccess())
            {
                bool hasTarget = SystemAPI.HasComponent<Target>(entity);
                bool useActiveSkill = SystemAPI.HasComponent<UseSkill>(entity);
                var statBuffer = SystemAPI.GetBuffer<StatMultiply>(entity);
                float skillActiveCooldown = statBuffer[(int)StatType.SkillActiveCooldown].Value;
                float skillAttackCooldown = statBuffer[(int)StatType.SkillAttackCooldown].Value;
                var characterConfig = character.ValueRO.GetConfig();
                uint activeSkill = characterConfig.GetSkill(SkillSlotType.Active);
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

                for (int i = 0; i < skills.Length; i++)
                {
                    ref var skill = ref skills.ElementAt(i);
                    var skillConfig = skill.GetConfig();
                    skill.IsActivate = false;

                    if (skill.IsChild || !hasTarget) continue;

                    skill.Cooldown += dt / (activeSkill == skillConfig.ID ? skillActiveCooldown : skillAttackCooldown);
                    if (skill.Cooldown <= skillConfig.Cooldown) continue;

                    bool isActiveAbility = skillConfig.ID == activeSkill;
                    if (isActiveAbility)
                    {
                        if (!useActiveSkill) continue;
                        ecb.RemoveComponent<UseSkill>(entity);
                    }
                    else if (!CanUseOnTarget(transform.ValueRO, targetTransform, skillConfig, characterConfig, targetConfig)) continue;

                    skill.Cooldown = 0;
                    TriggerAbility(ref state, ecb, skills, entity, transform.ValueRO.Position, skillConfig, skillAttackCooldown, ref skill);
                }
            }
            ecb.Playback(state.EntityManager);
        }

        private static bool CanUseOnTarget(in LocalTransform transform, in LocalTransform targetTransform, in SkillConfig skillConfig, in CharacterConfigData characterConfig, in CharacterConfigData targetConfig)
        {
            if (skillConfig.Radius == 0) return true;

            var distance = math.distance(transform.Position, targetTransform.Position);
            var maxDistance = skillConfig.Radius + characterConfig.ColliderRadius + targetConfig.ColliderRadius;
            return distance <= maxDistance;
        }

        private static void TriggerAbility(ref SystemState state, EntityCommandBuffer ecb, DynamicBuffer<Skill> abilities, Entity entity, float3 position, in SkillConfig skillConfig, float speedMultiply, ref Skill skill)
        {
            if (skillConfig.Type != SkillType.Skills)
            {
                skill.IsAnimation = true;
                PlayAbility(ref state, ecb, entity, position, skillConfig, speedMultiply);
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

        private static void PlayAbility(ref SystemState state, EntityCommandBuffer ecb, Entity entity, float3 position, in SkillConfig skillConfig, float speedMultiply)
        {
            if (skillConfig.Animation == AnimationType.Attack || skillConfig.Animation == AnimationType.Ability)
            {
                if (!state.EntityManager.HasComponent<MovementLock>(entity))
                    ecb.AddComponent<MovementLock>(entity);
            }

            ecb.CreateFrameEntity(new Animation { Character = entity, Type = skillConfig.Animation, Speed = speedMultiply });
            if (skillConfig.CastVFXPrefab != 0) ecb.CreateFrameEntity(new CreatePrefabEvent { ID = skillConfig.CastVFXPrefab, Position = position });
        }
    }
}
