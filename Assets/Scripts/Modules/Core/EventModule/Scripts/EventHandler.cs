using System;

namespace vikwhite
{
    public interface IEventHandler
    {
        Type EventType { get; }
        void Handle(object eventData);
    }
    
    public abstract class EventHandler<TEvent> : IEventHandler
    {
        public Type EventType => typeof(TEvent);

        public void Handle(object eventData)
        {
            if (eventData is null) return;
            if (eventData is not TEvent typedEvent) return;
            Handle(typedEvent);
        }

        protected abstract void Handle(TEvent evnt);
    }
}