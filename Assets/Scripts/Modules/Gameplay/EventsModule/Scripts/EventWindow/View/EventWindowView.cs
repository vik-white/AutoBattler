using UnityEngine;

namespace vikwhite
{
    public class EventWindowView : WindowView<EventWindowHierarchy, EventWindowViewModel>
    {
        private readonly IQuestItemViewFactory _questItemFactory;

        public EventWindowView(GameObject view, IQuestItemViewFactory questItemFactory) : base(view)
        {
            _questItemFactory = questItemFactory;
        }

        protected override void UpdateViewModel(EventWindowViewModel viewModel)
        {
            if (_view.Title != null) _view.Title.text = viewModel.Title;
            BindClick(_view.CloseButton, viewModel.Close);

            _view.QuestsContainer.gameObject.SetActive(viewModel.Type == GameEventType.Quest);
            _view.QuestsContainer.ClearChildren();

            if (viewModel.Type == GameEventType.Quest)
            {
                foreach (var quest in viewModel.Quests)
                    _questItemFactory.Get(quest, _view.QuestsContainer);
            }
        }
    }
}
