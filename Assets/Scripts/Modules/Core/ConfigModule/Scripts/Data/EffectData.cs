using System;

namespace vikwhite.ECS
{
    [Serializable]
    public struct EffectData
    {
        public EffectType Type;
        public EffectDependenceType Dependence;
        public float Value;
    }
}