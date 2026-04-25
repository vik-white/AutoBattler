using System.Linq;
using UnityEngine;

namespace vikwhite
{
    public class CharacterWindowView : WindowView<CharacterHierarchy, CharacterWindowViewModel>
    {
        public CharacterWindowView(GameObject view) : base(view)
        {
        }
        
        protected override void UpdateViewModel(CharacterWindowViewModel viewModel)
        {
            BindClick(_view.CloseButton, viewModel.Close);
        }
    }
}