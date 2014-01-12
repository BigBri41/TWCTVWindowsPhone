namespace Caliburn.Micro
{
    using System;

    public interface IEventAggregator
    {
        void Publish(object message);
        void Publish(object message, Action<System.Action> marshal);
        void Subscribe(object instance);
        void Unsubscribe(object instance);

        Action<System.Action> PublicationThreadMarshaller { get; set; }
    }
}

