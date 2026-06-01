using UniRx;

namespace vikwhite
{
    public class LoadingScreenViewModel : WindowViewModel
    {
        private readonly ILoadingScreenService _loadingScreenService;

        public IReadOnlyReactiveProperty<float> Progress => _loadingScreenService.Progress;

        public LoadingScreenViewModel(ILoadingScreenService loadingScreenService)
        {
            _loadingScreenService = loadingScreenService;
        }
    }
}
