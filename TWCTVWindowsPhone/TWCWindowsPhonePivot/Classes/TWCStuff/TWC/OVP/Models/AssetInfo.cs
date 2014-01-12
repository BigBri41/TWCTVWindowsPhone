namespace TWC.OVP.Models
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class AssetInfo
    {
        [DataMember(Name="details")]
        public AssetDetails Details { get; set; }

        [DataMember(Name="id")]
        public string ID { get; set; }

        [DataMember(Name="image_uri")]
        public string ImageUri { get; set; }

        [DataMember(Name="isEntitled")]
        public bool IsEntitled { get; set; }

        public bool IsFastForwardEnabled
        {
            get
            {
                if ((this.TricksMode != null) && (this.TricksMode.FastForward != null))
                {
                    return (this.TricksMode.FastForward.Count <= 0);
                }
                return true;
            }
        }

        [DataMember(Name="network")]
        public TWC.OVP.Models.Network Network { get; set; }

        [DataMember(Name="nmd_main_uri")]
        public string NmdMainUri { get; set; }

        [DataMember(Name="series_title")]
        public string SeriesTitle { get; set; }

        [DataMember(Name="streams")]
        public TWC.OVP.Models.Streams Streams { get; set; }

        [DataMember(Name="title")]
        public string Title { get; set; }

        [DataMember(Name="tricks_mode")]
        public TWC.OVP.Models.TricksMode TricksMode { get; set; }

        [DataMember(Name="type")]
        public string Type { get; set; }

        [DataMember(Name="uri")]
        public string Uri { get; set; }
    }
}

