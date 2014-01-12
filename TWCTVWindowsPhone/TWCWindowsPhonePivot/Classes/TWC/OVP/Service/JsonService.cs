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
    public interface IResult
    {
        // Events
        event EventHandler<ResultCompletionEventArgs> Completed;

        // Methods
        void Execute(ActionExecutionContext context);
    }

 

    public class JsonService
    {
        // Methods
        public static IResult CreateJsonRequest<T>(string uri, Action<T> callback, Action<Exception> faultCallback = null, Action<Exception> exceptionCallback = null, int timeoutMilliSeconds = 0x2710, Func<string, string> processJson = null) where T : class
        {
            JsonRequestResult<T> result = OVPClientHttpRequest.JsonRequest<T>(uri, timeoutMilliSeconds, false, processJson);
            if (result.Exception != null)
            {
                if (exceptionCallback == null)
                {
                    throw result.Exception;
                }
                exceptionCallback(result.Exception);
            }
            else if (callback != null)
            {
                callback(result.Object);
            }
            return null;
        }
    }

 

}
