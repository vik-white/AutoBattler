namespace vikwhite
{
    public interface IEventWindow : IWindowPresenter
    {
        void ShowWindow(GameEvent gameEvent);
    }

    public class EventWindow : WindowPresenter<EventWindowView, EventWindowViewModel>, IEventWindow
    {
        public override string AssetName => "UI/Prefabs/EventWindow/EventWindow";

        public void ShowWindow(GameEvent gameEvent)
        {
            var window = _viewModelFactory.CreateViewModel<EventWindowViewModel, GameEvent>(gameEvent);
            ShowWindow(window);
        }
    }
}
