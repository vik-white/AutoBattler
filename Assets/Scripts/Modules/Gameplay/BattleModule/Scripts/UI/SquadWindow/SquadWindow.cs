namespace vikwhite
{
    public interface ISquadWindow : IWindowPresenter
    {
        bool IsBattleStartAnimationRequested { get; }
        void ShowWindow();
    }
    
    public class SquadWindow : WindowPresenter<SquadWindowView, SquadWindowViewModel>, ISquadWindow
    {
        public bool IsBattleStartAnimationRequested { get; private set; }
        public override string AssetName => "UI/Prefabs/SquadWindow/SquadWindow";
        
        public void ShowWindow()
        {
            IsBattleStartAnimationRequested = false;
            var window = _viewModelFactory.CreateViewModel<SquadWindowViewModel>();
            ShowWindow(window);
        }
    }
}
