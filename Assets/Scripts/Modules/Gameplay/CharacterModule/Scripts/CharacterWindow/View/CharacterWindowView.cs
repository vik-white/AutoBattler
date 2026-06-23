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
            BindClick(_view.InfoButton, viewModel.OnOpenUpgradeInfo);
            BindClick(_view.AscendButton, viewModel.OnOpenAscendInfo);
            BindClick(_view.PreviousCharacterButton, viewModel.OnSelectPreviousCharacter);
            BindClick(_view.NextCharacterButton, viewModel.OnSelectNextCharacter);
            Bind(viewModel.Level, level => _view.Level.text = level.ToString());
            Bind(viewModel.ExpResources.Amount, _ => SetLevelUpPrice());
            CreateView<StarsView, StarsHierarchy>(_view.Stars).Initialize(viewModel.Stars);
            _view.Name.text = viewModel.Name;
            _view.Image.sprite = viewModel.Image;
            _view.ClassIcon.sprite = viewModel.ClassIcon;
            SetLevelUpPrice();
            _view.R.gameObject.SetActive(viewModel.Rarity == RarityType.Rare);
            _view.SR.gameObject.SetActive(viewModel.Rarity == RarityType.Epic);
            _view.SSR.gameObject.SetActive(viewModel.Rarity == RarityType.Legendary);
        }

        private void SetLevelUpPrice() => _view.LevelUpPrice.text = $"{ViewModel.ExpResources.Amount}/{ViewModel.LevelUpPrice}";
    }
}
