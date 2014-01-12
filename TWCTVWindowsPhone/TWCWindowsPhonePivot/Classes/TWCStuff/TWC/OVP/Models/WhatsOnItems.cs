namespace TWC.OVP.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class WhatsOnItems
    {
        [DataMember(Name="item")]
        public List<WhatsOn> Items { get; set; }
    }
}

