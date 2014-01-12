namespace TWC.OVP.Models
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class Advisories
    {
        [DataMember(Name="adultSituations")]
        public bool AdultSituations { get; set; }

        [DataMember(Name="briefNudity")]
        public bool BriefNudity { get; set; }

        [DataMember(Name="graphicLanguage")]
        public bool GraphicLanguage { get; set; }

        [DataMember(Name="graphicViolence")]
        public bool GraphicViolence { get; set; }

        [DataMember(Name="language")]
        public bool Language { get; set; }

        [DataMember(Name="mildViolence")]
        public bool MildViolence { get; set; }

        [DataMember(Name="nudity")]
        public bool Nudity { get; set; }

        [DataMember(Name="rape")]
        public bool Rape { get; set; }

        [DataMember(Name="strongSexualContent")]
        public bool StrongSexualContent { get; set; }

        [DataMember(Name="violence")]
        public bool Violence { get; set; }
    }
}

