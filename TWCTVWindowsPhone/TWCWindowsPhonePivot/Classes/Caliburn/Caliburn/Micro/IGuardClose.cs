namespace Caliburn.Micro
{
    using System;

    public interface IGuardClose : IClose
    {
        void CanClose(Action<bool> callback);
    }
}

