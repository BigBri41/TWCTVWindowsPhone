namespace TWC.OVP.ViewModels
{
    using Caliburn.Micro;
    using Microsoft.SilverlightMediaFramework.Core;
    using Microsoft.SilverlightMediaFramework.Core.Media;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using TWC.OVP.Behaviors;
    using TWC.OVP.Controls;
    using TWC.OVP.Framework.Models;
    using TWC.OVP.Framework.ViewModels;
    using TWC.OVP.Messages;
    using TWC.OVP.Services;
    using TWC.OVP.Views;

    public class PlayerViewModel : BaseViewModel, IHandle<SettingsChangedMessage>, IHandle
    {
        private IEnumerable<StreamMetadata> _availableCaptionStreams;
        private double _captionBubbleLanguageVerticalOffsetAdjustment;
        private TWC.OVP.Framework.Models.CaptionsOverrideSettings _captionsOverrideSettings;
        private int _captionStreamSelectedIndex;
        private bool _captionStreamsLoaded;
        private IEventAggregator _eventAggregator;
        private bool _isClosedCaptioningAvailable;
        private string _LatestCommand = "";
        private TWC.OVP.Behaviors.PlayerCommand _playerCommand;
        private StreamMetadata _selectedCaptionStream;
        private ISettingsService _settingsService;
        private ObservableCollection<string> _userFriendlyStreamNames = new ObservableCollection<string>();
        private string _visibleCaptionText;
        public bool ReportPosition;

        public PlayerViewModel(IEventAggregator eventAggregator, ISettingsService settingsService)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                this._settingsService = settingsService;
                this._eventAggregator = eventAggregator;
                this._eventAggregator.Subscribe(this);
            }
        }

        public void EndPlayback()
        {
            if ((this.Instance != null) && ((this.Instance.Playlist != null) && (this.Instance.Playlist.Count > 0)))
            {
                this.Stop();
                this.Instance.Playlist.Clear();
            }
        }

        public void Handle(SettingsChangedMessage message)
        {
            this.CaptionsOverrideSettings = this._settingsService.CaptionsOverrideSettings;
            this.Instance.RedrawActiveCaptions();
        }

        private void InitializeUserFriendlyStreamNames()
        {
            this.UserFriendlyStreamNames.Clear();
            if (this.AvailableCaptionStreams != null)
            {
                foreach (StreamMetadata metadata in this.AvailableCaptionStreams)
                {
                    string str;
                    if (((str = metadata.Name) != null) && (str == "textstream_eng"))
                    {
                        this.UserFriendlyStreamNames.Add("English");
                    }
                    else
                    {
                        this.UserFriendlyStreamNames.Add(metadata.Name);
                    }
                }
            }
        }

        private void Instance_ErrorOccurred(object sender, VideoPlayerErrorEventArgs e)
        {
            this._eventAggregator.Publish(new ErrorMessage(e.ErrorMessage, e.ErrorDetail, "", ErrorMessageType.PlayerbackError, 0x3e8, null, true));
        }

        private void Instance_IsMutedChanged(object sender, CustomEventArgs<bool> e)
        {
            this._settingsService.IsMuted = this.Instance.IsMuted;
            this._eventAggregator.Publish(new EGPlayerMuteToggledEventMessage(e.Value));
        }

        private void Instance_MediaEnded(object sender, EventArgs e)
        {
            this._eventAggregator.Publish(new MediaEndedEventMessage());
        }

        private void Instance_MediaOpened(object sender, EventArgs e)
        {
            this.AvailableCaptionStreams = this.Instance.AvailableCaptionStreams;
            this.CaptionStreamsLoaded = true;
            if (this._settingsService.IsClosedCaptioningEnabled)
            {
                this.StartCaptionStream();
            }
        }

        private void Instance_PlaybackPositionChanged(object sender, CustomEventArgs<TimeSpan> e)
        {
            if (this.ReportPosition)
            {
                this._eventAggregator.Publish(new PlaybackPositionChangedMessage(e.Value, this.Instance.EndPosition, this.MediaPath));
            }
        }

        private void Instance_PlaylistItemChanged(object sender, CustomEventArgs<PlaylistItem> e)
        {
            this.AvailableCaptionStreams = null;
            this.CaptionStreamsLoaded = false;
            this.VisibleCaptionText = "";
        }

        private void Instance_VolumeLevelChanged(object sender, CustomEventArgs<double> e)
        {
            this._settingsService.VolumeLevel = this.Instance.VolumeLevel;
            this._eventAggregator.Publish(new EGPlayerVolumeChangedEventMessage(this.Instance.VolumeLevel, this.Instance.IsMuted));
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            if (this.Instance != null)
            {
                this.Instance.VolumeLevel = this._settingsService.VolumeLevel;
                this.Instance.IsMuted = this._settingsService.IsMuted;
                this.Instance.PlaybackPositionChanged += new EventHandler<CustomEventArgs<TimeSpan>>(this.Instance_PlaybackPositionChanged);
                this.Instance.VolumeLevelChanged += new EventHandler<CustomEventArgs<double>>(this.Instance_VolumeLevelChanged);
                this.Instance.IsMutedChanged += new EventHandler<CustomEventArgs<bool>>(this.Instance_IsMutedChanged);
                this.Instance.MediaOpened += new EventHandler(this.Instance_MediaOpened);
                this.Instance.ErrorOccurred += new EventHandler<VideoPlayerErrorEventArgs>(this.Instance_ErrorOccurred);
                this.Instance.PlaylistItemChanged += new EventHandler<CustomEventArgs<PlaylistItem>>(this.Instance_PlaylistItemChanged);
                this.Instance.MediaEnded += new EventHandler(this.Instance_MediaEnded);
            }
            if (view.GetType() == typeof(PlayerView))
            {
                this.NotifyOfPropertyChange<AesopVideoPlayer>(Expression.Lambda<System.Func<AesopVideoPlayer>>(Expression.Property(Expression.Constant(this, typeof(PlayerViewModel)), (MethodInfo) methodof(PlayerViewModel.get_Instance)), new ParameterExpression[0]));
            }
        }

        public void Pause()
        {
            this._LatestCommand = "OnCmdPause";
            this.PlayerCommand = TWC.OVP.Behaviors.PlayerCommand.Pause;
        }

        public void Play()
        {
            this._LatestCommand = "OnCmdPlay";
            this.PlayerCommand = TWC.OVP.Behaviors.PlayerCommand.Play;
        }

        public void Replay()
        {
            this._LatestCommand = "OnCmdReplay";
            this.SeekToPosition(TimeSpan.Zero);
            this.PlayerCommand = TWC.OVP.Behaviors.PlayerCommand.Play;
        }

        public void SeekToPosition(TimeSpan timeSpan)
        {
            this._LatestCommand = "OnCmdSeekToPosition";
            this.Instance.SeekToPosition(timeSpan);
        }

        private void StartCaptionStream()
        {
            if ((this.AvailableCaptionStreams == null) || !this.AvailableCaptionStreams.Any<StreamMetadata>())
            {
                this.SelectedCaptionStream = null;
            }
            else
            {
                StreamMetadata metadata = Enumerable.FirstOrDefault<StreamMetadata>(this.AvailableCaptionStreams, s => s.Name == this._settingsService.PrevCaptionStreamName);
                if (metadata != null)
                {
                    this.SelectedCaptionStream = metadata;
                }
                else
                {
                    metadata = Enumerable.FirstOrDefault<StreamMetadata>(this.AvailableCaptionStreams, s => s.Language == this._settingsService.PrevCaptionStreamLanguage);
                    if (metadata != null)
                    {
                        this.SelectedCaptionStream = metadata;
                    }
                    else
                    {
                        this.SelectedCaptionStream = this.AvailableCaptionStreams.FirstOrDefault<StreamMetadata>();
                    }
                }
            }
        }

        public void Stop()
        {
            this._LatestCommand = "OnCmdStop";
            this.PlayerCommand = TWC.OVP.Behaviors.PlayerCommand.Stop;
        }

        public void TogglePlay()
        {
            if ((this.PlayerCommand == TWC.OVP.Behaviors.PlayerCommand.Play) || (this.PlayerCommand == TWC.OVP.Behaviors.PlayerCommand.Replay))
            {
                this.Pause();
                this._eventAggregator.Publish(new EGPlayerPauseToggledEventMessage(true, "user"));
            }
            else
            {
                this.Play();
                this._eventAggregator.Publish(new EGPlayerPauseToggledEventMessage(false, "user"));
            }
        }

        public IEnumerable<StreamMetadata> AvailableCaptionStreams
        {
            get
            {
                return this._availableCaptionStreams;
            }
            set
            {
                this._availableCaptionStreams = value;
                this.IsClosedCaptioningAvailable = (this._availableCaptionStreams != null) && this._availableCaptionStreams.Any<StreamMetadata>();
                this.InitializeUserFriendlyStreamNames();
                this.NotifyOfPropertyChange<IEnumerable<StreamMetadata>>(Expression.Lambda<System.Func<IEnumerable<StreamMetadata>>>(Expression.Property(Expression.Constant(this, typeof(PlayerViewModel)), (MethodInfo) methodof(PlayerViewModel.get_AvailableCaptionStreams)), new ParameterExpression[0]));
            }
        }

        public double CaptionBubbleLanguageVerticalOffsetAdjustment
        {
            get
            {
                return this._captionBubbleLanguageVerticalOffsetAdjustment;
            }
            set
            {
                this._captionBubbleLanguageVerticalOffsetAdjustment = value;
                this.NotifyOfPropertyChange<double>(Expression.Lambda<System.Func<double>>(Expression.Property(Expression.Constant(this, typeof(PlayerViewModel)), (MethodInfo) methodof(PlayerViewModel.get_CaptionBubbleLanguageVerticalOffsetAdjustment)), new ParameterExpression[0]));
            }
        }

        public TWC.OVP.Framework.Models.CaptionsOverrideSettings CaptionsOverrideSettings
        {
            get
            {
                return this._captionsOverrideSettings;
            }
            set
            {
                if (this._captionsOverrideSettings != value)
                {
                    this._captionsOverrideSettings = value;
                    this.NotifyOfPropertyChange<TWC.OVP.Framework.Models.CaptionsOverrideSettings>(Expression.Lambda<System.Func<TWC.OVP.Framework.Models.CaptionsOverrideSettings>>(Expression.Property(Expression.Constant(this, typeof(PlayerViewModel)), (MethodInfo) methodof(PlayerViewModel.get_CaptionsOverrideSettings)), new ParameterExpression[0]));
                }
            }
        }

        public int CaptionStreamSelectedIndex
        {
            get
            {
                return this._captionStreamSelectedIndex;
            }
            set
            {
                this._captionStreamSelectedIndex = value;
                this.NotifyOfPropertyChange<int>(Expression.Lambda<System.Func<int>>(Expression.Property(Expression.Constant(this, typeof(PlayerViewModel)), (MethodInfo) methodof(PlayerViewModel.get_CaptionStreamSelectedIndex)), new ParameterExpression[0]));
            }
        }

        public bool CaptionStreamsLoaded
        {
            get
            {
                return this._captionStreamsLoaded;
            }
            set
            {
                this._captionStreamsLoaded = value;
                this.NotifyOfPropertyChange("CaptionStreamsLoaded");
            }
        }

        public static System.Net.CookieContainer CookieContainer
        {
            [CompilerGenerated]
            get
            {
                return <CookieContainer>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                <CookieContainer>k__BackingField = value;
            }
        }

        public AesopVideoPlayer Instance
        {
            get
            {
                PlayerView view = this.GetView(null) as PlayerView;
                if (view != null)
                {
                    return view.VideoPlayer;
                }
                return null;
            }
        }

        public bool IsClosedCaptioningAvailable
        {
            get
            {
                return this._isClosedCaptioningAvailable;
            }
            set
            {
                this._isClosedCaptioningAvailable = value;
                this.NotifyOfPropertyChange<bool>(Expression.Lambda<System.Func<bool>>(Expression.Property(Expression.Constant(this, typeof(PlayerViewModel)), (MethodInfo) methodof(PlayerViewModel.get_IsClosedCaptioningAvailable)), new ParameterExpression[0]));
                this.NotifyOfPropertyChange<bool>(Expression.Lambda<System.Func<bool>>(Expression.Property(Expression.Constant(this, typeof(PlayerViewModel)), (MethodInfo) methodof(PlayerViewModel.get_IsClosedCaptioningNotAvailable)), new ParameterExpression[0]));
            }
        }

        public bool IsClosedCaptioningEnabled
        {
            get
            {
                return (this.SelectedCaptionStream != null);
            }
            set
            {
                if (value)
                {
                    this.StartCaptionStream();
                }
                else
                {
                    this.SelectedCaptionStream = null;
                }
                this._eventAggregator.Publish(new EGClosedCaptioningToggledEventMessage(value));
                this.NotifyOfPropertyChange<bool>(Expression.Lambda<System.Func<bool>>(Expression.Property(Expression.Constant(this, typeof(PlayerViewModel)), (MethodInfo) methodof(PlayerViewModel.get_IsClosedCaptioningEnabled)), new ParameterExpression[0]));
            }
        }

        public bool IsClosedCaptioningNotAvailable
        {
            get
            {
                return !this._isClosedCaptioningAvailable;
            }
        }

        public bool IsFreewheelEnabled { get; set; }

        public static bool IsTestMode
        {
            [CompilerGenerated]
            get
            {
                return <IsTestMode>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                <IsTestMode>k__BackingField = value;
            }
        }

        public string LatestCommand
        {
            get
            {
                return this._LatestCommand;
            }
            set
            {
                this._LatestCommand = value;
            }
        }

        public string MediaPath { get; set; }

        public TWC.OVP.Behaviors.PlayerCommand PlayerCommand
        {
            get
            {
                return this._playerCommand;
            }
            set
            {
                if (this._playerCommand != value)
                {
                    this._playerCommand = value;
                    this.NotifyOfPropertyChange<TWC.OVP.Behaviors.PlayerCommand>(Expression.Lambda<System.Func<TWC.OVP.Behaviors.PlayerCommand>>(Expression.Property(Expression.Constant(this, typeof(PlayerViewModel)), (MethodInfo) methodof(PlayerViewModel.get_PlayerCommand)), new ParameterExpression[0]));
                }
            }
        }

        public StreamMetadata SelectedCaptionStream
        {
            get
            {
                return this._selectedCaptionStream;
            }
            set
            {
                this._selectedCaptionStream = value;
                if ((this.AvailableCaptionStreams != null) && this.AvailableCaptionStreams.Any<StreamMetadata>())
                {
                    if (value != null)
                    {
                        this._settingsService.PrevCaptionStreamName = value.Name;
                        this._settingsService.PrevCaptionStreamLanguage = value.Language;
                        this._settingsService.IsClosedCaptioningEnabled = true;
                    }
                    else
                    {
                        this._settingsService.IsClosedCaptioningEnabled = false;
                    }
                }
                if (this._selectedCaptionStream == null)
                {
                    this.CaptionStreamSelectedIndex = -1;
                    this.VisibleCaptionText = "";
                }
                else
                {
                    this.CaptionStreamSelectedIndex = this.AvailableCaptionStreams.ToList<StreamMetadata>().IndexOf(this._selectedCaptionStream);
                }
                this.NotifyOfPropertyChange<StreamMetadata>(Expression.Lambda<System.Func<StreamMetadata>>(Expression.Property(Expression.Constant(this, typeof(PlayerViewModel)), (MethodInfo) methodof(PlayerViewModel.get_SelectedCaptionStream)), new ParameterExpression[0]));
                this.NotifyOfPropertyChange<bool>(Expression.Lambda<System.Func<bool>>(Expression.Property(Expression.Constant(this, typeof(PlayerViewModel)), (MethodInfo) methodof(PlayerViewModel.get_IsClosedCaptioningEnabled)), new ParameterExpression[0]));
            }
        }

        public static System.Net.CookieContainer StreamCookies
        {
            [CompilerGenerated]
            get
            {
                return <StreamCookies>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                <StreamCookies>k__BackingField = value;
            }
        }

        public ObservableCollection<string> UserFriendlyStreamNames
        {
            get
            {
                return this._userFriendlyStreamNames;
            }
            set
            {
                this._userFriendlyStreamNames = value;
                this.NotifyOfPropertyChange<ObservableCollection<string>>(Expression.Lambda<System.Func<ObservableCollection<string>>>(Expression.Property(Expression.Constant(this, typeof(PlayerViewModel)), (MethodInfo) methodof(PlayerViewModel.get_UserFriendlyStreamNames)), new ParameterExpression[0]));
            }
        }

        public string VisibleCaptionText
        {
            get
            {
                return this._visibleCaptionText;
            }
            set
            {
                if (this._visibleCaptionText != value)
                {
                    this._visibleCaptionText = value;
                    this.NotifyOfPropertyChange<string>(Expression.Lambda<System.Func<string>>(Expression.Property(Expression.Constant(this, typeof(PlayerViewModel)), (MethodInfo) methodof(PlayerViewModel.get_VisibleCaptionText)), new ParameterExpression[0]));
                }
            }
        }
    }
}

