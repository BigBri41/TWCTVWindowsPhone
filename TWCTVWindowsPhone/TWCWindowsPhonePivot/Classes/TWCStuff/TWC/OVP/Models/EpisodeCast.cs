namespace TWC.OVP.Models
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class EpisodeCast
    {
        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name="role")]
        public string Role { get; set; }
    }
}

