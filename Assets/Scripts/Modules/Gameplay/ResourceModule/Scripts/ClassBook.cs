using UniRx;

namespace vikwhite
{
    public class ClassBook
    {
        public CharacterClassType Class;
        public ReactiveProperty<int> Amount;

        public ClassBook(CharacterClassType @class, int amount)
        {
            Class = @class;
            Amount = new ReactiveProperty<int>(amount);
        }
    }
}
