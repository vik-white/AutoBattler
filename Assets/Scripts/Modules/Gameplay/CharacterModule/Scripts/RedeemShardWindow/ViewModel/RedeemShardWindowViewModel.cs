using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace vikwhite
{
    public class RedeemShardWindowViewModel : WindowViewModel<Character>
    {
        private readonly IClassShardService _classShards;
        private readonly ClassShardKey _key;
        private readonly ReactiveProperty<int> _selected = new(0);

        public string Name;
        public Sprite Image;
        public IReadOnlyReactiveProperty<int> Selected;
        public IReadOnlyReactiveProperty<int> ClassShardsAmount;
        public UnityAction OnAdd;
        public UnityAction OnAddMax;
        public UnityAction OnRemove;
        public UnityAction OnRedeem;

        public RedeemShardWindowViewModel(Character character, IClassShardService classShards) : base(character)
        {
            _classShards = classShards;
            _key = new ClassShardKey(character.Config.Class, character.Config.Rarity);
            Name = character.Config.Name;
            Image = character.Config.Image;
            AddDisposable(_selected);
            Selected = _selected;
            ClassShardsAmount = _classShards.GetAmount(_key);
            OnAdd = Add;
            OnAddMax = AddMax;
            OnRemove = Remove;
            OnRedeem = Redeem;
        }

        private int GetAvailable() => ClassShardsAmount?.Value ?? 0;

        private void Add()
        {
            if (_selected.Value < GetAvailable()) _selected.Value++;
        }

        private void AddMax()
        {
            _selected.Value = GetAvailable();
        }

        private void Remove()
        {
            if (_selected.Value > 0) _selected.Value--;
        }

        private void Redeem()
        {
            if (_selected.Value <= 0) return;
            if (!_classShards.CanSpend(_key, _selected.Value)) return;
            int amount = _selected.Value;
            _classShards.Spend(_key, amount);
            Model.AddShards(amount);
            _selected.Value = 0;
        }

        public override void Dispose()
        {
            base.Dispose();
            OnAdd = null;
            OnAddMax = null;
            OnRemove = null;
            OnRedeem = null;
        }
    }
}
