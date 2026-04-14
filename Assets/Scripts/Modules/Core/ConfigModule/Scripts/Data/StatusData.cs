using System;

namespace vikwhite.ECS
{
    [Serializable]
    public struct StatusData
    {
        public EffectType Type;
        public float Value;
        public float Duration;
        public float Period;
    }
}