using System;
using System.Collections.Generic;
using vikwhite.Data;

namespace vikwhite
{
    public interface IRewardsData
    {
        string ID { get; }
        IReadOnlyCollection<RewardData> Rewards { get; }
        IReadOnlyCollection<RewardData> RewardBasket { get; }
    }

    [Serializable]
    public class RewardsData: IRewardsData, ICustomJsonParser
    {
        public string ID;
        public List<RewardData> Rewards;
        public List<RewardData> RewardBasket;
        
        string IRewardsData.ID => ID;
        IReadOnlyCollection<RewardData> IRewardsData.Rewards => Rewards;
        IReadOnlyCollection<RewardData> IRewardsData.RewardBasket => RewardBasket;

        public void Parse(Dictionary<string, string> row)
        {
            Rewards = new List<RewardData>();
            foreach (var rewardString in row["Reward"].Split(';'))
            {
                if(TryParseReward(rewardString, out var reward)) Rewards.Add(reward);
            }
            
            RewardBasket = new List<RewardData>();
            foreach (var rewardString in row["RewardBasket"].Split(';'))
            {
                if(TryParseReward(rewardString, out var reward)) RewardBasket.Add(reward);
            }
        }
        
        public static bool TryParseReward(string rewardString, out RewardData data)
        {
            data = default;
            if(rewardString == "" || rewardString == null) return false;
            var parts = rewardString.Split(':');
            var typeString = parts[0].Trim();
            var idString = parts[1].Trim();
            var valueString = parts[2].Trim();
            var probability = parts.Length > 3 ? parts[3].ToFloat() : 1f;

            if (!Enum.TryParse<RewardType>(typeString, out var type)) return false;

            if (!valueString.TryParseValueRange(out var minValue, out var maxValue)) return false;

            data = new RewardData
            {
                Type = type,
                MinValue = minValue,
                MaxValue = maxValue,
                Probability = probability
            };

            switch (type)
            {
                case RewardType.Res:
                    if (!Enum.TryParse<ResourceType>(idString, out var resourceType)) return false;
                    data.ResourceType = resourceType;
                    break;
                case RewardType.Shard:
                    data.ID = idString;
                    break;
                case RewardType.ShardGroup:
                    if (!Enum.TryParse<ShardGroupType>(idString, out var shardGroupType)) return false;
                    data.ShardGroupType = shardGroupType;
                    break;
            }

            return true;
        }
    }
}