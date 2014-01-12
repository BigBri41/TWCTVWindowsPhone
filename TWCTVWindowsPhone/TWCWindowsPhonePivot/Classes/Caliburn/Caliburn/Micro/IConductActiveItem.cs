namespace Caliburn.Micro
{
    using System.ComponentModel;

    public interface IConductActiveItem : IConductor, IParent, INotifyPropertyChangedEx, INotifyPropertyChanged, IHaveActiveItem
    {
    }
}

