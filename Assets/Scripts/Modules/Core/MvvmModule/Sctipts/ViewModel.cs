using System;

namespace vikwhite
{
    public interface IViewModel : IDisposable { }

    public interface IViewModel<T> : IViewModel { }

    public abstract class ViewModel<TModel> : ViewModel, IViewModel<TModel>
    {
        public TModel Model { get; }

        protected ViewModel(TModel model)
        {
            Model = model;
        }
    }
    
    public abstract class ViewModel : DisposableCollector, IViewModel
    {
        protected readonly IViewModelFactory _viewModelFactory = DI.Resolve<IViewModelFactory>();

        protected TViewModel CreateViewModel<TViewModel, TTargetModel>(TTargetModel model) where TViewModel : ViewModel<TTargetModel>
        {
            TViewModel viewModel = _viewModelFactory.CreateViewModel<TViewModel, TTargetModel>(model);
            return viewModel;
        }
    }
}