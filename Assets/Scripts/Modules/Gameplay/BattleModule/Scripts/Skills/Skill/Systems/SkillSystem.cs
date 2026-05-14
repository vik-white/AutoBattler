using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

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
                bool useActiveSkillRequested = SystemAPI.HasComponent<UseSkill>(entity);
                var statMultipliers = SystemAPI.GetBuffer<StatMultiply>(entity);
                float skillActiveCooldown = statMultipliers[(int)StatType.SkillActiveCooldown].Value;
                float skillAttackCooldown = statMultipliers[(int)StatType.SkillAttackCooldown].Value;
                var characterConfig = character.ValueRO.GetConfig();
                uint activeSkillId = characterConfig.GetSkill(SkillSlotType.Active);
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
                    skill.IsActivated = false;

                    if (skill.IsChild || !hasTarget) continue;

                    skill.Cooldown += dt * (1f / (activeSkillId == skillConfig.ID ? skillActiveCooldown : skillAttackCooldown));
                    if (skill.Cooldown <= skillConfig.Cooldown) continue;

                    bool isActiveSkill = skillConfig.ID == activeSkillId;
                    if (isActiveSkill)
                    {
                        if (!useActiveSkillRequested) continue;
                        ecb.RemoveComponent<UseSkill>(entity);
                    }
                    else if (!CanUseOnTarget(transform.ValueRO, targetTransform, skillConfig, characterConfig, targetConfig)) continue;

                    if (!isActiveSkill && Random.value > skillConfig.Chance)
                    {
                        skill.Cooldown = 0;
                        continue;
                    }

                    skill.Cooldown = 0;
                    TriggerSkill(ref state, ecb, skills, entity, transform.ValueRO.Position, skillConfig, 1f / skillAttackCooldown, ref skill);
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

        private static void TriggerSkill(ref SystemState state, EntityCommandBuffer ecb, DynamicBuffer<Skill> skills, Entity entity, float3 position, in SkillConfig skillConfig, float speedMultiplier, ref Skill skill)
        {
            if (skillConfig.Type != SkillType.Skills)
            {
                skill.IsAnimating = true;
                PlaySkill(ref state, ecb, entity, position, skillConfig, speedMultiplier);
                return;
            }

            for (int i = 0; i < skills.Length; i++)
            {
                ref var childSkill = ref skills.ElementAt(i);
                if (!childSkill.IsChild) continue;

                var childConfig = childSkill.GetConfig();
                childSkill.IsActivated = false;
                childSkill.IsAnimating = true;
                PlaySkill(ref state, ecb, entity, position, childConfig, speedMultiplier);
            }
        }

        private static void PlaySkill(ref SystemState state, EntityCommandBuffer ecb, Entity entity, float3 position, in SkillConfig skillConfig, float speedMultiplier)
        {
            if (skillConfig.Animation == AnimationType.Attack || skillConfig.Animation == AnimationType.Ability)
            {
                if (!state.EntityManager.HasComponent<MovementLock>(entity))
                    ecb.AddComponent<MovementLock>(entity);
            }

            ecb.CreateFrameEntity(new Animation { Character = entity, Type = skillConfig.Animation, Speed = speedMultiplier });
            if (skillConfig.CastVFXPrefab != 0) ecb.CreateFrameEntity(new CreatePrefabEvent { ID = skillConfig.CastVFXPrefab, Position = position });
        }
    }
}
