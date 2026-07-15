using UniRx;

namespace vikwhite
{
    public class Room
    {
        private readonly IEventDispatcher _dispatcher;
        private readonly IRoomsService _roomsService;

        public RoomType Type;
        public ReactiveProperty<int> Level;
        public ReactiveProperty<float> Production;

        public Room(IEventDispatcher dispatcher, IRoomsService roomsService)
        {
            _dispatcher = dispatcher;
            _roomsService = roomsService;
        }

        public void Initialize(RoomType type, int level, float production)
        {
            Type = type;
            Level = new ReactiveProperty<int>(level);
            Production = new ReactiveProperty<float>(production);
            Level.Skip(1).Subscribe(value => _dispatcher.Dispatch(new ChangeRoomLevelEvent(Type, value)));
            Production.Skip(1).Subscribe(value => _dispatcher.Dispatch(new ChangeRoomProductionEvent(Type, value)));
        }

        public void Upgrade() => _roomsService.Upgrade(this);

        internal void UpgradeLevel() => Level.Value++;

        internal void AddProduction(float value) => Production.Value += value;
    }
}
