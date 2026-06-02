namespace vikwhite
{
    public class EventsModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<IEventsService, EventsService>();

            Register<IEventItemViewFactory, EventItemViewFactory>();
            Register<EventItemViewModel>();
            Register<EventItemView>();

            Register<IEventWindow, EventWindow>();
            Register<EventWindowViewModel>();
            Register<EventWindowView>();
        }
    }
}
