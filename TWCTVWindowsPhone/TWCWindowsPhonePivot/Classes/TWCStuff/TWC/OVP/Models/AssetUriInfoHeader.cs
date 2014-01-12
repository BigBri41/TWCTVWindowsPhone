namespace TWC.OVP.Models
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class AssetUriInfoHeader
    {
        [DataMember(Name="error")]
        public string Error { get; set; }

        [DataMember(Name="data")]
        public AssetUriInfo Info { get; set; }
    }
}

