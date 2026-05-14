using UnityEngine;

namespace vikwhite
{
    public class RedeemBookWindowView : WindowView<RedeemBookHierarchy, RedeemBookWindowViewModel>
    {
        public RedeemBookWindowView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(RedeemBookWindowViewModel viewModel)
        {
            BindClick(_view.CloseButton, viewModel.Close);
            BindClick(_view.AddButton, viewModel.OnAdd);
            BindClick(_view.AddMaxButton, viewModel.OnAddMax);
            BindClick(_view.RemoveButton, viewModel.OnRemove);
            BindClick(_view.RedeemButton, viewModel.OnRedeem);
            Bind(viewModel.Selected, _ => UpdateTexts(viewModel));
            Bind(viewModel.ClassBooksAmount, _ => UpdateTexts(viewModel));
        }

        private void UpdateTexts(RedeemBookWindowViewModel viewModel)
        {
            _view.ClassBooks.text = (viewModel.ClassBooksAmount.Value - viewModel.Selected.Value).ToString();
            _view.Books.text = $"+{viewModel.Selected.Value}";
        }
    }
}
