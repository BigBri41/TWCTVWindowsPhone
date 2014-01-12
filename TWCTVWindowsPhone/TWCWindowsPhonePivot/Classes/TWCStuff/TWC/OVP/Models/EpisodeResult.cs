namespace TWC.OVP.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class EpisodeResult
    {
        [DataMember(Name="episodeDetails")]
        public List<EpisodeDetail> EpisodeDetails { get; set; }

        [DataMember(Name="error")]
        public bool Error { get; set; }

        [DataMember(Name="message")]
        public string Message { get; set; }
    }
}

