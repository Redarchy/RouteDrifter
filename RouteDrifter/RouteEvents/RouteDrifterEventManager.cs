using System;
using System.Collections.Generic;

namespace RouteDrifter.RouteEvents
{
    public class RouteDrifterEventManager
    {
        public class EventHandlers<EventType> where EventType : RouteDrifterEvent
        {
            private List<Action<EventType>> handlers = new List<Action<EventType>>();
            private static EventHandlers<EventType> _instance = null;
            private static EventHandlers<EventType> instance { get => _instance ?? (_instance = new EventHandlers<EventType>()); }

            public static void Register(Action<EventType> handler)
            {
                if (instance.handlers.Contains(handler))
                {
                    return;
                }
                instance.handlers.Add(handler);
            }

            public static void Unregister(Action<EventType> handler)
            {
                instance.handlers.Remove(handler);
            }

            public static void Handle(EventType eventData)
            {
                if (instance.handlers != null)
                {
                    for (int i = instance.handlers.Count - 1; i >= 0; i--)
                    {
                        instance.handlers[i](eventData);
                    }
                }
            }
        }

        public static void RegisterHandler<EventType>(Action<EventType> handler) where EventType : RouteDrifterEvent
        {
            EventHandlers<EventType>.Register(handler);
        }

        public static void UnregisterHandler<EventType>(Action<EventType> handler) where EventType : RouteDrifterEvent
        {
            EventHandlers<EventType>.Unregister(handler);
        }

        public static void Send<EventType>(EventType eventData) where EventType : RouteDrifterEvent
        {
            EventHandlers<EventType>.Handle(eventData);
        }
    }
}
