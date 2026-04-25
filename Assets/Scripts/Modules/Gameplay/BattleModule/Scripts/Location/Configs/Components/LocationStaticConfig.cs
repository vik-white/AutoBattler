using Unity.Collections;

namespace vikwhite.ECS
{
    public struct LocationStaticConfig : IID
    {
        public uint ID { get; set; }
        public FixedList128Bytes<uint> Enemies;
    }
}
