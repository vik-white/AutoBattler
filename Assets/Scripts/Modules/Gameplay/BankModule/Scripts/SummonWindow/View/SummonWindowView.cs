using UnityEngine;

namespace vikwhite
{
    public class SummonWindowView : WindowView<SummonWindowHierarchy, SummonWindowViewModel>
    {
        private readonly IResourceViewFactory _resourceViewFactory;
        
        public SummonWindowView(GameObject view, IResourceViewFactory resourceViewFactory) : base(view)
        {
            _resourceViewFactory = resourceViewFactory;
        }

        protected override void UpdateViewModel(SummonWindowViewModel viewModel)
        {
            BindClick(_view.CloseButton, viewModel.Close);
            CreateView<SummonItemView, SummonItemHierarchy>(_view.Summon1).Initialize(viewModel.SummonItems[0]);
            CreateView<SummonItemView, SummonItemHierarchy>(_view.Summon2).Initialize(viewModel.SummonItems[1]);
            _view.ResourcesContainer.ClearChildren();
            foreach (var resource in viewModel.Resources)
                _resourceViewFactory.Get(resource, _view.ResourcesContainer);
        }
    }
}