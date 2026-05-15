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
            
            if (HasTarget(config, TargetType.Self))
                AddTarget(targets, entity);

            if (HasTarget(config, TargetType.Trigger))
                AddTarget(targets, trigger);

            if (HasTarget(config, TargetType.Target) && TryGetSelectedTarget(entity, selectedTargets, out var selectedTarget))
                AddTarget(targets, selectedTarget);
            
            if (HasTarget(config, TargetType.Allies))
                AddTargets(targets, entity, isEnemy ? enemies : allies);
            
            if (HasTarget(config, TargetType.Enemies))
                AddTargets(targets, entity, isEnemy ? allies : enemies);

            return targets;
        }

        public static bool TryGetTarget(BlobAssetReference<SkillConfig> skill, Entity entity, Entity trigger, ComponentLookup<Target> targets, out Entity target)
        {
            var config = skill.Value;
            if (HasTarget(config, TargetType.Trigger) && trigger != Entity.Null)
            {
                target = trigger;
                return true;
            }

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
            return skillConfig.Animation is AnimationType.Attack or AnimationType.Ability;
        }

        public static void ClearPending(ref Skill skill)
        {
            skill.IsPending = false;
            skill.Trigger = Entity.Null;
        }
        
        public static bool CanProcessOwner(Entity owner, in SkillTriggerRequest request, in SkillTriggerContext context)
        {
            return !context.Dead.HasComponent(owner) || request.AllowDeadSourceOwner && owner == request.Source;
        }

        public static bool CanTriggerSkill(in Skill skill, DynamicBuffer<Skill> skills, Entity owner, in LocalTransform ownerTransform, in CharacterConfigData ownerConfig, in SkillConfig skillConfig, in SkillTriggerRequest request, in SkillTriggerContext context)
        {
            return CanStartSkill(skill, skills, skillConfig)
                   && MatchesRequestedSkill(owner, skillConfig, request)
                   && skillConfig.Trigger == request.Trigger
                   && MatchesTriggerSource(owner, request.Source, skillConfig.TriggerSource, context)
                   && skill.Cooldown >= skillConfig.Cooldown
                   && CanUseSkill(owner, ownerTransform, skillConfig, ownerConfig, request, context);
        }

        private static bool MatchesTriggerSource(Entity owner, Entity source, TargetType triggerSource, in SkillTriggerContext context)
        {
            return triggerSource switch
            {
                TargetType.Self => IsValidTriggerSource(source, context) && source == owner,
                TargetType.Target => IsSelectedTriggerSource(owner, source, context),
                TargetType.Allies => IsRelatedTriggerSource(owner, source, context, true),
                TargetType.Enemies => IsRelatedTriggerSource(owner, source, context, false),
                _ => false
            };
        }

        private static bool CanUseSkill(Entity owner, in LocalTransform ownerTransform, in SkillConfig skillConfig, in CharacterConfigData ownerConfig, in SkillTriggerRequest request, in SkillTriggerContext context)
        {
            if (HasTarget(skillConfig, TargetType.Trigger))
                return CanUseTarget(request.TriggerEntity, ownerTransform, skillConfig, ownerConfig, request.ShouldIgnoreRadius(owner), request.Trigger == TriggerType.Dead, context);

            if (HasTarget(skillConfig, TargetType.Target))
                return CanUseSelectedTarget(owner, ownerTransform, skillConfig, ownerConfig, request, context);

            if (!NeedsTarget(skillConfig)) return true;

            return CanUseSelectedTarget(owner, ownerTransform, skillConfig, ownerConfig, request, context);
        }

        private static bool CanUseTarget(Entity target, in LocalTransform ownerTransform, in SkillConfig skillConfig, in CharacterConfigData ownerConfig, bool ignoreRadius, bool allowDeadTarget, in SkillTriggerContext context)
        {
            return IsUsableTarget(target, allowDeadTarget, context)
                   && (ignoreRadius || skillConfig.Radius == 0f || IsTargetInRadius(target, ownerTransform, skillConfig, ownerConfig, context));
        }

        private static bool NeedsTarget(in SkillConfig skillConfig)
        {
            if (skillConfig.Type is SkillType.MeleeAttack or SkillType.RangeAttack) return true;
            if (skillConfig.Type != SkillType.Skills) return false;

            foreach (var target in skillConfig.Targets)
                if (RequiresConcreteTarget(target))
                    return true;

            return false;
        }

        private static bool CanStartSkill(in Skill skill, DynamicBuffer<Skill> skills, in SkillConfig skillConfig)
        {
            if (skill.IsChild || skill.IsPending) return false;
            return skillConfig.Type != SkillType.Skills || !HasPendingChildSkill(skills);
        }

        private static bool MatchesRequestedSkill(Entity owner, in SkillConfig skillConfig, in SkillTriggerRequest request)
        {
            var requestedSkillId = request.GetRequestedSkillID(owner);
            return requestedSkillId == 0 || skillConfig.ID == requestedSkillId;
        }

        private static bool CanUseSelectedTarget(Entity owner, in LocalTransform ownerTransform, in SkillConfig skillConfig, in CharacterConfigData ownerConfig, in SkillTriggerRequest request, in SkillTriggerContext context)
        {
            return TryGetSelectedTarget(owner, context.Targets, out var target)
                   && CanUseTarget(target, ownerTransform, skillConfig, ownerConfig, request.ShouldIgnoreRadius(owner), false, context);
        }

        private static bool IsUsableTarget(Entity target, bool allowDeadTarget, in SkillTriggerContext context)
        {
            if (target == Entity.Null) return false;
            if (!allowDeadTarget && context.Dead.HasComponent(target)) return false;
            return context.Transforms.HasComponent(target) && context.Characters.HasComponent(target);
        }

        private static bool IsTargetInRadius(Entity target, in LocalTransform ownerTransform, in SkillConfig skillConfig, in CharacterConfigData ownerConfig, in SkillTriggerContext context)
        {
            var targetConfig = context.Characters[target].GetConfig();
            var distance = math.distance(ownerTransform.Position, context.Transforms[target].Position);
            var maxDistance = skillConfig.Radius + ownerConfig.ColliderRadius + targetConfig.ColliderRadius;
            return distance <= maxDistance;
        }

        private static bool IsSelectedTriggerSource(Entity owner, Entity source, in SkillTriggerContext context)
        {
            return IsValidTriggerSource(source, context)
                   && TryGetSelectedTarget(owner, context.Targets, out var selectedTarget)
                   && source == selectedTarget;
        }

        private static bool IsRelatedTriggerSource(Entity owner, Entity source, in SkillTriggerContext context, bool shouldMatchOwnerTeam)
        {
            if (!IsValidTriggerSource(source, context) || source == owner) return false;

            var ownerIsEnemy = context.Enemies.HasComponent(owner);
            var sourceIsEnemy = context.Enemies.HasComponent(source);
            return (ownerIsEnemy == sourceIsEnemy) == shouldMatchOwnerTeam;
        }

        private static bool IsValidTriggerSource(Entity source, in SkillTriggerContext context)
        {
            return source != Entity.Null && context.Characters.HasComponent(source);
        }

        private static bool RequiresConcreteTarget(TargetType target)
        {
            return target is TargetType.Enemies or TargetType.Trigger or TargetType.Target;
        }

        private static bool HasTarget(in SkillConfig skillConfig, TargetType target)
        {
            return skillConfig.Targets.Contains(target);
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

        private static void AddTargets(List<Entity> targets, Entity owner, NativeArray<Entity> source)
        {
            foreach (var target in source)
            {
                if (target != owner)
                    AddTarget(targets, target);
            }
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
