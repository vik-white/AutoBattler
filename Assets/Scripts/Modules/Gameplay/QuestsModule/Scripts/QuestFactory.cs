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

        public QuestFactory(DiContainer container, IRewardFactory rewardFactory)
        {
            _container = container;
            _rewardFactory = rewardFactory;
        }

        public Quest Create(IQuestData data)
        {
            if (data == null) return null;
            var quest = _container.Resolve<Quest>();
            quest.Initialize(data);
            return quest;
        }
    }
}
