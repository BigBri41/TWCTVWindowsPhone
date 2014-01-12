namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class EGChannelListScrolledEventMessage
    {
        public EGChannelListScrolledEventMessage(int index)
        {
            this.Index = index;
        }

        public int Index { get; set; }
    }
}

