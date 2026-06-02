using UnityEngine;

namespace vikwhite
{
    public class EventItemView : WindowView<EventItemHierarchy, EventItemViewModel>
    {
        public EventItemView(GameObject view) : base(view) { }

        protected override void UpdateViewModel(EventItemViewModel viewModel)
        {
            if (_view.Name != null) _view.Name.text = viewModel.Name;
            BindClick(_view.Button, viewModel.OnClick);
        }
    }
}
