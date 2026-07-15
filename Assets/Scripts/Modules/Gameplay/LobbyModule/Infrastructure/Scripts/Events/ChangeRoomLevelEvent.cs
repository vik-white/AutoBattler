namespace vikwhite
{
    public class ChangeRoomLevelEvent
    {
        public RoomType Type;
        public int Level;

        public ChangeRoomLevelEvent(RoomType type, int level)
        {
            Type = type;
            Level = level;
        }
    }

    public class ChangeRoomProductionEvent
    {
        public RoomType Type;
        public float Production;

        public ChangeRoomProductionEvent(RoomType type, float production)
        {
            Type = type;
            Production = production;
        }
    }

    public class ChangeRoomProductionCollectionTimeEvent
    {
        public RoomType Type;
        public long UnixTime;

        public ChangeRoomProductionCollectionTimeEvent(RoomType type, long unixTime)
        {
            Type = type;
            UnixTime = unixTime;
        }
    }
}
