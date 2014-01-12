namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class EGBitRateChangedEventMessage
    {
        public EGBitRateChangedEventMessage(long newBitRate)
        {
            this.NewBitRate = newBitRate;
        }

        public long NewBitRate { get; set; }
    }
}

