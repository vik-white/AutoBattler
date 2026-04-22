using vikwhite.Data;

namespace vikwhite
{
    public class CardViewModel: WindowViewModel<ICharacterData>
    {
        public string ID { get; }
        
        public CardViewModel(ICharacterData model) : base(model)
        {
            ID = model.ID;
        }
    }
}