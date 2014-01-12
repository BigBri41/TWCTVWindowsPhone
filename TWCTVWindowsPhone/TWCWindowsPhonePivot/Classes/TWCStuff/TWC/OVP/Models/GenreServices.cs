namespace TWC.OVP.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class GenreServices
    {
        [DataMember(Name="service")]
        public List<GenreServiceInfo> ServiceInfos { get; set; }
    }
}

