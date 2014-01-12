namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class EGPlaybackRestartedEventMessage
    {
        public EGPlaybackRestartedEventMessage(double currentPlaybackTimestamp, double volume, bool isMuted)
        {
            this.CurrentPlaybackTimestamp = currentPlaybackTimestamp;
            this.Volume = volume;
            this.IsMuted = isMuted;
        }

        public double CurrentPlaybackTimestamp { get; set; }

        public bool IsMuted { get; set; }

        public double Volume { get; set; }
    }
}

