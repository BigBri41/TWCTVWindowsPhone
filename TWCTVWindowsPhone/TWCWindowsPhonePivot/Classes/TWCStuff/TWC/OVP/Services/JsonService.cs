namespace TWC.OVP.Services
{
    using Caliburn.Micro;
    using System;
    using System.Runtime.InteropServices;

    public class JsonService
    {
        public static IResult CreateJsonRequest<T>(string uri, Action<T> callback, Action<Exception> faultCallback = null, Action<Exception> exceptionCallback = null, int timeoutMilliSeconds = 0x2710, System.Func<string, string> processJson = null) where T: class
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

