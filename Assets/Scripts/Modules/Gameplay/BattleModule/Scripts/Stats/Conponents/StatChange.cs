using Unity.Entities;

namespace vikwhite.ECS
{
    public struct StatChange : IComponentData
    {
        public Entity Target;
        public StatType Type;
        public float Value;
    }
}