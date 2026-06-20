using UnityEngine;

namespace vikwhite
{
    public class SummonItemView : WindowView<SummonItemHierarchy, SummonItemViewModel>
    {
        public SummonItemView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(SummonItemViewModel viewModel)
        {
            _view.Title.text = viewModel.Title;
            //_view.Count.text = viewModel.Count;
            _view.BuyX1Text.text = viewModel.BuyX1Text;
            _view.BuyX10Text.text = viewModel.BuyX10Text;
            _view.PriceX1.text = viewModel.PriceX1Text;
            _view.PriceX10.text = viewModel.PriceX10Text;
            
            _view.BuyX1Icon.sprite = viewModel.CurrencyIcon;
            _view.BuyX10Icon.sprite = viewModel.CurrencyIcon;

            BindClick(_view.BuyX1Button, viewModel.OnBuyX1);
            BindClick(_view.BuyX10Button, viewModel.OnBuyX10);
        }
    }
}
