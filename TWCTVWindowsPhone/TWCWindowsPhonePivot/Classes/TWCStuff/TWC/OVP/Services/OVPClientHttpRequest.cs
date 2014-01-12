namespace TWC.OVP.Services
{
    using Caliburn.Micro;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Browser;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using System.Windows;
    using TWC.OVP;
    using TWC.OVP.Framework.Extensions;
    using TWC.OVP.Messages;

    public class OVPClientHttpRequest
    {
        private static Dictionary<string, string> _dictCookieNameMapping = new Dictionary<string, string>();

        static OVPClientHttpRequest()
        {
            _dictCookieNameMapping["wayfarer_ns"] = "Wayfarer";
            _dictCookieNameMapping["vs_guid_ns"] = "vs_guid";
        }

        public static WebRequestResult BrowserHttpRequest(string url, int timeoutMilliSeconds)
        {
            return HttpRequest(url, timeoutMilliSeconds, false);
        }

        public static JsonRequestResult<T> BrowserJsonRequest<T>(string url, int timeoutMilliSeconds) where T: class
        {
            return JsonRequest<T>(url, timeoutMilliSeconds, false, null);
        }

        public static WebRequestResult ClientHttpRequest(string url, int timeoutMilliSeconds)
        {
            return HttpRequest(url, timeoutMilliSeconds, true);
        }

        public static JsonRequestResult<T> ClientJsonRequest<T>(string url, int timeoutMilliSeconds) where T: class
        {
            return JsonRequest<T>(url, timeoutMilliSeconds, true, null);
        }

        public static Task<JsonRequestResult<T>> CreateJsonRequestTask<T>(string url, int timeoutMilliSeconds, bool isClientRequest) where T: class
        {
            return new Task<JsonRequestResult<T>>(() => JsonRequest<T>(url, timeoutMilliSeconds, isClientRequest, null));
        }

        private static WebRequestResult HttpRequest(string url, int timeoutMilliSeconds, bool isClientRequest)
        {
            HttpWebRequest request;
            System.Action action2 = null;
            System.Action action3 = null;
            WebRequestResult result = new WebRequestResult();
            Uri requestUri = new Uri(url);
            if (isClientRequest)
            {
                request = WebRequestCreator.ClientHttp.Create(requestUri) as HttpWebRequest;
                CookieCollection cookies = MapOVPCookies(CookieHelper.GetCookies());
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(requestUri, cookies);
            }
            else
            {
                request = WebRequestCreator.BrowserHttp.Create(requestUri) as HttpWebRequest;
            }
            Task<WebResponse> task = Task.Factory.FromAsync<WebResponse>(new System.Func<AsyncCallback, object, IAsyncResult>(request.BeginGetResponse), new System.Func<IAsyncResult, WebResponse>(request.EndGetResponse), request);
            try
            {
                if (task.Wait(timeoutMilliSeconds))
                {
                    System.Action action = null;
                    HttpWebResponse response;
                    if (task.Status == TaskStatus.RanToCompletion)
                    {
                        response = task.Result as HttpWebResponse;
                        result.HttpStatusCode = (int) response.StatusCode;
                        result.Response = response;
                        if (isClientRequest)
                        {
                            if (action == null)
                            {
                                action = delegate {
                                    CookieHelper.SetCookiesToBrowser(response.Cookies);
                                };
                            }
                            action.OnUIThread();
                        }
                    }
                }
                else
                {
                    if (action2 == null)
                    {
                        action2 = delegate {
                            ((App) Application.Current).EventAggregator.Publish(new EGServiceErrorOccured(0x194, requestUri.GetServiceUri(), requestUri.Query, "", "", true, false));
                        };
                    }
                    action2.OnUIThread();
                    request.Abort();
                    throw new TimeoutException(string.Format("Request timeout. Url:{0}", url));
                }
            }
            catch (Exception exception)
            {
                WebException innerException = exception.InnerException as WebException;
                if (innerException != null)
                {
                    result.Response = (HttpWebResponse) innerException.Response;
                    result.HttpStatusCode = (int) result.Response.StatusCode;
                    result.Exception = innerException;
                    if (action3 == null)
                    {
                        action3 = delegate {
                            ((App) Application.Current).EventAggregator.Publish(new EGServiceErrorOccured(result.HttpStatusCode, requestUri.GetServiceUri(), requestUri.Query, result.Response.GetResponseBody(), "", false, false));
                        };
                    }
                    action3.OnUIThread();
                }
                else
                {
                    result.Exception = exception;
                }
            }
            return result;
        }

        public static JsonRequestResult<T> JsonRequest<T>(string url, int timeoutMilliSeconds, bool isClientRequest, System.Func<string, string> processJson = null) where T: class
        {
            System.Action action = null;
            string json;
            HttpWebResponse response;
            WebRequestResult result = HttpRequest(url, timeoutMilliSeconds, isClientRequest);
            JsonRequestResult<T> result2 = new JsonRequestResult<T>(result);
            if (result2.Exception == null)
            {
                json = string.Empty;
                response = result.Response;
                if (result.HttpStatusCode != 200)
                {
                    return result2;
                }
                try
                {
                    json = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    if (processJson != null)
                    {
                        json = processJson(json);
                    }
                    T local = json.DeserializeFromJson<T>();
                    result2.Object = local;
                }
                catch (Exception exception)
                {
                    if (action == null)
                    {
                        action = delegate {
                            ((App) Application.Current).EventAggregator.Publish(new EGServiceErrorOccured(0x194, response.ResponseUri.GetServiceUri(), response.ResponseUri.Query, json, "", false, true));
                        };
                    }
                    action.OnUIThread();
                    throw exception;
                }
            }
            return result2;
        }

        private static CookieCollection MapOVPCookies(CookieCollection inputCookies)
        {
            CookieCollection cookies = new CookieCollection();
            foreach (Cookie cookie in inputCookies)
            {
                if (_dictCookieNameMapping.ContainsKey(cookie.Name))
                {
                    cookies.Add(new Cookie(_dictCookieNameMapping[cookie.Name], cookie.Value));
                }
                else
                {
                    cookies.Add(cookie);
                }
            }
            return cookies;
        }
    }
}

