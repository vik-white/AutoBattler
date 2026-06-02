using System.Collections.Generic;
using UniRx;

namespace vikwhite
{
    public class Quest
    {
        public string ID;
        public QuestType Type;
        public string Description;
        public int Amount;
        public bool Global;
        public ReactiveProperty<int> Progress;
        public ReactiveProperty<bool> Claimed;
        public List<Reward> Rewards;

        public Quest(string id, QuestType type, string description, int amount, bool global, List<Reward> rewards)
        {
            ID = id;
            Type = type;
            Description = description;
            Amount = amount;
            Global = global;
            Rewards = rewards;
            Progress = new ReactiveProperty<int>(0);
            Claimed = new ReactiveProperty<bool>(false);
        }

        public bool IsCompleted => Progress.Value >= Amount;
        public bool IsClaimable => IsCompleted && Claimed.Value == false;
    }
}
