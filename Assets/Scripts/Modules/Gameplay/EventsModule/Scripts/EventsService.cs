using System.Collections.Generic;
using vikwhite.Data;

namespace vikwhite
{
    public interface IEventsService
    {
        void Initialize();
        GameEvent Get(string id);
        IReadOnlyList<GameEvent> GetAll();
    }

    public class EventsService : IEventsService
    {
        private readonly IConfigs _configs;
        private readonly List<GameEvent> _events = new();
        private readonly Dictionary<string, GameEvent> _byId = new();

        public EventsService(IConfigs configs)
        {
            _configs = configs;
        }

        public void Initialize()
        {
            if (_events.Count > 0) return;

            foreach (var data in _configs.Events.GetAll())
            {
                if (data == null || string.IsNullOrEmpty(data.ID)) continue;
                if (_byId.ContainsKey(data.ID)) continue;

                var gameEvent = new GameEvent(data.ID, data.Name, data.Type, data.Duration, new List<string>(data.Quests));
                _events.Add(gameEvent);
                _byId.Add(data.ID, gameEvent);
            }
        }

        public GameEvent Get(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return _byId.TryGetValue(id, out var gameEvent) ? gameEvent : null;
        }

        public IReadOnlyList<GameEvent> GetAll() => _events;
    }
}
