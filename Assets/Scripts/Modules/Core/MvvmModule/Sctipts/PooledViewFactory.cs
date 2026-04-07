using UnityEngine;

namespace vikwhite
{
    public interface IPooledViewFactory<TView, TViewModel> where TView : View, IView<TViewModel> where TViewModel : IViewModel
    {
        TView Get(TViewModel viewModel, Transform parent);
    }
    
    public abstract class PooledViewFactory<TView, TViewModel> : IPooledViewFactory<TView, TViewModel> where TView : View, IView<TViewModel> where TViewModel : IViewModel
    {
        private readonly IAssetLoader _assetLoader = DI.Resolve<IAssetLoader>();
        private readonly IViewFactory _viewFactory = DI.Resolve<IViewFactory>();
        private readonly GameObject _prefab;
        
        protected PooledViewFactory() 
        {
            _prefab = _assetLoader.Load(AssetName);
        }

        public abstract string AssetName { get; }

        public TView Get(TViewModel viewModel, Transform parent)
        {
            TView view = _viewFactory.CreateView<TView>(_prefab, parent);
            view.Initialize(viewModel);
            return view;
        }
    }
}