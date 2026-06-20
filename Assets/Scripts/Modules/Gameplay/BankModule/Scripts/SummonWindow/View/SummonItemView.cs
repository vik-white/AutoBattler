using UnityEngine;

namespace vikwhite
{
    public class SummonItemView : WindowView<SummonItemHierarchy, SummonItemViewModel>
    {
        public SummonItemView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(SummonItemViewModel viewModel)
        {
            _view.Title.text = viewModel.Title;
            _view.PriceX1.text = $"x{viewModel.PriceX1}";
            _view.PriceX10.text = $"x{viewModel.PriceX10}";
            BindClick(_view.BuyX1Button, viewModel.OnBuyX1);
            BindClick(_view.BuyX10Button, viewModel.OnBuyX10);
        }
    }
}
