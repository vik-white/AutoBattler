using System.Collections.Generic;
using UniRx;

namespace vikwhite
{
    public class Quest
    {
        private readonly IEventDispatcher _dispatcher;

        public string ID;
        public string EventID;
        public QuestType Type;
        public string Description;
        public int Amount;
        public string TargetID;
        public ResourceType Resource;
        public bool Global;
        public ReactiveProperty<int> Progress;
        public ReactiveProperty<bool> Claimed;
        public List<Reward> Rewards;

        public Quest(IEventDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Initialize(string eventId, IQuestData data, List<Reward> rewards, int progress, bool claimed)
        {
            EventID = eventId;
            ID = data.ID;
            Type = data.Type;
            Description = data.Description;
            Amount = data.Amount;
            TargetID = data.TargetID;
            Resource = data.Resource;
            Global = data.Global;
            if (Type == QuestType.CompleteLevel && Amount <= 0) Amount = 1;
            Rewards = rewards ?? new List<Reward>();
            Progress = new ReactiveProperty<int>(progress);
            Claimed = new ReactiveProperty<bool>(claimed);
            Progress.Skip(1).Subscribe(value => _dispatcher.Dispatch(new ChangeQuestProgressEvent(EventID, ID, value)));
            Claimed.Skip(1).Subscribe(value => _dispatcher.Dispatch(new ChangeQuestClaimedEvent(EventID, ID, value)));
        }

        public bool IsCompleted => Progress.Value >= Amount;
        public bool IsClaimable => IsCompleted && Claimed.Value == false;
    }
}
