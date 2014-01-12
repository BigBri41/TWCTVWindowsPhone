namespace TWC.OVP.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class TricksMode
    {
        [DataMember(Name="FASTFORWARD")]
        public List<TWC.OVP.Models.FastForward> FastForward { get; set; }
    }
}

