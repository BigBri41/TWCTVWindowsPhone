namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class EGPlaybackStoppedEventMessage
    {
        public EGPlaybackStoppedEventMessage(string triggeredBy, double currentPlaybackTimestamp)
        {
            this.TriggeredBy = triggeredBy;
            this.CurrentPlaybackTimestamp = currentPlaybackTimestamp;
        }

        public double CurrentPlaybackTimestamp { get; set; }

        public string TriggeredBy { get; set; }
    }
}

