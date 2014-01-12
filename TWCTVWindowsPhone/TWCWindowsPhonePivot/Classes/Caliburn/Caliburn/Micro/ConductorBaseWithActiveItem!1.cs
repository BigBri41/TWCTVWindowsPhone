namespace Caliburn.Micro
{
    using System;
    using System.ComponentModel;

    public abstract class ConductorBaseWithActiveItem<T> : ConductorBase<T>, IConductActiveItem, IConductor, IParent, INotifyPropertyChangedEx, INotifyPropertyChanged, IHaveActiveItem
    {
        private T activeItem;

        protected ConductorBaseWithActiveItem()
        {
        }

        protected virtual void ChangeActiveItem(T newItem, bool closePrevious)
        {
            ScreenExtensions.TryDeactivate(this.activeItem, closePrevious);
            newItem = this.EnsureItem(newItem);
            if (base.IsActive)
            {
                ScreenExtensions.TryActivate(newItem);
            }
            this.activeItem = newItem;
            this.NotifyOfPropertyChange("ActiveItem");
            this.OnActivationProcessed(this.activeItem, true);
        }

        public T ActiveItem
        {
            get
            {
                return this.activeItem;
            }
            set
            {
                this.ActivateItem(value);
            }
        }

        object IHaveActiveItem.ActiveItem
        {
            get
            {
                return this.ActiveItem;
            }
            set
            {
                this.ActiveItem = (T) value;
            }
        }
    }
}

