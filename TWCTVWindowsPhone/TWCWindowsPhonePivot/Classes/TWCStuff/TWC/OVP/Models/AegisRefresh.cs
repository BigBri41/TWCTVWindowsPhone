namespace TWC.OVP.Models
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class AegisRefresh
    {
        [DataMember(Name="token_refresh_seconds")]
        public string TokenRefreshSeconds { get; set; }
    }
}

