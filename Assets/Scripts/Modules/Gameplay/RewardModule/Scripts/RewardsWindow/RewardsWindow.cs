using System.Collections.Generic;

namespace vikwhite
{
    public interface IRewardsWindow : IWindowPresenter
    {
        void ShowWindow(List<Reward> rewards);
    }

    public class RewardsWindow : WindowPresenter<RewardsWindowView, RewardsWindowViewModel>, IRewardsWindow
    {
        public override string AssetName => "UI/Prefabs/RewardsWindow/RewardsWindow";

        public void ShowWindow(List<Reward> rewards)
        {
            var window = _viewModelFactory.CreateViewModel<RewardsWindowViewModel, List<Reward>>(rewards);
            ShowWindow(window);
        }
    }
}
