namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class LocationChangedEventMessage
    {
        public LocationChangedEventMessage(bool isOutOfHome, bool isOutOfCountry, bool isCurrentStreamAvailableOutOfHome)
        {
            this.IsOutOfHome = isOutOfHome;
            this.IsOutOfCountry = isOutOfCountry;
            this.IsCurrentStreamAvailableOutOfHome = isCurrentStreamAvailableOutOfHome;
        }

        public bool IsCurrentStreamAvailableOutOfHome { get; set; }

        public bool IsOutOfCountry { get; set; }

        public bool IsOutOfHome { get; set; }
    }
}

