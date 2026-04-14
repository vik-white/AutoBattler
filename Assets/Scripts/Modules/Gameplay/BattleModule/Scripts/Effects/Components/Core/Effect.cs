using Unity.Entities;

namespace vikwhite.ECS
{
    public struct Effect : IComponentData
    {
        public AbilityLevelData Ability;
        public float Value;
    }
}