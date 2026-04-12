using UnityEngine;

namespace vikwhite
{
    public class MapWindowView : WindowView<MapWindowHierarchy, MapWindowViewModel>
    {
        private readonly IMapItemViewFactory _mapItemViewFactory;
        
        public MapWindowView(GameObject view, IMapItemViewFactory mapItemViewFactory) : base(view)
        {
            _mapItemViewFactory = mapItemViewFactory;
        }
        
        protected override void UpdateViewModel(MapWindowViewModel viewModel)
        {
            foreach (Transform child in _view.MapItemContainer) GameObject.Destroy(child.gameObject);
            foreach (var mapItem in viewModel.MapItems)
            {
                _mapItemViewFactory.Get(mapItem, _view.MapItemContainer);
            }
        }
    }
}