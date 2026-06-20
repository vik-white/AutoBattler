using UnityEngine;

namespace vikwhite
{
    public class SummonWindowView : WindowView<SummonWindowHierarchy, SummonWindowViewModel>
    {
        public SummonWindowView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(SummonWindowViewModel viewModel)
        {
            BindClick(_view.CloseButton, viewModel.Close);
        }
    }
}
