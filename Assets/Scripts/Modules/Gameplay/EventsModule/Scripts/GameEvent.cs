using System.Collections.Generic;

namespace vikwhite
{
    public class GameEvent
    {
        public string ID;
        public string Name;
        public GameEventType Type;
        public int Duration;
        public List<Quest> Quests;

        public GameEvent(string id, string name, GameEventType type, int duration, List<Quest> quests)
        {
            ID = id;
            Name = name;
            Type = type;
            Duration = duration;
            Quests = quests ?? new List<Quest>();
        }
    }
}
