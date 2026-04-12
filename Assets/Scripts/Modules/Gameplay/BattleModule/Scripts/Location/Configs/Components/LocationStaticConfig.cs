using Unity.Collections;
using Unity.Entities;

namespace vikwhite.ECS
{
    public struct LocationStaticConfig : IBufferElementData, IID
    {
        public uint ID { get; set; }
        public FixedList128Bytes<uint> Enemies;
    }
}