namespace TWC.OVP.Models
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class AssetInfoHeader
    {
        [DataMember(Name="error")]
        public string Error { get; set; }

        [DataMember(Name="data")]
        public AssetInfo Info { get; set; }
    }
}

