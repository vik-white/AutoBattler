using UniRx;
using UnityEngine.Events;

namespace vikwhite
{
    public class RedeemBookWindowViewModel : WindowViewModel<Character>
    {
        private readonly IClassBookService _classBooks;
        private readonly IResourceService _resources;
        private readonly CharacterClassType _class;
        private readonly ReactiveProperty<int> _selected = new(0);

        public IReadOnlyReactiveProperty<int> Selected;
        public IReadOnlyReactiveProperty<int> BooksAmount;
        public IReadOnlyReactiveProperty<int> ClassBooksAmount;
        public UnityAction OnAdd;
        public UnityAction OnAddMax;
        public UnityAction OnRemove;
        public UnityAction OnRedeem;

        public RedeemBookWindowViewModel(Character character, IClassBookService classBooks, IResourceService resources) : base(character)
        {
            _classBooks = classBooks;
            _resources = resources;
            _class = character.Config.Class;
            AddDisposable(_selected);
            Selected = _selected;
            BooksAmount = resources.GetAmount(ResourceType.Book);
            ClassBooksAmount = _classBooks.GetAmount(_class);
            OnAdd = Add;
            OnAddMax = AddMax;
            OnRemove = Remove;
            OnRedeem = Redeem;
        }

        private int GetAvailable() => BooksAmount?.Value ?? 0;

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
            if (_resources.GetAmount(ResourceType.Book).Value < _selected.Value) return;
            int amount = _selected.Value;
            _resources.Spend(ResourceType.Book, amount);
            _classBooks.Add(_class, amount);
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
