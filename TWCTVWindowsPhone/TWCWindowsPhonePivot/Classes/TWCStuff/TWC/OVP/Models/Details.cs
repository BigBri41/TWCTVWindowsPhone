namespace TWC.OVP.Models
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class Details
    {
        [DataMember(Name="cc")]
        public bool CC { get; set; }

        [DataMember(Name="dolby")]
        public bool Dolby { get; set; }

        [DataMember(Name="ei")]
        public bool Ei { get; set; }

        [DataMember(Name="enhanced")]
        public bool Enhanced { get; set; }

        [DataMember(Name="family")]
        public bool Family { get; set; }

        [DataMember(Name="hdtv")]
        public bool Hdtv { get; set; }

        [DataMember(Name="letterbox")]
        public bool Letterbox { get; set; }

        [DataMember(Name="lifestyle")]
        public bool Lifestyle { get; set; }

        [DataMember(Name="movie")]
        public bool Movie { get; set; }

        [DataMember(Name="new")]
        public bool New { get; set; }

        [DataMember(Name="news")]
        public bool News { get; set; }

        [DataMember(Name="series")]
        public bool Series { get; set; }

        [DataMember(Name="sports")]
        public bool Sports { get; set; }

        [DataMember(Name="stereo")]
        public bool Stereo { get; set; }
    }
}

