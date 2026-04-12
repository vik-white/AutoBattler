using Unity.Entities;

namespace vikwhite.ECS
{
    public struct Time : IComponentData
    {
        public float TotalTime;
        public float DeltaTime;
        public bool IsPaused;
    }
}