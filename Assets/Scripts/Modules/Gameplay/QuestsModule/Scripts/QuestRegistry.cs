using System.Collections.Generic;

namespace vikwhite
{
    public interface IQuestRegistry
    {
        IReadOnlyList<Quest> Quests { get; }
        void Register(Quest quest);
        void Unregister(Quest quest);
    }

    public class QuestRegistry : IQuestRegistry
    {
        private readonly List<Quest> _quests = new();

        public IReadOnlyList<Quest> Quests => _quests;

        public void Register(Quest quest)
        {
            if (quest == null) return;
            if (_quests.Contains(quest)) return;
            _quests.Add(quest);
        }

        public void Unregister(Quest quest)
        {
            if (quest == null) return;
            _quests.Remove(quest);
        }
    }
}
