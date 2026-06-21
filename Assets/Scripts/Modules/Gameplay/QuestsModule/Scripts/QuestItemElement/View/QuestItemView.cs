using UnityEngine;

namespace vikwhite
{
    public class QuestItemView : WindowView<QuestItemHierarchy, QuestItemViewModel>
    {
        private readonly IRewardItemViewFactory _rewardItemFactory;

        public QuestItemView(GameObject view, IRewardItemViewFactory rewardItemFactory) : base(view)
        {
            _rewardItemFactory = rewardItemFactory;
        }

        protected override void UpdateViewModel(QuestItemViewModel viewModel)
        {
            _view.Description.text = viewModel.Description;
            Bind(viewModel.Claimed, RefreshClaimed);
            Bind(viewModel.Claimable, RefreshClaim);
            Bind(viewModel.Progress, progress => RefreshProgress(viewModel, progress));
            BindClick(_view.ClaimButton, viewModel.OnClaim);
            _view.RewardsContainer.ClearChildren();
            foreach (var reward in viewModel.Rewards)
                _rewardItemFactory.Get(reward, _view.RewardsContainer);
        }
        
        private void RefreshProgress(QuestItemViewModel viewModel, int progress)
        {
            _view.Progress.text = $"({progress}/{viewModel.Amount})";
        }

        private void RefreshClaim(bool claimable)
        {
            _view.ClaimButton.gameObject.SetActive(claimable);
            _view.GoButton.gameObject.SetActive(!claimable);
        }
        
        private void RefreshClaimed(bool claimed)
        {
            if(!claimed) return;
            _view.ClaimButton.gameObject.SetActive(false);
            _view.GoButton.gameObject.SetActive(false);
        }
    }
}
