namespace vikwhite
{
    public interface IRedeemShardWindow : IWindowPresenter
    {
        void ShowWindow(Character character);
    }

    public class RedeemShardWindow : WindowPresenter<RedeemShardWindowView, RedeemShardWindowViewModel>, IRedeemShardWindow
    {
        public override string AssetName => "UI/Prefabs/RedeemShardWindow/RedeemShardWindow";

        public void ShowWindow(Character character)
        {
            var window = _viewModelFactory.CreateViewModel<RedeemShardWindowViewModel, Character>(character);
            ShowWindow(window);
        }
    }
}
