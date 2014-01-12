namespace TWC.OVP.Services
{
    using System;
    using System.Runtime.CompilerServices;

    public class JsonRequestResult<T> : WebRequestResult where T: class
    {
        public JsonRequestResult(WebRequestResult result)
        {
            base.Response = result.Response;
            base.HttpStatusCode = result.HttpStatusCode;
            base.Exception = result.Exception;
        }

        public T Object { get; set; }

        public string RespondText { get; set; }
    }
}

