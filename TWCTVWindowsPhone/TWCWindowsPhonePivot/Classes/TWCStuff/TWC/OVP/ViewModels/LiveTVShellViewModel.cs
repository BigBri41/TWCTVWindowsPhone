namespace TWC.OVP.ViewModels
{
    using Caliburn.Micro;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Threading;
    using TWC.OVP;
    using TWC.OVP.Messages;
    using TWC.OVP.Models;
    using TWC.OVP.Services;

    public class LiveTVShellViewModel : BaseShellViewModel
    {
        private ChannelBrowserViewModel _channelBrowser;
        private readonly DispatcherTimer _channelBrowserTimeoutTimer;
        private const double _channelThrottleThresholdSeconds = 1.0;
        private DispatcherTimer _channelThrottleTimer;
        private string _currentChannelBrowserState;
        private bool _isChannelBrowserOpen;
        private bool _isMouseOverChannelBrowser;
        private bool _isUserInteracting;
        private DateTime _lastChannelStartTime;
        private DateTime _lastEpisodeInfoRefreshTime;
        private DateTime _nextAirTime;
        private ChannelViewModel _pendingStartChannel;
        private bool _searchHasFocus;
        private ISettingsService _settingsService;
        public static readonly string HideChannelBrowserState = "HideChannelBrowser";
        public static readonly string LoadingState = "Loading";
        public static readonly string NotLoadingState = "NotLoading";
        public static readonly string ShowChannelBrowserState = "ShowChannelBrowser";

        public event RoutedEventHandler ChannelBrowserHiding;

        public event RoutedEventHandler ChannelBrowserShowing;

        public LiveTVShellViewModel(AssetViewerViewModel assetViewerViewModel, InteractionViewModel interactionViewModel, AssetInfoViewModel assetInfoViewModel, ChannelBrowserViewModel channelBrowserViewModel, CaptionSettingsViewModel captionSettingsViewModel, ISettingsService settingsService, IEventAggregator eventAggregator) : base(eventAggregator)
        {
            this._lastChannelStartTime = DateTime.MinValue;
            this._lastEpisodeInfoRefreshTime = DateTime.MinValue;
            this._nextAirTime = DateTime.MaxValue;
            this._currentChannelBrowserState = HideChannelBrowserState;
            this._settingsService = settingsService;
            base.AssetViewer = assetViewerViewModel;
            base.Interaction = interactionViewModel;
            this.ChannelBrowser = channelBrowserViewModel;
            base.CaptionSettings = captionSettingsViewModel;
            DispatcherTimer timer = new DispatcherTimer {
                Interval = new TimeSpan(0, 0, 0, 2)
            };
            this._channelBrowserTimeoutTimer = timer;
            this._channelBrowserTimeoutTimer.Tick += new EventHandler(this.BrowserTimeoutTimerTick);
            this._channelThrottleTimer = new DispatcherTimer();
            this._channelThrottleTimer.Interval = TimeSpan.FromSeconds(1.0);
            this._channelThrottleTimer.Tick += new EventHandler(this._channelThrottleTimer_Tick);
        }

        private void _channelThrottleTimer_Tick(object sender, EventArgs e)
        {
            this.StartChannel(this._pendingStartChannel);
            this._channelThrottleTimer.Stop();
        }

        private void BrowserTimeoutTimerTick(object sender, EventArgs e)
        {
            if (!this.IsMouseOverChannelBrowserOrSearchHasFocus && !this._isUserInteracting)
            {
                this.HideChannelBrowser();
            }
            else
            {
                this._isUserInteracting = false;
            }
        }

        private void ChannelBrowser_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;
            if (propertyName != null)
            {
                if (!(propertyName == "CurrentChannel"))
                {
                    if (!(propertyName == "FilteredChannels"))
                    {
                        if (propertyName == "SearchHasFocus")
                        {
                            this.SearchHasFocus = this.ChannelBrowser.SearchHasFocus;
                        }
                        return;
                    }
                }
                else
                {
                    this.CurrentChannel = this.ChannelBrowser.CurrentChannel;
                    return;
                }
                base.StartRefreshThread();
            }
        }

        private void FireChannelBrowserHiding()
        {
            if (this.ChannelBrowserHiding != null)
            {
                this.ChannelBrowserHiding(this, new RoutedEventArgs());
            }
        }

        private void FireChannelBrowserShowing()
        {
            if (this.ChannelBrowserShowing != null)
            {
                this.ChannelBrowserShowing(this, new RoutedEventArgs());
            }
        }

        protected override IEnumerable<string> GetCurrentStates()
        {
            yield return this.CurrentChannelBrowserState;
            yield return this.CurrentAssetInfoState;
            yield return this.CurrentCaptionSettingsState;
            yield return this.CurrentCaptionBubblePopupState;
        }

        private int GetDurationFromRunTime(string runTime)
        {
            if (runTime != null)
            {
                int num = Convert.ToInt32(runTime.Substring(2, 2));
                int num2 = Convert.ToInt32(runTime.Substring(5, 2));
                return ((num * 60) + num2);
            }
            return 0;
        }

        private string GetWayfarerCookie()
        {
            foreach (string str2 in HtmlPage.Document.Cookies.Split(new char[] { ';' }))
            {
                if (str2.Split(new char[] { '=' })[0].Trim() == "wayfarer_ns")
                {
                    return str2.Replace("wayfarer_ns=", "");
                }
            }
            return null;
        }

        public void HideChannelBrowser()
        {
            this.FireChannelBrowserHiding();
            this._channelBrowserTimeoutTimer.Stop();
            this.CurrentChannelBrowserState = HideChannelBrowserState;
        }

        public void NextChannel()
        {
            this.ChannelBrowser.NextChannel();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            this.ActivateItem(base.AssetViewer);
            this.ActivateItem(this.ChannelBrowser);
            this.ActivateItem(base.CaptionSettings);
        }

        public void PreviousChannel()
        {
            this.ChannelBrowser.PreviousChannel();
        }

        private string ProxifyUrl(string url)
        {
            return url;
        }

        private string ProxifyUrl(Uri uri)
        {
            return this.ProxifyUrl(uri.ToString());
        }

        private void RefreshEpisodeDetailsFromNmd()
        {
            AssetViewModel onNow = this.CurrentChannel.OnNow;
            if (onNow != null)
            {
                this._lastEpisodeInfoRefreshTime = DateTime.Now;
                Task.Factory.StartNew((System.Action) (() => LiveTVService.LoadEpisodeDetailsNMD(onNow.EpisodeID, episodeData => delegate {
                    onNow.Title = (from c in episodeData.title
                        where c.type == "full"
                        select c.value).FirstOrDefault<string>();
                    onNow.EpisodeName = (from c in episodeData.title
                        where c.type == "episode"
                        select c.value).FirstOrDefault<string>();
                    onNow.Description = (from c in episodeData.description
                        orderby Math.Abs((int) (c.valueSize - 250))
                        select c.value).FirstOrDefault<string>();
                    onNow.Duration = this.GetDurationFromRunTime(episodeData.runTime);
                    string[] strArray = (from c in episodeData.cast
                        where (c.role.ToLowerInvariant() == "actor") && !string.IsNullOrWhiteSpace(c.person.firstName + " " + c.person.lastName)
                        select c.person.firstName + " " + c.person.lastName).ToArray<string>();
                    onNow.Cast = string.Join(", ", strArray);
                    onNow.ShortCast = string.Join(", ", strArray.Take<string>(2));
                    onNow.Director = string.Join(", ", (IEnumerable<string>) (from c in episodeData.crew
                        where (c.role.ToLowerInvariant() == "director") && !string.IsNullOrWhiteSpace(c.person.firstName + " " + c.person.lastName)
                        select c.person.firstName + " " + c.person.lastName));
                    onNow.Rating = episodeData.rating;
                    if (Application.Current.Host.Source.Scheme == "https")
                    {
                        onNow.ImageUri = new Uri(string.Format("https://services.timewarnercable.com/imageserver/program/{0}?width=160&height=240&default=false", onNow.EpisodeID));
                    }
                    else
                    {
                        string wayfarerCookie = this.GetWayfarerCookie();
                        if (!string.IsNullOrWhiteSpace(wayfarerCookie))
                        {
                            onNow.ImageUri = new Uri(string.Format("http://services.timewarnercable.com/imageserver/program/{0}?width=160&height=240&default=false&wayfarer={1}", onNow.EpisodeID, wayfarerCookie));
                        }
                    }
                    ScriptBridge.PageConsoleLog(string.Format("End RefreshEpisodeDetailsFromNmd,onNow:{0} {1} {2} {3}", new object[] { onNow.Title, onNow.AirDate, onNow.Duration, onNow.EpisodeID }));
                }.OnUIThread(), null)));
            }
        }

        protected override void RefreshThreadTasks()
        {
            if (this.CurrentChannel != null)
            {
                Task.Factory.StartNew(delegate {
                    try
                    {
                        this.ChannelBrowser.RefreshWhatsOn();
                    }
                    catch (Exception)
                    {
                    }
                }).Wait();
                () => base.AssetViewer.NotifyBeacon().OnUIThread();
            }
        }

        public void RequestFocus()
        {
            base._eventAggregator.Publish(new FocusRequiredEventMessage());
        }

        public void ShowChannelBrowser()
        {
            if (this.CurrentChannelBrowserState != ShowChannelBrowserState)
            {
                this.FireChannelBrowserShowing();
                this.CurrentChannelBrowserState = ShowChannelBrowserState;
            }
        }

        public void ShowChannelBrowserWithTimeout()
        {
            this._channelBrowserTimeoutTimer.Start();
            this.ShowChannelBrowser();
        }

        private void StartChannel(ChannelViewModel channelViewModel)
        {
            this.RefreshEpisodeDetailsFromNmd();
            AssetViewModel onNow = channelViewModel.OnNow;
            if (onNow == null)
            {
                onNow = new AssetViewModel {
                    IsLive = true,
                    IsEncrypted = true,
                    Title = channelViewModel.OnNowEpisodeTitle
                };
            }
            onNow.ChannelTitle = channelViewModel.NetworkName;
            onNow.ThumbSource = channelViewModel.FirstLogo;
            onNow.StreamUrl = channelViewModel.SmoothStreamUrl;
            onNow.IsAvailableOutOfHome = channelViewModel.IsAvailableOutOfHome;
            base.AssetViewer.Asset = onNow;
            base.Interaction.DismissErrorMessage();
        }

        private void ThrottleChannel(ChannelViewModel channelViewModel)
        {
            this._channelThrottleTimer.Stop();
            TimeSpan span = (TimeSpan) (DateTime.Now - this._lastChannelStartTime);
            if (span.TotalSeconds > 1.0)
            {
                this.StartChannel(channelViewModel);
            }
            else
            {
                this._pendingStartChannel = channelViewModel;
                this._channelThrottleTimer.Start();
            }
            this._lastChannelStartTime = DateTime.Now;
        }

        public void ToggleChannelBrowserWithDelay()
        {
            if ((this.CurrentChannelBrowserState == HideChannelBrowserState) && base.IsControllerHide)
            {
                this.ShowChannelBrowserWithTimeout();
                base.IsControllerHide = false;
            }
            else
            {
                this.HideChannelBrowser();
                base.IsControllerHide = true;
            }
        }

        public ChannelBrowserViewModel ChannelBrowser
        {
            get
            {
                return this._channelBrowser;
            }
            set
            {
                if (this._channelBrowser != value)
                {
                    this._channelBrowser = value;
                    this.NotifyOfPropertyChange<ChannelBrowserViewModel>(System.Linq.Expressions.Expression.Lambda<System.Func<ChannelBrowserViewModel>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(LiveTVShellViewModel)), (MethodInfo) methodof(LiveTVShellViewModel.get_ChannelBrowser)), new ParameterExpression[0]));
                    this._channelBrowser.PropertyChanged += new PropertyChangedEventHandler(this.ChannelBrowser_PropertyChanged);
                }
            }
        }

        public ChannelViewModel CurrentChannel
        {
            get
            {
                return this.ChannelBrowser.CurrentChannel;
            }
            set
            {
                this._isUserInteracting = true;
                bool flag = false;
                if (base.IsAssetInfoPanelShown)
                {
                    base.IsAssetInfoPanelShown = false;
                    flag = true;
                }
                if (value != null)
                {
                    this.ThrottleChannel(value);
                    if (flag)
                    {
                        base.IsAssetInfoPanelShown = true;
                    }
                    this.NotifyOfPropertyChange<ChannelViewModel>(System.Linq.Expressions.Expression.Lambda<System.Func<ChannelViewModel>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(LiveTVShellViewModel)), (MethodInfo) methodof(LiveTVShellViewModel.get_CurrentChannel)), new ParameterExpression[0]));
                }
            }
        }

        public string CurrentChannelBrowserState
        {
            get
            {
                return this._currentChannelBrowserState;
            }
            set
            {
                if (this._currentChannelBrowserState != value)
                {
                    if (this._currentChannelBrowserState == HideChannelBrowserState)
                    {
                        this.ChannelBrowser.ClearSearch();
                    }
                    this.ChannelBrowser.IsOnNowShown = true;
                    this._currentChannelBrowserState = value;
                    if (this._currentChannelBrowserState == ShowChannelBrowserState)
                    {
                        this.IsChannelBrowserOpen = true;
                    }
                    else
                    {
                        this.IsChannelBrowserOpen = false;
                    }
                    this.NotifyOfPropertyChange<IEnumerable<string>>(System.Linq.Expressions.Expression.Lambda<System.Func<IEnumerable<string>>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(LiveTVShellViewModel)), (MethodInfo) methodof(BaseShellViewModel.get_CurrentVisualStates)), new ParameterExpression[0]));
                }
            }
        }

        public bool IsChannelBrowserOpen
        {
            get
            {
                return this._isChannelBrowserOpen;
            }
            set
            {
                this._isChannelBrowserOpen = value;
                this.NotifyOfPropertyChange<bool>(System.Linq.Expressions.Expression.Lambda<System.Func<bool>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(LiveTVShellViewModel)), (MethodInfo) methodof(LiveTVShellViewModel.get_IsChannelBrowserOpen)), new ParameterExpression[0]));
            }
        }

        public bool IsMouseOverChannelBrowser
        {
            get
            {
                return this._isMouseOverChannelBrowser;
            }
            set
            {
                this._isMouseOverChannelBrowser = value;
                if (this._isMouseOverChannelBrowser)
                {
                    this.CurrentChannelBrowserState = ShowChannelBrowserState;
                }
                this.NotifyOfPropertyChange<bool>(System.Linq.Expressions.Expression.Lambda<System.Func<bool>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(LiveTVShellViewModel)), (MethodInfo) methodof(LiveTVShellViewModel.get_IsMouseOverChannelBrowser)), new ParameterExpression[0]));
                this.NotifyOfPropertyChange<bool>(System.Linq.Expressions.Expression.Lambda<System.Func<bool>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(LiveTVShellViewModel)), (MethodInfo) methodof(LiveTVShellViewModel.get_IsMouseOverChannelBrowserOrSearchHasFocus)), new ParameterExpression[0]));
            }
        }

        public bool IsMouseOverChannelBrowserOrSearchHasFocus
        {
            get
            {
                if (!this.IsMouseOverChannelBrowser)
                {
                    return this.SearchHasFocus;
                }
                return true;
            }
        }

        public bool SearchHasFocus
        {
            get
            {
                return this._searchHasFocus;
            }
            set
            {
                this._searchHasFocus = value;
                this.NotifyOfPropertyChange<bool>(System.Linq.Expressions.Expression.Lambda<System.Func<bool>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(LiveTVShellViewModel)), (MethodInfo) methodof(LiveTVShellViewModel.get_SearchHasFocus)), new ParameterExpression[0]));
                this.NotifyOfPropertyChange<bool>(System.Linq.Expressions.Expression.Lambda<System.Func<bool>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(LiveTVShellViewModel)), (MethodInfo) methodof(LiveTVShellViewModel.get_IsMouseOverChannelBrowserOrSearchHasFocus)), new ParameterExpression[0]));
            }
        }

    }
}

