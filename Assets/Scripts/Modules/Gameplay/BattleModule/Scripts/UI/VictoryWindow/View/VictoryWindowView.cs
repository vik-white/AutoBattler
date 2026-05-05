using UnityEngine;

namespace vikwhite
{
    public class VictoryWindowView : WindowView<VictoryWindowHierarchy, VictoryWindowViewModel>
    {
        public VictoryWindowView(GameObject view) : base(view)
        {
        }
        
        protected override void UpdateViewModel(VictoryWindowViewModel viewModel)
        {
            BindClick(_view.CloseButton, viewModel.OnEnd);
            //_view.Reward.text = viewModel.Reward.ToString();
        }
    }
}