namespace Caliburn.Micro
{
    using System;

    public interface IActivate
    {
        event EventHandler<ActivationEventArgs> Activated;

        void Activate();

        bool IsActive { get; }
    }
}

