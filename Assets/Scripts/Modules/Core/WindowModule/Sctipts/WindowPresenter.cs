namespace vikwhite
{
    public abstract class WindowPresenter<TView, TViewModel> where TView : View, IWindowView<TViewModel> where TViewModel : IWindowViewModel
    {
        protected readonly IWindowViewFactory _windowViewFactory = DI.Resolve<IWindowViewFactory>();
        protected readonly IViewModelFactory _viewModelFactory = DI.Resolve<IViewModelFactory>();
        protected readonly IWindowManager _windowManager = DI.Resolve<IWindowManager>();
        private bool _isShowing;
        private TView _view;
        private TViewModel _viewModel;
        public abstract string AssetName { get; }

        public void ShowWindow(TViewModel viewModel)
        {
            if (_isShowing) _viewModel.Close();
            _viewModel = viewModel;
            _view = _windowViewFactory.GetWindowView<TView>(AssetName);
            _view.Initialize(_viewModel);
            _viewModel.OnClose += CloseWindow;
            _windowManager.ShowWindow(_view);
            _isShowing = true;
        }
        
        private void CloseWindow()
        {
            if (_isShowing == false) return;
            _viewModel.OnClose -= CloseWindow;
            _windowManager.CloseWindow(_view);
            _isShowing = false;
        }
    }
}