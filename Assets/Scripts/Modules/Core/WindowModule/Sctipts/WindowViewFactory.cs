using System.Collections.Generic;
using UnityEngine;

namespace vikwhite
{
    public interface IWindowViewFactory
    {
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
        private readonly IUIRoot _uiRoot;
        private readonly Dictionary<string, WindowViewData> _windowViews = new();

        public WindowViewFactory(IAssetLoader assetLoader, IViewFactory viewFactory, IUIRoot uiRoot)
        {
            _assetLoader = assetLoader;
            _viewFactory = viewFactory;
            _uiRoot = uiRoot;
        }

        public TView GetWindowView<TView>(string assetName) where TView : View, IWindowView
        {
            if (!_windowViews.TryGetValue(assetName, out WindowViewData viewData))
            {
                GameObject windowAsset = _assetLoader.Load(assetName);
                TView window = _viewFactory.CreateView<TView>(windowAsset, _uiRoot.GetLayer(UILayer.WINDOW));
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