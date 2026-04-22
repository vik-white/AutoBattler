namespace vikwhite
{
    public interface ISquadWindow : IWindowPresenter
    {
        void ShowWindow();
    }
    
    public class SquadWindow : WindowPresenter<SquadWindowView, SquadWindowViewModel>, ISquadWindow
    {
        public override string AssetName => "UI/Prefabs/SquadWindow/SquadWindow";
        
        public void ShowWindow()
        {
            var window = _viewModelFactory.CreateViewModel<SquadWindowViewModel>();
            ShowWindow(window);
        }
    }
}