namespace TWC.OVP.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class EpisodeDetail
    {
        [DataMember(Name="advisories")]
        public TWC.OVP.Models.Advisories Advisories { get; set; }

        [DataMember(Name="airDateTime")]
        public string AirDateTime { get; set; }

        [DataMember(Name="cast")]
        public List<EpisodeCast> Cast { get; set; }

        [DataMember(Name="credits")]
        public List<Credit> Credits { get; set; }

        [DataMember(Name="desc")]
        public string Description { get; set; }

        [DataMember(Name="details")]
        public TWC.OVP.Models.Details Details { get; set; }

        [DataMember(Name="dubbed")]
        public TWC.OVP.Models.Dubbed Dubbed { get; set; }

        [DataMember(Name="duration")]
        public int Duration { get; set; }

        [DataMember(Name="episodeId")]
        public string EpisodeId { get; set; }

        [DataMember(Name="episodeTitle")]
        public string EpisodeTitle { get; set; }

        [DataMember(Name="genre01")]
        public string Genre01 { get; set; }

        [DataMember(Name="genre02")]
        public string Genre02 { get; set; }

        [DataMember(Name="gmTime")]
        public int GmTime { get; set; }

        [DataMember(Name="prem_fin")]
        public string PremFin { get; set; }

        [DataMember(Name="ratings")]
        public TWC.OVP.Models.Ratings Ratings { get; set; }

        [DataMember(Name="sap")]
        public TWC.OVP.Models.Sap Sap { get; set; }

        [DataMember(Name="serviceId")]
        public string ServiceId { get; set; }

        [DataMember(Name="subtitled")]
        public TWC.OVP.Models.Subtitled Subtitled { get; set; }

        [DataMember(Name="title")]
        public string Title { get; set; }

        [DataMember(Name="year")]
        public string Year { get; set; }
    }
}

