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
                case ResourceReward res: return _configs.ResourceIcons[res.ResourceType];
                case ShardReward shard: return GetShardIcon(shard);
                case ClassBookReward book: return GetBookIcon(book);
                default: return null;
            }
        }
        
        private Sprite GetShardIcon(ShardReward shard)
        {
            switch (_configs.Characters.Get(shard.ID).Rarity)
            {
                case RarityType.Rare: return _configs.ResourceIcons[ResourceType.ShardRare];
                case RarityType.Epic: return _configs.ResourceIcons[ResourceType.ShardEpic];
                case RarityType.Legendary: return _configs.ResourceIcons[ResourceType.ShardLegendary];
                default: return _configs.ResourceIcons[ResourceType.ShardRare];
            }
        }
        
        private Sprite GetBookIcon(ClassBookReward book)
        {
            switch (book.Class)
            {
                case CharacterClassType.Tank: return _configs.ResourceIcons[ResourceType.BookTank];
                case CharacterClassType.Assassin: return _configs.ResourceIcons[ResourceType.BookAssassin];
                case CharacterClassType.Mystic: return _configs.ResourceIcons[ResourceType.BookMystic];
                case CharacterClassType.Mage: return _configs.ResourceIcons[ResourceType.BookMage];
                case CharacterClassType.Support: return _configs.ResourceIcons[ResourceType.BookSupport];
                default: return _configs.ResourceIcons[ResourceType.ShardRare];
            }
        }
        
        private Sprite GetRarityBG(Reward reward)
        {
            switch (reward)
            {
                case ResourceReward res: return _configs.RarityBG[RarityType.Common];
                case ShardReward shard: return _configs.RarityBG[_configs.Characters.Get(shard.ID).Rarity];
                case ClassShardReward classShard: return _configs.RarityBG[classShard.Rarity];
                case ClassBookReward book: return _configs.RarityBG[RarityType.Epic];
                default: return null;
            }
        }
    }
}