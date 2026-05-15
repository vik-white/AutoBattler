using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

namespace vikwhite.ECS
{
    public static partial class SkillHandler
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
    }
}
