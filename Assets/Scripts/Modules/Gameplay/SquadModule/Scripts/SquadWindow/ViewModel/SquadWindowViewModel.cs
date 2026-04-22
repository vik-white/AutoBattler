using System.Collections.Generic;
using UnityEngine.Events;
using vikwhite.Data;

namespace vikwhite
{
    public class SquadWindowViewModel: WindowViewModel<bool>
    {
        public List<CardViewModel> Cards = new();
        public UnityAction<int, string> OnSetCharacter;
        public UnityAction<int> OnRemoveCharacter;
        
        public SquadWindowViewModel(bool model, IConfigs configs) : base(model)
        {
            foreach (var character in configs.Characters.GetAll())
            {
                Cards.Add(CreateViewModel<CardViewModel, ICharacterData>(character));
            }
        }
        
        public override void Dispose()
        {
            base.Dispose();
            OnSetCharacter = null;
            OnRemoveCharacter = null;
        }
    }
}