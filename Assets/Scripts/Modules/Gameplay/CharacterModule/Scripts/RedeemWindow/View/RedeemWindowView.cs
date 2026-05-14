using UnityEngine;

namespace vikwhite
{
    public class RedeemWindowView : WindowView<RedeemHierarchy, RedeemWindowViewModel>
    {
        public RedeemWindowView(GameObject view) : base(view) { }
        
        protected override void UpdateViewModel(RedeemWindowViewModel viewModel)
        {
            BindClick(_view.CloseButton, viewModel.Close);
            BindClick(_view.AddButton, viewModel.OnAdd);
            BindClick(_view.AddMaxButton, viewModel.OnAddMax);
            BindClick(_view.RemoveButton, viewModel.OnRemove);
            BindClick(_view.RedeemButton, viewModel.OnRedeem);
            _view.Name.text = viewModel.Name;
            Bind(viewModel.Selected, _ => UpdateTexts(viewModel));
            Bind(viewModel.ClassShardsAmount, _ => UpdateTexts(viewModel));
        }
        
        private void UpdateTexts(RedeemWindowViewModel viewModel)
        {
            _view.ClassShards.text = (viewModel.ClassShardsAmount.Value - viewModel.Selected.Value).ToString();
            _view.Shards.text = $"+{viewModel.Selected.Value}";
            _view.Image.sprite = viewModel.Image;
        }
    }
}
