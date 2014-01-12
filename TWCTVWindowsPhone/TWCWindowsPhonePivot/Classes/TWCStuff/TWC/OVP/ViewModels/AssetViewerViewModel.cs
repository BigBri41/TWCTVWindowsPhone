namespace TWC.OVP.ViewModels
{
    using Caliburn.Micro;
    using Microsoft.SilverlightMediaFramework.Core;
    using Microsoft.SilverlightMediaFramework.Plugins.Primitives;
    using Microsoft.SilverlightMediaFramework.Utilities.Metadata;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Browser;
    using System.Windows.Threading;
    using TWC.OVP;
    using TWC.OVP.Controls;
    using TWC.OVP.Decryption;
    using TWC.OVP.Framework;
    using TWC.OVP.Framework.Extensions;
    using TWC.OVP.Framework.ViewModels;
    using TWC.OVP.Messages;
    using TWC.OVP.Models;
    using TWC.OVP.Services;

    public class AssetViewerViewModel : BaseViewModel, IHandle<StartOnDemandStreamMessage>, IHandle<TogglePlayMessage>, IHandle<ErrorMessage>, IHandle<OutOfHomeStatusChangedMessage>, IHandle
    {
        private DateTime _AegisTokenObtainedTime;
        private int _AegisTokenRefreshSeconds;
        private DispatcherTimer _AegisTokenRefreshTimer;
        private AssetViewModel _asset;
        private CancellationTokenSource _cancellationTokenSource;
        private int _controllerRow = 2;
        private string _currentControllerState = VisualStates.ShowControllerState;
        private string _currentWindowState;
        private IEventAggregator _eventAggregator;
        private static int[] _InitialRetryIntervals = new int[] { 
            10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 30, 30, 30, 30, 
            30, 30, 60, 60, 60, 60, 60
         };
        private bool _isBuffering;
        private bool _IsCurrentStreamStarted;
        private bool _isFullScreen;
        private bool _IsLoading;
        private bool _isMuted;
        private bool _IsOutOfHomeDebug;
        private bool _IsRestarting;
        private bool _IsRetrying;
        private bool _IsWaitingToRestart;
        private TimeSpan _lastPostion = TimeSpan.Zero;
        private DateTime _lastRetryTime = DateTime.Now;
        private DateTime _LiveAssetStartTime;
        private LocationService _locationService;
        private PlayerViewModel _player;
        private int _RetryCount;
        private DispatcherTimer _RetryTimer;
        private InteractionViewModel Interaction;
        private AssetViewModel lastPlayingAsset;
        private TimeSpan pendingTimeSpan = TimeSpan.Zero;

        public AssetViewerViewModel(PlayerViewModel playerViewModel, IEventAggregator eventAggregator, LocationService locationService)
        {
            this._eventAggregator = eventAggregator;
            this._locationService = locationService;
            this.Player = playerViewModel;
            this._eventAggregator.Subscribe(this);
            Application.Current.Host.Content.FullScreenChanged += new EventHandler(this.FullScreenChanged);
            this._RetryTimer = new DispatcherTimer();
            this._RetryTimer.Interval = TimeSpan.FromSeconds(1.0);
            this._RetryTimer.Tick += new EventHandler(this._RetryTimer_Tick);
            this._AegisTokenRefreshTimer = new DispatcherTimer();
            this._AegisTokenRefreshTimer.Tick += new EventHandler(this.AegisTokenRefreshTimer_Tick);
        }

        private void _RetryTimer_Tick(object sender, EventArgs e)
        {
            int num = 600;
            if (this._RetryCount < _InitialRetryIntervals.Length)
            {
                num = _InitialRetryIntervals[this._RetryCount];
            }
            TimeSpan span = (TimeSpan) (DateTime.Now - this._lastRetryTime);
            if (span.TotalSeconds >= num)
            {
                ScriptBridge.PageConsoleLog(string.Format("Retry count:{0}, interval {1}", this._RetryCount, num));
                this._lastRetryTime = DateTime.Now;
                this._RetryTimer.Stop();
                if (this.Asset != null)
                {
                    this._RetryCount++;
                    this.RestartStream();
                }
            }
        }

        private string AddManifestIfNecessary(string url)
        {
            if (!url.ToLowerInvariant().Contains("/manifest"))
            {
                if (url.Contains(".isml"))
                {
                    url = url.Replace(".isml", ".isml/Manifest");
                    return url;
                }
                url = url.Replace(".ism", ".ism/Manifest");
            }
            return url;
        }

        private void AegisTokenRefreshTimer_Tick(object sender, EventArgs e)
        {
            this._AegisTokenRefreshTimer.Stop();
            this.RefreshAegisToken();
        }

        private MetadataCollection BuildCustomMetadata()
        {
            MetadataCollection metadatas = new MetadataCollection();
            if (this.Asset.IsLive)
            {
                metadatas.Add("FreeWheelGenericPlugin.Context", this.Asset.NetworkID);
                return metadatas;
            }
            string str = this.Asset.MediaPath.Split(new char[] { '/' }).Last<string>();
            if (!string.IsNullOrEmpty(str))
            {
                metadatas.Add("FreeWheelGenericPlugin.Context", str);
            }
            return metadatas;
        }

        public void ButtonTest()
        {
            this.RestartStream();
        }

        public void CheckLicenseExpiration()
        {
            if (((this.Asset != null) && this.Asset.IsLive) && this.Asset.LicenseExpirationTime.HasValue)
            {
                DateTime? licenseExpirationTime = this.Asset.LicenseExpirationTime;
                DateTime time2 = DateTime.Now.AddMinutes(1.0);
                if (licenseExpirationTime.HasValue ? (licenseExpirationTime.GetValueOrDefault() < time2) : false)
                {
                    Trace.WriteLine("vxtoken expired. restarting stream");
                    this.RestartStream();
                }
            }
        }

        private string CleanupImageUri(Uri imageUri)
        {
            string str = imageUri.ToString();
            if (imageUri.Scheme != "http")
            {
                str = str.Replace("https://", "http://");
            }
            int index = str.IndexOf("&wayfarer=");
            if (index > -1)
            {
                str = str.Substring(0, index);
            }
            return str;
        }

        public void FullScreenChanged(object sender, EventArgs e)
        {
            this.SetScreenState();
        }

        protected override IEnumerable<string> GetCurrentStates()
        {
            yield return this.CurrentControllerState;
            yield return this.CurrentWindowState;
        }

        private string GetVXToken()
        {
            foreach (string str2 in HtmlPage.Document.Cookies.Split(new char[] { ';' }))
            {
                if (str2.Split(new char[] { '=' })[0].Trim() == "vxtoken")
                {
                    return str2.Replace("vxtoken=", "").Trim();
                }
            }
            return null;
        }

        private DateTime? GetVXTokenExpirationTime()
        {
            string vXToken = this.GetVXToken();
            if (string.IsNullOrWhiteSpace(vXToken))
            {
                return null;
            }
            byte[] bytes = Convert.FromBase64String(vXToken);
            foreach (string str3 in Encoding.UTF8.GetString(bytes, 0, bytes.Length).Split(new char[] { '&' }))
            {
                string[] strArray2 = str3.Split(new char[] { '=' });
                if (strArray2[0] == "expiry")
                {
                    int num = int.Parse(strArray2[1]);
                    DateTime time2 = new DateTime(0x7b2, 1, 1, 0, 0, 0, 0);
                    return new DateTime?(time2.AddSeconds((double) num).ToLocalTime());
                }
            }
            return new DateTime?(DateTime.MinValue);
        }

        private string GetWayfarerCookie()
        {
            foreach (string str2 in HtmlPage.Document.Cookies.Split(new char[] { ';' }))
            {
                if (str2.Split(new char[] { '=' })[0].Trim() == "wayfarer_ns")
                {
                    return str2.Replace("wayfarer_ns=", "").Trim();
                }
            }
            return null;
        }

        public void Handle(ErrorMessage message)
        {
            if ((message.MessageType == ErrorMessageType.PlayerbackError) || (message.MessageType == ErrorMessageType.Unhandled))
            {
                this._eventAggregator.Publish(new EGStreamFailedEventMessage("connectivity", ""));
                this._RetryTimer.Start();
                this._RetryTimer_Tick(null, null);
            }
        }

        public void Handle(OutOfHomeStatusChangedMessage message)
        {
            System.Action action = null;
            System.Action action2 = null;
            System.Action action3 = null;
            ((App) Application.Current).ShellViewModel.ExitFullScreen();
            () => this._eventAggregator.Publish(new LocationChangedEventMessage(message.IsOutOfHome, message.IsOutOfCountry, this.Asset.IsAvailableOutOfHome)).OnUIThread();
            if (message.IsOutOfCountry)
            {
                this._eventAggregator.Publish(new ErrorMessage(MessageText.TitleNotAvailable, MessageText.OOCNotAvailable, "", ErrorMessageType.Message, 0x3e8, null, true));
            }
            else if ((this.Asset != null) && !this.Asset.IsLive)
            {
                if (message.IsOutOfHome)
                {
                    if (this.Asset.IsAvailableOutOfHome)
                    {
                        ErrorMessage message2 = new ErrorMessage(MessageText.TitleOOH, MessageText.IHtoOOHVOD, "", ErrorMessageType.Message, 0x2710, null, true);
                        if (action == null)
                        {
                            action = delegate {
                                this._IsWaitingToRestart = true;
                                this._eventAggregator.Publish(new LocationChangeAcknowledgedEventMessage(message.IsOutOfHome, message.IsOutOfCountry, this.Asset.IsAvailableOutOfHome, false));
                            };
                        }
                        message2.MessageDismissed += action;
                        this._eventAggregator.Publish(message2);
                    }
                    else
                    {
                        ErrorMessage message3 = new ErrorMessage(MessageText.TitleNotAvailable, MessageText.NotAvailableOOH, "", ErrorMessageType.Message, 0x3e8, null, true);
                        if (action2 == null)
                        {
                            action2 = () => this._eventAggregator.Publish(new LocationChangeAcknowledgedEventMessage(message.IsOutOfHome, message.IsOutOfCountry, this.Asset.IsAvailableOutOfHome, false));
                        }
                        message3.MessageDismissed += action2;
                        this._eventAggregator.Publish(message3);
                    }
                }
                else
                {
                    ErrorMessage message4 = new ErrorMessage(MessageText.TitleIH, MessageText.OOHtoIHVOD, "", ErrorMessageType.Message, 0x2710, null, true);
                    if (action3 == null)
                    {
                        action3 = delegate {
                            this._IsWaitingToRestart = true;
                            this._eventAggregator.Publish(new LocationChangeAcknowledgedEventMessage(message.IsOutOfHome, message.IsOutOfCountry, this.Asset.IsAvailableOutOfHome, false));
                        };
                    }
                    message4.MessageDismissed += action3;
                    this._eventAggregator.Publish(message4);
                }
            }
        }

        public void Handle(StartOnDemandStreamMessage message)
        {
            AssetInfo assetInfo = message.AssetJson.DeserializeFromJson<AssetInfo>();
            this.StartOnDemandStream(assetInfo, message.StartTime);
        }

        public void Handle(TogglePlayMessage message)
        {
            this.TogglePlay();
        }

        private void Instance_MediaEnded(object sender, EventArgs e)
        {
            this._eventAggregator.Publish(new EGPlaybackStoppedEventMessage("streamEnd", this.Player.Instance.PlaybackPosition.TotalSeconds));
        }

        private void Instance_MediaOpened(object sender, EventArgs e)
        {
            if (!this.Asset.IsLive && (this.Asset.Duration == 0))
            {
                this.Asset.Duration = Convert.ToInt32(this.Player.Instance.EndPosition.TotalMinutes);
            }
        }

        private void Instance_PlaybackBitrateChanged(object sender, CustomEventArgs<long> e)
        {
            this._eventAggregator.Publish(new EGBitRateChangedEventMessage(this.Player.Instance.PlaybackBitrate));
        }

        private void Instance_PlaybackPositionChanged(object sender, CustomEventArgs<TimeSpan> e)
        {
            if (e.Value > TimeSpan.Zero)
            {
                this._lastPostion = e.Value;
            }
        }

        private void interactionViewModel_ErrorStateChanged(object sender, EventArgs e)
        {
            this.NotifyOfPropertyChange<bool>(System.Linq.Expressions.Expression.Lambda<System.Func<bool>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(AssetViewerViewModel)), (MethodInfo) methodof(AssetViewerViewModel.get_IsLoading)), new ParameterExpression[0]));
        }

        public void NotifyBeacon()
        {
            MediaPluginState[] stateArray2 = new MediaPluginState[2];
            stateArray2[1] = MediaPluginState.Stopped;
            MediaPluginState[] source = stateArray2;
            if (!source.Contains<MediaPluginState>(this.Player.Instance.PlayState) && (this.Asset != null))
            {
                object obj2 = !this.Asset.AirDate.HasValue ? null : ((object) this.Asset.AirDate.Value.ToUniversalTime());
                this._eventAggregator.Publish(new RSNPlayerBeaconEventMessage(new object[] { this.Player.Instance.PlaybackPosition.TotalSeconds, this.Asset.EpisodeID, this.Asset.Title, obj2, this.Asset.NetworkID, this.Asset.ServiceID, this.Asset.ChannelTitle }));
            }
        }

        private void NotifyStreamStarted(AssetViewModel assetVM)
        {
            object obj2 = !assetVM.AirDate.HasValue ? null : ((object) assetVM.AirDate.Value);
            this._eventAggregator.Publish(new RSNStreamStartedEventMessage(new object[] { 0, assetVM.EpisodeID, assetVM.Title, obj2, assetVM.NetworkID, assetVM.ServiceID, assetVM.ChannelTitle }));
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            this.Interaction = ((App) Application.Current).InteractionViewModel;
            this.Interaction.ErrorStateChanged += new EventHandler(this.interactionViewModel_ErrorStateChanged);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            this.Player.Instance.PlayStateChanged += new EventHandler<CustomEventArgs<MediaPluginState>>(this.PlayerInstancePlayStateChanged);
            this.Player.Instance.PlaybackPositionChanged += new EventHandler<CustomEventArgs<TimeSpan>>(this.Instance_PlaybackPositionChanged);
            this.Player.Instance.MediaOpened += new EventHandler(this.Instance_MediaOpened);
            this.Player.Instance.Playlist = new ObservableCollection<PlaylistItem>();
            this.Player.Instance.MediaEnded += new EventHandler(this.Instance_MediaEnded);
            this.Player.Instance.PlaybackBitrateChanged += new EventHandler<CustomEventArgs<long>>(this.Instance_PlaybackBitrateChanged);
        }

        private void PlayerInstancePlayStateChanged(object sender, CustomEventArgs<MediaPluginState> e)
        {
            this.IsLoading = false;
            if (((MediaPluginState) e.Value) == MediaPluginState.Buffering)
            {
                if (!this._isBuffering)
                {
                    this._isBuffering = true;
                    if (this._IsCurrentStreamStarted)
                    {
                        if (this.Asset.IsLive)
                        {
                            TimeSpan span = (TimeSpan) (DateTime.Now - this._LiveAssetStartTime);
                            this._eventAggregator.Publish(new EGBufferingStartedEventMessage(span.TotalSeconds));
                        }
                        else
                        {
                            this._eventAggregator.Publish(new EGBufferingStartedEventMessage(this.Player.Instance.PlaybackPosition.TotalSeconds));
                        }
                    }
                }
            }
            else if (this._isBuffering)
            {
                this._isBuffering = false;
                if (this._IsCurrentStreamStarted)
                {
                    if (this.Asset.IsLive)
                    {
                        TimeSpan span3 = (TimeSpan) (DateTime.Now - this._LiveAssetStartTime);
                        this._eventAggregator.Publish(new EGBufferingEndedEventMessage(span3.TotalSeconds));
                    }
                    else
                    {
                        this._eventAggregator.Publish(new EGBufferingEndedEventMessage(this.Player.Instance.PlaybackPosition.TotalSeconds));
                    }
                }
                if ((!this.Asset.IsLive && (this.pendingTimeSpan > TimeSpan.Zero)) && (this.Player.Instance.PlaybackPosition < this.pendingTimeSpan))
                {
                    this.Player.Instance.SeekToPosition(this.pendingTimeSpan);
                    this.pendingTimeSpan = TimeSpan.Zero;
                }
            }
            if (((MediaPluginState) e.Value) == MediaPluginState.Playing)
            {
                this._IsRetrying = false;
                this._RetryCount = 0;
                this._IsWaitingToRestart = false;
                this.IsLoading = false;
                this._IsCurrentStreamStarted = true;
            }
            this._eventAggregator.Publish(new PlayStateChangedMessage(e.Value, this.Player.LatestCommand));
            Trace.WriteLine(string.Format("{0} {1}", e.Value.ToString(), this.Player.LatestCommand));
            this.Player.LatestCommand = "";
        }

        private void RefreshAegisToken()
        {
            JsonRequestResult<AegisRefresh> result = OVPClientHttpRequest.BrowserJsonRequest<AegisRefresh>(LiveTVService.AegisRefresh, 0x2710);
            if (result.Exception != null)
            {
                this._eventAggregator.Publish(new ErrorMessage(MessageText.TitleDefault, MessageText.VideoTemporarilyUnavailable, result.Exception.ToString(), ErrorMessageType.PlayerbackError, 0x3e8, null, true));
            }
            else if (result.HttpStatusCode == 200)
            {
                this.SetAegisTokenRefreshSeconds(result.Object.TokenRefreshSeconds);
            }
            else
            {
                this.Player.Stop();
                ErrorMessage message = new ErrorMessage(MessageText.TitleTooManySessions, MessageText.TooManySessions, null, ErrorMessageType.Message, 0x3e8, null, true);
                this._eventAggregator.Publish(message);
            }
        }

        public void ReplayAsset()
        {
            this.Player.Replay();
        }

        public void RestartStream()
        {
            this._IsRestarting = true;
            this._IsRetrying = true;
            if (this.Asset != null)
            {
                this.Player.Stop();
                this.StartStreamWithOOHCheckAsync(this.Asset);
            }
        }

        private void SetAegisTokenRefreshSeconds(string input)
        {
            delegate {
                this._AegisTokenRefreshTimer.Stop();
                if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out this._AegisTokenRefreshSeconds))
                {
                    this._AegisTokenRefreshTimer.Interval = TimeSpan.FromSeconds((double) this._AegisTokenRefreshSeconds);
                    this._AegisTokenRefreshTimer.Start();
                }
            }.OnUIThread();
        }

        private void SetScreenState()
        {
            if (Application.Current.Host.Content.IsFullScreen)
            {
                this.IsFullScreen = true;
                this.CurrentWindowState = VisualStates.FullScreenWindowState;
                this.ControllerRow = 1;
            }
            else
            {
                this.IsFullScreen = false;
                this.CurrentWindowState = VisualStates.EmbeddedWindowState;
                this.ControllerRow = 1;
            }
        }

        public void StartOnDemandStream(AssetInfo assetInfo, int startTime)
        {
            string title;
            string str2;
            string str3;
            string str4;
            Uri uri;
            if (string.IsNullOrWhiteSpace(assetInfo.SeriesTitle))
            {
                title = assetInfo.Title;
                str2 = string.Empty;
                str3 = string.Empty;
                str4 = string.Empty;
            }
            else
            {
                title = assetInfo.SeriesTitle;
                str2 = assetInfo.Title;
                str3 = string.Format("Season {0}", assetInfo.Details.SeasonNumber);
                str4 = string.Format("Episode {0}", assetInfo.Details.EpisodeNumber);
            }
            string str5 = assetInfo.Streams.Smooth.Uri.ToString();
            string host = Application.Current.Host.Source.Host;
            if (!str5.Contains(Uri.SchemeDelimiter))
            {
                str5 = "http://" + host + str5;
            }
            AssetViewModel model = new AssetViewModel {
                MediaPath = assetInfo.Uri,
                Title = title,
                SeriesTitle = assetInfo.SeriesTitle,
                Season = str3,
                Episode = str4,
                EpisodeName = str2,
                IsFastForwardEnabled = assetInfo.IsFastForwardEnabled,
                StartTime = startTime,
                IsLive = str5.ToLowerInvariant().Contains("/api/live/"),
                IsEncrypted = true,
                StreamUrl = str5
            };
            if (assetInfo.Details != null)
            {
                model.Rating = assetInfo.Details.Rating;
                model.Description = assetInfo.Details.Description;
                model.Year = assetInfo.Details.Year;
                model.IsEpisodic = assetInfo.Details.EpisodeNumber != 0;
                model.Cast = (assetInfo.Details.Actors != null) ? string.Join(", ", assetInfo.Details.Actors) : "";
                model.ShortCast = (assetInfo.Details.Actors != null) ? string.Join(", ", assetInfo.Details.Actors.Take<string>(2)) : "";
                model.Director = assetInfo.Details.Director;
            }
            if (assetInfo.Network != null)
            {
                model.NetworkUri = string.IsNullOrWhiteSpace(assetInfo.Network.ImageUri) ? null : new Uri(assetInfo.Network.ImageUri);
                model.IsAvailableOutOfHome = assetInfo.Network.AvailableOutOfHome;
            }
            Uri.TryCreate(assetInfo.ImageUri, UriKind.Absolute, out uri);
            if (Application.Current.Host.Source.Scheme == "http")
            {
                if (uri != null)
                {
                    string uriString = this.CleanupImageUri(uri);
                    if (!uriString.Contains("wayfarer"))
                    {
                        uriString = uriString + "&wayfarer=" + this.GetWayfarerCookie();
                    }
                    uri = new Uri(uriString);
                }
                if (model.NetworkUri != null)
                {
                    string str8 = this.CleanupImageUri(model.NetworkUri);
                    if (!str8.Contains("&sourceType"))
                    {
                        str8 = str8 + "&sourceType=colorhybrid";
                    }
                    if (!str8.Contains("wayfarer"))
                    {
                        str8 = str8 + "&wayfarer=" + this.GetWayfarerCookie();
                    }
                    model.NetworkUri = new Uri(str8);
                }
            }
            model.ImageUri = uri;
            this.Asset = model;
        }

        private void StartPlayer(AssetViewModel assetViewModel)
        {
            this._player.MediaPath = assetViewModel.MediaPath;
            this._player.ReportPosition = (this.Asset != null) && !this.Asset.IsLive;
            AESPlaylistItem item = new AESPlaylistItem {
                MediaSource = this.Asset.MediaSource,
                Title = assetViewModel.Title,
                ThumbSource = assetViewModel.ThumbSource,
                DeliveryMethod = DeliveryMethods.AdaptiveStreaming,
                JumpToLive = assetViewModel.IsLive,
                DecryptionInfo = assetViewModel.DecryptionInfo,
                IsEncrypted = assetViewModel.IsEncrypted,
                IsLive = assetViewModel.IsLive
            };
            item.CustomMetadata = this.BuildCustomMetadata();
            this._player.Instance.Playlist.Clear();
            this._player.Instance.Playlist.Add(item);
            this._player.Play();
            if (!this._IsRestarting)
            {
                this.lastPlayingAsset = assetViewModel;
                this._LiveAssetStartTime = DateTime.Now;
                this._eventAggregator.Publish(new EGPlaybackStartedEventMessage(this.Player.Instance.VolumeLevel, this.Player.Instance.IsMuted));
            }
            else
            {
                this._eventAggregator.Publish(new EGPlaybackRestartedEventMessage(this._lastPostion.TotalSeconds, this.Player.Instance.VolumeLevel, this.Player.Instance.IsMuted));
            }
        }

        private void StartStream(AssetViewModel assetViewModel, CancellationToken ct)
        {
            System.Action action = null;
            System.Action action2 = null;
            System.Action action3 = null;
            System.Action action4 = null;
            if (!ct.IsCancellationRequested)
            {
                this.IsLoading = true;
                try
                {
                    JsonRequestResult<AssetUriInfo> result;
                    if (this.Asset.IsLive)
                    {
                        if (this._IsRestarting)
                        {
                            if (action == null)
                            {
                                action = () => this._eventAggregator.Publish(new EGChannelListChannelSelectedEventMessage("TMS", this.Asset.ServiceID.ToString(), "restarted"));
                            }
                            action.OnUIThread();
                        }
                    }
                    else
                    {
                        if (this._lastPostion > TimeSpan.Zero)
                        {
                            this.pendingTimeSpan = this._lastPostion;
                        }
                        else
                        {
                            this.pendingTimeSpan = (assetViewModel.StartTime > 0) ? TimeSpan.FromSeconds((double) assetViewModel.StartTime) : TimeSpan.Zero;
                        }
                        if (this._IsRestarting)
                        {
                            if (action2 == null)
                            {
                                action2 = () => this._eventAggregator.Publish(new EGContentPlayedEventMessage(this.pendingTimeSpan.TotalSeconds, this.Player.Instance.VolumeLevel, this.Player.Instance.IsMuted, "restarted"));
                            }
                            action2.OnUIThread();
                        }
                        else
                        {
                            if (action3 == null)
                            {
                                action3 = () => this._eventAggregator.Publish(new EGContentPlayedEventMessage(this.pendingTimeSpan.TotalSeconds, this.Player.Instance.VolumeLevel, this.Player.Instance.IsMuted, "userSelected"));
                            }
                            action3.OnUIThread();
                        }
                    }
                    if (((App) Application.Current).OVPApplicationMode == AppMode.SportsNetwork)
                    {
                        result = OVPClientHttpRequest.BrowserJsonRequest<AssetUriInfo>(assetViewModel.StreamUrl, 0x1770);
                    }
                    else
                    {
                        result = OVPClientHttpRequest.BrowserJsonRequest<AssetUriInfo>(assetViewModel.StreamUrl, 0x1770);
                    }
                    if (!ct.IsCancellationRequested)
                    {
                        if (result.HttpStatusCode == 200)
                        {
                            this.SetAegisTokenRefreshSeconds(result.Object.TokenRefreshSeconds);
                            WebRequestResult result2 = OVPClientHttpRequest.BrowserHttpRequest(result.Object.KeyUri.AbsoluteUri, 0x1770);
                            if (!ct.IsCancellationRequested)
                            {
                                if (result2.Exception != null)
                                {
                                    ErrorMessage message = new ErrorMessage(MessageText.TitleDefault, MessageText.VideoTemporarilyUnavailable, result2.Exception.ToString(), ErrorMessageType.PlayerbackError, 0x3e8, null, true);
                                    this._eventAggregator.Publish(message);
                                }
                                else
                                {
                                    System.IO.Stream responseStream = result2.Response.GetResponseStream();
                                    byte[] buffer = new byte[result2.Response.ContentLength];
                                    responseStream.Read(buffer, 0, (int) result2.Response.ContentLength);
                                    string url = result.Object.StreamUri.ToString();
                                    url = this.AddManifestIfNecessary(url);
                                    AESDecryptionInfo info = new AESDecryptionInfo(result.Object.IV, buffer) {
                                        SessionID = url.Substring(url.ToLowerInvariant().LastIndexOf("?sessionid=") + 1)
                                    };
                                    assetViewModel.MediaSource = new Uri(url);
                                    assetViewModel.DecryptionInfo = info;
                                    if (!ct.IsCancellationRequested)
                                    {
                                        if (action4 == null)
                                        {
                                            action4 = delegate {
                                                this._eventAggregator.Publish(new EGStreamURIObtainedEventMessage(assetViewModel.StreamUrl, assetViewModel.IsFastForwardEnabled));
                                                assetViewModel.LicenseExpirationTime = this.GetVXTokenExpirationTime();
                                                Trace.WriteLine(string.Format("vxtoken acquired - expires {0}", assetViewModel.LicenseExpirationTime));
                                                this.Interaction.DismissPlaybackErrorMessage();
                                                this.StartPlayer(assetViewModel);
                                                this.NotifyStreamStarted(assetViewModel);
                                            };
                                        }
                                        action4.OnUIThread();
                                    }
                                }
                            }
                        }
                        else if (result.HttpStatusCode == 0x1ad)
                        {
                            if (!ct.IsCancellationRequested)
                            {
                                ErrorMessage message2 = new ErrorMessage(MessageText.TitleTooManySessions, MessageText.TooManySessions, result.RespondText, ErrorMessageType.Message, 0x3e8, null, true);
                                this._eventAggregator.Publish(message2);
                            }
                        }
                        else
                        {
                            string str2 = "";
                            if (result.Exception != null)
                            {
                                str2 = result.Exception.ToString();
                            }
                            throw new Exception(string.Format("Requesting returned unknow error code {0}.RespondText:{1}, {2}", result.HttpStatusCode, result.RespondText, str2));
                        }
                    }
                }
                catch (Exception exception)
                {
                    if (!ct.IsCancellationRequested)
                    {
                        this._eventAggregator.Publish(new ErrorMessage(MessageText.TitleDefault, MessageText.VideoTemporarilyUnavailable, exception.ToString(), ErrorMessageType.PlayerbackError, 0x3e8, null, true));
                        Console.WriteLine(exception);
                    }
                }
            }
        }

        public void StartStreamWithOOHCheckAsync(AssetViewModel assetViewModel)
        {
            if (this._cancellationTokenSource != null)
            {
                this._cancellationTokenSource.Cancel();
            }
            this._cancellationTokenSource = new CancellationTokenSource();
            CancellationToken ct = this._cancellationTokenSource.Token;
            Task.Factory.StartNew(delegate {
                System.Action<bool, bool, bool> callback = null;
                if (!ct.IsCancellationRequested)
                {
                    this.IsLoading = true;
                    if (((App) Application.Current).OVPApplicationMode == AppMode.SportsNetwork)
                    {
                        this.StartStream(assetViewModel, ct);
                    }
                    else
                    {
                        if (callback == null)
                        {
                            callback = delegate (bool isOOH, bool isOutOfCountry, bool isLocationChanged) {
                                System.Action action = null;
                                if (!ct.IsCancellationRequested)
                                {
                                    if (assetViewModel.IsLive)
                                    {
                                        if (!isOOH || (isOOH && assetViewModel.IsAvailableOutOfHome))
                                        {
                                            this.StartStream(assetViewModel, ct);
                                        }
                                    }
                                    else if (!isOOH || (isOOH && assetViewModel.IsAvailableOutOfHome))
                                    {
                                        this.StartStream(assetViewModel, ct);
                                    }
                                    else
                                    {
                                        this.IsLoading = false;
                                        ErrorMessage message = new ErrorMessage(MessageText.TitleNotAvailable, MessageText.NotAvailableOOH, "", ErrorMessageType.Message, 0x3e8, null, true);
                                        if (action == null)
                                        {
                                            action = () => this._eventAggregator.Publish(new LocationChangeAcknowledgedEventMessage(isOOH, isOutOfCountry, assetViewModel.IsAvailableOutOfHome, false));
                                        }
                                        message.MessageDismissed += action;
                                        this._eventAggregator.Publish(message);
                                    }
                                }
                            };
                        }
                        this._locationService.CheckLocation(callback);
                    }
                }
            }, this._cancellationTokenSource.Token);
        }

        public void TogglePlay()
        {
            if (this._IsWaitingToRestart)
            {
                this.StartStreamWithOOHCheckAsync(this.Asset);
            }
            else
            {
                this.Player.TogglePlay();
            }
        }

        public DateTime AegisTokenObtainedTime
        {
            get
            {
                return this._AegisTokenObtainedTime;
            }
            set
            {
                this._AegisTokenObtainedTime = value;
                this.NotifyOfPropertyChange<DateTime>(System.Linq.Expressions.Expression.Lambda<System.Func<DateTime>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(AssetViewerViewModel)), (MethodInfo) methodof(AssetViewerViewModel.get_AegisTokenObtainedTime)), new ParameterExpression[0]));
            }
        }

        public int AegisTokenRefreshSeconds
        {
            get
            {
                return this._AegisTokenRefreshSeconds;
            }
            set
            {
                this._AegisTokenRefreshSeconds = value;
            }
        }

        public AssetViewModel Asset
        {
            get
            {
                return this._asset;
            }
            set
            {
                AssetViewModel model = this._asset;
                this._asset = value;
                if (((model != null) && (this._asset != null)) && (model.StreamUrl == this._asset.StreamUrl))
                {
                    this._asset.LicenseExpirationTime = model.LicenseExpirationTime;
                }
                else
                {
                    if ((this.Interaction.CurrentErrorMessage != null) && (this.Interaction.CurrentErrorMessage.MessageType != ErrorMessageType.Message))
                    {
                        this.Interaction.DismissErrorMessage();
                    }
                    this.Player.EndPlayback();
                    if (this.lastPlayingAsset != null)
                    {
                        TimeSpan span = (TimeSpan) (DateTime.Now - this._LiveAssetStartTime);
                        this._eventAggregator.Publish(new EGPlaybackStoppedEventMessage("streamSwitch", span.TotalSeconds));
                    }
                    this._IsRestarting = false;
                    this._IsRetrying = false;
                    this._RetryCount = 0;
                    this._IsWaitingToRestart = false;
                    this._IsCurrentStreamStarted = false;
                    if (this.Asset != null)
                    {
                        this.StartStreamWithOOHCheckAsync(this.Asset);
                    }
                    this.SetScreenState();
                }
                this.NotifyOfPropertyChange<AssetViewModel>(System.Linq.Expressions.Expression.Lambda<System.Func<AssetViewModel>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(AssetViewerViewModel)), (MethodInfo) methodof(AssetViewerViewModel.get_Asset)), new ParameterExpression[0]));
            }
        }

        public int ControllerRow
        {
            get
            {
                return this._controllerRow;
            }
            set
            {
                if (this._controllerRow != value)
                {
                    this._controllerRow = value;
                    this.NotifyOfPropertyChange<int>(System.Linq.Expressions.Expression.Lambda<System.Func<int>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(AssetViewerViewModel)), (MethodInfo) methodof(AssetViewerViewModel.get_ControllerRow)), new ParameterExpression[0]));
                }
            }
        }

        public string CurrentControllerState
        {
            get
            {
                return this._currentControllerState;
            }
            set
            {
                if (this._currentControllerState != value)
                {
                    this._currentControllerState = value;
                    this.NotifyOfPropertyChange<IEnumerable<string>>(System.Linq.Expressions.Expression.Lambda<System.Func<IEnumerable<string>>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(AssetViewerViewModel)), (MethodInfo) methodof(BaseViewModel.get_CurrentVisualStates)), new ParameterExpression[0]));
                }
            }
        }

        public string CurrentWindowState
        {
            get
            {
                return this._currentWindowState;
            }
            set
            {
                if (this._currentControllerState != value)
                {
                    this._currentWindowState = value;
                    this.NotifyOfPropertyChange<IEnumerable<string>>(System.Linq.Expressions.Expression.Lambda<System.Func<IEnumerable<string>>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(AssetViewerViewModel)), (MethodInfo) methodof(BaseViewModel.get_CurrentVisualStates)), new ParameterExpression[0]));
                }
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
                this._isFullScreen = value;
                this.NotifyOfPropertyChange<IEnumerable<string>>(System.Linq.Expressions.Expression.Lambda<System.Func<IEnumerable<string>>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(AssetViewerViewModel)), (MethodInfo) methodof(BaseViewModel.get_CurrentVisualStates)), new ParameterExpression[0]));
            }
        }

        public bool IsLoading
        {
            get
            {
                return (this._IsLoading && !this.Interaction.IsShowingErrorMessage);
            }
            set
            {
                this._IsLoading = value;
                this.NotifyOfPropertyChange<bool>(System.Linq.Expressions.Expression.Lambda<System.Func<bool>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(AssetViewerViewModel)), (MethodInfo) methodof(AssetViewerViewModel.get_IsLoading)), new ParameterExpression[0]));
            }
        }

        public bool IsMuted
        {
            get
            {
                return this._isMuted;
            }
            set
            {
                this._isMuted = value;
                this.NotifyOfPropertyChange<bool>(System.Linq.Expressions.Expression.Lambda<System.Func<bool>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(AssetViewerViewModel)), (MethodInfo) methodof(AssetViewerViewModel.get_IsMuted)), new ParameterExpression[0]));
            }
        }

        public bool IsOutOfHomeDebug
        {
            get
            {
                return this._IsOutOfHomeDebug;
            }
            set
            {
                this._IsOutOfHomeDebug = value;
                this.NotifyOfPropertyChange<bool>(System.Linq.Expressions.Expression.Lambda<System.Func<bool>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(AssetViewerViewModel)), (MethodInfo) methodof(AssetViewerViewModel.get_IsOutOfHomeDebug)), new ParameterExpression[0]));
            }
        }

        public bool IsRetrying
        {
            get
            {
                return this._IsRetrying;
            }
            set
            {
                this._IsRetrying = value;
            }
        }

        public PlayerViewModel Player
        {
            get
            {
                return this._player;
            }
            set
            {
                if (this._player != value)
                {
                    this._player = value;
                    this.NotifyOfPropertyChange<PlayerViewModel>(System.Linq.Expressions.Expression.Lambda<System.Func<PlayerViewModel>>(System.Linq.Expressions.Expression.Property(System.Linq.Expressions.Expression.Constant(this, typeof(AssetViewerViewModel)), (MethodInfo) methodof(AssetViewerViewModel.get_Player)), new ParameterExpression[0]));
                }
            }
        }

        public bool ShowErrorDetail { get; set; }


        public static class VisualStates
        {
            public static readonly string EmbeddedWindowState = "Embedded";
            public static readonly string FullScreenWindowState = "FullScreen";
            public static readonly string HideControllerState = "HideControllerBar";
            public static readonly string ShowControllerState = "ShowControllerBar";
        }
    }
}

