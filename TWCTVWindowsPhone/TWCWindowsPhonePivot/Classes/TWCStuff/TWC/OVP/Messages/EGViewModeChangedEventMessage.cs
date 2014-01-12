namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class EGViewModeChangedEventMessage
    {
        public EGViewModeChangedEventMessage(bool isFullScreen)
        {
            this.IsFullScreen = isFullScreen;
        }

        public bool IsFullScreen { get; set; }
    }
}

