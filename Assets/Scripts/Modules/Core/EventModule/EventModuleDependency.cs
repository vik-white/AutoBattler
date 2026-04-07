namespace vikwhite
{
    public class EventModuleDependency : DiModule
    {
        protected override void Register()
        {
            Register<IEventDispatcher, EventDispatcher>();
        }
    }
}