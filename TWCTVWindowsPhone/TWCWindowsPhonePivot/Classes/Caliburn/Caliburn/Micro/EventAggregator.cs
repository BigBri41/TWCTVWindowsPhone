namespace Caliburn.Micro
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class EventAggregator : IEventAggregator
    {
        public static Action<System.Action> DefaultPublicationThreadMarshaller;
        private readonly List<Handler> handlers = new List<Handler>();

        static EventAggregator()
        {
            DefaultPublicationThreadMarshaller = action => action();
        }

        public EventAggregator()
        {
            this.PublicationThreadMarshaller = DefaultPublicationThreadMarshaller;
        }

        public virtual void Publish(object message)
        {
            this.Publish(message, this.PublicationThreadMarshaller);
        }

        public virtual void Publish(object message, Action<System.Action> marshal)
        {
            Handler[] toNotify;
            lock (this.handlers)
            {
                toNotify = this.handlers.ToArray();
            }
            marshal(delegate {
                Action<Handler> action = null;
                Type messageType = message.GetType();
                List<Handler> source = (from handler in toNotify
                    where !handler.Handle(messageType, message)
                    select handler).ToList<Handler>();
                if (source.Any<Handler>())
                {
                    lock (this.handlers)
                    {
                        if (action == null)
                        {
                            action = (Action<Handler>) (x => this.handlers.Remove(x));
                        }
                        source.Apply<Handler>(action);
                    }
                }
            });
        }

        public virtual void Subscribe(object instance)
        {
            System.Func<Handler, bool> func = null;
            lock (this.handlers)
            {
                if (func == null)
                {
                    func = x => x.Matches(instance);
                }
                if (!Enumerable.Any<Handler>(this.handlers, func))
                {
                    this.handlers.Add(new Handler(instance));
                }
            }
        }

        public virtual void Unsubscribe(object instance)
        {
            System.Func<Handler, bool> func = null;
            lock (this.handlers)
            {
                if (func == null)
                {
                    func = x => x.Matches(instance);
                }
                Handler item = Enumerable.FirstOrDefault<Handler>(this.handlers, func);
                if (item != null)
                {
                    this.handlers.Remove(item);
                }
            }
        }

        public Action<System.Action> PublicationThreadMarshaller { get; set; }

        protected class Handler
        {
            private readonly WeakReference reference;
            private readonly Dictionary<Type, MethodInfo> supportedHandlers = new Dictionary<Type, MethodInfo>();

            public Handler(object handler)
            {
                this.reference = new WeakReference(handler);
                IEnumerable<Type> enumerable = from x in handler.GetType().GetInterfaces()
                    where typeof(IHandle).IsAssignableFrom(x) && x.IsGenericType
                    select x;
                foreach (Type type in enumerable)
                {
                    Type type2 = type.GetGenericArguments()[0];
                    MethodInfo method = type.GetMethod("Handle");
                    this.supportedHandlers[type2] = method;
                }
            }

            public bool Handle(Type messageType, object message)
            {
                object target = this.reference.Target;
                if (target == null)
                {
                    return false;
                }
                foreach (KeyValuePair<Type, MethodInfo> pair in this.supportedHandlers)
                {
                    if (pair.Key.IsAssignableFrom(messageType))
                    {
                        pair.Value.Invoke(target, new object[] { message });
                        return true;
                    }
                }
                return true;
            }

            public bool Matches(object instance)
            {
                return (this.reference.Target == instance);
            }
        }
    }
}

