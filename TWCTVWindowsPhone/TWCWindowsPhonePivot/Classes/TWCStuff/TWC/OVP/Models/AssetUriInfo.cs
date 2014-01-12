namespace TWC.OVP.Models
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class AssetUriInfo
    {
        [DataMember(Name="iv")]
        public string IV { get; set; }

        [DataMember(Name="key_url")]
        public Uri KeyUri { get; set; }

        [DataMember(Name="stream_url")]
        public Uri StreamUri { get; set; }

        [DataMember(Name="token_refresh_seconds")]
        public string TokenRefreshSeconds { get; set; }
    }
}

