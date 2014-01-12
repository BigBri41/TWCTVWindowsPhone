namespace TWC.OVP.Controls
{
    using Microsoft.SilverlightMediaFramework.Core;
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Shapes;
    using System.Windows.Threading;
    using TWC.OVP;
    using TWC.OVP.Framework.Controls;
    using TWC.OVP.Messages;
    using TWC.OVP.ViewModels;

    public class OnDemandController : UserControl
    {
        private bool _contentLoaded;
        private readonly DispatcherTimer _controllerTimeoutTimer;
        private bool _isUserInteracting;
        private bool _mouseOverHold;
        public static readonly DependencyProperty AssetViewerProperty = DependencyProperty.Register("AssetViewer", typeof(AssetViewerViewModel), typeof(OnDemandController), new PropertyMetadata(null));
        internal Grid BackgroundContainer;
        internal Grid BackgroundFullscreen;
        internal Grid BackgroundRectangle;
        internal Popup CaptionsNAPopup;
        internal TextBlock Cast;
        internal ToggleButton ClosedCaptioning;
        internal Rectangle ClosedCaptioningNotAvailableElement;
        internal ConstrainedTimeline constrainedTimeline;
        internal Grid ContentGrid;
        internal VisualStateGroup ControllerBarStates;
        private const int ControllerHideInterval = 4;
        public static readonly DependencyProperty CurrentPlaybackPositionProperty = DependencyProperty.Register("CurrentPlaybackPosition", typeof(TimeSpan), typeof(OnDemandController), new PropertyMetadata(null));
        internal VisualState Embedded;
        internal Grid EpisodeInfoPanel;
        internal TextBlock EpisodeName;
        internal Button ExitFullScreenButton;
        internal Grid ExitFullscreenGrid;
        internal VisualState FullScreen;
        internal Grid FullScreenGrid;
        internal VisualState HideControllerBar;
        private const string HideStateName = "HideControllerBar";
        internal ToggleButton InfoButton;
        public static readonly DependencyProperty IsAssetInfoOpenProperty = DependencyProperty.Register("IsAssetInfoOpen", typeof(bool), typeof(OnDemandController), new PropertyMetadata(false));
        public static readonly DependencyProperty IsChannelBrowserOpenProperty = DependencyProperty.Register("IsChannelBrowserOpen", typeof(bool), typeof(OnDemandController), new PropertyMetadata(false));
        public static readonly DependencyProperty IsEpisodicProperty = DependencyProperty.Register("IsEpisodic", typeof(bool), typeof(OnDemandController), new PropertyMetadata(false, new System.Windows.PropertyChangedCallback(OnDemandController.IsEpisodicChanged)));
        public static readonly DependencyProperty IsHideProperty = DependencyProperty.Register("IsHide", typeof(bool), typeof(OnDemandController), new PropertyMetadata(false, new System.Windows.PropertyChangedCallback(OnDemandController.IsHidePropertyChanged)));
        public static readonly DependencyProperty IsLiveProperty = DependencyProperty.Register("IsLive", typeof(bool), typeof(OnDemandController), new PropertyMetadata(false, new System.Windows.PropertyChangedCallback(OnDemandController.PropertyChangedCallback)));
        internal Grid LayoutRoot;
        internal VisualState Live;
        internal Image LiveLogoSeperator;
        internal Border LiveTopBorder;
        internal Grid MetaDataGrid;
        internal VisualStateGroup MetaDataStates;
        internal Button NormalFullScreenButton;
        internal VisualState OnDemand;
        internal Grid OnDemandLeftGrid;
        internal Image OnDemandNetworkLogo;
        internal Grid PlayElementsGrid;
        internal TextBlock ProgramTitle;
        internal Button ReplayElement;
        internal Grid RightGrid;
        internal TextBlock SeasonAndEpisodeLabel;
        internal Image SeperatorImage1;
        internal Image SeperatorImage2;
        internal Image SeperatorImage3;
        internal Image SeperatorImage5;
        internal Image SeperatorImage6;
        internal VisualStateGroup ShellStates;
        internal VisualState ShowControllerBar;
        internal VisualState ShowEpisodeInfo;
        internal VisualState ShowMovieInfo;
        private const string ShowStateName = "ShowControllerBar";
        internal UserControl userControl;
        internal TWC.OVP.Framework.Controls.VolumeControl VolumeButton;
        internal VisualStateGroup WindowStates;

        public event RoutedEventHandler ControllerHiding;

        public event RoutedEventHandler ControllerShowing;

        public OnDemandController()
        {
            this.InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer {
                Interval = new TimeSpan(0, 0, 0, 4)
            };
            this._controllerTimeoutTimer = timer;
            this._controllerTimeoutTimer.Tick += new EventHandler(this.ControllerTimeoutTimerTick);
            Application.Current.Host.Content.FullScreenChanged += new EventHandler(this.IsFullScreenChanged);
            this.constrainedTimeline.Scrubbing += new EventHandler<CustomEventArgs<TimeSpan>>(this.ConstrainedTimelineScrubbing);
            this.constrainedTimeline.ScrubbingCompleted += new EventHandler<CustomEventArgs<TimeSpan>>(this.ConstrainedTimelineScrubbingCompleted);
            this.constrainedTimeline.ScrubbingStarted += new EventHandler<CustomEventArgs<TimeSpan>>(this.constrainedTimeline_ScrubbingStarted);
            this.InfoButton.MouseEnter += new MouseEventHandler(this.ControllerButtonMouseEnter);
            this.InfoButton.MouseLeave += new MouseEventHandler(this.ControllerButtonMouseLeave);
            this.VolumeButton.MouseEnter += new MouseEventHandler(this.ControllerButtonMouseEnter);
            this.VolumeButton.MouseLeave += new MouseEventHandler(this.ControllerButtonMouseLeave);
            this.ClosedCaptioning.MouseEnter += new MouseEventHandler(this.ControllerButtonMouseEnter);
            this.ClosedCaptioning.MouseLeave += new MouseEventHandler(this.ControllerButtonMouseLeave);
            this.ExitFullScreenButton.MouseEnter += new MouseEventHandler(this.ControllerButtonMouseEnter);
            this.ExitFullScreenButton.MouseLeave += new MouseEventHandler(this.ControllerButtonMouseLeave);
            this.NormalFullScreenButton.MouseEnter += new MouseEventHandler(this.ControllerButtonMouseEnter);
            this.NormalFullScreenButton.MouseLeave += new MouseEventHandler(this.ControllerButtonMouseLeave);
            this.OnDemandLeftGrid.MouseLeftButtonDown += new MouseButtonEventHandler(this.OnDemandLeftGrid_MouseLeftButtonDown);
        }

        private void _HideController()
        {
            this._controllerTimeoutTimer.Stop();
            this._isUserInteracting = false;
            this.FireControllerHiding();
            VisualStateManager.GoToState(this, "HideControllerBar", true);
        }

        private void _ShowController()
        {
            this._isUserInteracting = true;
            this.FireControllerShowing();
            VisualStateManager.GoToState(this, "ShowControllerBar", true);
        }

        private void constrainedTimeline_ScrubbingStarted(object sender, CustomEventArgs<TimeSpan> e)
        {
        }

        private void ConstrainedTimelineScrubbing(object sender, CustomEventArgs<TimeSpan> e)
        {
        }

        private void ConstrainedTimelineScrubbingCompleted(object sender, CustomEventArgs<TimeSpan> e)
        {
            if ((this.AssetViewer != null) && (this.constrainedTimeline.IsFastForwardEnabled || (!this.constrainedTimeline.IsFastForwardEnabled && (e.Value.CompareTo(this.CurrentPlaybackPosition) < 0))))
            {
                ((App) Application.Current).EventAggregator.Publish(new UserInteractionMessage("ConstrainedTimelineScrubbing"));
                ((App) Application.Current).EventAggregator.Publish(new EGStreamScrubbedMessage(e.Value.TotalSeconds, false, "drag"));
                this.AssetViewer.Player.SeekToPosition(e.Value);
            }
        }

        private void ControllerButtonMouseEnter(object sender, MouseEventArgs e)
        {
            this._mouseOverHold = true;
        }

        private void ControllerButtonMouseLeave(object sender, MouseEventArgs e)
        {
            this._mouseOverHold = false;
        }

        private void ControllerTimeoutTimerTick(object sender, EventArgs e)
        {
            if ((this._isUserInteracting || this.VolumeButton.IsMouseOverPopup) || ((this._mouseOverHold || this.IsChannelBrowserOpen) || this.IsAssetInfoOpen))
            {
                this._isUserInteracting = false;
            }
            else
            {
                this.IsHide = true;
            }
        }

        private void FireControllerHiding()
        {
            if (this.ControllerHiding != null)
            {
                this.ControllerHiding(this, new RoutedEventArgs());
            }
        }

        private void FireControllerShowing()
        {
            if (this.ControllerShowing != null)
            {
                this.ControllerShowing(this, new RoutedEventArgs());
            }
        }

        public void HideController()
        {
            this.IsHide = true;
        }

        public void HideControllerWithDelay()
        {
            this.IsHide = true;
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Application.LoadComponent(this, new Uri("/TWC.OVP;component/Controls/OnDemandController.xaml", UriKind.Relative));
                this.userControl = (UserControl) base.FindName("userControl");
                this.LayoutRoot = (Grid) base.FindName("LayoutRoot");
                this.WindowStates = (VisualStateGroup) base.FindName("WindowStates");
                this.Embedded = (VisualState) base.FindName("Embedded");
                this.FullScreen = (VisualState) base.FindName("FullScreen");
                this.ControllerBarStates = (VisualStateGroup) base.FindName("ControllerBarStates");
                this.ShowControllerBar = (VisualState) base.FindName("ShowControllerBar");
                this.HideControllerBar = (VisualState) base.FindName("HideControllerBar");
                this.MetaDataStates = (VisualStateGroup) base.FindName("MetaDataStates");
                this.ShowMovieInfo = (VisualState) base.FindName("ShowMovieInfo");
                this.ShowEpisodeInfo = (VisualState) base.FindName("ShowEpisodeInfo");
                this.ShellStates = (VisualStateGroup) base.FindName("ShellStates");
                this.Live = (VisualState) base.FindName("Live");
                this.OnDemand = (VisualState) base.FindName("OnDemand");
                this.ContentGrid = (Grid) base.FindName("ContentGrid");
                this.LiveTopBorder = (Border) base.FindName("LiveTopBorder");
                this.BackgroundContainer = (Grid) base.FindName("BackgroundContainer");
                this.BackgroundRectangle = (Grid) base.FindName("BackgroundRectangle");
                this.BackgroundFullscreen = (Grid) base.FindName("BackgroundFullscreen");
                this.OnDemandLeftGrid = (Grid) base.FindName("OnDemandLeftGrid");
                this.PlayElementsGrid = (Grid) base.FindName("PlayElementsGrid");
                this.ReplayElement = (Button) base.FindName("ReplayElement");
                this.SeperatorImage1 = (Image) base.FindName("SeperatorImage1");
                this.SeperatorImage2 = (Image) base.FindName("SeperatorImage2");
                this.OnDemandNetworkLogo = (Image) base.FindName("OnDemandNetworkLogo");
                this.LiveLogoSeperator = (Image) base.FindName("LiveLogoSeperator");
                this.MetaDataGrid = (Grid) base.FindName("MetaDataGrid");
                this.ProgramTitle = (TextBlock) base.FindName("ProgramTitle");
                this.Cast = (TextBlock) base.FindName("Cast");
                this.EpisodeInfoPanel = (Grid) base.FindName("EpisodeInfoPanel");
                this.EpisodeName = (TextBlock) base.FindName("EpisodeName");
                this.SeasonAndEpisodeLabel = (TextBlock) base.FindName("SeasonAndEpisodeLabel");
                this.RightGrid = (Grid) base.FindName("RightGrid");
                this.VolumeButton = (TWC.OVP.Framework.Controls.VolumeControl) base.FindName("VolumeButton");
                this.SeperatorImage3 = (Image) base.FindName("SeperatorImage3");
                this.InfoButton = (ToggleButton) base.FindName("InfoButton");
                this.SeperatorImage5 = (Image) base.FindName("SeperatorImage5");
                this.FullScreenGrid = (Grid) base.FindName("FullScreenGrid");
                this.NormalFullScreenButton = (Button) base.FindName("NormalFullScreenButton");
                this.ExitFullscreenGrid = (Grid) base.FindName("ExitFullscreenGrid");
                this.ExitFullScreenButton = (Button) base.FindName("ExitFullScreenButton");
                this.SeperatorImage6 = (Image) base.FindName("SeperatorImage6");
                this.ClosedCaptioning = (ToggleButton) base.FindName("ClosedCaptioning");
                this.ClosedCaptioningNotAvailableElement = (Rectangle) base.FindName("ClosedCaptioningNotAvailableElement");
                this.constrainedTimeline = (ConstrainedTimeline) base.FindName("constrainedTimeline");
                this.CaptionsNAPopup = (Popup) base.FindName("CaptionsNAPopup");
            }
        }

        private static void IsEpisodicChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnDemandController control = d as OnDemandController;
            if ((bool) e.NewValue)
            {
                VisualStateManager.GoToState(control, "ShowEpisodeInfo", true);
            }
            else
            {
                VisualStateManager.GoToState(control, "ShowMovieInfo", true);
            }
        }

        private void IsFullScreenChanged(object sender, EventArgs e)
        {
            if (Application.Current.Host.Content.IsFullScreen)
            {
                VisualStateManager.GoToState(this, "FullScreen", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Embedded", true);
            }
            this.ShowControllerWithTimeout();
        }

        private static void IsHidePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnDemandController controller = d as OnDemandController;
            if ((bool) e.NewValue)
            {
                controller._HideController();
            }
            else
            {
                controller._controllerTimeoutTimer.Start();
                controller._ShowController();
            }
        }

        private void OnDemandLeftGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            OnDemandController control = dependencyObject as OnDemandController;
            if ((bool) args.NewValue)
            {
                VisualStateManager.GoToState(control, "Live", true);
            }
            else
            {
                VisualStateManager.GoToState(control, "OnDemand", true);
            }
        }

        public void ShowController()
        {
            this.IsHide = false;
        }

        public void ShowControllerWithTimeout()
        {
            this.IsHide = false;
        }

        public void ToggleHide()
        {
            this.IsHide = !this.IsHide;
        }

        public AssetViewerViewModel AssetViewer
        {
            get
            {
                return (AssetViewerViewModel) base.GetValue(AssetViewerProperty);
            }
            set
            {
                base.SetValue(AssetViewerProperty, value);
            }
        }

        public TimeSpan CurrentPlaybackPosition
        {
            get
            {
                return (TimeSpan) base.GetValue(CurrentPlaybackPositionProperty);
            }
            set
            {
                base.SetValue(CurrentPlaybackPositionProperty, value);
            }
        }

        public bool IsAssetInfoOpen
        {
            get
            {
                return (bool) base.GetValue(IsAssetInfoOpenProperty);
            }
            set
            {
                base.SetValue(IsAssetInfoOpenProperty, value);
            }
        }

        public bool IsChannelBrowserOpen
        {
            get
            {
                return (bool) base.GetValue(IsChannelBrowserOpenProperty);
            }
            set
            {
                base.SetValue(IsChannelBrowserOpenProperty, value);
            }
        }

        public bool IsEpisodic
        {
            get
            {
                return (bool) base.GetValue(IsEpisodicProperty);
            }
            set
            {
                base.SetValue(IsEpisodicProperty, value);
            }
        }

        public bool IsHide
        {
            get
            {
                return (bool) base.GetValue(IsHideProperty);
            }
            set
            {
                base.SetValue(IsHideProperty, value);
            }
        }

        public bool IsLive
        {
            get
            {
                return (bool) base.GetValue(IsLiveProperty);
            }
            set
            {
                base.SetValue(IsLiveProperty, value);
            }
        }
    }
}

