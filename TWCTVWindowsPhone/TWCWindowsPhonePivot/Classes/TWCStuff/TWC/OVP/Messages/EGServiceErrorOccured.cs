namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class EGServiceErrorOccured
    {
        public EGServiceErrorOccured(int httpStatusCode, string servicesURI, string requestParameters, string responseBody, string impactedCapability, bool isRequestTimeout, bool isUnexpectedResponse)
        {
            this.HTTPStatusCode = httpStatusCode;
            this.ServicesURI = servicesURI;
            this.RequestParameters = requestParameters;
            this.ResponseBody = responseBody;
            this.ImpactedCapability = impactedCapability;
            this.IsRequestTimeout = isRequestTimeout;
            this.IsUnexpectedResponse = isUnexpectedResponse;
        }

        public int HTTPStatusCode { get; set; }

        public string ImpactedCapability { get; set; }

        public bool IsRequestTimeout { get; set; }

        public bool IsUnexpectedResponse { get; set; }

        public string RequestParameters { get; set; }

        public string ResponseBody { get; set; }

        public string ServicesURI { get; set; }
    }
}

