using System.Collections.Generic;
using UnityEngine.Events;

namespace vikwhite
{
    public class MetaWindowViewModel : WindowViewModel
    {
        public List<MetaItemViewModel> Characters { get; } = new();
        public UnityAction OnSummon;
        
        public MetaWindowViewModel(ICharactersService characters, ISummonWindow summonWindow)
        {
            OnSummon = summonWindow.ShowWindow;

            foreach (var character in characters.GetCharacters())
                Characters.Add(CreateViewModel<MetaItemViewModel, Character>(character));
        }

        public override void Dispose()
        {
            base.Dispose();
            OnSummon = null;
        }
    }
}
