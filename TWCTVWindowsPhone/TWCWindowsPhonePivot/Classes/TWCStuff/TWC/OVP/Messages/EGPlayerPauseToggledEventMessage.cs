namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class EGPlayerPauseToggledEventMessage
    {
        public EGPlayerPauseToggledEventMessage(bool isPaused, string triggeredBy)
        {
            this.IsPaused = isPaused;
            this.TriggeredBy = triggeredBy;
        }

        public bool IsPaused { get; set; }

        public string TriggeredBy { get; set; }
    }
}

