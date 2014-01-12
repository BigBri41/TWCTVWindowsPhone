namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class EGPlayerVolumeChangedEventMessage
    {
        public EGPlayerVolumeChangedEventMessage(double newVolumeLevel, bool isMuted)
        {
            this.NewVolumeLevel = newVolumeLevel;
            this.IsMuted = isMuted;
        }

        public bool IsMuted { get; set; }

        public double NewVolumeLevel { get; set; }
    }
}

