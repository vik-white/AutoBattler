using UnityEngine;
using vikwhite.Data;

namespace vikwhite
{
    public class RewardItemViewModel : WindowViewModel<Reward>
    {
        private readonly IConfigs _configs;
        public int Value;
        public Sprite Icon;

        public RewardItemViewModel(Reward model, IConfigs configs) : base(model)
        {
            _configs = configs;
            Value = model.Value;
            Icon = GetIcon(model);
        }

        private Sprite GetIcon(Reward reward)
        {
            switch (reward)
            {
                case ResourceReward res:
                    return _configs.ResourceIcons.TryGetValue(res.ResourceType, out var sprite) ? sprite : null;
                case ShardReward shard:
                    return _configs.Characters.Get(shard.ID).Image;
                default:
                    return null;
            }
        }
    }
}