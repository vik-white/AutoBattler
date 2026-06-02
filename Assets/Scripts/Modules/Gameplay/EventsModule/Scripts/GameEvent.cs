using System.Collections.Generic;

namespace vikwhite
{
    public class GameEvent
    {
        public string ID;
        public string Name;
        public GameEventType Type;
        public int Duration;
        public List<string> QuestIds;

        public GameEvent(string id, string name, GameEventType type, int duration, List<string> questIds)
        {
            ID = id;
            Name = name;
            Type = type;
            Duration = duration;
            QuestIds = questIds ?? new List<string>();
        }
    }
}
