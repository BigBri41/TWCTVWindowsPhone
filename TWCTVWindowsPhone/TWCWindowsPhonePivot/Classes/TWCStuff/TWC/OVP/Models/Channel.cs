namespace TWC.OVP.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class Channel
    {
        public bool availableOutOfHome { get; set; }

        public string callSign { get; set; }

        public string logoUrl { get; set; }

        public string networkName { get; set; }

        public List<Stream> streams { get; set; }

        public string tmsId { get; set; }
    }
}

