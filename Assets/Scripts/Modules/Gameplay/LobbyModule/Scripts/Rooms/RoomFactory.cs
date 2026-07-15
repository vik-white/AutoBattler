namespace vikwhite
{
    public interface IRoomFactory
    {
        Room Create(RoomType type, int level, float production);
    }

    public class RoomFactory : IRoomFactory
    {
        private readonly DiContainer _container;

        public RoomFactory(DiContainer container)
        {
            _container = container;
        }

        public Room Create(RoomType type, int level, float production)
        {
            var room = _container.Resolve<Room>();
            room.Initialize(type, level, production);
            return room;
        }
    }
}
