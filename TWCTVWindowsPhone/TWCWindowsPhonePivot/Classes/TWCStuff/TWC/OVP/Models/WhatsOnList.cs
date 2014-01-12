namespace TWC.OVP.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class WhatsOnList
    {
        [DataMember(Name="items")]
        public List<WhatsOnItems> Items { get; set; }
    }
}

