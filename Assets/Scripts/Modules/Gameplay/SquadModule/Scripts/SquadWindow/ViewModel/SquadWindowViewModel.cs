using UnityEngine.Events;

namespace vikwhite
{
    public class SquadWindowViewModel: WindowViewModel<bool>
    {
        public UnityAction<int, string> OnSetCharacter;
        public UnityAction<int> OnRemoveCharacter;
        
        public SquadWindowViewModel(bool model) : base(model)
        {
        }
        
        public override void Dispose()
        {
            base.Dispose();
            OnSetCharacter = null;
            OnRemoveCharacter = null;
        }
    }
}