using System;
using Unity.Mathematics;

namespace vikwhite.Data
{
    public interface IHexPositionsData
    {
        string ID { get; }
        int2 C1 { get; }
        int2 C2 { get; }
        int2 C3 { get; }
        int2 C4 { get; }
        int2 C5 { get; }
    }
    
    [Serializable]
    public class HexPositionsData : IHexPositionsData
    {
        public string ID;
        public int2 C1;
        public int2 C2;
        public int2 C3;
        public int2 C4;
        public int2 C5;
        
        string IHexPositionsData.ID => ID;
        int2 IHexPositionsData.C1 => C1;
        int2 IHexPositionsData.C2 => C2;
        int2 IHexPositionsData.C3 => C3;
        int2 IHexPositionsData.C4 => C4;
        int2 IHexPositionsData.C5 => C5;
    }
}