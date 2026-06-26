using UnityEngine;
using vikwhite.Data;

namespace vikwhite
{
    public class RewardItemViewModel : WindowViewModel<Reward>
    {
        private readonly IConfigs _configs;
        public int Value;
        public Sprite Icon;
        public Sprite Shard;
        public Sprite RarityBG;

        public RewardItemViewModel(Reward model, IConfigs configs) : base(model)
        {
            _configs = configs;
            Value = model.Value;
            Icon = GetIcon(model);
            RarityBG = GetRarityBG(model);
            if (model is ShardReward shard) Shard = configs.Characters.Get(shard.ID).ShardImage;
        }

        private Sprite GetIcon(Reward reward)
        {
            switch (reward)
            {
                case ResourceReward res: return _configs.UI.ResourceIcons[res.ResourceType];
                case ShardReward shard: return GetShardIcon(shard);
                default: return null;
            }
        }
        
        private Sprite GetShardIcon(ShardReward shard)
        {
            switch (_configs.Characters.Get(shard.ID).Rarity)
            {
                case RarityType.Rare: return _configs.UI.ResourceIcons[ResourceType.ShardRare];
                case RarityType.Epic: return _configs.UI.ResourceIcons[ResourceType.ShardEpic];
                case RarityType.Legendary: return _configs.UI.ResourceIcons[ResourceType.ShardLegendary];
                default: return _configs.UI.ResourceIcons[ResourceType.ShardRare];
            }
        }
        
        private Sprite GetRarityBG(Reward reward)
        {
            switch (reward)
            {
                case ResourceReward res:
                {
                    var rarity = _configs.Resources.Get(res.ResourceType.ToString()).Rarity;
                    return _configs.UI.Rarities[rarity].RewardBG;
                }
                case ShardReward shard: return _configs.UI.Rarities[_configs.Characters.Get(shard.ID).Rarity].RewardBG;
                default: return null;
            }
        }
    }
}