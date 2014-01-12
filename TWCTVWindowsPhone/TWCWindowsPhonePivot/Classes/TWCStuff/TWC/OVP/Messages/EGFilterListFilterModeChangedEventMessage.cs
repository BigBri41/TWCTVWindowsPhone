namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class EGFilterListFilterModeChangedEventMessage
    {
        public EGFilterListFilterModeChangedEventMessage(string filterMode)
        {
            this.FilterMode = filterMode;
        }

        public string FilterMode { get; set; }
    }
}

