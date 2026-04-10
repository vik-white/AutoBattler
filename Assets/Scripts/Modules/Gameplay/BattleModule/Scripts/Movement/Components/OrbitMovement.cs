using Unity.Entities;

namespace vikwhite.ECS
{
    public struct OrbitMovement : IComponentData
    {
        public float Radius;
        public float Phase;
    }
}