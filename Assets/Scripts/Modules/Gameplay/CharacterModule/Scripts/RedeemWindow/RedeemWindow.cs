namespace vikwhite
{
    public interface IRedeemWindow : IWindowPresenter
    {
        void ShowWindow(Character character);
    }
    
    public class RedeemWindow : WindowPresenter<RedeemWindowView, RedeemWindowViewModel>, IRedeemWindow
    {
        public override string AssetName => "UI/Prefabs/RedeemWindow/RedeemWindow";
        
        public void ShowWindow(Character character)
        {
            var window = _viewModelFactory.CreateViewModel<RedeemWindowViewModel, Character>(character);
            ShowWindow(window);
        }
    }
}
