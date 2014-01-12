namespace TWC.OVP.Services
{
    using System;
    using System.Net;
    using System.Runtime.CompilerServices;

    public class WebRequestResult
    {
        public System.Exception Exception { get; set; }

        public int HttpStatusCode { get; set; }

        public HttpWebResponse Response { get; set; }
    }
}

