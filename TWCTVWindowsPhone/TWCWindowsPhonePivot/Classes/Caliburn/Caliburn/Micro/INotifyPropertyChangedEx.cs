namespace Caliburn.Micro
{
    using System;
    using System.ComponentModel;

    public interface INotifyPropertyChangedEx : INotifyPropertyChanged
    {
        void NotifyOfPropertyChange(string propertyName);
        void Refresh();

        bool IsNotifying { get; set; }
    }
}

