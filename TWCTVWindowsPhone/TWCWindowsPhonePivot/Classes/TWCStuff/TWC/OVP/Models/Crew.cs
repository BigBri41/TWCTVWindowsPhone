namespace TWC.OVP.Models
{
    using System;
    using System.Runtime.CompilerServices;

    public class Crew
    {
        public Link2 link { get; set; }

        public object nmdId { get; set; }

        public int ord { get; set; }

        public Person2 person { get; set; }

        public string personID { get; set; }

        public string role { get; set; }
    }
}

