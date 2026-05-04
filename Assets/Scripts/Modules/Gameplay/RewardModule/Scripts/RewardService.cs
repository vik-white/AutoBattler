using System.Collections.Generic;
using UnityEngine;

namespace vikwhite
{
    public interface IRewardService
    {
        void Add(Reward reward);
        void Add(IEnumerable<Reward> rewards);
    }

    public class RewardService : IRewardService
    {
        private readonly IResourceService _resources;
        private readonly ICharactersService _characters;

        public RewardService(IResourceService resources, ICharactersService characters)
        {
            _resources = resources;
            _characters = characters;
        }

        public void Add(IEnumerable<Reward> rewards)
        {
            if (rewards == null) return;
            foreach (var reward in rewards) Add(reward);
        }

        public void Add(Reward reward)
        {
            if (reward == null || reward.Value <= 0) return;

            switch (reward)
            {
                case ResourceReward res:
                    _resources.Add(res.ResourceType, res.Value);
                    break;
                case ShardReward shard:
                    _characters.GetCharacter(shard.ID).AddShards(shard.Value);
                    break;
            }
        }
    }
}
