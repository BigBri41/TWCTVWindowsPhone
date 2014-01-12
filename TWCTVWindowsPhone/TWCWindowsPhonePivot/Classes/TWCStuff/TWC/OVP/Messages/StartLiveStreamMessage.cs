namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class StartLiveStreamMessage
    {
        public StartLiveStreamMessage(string streamUrl)
        {
            this.StreamUrl = streamUrl;
        }

        public string StreamUrl { get; set; }
    }
}

