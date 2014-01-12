namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class SelectChannelMessage
    {
        public SelectChannelMessage(string tmsId)
        {
            this.TmsId = tmsId;
        }

        public string TmsId { get; set; }
    }
}

