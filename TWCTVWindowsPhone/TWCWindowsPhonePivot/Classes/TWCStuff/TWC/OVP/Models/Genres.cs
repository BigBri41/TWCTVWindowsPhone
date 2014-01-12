namespace TWC.OVP.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class Genres
    {
        [DataMember(Name="genre")]
        public List<GenreInfo> GenreInfos { get; set; }
    }
}

