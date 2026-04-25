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
            BindClick(_view.UpgradeButton, viewModel.OnUpgrade);
            Bind(viewModel.Level, level => _view.Level.text = level.ToString());
            Bind(viewModel.Health, health => _view.Health.text = ((int)health).ToString());
            _view.Name.text = viewModel.Name;
            _view.Image.sprite = viewModel.Image;
            _view.Price.text = viewModel.Price.ToString();
            _view.AbilityIcon.sprite = viewModel.AbilityImage;
            _view.AbilityDescription.text = viewModel.AbilityDescription;
            _view.ResourcesContainer.ClearChildren();
            foreach (var resource in viewModel.Resources)
                _resourceViewFactory.Get(resource, _view.ResourcesContainer);
        }
    }
}