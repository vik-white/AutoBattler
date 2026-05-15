using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    public static class SkillHandler
    {
        public static List<Entity> GetTargets(BlobAssetReference<SkillConfig> skill, Entity entity, Entity trigger, ComponentLookup<Target> selectedTargets, bool isEnemy, NativeArray<Entity> enemies, NativeArray<Entity> allies)
        {
            var targets = new List<Entity>();
            var config = skill.Value;
            
            if (config.Targets.Contains(TargetType.Self)) 
                AddTarget(targets, entity);

            if (config.Targets.Contains(TargetType.Trigger))
                AddTarget(targets, trigger);

            if (config.Targets.Contains(TargetType.Target) && TryGetSelectedTarget(entity, selectedTargets, out var selectedTarget))
                AddTarget(targets, selectedTarget);
            
            if (config.Targets.Contains(TargetType.Allies))
            {
                if (!isEnemy)
                {
                    foreach (var ally in allies)
                    {
                        if(ally != entity) AddTarget(targets, ally);
                    }
                }
                else
                {
                    foreach (var ally in enemies)
                    {
                        if(ally != entity) AddTarget(targets, ally);
                    }
                }
            }
            
            if (config.Targets.Contains(TargetType.Enemies))
            {
                if (!isEnemy)
                {
                    foreach (var enemy in enemies)
                    {
                        if(enemy != entity) AddTarget(targets, enemy);
                    }
                }
                else
                {
                    foreach (var enemy in allies)
                    {
                        if(enemy != entity) AddTarget(targets, enemy);
                    }
                }
            }
            return targets;
        }

        public static bool TryGetTarget(BlobAssetReference<SkillConfig> skill, Entity entity, Entity trigger, ComponentLookup<Target> targets, out Entity target)
        {
            var config = skill.Value;
            if (config.Targets.Contains(TargetType.Trigger) && trigger != Entity.Null)
            {
                target = trigger;
                return true;
            }

            if (config.Targets.Contains(TargetType.Target) && TryGetSelectedTarget(entity, targets, out target))
                return true;

            if (TryGetSelectedTarget(entity, targets, out target))
                return true;

            target = Entity.Null;
            return false;
        }
        
        public static float GetCooldownRate(uint activeSkillId, uint skillId, DynamicBuffer<StatMultiply> statMultipliers)
        {
            var statType = skillId == activeSkillId ? StatType.SkillActiveCooldown : StatType.SkillAttackCooldown;

            var index = (int)statType;
            if (index < 0 || index >= statMultipliers.Length) return 1f;

            var multiplier = statMultipliers[index].Value;
            return multiplier <= 0f ? 1f : 1f / multiplier;
        }

        public static bool HasActivationAnimation(in SkillConfig skillConfig)
        {
            return skillConfig.Animation == AnimationType.Attack || skillConfig.Animation == AnimationType.Ability;
        }

        public static void ClearPending(ref Skill skill)
        {
            skill.IsPending = false;
            skill.Trigger = Entity.Null;
        }
        
        public static bool CanProcessOwner(Entity owner, in SkillTriggerRequest request, in SkillTriggerContext context)
        {
            if (!context.Dead.HasComponent(owner)) return true;
            return request.AllowDeadSourceOwner && owner == request.Source;
        }

        public static bool CanTriggerSkill(in Skill skill, DynamicBuffer<Skill> skills, Entity owner, in LocalTransform ownerTransform, in CharacterConfigData ownerConfig, in SkillConfig skillConfig, in SkillTriggerRequest request, in SkillTriggerContext context)
        {
            if (skill.IsChild) return false;
            if (skill.IsPending) return false;
            if (skillConfig.Type == SkillType.Skills && HasPendingChildSkill(skills)) return false;

            var requestedSkillId = request.GetRequestedSkillID(owner);
            if (requestedSkillId != 0 && skillConfig.ID != requestedSkillId) return false;
            if (skillConfig.Trigger != request.Trigger) return false;
            if (!MatchesTriggerSource(owner, request.Source, skillConfig.TriggerSource, context)) return false;
            if (skill.Cooldown < skillConfig.Cooldown) return false;

            return CanUseSkill(owner, ownerTransform, skillConfig, ownerConfig, request, context);
        }

        private static bool MatchesTriggerSource(Entity owner, Entity source, TargetType triggerSource, in SkillTriggerContext context)
        {
            if (source == Entity.Null || !context.Characters.HasComponent(source)) return false;

            if (triggerSource == TargetType.Self) return source == owner;
            if (triggerSource == TargetType.Target)
                return TryGetSelectedTarget(owner, context.Targets, out var selectedTarget) && source == selectedTarget;

            if (source == owner) return false;

            var ownerIsEnemy = context.Enemies.HasComponent(owner);
            var sourceIsEnemy = context.Enemies.HasComponent(source);

            return triggerSource switch
            {
                TargetType.Allies => ownerIsEnemy == sourceIsEnemy,
                TargetType.Enemies => ownerIsEnemy != sourceIsEnemy,
                _ => false
            };
        }

        private static bool CanUseSkill(Entity owner, in LocalTransform ownerTransform, in SkillConfig skillConfig, in CharacterConfigData ownerConfig, in SkillTriggerRequest request, in SkillTriggerContext context)
        {
            if (skillConfig.Targets.Contains(TargetType.Trigger))
                return CanUseTarget(request.TriggerEntity, ownerTransform, skillConfig, ownerConfig, request.ShouldIgnoreRadius(owner), request.Trigger == TriggerType.Dead, context);

            if (skillConfig.Targets.Contains(TargetType.Target))
            {
                if (!TryGetSelectedTarget(owner, context.Targets, out var selectedTarget)) return false;
                return CanUseTarget(selectedTarget, ownerTransform, skillConfig, ownerConfig, request.ShouldIgnoreRadius(owner), false, context);
            }

            if (!NeedsTarget(skillConfig)) return true;
            if (!TryGetSelectedTarget(owner, context.Targets, out var target)) return false;

            return CanUseTarget(target, ownerTransform, skillConfig, ownerConfig, request.ShouldIgnoreRadius(owner), false, context);
        }

        private static bool CanUseTarget(Entity target, in LocalTransform ownerTransform, in SkillConfig skillConfig, in CharacterConfigData ownerConfig, bool ignoreRadius, bool allowDeadTarget, in SkillTriggerContext context)
        {
            if (target == Entity.Null) return false;
            if (!allowDeadTarget && context.Dead.HasComponent(target)) return false;
            if (!context.Transforms.HasComponent(target) || !context.Characters.HasComponent(target)) return false;
            if (ignoreRadius || skillConfig.Radius == 0f) return true;

            var targetConfig = context.Characters[target].GetConfig();
            var distance = math.distance(ownerTransform.Position, context.Transforms[target].Position);
            var maxDistance = skillConfig.Radius + ownerConfig.ColliderRadius + targetConfig.ColliderRadius;
            return distance <= maxDistance;
        }

        private static bool NeedsTarget(in SkillConfig skillConfig)
        {
            if (skillConfig.Type is SkillType.MeleeAttack or SkillType.RangeAttack) return true;
            if (skillConfig.Type != SkillType.Skills) return false;

            foreach (var target in skillConfig.Targets)
                if (target == TargetType.Enemies || target == TargetType.Trigger || target == TargetType.Target)
                    return true;

            return false;
        }

        private static bool TryGetSelectedTarget(Entity entity, ComponentLookup<Target> targets, out Entity target)
        {
            if (targets.HasComponent(entity))
            {
                target = targets[entity].Value;
                return target != Entity.Null;
            }

            target = Entity.Null;
            return false;
        }

        private static void AddTarget(List<Entity> targets, Entity target)
        {
            if (target == Entity.Null || targets.Contains(target)) return;
            targets.Add(target);
        }

        private static bool HasPendingChildSkill(DynamicBuffer<Skill> skills)
        {
            for (int i = 0; i < skills.Length; i++)
            {
                var skill = skills[i];
                if (skill.IsChild && skill.IsPending)
                    return true;
            }

            return false;
        }
    }
}
