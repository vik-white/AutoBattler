using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    public static partial class SkillHandler
    {
        public static bool CanProcessOwner(Entity owner, in SkillTriggerRequest request, in SkillTriggerContext context)
        {
            return !context.Dead.HasComponent(owner) || request.AllowDeadSourceOwner && owner == request.Source;
        }

        public static bool CanTriggerSkill(in Skill skill, DynamicBuffer<StarterSkill> pendingSkills, Entity owner, in LocalTransform ownerTransform, in CharacterConfigData ownerConfig, in SkillConfig skillConfig, in SkillTriggerRequest request, in SkillTriggerContext context)
        {
            return CanStartActivation(skillConfig, pendingSkills)
                   && MatchesRequestedSkill(owner, skillConfig, request)
                   && skillConfig.Trigger == request.Trigger
                   && MatchesTriggerSource(owner, request.Source, skillConfig.TriggerSource, context)
                   && skill.Cooldown >= skillConfig.Cooldown
                   && CanUseSkill(owner, ownerTransform, skillConfig, ownerConfig, request, context);
        }

        private static bool CanStartActivation(in SkillConfig skillConfig, DynamicBuffer<StarterSkill> pendingSkills)
        {
            return !HasActivationAnimation(skillConfig) || !HasPendingAnimatedSkill(pendingSkills);
        }

        private static bool HasPendingAnimatedSkill(DynamicBuffer<StarterSkill> pendingSkills)
        {
            foreach (var pendingSkill in pendingSkills)
            {
                if (pendingSkill.WaitForAnimation)
                    return true;
            }

            return false;
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
    }
}
