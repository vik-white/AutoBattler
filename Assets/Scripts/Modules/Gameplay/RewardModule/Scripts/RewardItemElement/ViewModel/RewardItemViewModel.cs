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
                case ClassBookReward book: return GetBookIcon(book);
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
        
        private Sprite GetBookIcon(ClassBookReward book)
        {
            switch (book.Class)
            {
                case CharacterClassType.Tank: return _configs.UI.ResourceIcons[ResourceType.BookTank];
                case CharacterClassType.Assassin: return _configs.UI.ResourceIcons[ResourceType.BookAssassin];
                case CharacterClassType.Mystic: return _configs.UI.ResourceIcons[ResourceType.BookMystic];
                case CharacterClassType.Mage: return _configs.UI.ResourceIcons[ResourceType.BookMage];
                case CharacterClassType.Support: return _configs.UI.ResourceIcons[ResourceType.BookSupport];
                default: return _configs.UI.ResourceIcons[ResourceType.ShardRare];
            }
        }
        
        private Sprite GetRarityBG(Reward reward)
        {
            switch (reward)
            {
                case ResourceReward res: return _configs.UI.Rarities[RarityType.Common].RewardBG;
                case ShardReward shard: return _configs.UI.Rarities[_configs.Characters.Get(shard.ID).Rarity].RewardBG;
                case ClassShardReward classShard: return _configs.UI.Rarities[classShard.Rarity].RewardBG;
                case ClassBookReward book: return _configs.UI.Rarities[RarityType.Epic].RewardBG;
                default: return null;
            }
        }
    }
}