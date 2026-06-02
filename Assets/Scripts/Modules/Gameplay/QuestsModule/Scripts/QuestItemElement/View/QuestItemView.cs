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

            Bind(viewModel.Progress, progress => RefreshProgress(viewModel, progress));
            Bind(viewModel.Claimable, claimable => RefreshClaim(viewModel, claimable));
            Bind(viewModel.Claimed, claimed => RefreshClaimedLabel(claimed));

            BindClick(_view.ClaimButton, viewModel.OnClaim);

            _view.RewardsContainer.ClearChildren();
            foreach (var reward in viewModel.Rewards)
                _rewardItemFactory.Get(reward, _view.RewardsContainer);
        }

        private void RefreshProgress(QuestItemViewModel viewModel, int progress)
        {
            _view.Progress.text = $"{progress}/{viewModel.Amount}";
            if (_view.ProgressBar != null)
            {
                _view.ProgressBar.maxValue = Mathf.Max(viewModel.Amount, 1);
                _view.ProgressBar.value = progress;
            }
        }

        private void RefreshClaim(QuestItemViewModel viewModel, bool claimable)
        {
            _view.ClaimButton.gameObject.SetActive(viewModel.Claimed.Value == false);
            _view.ClaimButton.interactable = claimable;
        }

        private void RefreshClaimedLabel(bool claimed)
        {
            if (_view.ClaimedLabel != null) _view.ClaimedLabel.SetActive(claimed);
            if (claimed) _view.ClaimButton.gameObject.SetActive(false);
        }
    }
}
