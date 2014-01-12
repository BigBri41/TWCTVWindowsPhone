namespace TWC.OVP.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class Review
    {
        public string description { get; set; }

        public string provider { get; set; }

        public string rating { get; set; }

        public List<ReviewProperty> reviewProperties { get; set; }

        public string summary { get; set; }
    }
}

