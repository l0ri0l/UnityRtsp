using System.Collections.Generic;

namespace Arwel.Scripts.Domains.EventBus
{
    public static class EventBus<T> where T : class, IEvent
    {
        private static List<IEventSubscriber<T>> Subscribers = new();
               
        public static void Register(IEventSubscriber<T> eventSubscriber)
        {
            Subscribers.Add(eventSubscriber);
        }

        public static void UnRegister(IEventSubscriber<T> eventSubscriber)
        {
            Subscribers.Remove(eventSubscriber);
        }

        public static void Raise(T e)
        {
            foreach(var subs in Subscribers)
            {
                subs.OnEvent(e);
            }
        }

        public static void Clear()
        {
            Subscribers.Clear();
        }

    }
}
