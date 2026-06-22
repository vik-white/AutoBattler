using UnityEngine;

namespace vikwhite
{
    public class MapItemView : WindowView<MapItemHierarchy, MapItemViewModel>
    {
        public MapItemView(GameObject view) : base(view) { }
        
        protected override void UpdateViewModel(MapItemViewModel viewModel)
        {
            _view.Title.text = viewModel.Title;
            BindClick(_view.Button, viewModel.OnSelect);
        }
    }
}