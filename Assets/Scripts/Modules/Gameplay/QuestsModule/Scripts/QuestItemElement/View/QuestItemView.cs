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
            Bind(viewModel.Claimed, _ => RefreshButtons(viewModel));
            Bind(viewModel.Claimable, _ => RefreshButtons(viewModel));
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

        private void RefreshButtons(QuestItemViewModel viewModel)
        {
            bool claimed = viewModel.Claimed.Value;
            bool claimable = viewModel.Claimable.Value;
            _view.ClaimButton.gameObject.SetActive(!claimed && claimable);
            _view.GoButton.gameObject.SetActive(!claimed && !claimable);
        }
    }
}
