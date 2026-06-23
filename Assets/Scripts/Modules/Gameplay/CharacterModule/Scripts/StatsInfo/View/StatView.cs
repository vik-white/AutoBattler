using UnityEngine;

namespace vikwhite
{
    public class StatView : View<StatHierarchy, StatViewModel>
    {
        public StatView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(StatViewModel viewModel)
        {
            _view.Title.text = viewModel.Title;
            Bind(viewModel.Amount, value => _view.Amount.text = value);
            Bind(viewModel.AmountUpgrade, value => _view.AmountUpgrade.text = value);
        }
    }
}
