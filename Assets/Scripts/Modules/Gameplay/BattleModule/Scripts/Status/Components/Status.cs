using Unity.Entities;

namespace vikwhite.ECS
{
    public struct Status : IComponentData
    {
        public AbilityLevelData Ability;
        public EffectType Type;
        public float Value;
        public float Duration;
        public float TileLeft;
        public float Period;
        public float TimeSinceLastTick;
    }
}