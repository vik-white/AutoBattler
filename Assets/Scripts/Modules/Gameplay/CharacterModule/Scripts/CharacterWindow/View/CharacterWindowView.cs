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
            BindClick(_view.UpgradeButton, viewModel.OnUpgrade);
            Bind(viewModel.Level, level => _view.Level.text = level.ToString());
            Bind(viewModel.Health, health => _view.Health.text = ((int)health).ToString());
            _view.Name.text = viewModel.Name;
            _view.Image.sprite = viewModel.Image;
        }
    }
}