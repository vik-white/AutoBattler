using UniRx;
using UnityEngine;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class RedeemBookWindowViewModel : WindowViewModel<Character>
    {
        private readonly IResourceService _resources;
        private readonly ResourceType _bookClassType;
        private readonly ReactiveProperty<int> _selected = new(0);

        public IReadOnlyReactiveProperty<int> Selected;
        public IReadOnlyReactiveProperty<int> BooksAmount;
        public IReadOnlyReactiveProperty<int> ClassBooksAmount;
        public UnityAction OnAdd;
        public UnityAction OnRemove;
        public UnityAction OnRedeem;
        public Sprite ClassBookIcon;
        public UnityAction<int> OnSelect;

        public RedeemBookWindowViewModel(Character character, IResourceService resources, IConfigs configs) : base(character)
        {
            _resources = resources;
            _bookClassType = ResourceHandler.GetBookResourceType(character.Config.Class);
            BooksAmount = resources.GetAmount(ResourceType.Book);
            ClassBooksAmount = resources.Get(_bookClassType).Amount;
            ClassBookIcon = configs.UI.ResourceIcons[_bookClassType];
            AddDisposable(_selected);
            Selected = _selected;
            AddDisposable(BooksAmount.Subscribe(_ => ClampSelected()));
            OnAdd = Add;
            OnRemove = Remove;
            OnRedeem = Redeem;
            OnSelect = SetSelected;
        }

        private int GetAvailable() => BooksAmount?.Value ?? 0;

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
            if (_resources.GetAmount(ResourceType.Book).Value < _selected.Value) return;
            int amount = _selected.Value;
            _resources.Spend(ResourceType.Book, amount);
            _resources.Add(_bookClassType, amount);
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
