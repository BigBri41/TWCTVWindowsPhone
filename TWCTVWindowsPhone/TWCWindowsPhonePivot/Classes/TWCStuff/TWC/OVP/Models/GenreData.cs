namespace TWC.OVP.Models
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class GenreData
    {
        [DataMember(Name="genres")]
        public TWC.OVP.Models.Genres Genres { get; set; }
    }
}

