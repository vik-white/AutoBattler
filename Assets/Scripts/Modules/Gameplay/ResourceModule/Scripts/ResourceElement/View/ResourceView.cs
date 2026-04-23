using UnityEngine;
using vikwhite.Data;

namespace vikwhite
{
    public class ResourceView : WindowView<ResourceHierarchy, ResourceViewModel>
    {
        private readonly IConfigs _configs;
        
        public ResourceView(GameObject view, IConfigs configs) : base(view)
        {
            _configs = configs;
        }

        protected override void UpdateViewModel(ResourceViewModel viewModel)
        {
            Bind(viewModel.Amount, SetAmount);
            _view.Icon.sprite = _configs.ResourceIcons[viewModel.Type];
        }
        
        private void SetAmount(int amount) => _view.Amount.text = amount.ToString();
    }
}