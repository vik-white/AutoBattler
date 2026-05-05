using UnityEngine;

namespace vikwhite
{
    public class VictoryWindowView : WindowView<VictoryWindowHierarchy, VictoryWindowViewModel>
    {
        private readonly IRewardItemViewFactory _rewardItemFactory;
        
        public VictoryWindowView(GameObject view, IRewardItemViewFactory rewardItemFactory) : base(view)
        {
            _rewardItemFactory = rewardItemFactory;
        }
        
        protected override void UpdateViewModel(VictoryWindowViewModel viewModel)
        {
            BindClick(_view.CloseButton, viewModel.OnEnd);
            _view.RewardContainer.ClearChildren();
            foreach (var reward in viewModel.Rewards)
                _rewardItemFactory.Get(reward, _view.RewardContainer);
        }
    }
}
