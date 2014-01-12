namespace Caliburn.Micro
{
    using System;

    public interface IHandle<TMessage> : IHandle
    {
        void Handle(TMessage message);
    }
}

