namespace TWC.OVP.Models
{
    using System;
    using System.Runtime.CompilerServices;

    public class Cast
    {
        public string characterName { get; set; }

        public Link link { get; set; }

        public object nmdId { get; set; }

        public int ord { get; set; }

        public Person person { get; set; }

        public string personID { get; set; }

        public string role { get; set; }
    }
}

