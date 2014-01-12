namespace TWC.OVP.Models
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class GenreServiceData
    {
        [DataMember(Name="services")]
        public GenreServices Services { get; set; }
    }
}

