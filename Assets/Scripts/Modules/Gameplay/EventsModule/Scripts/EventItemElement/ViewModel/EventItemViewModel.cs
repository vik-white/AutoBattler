using UnityEngine.Events;

namespace vikwhite
{
    public class EventItemViewModel : WindowViewModel<GameEvent>
    {
        private readonly IEventWindow _eventWindow;

        public string Name;
        public GameEventType Type;
        public UnityAction OnClick;

        public EventItemViewModel(GameEvent model, IEventWindow eventWindow) : base(model)
        {
            _eventWindow = eventWindow;
            Name = model.Name;
            Type = model.Type;
            OnClick = OpenWindow;
        }

        private void OpenWindow()
        {
            _eventWindow.ShowWindow(Model);
        }

        public override void Dispose()
        {
            base.Dispose();
            OnClick = null;
        }
    }
}
