using UniRx;
using UnityEngine;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class RedeemShardWindowViewModel : WindowViewModel<Character>
    {
        private readonly ReactiveProperty<int> _selected = new(0);
        private readonly IResourceService _resource;
        private ResourceType _shardType;
        
        public string Name;
        public Sprite HeroShardIcon;
        public Sprite ShardIcon;
        public IReadOnlyReactiveProperty<int> Selected;
        public IReadOnlyReactiveProperty<int> ShardsAmount;
        public IReadOnlyReactiveProperty<int> HeroShardsAmount;
        public UnityAction OnAdd;
        public UnityAction OnRemove;
        public UnityAction OnRedeem;
        public UnityAction<int> OnSelect;

        public RedeemShardWindowViewModel(Character character, IConfigs configs, IResourceService resource) : base(character)
        {
            _resource = resource;
            Name = character.Config.Name;
            _shardType = ResourceHandler.GetShardResourceType(character.Config.Rarity);
            ShardsAmount = resource.Get(_shardType).Amount;
            HeroShardsAmount = character.Shards;
            HeroShardIcon = character.Config.ShardImage;
            ShardIcon = configs.UI.Rarities[character.Config.Rarity].Shard;
            AddDisposable(_selected);
            Selected = _selected;
            AddDisposable(ShardsAmount.Subscribe(_ => ClampSelected()));
            OnAdd = Add;
            OnRemove = Remove;
            OnRedeem = Redeem;
            OnSelect = SetSelected;
        }

        private int GetAvailable() => ShardsAmount?.Value ?? 0;

        private void Add()
        {
            SetSelected(_selected.Value + 1);
        }

        private void Remove()
        {
            SetSelected(_selected.Value - 1);
        }

        private void SetSelected(int amount)
        {
            _selected.Value = Mathf.Clamp(amount, 0, GetAvailable());
        }

        private void ClampSelected()
        {
            SetSelected(_selected.Value);
        }

        private void Redeem()
        {
            if (_selected.Value <= 0) return;
            int amount = _selected.Value;
            _resource.Spend(_shardType, amount);
            Model.AddShards(amount);
            _selected.Value = 0;
        }

        public override void Dispose()
        {
            base.Dispose();
            OnAdd = null;
            OnRemove = null;
            OnRedeem = null;
            OnSelect = null;
        }
    }
}
