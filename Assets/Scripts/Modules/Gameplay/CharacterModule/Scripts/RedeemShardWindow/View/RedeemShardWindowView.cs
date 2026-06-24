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
            Bind(viewModel.Selected, _ => UpdateState(viewModel));
            Bind(viewModel.ShardsAmount, _ => UpdateState(viewModel));
            _view.Slider.Initialize(progress => viewModel.OnSelect?.Invoke(Mathf.RoundToInt(progress * viewModel.ShardsAmount.Value)));
        }

        private void UpdateState(RedeemShardWindowViewModel viewModel)
        {
            _view.ShardsAmount.text = $"{viewModel.ShardsAmount.Value - viewModel.Selected.Value}";
            _view.HeroShardsAmount.text = $"{viewModel.HeroShardsAmount.Value}";
            _view.CurrentShardsAmount.text = $"{viewModel.Selected.Value}";
            _view.HeroShardIcon.sprite = viewModel.HeroShardIcon;
            _view.ShardIcon1.sprite = viewModel.ShardIcon;
            _view.ShardIcon2.sprite = viewModel.ShardIcon;
            _view.RedeemButton.interactable = viewModel.Selected.Value > 0;
            _view.Slider.SetValue(viewModel.ShardsAmount.Value > 0 ? viewModel.Selected.Value / (float)viewModel.ShardsAmount.Value : 0f);
        }
    }
}
