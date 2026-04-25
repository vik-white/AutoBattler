using Unity.Collections;
using vikwhite.Data;

namespace vikwhite.ECS
{
    public struct LocationStaticConfig : IID
    {
        public uint ID { get; set; }
        public FixedList128Bytes<CharacterLevelData> Enemies;
    }
}
