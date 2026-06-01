namespace vikwhite
{
    public interface ILoadingScreenWindow : IWindowPresenter
    {
        void ShowWindow();
    }

    public class LoadingScreenWindow : WindowPresenter<LoadingScreenView, LoadingScreenViewModel>, ILoadingScreenWindow
    {
        public override string AssetName => "UI/Prefabs/LoadingScreen/LoadingScreen";

        public void ShowWindow()
        {
            var viewModel = _viewModelFactory.CreateViewModel<LoadingScreenViewModel>();
            ShowWindow(viewModel);
        }
    }
}
