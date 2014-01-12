namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class OutOfHomeStatusChangedMessage
    {
        public OutOfHomeStatusChangedMessage(bool isOutOfHome, bool isOutOfCountry)
        {
            this.IsOutOfHome = isOutOfHome;
            this.IsOutOfCountry = isOutOfCountry;
        }

        public bool IsOutOfCountry { get; set; }

        public bool IsOutOfHome { get; set; }
    }
}

