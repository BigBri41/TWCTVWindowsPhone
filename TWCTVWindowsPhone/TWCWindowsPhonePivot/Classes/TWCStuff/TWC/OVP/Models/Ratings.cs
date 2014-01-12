namespace TWC.OVP.Models
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class Ratings
    {
        [DataMember(Name="dialog")]
        public bool Dialog { get; set; }

        [DataMember(Name="fv")]
        public bool Fv { get; set; }

        [DataMember(Name="language")]
        public bool Language { get; set; }

        [DataMember(Name="mpaa")]
        public string MPAA { get; set; }

        [DataMember(Name="sex")]
        public bool Sex { get; set; }

        [DataMember(Name="star")]
        public string Star { get; set; }

        [DataMember(Name="tv")]
        public string TV { get; set; }

        [DataMember(Name="violence")]
        public bool Violence { get; set; }
    }
}

