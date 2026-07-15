using UnityEngine;

namespace vikwhite
{
    public class RoomLineView : WindowView<RoomLineHierarchy, RoomLineViewModel>
    {
        public RoomLineView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(RoomLineViewModel viewModel)
        {
            _view.Label.text = viewModel.Label;
            _view.Value.text = viewModel.Value;
        }
    }
}
