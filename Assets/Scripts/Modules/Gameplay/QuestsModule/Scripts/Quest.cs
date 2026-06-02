using System.Collections.Generic;
using UniRx;

namespace vikwhite
{
    public class Quest
    {
        private readonly IRewardFactory _rewardFactory;
        
        public string ID;
        public QuestType Type;
        public string Description;
        public int Amount;
        public string TargetID;
        public bool Global;
        public ReactiveProperty<int> Progress;
        public ReactiveProperty<bool> Claimed;
        public List<Reward> Rewards;

        public Quest(IRewardFactory rewardFactory)
        {
            _rewardFactory = rewardFactory;
        }
        
        public void Initialize(IQuestData data)
        {
            ID = data.ID;
            Type = data.Type;
            Description = data.Description;
            Amount = data.Amount;
            TargetID = data.TargetID;
            Global = data.Global;
            if (Type == QuestType.CompleteLevel && Amount <= 0) Amount = 1;
            Rewards = _rewardFactory.CreateFromData(data.Rewards);
            Progress = new ReactiveProperty<int>(0);
            Claimed = new ReactiveProperty<bool>(false);
        }

        public bool IsCompleted => Progress.Value >= Amount;
        public bool IsClaimable => IsCompleted && Claimed.Value == false;
    }
}
