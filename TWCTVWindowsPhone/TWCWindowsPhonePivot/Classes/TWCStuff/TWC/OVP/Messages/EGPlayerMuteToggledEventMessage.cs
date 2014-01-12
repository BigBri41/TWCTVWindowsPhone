namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class EGPlayerMuteToggledEventMessage
    {
        public EGPlayerMuteToggledEventMessage(bool isMuted)
        {
            this.IsMuted = isMuted;
        }

        public bool IsMuted { get; set; }

        public TimeSpan Position { get; set; }
    }
}

