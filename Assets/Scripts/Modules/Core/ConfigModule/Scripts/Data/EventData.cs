using System;
using System.Collections.Generic;
using vikwhite.Data;

namespace vikwhite
{
    public interface IEventData
    {
        string ID { get; }
        string Name { get; }
        GameEventType Type { get; }
        int Duration { get; }
        IReadOnlyList<string> Quests { get; }
    }

    [Serializable]
    public class EventData : IEventData, ICustomJsonParser
    {
        public string ID;
        public string Name;
        public GameEventType Type;
        public int Duration;
        public List<string> Quests;

        string IEventData.ID => ID;
        string IEventData.Name => Name;
        GameEventType IEventData.Type => Type;
        int IEventData.Duration => Duration;
        IReadOnlyList<string> IEventData.Quests => Quests;

        public void Parse(Dictionary<string, string> row)
        {
            Quests = new List<string>();
            if (!row.TryGetValue("Quests", out var questsString)) return;
            if (string.IsNullOrEmpty(questsString)) return;

            foreach (var questId in questsString.Split(';'))
            {
                var trimmed = questId.Trim();
                if (!string.IsNullOrEmpty(trimmed)) Quests.Add(trimmed);
            }
        }
    }
}
