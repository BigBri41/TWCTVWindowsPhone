namespace TWC.OVP.Views
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using TWC.OVP.Controls;
    using TWC.OVP.ViewModels;

    public class PlayerView : UserControl
    {
        private bool _contentLoaded;
        internal AesopVideoPlayer VideoPlayer;

        public PlayerView()
        {
            this.InitializeComponent();
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Application.LoadComponent(this, new Uri("/TWC.OVP;component/Views/PlayerView.xaml", UriKind.Relative));
                this.VideoPlayer = (AesopVideoPlayer) base.FindName("VideoPlayer");
            }
        }

        public PlayerViewModel DesignerContext
        {
            get
            {
                return null;
            }
        }
    }
}

