using System;

namespace vikwhite.ECS
{
    [Serializable]
    public struct StatusData
    {
        public EffectType Type;
        public StatType Stat;
        public bool UseStat;
        public float Value;
        public float Duration;
        public float Period;
    }
}
