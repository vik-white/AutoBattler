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
        private readonly IRewardFactory _rewardFactory;
        private readonly Dictionary<string, Quest> _quests = new();

        public QuestsService(IConfigs configs, IRewardFactory rewardFactory)
        {
            _configs = configs;
            _rewardFactory = rewardFactory;
        }

        public void Initialize()
        {
            if (_quests.Count > 0) return;

            foreach (var data in _configs.Quests.GetAll())
            {
                if (data == null || string.IsNullOrEmpty(data.ID)) continue;
                if (_quests.ContainsKey(data.ID)) continue;
                _quests.Add(data.ID, CreateQuest(data));
            }
        }

        public Quest Get(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            return _quests.TryGetValue(id, out var quest) ? quest : null;
        }

        public IReadOnlyCollection<Quest> GetAll() => _quests.Values;

        private Quest CreateQuest(IQuestData data)
        {
            var rewards = _rewardFactory.CreateFromData(data.Rewards);
            return new Quest(data.ID, data.Type, data.Description, data.Amount, data.Global, rewards);
        }
    }
}
