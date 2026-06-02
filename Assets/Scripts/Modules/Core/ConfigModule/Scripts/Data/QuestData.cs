using System;
using System.Collections.Generic;
using vikwhite.Data;

namespace vikwhite
{
    public interface IQuestData
    {
        string ID { get; }
        QuestType Type { get; }
        string Value { get; }
        bool Global { get; }
        string Description { get; }
        IReadOnlyCollection<RewardData> Rewards { get; }
        int Amount { get; }
        string TargetID { get; }
        ResourceType Resource { get; }
    }

    [Serializable]
    public class QuestData : IQuestData, ICustomJsonParser
    {
        public string ID;
        public QuestType Type;
        public string Value;
        public bool Global;
        public string Description;
        public List<RewardData> Rewards;
        public int Amount;
        public string TargetID;
        public ResourceType Resource;

        string IQuestData.ID => ID;
        QuestType IQuestData.Type => Type;
        string IQuestData.Value => Value;
        bool IQuestData.Global => Global;
        string IQuestData.Description => Description;
        IReadOnlyCollection<RewardData> IQuestData.Rewards => Rewards;
        int IQuestData.Amount => Amount;
        string IQuestData.TargetID => TargetID;
        ResourceType IQuestData.Resource => Resource;

        public void Parse(Dictionary<string, string> row)
        {
            ParseValue();
            ParseRewards(row);
        }

        private void ParseValue()
        {
            TargetID = null;
            Amount = 0;
            Resource = default;

            if (string.IsNullOrWhiteSpace(Value)) return;

            switch (Type)
            {
                case QuestType.CompleteLevel:
                    TargetID = Value.Trim();
                    break;

                case QuestType.CompleteLevels:
                    int.TryParse(Value.Trim(), out Amount);
                    break;

                case QuestType.CharacterLevelUpAmount:
                case QuestType.CharacterReachLevel:
                {
                    var parts = Value.Split(':');
                    if (parts.Length < 2) break;
                    TargetID = parts[0].Trim();
                    int.TryParse(parts[1].Trim(), out Amount);
                    break;
                }

                case QuestType.CollectResource:
                case QuestType.SpendResource:
                {
                    var parts = Value.Split(':');
                    if (parts.Length < 2) break;
                    Enum.TryParse(parts[0].Trim(), out Resource);
                    int.TryParse(parts[1].Trim(), out Amount);
                    break;
                }
            }
        }

        private void ParseRewards(Dictionary<string, string> row)
        {
            Rewards = new List<RewardData>();
            if (!row.TryGetValue("Rewards", out var rewardsString)) return;
            if (string.IsNullOrEmpty(rewardsString)) return;

            foreach (var rewardString in rewardsString.Split(';'))
            {
                if (RewardsData.TryParseReward(rewardString, out var reward)) Rewards.Add(reward);
            }
        }
    }
}
