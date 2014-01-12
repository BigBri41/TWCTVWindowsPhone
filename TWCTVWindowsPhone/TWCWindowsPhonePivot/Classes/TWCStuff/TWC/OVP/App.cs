namespace TWC.OVP
{
    using Caliburn.Micro;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using TWC.OVP.Framework;
    using TWC.OVP.Framework.Utilities;
    using TWC.OVP.Services;
    using TWC.OVP.ViewModels;

    public class App : Application
    {
        private bool _contentLoaded;
        private IEventAggregator _eventAggregator;

        public App()
        {
            this.InitializeComponent();
            base.UnhandledException += new EventHandler<ApplicationUnhandledExceptionEventArgs>(this.App_UnhandledException);
        }

        private void App_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            TWC.OVP.Framework.Utilities.Log.Fatal("Unhandled exception: " + e.ExceptionObject.ToString(), new object[0]);
            string playbackDurationText = "";
            try
            {
                playbackDurationText = this.PlayerViewModel.Instance.GetPlaybackDurationText();
            }
            catch (Exception)
            {
            }
            try
            {
                Trace.WriteLine("Unhandled exception: " + e.ExceptionObject.ToString() + Environment.NewLine + playbackDurationText);
            }
            catch (Exception)
            {
            }
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Application.LoadComponent(this, new Uri("/TWC.OVP;component/App.xaml", UriKind.Relative));
            }
        }

        public IEventAggregator EventAggregator
        {
            get
            {
                return this._eventAggregator;
            }
            set
            {
                this._eventAggregator = value;
                this._eventAggregator.Subscribe(this);
            }
        }

        public TWC.OVP.ViewModels.InteractionViewModel InteractionViewModel { get; set; }

        public bool IsAdEnabled { get; set; }

        public TWC.OVP.Services.LocationService LocationService { get; set; }

        public AppMode OVPApplicationMode { get; set; }

        public TWC.OVP.ViewModels.PlayerViewModel PlayerViewModel { get; set; }

        public BaseShellViewModel ShellViewModel { get; set; }
    }
}

