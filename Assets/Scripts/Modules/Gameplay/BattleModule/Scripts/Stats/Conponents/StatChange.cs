using Unity.Entities;

namespace vikwhite.ECS
{
    public struct StatChange : IComponentData
    {
        public StatID ID;
        public float Value;
    }
}