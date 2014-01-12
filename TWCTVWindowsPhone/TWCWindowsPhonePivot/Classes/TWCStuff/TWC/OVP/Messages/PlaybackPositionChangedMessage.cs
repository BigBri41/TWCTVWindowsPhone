namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class PlaybackPositionChangedMessage
    {
        public PlaybackPositionChangedMessage(TimeSpan playbackPosition, TimeSpan endPosition, string mediaPath)
        {
            this.PlaybackPosition = playbackPosition;
            this.EndPosition = endPosition;
            this.MediaPath = mediaPath;
        }

        public TimeSpan EndPosition { get; set; }

        public string MediaPath { get; set; }

        public TimeSpan PlaybackPosition { get; set; }
    }
}

