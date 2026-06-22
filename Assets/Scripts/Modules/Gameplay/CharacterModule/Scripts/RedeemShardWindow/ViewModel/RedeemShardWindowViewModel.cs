using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace vikwhite
{
    public class RedeemShardWindowViewModel : WindowViewModel<Character>
    {
        private readonly ReactiveProperty<int> _selected = new(0);

        public string Name;
        public Sprite Image;
        public IReadOnlyReactiveProperty<int> Selected;
        public IReadOnlyReactiveProperty<int> ClassShardsAmount;
        public UnityAction OnAdd;
        public UnityAction OnAddMax;
        public UnityAction OnRemove;
        public UnityAction OnRedeem;

        public RedeemShardWindowViewModel(Character character) : base(character)
        {
            Name = character.Config.Name;
            Image = character.Config.Image;
            AddDisposable(_selected);
            Selected = _selected;
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
            int amount = _selected.Value;
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
