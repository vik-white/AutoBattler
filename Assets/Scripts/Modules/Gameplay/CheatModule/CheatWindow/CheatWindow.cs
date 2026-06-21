namespace vikwhite
{
    public interface ICheatWindow : IWindowPresenter
    {
        void ShowWindow();
        void ToggleWindow();
    }
    
    public class CheatWindow : WindowPresenter<CheatWindowView, CheatWindowViewModel>, ICheatWindow
    {
        public override string AssetName => "UI/Prefabs/CheatWindow/CheatWindow";
        public void ShowWindow()
        {
            var window = _viewModelFactory.CreateViewModel<CheatWindowViewModel>();
            ShowWindow(window);
        }

        public void ToggleWindow()
        {
            if (IsShowing)
            {
                CloseWindow();
                return;
            }

            ShowWindow();
        }
    }
}
