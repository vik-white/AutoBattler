using System.Collections.Generic;

namespace vikwhite
{
    public class Dispatcher: Component
    {
        protected IEventDispatcher _dispatcher = DI.Resolve<IEventDispatcher>();
        protected void Dispatch<T>(T data) => _dispatcher.Dispatch(data);
    }
}