namespace TWC.OVP.Models
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class Genre
    {
        [DataMember(Name="name")]
        public string Name { get; set; }
    }
}

