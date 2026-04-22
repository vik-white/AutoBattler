using vikwhite.Data;

namespace vikwhite
{
    public class CardViewModel: WindowViewModel<ICharacterData>
    {
        public CardViewModel(ICharacterData model) : base(model)
        {
        }
    }
}