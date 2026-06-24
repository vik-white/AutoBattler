using UnityEngine;

namespace vikwhite
{
    public class RedeemShardWindowView : WindowView<RedeemShardHierarchy, RedeemShardWindowViewModel>
    {
        public RedeemShardWindowView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(RedeemShardWindowViewModel viewModel)
        {
            BindClick(_view.CloseButton, viewModel.Close);
            BindClick(_view.CloseFadeButton, viewModel.Close);
            BindClick(_view.AddButton, viewModel.OnAdd);
            BindClick(_view.RemoveButton, viewModel.OnRemove);
            BindClick(_view.RedeemButton, viewModel.OnRedeem);
            Bind(viewModel.Selected, _ => UpdateTexts(viewModel));
            Bind(viewModel.ShardsAmount, _ => UpdateTexts(viewModel));
        }

        private void UpdateTexts(RedeemShardWindowViewModel viewModel)
        {
            _view.ShardsAmount.text = $"{viewModel.ShardsAmount.Value - viewModel.Selected.Value}";
            _view.HeroShardsAmount.text = $"{viewModel.HeroShardsAmount.Value}";
            _view.CurrentShardsAmount.text = $"{viewModel.Selected.Value}";
            _view.HeroShardIcon.sprite = viewModel.HeroShardIcon;
            _view.ShardIcon1.sprite = viewModel.ShardIcon;
            _view.ShardIcon2.sprite = viewModel.ShardIcon;
        }
    }
}
