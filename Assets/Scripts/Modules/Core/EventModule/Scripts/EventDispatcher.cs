using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            var handlersList = handlers.ToList();
            _handlers = handlersList.ToDictionary(h => h.EventType);
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
