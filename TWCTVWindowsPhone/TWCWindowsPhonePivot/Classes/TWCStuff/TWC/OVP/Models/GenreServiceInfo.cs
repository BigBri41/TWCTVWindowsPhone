namespace TWC.OVP.Models
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class GenreServiceInfo
    {
        [DataMember(Name="genre")]
        public string Genre { get; set; }

        [DataMember(Name="index")]
        public string Index { get; set; }

        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name="provider")]
        public string Provider { get; set; }
    }
}

