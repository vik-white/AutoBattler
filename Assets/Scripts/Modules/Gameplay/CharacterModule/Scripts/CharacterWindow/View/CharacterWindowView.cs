using System.Linq;
using UnityEngine;

namespace vikwhite
{
    public class CharacterWindowView : WindowView<CharacterHierarchy, CharacterWindowViewModel>
    {
        private readonly IResourceViewFactory _resourceViewFactory;
        
        public CharacterWindowView(GameObject view, IResourceViewFactory resourceViewFactory) : base(view)
        {
            _resourceViewFactory = resourceViewFactory;
        }
        
        protected override void UpdateViewModel(CharacterWindowViewModel viewModel)
        {
            BindClick(_view.CloseButton, viewModel.Close);
            BindClick(_view.LevelUpButton, viewModel.OnUpgradeLevel);
            BindClick(_view.StarsUpButton, viewModel.OnStarsUpgrade);
            BindClick(_view.SkillUpButton, viewModel.OnSkillUpgrade);
            BindClick(_view.RedeemButton, viewModel.OnRedeem);
            Bind(viewModel.Level, level => _view.Level.text = level.ToString());
            Bind(viewModel.Stars, stars => _view.Stars.text = stars.ToString());
            Bind(viewModel.SkillLevel, skill => _view.Skill.text = skill.ToString());
            Bind(viewModel.Health, health => _view.Health.text = ((int)health).ToString());
            Bind(viewModel.Attack, attack => _view.Attack.text = ((int)attack).ToString());
            Bind(viewModel.Shards, shards => _view.Shards.text = $"{shards}/{viewModel.StarUpPrice}");
            Bind(viewModel.Shards, shards => _view.StarsBar.localScale = new Vector3(Mathf.Clamp01(shards / (float)viewModel.StarUpPrice), 1f, 1f));
            Bind(viewModel.ClassShards, classShards => _view.ClassShards.text = classShards.ToString());
            _view.Name.text = viewModel.Name;
            _view.Class.text = viewModel.Class;
            _view.Rarity.text = viewModel.Rarity;
            _view.Rarity.color = viewModel.RarityColor;
            _view.Image.sprite = viewModel.Image;
            _view.LevelUpPrice.text = $"{viewModel.LevelUpPrice} Gold";
            _view.SkillUpPrice.text = $"{viewModel.SkillUpPrice} Book";
            _view.AbilityIcon.sprite = viewModel.AbilityImage;
            _view.AbilityDescription.text = viewModel.AbilityDescription;
            _view.ResourcesContainer.ClearChildren();
            foreach (var resource in viewModel.Resources)
                _resourceViewFactory.Get(resource, _view.ResourcesContainer);
        }
    }
}