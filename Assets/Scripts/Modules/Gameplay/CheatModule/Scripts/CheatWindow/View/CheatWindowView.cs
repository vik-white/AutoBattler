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
            BindClick(_view.CloseButton, viewModel.Close);
            BindClick(_view.AddGemButton, viewModel.OnAddGem);
            BindClick(_view.AddExpButton, viewModel.OnAddGold);
            BindClick(_view.AddBookButton, viewModel.OnAddBook);
            BindClick(_view.AddKeyCommonButton, viewModel.OnAddKeyCommon);
            BindClick(_view.AddKeyEpicButton, viewModel.OnAddKeyEpic);
            BindClick(_view.AddGoldButton, viewModel.OnAddGold);
            foreach (Transform child in _view.MapItemContainer) GameObject.Destroy(child.gameObject);
            foreach (var mapItem in viewModel.MapItems)
            {
                _mapItemViewFactory.Get(mapItem, _view.MapItemContainer);
            }
        }
    }
}