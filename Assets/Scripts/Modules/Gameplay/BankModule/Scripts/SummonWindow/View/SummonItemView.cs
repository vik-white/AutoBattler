using UnityEngine;

namespace vikwhite
{
    public class SummonItemView : WindowView<SummonItemHierarchy, SummonItemViewModel>
    {
        private readonly IResourceViewFactory _resourceViewFactory;

        public SummonItemView(GameObject view, IResourceViewFactory resourceViewFactory) : base(view)
        {
            _resourceViewFactory = resourceViewFactory;
        }

        protected override void UpdateViewModel(SummonItemViewModel viewModel)
        {
            _view.Title.text = viewModel.Title;
            _view.PriceX1.text = viewModel.PriceX1.ToString();
            _view.PriceX10.text = viewModel.PriceX10.ToString();
            BindClick(_view.BuyX1Button, viewModel.OnBuyX1);
            BindClick(_view.BuyX10Button, viewModel.OnBuyX10);

            _view.ResourceContainer.ClearChildren();
            _resourceViewFactory.Get(viewModel.Resource, _view.ResourceContainer);
        }
    }
}
