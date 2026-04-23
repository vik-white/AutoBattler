namespace vikwhite
{
    public interface IVictoryWindow : IWindowPresenter
    {
        void ShowWindow();
    }
    
    public class VictoryWindow : WindowPresenter<VictoryWindowView, VictoryWindowViewModel>, IVictoryWindow
    {
        public override string AssetName => "UI/Prefabs/VictoryWindow/VictoryWindow";
        public void ShowWindow()
        {
            var window = _viewModelFactory.CreateViewModel<VictoryWindowViewModel>();
            ShowWindow(window);
        }
    }
}