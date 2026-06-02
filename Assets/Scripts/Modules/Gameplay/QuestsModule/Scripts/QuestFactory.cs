namespace vikwhite
{
    public interface IQuestFactory
    {
        Quest Create(string eventId, IQuestData data);
    }

    public class QuestFactory : IQuestFactory
    {
        private readonly DiContainer _container;
        private readonly IRewardFactory _rewardFactory;
        private readonly IQuestRegistry _registry;
        private readonly IProfileService _profile;
        private readonly IEventDispatcher _dispatcher;

        public QuestFactory(
            DiContainer container,
            IRewardFactory rewardFactory,
            IQuestRegistry registry,
            IProfileService profile,
            IEventDispatcher dispatcher)
        {
            _container = container;
            _rewardFactory = rewardFactory;
            _registry = registry;
            _profile = profile;
            _dispatcher = dispatcher;
        }

        public Quest Create(string eventId, IQuestData data)
        {
            if (data == null) return null;

            int progress = 0;
            bool claimed = false;
            var existing = FindProfileData(eventId, data.ID);
            if (existing != null)
            {
                progress = existing.Progress;
                claimed = existing.Claimed;
            }
            else
            {
                _dispatcher.Dispatch(new CreateQuestProfileEvent(eventId, data.ID));
            }

            var rewards = _rewardFactory.CreateFromData(data.Rewards);
            var quest = _container.Resolve<Quest>();
            quest.Initialize(eventId, data, rewards, progress, claimed);
            _registry.Register(quest);
            return quest;
        }

        private QuestProfileData FindProfileData(string eventId, string questId)
        {
            for (int i = 0; i < _profile.Data.Quests.Count; i++)
            {
                var entry = _profile.Data.Quests[i];
                if (entry.EventID == eventId && entry.QuestID == questId) return entry;
            }
            return null;
        }
    }
}
