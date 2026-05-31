namespace vikwhite
{
    public interface ILoadingScreenWindow : IWindowPresenter
    {
        void ShowWindow();
        void SetProgress(float progress);
    }

    public class LoadingScreenWindow : WindowPresenter<LoadingScreenView, LoadingScreenViewModel>, ILoadingScreenWindow
    {
        private LoadingScreenViewModel _viewModel;

        public override string AssetName => "UI/Prefabs/LoadingScreen/LoadingScreen";

        public void ShowWindow()
        {
            _viewModel = _viewModelFactory.CreateViewModel<LoadingScreenViewModel>();
            ShowWindow(_viewModel);
        }

        public void SetProgress(float progress)
        {
            _viewModel?.SetProgress(progress);
        }
    }
}
