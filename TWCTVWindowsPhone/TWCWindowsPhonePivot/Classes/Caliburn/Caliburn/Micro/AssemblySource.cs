namespace Caliburn.Micro
{
    using System;

    public static class AssemblySource
    {
        public static readonly IObservableCollection<Assembly> Instance = new BindableCollection<Assembly>();
    }
}

