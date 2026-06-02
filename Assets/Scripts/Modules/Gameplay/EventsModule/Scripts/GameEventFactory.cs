using System.Collections.Generic;

namespace vikwhite
{
    public interface IGameEventFactory
    {
        GameEvent Create(IEventData data);
    }

    public class GameEventFactory : IGameEventFactory
    {
        public GameEvent Create(IEventData data)
        {
            if (data == null) return null;
            var questIds = data.Quests != null ? new List<string>(data.Quests) : new List<string>();
            return new GameEvent(data.ID, data.Name, data.Type, data.Duration, questIds);
        }
    }
}
