using UnityEngine;

namespace vikwhite
{
    public class RewardItemView : WindowView<RewardItemHierarchy, RewardItemViewModel>
    {
        public RewardItemView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(RewardItemViewModel viewModel)
        {
            _view.Icon.sprite = viewModel.Icon;
            _view.Value.text = viewModel.Value.ToString();
        }
    }
}
