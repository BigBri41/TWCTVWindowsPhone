namespace Caliburn.Micro
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    public class BindableCollection<T> : ObservableCollection<T>, IObservableCollection<T>, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, INotifyPropertyChangedEx, INotifyPropertyChanged, INotifyCollectionChanged
    {
        private bool isNotifying;

        public BindableCollection()
        {
            this.IsNotifying = true;
        }

        public BindableCollection(IEnumerable<T> collection)
        {
            this.IsNotifying = true;
            this.AddRange(collection);
        }

        public virtual void AddRange(IEnumerable<T> items)
        {
            delegate {
                bool isNotifying = ((BindableCollection<T>) this).IsNotifying;
                ((BindableCollection<T>) this).IsNotifying = false;
                int index = ((BindableCollection<T>) this).Count;
                foreach (T local in items)
                {
                    ((BindableCollection<T>) this).InsertItemBase(index, local);
                    index++;
                }
                ((BindableCollection<T>) this).IsNotifying = isNotifying;
                ((BindableCollection<T>) this).OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                ((BindableCollection<T>) this).OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));
            }.OnUIThread();
        }

        protected sealed override void ClearItems()
        {
            new System.Action(this.ClearItemsBase).OnUIThread();
        }

        protected virtual void ClearItemsBase()
        {
            base.ClearItems();
        }

        protected sealed override void InsertItem(int index, T item)
        {
            () => ((BindableCollection<T>) this).InsertItemBase(index, item).OnUIThread();
        }

        protected virtual void InsertItemBase(int index, T item)
        {
            base.InsertItem(index, item);
        }

        public void NotifyOfPropertyChange(string propertyName)
        {
            if (this.IsNotifying)
            {
                () => ((BindableCollection<T>) this).RaisePropertyChangedEventImmediately(new PropertyChangedEventArgs(propertyName)).OnUIThread();
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.IsNotifying)
            {
                base.OnCollectionChanged(e);
            }
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext c)
        {
            this.IsNotifying = true;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.IsNotifying)
            {
                base.OnPropertyChanged(e);
            }
        }

        private void RaisePropertyChangedEventImmediately(PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(e);
        }

        public void Refresh()
        {
            delegate {
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                this.OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));
            }.OnUIThread();
        }

        protected sealed override void RemoveItem(int index)
        {
            () => ((BindableCollection<T>) this).RemoveItemBase(index).OnUIThread();
        }

        protected virtual void RemoveItemBase(int index)
        {
            base.RemoveItem(index);
        }

        public virtual void RemoveRange(IEnumerable<T> items)
        {
            delegate {
                bool isNotifying = ((BindableCollection<T>) this).IsNotifying;
                ((BindableCollection<T>) this).IsNotifying = false;
                foreach (T local in items)
                {
                    int index = ((BindableCollection<T>) this).IndexOf(local);
                    ((BindableCollection<T>) this).RemoveItemBase(index);
                }
                ((BindableCollection<T>) this).IsNotifying = isNotifying;
                ((BindableCollection<T>) this).OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                ((BindableCollection<T>) this).OnPropertyChanged(new PropertyChangedEventArgs(string.Empty));
            }.OnUIThread();
        }

        protected sealed override void SetItem(int index, T item)
        {
            () => ((BindableCollection<T>) this).SetItemBase(index, item).OnUIThread();
        }

        protected virtual void SetItemBase(int index, T item)
        {
            base.SetItem(index, item);
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

