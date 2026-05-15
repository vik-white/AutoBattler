using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

namespace vikwhite.ECS
{
    public static class SkillHandler
    {
        public static List<Entity> GetTargets(BlobAssetReference<SkillConfig> skill, Entity entity, bool isEnemy, NativeArray<Entity> enemies, NativeArray<Entity> allies)
        {
            var targets = new List<Entity>();
            var config = skill.Value;
            
            if (config.Targets.Contains(TargetType.Self)) 
                targets.Add(entity);
            
            if (config.Targets.Contains(TargetType.Allies))
            {
                if (!isEnemy)
                {
                    foreach (var ally in allies)
                    {
                        if(ally != entity) targets.Add(ally);
                    }
                }
                else
                {
                    foreach (var ally in enemies)
                    {
                        if(ally != entity) targets.Add(ally);
                    }
                }
            }
            
            if (config.Targets.Contains(TargetType.Enemies))
            {
                if (!isEnemy)
                {
                    foreach (var enemy in enemies)
                    {
                        if(enemy != entity) targets.Add(enemy);
                    }
                }
                else
                {
                    foreach (var enemy in allies)
                    {
                        if(enemy != entity) targets.Add(enemy);
                    }
                }
            }
            return targets;
        }
    }
}
