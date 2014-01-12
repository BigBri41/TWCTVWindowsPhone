namespace Caliburn.Micro
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;

    public abstract class ConductorBase<T> : Screen, IConductor, INotifyPropertyChangedEx, INotifyPropertyChanged, IParent<T>, IParent
    {
        private ICloseStrategy<T> closeStrategy;

        public event EventHandler<ActivationProcessedEventArgs> ActivationProcessed;

        protected ConductorBase()
        {
            this.ActivationProcessed = delegate {
            };
        }

        public abstract void ActivateItem(T item);
        void IConductor.ActivateItem(object item)
        {
            this.ActivateItem((T) item);
        }

        void IConductor.DeactivateItem(object item, bool close)
        {
            this.DeactivateItem((T) item, close);
        }

        IEnumerable IParent.GetChildren()
        {
            return this.GetChildren();
        }

        public abstract void DeactivateItem(T item, bool close);
        protected virtual T EnsureItem(T newItem)
        {
            IChild child = newItem as IChild;
            if ((child != null) && (child.Parent != this))
            {
                child.Parent = this;
            }
            return newItem;
        }

        public abstract IEnumerable<T> GetChildren();
        protected virtual void OnActivationProcessed(T item, bool success)
        {
            if (item != null)
            {
                ActivationProcessedEventArgs e = new ActivationProcessedEventArgs {
                    Item = item,
                    Success = success
                };
                this.ActivationProcessed(this, e);
            }
        }

        public ICloseStrategy<T> CloseStrategy
        {
            get
            {
                return (this.closeStrategy ?? (this.closeStrategy = new DefaultCloseStrategy<T>(false)));
            }
            set
            {
                this.closeStrategy = value;
            }
        }
    }
}

