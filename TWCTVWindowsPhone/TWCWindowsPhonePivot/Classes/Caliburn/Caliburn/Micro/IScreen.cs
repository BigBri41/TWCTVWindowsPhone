namespace Caliburn.Micro
{
    using System.ComponentModel;

    public interface IScreen : IHaveDisplayName, IActivate, IDeactivate, IGuardClose, IClose, INotifyPropertyChangedEx, INotifyPropertyChanged
    {
    }
}

