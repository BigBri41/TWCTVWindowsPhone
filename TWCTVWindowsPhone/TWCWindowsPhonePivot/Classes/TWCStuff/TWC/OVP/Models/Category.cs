namespace TWC.OVP.Models
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class Category
    {
        [DataMember(Name="title")]
        public string Title { get; set; }

        [DataMember(Name="uri")]
        public string Uri { get; set; }
    }
}

