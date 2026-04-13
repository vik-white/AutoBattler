using Unity.Collections;
using Unity.Entities;

namespace vikwhite.ECS
{
    public struct Effects : IComponentData
    {
        public FixedList64Bytes<EffectData> Array;
    }
}