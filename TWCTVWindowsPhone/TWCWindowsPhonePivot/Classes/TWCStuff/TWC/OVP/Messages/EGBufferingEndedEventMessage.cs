namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class EGBufferingEndedEventMessage
    {
        public EGBufferingEndedEventMessage(double playbackResumeTimestamp)
        {
            this.PlaybackResumeTimestamp = playbackResumeTimestamp;
        }

        public double PlaybackResumeTimestamp { get; set; }
    }
}

