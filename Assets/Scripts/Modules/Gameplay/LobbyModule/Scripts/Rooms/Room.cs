using UniRx;

namespace vikwhite
{
    public class Room
    {
        public RoomType Type;
        public ReactiveProperty<int> Level;

        public Room(RoomType type, int level)
        {
            Type = type;
            Level = new ReactiveProperty<int>(level);
        }

        public void Upgrade()
        {
            Level.Value++;
        }
    }
}