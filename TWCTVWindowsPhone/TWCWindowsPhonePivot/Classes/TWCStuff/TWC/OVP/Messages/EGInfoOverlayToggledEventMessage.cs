namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class EGInfoOverlayToggledEventMessage
    {
        public EGInfoOverlayToggledEventMessage(bool enabled)
        {
            this.Enabled = enabled;
        }

        public bool Enabled { get; set; }
    }
}

