namespace TWC.OVP.Models
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class Streams
    {
        [DataMember(Name="hls")]
        public TWC.OVP.Models.Hls Hls { get; set; }

        [DataMember(Name="smooth")]
        public TWC.OVP.Models.Smooth Smooth { get; set; }
    }
}

