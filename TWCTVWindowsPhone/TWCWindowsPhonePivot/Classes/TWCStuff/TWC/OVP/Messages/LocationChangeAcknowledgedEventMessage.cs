namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class LocationChangeAcknowledgedEventMessage
    {
        public LocationChangeAcknowledgedEventMessage(bool isOutOfHome, bool isOutOfCountry, bool isCurrentStreamAvailableOutOfHome, bool isHavingAvailableChannel)
        {
            this.IsOutOfHome = isOutOfHome;
            this.IsOutOfCountry = isOutOfCountry;
            this.IsCurrentStreamAvailableOutOfHome = isCurrentStreamAvailableOutOfHome;
            this.IsHavingAvailableChannel = isHavingAvailableChannel;
        }

        public bool IsCurrentStreamAvailableOutOfHome { get; set; }

        public bool IsHavingAvailableChannel { get; set; }

        public bool IsOutOfCountry { get; set; }

        public bool IsOutOfHome { get; set; }
    }
}

