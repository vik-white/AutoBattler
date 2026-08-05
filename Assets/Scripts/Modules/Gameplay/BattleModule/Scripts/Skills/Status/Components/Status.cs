using Unity.Entities;

namespace vikwhite.ECS
{
    public struct Status : IComponentData
    {
        public BlobAssetReference<SkillConfig> Ability;
        public EffectType Type;
        public StatType Stat;
        public float Value;
        public float Duration;
        public float TileLeft;
        public float Period;
        public float TimeSinceLastTick;
    }
}
