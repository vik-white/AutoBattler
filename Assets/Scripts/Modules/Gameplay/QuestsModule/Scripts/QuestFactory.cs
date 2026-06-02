namespace vikwhite
{
    public interface IQuestFactory
    {
        Quest Create(IQuestData data);
    }

    public class QuestFactory : IQuestFactory
    {
        private readonly DiContainer _container;
        private readonly IRewardFactory _rewardFactory;
        private readonly IQuestRegistry _registry;

        public QuestFactory(DiContainer container, IRewardFactory rewardFactory, IQuestRegistry registry)
        {
            _container = container;
            _rewardFactory = rewardFactory;
            _registry = registry;
        }

        public Quest Create(IQuestData data)
        {
            if (data == null) return null;
            var quest = _container.Resolve<Quest>();
            quest.Initialize(data);
            _registry.Register(quest);
            return quest;
        }
    }
}
