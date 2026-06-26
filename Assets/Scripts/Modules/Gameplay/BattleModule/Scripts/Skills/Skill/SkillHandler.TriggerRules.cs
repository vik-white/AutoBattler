using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace vikwhite.ECS
{
    public static partial class SkillHandler
    {
        public static bool CanTriggerSkill(in Skill skill, DynamicBuffer<StarterSkill> starterSkills, Entity owner, in LocalTransform ownerTransform, in CharacterConfigData ownerConfig, in SkillConfig skillConfig, in SkillTriggerRequest request, in SkillTriggerContext context)
        {
            return CanProcessOwner(owner, request, context)
                   && MatchesRequestedSkill(owner, skillConfig, request)
                   && skillConfig.Trigger == request.Trigger
                   && skill.Cooldown >= skillConfig.Cooldown
                   && CanStartAnimation(owner, skillConfig, starterSkills, request, context)
                   && MatchesTriggerSource(owner, request.Source, skillConfig.TriggerSource, context)
                   && CanUseSkill(owner, ownerTransform, skillConfig, ownerConfig, request, context);
        }

        private static bool CanProcessOwner(Entity owner, in SkillTriggerRequest request, in SkillTriggerContext context)
        {
            return !context.Dead.HasComponent(owner) || request.AllowDeadSourceOwner && owner == request.Source;
        }

        private static bool CanStartAnimation(Entity owner, in SkillConfig skillConfig, DynamicBuffer<StarterSkill> starterSkills, in SkillTriggerRequest request, in SkillTriggerContext context)
        {
            if (!HasActivationAnimation(skillConfig)) return true;

            var isManualActivation = request.Trigger == TriggerType.Activate && request.GetRequestedSkillID(owner) == skillConfig.ID;
            if (context.ActiveSkillAnimationLocks.HasComponent(owner)) return false;
            if (!isManualActivation && context.MovementLocks.HasComponent(owner)) return false;

            foreach (var starterSkill in starterSkills)
            {
                if (!starterSkill.WaitForAnimation) continue;
                if (isManualActivation && starterSkill.Skill.Value.Trigger != TriggerType.Activate)
                    continue;

                return false;
            }

            return true;
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

            return !NeedsSelectedTarget(skillConfig) || CanUseSelectedTarget(owner, ownerTransform, skillConfig, ownerConfig, request, context);
        }

        private static bool CanUseTarget(Entity target, in LocalTransform ownerTransform, in SkillConfig skillConfig, in CharacterConfigData ownerConfig, bool ignoreRadius, bool allowDeadTarget, in SkillTriggerContext context)
        {
            return IsUsableTarget(target, allowDeadTarget, context)
                   && (ignoreRadius || skillConfig.Radius == 0f || IsTargetInRadius(target, ownerTransform, skillConfig, ownerConfig, context));
        }

        private static bool NeedsSelectedTarget(in SkillConfig skillConfig)
        {
            if (skillConfig.Type is SkillType.MeleeAttack or SkillType.RangeAttack) return true;
            return skillConfig.Type == SkillType.Skills && HasTarget(skillConfig, TargetType.Enemies);
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
    }
}
