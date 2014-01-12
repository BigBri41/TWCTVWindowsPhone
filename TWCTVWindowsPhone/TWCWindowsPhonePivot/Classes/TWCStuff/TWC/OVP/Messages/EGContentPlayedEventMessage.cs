namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class EGContentPlayedEventMessage
    {
        public EGContentPlayedEventMessage(double currentPlaybackTimestamp, double volume, bool isMuted, string triggeredBy)
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
}

