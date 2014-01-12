namespace TWC.OVP
{
    using Caliburn.Micro;
    using Microsoft.SilverlightMediaFramework.Plugins.Primitives;
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows.Browser;
    using TWC.OVP.Framework;
    using TWC.OVP.Messages;

    public class ScriptBridge : IHandle<PlaybackPositionChangedMessage>, IHandle<PlayStateChangedMessage>, IHandle<ChannelBrowserScrollMessage>, IHandle<UserInteractionMessage>, IHandle<EGStreamScrubbedMessage>, IHandle<EGAdStoppedEventMessage>, IHandle<EGBitRateChangedEventMessage>, IHandle<EGBufferingStartedEventMessage>, IHandle<EGBufferingEndedEventMessage>, IHandle<EGClosedCaptioningToggledEventMessage>, IHandle<EGInfoOverlayToggledEventMessage>, IHandle<EGPlaybackStartedEventMessage>, IHandle<EGPlaybackStoppedEventMessage>, IHandle<EGPlayerMuteToggledEventMessage>, IHandle<EGPlayerPauseToggledEventMessage>, IHandle<EGPlayerVolumeChangedEventMessage>, IHandle<EGStreamFailedEventMessage>, IHandle<EGStreamURIObtainedEventMessage>, IHandle<EGViewModeChangedEventMessage>, IHandle<EGChannelListChannelSelectedEventMessage>, IHandle<EGChannelListScrolledEventMessage>, IHandle<EGFilterListFilterModeChangedEventMessage>, IHandle<EGPlaybackRestartedEventMessage>, IHandle<EGContentPlayedEventMessage>, IHandle<MediaEndedEventMessage>, IHandle<FocusRequiredEventMessage>, IHandle<RSNStreamStartedEventMessage>, IHandle<RSNPlayerBeaconEventMessage>, IHandle<LocationChangedEventMessage>, IHandle<LocationChangeAcknowledgedEventMessage>, IHandle<EGServiceErrorOccured>, IHandle<EGUserMessageDisplayed>, IHandle
    {
        private IEventAggregator _eventAggregator;
        private string _lastUserInteraction = "";
        private int previousPlaybackPosition;

        [ScriptableMember]
        public event EventHandler AdStarted;

        [ScriptableMember]
        public event EventHandler<AdStoppedEventArgs> AdStopped;

        [ScriptableMember]
        public event EventHandler<BitRateChangedEventArgs> BitRateChanged;

        [ScriptableMember]
        public event EventHandler<BufferingEventArgs> BufferingEnded;

        [ScriptableMember]
        public event EventHandler<BufferingEventArgs> BufferingStarted;

        [ScriptableMember]
        public event EventHandler<ChannelListChannelSelectedEventArgs> ChannelListChannelSelected;

        [ScriptableMember]
        public event EventHandler<ChannelListScrolledEventArgs> ChannelListScrolled;

        [ScriptableMember]
        public event EventHandler<ToggledEventArgs> ClosedCaptioningToggled;

        [ScriptableMember]
        public event EventHandler<ContentPlayedEventArgs> ContentPlayed;

        [ScriptableMember]
        public event EventHandler<FilterListFilterModeChangedEventArgs> FilterListFilterModeChanged;

        [ScriptableMember]
        public event EventHandler FocusRequired;

        [ScriptableMember]
        public event EventHandler<ToggledEventArgs> InfoOverlayToggled;

        [ScriptableMember]
        public event EventHandler<LocationChangeAcknowledgedEventArgs> LocationChangeAcknowledged;

        [ScriptableMember]
        public event EventHandler<LocationChangedEventArgs> LocationChanged;

        [ScriptableMember]
        public event EventHandler<PlaybackRestartedEventArgs> PlaybackRestarted;

        [ScriptableMember]
        public event EventHandler<PlaybackStartedEventArgs> PlaybackStarted;

        [ScriptableMember]
        public event EventHandler<PlaybackStoppedEventArgs> PlaybackStopped;

        [ScriptableMember]
        public event EventHandler<PlayerMuteToggledEventArgs> PlayerMuteToggled;

        [ScriptableMember]
        public event EventHandler<PlayerPauseToggledEventArgs> PlayerPauseToggled;

        [ScriptableMember]
        public event EventHandler<PlayerVolumeChangedEventArgs> PlayerVolumeChanged;

        [ScriptableMember]
        public event EventHandler<ServiceErrorOccuredEventArgs> ServiceErrorOccured;

        [ScriptableMember]
        public event EventHandler<StreamFailedEventArgs> StreamFailed;

        [ScriptableMember]
        public event EventHandler<StreamScrubbedEventArgs> StreamScrubbed;

        [ScriptableMember]
        public event EventHandler<StreamURIObtainedEventArgs> StreamURIObtained;

        [ScriptableMember]
        public event EventHandler<UserMessageDisplayedEventArgs> UserMessageDisplayed;

        [ScriptableMember]
        public event EventHandler<ViewModeChangedEventArgs> ViewModeChanged;

        public ScriptBridge(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
            this._eventAggregator.Subscribe(this);
        }

        public static object EvalJs(string js)
        {
            js = string.Format("try{{ {0} }} catch(ex){{}}", js);
            js = js.Replace(Environment.NewLine, @"\n");
            return HtmlPage.Window.Eval(js);
        }

        public void Handle(ChannelBrowserScrollMessage message)
        {
            this.OnChannelListScrolled(this, new ChannelListScrolledEventArgs(message.Value));
        }

        public void Handle(EGAdStoppedEventMessage message)
        {
            this.OnAdStopped(this, new AdStoppedEventArgs(message.TriggeredBy));
        }

        public void Handle(EGBitRateChangedEventMessage message)
        {
            this.OnBitRateChanged(this, new BitRateChangedEventArgs(message.NewBitRate));
        }

        public void Handle(EGBufferingEndedEventMessage message)
        {
            this.OnBufferingEnded(this, new BufferingEventArgs(message.PlaybackResumeTimestamp));
        }

        public void Handle(EGBufferingStartedEventMessage message)
        {
            this.OnBufferingStarted(this, new BufferingEventArgs(message.PlaybackResumeTimestamp));
        }

        public void Handle(EGChannelListChannelSelectedEventMessage message)
        {
            this.OnChannelListChannelSelected(this, new ChannelListChannelSelectedEventArgs(message.ChannelIDType, message.ChannelID, message.TriggeredBy));
        }

        public void Handle(EGChannelListScrolledEventMessage message)
        {
            this.OnChannelListScrolled(this, new ChannelListScrolledEventArgs(message.Index));
        }

        public void Handle(EGClosedCaptioningToggledEventMessage message)
        {
            this.OnClosedCaptioningToggled(this, new ToggledEventArgs(message.Enabled));
        }

        public void Handle(EGContentPlayedEventMessage message)
        {
            this.OnContentPlayed(this, new ContentPlayedEventArgs(message.CurrentPlaybackTimestamp, message.Volume, message.IsMuted, message.TriggeredBy));
        }

        public void Handle(EGFilterListFilterModeChangedEventMessage message)
        {
            this.OnFilterListFilterModeChanged(this, new FilterListFilterModeChangedEventArgs(message.FilterMode));
        }

        public void Handle(EGInfoOverlayToggledEventMessage message)
        {
            this.OnInfoOverlayToggled(this, new ToggledEventArgs(message.Enabled));
        }

        public void Handle(EGPlaybackRestartedEventMessage message)
        {
            this.OnPlaybackRestarted(this, new PlaybackRestartedEventArgs(message.CurrentPlaybackTimestamp, message.Volume, message.IsMuted));
        }

        public void Handle(EGPlaybackStartedEventMessage message)
        {
            this.OnPlaybackStarted(this, new PlaybackStartedEventArgs(message.Volume, message.IsMuted));
        }

        public void Handle(EGPlaybackStoppedEventMessage message)
        {
            this.OnPlaybackStopped(this, new PlaybackStoppedEventArgs(message.TriggeredBy, message.CurrentPlaybackTimestamp));
        }

        public void Handle(EGPlayerMuteToggledEventMessage message)
        {
            this.OnPlayerMuteToggled(this, new PlayerMuteToggledEventArgs(message.IsMuted));
        }

        public void Handle(EGPlayerPauseToggledEventMessage message)
        {
            this.OnPlayerPauseToggled(this, new PlayerPauseToggledEventArgs(message.IsPaused, message.TriggeredBy));
        }

        public void Handle(EGPlayerVolumeChangedEventMessage message)
        {
            this.OnPlayerVolumeChanged(this, new PlayerVolumeChangedEventArgs(message.NewVolumeLevel, message.IsMuted));
        }

        public void Handle(EGServiceErrorOccured message)
        {
            this.OnServiceErrorOccured(this, new ServiceErrorOccuredEventArgs(message.HTTPStatusCode, message.ServicesURI, message.RequestParameters, message.ResponseBody, message.ImpactedCapability, message.IsRequestTimeout, message.IsUnexpectedResponse));
        }

        public void Handle(EGStreamFailedEventMessage message)
        {
            this.OnStreamFailed(this, new StreamFailedEventArgs(message.ErrorType, message.LastBitRate));
        }

        public void Handle(EGStreamScrubbedMessage message)
        {
            this.OnStreamScrubbed(this, new StreamScrubbedEventArgs(message.ScrubEnd, message.Blocked, message.InteractionType));
        }

        public void Handle(EGStreamURIObtainedEventMessage message)
        {
            this.OnStreamURIObtained(this, new StreamURIObtainedEventArgs(message.StreamURI, message.ScrubbingEnabled));
        }

        public void Handle(EGUserMessageDisplayed message)
        {
            this.OnUserMessageDisplayed(this, new UserMessageDisplayedEventArgs(message.Message, message.DisplayType));
        }

        public void Handle(EGViewModeChangedEventMessage message)
        {
            this.OnViewModeChanged(this, new ViewModeChangedEventArgs(message.IsFullScreen));
        }

        public void Handle(FocusRequiredEventMessage message)
        {
            this.OnFocusRequired(this, new EventArgs());
        }

        public void Handle(LocationChangeAcknowledgedEventMessage message)
        {
            this.OnLocationChangeAcknowledged(this, new LocationChangeAcknowledgedEventArgs(message.IsOutOfHome, message.IsOutOfCountry, message.IsCurrentStreamAvailableOutOfHome, message.IsHavingAvailableChannel));
        }

        public void Handle(LocationChangedEventMessage message)
        {
            this.OnLocationChanged(this, new LocationChangedEventArgs(message.IsOutOfHome, message.IsOutOfCountry, message.IsCurrentStreamAvailableOutOfHome));
        }

        public void Handle(MediaEndedEventMessage message)
        {
            this.NotifyMediaEnded();
        }

        public void Handle(PlaybackPositionChangedMessage message)
        {
            this.NotifyPlaybackPositionChanged(message.PlaybackPosition, message.EndPosition, message.MediaPath);
        }

        public void Handle(PlayStateChangedMessage message)
        {
            this.NotifyPlayStateChanged(message.PlayState, message.StateChangeCause);
        }

        public void Handle(RSNPlayerBeaconEventMessage message)
        {
            this.NotifyPlayerBeacon(message.Args);
        }

        public void Handle(RSNStreamStartedEventMessage message)
        {
            this.NotifyStreamStarted(message.Args);
        }

        public void Handle(UserInteractionMessage message)
        {
            this._lastUserInteraction = message.UserAction;
        }

        public void NotifyMediaEnded()
        {
            string js = string.Format("playerMediaEnded()", new object[0]);
            try
            {
                EvalJs(js);
            }
            catch (Exception)
            {
            }
        }

        private void NotifyPlaybackPositionChanged(TimeSpan playbackPosition, TimeSpan endPosition, string mediaPath)
        {
            if ((playbackPosition != TimeSpan.Zero) && (((int) playbackPosition.TotalSeconds) != this.previousPlaybackPosition))
            {
                this.previousPlaybackPosition = (int) playbackPosition.TotalSeconds;
                string js = string.Format("playerPositionUpdated('{0}', {1}, {2});", mediaPath, (int) endPosition.TotalSeconds, (int) playbackPosition.TotalSeconds);
                try
                {
                    EvalJs(js);
                }
                catch (Exception)
                {
                }
            }
        }

        public void NotifyPlayerBeacon(params object[] values)
        {
            string js = string.Format("playerBeacon({0},\"{1}\",\"{2}\",\"{3:yyyyMMddHHmm}\",\"{4}\",{5}, \"{6}\");", values);
            try
            {
                EvalJs(js);
            }
            catch (Exception)
            {
            }
        }

        public void NotifyPlayStateChanged(MediaPluginState mediaPluginState, string changeCause)
        {
            string js = string.Format("playerMediaStateChanged('{0}','{1}','{2}');", mediaPluginState.ToString(), changeCause, this._lastUserInteraction);
            this._lastUserInteraction = "";
            try
            {
                EvalJs(js);
            }
            catch (Exception)
            {
            }
        }

        public void NotifyStreamStarted(params object[] args)
        {
            string js = string.Format("playerStreamStarted({0},\"{1}\",\"{2}\",\"{3:yyyyMMddHHmm}\",\"{4}\",{5}, \"{6}\");", args);
            try
            {
                EvalJs(js);
            }
            catch (Exception)
            {
            }
        }

        private void OnAdStarted(object sender, EventArgs e)
        {
            if (this.AdStarted != null)
            {
                this.AdStarted(sender, e);
            }
        }

        private void OnAdStopped(object sender, AdStoppedEventArgs e)
        {
            if (this.AdStopped != null)
            {
                this.AdStopped(sender, e);
            }
        }

        public void OnAppLoaded()
        {
            EvalJs("onApplicationLoaded();");
        }

        private void OnBitRateChanged(object sender, BitRateChangedEventArgs e)
        {
            if (this.BitRateChanged != null)
            {
                this.BitRateChanged(sender, e);
            }
        }

        private void OnBufferingEnded(object sender, BufferingEventArgs e)
        {
            if (this.BufferingEnded != null)
            {
                this.BufferingEnded(sender, e);
            }
        }

        private void OnBufferingStarted(object sender, BufferingEventArgs e)
        {
            if (this.BufferingStarted != null)
            {
                this.BufferingStarted(sender, e);
            }
        }

        private void OnChannelListChannelSelected(object sender, ChannelListChannelSelectedEventArgs e)
        {
            if (this.ChannelListChannelSelected != null)
            {
                this.ChannelListChannelSelected(sender, e);
            }
        }

        private void OnChannelListScrolled(object sender, ChannelListScrolledEventArgs e)
        {
            if (this.ChannelListScrolled != null)
            {
                this.ChannelListScrolled(sender, e);
            }
        }

        private void OnClosedCaptioningToggled(object sender, ToggledEventArgs e)
        {
            if (this.ClosedCaptioningToggled != null)
            {
                this.ClosedCaptioningToggled(sender, e);
            }
        }

        private void OnContentPlayed(object sender, ContentPlayedEventArgs e)
        {
            if (this.ContentPlayed != null)
            {
                this.ContentPlayed(sender, e);
            }
        }

        private void OnFilterListFilterModeChanged(object sender, FilterListFilterModeChangedEventArgs e)
        {
            if (this.FilterListFilterModeChanged != null)
            {
                this.FilterListFilterModeChanged(sender, e);
            }
        }

        private void OnFocusRequired(object sender, EventArgs e)
        {
            if (this.FocusRequired != null)
            {
                this.FocusRequired(sender, e);
            }
        }

        private void OnInfoOverlayToggled(object sender, ToggledEventArgs e)
        {
            if (this.InfoOverlayToggled != null)
            {
                this.InfoOverlayToggled(sender, e);
            }
        }

        private void OnLocationChangeAcknowledged(object sender, LocationChangeAcknowledgedEventArgs e)
        {
            if (this.LocationChangeAcknowledged != null)
            {
                this.LocationChangeAcknowledged(sender, e);
            }
        }

        private void OnLocationChanged(object sender, LocationChangedEventArgs e)
        {
            if (this.LocationChanged != null)
            {
                this.LocationChanged(sender, e);
            }
        }

        private void OnPlaybackRestarted(object sender, PlaybackRestartedEventArgs e)
        {
            if (this.PlaybackRestarted != null)
            {
                this.PlaybackRestarted(sender, e);
            }
        }

        private void OnPlaybackStarted(object sender, PlaybackStartedEventArgs e)
        {
            if (this.PlaybackStarted != null)
            {
                this.PlaybackStarted(sender, e);
            }
        }

        private void OnPlaybackStopped(object sender, PlaybackStoppedEventArgs e)
        {
            if (this.PlaybackStopped != null)
            {
                this.PlaybackStopped(sender, e);
            }
        }

        private void OnPlayerMuteToggled(object sender, PlayerMuteToggledEventArgs e)
        {
            if (this.PlayerMuteToggled != null)
            {
                this.PlayerMuteToggled(sender, e);
            }
        }

        private void OnPlayerPauseToggled(object sender, PlayerPauseToggledEventArgs e)
        {
            if (this.PlayerPauseToggled != null)
            {
                this.PlayerPauseToggled(sender, e);
            }
        }

        private void OnPlayerVolumeChanged(object sender, PlayerVolumeChangedEventArgs e)
        {
            if (this.PlayerVolumeChanged != null)
            {
                this.PlayerVolumeChanged(sender, e);
            }
        }

        private void OnServiceErrorOccured(object sender, ServiceErrorOccuredEventArgs e)
        {
            if (this.ServiceErrorOccured != null)
            {
                this.ServiceErrorOccured(sender, e);
            }
        }

        private void OnStreamFailed(object sender, StreamFailedEventArgs e)
        {
            if (this.StreamFailed != null)
            {
                this.StreamFailed(sender, e);
            }
        }

        private void OnStreamScrubbed(object sender, StreamScrubbedEventArgs e)
        {
            if (this.StreamScrubbed != null)
            {
                this.StreamScrubbed(sender, e);
            }
        }

        private void OnStreamURIObtained(object sender, StreamURIObtainedEventArgs e)
        {
            if (this.StreamURIObtained != null)
            {
                this.StreamURIObtained(sender, e);
            }
        }

        private void OnUserMessageDisplayed(object sender, UserMessageDisplayedEventArgs e)
        {
            if (this.UserMessageDisplayed != null)
            {
                this.UserMessageDisplayed(sender, e);
            }
        }

        private void OnViewModeChanged(object sender, ViewModeChangedEventArgs e)
        {
            if (this.ViewModeChanged != null)
            {
                this.ViewModeChanged(sender, e);
            }
        }

        public static void PageConsoleLog(string text)
        {
        }

        [ScriptableMember]
        public void PlayAsset(string assetJson)
        {
            this.PlayAsset(assetJson, 0);
        }

        [ScriptableMember]
        public void PlayAsset(string assetJson, int startTime)
        {
            try
            {
                this._eventAggregator.Publish(new StartOnDemandStreamMessage(assetJson, startTime));
            }
            catch (Exception exception)
            {
                Trace.WriteLine(string.Format("error during ProcessAsset: {0}", exception));
            }
        }

        [ScriptableMember]
        public void PlayLiveStream(string streamUrl)
        {
            try
            {
                this._eventAggregator.Publish(new StartLiveStreamMessage(streamUrl));
            }
            catch (Exception exception)
            {
                Trace.WriteLine(string.Format("error during PlayChannel: {0}", exception));
            }
        }

        [ScriptableMember]
        public void PlayTestVideo()
        {
            string assetJson = "{\"type\":\"event\",\"details\":{\"actors\":[\"Joel McHale\",\"Gillian Jacobs\"],\"available_date\":\"2013-02-11T00:00:00.000-05:00\",\"closed_captioned\":true,\"director\":\"\",\"display_runtime\":\"23:00\",\"episode_number\":1,\"expiration_date\":\"2013-03-14T23:59:59.000-04:00\",\"genres\":[{\"name\":\"Entertainment\"}],\"long_desc\":\"Dean Pelton devises a way for students to compete for class space; Abed stresses over the thought of the study group breaking up after graduation.\",\"num_assets\":1,\"original_air_date\":\"2013-02-15\",\"original_network_name\":\"\",\"rating\":\"TV-PG\",\"season_number\":4,\"short_desc\":\"Dean Pelton devises a way for students to compete for class space; Abed stresses over the thought of the study group breaking up after graduation.\",\"year\":2012},\"id\":17983555938,\"image_uri\":\"http://services.timewarnercable.com/imageserver/program/EP011541620075?productId=PTOD&providerId=NBC_NETSHOWS_HD&wayfarer=fuOO4zSBNu3SpxcMqJjyl%2BjjSokSHpOXc3ycfOayGCGZyMMQu7UFlhJZJqHNFgIe76QRV1u0xy7wTsp5OkSZM56bZj8yWr1HtaTyEmqmC0OP3U7z%2FO%2BletY7o03QIbjTlorVdt%2FlCfiD4gL%2Bhnmy3kb1UjVT2WBO7yg7GvcwFiBDBY9c094cNzOTZsjfDmtrH6%2Fz64s%2FnlBu9YiThz29gxs8vjsSW1iInlZMBoCwAZ3Dql7sSyjy0g%3D%3D\",\"network\":{\"type\":\"category_list\",\"callsign\":\"WCNCDT\",\"id\":688195591,\"image_uri\":\"http://services.timewarnercable.com/imageserver/image/default?productId=PTOD&providerId=NBCHD&wayfarer=fuOO4zSBNu3SpxcMqJjyl%2BjjSokSHpOXc3ycfOayGCGZyMMQu7UFlhJZJqHNFgIe76QRV1u0xy7wTsp5OkSZM56bZj8yWr1HtaTyEmqmC0OP3U7z%2FO%2BletY7o03QIbjTlorVdt%2FlCfiD4gL%2Bhnmy3kb1UjVT2WBO7yg7GvcwFiBDBY9c094cNzOTZsjfDmtrH6%2Fz64s%2FnlBu9YiThz29gxs8vjsSW1iInlZMBoCwAZ3Dql7sSyjy0g%3D%3D\",\"name\":\"NBC\",\"num_categories\":33,\"product_provider\":\"PTOD:NBC_NETSHOWS_HD\",\"product_providers\":[\"PTOD:NBC_NETSHOWS_HD\",\"PTOD:NBCHD\"]},\"nmd_main_uri\":\"https://services.timewarnercable.com/nmd/v3/program/tms/EP011541620075\",\"streams\":{\"hls\":{\"flags\":[\"stereo\",\"standard_format\",\"hd\",\"movie\",\"mezzanine\",\"drm_none\"],\"uri\":\"/api/vod/v3/assets/17983555938/streams/baks/hls\"},\"smooth\":{\"flags\":[\"stereo\",\"standard_format\",\"hd\",\"movie\",\"mezzanine\",\"drm_none\"],\"uri\":\"https://services.timewarnercable.com/api/vod/v3/assets/17983555938/streams/baks/smooth\"}},\"title\":\"History 101\",\"tricks_mode\":{\"FASTFORWARD\":[{\"end\":1380,\"start\":0}]},\"uri\":\"/api/vod/v3/assets/17983555938\",\"startTime\":14,\"percent_finished\":0.9900990099009901,\"isBlocked\":false,\"isEntitled\":true,\"series_title\":\"Community\"}";
            this._eventAggregator.Publish(new StartOnDemandStreamMessage(assetJson, 0));
        }

        [ScriptableMember]
        public void SelectChannel(string tmsId)
        {
            try
            {
                this._eventAggregator.Publish(new SelectChannelMessage(tmsId));
            }
            catch (Exception exception)
            {
                Trace.WriteLine(string.Format("error during PlayChannel: {0}", exception));
            }
        }

        [ScriptableMember]
        public void TogglePlay()
        {
            this._eventAggregator.Publish(new TogglePlayMessage());
        }

        [ScriptableType]
        public class AdStoppedEventArgs : EventArgs
        {
            public AdStoppedEventArgs(string triggeredBy)
            {
                this.TriggeredBy = triggeredBy;
            }

            public string TriggeredBy { get; set; }
        }

        [ScriptableType]
        public class BitRateChangedEventArgs : EventArgs
        {
            public BitRateChangedEventArgs(long newBitRate)
            {
                this.NewBitRate = newBitRate;
            }

            public long NewBitRate { get; set; }
        }

        [ScriptableType]
        public class BufferingEventArgs : EventArgs
        {
            public BufferingEventArgs(double playbackResumeTimestamp)
            {
                this.PlaybackResumeTimestamp = playbackResumeTimestamp;
            }

            public double PlaybackResumeTimestamp { get; set; }
        }

        [ScriptableType]
        public class ChannelListChannelSelectedEventArgs : EventArgs
        {
            public ChannelListChannelSelectedEventArgs(string channelIDType, string channelID, string triggeredBy)
            {
                this.ChannelIDType = channelIDType;
                this.ChannelID = channelID;
                this.TriggeredBy = triggeredBy;
            }

            public string ChannelID { get; set; }

            public string ChannelIDType { get; set; }

            public string TriggeredBy { get; set; }
        }

        [ScriptableType]
        public class ChannelListScrolledEventArgs : EventArgs
        {
            public ChannelListScrolledEventArgs(int index)
            {
                this.Index = index;
            }

            public int Index { get; set; }
        }

        [ScriptableType]
        public class ContentPlayedEventArgs : EventArgs
        {
            public ContentPlayedEventArgs(double currentPlaybackTimestamp, double volume, bool isMuted, string triggeredBy)
            {
                this.CurrentPlaybackTimestamp = currentPlaybackTimestamp;
                this.Volume = volume;
                this.IsMuted = isMuted;
                this.TriggeredBy = triggeredBy;
            }

            public double CurrentPlaybackTimestamp { get; set; }

            public bool IsMuted { get; set; }

            public string TriggeredBy { get; set; }

            public double Volume { get; set; }
        }

        [ScriptableType]
        public class FilterListFilterModeChangedEventArgs : EventArgs
        {
            public FilterListFilterModeChangedEventArgs(string filterMode)
            {
                this.FilterMode = filterMode;
            }

            public string FilterMode { get; set; }
        }

        [ScriptableType]
        public class LocationChangeAcknowledgedEventArgs : EventArgs
        {
            public LocationChangeAcknowledgedEventArgs(bool isOutOfHome, bool isOutOfCountry, bool isCurrentStreamAvailableOutOfHome, bool isHavingAvailableChannel)
            {
                this.IsOutOfHome = isOutOfHome;
                this.IsOutOfCountry = isOutOfCountry;
                this.IsCurrentStreamAvailableOutOfHome = isCurrentStreamAvailableOutOfHome;
                this.IsHavingAvailableChannel = isHavingAvailableChannel;
            }

            public bool IsCurrentStreamAvailableOutOfHome { get; set; }

            public bool IsHavingAvailableChannel { get; set; }

            public bool IsOutOfCountry { get; set; }

            public bool IsOutOfHome { get; set; }
        }

        [ScriptableType]
        public class LocationChangedEventArgs : EventArgs
        {
            public LocationChangedEventArgs(bool isOutOfHome, bool isOutOfCountry, bool isCurrentStreamAvailableOutOfHome)
            {
                this.IsOutOfHome = isOutOfHome;
                this.IsOutOfCountry = isOutOfCountry;
                this.IsCurrentStreamAvailableOutOfHome = isCurrentStreamAvailableOutOfHome;
            }

            public bool IsCurrentStreamAvailableOutOfHome { get; set; }

            public bool IsOutOfCountry { get; set; }

            public bool IsOutOfHome { get; set; }
        }

        [ScriptableType]
        public class PlaybackRestartedEventArgs : EventArgs
        {
            public PlaybackRestartedEventArgs(double currentPlaybackTimestamp, double volume, bool isMuted)
            {
                this.CurrentPlaybackTimestamp = currentPlaybackTimestamp;
                this.Volume = volume;
                this.IsMuted = isMuted;
            }

            public double CurrentPlaybackTimestamp { get; set; }

            public bool IsMuted { get; set; }

            public double Volume { get; set; }
        }

        [ScriptableType]
        public class PlaybackStartedEventArgs : EventArgs
        {
            public PlaybackStartedEventArgs(double volume, bool isMuted)
            {
                this.Volume = volume;
                this.IsMuted = isMuted;
            }

            public bool IsMuted { get; set; }

            public double Volume { get; set; }
        }

        [ScriptableType]
        public class PlaybackStoppedEventArgs : EventArgs
        {
            public PlaybackStoppedEventArgs(string triggeredBy, double currentPlaybackTimestamp)
            {
                this.TriggeredBy = triggeredBy;
                this.CurrentPlaybackTimestamp = currentPlaybackTimestamp;
            }

            public double CurrentPlaybackTimestamp { get; set; }

            public string TriggeredBy { get; set; }
        }

        [ScriptableType]
        public class PlayerMuteToggledEventArgs : EventArgs
        {
            public PlayerMuteToggledEventArgs(bool isMuted)
            {
                this.IsMuted = isMuted;
            }

            public bool IsMuted { get; set; }

            public TimeSpan Position { get; set; }
        }

        [ScriptableType]
        public class PlayerPauseToggledEventArgs : EventArgs
        {
            public PlayerPauseToggledEventArgs(bool isPaused, string triggeredBy)
            {
                this.IsPaused = isPaused;
                this.TriggeredBy = triggeredBy;
            }

            public bool IsPaused { get; set; }

            public string TriggeredBy { get; set; }
        }

        [ScriptableType]
        public class PlayerVolumeChangedEventArgs : EventArgs
        {
            public PlayerVolumeChangedEventArgs(double newVolumeLevel, bool isMuted)
            {
                this.NewVolumeLevel = newVolumeLevel;
                this.IsMuted = isMuted;
            }

            public bool IsMuted { get; set; }

            public double NewVolumeLevel { get; set; }
        }

        [ScriptableType]
        public class ServiceErrorOccuredEventArgs : EventArgs
        {
            public ServiceErrorOccuredEventArgs(int httpStatusCode, string servicesURI, string requestParameters, string responseBody, string impactedCapability, bool isRequestTimeout, bool isUnexpectedResponse)
            {
                this.HTTPStatusCode = httpStatusCode;
                this.ServicesURI = servicesURI;
                this.RequestParameters = requestParameters;
                this.ResponseBody = responseBody;
                this.ImpactedCapability = impactedCapability;
                this.IsRequestTimeout = isRequestTimeout;
                this.IsUnexpectedResponse = isUnexpectedResponse;
            }

            public int HTTPStatusCode { get; set; }

            public string ImpactedCapability { get; set; }

            public bool IsRequestTimeout { get; set; }

            public bool IsUnexpectedResponse { get; set; }

            public string RequestParameters { get; set; }

            public string ResponseBody { get; set; }

            public string ServicesURI { get; set; }
        }

        [ScriptableType]
        public class StreamFailedEventArgs : EventArgs
        {
            public StreamFailedEventArgs(string errorType, string lastBitRate)
            {
                this.ErrorType = errorType;
                this.LastBitRate = lastBitRate;
            }

            public string ErrorType { get; set; }

            public string LastBitRate { get; set; }
        }

        [ScriptableType]
        public class StreamScrubbedEventArgs : EventArgs
        {
            public StreamScrubbedEventArgs(double scrubEnd, bool blocked, string interactionType)
            {
                this.ScrubEnd = scrubEnd;
                this.Blocked = blocked;
                this.InteractionType = interactionType;
            }

            public bool Blocked { get; set; }

            public string InteractionType { get; set; }

            public double ScrubEnd { get; set; }
        }

        [ScriptableType]
        public class StreamURIObtainedEventArgs : EventArgs
        {
            public StreamURIObtainedEventArgs(string streamURI, bool scrubbingEnabled)
            {
                this.StreamURI = streamURI;
                this.ScrubbingEnabled = scrubbingEnabled;
            }

            public bool ScrubbingEnabled { get; set; }

            public string StreamURI { get; set; }
        }

        [ScriptableType]
        public class ToggledEventArgs : EventArgs
        {
            public ToggledEventArgs(bool enabled)
            {
                this.Enabled = enabled;
            }

            public bool Enabled { get; set; }
        }

        [ScriptableType]
        public class UserMessageDisplayedEventArgs : EventArgs
        {
            public UserMessageDisplayedEventArgs(string message, string displayType)
            {
                this.Message = message;
                this.DisplayType = displayType;
            }

            public string DisplayType { get; set; }

            public string Message { get; set; }
        }

        [ScriptableType]
        public class ViewModeChangedEventArgs : EventArgs
        {
            public ViewModeChangedEventArgs(bool isFullScreen)
            {
                this.IsFullScreen = isFullScreen;
            }

            public bool IsFullScreen { get; set; }
        }
    }
}

