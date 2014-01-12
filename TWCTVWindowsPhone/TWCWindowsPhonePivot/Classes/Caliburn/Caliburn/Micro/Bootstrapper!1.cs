namespace Caliburn.Micro
{
    using System;
    using System.Windows;

    public class Bootstrapper<TRootModel> : Bootstrapper
    {
        public Bootstrapper() : base(true)
        {
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            Bootstrapper.DisplayRootViewFor(base.Application, typeof(TRootModel));
        }
    }
}

