namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class StartOnDemandStreamMessage
    {
        public StartOnDemandStreamMessage(string assetJson, int startTime)
        {
            this.AssetJson = assetJson;
            this.StartTime = startTime;
        }

        public string AssetJson { get; set; }

        public int StartTime { get; set; }
    }
}

