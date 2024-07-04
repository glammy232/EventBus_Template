using EventBus.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBus
{
    public static class EventBus
    {
        private static Dictionary<Type, List<IGlobalSubscriber>> Subscribers
        = new Dictionary<Type, List<IGlobalSubscriber>>();

        public static void Subscribe(IGlobalSubscriber subscriber)
        {
            List<Type> subscriberTypes = GetSubscribersTypes(subscriber);
            foreach (Type t in subscriberTypes)
            {
                if (!Subscribers.ContainsKey(t))
                    Subscribers[t] = new List<IGlobalSubscriber>();
                Subscribers[t].Add(subscriber);
            }
        }
        public static void Unsubscribe(IGlobalSubscriber subscriber)
        {
            List<Type> subscriberTypes = GetSubscribersTypes(subscriber);
            foreach (Type t in subscriberTypes)
            {
                if (Subscribers.ContainsKey(t))
                    Subscribers[t].Remove(subscriber);
            }
        }
        public static void RaiseEvent<TSubscriber>(Action<TSubscriber> action)
        where TSubscriber : IGlobalSubscriber
        {
            List<IGlobalSubscriber> subscribers = Subscribers[typeof(TSubscriber)];
            foreach (IGlobalSubscriber subscriber in subscribers)
            {
                action.Invoke(subscriber.ConvertTo<TSubscriber>());// áûëî subscriber as TSubscriber
            }
        }
        public static List<Type> GetSubscribersTypes(IGlobalSubscriber globalSubscriber)
        {
            Type type = globalSubscriber.GetType();
            List<Type> subscriberTypes = type
                .GetInterfaces()
                .Where(it =>
                        it.GetInterfaces().Contains(typeof(IGlobalSubscriber)) &&
                        it != typeof(IGlobalSubscriber))
                .ToList();
            return subscriberTypes;
        }
    }
}
