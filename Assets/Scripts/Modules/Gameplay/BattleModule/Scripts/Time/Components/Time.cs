using Unity.Entities;

namespace vikwhite.ECS
{
    public struct Time : IComponentData
    {
        public float DeltaTime;
        public bool IsPaused;
    }
}