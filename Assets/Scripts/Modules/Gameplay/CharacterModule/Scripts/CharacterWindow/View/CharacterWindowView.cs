using UnityEngine;

namespace vikwhite
{
    public class CharacterWindowView : WindowView<CharacterHierarchy, CharacterWindowViewModel>
    {
        public CharacterWindowView(GameObject view) : base(view) { }
        
        protected override void UpdateViewModel(CharacterWindowViewModel viewModel)
        {
            BindClick(_view.CloseButton, viewModel.Close);
            BindClick(_view.LevelUpButton, viewModel.OnUpgradeLevel);
            Bind(viewModel.Level, level => _view.Level.text = level.ToString());
            Bind(viewModel.ExpResources.Amount, _ => SetLevelUpPrice());
            CreateView<StarsView, StarsHierarchy>(_view.Stars).Initialize(viewModel.Stars);
            _view.Name.text = viewModel.Name;
            _view.Image.sprite = viewModel.Image;
            SetLevelUpPrice();
        }

        private void SetLevelUpPrice() => _view.LevelUpPrice.text = $"{ViewModel.ExpResources.Amount}/{ViewModel.LevelUpPrice}";
    }
}