namespace TWC.OVP.Models
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class EpisodeData
    {
        [DataMember(Name="result")]
        public EpisodeResult Result { get; set; }
    }
}

