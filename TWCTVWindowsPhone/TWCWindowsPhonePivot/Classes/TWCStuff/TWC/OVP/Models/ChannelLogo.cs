namespace TWC.OVP.Models
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class ChannelLogo
    {
        [DataMember(Name="dimensions")]
        public string Dimensions { get; set; }

        [DataMember(Name="url")]
        public string Url { get; set; }
    }
}

