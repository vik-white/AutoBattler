using System.Collections.Generic;
using UnityEngine;

namespace vikwhite
{
    public interface IWindowViewFactory
    {
        void Initialize(Transform container);
        TView GetWindowView<TView>(string assetName) where TView : View, IWindowView;
    }
    
    public class WindowViewData
    {
        public GameObject Asset;
        public IWindowView View;
    }
    
    public class WindowViewFactory : IWindowViewFactory
    {
        private readonly IAssetLoader _assetLoader;
        private readonly IViewFactory _viewFactory;
        private readonly Dictionary<string, WindowViewData> _windowViews = new();
        private Transform _container;

        public WindowViewFactory(IAssetLoader assetLoader, IViewFactory viewFactory)
        {
            _assetLoader = assetLoader;
            _viewFactory = viewFactory;
        }
        
        public void Initialize(Transform container)
        {
            _container = container;
        }

        public TView GetWindowView<TView>(string assetName) where TView : View, IWindowView
        {
            if (!_windowViews.TryGetValue(assetName, out WindowViewData viewData))
            {
                GameObject windowAsset = _assetLoader.Load(assetName);
                TView window = _viewFactory.CreateView<TView>(windowAsset, _container);
                viewData = new WindowViewData()
                {
                    Asset = windowAsset,
                    View = window,
                };
                _windowViews.Add(assetName, viewData);
            }
            return (TView) viewData.View;
        }
        
        public void Dispose()
        {
            foreach (WindowViewData viewData in _windowViews.Values)
            {
                viewData.View.DisposeAndDestroy();
                //viewData.Asset.Dispose();
            }
            _windowViews.Clear();
        }
    }
}