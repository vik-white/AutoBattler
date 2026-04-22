namespace vikwhite
{
    public interface ILobbyWindow : IWindowPresenter
    {
        void ShowWindow();
    }
    
    public class LobbyWindow : WindowPresenter<LobbyWindowView, LobbyWindowViewModel>, ILobbyWindow
    {
        public override string AssetName => "UI/LobbyWindow/LobbyWindow";
        
        public void ShowWindow()
        {
            var window = _viewModelFactory.CreateViewModel<LobbyWindowViewModel>();
            ShowWindow(window);
        }
    }
}