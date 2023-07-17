using System;
using System.Collections.Generic;

//TODO as static, generate when not existing
namespace UnityToolbox.Events
{
    /// <summary>
    /// The main module for all events.
    /// </summary>
    public class EventAggregator : Module
    {
        private readonly Dictionary<Type, PubSubEventBase> _events;

        public EventAggregator()
        {
            _events = new Dictionary<Type, PubSubEventBase>();
        }

        /// <summary>
        /// Returns a event of given type. Creates one if it does not exist yet.
        /// </summary>
        /// <typeparam name="T">The type of the event. Must be <see cref="PubSubEventBase"/></typeparam>
        /// <returns>A <see cref="PubSubEventBase"/></returns>
        public T GetEvent<T>() where T : PubSubEventBase, new()
        {
            if (!_events.ContainsKey(typeof(T)))
                _events.Add(typeof(T), new T());

            return (T)_events[typeof(T)];
        }
    }
}
