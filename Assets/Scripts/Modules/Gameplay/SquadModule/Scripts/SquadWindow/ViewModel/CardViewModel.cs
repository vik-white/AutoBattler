using UnityEngine.Events;

namespace vikwhite
{
    public class CardViewModel: WindowViewModel<Character>
    {
        public UnityAction OnSelect;
        public string ID { get; }
        
        public CardViewModel(Character model, ICharacterWindow characterWindow) : base(model)
        {
            ID = model.ID;
            OnSelect = () => characterWindow.ShowWindow(model);
        }
        
        public override void Dispose()
        {
            base.Dispose();
            OnSelect = null;
        }
    }
}