using Unity.Entities;

namespace vikwhite.ECS
{
    public struct Status : IComponentData
    {
        public float Value;
        public float Duration;
        public float TileLeft;
        public float Period;
        public float TimeSinceLastTick;
    }
}