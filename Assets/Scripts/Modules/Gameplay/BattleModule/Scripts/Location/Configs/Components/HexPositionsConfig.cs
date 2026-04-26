using Unity.Collections;
using Unity.Mathematics;
using vikwhite.Data;

namespace vikwhite.ECS
{
    public struct HexPositionsConfig : IID
    {
        public uint ID { get; set; }
        public int2 C1;
        public int2 C2;
        public int2 C3;
        public int2 C4;
        public int2 C5;
    }
}
