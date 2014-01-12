namespace Caliburn.Micro
{
    using System;

    public interface IDeactivate
    {
        event EventHandler<DeactivationEventArgs> AttemptingDeactivation;

        event EventHandler<DeactivationEventArgs> Deactivated;

        void Deactivate(bool close);
    }
}

