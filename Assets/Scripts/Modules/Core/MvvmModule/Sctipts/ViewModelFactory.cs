namespace vikwhite
{
    public interface IViewModelFactory
    {
        TViewModel CreateViewModel<TViewModel, TTargetModel>(TTargetModel model) where TViewModel : IViewModel<TTargetModel>;
        TViewModel CreateViewModel<TViewModel>() where TViewModel : IViewModel;
    }
    
    public class ViewModelFactory : IViewModelFactory
    {
        private readonly DiContainer _container;
        
        public ViewModelFactory(DiContainer container)
        {
            _container = container;
        }
		
        public TViewModel CreateViewModel<TViewModel, TTargetModel>(TTargetModel model) where TViewModel : IViewModel<TTargetModel>
        {
            TViewModel viewModel = _container.Resolve<TViewModel, TTargetModel>(model);
            return viewModel;
        }

        public TViewModel CreateViewModel<TViewModel>() where TViewModel : IViewModel
        {
            TViewModel viewModel = _container.Resolve<TViewModel>();
            return viewModel;
        }
    }
}
