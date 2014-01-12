namespace TWC.OVP.Models
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class Program
    {
        public List<string> advisory { get; set; }

        public List<object> awards { get; set; }

        public List<Cast> cast { get; set; }

        public string colorCode { get; set; }

        public List<Crew> crew { get; set; }

        public List<Description> description { get; set; }

        public List<string> genre { get; set; }

        public List<Image> images { get; set; }

        public string officialURL { get; set; }

        public string origAudioLang { get; set; }

        public string programType { get; set; }

        public string provider { get; set; }

        public string rating { get; set; }

        public List<Release> releases { get; set; }

        public Reviews reviews { get; set; }

        public string rootID { get; set; }

        public string runTime { get; set; }

        public List<Title> title { get; set; }

        public string tmsId { get; set; }

        public string tmsStarRating { get; set; }
    }
}

