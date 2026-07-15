using UnityEngine;

namespace vikwhite
{
    public class RoomLineView : WindowView<RoomLineHierarchy, RoomLineViewModel>
    {
        private readonly Color _defaultValueColor;

        public RoomLineView(GameObject view) : base(view)
        {
            _defaultValueColor = _view.Value.color;
        }

        protected override void UpdateViewModel(RoomLineViewModel viewModel)
        {
            _view.Label.text = viewModel.Label;
            _view.Value.text = viewModel.Value;
            _view.Value.color = viewModel.RequirementMet.HasValue
                ? viewModel.RequirementMet.Value ? Color.green : Color.red
                : _defaultValueColor;
        }
    }
}
