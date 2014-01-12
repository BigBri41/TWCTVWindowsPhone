namespace TWC.OVP.Models
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class Episode
    {
        [DataMember(Name="data")]
        public EpisodeData Data { get; set; }

        [DataMember(Name="error")]
        public string Error { get; set; }
    }
}

