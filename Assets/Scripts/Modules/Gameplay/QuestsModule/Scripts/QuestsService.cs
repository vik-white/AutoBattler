using System.Collections.Generic;
using vikwhite.Data;

namespace vikwhite
{
    public interface IQuestsService
    {
        void Initialize();
        Quest Get(string id);
        IReadOnlyCollection<Quest> GetAll();
    }

    public class QuestsService : IQuestsService
    {
        private readonly IConfigs _configs;
        private readonly IQuestFactory _questFactory;
        private readonly Dictionary<string, Quest> _quests = new();

        public QuestsService(IConfigs configs, IQuestFactory questFactory)
        {
            _configs = configs;
            _questFactory = questFactory;
        }

        public void Initialize()
        {
            if (_quests.Count > 0) return;

            foreach (var data in _configs.Quests.GetAll())
            {
                if (data == null || string.IsNullOrEmpty(data.ID)) continue;
                if (_quests.ContainsKey(data.ID)) continue;

                var quest = _questFactory.Create(data);
                if (quest == null) continue;
                _quests.Add(quest.ID, quest);
            }
        }

        public Quest Get(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return _quests.TryGetValue(id, out var quest) ? quest : null;
        }

        public IReadOnlyCollection<Quest> GetAll() => _quests.Values;
    }
}