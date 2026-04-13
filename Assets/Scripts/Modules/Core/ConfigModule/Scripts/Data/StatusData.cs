using System;

namespace vikwhite.ECS
{
    [Serializable]
    public struct StatusData
    {
        public StatusType Type;
        public float Value;
        public float Duration;
        public float Period;
    }
}