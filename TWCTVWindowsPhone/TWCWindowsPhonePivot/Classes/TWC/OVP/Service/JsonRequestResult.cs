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
    public class JsonRequestResult<T> : WebRequestResult where T : class
    {
        // Methods
        public JsonRequestResult(WebRequestResult result)
        {
            base.Response = result.Response;
            base.HttpStatusCode = result.HttpStatusCode;
            base.Exception = result.Exception;
        }

        // Properties
        public T Object { get; set; }

        public string RespondText { get; set; }
    }

 

}
