namespace TWC.OVP.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class Smooth
    {
        [DataMember(Name="flags")]
        public List<string> Flags { get; set; }

        [DataMember(Name="uri")]
        public string Uri { get; set; }
    }
}

