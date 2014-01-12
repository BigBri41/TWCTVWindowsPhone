namespace TWC.OVP.Views
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Shapes;
    using TWC.OVP.Framework.Controls;

    public class SportsNetworkShellView : UserControl
    {
        private bool _contentLoaded;
        internal ContentControl AssetViewer;
        internal Rectangle BackgroundRectangle;
        internal VisualStateGroup CaptionSettingsStates;
        internal ToggleButton ClosedCaptioning;
        internal Grid Controller;
        internal VisualStateGroup ControllerStates;
        internal VisualState Embedded;
        internal Button ExitFullScreenButton;
        internal Grid ExitFullScreenGrid;
        internal VisualState FullScreen;
        internal Button FullScreenButton;
        internal Grid FullScreenGrid;
        internal VisualState HideCaptionSettings;
        internal VisualState HideController;
        internal ContentControl Interaction;
        internal Grid LayoutRoot;
        internal VisualState ShowCaptionSettings;
        internal VisualState ShowController;
        internal VolumeControl volumeControl;
        internal VisualStateGroup WindowStates;

        public SportsNetworkShellView()
        {
            this.InitializeComponent();
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Application.LoadComponent(this, new Uri("/TWC.OVP;component/Views/Shell/SportsNetworkShellView.xaml", UriKind.Relative));
                this.LayoutRoot = (Grid) base.FindName("LayoutRoot");
                this.ControllerStates = (VisualStateGroup) base.FindName("ControllerStates");
                this.ShowController = (VisualState) base.FindName("ShowController");
                this.HideController = (VisualState) base.FindName("HideController");
                this.WindowStates = (VisualStateGroup) base.FindName("WindowStates");
                this.FullScreen = (VisualState) base.FindName("FullScreen");
                this.Embedded = (VisualState) base.FindName("Embedded");
                this.CaptionSettingsStates = (VisualStateGroup) base.FindName("CaptionSettingsStates");
                this.ShowCaptionSettings = (VisualState) base.FindName("ShowCaptionSettings");
                this.HideCaptionSettings = (VisualState) base.FindName("HideCaptionSettings");
                this.BackgroundRectangle = (Rectangle) base.FindName("BackgroundRectangle");
                this.AssetViewer = (ContentControl) base.FindName("AssetViewer");
                this.Controller = (Grid) base.FindName("Controller");
                this.volumeControl = (VolumeControl) base.FindName("volumeControl");
                this.ClosedCaptioning = (ToggleButton) base.FindName("ClosedCaptioning");
                this.FullScreenGrid = (Grid) base.FindName("FullScreenGrid");
                this.FullScreenButton = (Button) base.FindName("FullScreenButton");
                this.ExitFullScreenGrid = (Grid) base.FindName("ExitFullScreenGrid");
                this.ExitFullScreenButton = (Button) base.FindName("ExitFullScreenButton");
                this.Interaction = (ContentControl) base.FindName("Interaction");
            }
        }
    }
}

