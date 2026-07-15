namespace vikwhite
{
    public interface IRoomFactory
    {
        Room Create(RoomType type, int level, float production, float capacity, long lastProductionCollectionUnixTime);
    }

    public class RoomFactory : IRoomFactory
    {
        private readonly DiContainer _container;

        public RoomFactory(DiContainer container)
        {
            _container = container;
        }

        public Room Create(
            RoomType type,
            int level,
            float production,
            float capacity,
            long lastProductionCollectionUnixTime)
        {
            var room = _container.Resolve<Room>();
            room.Initialize(type, level, production, capacity, lastProductionCollectionUnixTime);
            return room;
        }
    }
}
