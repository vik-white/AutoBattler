using Unity.Collections;
using Unity.Entities;

namespace vikwhite.ECS
{
    public struct AbilityLevelsConfig : IBufferElementData, IID
    {
        public uint ID { get; set; }
        public AbilityType Type;
        public BlobAssetReference<BlobArrayContainer<AbilityConfig>> Levels;
    }
}