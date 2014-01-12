namespace TWC.OVP.Controls
{
    using Microsoft.SilverlightMediaFramework.Core;
    using Microsoft.SilverlightMediaFramework.Plugins.Primitives;
    using Microsoft.SilverlightMediaFramework.Utilities.Extensions;
    using Microsoft.SilverlightMediaFramework.Utilities.Metadata;
    using Microsoft.Web.Media.SmoothStreaming;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;
    using TWC.OVP;
    using TWC.OVP.Framework;
    using TWC.OVP.Framework.Utilities;
    using TWC.OVP.Messages;

    public class VideoPlayer : SMFPlayer
    {
        private DateTime _bufferingStartTime = DateTime.MinValue;
        private DispatcherTimer _excessBufferingTimer = new DispatcherTimer();
        private string _lastError;
        private DateTime? _playbackStartTime = null;
        private bool _seekingToLive;
        private List<DateTime> _seekToLiveTimes = new List<DateTime>();
        public static readonly DependencyProperty IsFreewheelEnabledProperty = DependencyProperty.Register("IsFreewheelEnabled", typeof(bool), typeof(VideoPlayer), new PropertyMetadata(new PropertyChangedCallback(VideoPlayer.OnIsFreewheelEnabledPropertyChanged)));
        public const int MaxBufferingSeconds = 20;
        public const int MaxSeeksToLivePerMinute = 3;
        private bool seekInProgress;

        public event EventHandler<VideoPlayerErrorEventArgs> ErrorOccurred;

        public VideoPlayer()
        {
            base.DefaultStyleKey = typeof(SMFPlayer);
            if (!DesignerProperties.IsInDesignTool)
            {
                base.LogLevel = Microsoft.SilverlightMediaFramework.Plugins.Primitives.LogLevel.All;
                TraceLogWriter logWriter = new TraceLogWriter();
                base.Logger.RegisterLogWriter(logWriter);
                MetadataItem item = new MetadataItem {
                    Key = "FreeWheel.Plugin.IsEnabled",
                    Value = false
                };
                base.GlobalConfigMetadata.Add(item);
                base.PlaybackPositionChanged += new EventHandler<CustomEventArgs<TimeSpan>>(this.VideoPlayer_PlaybackPositionChanged);
                base.PlaylistItemChanged += new EventHandler<CustomEventArgs<PlaylistItem>>(this.VideoPlayer_PlaylistItemChanged);
                base.PlayStateChanged += new EventHandler<CustomEventArgs<MediaPluginState>>(this.VideoPlayer_PlayStateChanged);
                base.LogEntryReceived += new EventHandler<CustomEventArgs<LogEntry>>(this.VideoPlayer_LogEntryReceived);
            }
        }

        public string GetPlaybackDurationText()
        {
            if (!this._playbackStartTime.HasValue)
            {
                return string.Empty;
            }
            TimeSpan span = (TimeSpan) (DateTime.Now - this._playbackStartTime.Value);
            return ("Playback duration: " + span.ToString(@"d\.hh\:mm\:ss"));
        }

        private void OnError(string message, string detail)
        {
            string errorDetail = detail + Environment.NewLine + this.GetPlaybackDurationText();
            Trace.WriteLine("Player error: " + message + " Detail: " + errorDetail);
            if (this.ErrorOccurred != null)
            {
                this.ErrorOccurred(this, new VideoPlayerErrorEventArgs(message, errorDetail));
            }
        }

        private void OnIsFreewheelEnabledChanged(bool isEnabled)
        {
            MetadataItem item = (from c in base.GlobalConfigMetadata
                where c.Key == "FreeWheel.Plugin.IsEnabled"
                select c).FirstOrDefault<MetadataItem>();
            if (item == null)
            {
                MetadataItem item2 = new MetadataItem {
                    Key = "FreeWheel.Plugin.IsEnabled",
                    Value = isEnabled
                };
                base.GlobalConfigMetadata.Add(item2);
            }
            else
            {
                item.Value = isEnabled;
            }
        }

        private static void OnIsFreewheelEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as VideoPlayer).IfNotNull<VideoPlayer>(p => p.OnIsFreewheelEnabledChanged((bool) e.NewValue));
        }

        protected override void OnManifestReady()
        {
            base.OnManifestReady();
            this.SSME.LiveBackOff = TimeSpan.Zero;
            this.SSME.BufferingTime = TimeSpan.FromSeconds(4.0);
            this.SSME.LivePlaybackOffset = this.SSME.BufferingTime;
            this.SSME.ChunkDownloadFailed += new EventHandler<ChunkDownloadedEventArgs>(this.SSME_ChunkDownloadFailed);
        }

        protected override void OnMediaPluginLoaded()
        {
            base.OnMediaPluginLoaded();
            if ((base.CurrentPlaylistItem != null) && ((VideoPlayerPlaylistItem) base.CurrentPlaylistItem).IsLive)
            {
                this.SSME.CanPause = false;
                this.SSME.CanSeek = false;
            }
        }

        public void RedrawActiveCaptions()
        {
            (base.CaptionsPresenterElement as VideoPlayerCaptionsPresenter).IfNotNull<VideoPlayerCaptionsPresenter>(i => i.RedrawActiveCaptions());
        }

        private bool SeekToLiveFrequencyExceeded()
        {
            return ((from t in this._seekToLiveTimes
                where t > DateTime.Now.AddSeconds(-60.0)
                select t).Count<DateTime>() > 3);
        }

        private bool SeekToLiveOccurredRecently()
        {
            return (from t in this._seekToLiveTimes
                where t > DateTime.Now.AddSeconds(-10.0)
                select t).Any<DateTime>();
        }

        private void SSME_ChunkDownloadFailed(object sender, ChunkDownloadedEventArgs e)
        {
            Log.Warn("{0}({1}) {2}", new object[] { e.StatusCode, (int) e.StatusCode, e.CanonicalUri });
        }

        protected override void UpdateTimelinePositions(bool isSeeking = false, bool isStopping = false)
        {
            Action<VideoPlayerCaptionsPresenter> action = null;
            base.UpdateTimelinePositions(isSeeking, isStopping);
            this.seekInProgress = this.seekInProgress || isSeeking;
            try
            {
                if ((!isStopping && (base.RetryState == SMFPlayer.RetryStateEnum.NotRetrying)) && (base.ActiveMediaPlugin != null))
                {
                    VideoPlayerCaptionsPresenter captionsPresenterElement = base.CaptionsPresenterElement as VideoPlayerCaptionsPresenter;
                    if (action == null)
                    {
                        action = i => i.UpdateCaptionsTWC(base.RelativeMediaPluginPosition, this.seekInProgress);
                    }
                    captionsPresenterElement.IfNotNull<VideoPlayerCaptionsPresenter>(action);
                }
            }
            finally
            {
                this.seekInProgress = false;
            }
        }

        private void VideoPlayer_LogEntryReceived(object sender, CustomEventArgs<LogEntry> e)
        {
            if (e.Value.Severity == Microsoft.SilverlightMediaFramework.Plugins.Primitives.LogLevel.Error)
            {
                this._lastError = e.Value.Message;
            }
        }

        private void VideoPlayer_PlaybackPositionChanged(object sender, CustomEventArgs<TimeSpan> e)
        {
            this._seekToLiveTimes.RemoveAll(t => t < DateTime.Now.AddSeconds(-90.0));
            SmoothStreamingMediaElement visualElement = (SmoothStreamingMediaElement) base.ActiveMediaPlugin.VisualElement;
            if ((!this._seekingToLive && (visualElement.Position != TimeSpan.Zero)) && ((visualElement.Position < visualElement.StartPosition) && !this.SeekToLiveOccurredRecently()))
            {
                if (this.SeekToLiveFrequencyExceeded())
                {
                    this.Stop();
                    this.OnError(MessageText.PerformanceErrorMessage, "Max seek-to-live frequency exceeded.");
                }
                this._seekingToLive = true;
                this._seekToLiveTimes.Add(DateTime.Now);
                base.SeekToLive();
                this._seekingToLive = false;
            }
        }

        private void VideoPlayer_PlaylistItemChanged(object sender, CustomEventArgs<PlaylistItem> e)
        {
            this._playbackStartTime = null;
            this._seekToLiveTimes.Clear();
        }

        private void VideoPlayer_PlayStateChanged(object sender, CustomEventArgs<MediaPluginState> e)
        {
            if ((((MediaPluginState) e.Value) == MediaPluginState.Playing) && !this._playbackStartTime.HasValue)
            {
                this._playbackStartTime = new DateTime?(DateTime.Now);
            }
            if (((((MediaPluginState) e.Value) == MediaPluginState.Stopped) || (((MediaPluginState) e.Value) == MediaPluginState.Closed)) && (this.SSME != null))
            {
                this.SSME.ChunkDownloadFailed -= new EventHandler<ChunkDownloadedEventArgs>(this.SSME_ChunkDownloadFailed);
            }
            if (((MediaPluginState) e.Value) == MediaPluginState.Closed)
            {
                this.OnError(MessageText.Default, this._lastError);
            }
        }

        public bool IsFreewheelEnabled
        {
            get
            {
                return (bool) base.GetValue(IsFreewheelEnabledProperty);
            }
            set
            {
                base.SetValue(IsFreewheelEnabledProperty, value);
            }
        }

        public SmoothStreamingMediaElement SSME
        {
            get
            {
                return (base.ActiveMediaPlugin.VisualElement as SmoothStreamingMediaElement);
            }
        }

        public Microsoft.SilverlightMediaFramework.Core.Timeline Timeline
        {
            get
            {
                return base.TimelineElement;
            }
        }
    }
}

