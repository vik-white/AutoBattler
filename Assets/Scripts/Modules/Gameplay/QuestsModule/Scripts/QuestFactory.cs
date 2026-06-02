namespace vikwhite
{
    public interface IQuestFactory
    {
        Quest Create(IQuestData data);
    }

    public class QuestFactory : IQuestFactory
    {
        private readonly IRewardFactory _rewardFactory;

        public QuestFactory(IRewardFactory rewardFactory)
        {
            _rewardFactory = rewardFactory;
        }

        public Quest Create(IQuestData data)
        {
            if (data == null) return null;
            var rewards = _rewardFactory.CreateFromData(data.Rewards);
            return new Quest(data.ID, data.Type, data.Description, data.Amount, data.Global, rewards);
        }
    }
}
