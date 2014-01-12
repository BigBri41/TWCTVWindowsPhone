namespace Caliburn.Micro
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Runtime.Serialization;
    using System.Threading;

    public class PropertyChangedBase : INotifyPropertyChangedEx, INotifyPropertyChanged
    {
        private bool isNotifying;

        public event PropertyChangedEventHandler PropertyChanged;

        public PropertyChangedBase()
        {
            this.IsNotifying = true;
        }

        public virtual void NotifyOfPropertyChange<TProperty>(Expression<System.Func<TProperty>> property)
        {
            this.NotifyOfPropertyChange(property.GetMemberInfo().Name);
        }

        public virtual void NotifyOfPropertyChange(string propertyName)
        {
            System.Action action = null;
            if (this.IsNotifying)
            {
                if (action == null)
                {
                    action = () => this.RaisePropertyChangedEventCore(propertyName);
                }
                action.OnUIThread();
            }
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext c)
        {
            this.IsNotifying = true;
        }

        private void RaisePropertyChangedEventCore(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public virtual void RaisePropertyChangedEventImmediately(string propertyName)
        {
            if (this.IsNotifying)
            {
                this.RaisePropertyChangedEventCore(propertyName);
            }
        }

        public void Refresh()
        {
            this.NotifyOfPropertyChange(string.Empty);
        }

        public virtual bool ShouldSerializeIsNotifying()
        {
            return false;
        }

        public bool IsNotifying
        {
            get
            {
                return this.isNotifying;
            }
            set
            {
                this.isNotifying = value;
            }
        }
    }
}

