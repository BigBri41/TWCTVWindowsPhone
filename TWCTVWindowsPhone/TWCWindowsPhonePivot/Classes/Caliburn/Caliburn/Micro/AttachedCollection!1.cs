namespace Caliburn.Micro
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows;
    using System.Windows.Interactivity;

    public class AttachedCollection<T> : DependencyObjectCollection<T>, IAttachedObject where T: DependencyObject, IAttachedObject
    {
        private DependencyObject associatedObject;

        public AttachedCollection()
        {
            base.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
        }

        public void Attach(DependencyObject dependencyObject)
        {
            this.associatedObject = dependencyObject;
            this.Apply<T>(x => x.Attach(base.associatedObject));
        }

        public void Detach()
        {
            this.Apply<T>(x => x.Detach());
            this.associatedObject = null;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            System.Func<T, bool> func = null;
            System.Func<T, bool> func2 = null;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (func == null)
                    {
                        func = x => !base.Contains(x);
                    }
                    Enumerable.Where<T>(e.NewItems.OfType<T>(), func).Apply<T>(new Action<T>(this.OnItemAdded));
                    break;

                case NotifyCollectionChangedAction.Remove:
                    e.OldItems.OfType<T>().Apply<T>(new Action<T>(this.OnItemRemoved));
                    break;

                case NotifyCollectionChangedAction.Replace:
                    e.OldItems.OfType<T>().Apply<T>(new Action<T>(this.OnItemRemoved));
                    if (func2 == null)
                    {
                        func2 = x => !base.Contains(x);
                    }
                    Enumerable.Where<T>(e.NewItems.OfType<T>(), func2).Apply<T>(new Action<T>(this.OnItemAdded));
                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.Apply<T>(new Action<T>(this.OnItemRemoved));
                    this.Apply<T>(new Action<T>(this.OnItemAdded));
                    break;
            }
        }

        protected void OnItemAdded(T item)
        {
            if (this.associatedObject != null)
            {
                item.Attach(this.associatedObject);
            }
        }

        protected void OnItemRemoved(T item)
        {
            if (item.AssociatedObject != null)
            {
                item.Detach();
            }
        }

        DependencyObject IAttachedObject.AssociatedObject
        {
            get
            {
                return this.associatedObject;
            }
        }
    }
}

