using System;

namespace vikwhite
{
    [Serializable]
    public class RoomData
    {
        public RoomType Type;
        public int Level;
        public float Production;
        public float Capacity;
        public long LastProductionCollectionUnixTime;
    }
}
