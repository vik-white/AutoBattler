using UnityEngine;
using vikwhite.Data;

namespace vikwhite
{
    public class RewardItemViewModel : WindowViewModel<Reward>
    {
        private readonly IConfigs _configs;
        public int Value;
        public Sprite Icon;
        public Sprite RarityBG;

        public RewardItemViewModel(Reward model, IConfigs configs) : base(model)
        {
            _configs = configs;
            Value = model.Value;
            Icon = GetIcon(model);
            RarityBG = GetRarityBG(model);
        }

        private Sprite GetIcon(Reward reward)
        {
            switch (reward)
            {
                case ResourceReward res: return _configs.ResourceIcons[res.ResourceType];
                case ShardReward res: return _configs.ResourceIcons[ResourceType.ShardEpic];
                case ClassBookReward book: return _configs.ResourceIcons[ResourceType.Book];
                default: return null;
            }
        }
        
        private Sprite GetRarityBG(Reward reward)
        {
            switch (reward)
            {
                case ResourceReward res: return _configs.RarityBG[RarityType.Common];
                case ShardReward shard: return _configs.RarityBG[_configs.Characters.Get(shard.ID).Rarity];
                case ClassBookReward book: return _configs.RarityBG[RarityType.Epic];
                case ClassShardReward classShard: return _configs.RarityBG[classShard.Rarity];
                default: return null;
            }
        }
    }
}