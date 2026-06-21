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
            _view.BG.sprite = viewModel.RarityBG;
            _view.Shard.sprite = viewModel.Shard;
            _view.Shard.gameObject.SetActive(viewModel.Shard != null);
        }
    }
}
