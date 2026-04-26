using Unity.Collections;
using Unity.Entities;

namespace vikwhite.ECS
{
    public struct LocationStaticConfigsBlob : IComponentData
    {
        public BlobAssetReference<BlobArrayContainer<LocationStaticConfig>> Value;
    }

    public struct LocationFlowConfigsBlob : IComponentData
    {
        public BlobAssetReference<BlobArrayContainer<LocationFlowConfig>> Value;
    }

    public struct HexPositionsConfigsBlob : IComponentData
    {
        public BlobAssetReference<BlobArrayContainer<HexPositionsConfig>> Value;
    }
}
