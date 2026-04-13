using System;

namespace vikwhite.ECS
{
    [Serializable]
    public struct StatData
    {
        public StatType Type;
        public float Value;
        public float Duration;
    }
}