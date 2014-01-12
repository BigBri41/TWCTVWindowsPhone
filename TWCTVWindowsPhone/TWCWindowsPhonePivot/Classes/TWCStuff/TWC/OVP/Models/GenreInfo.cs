namespace TWC.OVP.Models
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class GenreInfo
    {
        [DataMember(Name="enabled")]
        public bool IsEnabled { get; set; }

        [DataMember(Name="external")]
        public bool IsExternal { get; set; }

        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name="url")]
        public string Url { get; set; }
    }
}

