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
        private readonly Dictionary<Type, List<IEventHandler>> _handlers;

        public EventDispatcher(IEnumerable<IEventHandler> handlers)
        {
            _handlers = handlers
                .GroupBy(handler => handler.EventType)
                .ToDictionary(group => group.Key, group => group.ToList());
        }

        public void Dispatch(object eventData)
        {
            if (eventData == null) return;
            if (!_handlers.TryGetValue(eventData.GetType(), out var handlers)) return;
            for (int i = 0; i < handlers.Count; i++)
                handlers[i].Handle(eventData);
        }
    }
}
