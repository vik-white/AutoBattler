using UnityEngine;

namespace vikwhite
{
    public class SummonWindowView : WindowView<SummonWindowHierarchy, SummonWindowViewModel>
    {
        private readonly ISummonItemViewFactory _summonItemViewFactory;

        public SummonWindowView(GameObject view, ISummonItemViewFactory summonItemViewFactory) : base(view)
        {
            _summonItemViewFactory = summonItemViewFactory;
        }

        protected override void UpdateViewModel(SummonWindowViewModel viewModel)
        {
            BindClick(_view.CloseButton, viewModel.Close);
            _view.SummonItemContainer.ClearChildren();
            foreach (var item in viewModel.SummonItems)
                _summonItemViewFactory.Get(item, _view.SummonItemContainer);
        }
    }
}
