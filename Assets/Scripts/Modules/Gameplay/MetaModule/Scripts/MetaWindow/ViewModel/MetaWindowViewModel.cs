using System.Collections.Generic;

namespace vikwhite
{
    public class MetaWindowViewModel : WindowViewModel
    {
        public List<MetaItemViewModel> Characters { get; } = new();
        
        public MetaWindowViewModel(ICharactersService characters)
        {
            foreach (var character in characters.GetCharacters())
                Characters.Add(CreateViewModel<MetaItemViewModel, Character>(character));
        }
    }
}
