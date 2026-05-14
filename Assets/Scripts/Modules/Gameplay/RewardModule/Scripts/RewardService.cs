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
        private readonly IClassShardService _classShards;
        private readonly IClassBookService _classBooks;
        private readonly ICharactersService _characters;

        public RewardService(IResourceService resources, IClassShardService classShards, IClassBookService classBooks, ICharactersService characters)
        {
            _resources = resources;
            _classShards = classShards;
            _classBooks = classBooks;
            _characters = characters;
        }

        public void Add(IEnumerable<Reward> rewards)
        {
            foreach (var reward in rewards) Add(reward);
        }

        public void Add(Reward reward)
        {
            switch (reward)
            {
                case ResourceReward res:
                    _resources.Add(res.ResourceType, res.Value);
                    break;
                case ClassShardReward classShard:
                    _classShards.Add(classShard.Class, classShard.Rarity, classShard.Value);
                    break;
                case ClassBookReward classBook:
                    _classBooks.Add(classBook.Class, classBook.Value);
                    break;
                case ShardReward shard:
                    _characters.GetCharacter(shard.ID).AddShards(shard.Value);
                    break;
            }
        }
    }
}