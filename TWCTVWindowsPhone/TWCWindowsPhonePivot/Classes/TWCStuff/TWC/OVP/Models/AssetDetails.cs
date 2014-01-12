namespace TWC.OVP.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class AssetDetails
    {
        [DataMember(Name="actors")]
        public List<string> Actors { get; set; }

        [DataMember(Name="num_assets")]
        public int AssetsCount { get; set; }

        [DataMember(Name="short_desc")]
        public string Description { get; set; }

        [DataMember(Name="director")]
        public string Director { get; set; }

        [DataMember(Name="episode_number")]
        public int EpisodeNumber { get; set; }

        [DataMember(Name="genres")]
        public List<Genre> Genres { get; set; }

        [DataMember(Name="original_air_date")]
        public string OriginalAirDate { get; set; }

        [DataMember(Name="original_network_name")]
        public string OriginalNetworkName { get; set; }

        [DataMember(Name="rating")]
        public string Rating { get; set; }

        [DataMember(Name="season_number")]
        public int SeasonNumber { get; set; }

        [DataMember(Name="year")]
        public int Year { get; set; }
    }
}

