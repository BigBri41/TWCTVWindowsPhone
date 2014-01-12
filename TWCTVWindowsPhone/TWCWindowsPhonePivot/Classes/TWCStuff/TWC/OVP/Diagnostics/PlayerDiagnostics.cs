namespace TWC.OVP.Diagnostics
{
    using Microsoft.Web.Media.SmoothStreaming;
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Media;
    using TWC.OVP;
    using TWC.OVP.Controls;
    using TWC.OVP.Framework.Extensions;
    using TWC.OVP.Services;
    using TWC.OVP.ViewModels;
    using TWC.OVP.Views;

    public class PlayerDiagnostics
    {
        private AssetViewerViewModel _assetViewerViewModel;
        private BaseShellViewModel _baseShellViewModel;
        private BufferingTimeMonitor _bufferingTimeMonitor;
        private PlayerViewModel _playerViewModel;
        private ISettingsService _settingsService;
        private VideoPlayer _videoPlayer;
        private string captionTextChangedCallback;

        public PlayerDiagnostics(PlayerViewModel playerViewModel, AssetViewerViewModel assetViewerViewModel, BaseShellViewModel shellViewModel, ISettingsService settingsService, ScriptBridge scriptBridge)
        {
            EventHandler<ViewAttachedEventArgs> handler = null;
            this._playerViewModel = playerViewModel;
            this._assetViewerViewModel = assetViewerViewModel;
            this._baseShellViewModel = shellViewModel;
            this._settingsService = settingsService;
            if (handler == null)
            {
                handler = delegate (object s, ViewAttachedEventArgs e) {
                    this._bufferingTimeMonitor = new BufferingTimeMonitor(this._playerViewModel.Instance);
                    this._videoPlayer = this._playerViewModel.Instance;
                };
            }
            this._playerViewModel.ViewAttached += handler;
            this._playerViewModel.PropertyChanged += new PropertyChangedEventHandler(this._playerViewModel_PropertyChanged);
        }

        private void _playerViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "VisibleCaptionText")
            {
                this.CaptionTextChanged(this._playerViewModel.VisibleCaptionText);
            }
        }

        [ScriptableMember]
        public int BufferingEventCount()
        {
            return this.BufferingEventCount(0.0);
        }

        [ScriptableMember]
        public int BufferingEventCount(double seconds)
        {
            if (this._bufferingTimeMonitor == null)
            {
                return 0;
            }
            return this._bufferingTimeMonitor.GetBufferingEventCount(seconds);
        }

        [ScriptableMember]
        public double BufferingPercentage()
        {
            return this.BufferingPercentage(0.0);
        }

        [ScriptableMember]
        public double BufferingPercentage(double seconds)
        {
            if (this._bufferingTimeMonitor == null)
            {
                return 0.0;
            }
            return this._bufferingTimeMonitor.GetBufferPercentage(seconds);
        }

        private void CaptionTextChanged(string captionText)
        {
            if (this.CaptionTextChangedCallback.IsNotNullOrWhiteSpace())
            {
                captionText = captionText.Replace("'", @"\'");
                ScriptBridge.EvalJs(string.Format("{0}('{1}');", this.CaptionTextChangedCallback, captionText));
            }
        }

        private Size GetActualVideoDimensions()
        {
            Size size;
            if ((((this._playerViewModel.Instance == null) || (this._playerViewModel.Instance.SSME == null)) || ((this._playerViewModel.Instance.SSME.ActualHeight <= 0.0) || (this._playerViewModel.Instance.SSME.ActualWidth <= 0.0))) || ((this._playerViewModel.Instance.SSME.NaturalVideoHeight <= 0) || (this._playerViewModel.Instance.SSME.NaturalVideoWidth <= 0)))
            {
                return new Size(0.0, 0.0);
            }
            SmoothStreamingMediaElement sSME = this._playerViewModel.Instance.SSME;
            double num = ((double) sSME.NaturalVideoHeight) / ((double) sSME.NaturalVideoWidth);
            double actualHeight = sSME.ActualHeight;
            double width = sSME.ActualHeight / num;
            if (width <= sSME.ActualWidth)
            {
                size = new Size(width, actualHeight);
            }
            else
            {
                size = new Size(sSME.ActualWidth, sSME.ActualWidth * num);
            }
            size.Height = Math.Floor(size.Height);
            size.Width = Math.Floor(size.Width);
            return size;
        }

        private string GetAssetInfoState()
        {
            if (this._settingsService.AppMode == AppMode.Default)
            {
                return this.GetCurrentVisualState(this._baseShellViewModel.GetView(null) as OnDemandShellView, "AssetInfoStates");
            }
            if (this._settingsService.AppMode == AppMode.Live)
            {
                return this.GetCurrentVisualState(this._baseShellViewModel.GetView(null) as LiveTVShellView, "AssetInfoStates");
            }
            return null;
        }

        private string GetChannelBrowserState()
        {
            if (this._settingsService.AppMode == AppMode.Live)
            {
                return this.GetCurrentVisualState(this._baseShellViewModel.GetView(null) as LiveTVShellView, "ChannelBrowserStates");
            }
            return null;
        }

        private OnDemandController GetControllerBar()
        {
            OnDemandController controller = null;
            if (this._settingsService.AppMode == AppMode.Default)
            {
                OnDemandShellView view = this._baseShellViewModel.GetView(null) as OnDemandShellView;
                if (view != null)
                {
                    controller = view.controller;
                }
                return controller;
            }
            if (this._settingsService.AppMode == AppMode.Live)
            {
                LiveTVShellView view2 = this._baseShellViewModel.GetView(null) as LiveTVShellView;
                if (view2 != null)
                {
                    controller = view2.controller;
                }
            }
            return controller;
        }

        private string GetControllerBarState()
        {
            OnDemandController controllerBar = this.GetControllerBar();
            if (controllerBar == null)
            {
                return null;
            }
            return this.GetCurrentVisualState(controllerBar, "ControllerBarStates");
        }

        private string GetCurrentVisualState(FrameworkElement element, string visualStateGroupName)
        {
            VisualStateGroup group = Enumerable.FirstOrDefault<VisualStateGroup>(VisualStateManager.GetVisualStateGroups(VisualTreeHelper.GetChild(element, 0) as FrameworkElement).Cast<VisualStateGroup>(), g => g.Name == visualStateGroupName);
            if ((group != null) && (group.CurrentState != null))
            {
                return group.CurrentState.Name;
            }
            return null;
        }

        private string GetSportsNetworkControllerBarState()
        {
            SportsNetworkShellView element = this._baseShellViewModel.GetView(null) as SportsNetworkShellView;
            return this.GetCurrentVisualState(element, "ControllerStates");
        }

        [ScriptableMember]
        public void NextChannel()
        {
            LiveTVShellViewModel model = this._baseShellViewModel as LiveTVShellViewModel;
            if (model != null)
            {
                model.NextChannel();
            }
        }

        [ScriptableMember]
        public void PreviousChannel()
        {
            LiveTVShellViewModel model = this._baseShellViewModel as LiveTVShellViewModel;
            if (model != null)
            {
                model.PreviousChannel();
            }
        }

        [ScriptableMember]
        public void ToggleCaptioning()
        {
            if (this._playerViewModel.IsClosedCaptioningAvailable)
            {
                this._playerViewModel.IsClosedCaptioningEnabled = !this._playerViewModel.IsClosedCaptioningEnabled;
            }
        }

        [ScriptableMember]
        public void ToggleChannelBrowser()
        {
            LiveTVShellViewModel model = this._baseShellViewModel as LiveTVShellViewModel;
            if (model != null)
            {
                if (model.CurrentChannelBrowserState == LiveTVShellViewModel.HideChannelBrowserState)
                {
                    model.CurrentChannelBrowserState = LiveTVShellViewModel.ShowChannelBrowserState;
                }
                else
                {
                    model.CurrentChannelBrowserState = LiveTVShellViewModel.HideChannelBrowserState;
                }
            }
        }

        [ScriptableMember]
        public void ToggleControllerBar()
        {
            if (this._settingsService.AppMode == AppMode.SportsNetwork)
            {
                SportsNetworkShellViewModel model = this._baseShellViewModel as SportsNetworkShellViewModel;
                if (model.CurrentControllerVisualState == "ShowController")
                {
                    model.CurrentControllerVisualState = "HideController";
                }
                else
                {
                    model.CurrentControllerVisualState = "ShowController";
                }
            }
            else
            {
                OnDemandController controllerBar = this.GetControllerBar();
                if (controllerBar != null)
                {
                    if (this.GetControllerBarState() == "ShowControllerBar")
                    {
                        controllerBar.HideController();
                    }
                    else
                    {
                        controllerBar.ShowController();
                    }
                }
            }
        }

        [ScriptableMember]
        public void ToggleInfoPanel()
        {
            if (this._baseShellViewModel.CurrentAssetInfoState == BaseShellViewModel.HideAssetInfoState)
            {
                this._baseShellViewModel.CurrentAssetInfoState = BaseShellViewModel.ShowAssetInfoState;
            }
            else
            {
                this._baseShellViewModel.CurrentAssetInfoState = BaseShellViewModel.HideAssetInfoState;
            }
        }

        [ScriptableMember]
        public void ToggleMute()
        {
            if (this._playerViewModel.Instance != null)
            {
                this._playerViewModel.Instance.IsMuted = !this._playerViewModel.Instance.IsMuted;
            }
        }

        [ScriptableMember]
        public void TogglePlay()
        {
            if (this._settingsService.AppMode == AppMode.Default)
            {
                this._assetViewerViewModel.TogglePlay();
            }
        }

        [ScriptableMember]
        public string CaptionTextChangedCallback
        {
            get
            {
                return this.captionTextChangedCallback;
            }
            set
            {
                this.captionTextChangedCallback = value;
            }
        }

        [ScriptableMember]
        public long CurrentBitrate
        {
            get
            {
                if (this._videoPlayer == null)
                {
                    return 0L;
                }
                return this._videoPlayer.PlaybackBitrate;
            }
        }

        [ScriptableMember]
        public double Height
        {
            get
            {
                return this.GetActualVideoDimensions().Height;
            }
        }

        [ScriptableMember]
        public bool IsCaptioningEnabled
        {
            get
            {
                return (this._playerViewModel.Instance.SelectedCaptionStream != null);
            }
        }

        [ScriptableMember]
        public bool IsChannelBrowserVisible
        {
            get
            {
                return (this.GetChannelBrowserState() == "ShowChannelBrowser");
            }
        }

        [ScriptableMember]
        public bool IsControllerBarVisible
        {
            get
            {
                if (this._settingsService.AppMode == AppMode.SportsNetwork)
                {
                    return (this.GetSportsNetworkControllerBarState() == "ShowController");
                }
                return (this.GetControllerBarState() == "ShowControllerBar");
            }
        }

        [ScriptableMember]
        public bool IsFullScreen
        {
            get
            {
                return Application.Current.Host.Content.IsFullScreen;
            }
        }

        [ScriptableMember]
        public bool IsInfoPanelVisible
        {
            get
            {
                return (this.GetAssetInfoState() == "ShowAssetInfo");
            }
        }

        [ScriptableMember]
        public bool IsMuted
        {
            get
            {
                return ((this._playerViewModel.Instance != null) && this._playerViewModel.Instance.IsMuted);
            }
        }

        [ScriptableMember]
        public string PlayState
        {
            get
            {
                if (this._videoPlayer == null)
                {
                    return null;
                }
                return this._videoPlayer.PlayState.ToString();
            }
        }

        [ScriptableMember]
        public string StreamUrl
        {
            get
            {
                if (((this._videoPlayer != null) && (this._videoPlayer.CurrentPlaylistItem != null)) && (this._videoPlayer.CurrentPlaylistItem.MediaSource != null))
                {
                    return this._videoPlayer.CurrentPlaylistItem.MediaSource.ToString();
                }
                return null;
            }
        }

        [ScriptableMember]
        public double Volume
        {
            get
            {
                if (this._playerViewModel.Instance != null)
                {
                    return this._playerViewModel.Instance.VolumeLevel;
                }
                return 0.0;
            }
            set
            {
                if (this._playerViewModel.Instance != null)
                {
                    this._playerViewModel.Instance.VolumeLevel = Math.Min(Math.Max(value, 0.0), 1.0);
                }
            }
        }

        [ScriptableMember]
        public double Width
        {
            get
            {
                return this.GetActualVideoDimensions().Width;
            }
        }
    }
}

