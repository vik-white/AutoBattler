using Unity.Entities;

namespace vikwhite.ECS
{
    public struct CreateStatus : IComponentData
    {
        public Entity Target;
        public StatusData Data;
    }
}