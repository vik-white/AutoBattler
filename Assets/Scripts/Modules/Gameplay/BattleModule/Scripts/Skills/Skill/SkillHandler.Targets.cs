using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

namespace vikwhite.ECS
{
    public static partial class SkillHandler
    {
        public static List<Entity> GetTargets(
            BlobAssetReference<SkillConfig> skill,
            Entity entity,
            Entity trigger,
            ComponentLookup<Target> selectedTargets,
            ComponentLookup<Health> healths,
            ComponentLookup<Character> characters,
            BufferLookup<StatMultiply> statMultipliers,
            bool isEnemy,
            NativeArray<Entity> enemies,
            NativeArray<Entity> allies)
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

            ApplyTargetConditions(targets, config, healths, characters, statMultipliers);
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

        private static void ApplyTargetConditions(
            List<Entity> targets,
            in SkillConfig config,
            ComponentLookup<Health> healths,
            ComponentLookup<Character> characters,
            BufferLookup<StatMultiply> statMultipliers)
        {
            if (targets.Count == 0 || !HasTargetConditions(config)) return;

            var selectedTarget = Entity.Null;
            foreach (var target in targets)
            {
                if (!CanEvaluateConditions(target, config, healths, characters, statMultipliers)) continue;
                if (selectedTarget == Entity.Null ||
                    IsBetterTarget(target, selectedTarget, config, healths, characters, statMultipliers))
                    selectedTarget = target;
            }

            targets.Clear();
            AddTarget(targets, selectedTarget);
        }

        private static bool HasTargetConditions(in SkillConfig config)
        {
            foreach (var condition in config.TargetConditions)
            {
                if (condition != TargetConditionType.None) return true;
            }

            return false;
        }

        private static bool CanEvaluateConditions(
            Entity target,
            in SkillConfig config,
            ComponentLookup<Health> healths,
            ComponentLookup<Character> characters,
            BufferLookup<StatMultiply> statMultipliers)
        {
            foreach (var condition in config.TargetConditions)
            {
                if (condition == TargetConditionType.None) continue;
                if (!TryGetConditionValue(target, condition, healths, characters, statMultipliers, out _))
                    return false;
            }

            return true;
        }

        private static bool IsBetterTarget(
            Entity candidate,
            Entity current,
            in SkillConfig config,
            ComponentLookup<Health> healths,
            ComponentLookup<Character> characters,
            BufferLookup<StatMultiply> statMultipliers)
        {
            foreach (var condition in config.TargetConditions)
            {
                if (condition == TargetConditionType.None) continue;

                TryGetConditionValue(candidate, condition, healths, characters, statMultipliers, out var candidateValue);
                TryGetConditionValue(current, condition, healths, characters, statMultipliers, out var currentValue);
                if (candidateValue == currentValue) continue;

                return condition is TargetConditionType.LowestHP or TargetConditionType.LowestATK
                    ? candidateValue < currentValue
                    : candidateValue > currentValue;
            }

            return false;
        }

        private static bool TryGetConditionValue(
            Entity target,
            TargetConditionType condition,
            ComponentLookup<Health> healths,
            ComponentLookup<Character> characters,
            BufferLookup<StatMultiply> statMultipliers,
            out float value)
        {
            switch (condition)
            {
                case TargetConditionType.LowestHP:
                case TargetConditionType.HighestHP:
                    if (healths.HasComponent(target))
                    {
                        value = healths[target].Value;
                        return true;
                    }
                    break;

                case TargetConditionType.LowestATK:
                case TargetConditionType.HighestATK:
                    if (characters.HasComponent(target) && statMultipliers.HasBuffer(target))
                    {
                        var stats = statMultipliers[target];
                        var attackIndex = (int)StatType.Attack;
                        if (attackIndex < stats.Length)
                        {
                            value = characters[target].GetConfig().Attack * stats[attackIndex].Value;
                            return true;
                        }
                    }
                    break;
            }

            value = 0f;
            return false;
        }
    }
}
