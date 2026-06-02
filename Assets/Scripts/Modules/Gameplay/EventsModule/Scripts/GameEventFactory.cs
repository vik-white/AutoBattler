using System.Collections.Generic;
using vikwhite.Data;

namespace vikwhite
{
    public interface IGameEventFactory
    {
        GameEvent Create(IEventData data);
    }

    public class GameEventFactory : IGameEventFactory
    {
        private readonly IConfigs _configs;
        private readonly IQuestFactory _questFactory;

        public GameEventFactory(IConfigs configs, IQuestFactory questFactory)
        {
            _configs = configs;
            _questFactory = questFactory;
        }

        public GameEvent Create(IEventData data)
        {
            if (data == null) return null;
            var quests = BuildQuests(data);
            return new GameEvent(data.ID, data.Name, data.Type, data.Duration, quests);
        }

        private List<Quest> BuildQuests(IEventData data)
        {
            var quests = new List<Quest>();
            if (data.Type != GameEventType.Quest || data.Quests == null) return quests;

            var seen = new HashSet<string>();
            foreach (var rawId in data.Quests)
            {
                var id = rawId?.Trim();
                if (string.IsNullOrEmpty(id) || !seen.Add(id)) continue;

                var questData = _configs.Quests.Get(id);
                if (questData == null) continue;

                var quest = _questFactory.Create(data.ID, questData);
                if (quest != null) quests.Add(quest);
            }

            return quests;
        }
    }
}
