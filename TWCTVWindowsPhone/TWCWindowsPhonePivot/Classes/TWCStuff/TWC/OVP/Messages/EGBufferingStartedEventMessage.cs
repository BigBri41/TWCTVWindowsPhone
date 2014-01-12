namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class EGBufferingStartedEventMessage
    {
        public EGBufferingStartedEventMessage(double playbackResumeTimestamp)
        {
            this.PlaybackResumeTimestamp = playbackResumeTimestamp;
        }

        public double PlaybackResumeTimestamp { get; set; }
    }
}

