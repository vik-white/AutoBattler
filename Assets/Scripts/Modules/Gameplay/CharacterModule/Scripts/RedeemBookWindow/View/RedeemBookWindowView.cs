using UnityEngine;

namespace vikwhite
{
    public class RedeemBookWindowView : WindowView<RedeemBookHierarchy, RedeemBookWindowViewModel>
    {
        public RedeemBookWindowView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(RedeemBookWindowViewModel viewModel)
        {
            BindClick(_view.CloseButton, viewModel.Close);
            BindClick(_view.CloseFadeButton, viewModel.Close);
            BindClick(_view.AddButton, viewModel.OnAdd);
            BindClick(_view.RemoveButton, viewModel.OnRemove);
            BindClick(_view.RedeemButton, viewModel.OnRedeem);
            Bind(viewModel.Selected, _ => UpdateState(viewModel));
            Bind(viewModel.ClassBooksAmount, _ => UpdateState(viewModel));
            _view.Slider.Initialize(progress => viewModel.OnSelect?.Invoke(Mathf.RoundToInt(progress * viewModel.BooksAmount.Value)));
        }

        private void UpdateState(RedeemBookWindowViewModel viewModel)
        {
            _view.BookAmount.text = $"{viewModel.BooksAmount.Value - viewModel.Selected.Value}";
            _view.ClassBookAmount.text = $"{viewModel.ClassBooksAmount.Value}";
            _view.CurrentBookAmount.text = $"{viewModel.Selected.Value}";
            _view.ClassBookIcon.sprite = viewModel.ClassBookIcon;
            _view.RedeemButton.interactable = viewModel.Selected.Value > 0;
            _view.Slider.SetValue(viewModel.BooksAmount.Value > 0 ? viewModel.Selected.Value / (float)viewModel.BooksAmount.Value : 0f);
        }
    }
}
