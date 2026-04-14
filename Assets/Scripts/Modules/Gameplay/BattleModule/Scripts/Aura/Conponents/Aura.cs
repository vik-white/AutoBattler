using Unity.Entities;

namespace vikwhite.ECS
{
    public struct Aura : IComponentData
    {
        public float Interval;
        public float TileLeft;
    }
}