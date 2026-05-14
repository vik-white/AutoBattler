using UnityEngine;
using vikwhite.Data;

namespace vikwhite
{
    public class RewardItemViewModel : WindowViewModel<Reward>
    {
        private readonly IConfigs _configs;
        public int Value;
        public Sprite Icon;
        public Color RarityColor;
        public string ClassName;

        public RewardItemViewModel(Reward model, IConfigs configs) : base(model)
        {
            _configs = configs;
            Value = model.Value;
            Icon = GetIcon(model);
            RarityColor = GetRarityColor(model);
            if (model is ClassShardReward) ClassName = (model as ClassShardReward).Class.ToString();
            if (model is ClassBookReward) ClassName = (model as ClassBookReward).Class.ToString();
        }

        private Sprite GetIcon(Reward reward)
        {
            switch (reward)
            {
                case ResourceReward res: return _configs.ResourceIcons[res.ResourceType];
                case ShardReward shard: return _configs.Characters.Get(shard.ID).Image;
                case ClassBookReward book: return _configs.ResourceIcons[ResourceType.Book];
                default: return null;
            }
        }
        
        private Color GetRarityColor(Reward reward)
        {
            switch (reward)
            {
                case ShardReward shard:
                    return _configs.RarityColors[_configs.Characters.Get(shard.ID).Rarity];
                case ClassShardReward classShard:
                    return _configs.RarityColors[classShard.Rarity];
                default:
                    return default;
            }
        }
    }
}