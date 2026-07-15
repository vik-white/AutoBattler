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
        public ReactiveProperty<float> Capacity;
        public ReactiveProperty<long> LastProductionCollectionUnixTime;

        public Room(IEventDispatcher dispatcher, IRoomsService roomsService)
        {
            _dispatcher = dispatcher;
            _roomsService = roomsService;
        }

        public void Initialize(
            RoomType type,
            int level,
            float production,
            float capacity,
            long lastProductionCollectionUnixTime)
        {
            Type = type;
            Level = new ReactiveProperty<int>(level);
            Production = new ReactiveProperty<float>(production);
            Capacity = new ReactiveProperty<float>(capacity);
            LastProductionCollectionUnixTime = new ReactiveProperty<long>(lastProductionCollectionUnixTime);
            Level.Skip(1).Subscribe(value => _dispatcher.Dispatch(new ChangeRoomLevelEvent(Type, value)));
            Production.Skip(1).Subscribe(value => _dispatcher.Dispatch(new ChangeRoomProductionEvent(Type, value)));
            Capacity.Skip(1).Subscribe(value => _dispatcher.Dispatch(new ChangeRoomCapacityEvent(Type, value)));
            LastProductionCollectionUnixTime.Skip(1).Subscribe(value =>
                _dispatcher.Dispatch(new ChangeRoomProductionCollectionTimeEvent(Type, value)));
        }

        public void Upgrade() => _roomsService.Upgrade(this);

        internal void UpgradeLevel() => Level.Value++;

        internal void AddProduction(float value) => Production.Value += value;

        internal void AddCapacity(float value) => Capacity.Value += value;

        internal void SetLastProductionCollectionTime(long unixTime) =>
            LastProductionCollectionUnixTime.Value = unixTime;
    }
}
