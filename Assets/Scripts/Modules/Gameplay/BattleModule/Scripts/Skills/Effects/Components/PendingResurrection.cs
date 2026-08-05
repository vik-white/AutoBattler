using Unity.Entities;

namespace vikwhite.ECS
{
    public struct PendingResurrection : IComponentData
    {
        public float HealthPercentage;
    }
}
