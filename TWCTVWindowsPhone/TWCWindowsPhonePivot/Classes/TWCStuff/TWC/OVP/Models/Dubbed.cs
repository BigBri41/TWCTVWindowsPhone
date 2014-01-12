namespace TWC.OVP.Models
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class Dubbed
    {
        [DataMember(Name="inuse")]
        public bool InUse { get; set; }

        [DataMember(Name="language")]
        public string Language { get; set; }
    }
}

