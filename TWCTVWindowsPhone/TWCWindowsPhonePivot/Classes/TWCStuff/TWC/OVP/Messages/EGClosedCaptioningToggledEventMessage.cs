namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class EGClosedCaptioningToggledEventMessage
    {
        public EGClosedCaptioningToggledEventMessage(bool enabled)
        {
            this.Enabled = enabled;
        }

        public bool Enabled { get; set; }
    }
}

