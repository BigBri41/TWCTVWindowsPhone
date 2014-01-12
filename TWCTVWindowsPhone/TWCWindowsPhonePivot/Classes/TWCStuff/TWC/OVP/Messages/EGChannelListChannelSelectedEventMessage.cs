namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class EGChannelListChannelSelectedEventMessage
    {
        public EGChannelListChannelSelectedEventMessage(string channelIDType, string channelID, string triggeredBy)
        {
            this.ChannelIDType = channelIDType;
            this.ChannelID = channelID;
            this.TriggeredBy = triggeredBy;
        }

        public string ChannelID { get; set; }

        public string ChannelIDType { get; set; }

        public string TriggeredBy { get; set; }
    }
}

