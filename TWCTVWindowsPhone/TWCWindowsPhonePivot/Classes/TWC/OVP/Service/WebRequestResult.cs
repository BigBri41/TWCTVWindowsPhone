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

namespace TWC.OVP.Service
{
    public class WebRequestResult
    {
        // Properties
        public Exception Exception { get; set; }

        public int HttpStatusCode { get; set; }

        public HttpWebResponse Response { get; set; }
    }

 
 

}
