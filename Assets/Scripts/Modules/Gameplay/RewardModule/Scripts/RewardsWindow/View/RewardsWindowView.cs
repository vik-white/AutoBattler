using UnityEngine;

namespace vikwhite
{
    public class RewardsWindowView : WindowView<RewardsWindowHierarchy, RewardsWindowViewModel>
    {
        private readonly IRewardItemViewFactory _rewardItemFactory;

        public RewardsWindowView(GameObject view, IRewardItemViewFactory rewardItemFactory) : base(view)
        {
            _rewardItemFactory = rewardItemFactory;
        }

        protected override void UpdateViewModel(RewardsWindowViewModel viewModel)
        {
            BindClick(_view.CloseButton, viewModel.Close);
            _view.RewardContainer.ClearChildren();
            foreach (var reward in viewModel.Rewards)
                _rewardItemFactory.Get(reward, _view.RewardContainer);
        }
    }
}
