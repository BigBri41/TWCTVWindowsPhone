namespace Caliburn.Micro
{
    using System;
    using System.ComponentModel;

    public interface IConductor : IParent, INotifyPropertyChangedEx, INotifyPropertyChanged
    {
        event EventHandler<ActivationProcessedEventArgs> ActivationProcessed;

        void ActivateItem(object item);
        void DeactivateItem(object item, bool close);
    }
}

