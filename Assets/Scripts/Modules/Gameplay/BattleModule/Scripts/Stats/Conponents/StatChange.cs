using Unity.Entities;

namespace vikwhite.ECS
{
    public struct StatChange : IComponentData
    {
        public StatType Type;
        public float Value;
    }
}