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
}
