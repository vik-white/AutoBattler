using UnityEngine;

namespace vikwhite
{
    public class DefeatWindowView : WindowView<DefeatWindowHierarchy, DefeatWindowViewModel>
    {
        public DefeatWindowView(GameObject view) : base(view)
        {
        }
        
        protected override void UpdateViewModel(DefeatWindowViewModel viewModel)
        {
            BindClick(_view.CloseButton, viewModel.OnEnd);
        }
    }
}