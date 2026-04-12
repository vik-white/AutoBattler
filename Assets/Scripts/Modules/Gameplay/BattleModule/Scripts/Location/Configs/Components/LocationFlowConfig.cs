using Unity.Entities;

namespace vikwhite.ECS
{
    public struct LocationFlowConfig : IBufferElementData, IID
    {
        public uint ID { get; set; }
        public BlobAssetReference<BlobArrayContainer<LocationFlowStepData>> Steps;
    }
}