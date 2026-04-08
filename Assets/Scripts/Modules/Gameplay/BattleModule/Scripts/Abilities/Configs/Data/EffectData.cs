using System;
using Unity.Entities;

namespace vikwhite.ECS
{
    [Serializable]
    public struct EffectData : IBufferElementData
    {
        public EffectID ID;
        public float Value;
    }
}