using System.Collections.Generic;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite
{
    public interface IRewardService
    {
        List<Reward> Create(string id);
    }

    public class RewardService : IRewardService
    {
        private readonly IConfigs _configs;

        public RewardService(IConfigs configs)
        {
            _configs = configs;
        }

        public List<Reward> Create(string id)
        {
            var result = new List<Reward>();
            var data = _configs.Rewards.Get(id);

            foreach (var rewardData in data.Rewards)
            {
                if (rewardData.Probability < 1f && Random.value > rewardData.Probability) continue;

                var reward = CreateReward(rewardData);
                if (reward == null) continue;

                reward.Value = Random.Range(rewardData.MinValue, rewardData.MaxValue + 1);
                result.Add(reward);
            }
            return result;
        }

        private static Reward CreateReward(RewardData data)
        {
            switch (data.Type)
            {
                case RewardType.Res:
                    return new ResourceReward { ResourceType = data.ResourceType };
                case RewardType.Shard:
                    return new ShardReward { ID = data.ID };
                case RewardType.ShardGroup:
                    return new ShardGroupReward { ShardGroupType = data.ShardGroupType };
                default:
                    return null;
            }
        }
    }
}