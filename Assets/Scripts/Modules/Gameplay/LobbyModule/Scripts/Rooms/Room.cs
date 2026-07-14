using UniRx;

namespace vikwhite
{
    public class Room
    {
        private readonly IEventDispatcher _dispatcher;

        public RoomType Type;
        public ReactiveProperty<int> Level;

        public Room(IEventDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Initialize(RoomType type, int level)
        {
            Type = type;
            Level = new ReactiveProperty<int>(level);
            Level.Skip(1).Subscribe(value => _dispatcher.Dispatch(new ChangeRoomLevelEvent(Type, value)));
        }

        public void Upgrade()
        {
            Level.Value++;
        }
    }
}
