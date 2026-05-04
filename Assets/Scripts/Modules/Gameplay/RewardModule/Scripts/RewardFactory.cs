using System.Collections.Generic;
using UnityEngine;
using vikwhite.Data;

namespace vikwhite
{
    public interface IRewardFactory
    {
        List<Reward> Create(string id);
    }

    public class RewardFactory : IRewardFactory
    {
        private readonly IConfigs _configs;

        public RewardFactory(IConfigs configs)
        {
            _configs = configs;
        }

        public List<Reward> Create(string id)
        {
            var result = new List<Reward>();
            var data = _configs.Rewards.Get(id);
            if (data == null) return result;

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

        private Reward CreateReward(RewardData data)
        {
            switch (data.Type)
            {
                case RewardType.Res:
                    return new ResourceReward { ResourceType = data.ResourceType };
                case RewardType.Shard:
                    return new ShardReward { ID = data.ID };
                case RewardType.ShardGroup:
                    return new ShardReward { ID = ResolveShardGroup(data.ShardGroupType) };
                default:
                    return null;
            }
        }

        private string ResolveShardGroup(ShardGroupType groupType)
        {
            switch (groupType)
            {
                case ShardGroupType.Any:
                    return PickRandomCharacterId(c => c.Squad);
                default:
                    return null;
            }
        }

        private string PickRandomCharacterId(System.Func<ICharacterData, bool> predicate)
        {
            var all = _configs.Characters.GetAll();
            string picked = null;
            int matches = 0;
            for (int i = 0; i < all.Count; i++)
            {
                var character = all[i];
                if (!predicate(character)) continue;

                matches++;
                if (Random.Range(0, matches) == 0) picked = character.ID;
            }
            return picked;
        }
    }
}
