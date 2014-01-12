namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class EGStreamFailedEventMessage
    {
        public EGStreamFailedEventMessage(string errorType, string lastBitRate)
        {
            this.ErrorType = errorType;
            this.LastBitRate = lastBitRate;
        }

        public string ErrorType { get; set; }

        public string LastBitRate { get; set; }
    }
}

