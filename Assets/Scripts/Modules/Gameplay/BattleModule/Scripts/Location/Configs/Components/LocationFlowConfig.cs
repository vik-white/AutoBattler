using Unity.Entities;

namespace vikwhite.ECS
{
    public struct LocationFlowConfig : IID
    {
        public uint ID { get; set; }
        public BlobArray<LocationFlowStepData> Steps;
    }
}
