namespace TWC.OVP.Decryption
{
    using System;
    using System.Net;
    using System.Runtime.CompilerServices;

    public class CacheItem
    {
        public long ContentLength { get; set; }

        public string ContentType { get; set; }

        public byte[] Data { get; set; }

        public DateTime DownloadTime { get; set; }

        public WebHeaderCollection Headers { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string StatusDescription { get; set; }
    }
}

