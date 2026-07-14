using UniRx;

namespace vikwhite
{
    public class Room
    {
        private readonly IEventDispatcher _dispatcher;

        public RoomType Type;
        public ReactiveProperty<int> Level;

        public Room(RoomType type, int level, IEventDispatcher dispatcher)
        {
            Type = type;
            Level = new ReactiveProperty<int>(level);
            _dispatcher = dispatcher;
            Level.Skip(1).Subscribe(value => _dispatcher.Dispatch(new ChangeRoomLevelEvent(Type, value)));
        }

        public void Upgrade()
        {
            Level.Value++;
        }
    }
}
