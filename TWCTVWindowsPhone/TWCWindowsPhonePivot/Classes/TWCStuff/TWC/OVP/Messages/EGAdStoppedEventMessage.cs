namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class EGAdStoppedEventMessage
    {
        public EGAdStoppedEventMessage(string triggeredBy)
        {
            this.TriggeredBy = triggeredBy;
        }

        public string TriggeredBy { get; set; }
    }
}

