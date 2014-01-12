namespace Caliburn.Micro
{
    using System;
    using System.Runtime.CompilerServices;

    public static class ScreenExtensions
    {
        public static void ActivateWith(this IActivate child, IActivate parent)
        {
            EventHandler<ActivationEventArgs> handler = (s, e) => child.Activate();
            parent.Activated += handler;
            IDeactivate deactivator = parent as IDeactivate;
            if (deactivator != null)
            {
                EventHandler<DeactivationEventArgs> handler2 = null;
                handler2 = delegate (object s, DeactivationEventArgs e) {
                    if (e.WasClosed)
                    {
                        parent.Activated -= handler;
                        deactivator.Deactivated -= handler2;
                    }
                };
                deactivator.Deactivated += handler2;
            }
        }

        public static void CloseItem<T>(this ConductorBase<T> conductor, T item)
        {
            conductor.DeactivateItem(item, true);
        }

        public static void CloseItem(this IConductor conductor, object item)
        {
            conductor.DeactivateItem(item, true);
        }

        public static void ConductWith<TChild, TParent>(this TChild child, TParent parent) where TChild: IActivate, IDeactivate where TParent: IActivate, IDeactivate
        {
            child.ActivateWith(parent);
            child.DeactivateWith(parent);
        }

        public static void DeactivateWith(this IDeactivate child, IDeactivate parent)
        {
            EventHandler<DeactivationEventArgs> handler = null;
            handler = delegate (object s, DeactivationEventArgs e) {
                child.Deactivate(e.WasClosed);
                if (e.WasClosed)
                {
                    parent.Deactivated -= handler;
                }
            };
            parent.Deactivated += handler;
        }

        public static void TryActivate(object potentialActivatable)
        {
            IActivate activate = potentialActivatable as IActivate;
            if (activate != null)
            {
                activate.Activate();
            }
        }

        public static void TryDeactivate(object potentialDeactivatable, bool close)
        {
            IDeactivate deactivate = potentialDeactivatable as IDeactivate;
            if (deactivate != null)
            {
                deactivate.Deactivate(close);
            }
        }
    }
}

