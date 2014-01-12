namespace Caliburn.Micro
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Threading;

    public class Screen : ViewAware, IScreen, IHaveDisplayName, IActivate, IDeactivate, IGuardClose, IClose, INotifyPropertyChangedEx, INotifyPropertyChanged, IChild
    {
        private string displayName;
        private bool isActive;
        private bool isInitialized;
        private static readonly ILog Log = LogManager.GetLog(typeof(Screen));
        private object parent;

        public event EventHandler<ActivationEventArgs> Activated;

        public event EventHandler<DeactivationEventArgs> AttemptingDeactivation;

        public event EventHandler<DeactivationEventArgs> Deactivated;

        public Screen()
        {
            this.DisplayName = base.GetType().FullName;
        }

        void IActivate.Activate()
        {
            if (!this.IsActive)
            {
                bool flag = false;
                if (!this.IsInitialized)
                {
                    this.IsInitialized = flag = true;
                    this.OnInitialize();
                }
                this.IsActive = true;
                Log.Info("Activating {0}.", new object[] { this });
                this.OnActivate();
                ActivationEventArgs e = new ActivationEventArgs {
                    WasInitialized = flag
                };
                this.Activated(this, e);
            }
        }

        void IDeactivate.Deactivate(bool close)
        {
            if (this.IsActive || this.IsInitialized)
            {
                DeactivationEventArgs e = new DeactivationEventArgs {
                    WasClosed = close
                };
                this.AttemptingDeactivation(this, e);
                this.IsActive = false;
                Log.Info("Deactivating {0}.", new object[] { this });
                this.OnDeactivate(close);
                DeactivationEventArgs args2 = new DeactivationEventArgs {
                    WasClosed = close
                };
                this.Deactivated(this, args2);
                if (close)
                {
                    base.Views.Clear();
                    Log.Info("Closed {0}.", new object[] { this });
                }
            }
        }

        public virtual void CanClose(Action<bool> callback)
        {
            callback(true);
        }

        private System.Action GetViewCloseAction(bool? dialogResult)
        {
            System.Action action2 = null;
            IConductor conductor = this.Parent as IConductor;
            if (conductor != null)
            {
                if (action2 == null)
                {
                    action2 = delegate {
                        conductor.CloseItem(this);
                    };
                }
                return action2;
            }
            using (Dictionary<object, object>.ValueCollection.Enumerator enumerator = base.Views.Values.GetEnumerator())
            {
                object contextualView;
                while (enumerator.MoveNext())
                {
                    contextualView = enumerator.Current;
                    System.Action action = null;
                    Type type = contextualView.GetType();
                    MethodInfo closeMethod = type.GetMethod("Close");
                    if (closeMethod != null)
                    {
                        return delegate {
                            closeMethod.Invoke(contextualView, null);
                        };
                    }
                    PropertyInfo isOpenProperty = type.GetProperty("IsOpen");
                    if (isOpenProperty != null)
                    {
                        if (action == null)
                        {
                            action = delegate {
                                isOpenProperty.SetValue(contextualView, false, null);
                            };
                        }
                        return action;
                    }
                }
            }
            return delegate {
                NotSupportedException exception = new NotSupportedException("TryClose requires a parent IConductor or a view with a Close method or IsOpen property.");
                Log.Error(exception);
                throw exception;
            };
        }

        protected virtual void OnActivate()
        {
        }

        protected virtual void OnDeactivate(bool close)
        {
        }

        protected virtual void OnInitialize()
        {
        }

        public void TryClose()
        {
            () => this.GetViewCloseAction(null)().OnUIThread();
        }

        public virtual string DisplayName
        {
            get
            {
                return this.displayName;
            }
            set
            {
                this.displayName = value;
                this.NotifyOfPropertyChange("DisplayName");
            }
        }

        public bool IsActive
        {
            get
            {
                return this.isActive;
            }
            private set
            {
                this.isActive = value;
                this.NotifyOfPropertyChange("IsActive");
            }
        }

        public bool IsInitialized
        {
            get
            {
                return this.isInitialized;
            }
            private set
            {
                this.isInitialized = value;
                this.NotifyOfPropertyChange("IsInitialized");
            }
        }

        public virtual object Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                this.parent = value;
                this.NotifyOfPropertyChange("Parent");
            }
        }
    }
}

