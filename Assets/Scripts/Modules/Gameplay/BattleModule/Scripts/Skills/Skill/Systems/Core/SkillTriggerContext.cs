using Unity.Entities;
using Unity.Transforms;

namespace vikwhite.ECS
{
    public struct SkillTriggerContext
    {
        public readonly ComponentLookup<LocalTransform> Transforms;
        public readonly ComponentLookup<Character> Characters;
        public readonly ComponentLookup<Enemy> Enemies;
        public readonly ComponentLookup<Dead> Dead;
        public readonly ComponentLookup<Target> Targets;

        public SkillTriggerContext(
            ComponentLookup<LocalTransform> transforms,
            ComponentLookup<Character> characters,
            ComponentLookup<Enemy> enemies,
            ComponentLookup<Dead> dead,
            ComponentLookup<Target> targets)
        {
            Transforms = transforms;
            Characters = characters;
            Enemies = enemies;
            Dead = dead;
            Targets = targets;
        }

        public bool IsAliveCharacter(Entity entity)
        {
            return Characters.HasComponent(entity) && !Dead.HasComponent(entity);
        }
    }
}
