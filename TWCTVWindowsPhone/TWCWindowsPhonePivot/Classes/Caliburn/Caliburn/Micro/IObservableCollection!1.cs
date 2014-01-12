namespace Caliburn.Micro
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    public interface IObservableCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, INotifyPropertyChangedEx, INotifyPropertyChanged, INotifyCollectionChanged
    {
        void AddRange(IEnumerable<T> items);
        void RemoveRange(IEnumerable<T> items);
    }
}

