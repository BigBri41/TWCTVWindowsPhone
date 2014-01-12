namespace Caliburn.Micro
{
    using System;
    using System.Runtime.InteropServices;

    public interface IViewAware
    {
        event EventHandler<ViewAttachedEventArgs> ViewAttached;

        void AttachView(object view, object context = null);
        object GetView(object context = null);
    }
}

