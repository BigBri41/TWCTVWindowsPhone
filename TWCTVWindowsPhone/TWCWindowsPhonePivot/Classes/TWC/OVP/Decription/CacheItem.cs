using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TWC.OVP.Decription
{
    public class CacheItem
    {
        // Properties
        public long ContentLength { get; set; }

        public string ContentType { get; set; }

        public byte[] Data { get; set; }

        public DateTime DownloadTime { get; set; }

        public WebHeaderCollection Headers { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string StatusDescription { get; set; }
    }

 

}
