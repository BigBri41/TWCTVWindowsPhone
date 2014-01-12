namespace TWC.OVP.Models
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class WhatsOn
    {
        [DataMember(Name="airDateTime")]
        public string AirDateTime { get; set; }

        [DataMember(Name="callSign")]
        public string CallSign { get; set; }

        [DataMember(Name="channelId")]
        public string ChannelId { get; set; }

        [DataMember(Name="duration")]
        public string Duration { get; set; }

        [DataMember(Name="episodeId")]
        public string EpisodeId { get; set; }

        [DataMember(Name="episodeTitle")]
        public string EpisodeTitle { get; set; }

        [DataMember(Name="genre01")]
        public string Genre01 { get; set; }

        [DataMember(Name="gmTime")]
        public string GmTime { get; set; }

        [DataMember(Name="rating")]
        public string Rating { get; set; }

        [DataMember(Name="shortDesc")]
        public string ShortDesc { get; set; }

        [DataMember(Name="starRating")]
        public string StarRating { get; set; }

        [DataMember(Name="title")]
        public string Title { get; set; }

        [DataMember(Name="tmsId")]
        public string TmsId { get; set; }
    }
}

