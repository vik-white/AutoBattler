using UnityEngine;

namespace vikwhite
{
    public class CharacterUpgradeWindowView : WindowView<CharacterUpgradeHierarchy, CharacterUpgradeWindowViewModel>
    {
        public CharacterUpgradeWindowView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(CharacterUpgradeWindowViewModel viewModel)
        {
            BindClick(_view.CloseButton, viewModel.Close);
            BindClick(_view.LevelUpButton, viewModel.OnUpgradeLevel);
            BindClick(_view.PreviousLevelButton, viewModel.OnSelectPreviousLevel);
            BindClick(_view.NextLevelButton, viewModel.OnSelectNextLevel);
            Bind(viewModel.SelectedLevel, level => _view.Level.text = level.ToString());
            Bind(viewModel.Might, might => _view.Might.text = might);
            Bind(viewModel.ExpResources.Amount, SetLevelUpPrice);
            Bind(viewModel.CanSelectPreviousLevel, value => _view.PreviousLevelButton.interactable = value);
            Bind(viewModel.CanSelectNextLevel, value => _view.NextLevelButton.interactable = value);
            CreateView<StarsView, StarsHierarchy>(_view.Stars).Initialize(viewModel.Stars);
            CreateView<StatsInfoView, StatsInfoHierarchy>(_view.StatsInfo).Initialize(viewModel.StatsInfo);
            _view.Name.text = viewModel.Name;
            _view.Image.sprite = viewModel.Image;
            _view.ClassIcon.sprite = viewModel.ClassIcon;
            SetLevelUpPrice(viewModel.ExpResources.Amount.Value);
        }

        private void SetLevelUpPrice(int amount)
        {
            var amountColor = amount >= ViewModel.LevelUpPrice ? ColorHandler.Green : ColorHandler.Red;
            _view.LevelUpPrice.text = $"{amount.ToString().Color(amountColor)}/{ViewModel.LevelUpPrice}";
        }
    }
}
