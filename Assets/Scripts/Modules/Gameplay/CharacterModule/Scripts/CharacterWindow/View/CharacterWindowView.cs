using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
            BindClick(_view.SkillsButton, viewModel.OnOpenSkills);
            BindClick(_view.BreakthroughButton, viewModel.OnOpenBreakthrough);
            Bind(viewModel.Level, level => _view.Level.text = level.ToString());
            Bind(viewModel.Might, might => _view.Might.text = might.ToString());
            Bind(viewModel.IsBreakthroughRequired, SetBreakthroughRequired);
            Bind(viewModel.ExpResources.Amount, SetLevelUpPrice);
            CreateView<StarsView, StarsHierarchy>(_view.Stars).Initialize(viewModel.Stars);
            _view.Name.text = viewModel.Name;
            _view.Image.sprite = viewModel.Image;
            _view.ClassIcon.sprite = viewModel.ClassIcon;
            SetLevelUpPrice(viewModel.ExpResources.Amount.Value);
            _view.R.gameObject.SetActive(viewModel.Rarity == RarityType.Rare);
            _view.SR.gameObject.SetActive(viewModel.Rarity == RarityType.Epic);
            _view.SSR.gameObject.SetActive(viewModel.Rarity == RarityType.Legendary);
        }

        private void SetLevelUpPrice(int amount)
        {
            var amountColor = amount >= ViewModel.LevelUpPrice ? ColorHandler.Green : ColorHandler.Red;
            _view.LevelUpPrice.text = $"{amount.ToString().Color(amountColor)}/{ViewModel.LevelUpPrice}";
        }

        private void SetBreakthroughRequired(bool isRequired)
        {
            _view.LevelUpButton.gameObject.SetActive(!isRequired);
            _view.BreakthroughButton.gameObject.SetActive(isRequired);
        }
    }
}
