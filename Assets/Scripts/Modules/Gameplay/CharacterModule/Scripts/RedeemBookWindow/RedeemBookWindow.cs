namespace vikwhite
{
    public interface IRedeemBookWindow : IWindowPresenter
    {
        void ShowWindow(Character character);
    }

    public class RedeemBookWindow : WindowPresenter<RedeemBookWindowView, RedeemBookWindowViewModel>, IRedeemBookWindow
    {
        public override string AssetName => "UI/Prefabs/RedeemBookWindow/RedeemBookWindow";

        public void ShowWindow(Character character)
        {
            var window = _viewModelFactory.CreateViewModel<RedeemBookWindowViewModel, Character>(character);
            ShowWindow(window);
        }
    }
}
