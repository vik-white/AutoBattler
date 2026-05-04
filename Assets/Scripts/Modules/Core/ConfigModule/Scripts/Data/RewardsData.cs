using System;
using System.Collections.Generic;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite
{
    public enum RewardType
    {
        Res = 0,
        Shard = 1,
        ShardGroup = 2
    }
    
    public enum ShardGroupType
    {
        Any = 0,
    }
    
    [Serializable]
    public struct RewardData
    {
        public RewardType Type;
        public ResourceType ResourceType;
        public ShardGroupType ShardGroupType;
        public string ID;
        public int MinValue;
        public int MaxValue;
        public float Probability;
    }

    public interface IRewardsData
    {
        string ID { get; }
        IReadOnlyCollection<RewardData> Rewards { get; }
    }

    [Serializable]
    public class RewardsData: IRewardsData, ICustomJsonParser
    {
        public string ID;
        public List<RewardData> Rewards;
        
        string IRewardsData.ID => ID;
        IReadOnlyCollection<RewardData> IRewardsData.Rewards => Rewards;

        public void Parse(Dictionary<string, string> row)
        {
            Rewards = new List<RewardData>();
            foreach (var rewardString in row["Reward"].Split(';'))
            {
                var parts = rewardString.Split(':');
                var typeString = parts[0].Trim();
                var idString = parts[1].Trim();
                var valueString = parts[2].Trim();
                var probability = parts.Length > 3 ? parts[3].ToFloat() : 1f;

                if (!Enum.TryParse<RewardType>(typeString, out var type)) continue;

                if (!valueString.TryParseValueRange(out var minValue, out var maxValue)) continue;

                var reward = new RewardData
                {
                    Type = type,
                    MinValue = minValue,
                    MaxValue = maxValue,
                    Probability = probability
                };

                switch (type)
                {
                    case RewardType.Res:
                        if (!Enum.TryParse<ResourceType>(idString, out var resourceType)) continue;
                        reward.ResourceType = resourceType;
                        break;
                    case RewardType.Shard:
                        reward.ID = idString;
                        break;
                    case RewardType.ShardGroup:
                        if (!Enum.TryParse<ShardGroupType>(idString, out var shardGroupType)) continue;
                        reward.ShardGroupType = shardGroupType;
                        break;
                }

                Rewards.Add(reward);
            }
        }
    }
}