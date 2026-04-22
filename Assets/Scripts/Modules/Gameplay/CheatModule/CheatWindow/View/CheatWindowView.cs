using UnityEngine;

namespace vikwhite
{
    public class CheatWindowView : WindowView<CheatWindowHierarchy, CheatWindowViewModel>
    {
        private readonly IMapItemViewFactory _mapItemViewFactory;
        
        public CheatWindowView(GameObject view, IMapItemViewFactory mapItemViewFactory) : base(view)
        {
            _mapItemViewFactory = mapItemViewFactory;
        }
        
        protected override void UpdateViewModel(CheatWindowViewModel viewModel)
        {
            foreach (Transform child in _view.MapItemContainer) GameObject.Destroy(child.gameObject);
            foreach (var mapItem in viewModel.MapItems)
            {
                _mapItemViewFactory.Get(mapItem, _view.MapItemContainer);
            }
        }
    }
}