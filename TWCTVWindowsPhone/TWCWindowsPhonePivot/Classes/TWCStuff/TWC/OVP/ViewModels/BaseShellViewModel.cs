namespace TWC.OVP.ViewModels
{
    using Caliburn.Micro;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;
    using TWC.OVP.Framework.ViewModels;
    using TWC.OVP.Messages;

    public abstract class BaseShellViewModel : BaseConductor
    {
        private AssetInfoViewModel _assetInfo;
        private DispatcherTimer _assetInfoTimer;
        private AssetViewerViewModel _assetViewer;
        private int _captionBubblePopupTimeout = 300;
        private CaptionSettingsViewModel _captionSettings;
        private string _currentAssetInfoState = HideAssetInfoState;
        private string _currentCaptionBubblePopupState;
        private string _currentCaptionSettingsState;
        private string _currentScreenState;
        protected EventAggregator _eventAggregator;
        private InteractionViewModel _Interaction;
        private bool _isAssetInfoPanelShown;
        private bool _isAssetInfoPopupShown;
        private bool _isControllerHide;
        private bool _isFullScreen;
        private bool _isMouseOverAssetInfoButton;
        private bool _isMouseOverAssetInfoPanel;
        private bool _isMouseOverAssetInfoPopup;
        private bool _isMouseOverCaptionBubble;
        private bool _isMouseOverCaptionButton;
        private bool _refreshRunning;
        public static readonly string EmbeddedWindowState = "Embedded";
        public static readonly string FullScreenWindowState = "FullScreen";
        public static readonly string HideAssetInfoState = "HideAssetInfo";
        public static readonly string HideCaptionSettingsState = "HideCaptionSettings";
        public static readonly string HideSettingsBubbleState = "HideSettingsBubble";
        private const int MillisecondsToPopupBubbleTimeout = 300;
        private const int SecondsToAssetInfoTimeout = 10;
        public static readonly string ShowAssetInfoState = "ShowAssetInfo";
        public static readonly string ShowCaptionSettingsState = "ShowCaptionSettings";
        public static readonly string ShowSettingsBubbleState = "ShowSettingsBubble";
        public static readonly string ShowSmallAssetInfoBubbleState = "ShowSmallAssetInfoPopupBubble";

        protected BaseShellViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator as EventAggregator;
            eventAggregator.Subscribe(this);
            Application.Current.Host.Content.FullScreenChanged += new EventHandler(this.ContentFullScreenChanged);
            base.PropertyChanged += new PropertyChangedEventHandler(this.BaseShellViewModelPropertyChanged);
        }

        private void BaseShellViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsAssetInfoPanelShown")
            {
                if (this.IsAssetInfoPanelShown)
                {
                    this.StopAssetInfoTimer();
                    this.StartAssetInfoTimer();
                }
                else
                {
                    this.StopAssetInfoTimer();
                }
            }
        }

        public void CollapseCaptionSettings()
        {
            this.CurrentCaptionSettingsState = HideCaptionSettingsState;
        }

        private void ContentFullScreenChanged(object sender, EventArgs e)
        {
            if (Application.Current.Host.Content.IsFullScreen)
            {
                this.IsFullScreen = true;
                this.CurrentScreenState = FullScreenWindowState;
            }
            else
            {
                this.IsFullScreen = false;
                this.CurrentScreenState = EmbeddedWindowState;
            }
        }

        private void EnsurePassageOfTime(DateTime start, TimeSpan duration)
        {
            TimeSpan span = (TimeSpan) (DateTime.Now - start);
            if (span < duration)
            {
                Thread.Sleep(duration - span);
            }
        }

        public void ExitFullScreen()
        {
            Application.Current.Host.Content.IsFullScreen = false;
            this._eventAggregator.Publish(new EGViewModeChangedEventMessage(Application.Current.Host.Content.IsFullScreen));
        }

        protected virtual IEnumerable<string> GetCurrentStates()
        {
            return null;
        }

        protected virtual void RefreshThreadTasks()
        {
        }

        private void StartAssetInfoTimer()
        {
            DispatcherTimer timer = new DispatcherTimer {
                Interval = new TimeSpan(0, 0, 0, 10)
            };
            this._assetInfoTimer = timer;
            this._assetInfoTimer.Tick += delegate (object o, EventArgs args) {
                if (this.IsMouseOverAssetInfoPanel || this.IsMouseOverAssetInfoButton)
                {
                    this.StopAssetInfoTimer();
                    this.StartAssetInfoTimer();
                }
                else
                {
                    this.IsAssetInfoPanelShown = false;
                    this.StopAssetInfoTimer();
                }
            };
            this._assetInfoTimer.Start();
        }

        protected void StartRefreshThread()
        {
            if (!this._refreshRunning)
            {
                new Thread(delegate {
                    this._refreshRunning = true;
                    this.EnsurePassageOfTime(DateTime.Now, TimeSpan.FromSeconds(60.0));
                    while (true)
                    {
                        DateTime start = DateTime.Now;
                        this.AssetViewer.CheckLicenseExpiration();
                        this.RefreshThreadTasks();
                        GC.GetTotalMemory(false);
                        this.EnsurePassageOfTime(start, TimeSpan.FromSeconds(60.0));
                    }
                }).Start();
                this._refreshRunning = true;
            }
        }

        private void StopAssetInfoTimer()
        {
            if (this._assetInfoTimer != null)
            {
                this._assetInfoTimer.Stop();
                this._assetInfoTimer = null;
            }
        }

        public void ToggleClosedCaptioningSettings()
        {
            if (this.CurrentCaptionSettingsState != ShowCaptionSettingsState)
            {
                if (this.IsAssetInfoPanelShown)
                {
                    this.IsAssetInfoPanelShown = false;
                }
                this.CurrentCaptionSettingsState = ShowCaptionSettingsState;
            }
            else
            {
                this.CurrentCaptionSettingsState = HideCaptionSettingsState;
            }
        }

        public void ToggleFullScreen()
        {
            Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;
            this._eventAggregator.Publish(new EGViewModeChangedEventMessage(Application.Current.Host.Content.IsFullScreen));
        }

        public void TogglePlay()
        {
            this.AssetViewer.TogglePlay();
        }

        public AssetInfoViewModel AssetInfo
        {
            get
            {
                return this._assetInfo;
            }
            set
            {
                if (this._assetInfo != value)
                {
                    this._assetInfo = value;
                    this.NotifyOfPropertyChange<AssetInfoViewModel>(System.Linq.Expressions.Expression.Lambda<System.Func<AssetInfoViewModel>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(BaseShellViewModel)), (MethodInfo) methodof(BaseShellViewModel.get_AssetInfo)), new ParameterExpression[0]));
                }
            }
        }

        public AssetViewerViewModel AssetViewer
        {
            get
            {
                return this._assetViewer;
            }
            set
            {
                if (this._assetViewer != value)
                {
                    this._assetViewer = value;
                    this.NotifyOfPropertyChange<AssetViewerViewModel>(System.Linq.Expressions.Expression.Lambda<System.Func<AssetViewerViewModel>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(BaseShellViewModel)), (MethodInfo) methodof(BaseShellViewModel.get_AssetViewer)), new ParameterExpression[0]));
                }
            }
        }

        public int BubblePopupTimeout
        {
            get
            {
                return 300;
            }
        }

        public int CaptionBubblePopupTimeout
        {
            get
            {
                return this._captionBubblePopupTimeout;
            }
            set
            {
                this._captionBubblePopupTimeout = value;
                this.NotifyOfPropertyChange<int>(System.Linq.Expressions.Expression.Lambda<System.Func<int>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(BaseShellViewModel)), (MethodInfo) methodof(BaseShellViewModel.get_CaptionBubblePopupTimeout)), new ParameterExpression[0]));
            }
        }

        public CaptionSettingsViewModel CaptionSettings
        {
            get
            {
                return this._captionSettings;
            }
            set
            {
                if (this._captionSettings != value)
                {
                    this._captionSettings = value;
                    this.NotifyOfPropertyChange<CaptionSettingsViewModel>(System.Linq.Expressions.Expression.Lambda<System.Func<CaptionSettingsViewModel>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(BaseShellViewModel)), (MethodInfo) methodof(BaseShellViewModel.get_CaptionSettings)), new ParameterExpression[0]));
                }
            }
        }

        public string CurrentAssetInfoState
        {
            get
            {
                return this._currentAssetInfoState;
            }
            set
            {
                if (this._currentAssetInfoState != value)
                {
                    this._currentAssetInfoState = value;
                    this.NotifyOfPropertyChange<IEnumerable<string>>(System.Linq.Expressions.Expression.Lambda<System.Func<IEnumerable<string>>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(BaseShellViewModel)), (MethodInfo) methodof(BaseShellViewModel.get_CurrentVisualStates)), new ParameterExpression[0]));
                }
            }
        }

        public string CurrentCaptionBubblePopupState
        {
            get
            {
                return this._currentCaptionBubblePopupState;
            }
            set
            {
                this._currentCaptionBubblePopupState = value;
                this.NotifyOfPropertyChange<IEnumerable<string>>(System.Linq.Expressions.Expression.Lambda<System.Func<IEnumerable<string>>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(BaseShellViewModel)), (MethodInfo) methodof(BaseShellViewModel.get_CurrentVisualStates)), new ParameterExpression[0]));
            }
        }

        public string CurrentCaptionSettingsState
        {
            get
            {
                return this._currentCaptionSettingsState;
            }
            set
            {
                this._currentCaptionSettingsState = value;
                this.NotifyOfPropertyChange<IEnumerable<string>>(System.Linq.Expressions.Expression.Lambda<System.Func<IEnumerable<string>>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(BaseShellViewModel)), (MethodInfo) methodof(BaseShellViewModel.get_CurrentVisualStates)), new ParameterExpression[0]));
            }
        }

        public string CurrentScreenState
        {
            get
            {
                return this._currentScreenState;
            }
            set
            {
                this._currentScreenState = value;
                this.NotifyOfPropertyChange<IEnumerable<string>>(System.Linq.Expressions.Expression.Lambda<System.Func<IEnumerable<string>>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(BaseShellViewModel)), (MethodInfo) methodof(BaseShellViewModel.get_CurrentVisualStates)), new ParameterExpression[0]));
            }
        }

        public virtual IEnumerable<string> CurrentVisualStates
        {
            get
            {
                return this.GetCurrentStates();
            }
        }

        public InteractionViewModel Interaction
        {
            get
            {
                return this._Interaction;
            }
            set
            {
                if (this._Interaction != value)
                {
                    this._Interaction = value;
                    this.NotifyOfPropertyChange<InteractionViewModel>(System.Linq.Expressions.Expression.Lambda<System.Func<InteractionViewModel>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(BaseShellViewModel)), (MethodInfo) methodof(BaseShellViewModel.get_Interaction)), new ParameterExpression[0]));
                }
            }
        }

        public bool IsAssetInfoPanelShown
        {
            get
            {
                return this._isAssetInfoPanelShown;
            }
            set
            {
                if (value != this._isAssetInfoPanelShown)
                {
                    this._isAssetInfoPanelShown = value;
                    if (value)
                    {
                        this.CollapseCaptionSettings();
                        this.CurrentAssetInfoState = ShowAssetInfoState;
                    }
                    else
                    {
                        this.CurrentAssetInfoState = HideAssetInfoState;
                        this.IsMouseOverAssetInfoPopup = false;
                    }
                    this._eventAggregator.Publish(new EGInfoOverlayToggledEventMessage(value));
                    this.NotifyOfPropertyChange<bool>(System.Linq.Expressions.Expression.Lambda<System.Func<bool>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(BaseShellViewModel)), (MethodInfo) methodof(BaseShellViewModel.get_IsAssetInfoPanelShown)), new ParameterExpression[0]));
                }
            }
        }

        public bool IsAssetInfoPopupShown
        {
            get
            {
                return this._isAssetInfoPopupShown;
            }
            set
            {
                if (!value || (this.CurrentAssetInfoState != ShowAssetInfoState))
                {
                    if (!value && (this.CurrentAssetInfoState != ShowAssetInfoState))
                    {
                        this.CurrentAssetInfoState = HideAssetInfoState;
                    }
                    else if (value)
                    {
                        this.CurrentAssetInfoState = ShowSmallAssetInfoBubbleState;
                    }
                    this._isAssetInfoPopupShown = value;
                    this.NotifyOfPropertyChange<bool>(System.Linq.Expressions.Expression.Lambda<System.Func<bool>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(BaseShellViewModel)), (MethodInfo) methodof(BaseShellViewModel.get_IsAssetInfoPopupShown)), new ParameterExpression[0]));
                }
            }
        }

        public bool IsControllerHide
        {
            get
            {
                return this._isControllerHide;
            }
            set
            {
                this._isControllerHide = value;
                this.NotifyOfPropertyChange<bool>(System.Linq.Expressions.Expression.Lambda<System.Func<bool>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(BaseShellViewModel)), (MethodInfo) methodof(BaseShellViewModel.get_IsControllerHide)), new ParameterExpression[0]));
            }
        }

        public bool IsFullScreen
        {
            get
            {
                return this._isFullScreen;
            }
            set
            {
                if (value != this._isFullScreen)
                {
                    this.IsAssetInfoPanelShown = false;
                }
                this._isFullScreen = value;
                this.NotifyOfPropertyChange<IEnumerable<string>>(System.Linq.Expressions.Expression.Lambda<System.Func<IEnumerable<string>>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(BaseShellViewModel)), (MethodInfo) methodof(BaseShellViewModel.get_CurrentVisualStates)), new ParameterExpression[0]));
            }
        }

        public bool IsMouseOverAssetInfoButton
        {
            get
            {
                return this._isMouseOverAssetInfoButton;
            }
            set
            {
                this._isMouseOverAssetInfoButton = value;
                this.NotifyOfPropertyChange<bool>(System.Linq.Expressions.Expression.Lambda<System.Func<bool>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(BaseShellViewModel)), (MethodInfo) methodof(BaseShellViewModel.get_IsMouseOverAssetInfoButton)), new ParameterExpression[0]));
                this.NotifyOfPropertyChange<bool>(System.Linq.Expressions.Expression.Lambda<System.Func<bool>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(BaseShellViewModel)), (MethodInfo) methodof(BaseShellViewModel.get_IsMouseOverAssetInfoControls)), new ParameterExpression[0]));
            }
        }

        public bool IsMouseOverAssetInfoControls
        {
            get
            {
                if (!this.IsMouseOverAssetInfoButton && !this.IsMouseOverAssetInfoPanel)
                {
                    return this.IsMouseOverAssetInfoPopup;
                }
                return true;
            }
        }

        public bool IsMouseOverAssetInfoPanel
        {
            get
            {
                return this._isMouseOverAssetInfoPanel;
            }
            set
            {
                this._isMouseOverAssetInfoPanel = value;
                this.NotifyOfPropertyChange<bool>(System.Linq.Expressions.Expression.Lambda<System.Func<bool>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(BaseShellViewModel)), (MethodInfo) methodof(BaseShellViewModel.get_IsMouseOverAssetInfoPanel)), new ParameterExpression[0]));
                this.NotifyOfPropertyChange<bool>(System.Linq.Expressions.Expression.Lambda<System.Func<bool>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(BaseShellViewModel)), (MethodInfo) methodof(BaseShellViewModel.get_IsMouseOverAssetInfoControls)), new ParameterExpression[0]));
            }
        }

        public bool IsMouseOverAssetInfoPopup
        {
            get
            {
                return this._isMouseOverAssetInfoPopup;
            }
            set
            {
                this._isMouseOverAssetInfoPopup = value;
                this.NotifyOfPropertyChange<bool>(System.Linq.Expressions.Expression.Lambda<System.Func<bool>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(BaseShellViewModel)), (MethodInfo) methodof(BaseShellViewModel.get_IsMouseOverAssetInfoPopup)), new ParameterExpression[0]));
                this.NotifyOfPropertyChange<bool>(System.Linq.Expressions.Expression.Lambda<System.Func<bool>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(BaseShellViewModel)), (MethodInfo) methodof(BaseShellViewModel.get_IsMouseOverAssetInfoControls)), new ParameterExpression[0]));
            }
        }

        public bool IsMouseOverCaptionBubble
        {
            get
            {
                return this._isMouseOverCaptionBubble;
            }
            set
            {
                this._isMouseOverCaptionBubble = value;
                this.NotifyOfPropertyChange<bool>(System.Linq.Expressions.Expression.Lambda<System.Func<bool>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(BaseShellViewModel)), (MethodInfo) methodof(BaseShellViewModel.get_IsMouseOverCaptionBubble)), new ParameterExpression[0]));
                this.NotifyOfPropertyChange<bool>(System.Linq.Expressions.Expression.Lambda<System.Func<bool>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(BaseShellViewModel)), (MethodInfo) methodof(BaseShellViewModel.get_IsMouseOverCaptionControls)), new ParameterExpression[0]));
            }
        }

        public bool IsMouseOverCaptionButton
        {
            get
            {
                return this._isMouseOverCaptionButton;
            }
            set
            {
                this._isMouseOverCaptionButton = value;
                this.NotifyOfPropertyChange<bool>(System.Linq.Expressions.Expression.Lambda<System.Func<bool>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(BaseShellViewModel)), (MethodInfo) methodof(BaseShellViewModel.get_IsMouseOverCaptionButton)), new ParameterExpression[0]));
                this.NotifyOfPropertyChange<bool>(System.Linq.Expressions.Expression.Lambda<System.Func<bool>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(BaseShellViewModel)), (MethodInfo) methodof(BaseShellViewModel.get_IsMouseOverCaptionControls)), new ParameterExpression[0]));
            }
        }

        public bool IsMouseOverCaptionControls
        {
            get
            {
                if (!this.IsMouseOverCaptionButton)
                {
                    return this.IsMouseOverCaptionBubble;
                }
                return true;
            }
        }
    }
}

