namespace vikwhite
{
    public interface IRewardsWindow : IWindowPresenter
    {
        void ShowWindow(string rewardId);
    }

    public class RewardsWindow : WindowPresenter<RewardsWindowView, RewardsWindowViewModel>, IRewardsWindow
    {
        public override string AssetName => "UI/Prefabs/RewardsWindow/RewardsWindow";

        public void ShowWindow(string rewardId)
        {
            var window = _viewModelFactory.CreateViewModel<RewardsWindowViewModel, string>(rewardId);
            ShowWindow(window);
        }
    }
}
