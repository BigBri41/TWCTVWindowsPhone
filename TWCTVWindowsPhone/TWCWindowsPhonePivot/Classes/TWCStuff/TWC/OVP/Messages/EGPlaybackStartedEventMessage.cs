namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class EGPlaybackStartedEventMessage
    {
        public EGPlaybackStartedEventMessage(double volume, bool isMuted)
        {
            this.Volume = volume;
            this.IsMuted = isMuted;
        }

        public bool IsMuted { get; set; }

        public double Volume { get; set; }
    }
}

