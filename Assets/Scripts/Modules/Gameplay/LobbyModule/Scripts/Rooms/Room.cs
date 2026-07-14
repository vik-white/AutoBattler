namespace vikwhite
{
    public class Room
    {
        public RoomType Type;
        public int Level;

        public Room(RoomType type, int level)
        {
            Type = type;
            Level = level;
        }
    }
}