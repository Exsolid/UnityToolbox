using System;
using System.Collections.Generic;

namespace UnityToolbox.General.SubEvents
{
    /// <summary>
    /// A generic version for subscription events.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PubSubEvent<T> : PubSubEventBase
    {
        private readonly Dictionary<SubscriptionToken, Action<T>> _delegates;

        public PubSubEvent()
        {
            _delegates = new Dictionary<SubscriptionToken, Action<T>>();
        }

        /// <summary>
        /// Subscribes to the event with a given delegate.
        /// </summary>
        /// <param name="action">The delegate which should be exectued once the event is triggered.</param>
        /// <returns>The token which is used to indentify the delegate.</returns>
        public SubscriptionToken Subscribe(Action<T> action)
        {
            SubscriptionToken g = new SubscriptionToken();
            _delegates.Add(g, action);
            return g;
        }

        /// <summary>
        /// Unsubscribes from the event.
        /// </summary>
        /// <param name="token">The token which is used to indentify the delegate.</param>
        public void Unsubscribe(SubscriptionToken token)
        {
            _delegates.Remove(token);
        }

        /// <summary>
        /// All subscripted delegates will be exectuted.
        /// </summary>
        public void Publish(T value)
        {
            foreach (KeyValuePair<SubscriptionToken, Action<T>> @delegate in _delegates)
            {
                @delegate.Value.Invoke(value);
            }
        }
    }

    /// <summary>
    /// The default implementation for subscription events.
    /// </summary>
    public class PubSubEvent : PubSubEventBase
    {
        private readonly Dictionary<SubscriptionToken, Action> _delegates;

        public PubSubEvent()
        {
            _delegates = new Dictionary<SubscriptionToken, Action>();
        }

        /// <summary>
        /// Subscribes to the event with a given delegate.
        /// </summary>
        /// <param name="action">The delegate which should be exectued once the event is triggered.</param>
        /// <returns>The token which is used to indentify the delegate.</returns>
        public SubscriptionToken Subscribe(Action action)
        {
            SubscriptionToken g = new SubscriptionToken();
            _delegates.Add(g, action);
            return g;
        }

        /// <summary>
        /// Subscribes to the event with a given delegate.
        /// </summary>
        /// <returns>The token which is used to indentify the delegate.</returns>
        public void Unsubscribe(SubscriptionToken token)
        {
            _delegates.Remove(token);
        }

        /// <summary>
        /// All subscripted delegates will be exectuted.
        /// </summary>
        public void Publish()
        {
            foreach (KeyValuePair<SubscriptionToken, Action> @delegate in _delegates)
            {
                @delegate.Value.Invoke();
            }
        }
    }
}