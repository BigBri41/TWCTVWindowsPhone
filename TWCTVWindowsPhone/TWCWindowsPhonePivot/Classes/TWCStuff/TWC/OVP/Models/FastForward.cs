namespace TWC.OVP.Models
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class FastForward
    {
        [DataMember(Name="end")]
        public int End { get; set; }

        [DataMember(Name="start")]
        public int Start { get; set; }
    }
}

