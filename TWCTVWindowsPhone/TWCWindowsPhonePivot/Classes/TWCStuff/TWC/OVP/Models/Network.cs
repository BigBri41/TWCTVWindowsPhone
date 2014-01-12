namespace TWC.OVP.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class Network
    {
        [DataMember(Name="availableOutOfHome")]
        public bool AvailableOutOfHome { get; set; }

        [DataMember(Name="callsign")]
        public string CallSign { get; set; }

        [DataMember(Name="num_categories")]
        public int CategoriesCount { get; set; }

        [DataMember(Name="image_uri")]
        public string ImageUri { get; set; }

        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name="product_provider")]
        public string ProductProvider { get; set; }

        [DataMember(Name="stars_id")]
        public string StarsID { get; set; }

        [DataMember(Name="stars_ids")]
        public List<string> StarsIDs { get; set; }

        [DataMember(Name="type")]
        public string Type { get; set; }

        [DataMember(Name="uri")]
        public string Uri { get; set; }
    }
}

