using System;
using System.Collections.Generic;
using System.Linq;

namespace vikwhite
{
    public interface IEventDispatcher
    {
        void Dispatch(object eventData);
    }
    
    public class EventDispatcher : IEventDispatcher
    {
        private readonly Dictionary<Type, IEventHandler> _handlers;

        public EventDispatcher(IEnumerable<IEventHandler> handlers)
        {
            _handlers = handlers.ToDictionary(h => h.EventType);
        }
        
        public void Dispatch(object eventData)
        {
            var type = eventData.GetType();
            if (_handlers.TryGetValue(type, out var handler))
            {
                handler.Handle(eventData);
            }
        }
    }
}