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
        public ReactiveProperty<long> LastProductionCollectionUnixTime;

        public Room(IEventDispatcher dispatcher, IRoomsService roomsService)
        {
            _dispatcher = dispatcher;
            _roomsService = roomsService;
        }

        public void Initialize(RoomType type, int level, float production, long lastProductionCollectionUnixTime)
        {
            Type = type;
            Level = new ReactiveProperty<int>(level);
            Production = new ReactiveProperty<float>(production);
            LastProductionCollectionUnixTime = new ReactiveProperty<long>(lastProductionCollectionUnixTime);
            Level.Skip(1).Subscribe(value => _dispatcher.Dispatch(new ChangeRoomLevelEvent(Type, value)));
            Production.Skip(1).Subscribe(value => _dispatcher.Dispatch(new ChangeRoomProductionEvent(Type, value)));
            LastProductionCollectionUnixTime.Skip(1).Subscribe(value =>
                _dispatcher.Dispatch(new ChangeRoomProductionCollectionTimeEvent(Type, value)));
        }

        public void Upgrade() => _roomsService.Upgrade(this);

        internal void UpgradeLevel() => Level.Value++;

        internal void AddProduction(float value) => Production.Value += value;

        internal void SetLastProductionCollectionTime(long unixTime) =>
            LastProductionCollectionUnixTime.Value = unixTime;
    }
}
