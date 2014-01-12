namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class ChannelBrowserScrollMessage
    {
        public ChannelBrowserScrollMessage(int value)
        {
            this.Value = value;
        }

        public int Value { get; set; }
    }
}

