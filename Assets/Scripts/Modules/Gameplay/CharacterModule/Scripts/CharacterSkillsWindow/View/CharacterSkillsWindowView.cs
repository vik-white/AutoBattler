using UnityEngine;

namespace vikwhite
{
    public class CharacterSkillsWindowView : WindowView<CharacterSkillsWindowHierarchy, CharacterSkillsWindowViewModel>
    {
        public CharacterSkillsWindowView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(CharacterSkillsWindowViewModel viewModel)
        {
            BindClick(_view.CloseButton, viewModel.Close);
            BindClick(_view.StatsButton, viewModel.OnOpenStats);
            BindClick(_view.UpgradeButton, viewModel.OnUpgradeSkill);

            Bind(viewModel.SkillName, value => _view.SkillName.text = value);
            Bind(viewModel.SkillDescription, value =>_view.SkillDescription.text = value);
            Bind(viewModel.SkillUpgradePrice, value =>_view.SkillUpgradePrice.text = value);
            Bind(viewModel.BooksAmount, value => _view.BooksAmount.text = value.ToString());
            Bind(viewModel.CanUpgradeSkill, value => _view.UpgradeButton.interactable = value);

            _view.Name.text = viewModel.Name;
            _view.Image.sprite = viewModel.Image;
            _view.ClassIcon.sprite = viewModel.ClassIcon;
            _view.BookClassIcon.sprite = viewModel.BookClassIcon;
            
            for (var i = 0; i < viewModel.Skills.Count; i++)
                CreateView<SkillItemView, SkillItemHierarchy>(_view.SkillItems[i]).Initialize(viewModel.Skills[i]);
        }
    }
}
