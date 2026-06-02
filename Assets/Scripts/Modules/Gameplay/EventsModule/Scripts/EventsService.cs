using System.Collections.Generic;
using vikwhite.Data;

namespace vikwhite
{
    public interface IEventsService
    {
        void Initialize();
        IReadOnlyList<GameEvent> GetAll();
    }

    public class EventsService : IEventsService
    {
        private readonly IConfigs _configs;
        private readonly IGameEventFactory _eventFactory;
        private readonly List<GameEvent> _events = new();

        public EventsService(IConfigs configs, IGameEventFactory eventFactory)
        {
            _configs = configs;
            _eventFactory = eventFactory;
        }

        public void Initialize()
        {
            if (_events.Count > 0) return;

            foreach (var data in _configs.Events.GetAll())
            {
                if (data == null || string.IsNullOrEmpty(data.ID)) continue;

                var gameEvent = _eventFactory.Create(data);
                if (gameEvent == null) continue;

                _events.Add(gameEvent);
            }
        }

        public IReadOnlyList<GameEvent> GetAll() => _events;
    }
}