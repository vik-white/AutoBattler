using System.Collections.Generic;
using UniRx;

namespace vikwhite
{
    public interface IClassBookService
    {
        void Initialize();
        ClassBook Get(CharacterClassType @class);
        ReactiveProperty<int> GetAmount(CharacterClassType @class);
        bool CanSpend(CharacterClassType @class, int amount);
        void Add(CharacterClassType @class, int amount);
        void Spend(CharacterClassType @class, int amount);
        IReadOnlyCollection<ClassBook> GetAll();
    }

    public class ClassBookService : IClassBookService
    {
        private readonly IProfileService _profile;
        private readonly IEventDispatcher _dispatcher;
        private readonly Dictionary<CharacterClassType, ClassBook> _books = new();

        public ClassBookService(IProfileService profile, IEventDispatcher dispatcher)
        {
            _profile = profile;
            _dispatcher = dispatcher;
        }

        public void Initialize()
        {
            _books.Clear();
            foreach (var data in _profile.Data.ClassBooks)
                _books[data.Class] = new ClassBook(data.Class, data.Amount);
        }

        public ClassBook Get(CharacterClassType @class) => _books.TryGetValue(@class, out var book) ? book : null;

        public ReactiveProperty<int> GetAmount(CharacterClassType @class) => Get(@class)?.Amount;

        public bool CanSpend(CharacterClassType @class, int amount) => amount > 0 && Get(@class) is { } book && book.Amount.Value >= amount;

        public void Add(CharacterClassType @class, int amount)
        {
            if (amount <= 0) return;
            var book = Get(@class);
            if (book == null) return;
            book.Amount.Value += amount;
            _dispatcher.Dispatch(new ChangeClassBookEvent(@class, book.Amount.Value));
        }

        public void Spend(CharacterClassType @class, int amount)
        {
            if (amount <= 0) return;
            var book = Get(@class);
            if (book == null) return;
            if (book.Amount.Value - amount < 0) return;
            book.Amount.Value -= amount;
            _dispatcher.Dispatch(new ChangeClassBookEvent(@class, book.Amount.Value));
        }

        public IReadOnlyCollection<ClassBook> GetAll() => _books.Values;
    }
}
