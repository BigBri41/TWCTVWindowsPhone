namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class EGStreamURIObtainedEventMessage
    {
        public EGStreamURIObtainedEventMessage(string streamURI, bool scrubbingEnabled)
        {
            this.StreamURI = streamURI;
            this.ScrubbingEnabled = scrubbingEnabled;
        }

        public bool ScrubbingEnabled { get; set; }

        public string StreamURI { get; set; }
    }
}

