namespace Caliburn.Micro
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;

    public class ActionExecutionContext : IDisposable
    {
        public System.Func<bool> CanExecute;
        public object EventArgs;
        private WeakReference message;
        public MethodInfo Method;
        private WeakReference source;
        private WeakReference target;
        private Dictionary<string, object> values;
        private WeakReference view;

        public event EventHandler Disposing;



        public void Dispose()
        {
            this.Disposing(this, System.EventArgs.Empty);
        }

        public object this[string key]
        {
            get
            {
                object obj2;
                if (this.values == null)
                {
                    this.values = new Dictionary<string, object>();
                }
                this.values.TryGetValue(key, out obj2);
                return obj2;
            }
            set
            {
                if (this.values == null)
                {
                    this.values = new Dictionary<string, object>();
                }
                this.values[key] = value;
            }
        }

        public ActionMessage Message
        {
            get
            {
                return ((this.message == null) ? null : (this.message.Target as ActionMessage));
            }
            set
            {
                this.message = new WeakReference(value);
            }
        }

        public FrameworkElement Source
        {
            get
            {
                return ((this.source == null) ? null : (this.source.Target as FrameworkElement));
            }
            set
            {
                this.source = new WeakReference(value);
            }
        }

        public object Target
        {
            get
            {
                return ((this.target == null) ? null : this.target.Target);
            }
            set
            {
                this.target = new WeakReference(value);
            }
        }

        public DependencyObject View
        {
            get
            {
                return ((this.view == null) ? null : (this.view.Target as DependencyObject));
            }
            set
            {
                this.view = new WeakReference(value);
            }
        }
    }
}

