using System.Collections.Generic;

namespace vikwhite
{
    public interface IMapWindow : IWindowPresenter
    {
        void ShowWindow();
    }
    
    public class MapWindow : WindowPresenter<MapWindowView, MapWindowViewModel>, IMapWindow
    {
        public override string AssetName => "UI/MapWindow/MapWindow";
        public void ShowWindow()
        {
            var window = _viewModelFactory.CreateViewModel<MapWindowViewModel>();
            ShowWindow(window);
        }
    }
}