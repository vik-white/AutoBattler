namespace vikwhite
{
    public interface IRoomFactory
    {
        Room Create(RoomType type, int level);
    }

    public class RoomFactory : IRoomFactory
    {
        private readonly DiContainer _container;

        public RoomFactory(DiContainer container)
        {
            _container = container;
        }

        public Room Create(RoomType type, int level)
        {
            var room = _container.Resolve<Room>();
            room.Initialize(type, level);
            return room;
        }
    }
}
