using Unity.Entities;

namespace vikwhite.ECS
{
    public struct CreateStatChange : IComponentData
    {
        public Entity Target;
        public StatData Data;
    }
}