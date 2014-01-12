using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using TWCWindowsPhonePivot.Classes;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Windows.Media.Imaging;
//using System.Runtime.Serialization.Json;
using ICSharpCode.SharpZipLib.GZip;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace TWCWindowsPhonePivot.Helpers
{
    public class TWCStaticCalls
    {
    
        public event EventHandler LoginFinished;
        public event EventHandler ChangeChannelFinished;
        public event EventHandler GetDevicesFinished;
        public event EventHandler GetGuideFinished;
        public event EventHandler GetShowsFinished;
        public event EventHandler GetRecordingsFinished;
        public event EventHandler GetShowImageFinished;

        public void OnLoginFinished(bool loggedIn)
        {
            if (loggedIn)
            {
                myModel = (TWCModel)parameters["myModel"];
            }

            EventHandler handler = LoginFinished;
            if (null != handler) handler(loggedIn, EventArgs.Empty);
        }

        public void OnChangeChannelFinished()
        {
            EventHandler handler = ChangeChannelFinished;
            if (null != handler) handler(this, EventArgs.Empty);
        }

        public void OnGetDevicesFinished()
        {
            EventHandler handler = GetDevicesFinished;
            if (null != handler) handler(this, EventArgs.Empty);
        }

        public void OnGetGuideFinished()
        {
            EventHandler handler = GetGuideFinished;
            if (null != handler) handler(this, EventArgs.Empty);
        }

        public void OnGetShowsFinished(string startChannelEndChannel)
        {
            EventHandler handler = GetShowsFinished;

            if (null != handler) handler(startChannelEndChannel, EventArgs.Empty);
        }

        public void OnGetRecordingsFinished()
        {
            EventHandler handler = GetRecordingsFinished;

            if (null != handler) handler(this, EventArgs.Empty);
        }

        public void OnGetShowImageFinished(string episodeId)
        {
            EventHandler handler = GetShowImageFinished;

            if (null != handler) handler(episodeId, EventArgs.Empty);
        }

        private TWCModel myModel = new TWCModel();

        public TWCModel Model { get { return myModel; } }
        
        private Dictionary<string, object> parameters { get; set; }

        public Dictionary<string, object> Parameters { get { return parameters; } }
        public CookieCollection cookieCollection = new CookieCollection();


        public TWCStaticCalls()
        {/*
            //Trust all certificates
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);

            // trust sender
            System.Net.ServicePointManager.ServerCertificateValidationCallback
                            = ((sender, cert, chain, errors) => cert.Subject.Contains("."));

            // validate cert by calling a function
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateRemoteCertificate);
          */

            parameters = new Dictionary<string, object>();
        }

        public TWCStaticCalls(TWCModel setModel)
        {
            parameters = new Dictionary<string, object>();
            myModel = setModel;
            parameters["myModel"] = setModel;
        }


        public string LoginGetWayfarer(string userName, string password, TWCModel myModel)
        {
            string returnString = "";
           
            LoginGetWayfarerRequest(userName, password, myModel);
           

            return returnString;
        }

        private void LoginGetWayfarerRequest(string userName, string password, TWCModel myModel)
        {

            try
            {
               

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://video2.timewarnercable.com/call?url=https://wayfarer.timewarnercable.com/wayfarer/authenticate");
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.dell.com");
                //request.CookieContainer = cookieContainer;
                request.Accept = "*/*";
                request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                
                request.Headers["X-Requested-With"]= @"XMLHttpRequest";
                request.Headers["Referer"] = "https://video2.timewarnercable.com/";
                request.Headers["Accept-Language"]= "en-us";
                request.Headers["Accept-Encoding"]= "gzip, deflate";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
                request.Headers["Cache-Control"]= "no-cache";
                //request.Headers.Set(HttpRequestHeader.Cookie, @"device_id=5572-239ffcbd-e8b6d19d-7bb56ee1-b631; header=%7B%22appStartup%22%3A%222013-08-09T20%3A21%3A21.468Z%22%2C%22sessionID%22%3A%226ab724f4-0274-44b5-bef2-8c0777b82dc9%22%2C%22previousSessionID%22%3A%22%22%2C%22UTCOffset%22%3A300%2C%22targetDataConsumers%22%3A%22PRODUCTION%22%2C%22logLevel%22%3A0%7D; clientDetails=%7B%22applicationName%22%3A%22OVP%22%2C%22applicationVersion%22%3A%222.3.0.1%22%2C%22apiVersion%22%3A%222.3.0.1%22%2C%22formFactor%22%3A%22PC%22%2C%22triggeredBy%22%3A%7B%22initiator%22%3A%22user%22%2C%22link%22%3A%22undefined%22%7D%2C%22deviceModel%22%3A%22Win32%22%2C%22deviceOS%22%3A%22Mozilla/5.0%20%28compatible%3B%20MSIE%209.0%3B%20Windows%20NT%206.1%3B%20WOW64%3B%20Trident/5.0%3B%20SLCC2%3B%20.NET%20CLR%202.0.50727%3B%20Media%20Center%20PC%206.0%3B%20MS-RTC%20LM%208%3B%20.NET%20CLR%203.5.30729%3B%20.NET%20CLR%203.0.30729%3B%20.NET4.0E%3B%20.NET%20CLR%201.1.4322%3B%20InfoPath.3%3B%20.NET4.0C%3B%20Zune%204.7%29%22%2C%22deviceID%22%3A%225572-239ffcbd-e8b6d19d-7bb56ee1-b631%22%7D");

                string deviceIdHeaderString = HttpUtility.UrlEncode(myModel.DeviceId);
                string headerString = HttpUtility.UrlEncode(myModel.HeaderOptionsJson);
                string clientDetailsString = HttpUtility.UrlEncode(myModel.ClientDetailsJson);
               
                //request.Headers.Set(HttpRequestHeader.Cookie, "device_id=" + deviceIdHeaderString + "; header=" + headerString + "; clientDetails=" + clientDetailsString + ";");
                request.Headers["Cookie"] = "device_id=" + deviceIdHeaderString + "; header=" + headerString + "; clientDetails=" + clientDetailsString + ";";
               
                request.Method = "POST";
                //request.ServicePoint.Expect100Continue = false;
                
              
                //request.ContentLength = postBytes.Length;
                
                //Stream stream = request.GetRequestStream();

                if (parameters.Select(x => x.Key == "request").Count() == 0)
                {
                    parameters.Add("request", request);
                }
                else
                {
                    parameters["request"] = request;
                }

                if (parameters.Select(x => x.Key == "userName").Count() == 0)
                {
                    parameters.Add("userName", userName);
                }
                else
                {
                    parameters["userName"] = userName;
                }

                if (parameters.Select(x => x.Key == "password").Count() == 0)
                {
                    parameters.Add("password", password);
                }
                else
                {
                    parameters["password"] = password;
                }

                if (parameters.Select(x => x.Key == "myModel").Count() == 0)
                {
                    parameters.Add("myModel", myModel);
                }
                else
                {
                    parameters["myModel"] = myModel;
                }

                request.BeginGetRequestStream(new AsyncCallback(LoginWayfarerRequestStreamCallback),parameters );

                

            }
            catch (WebException e)
            {

            }

        }



        private void LoginWayfarerRequestStreamCallback(IAsyncResult asynchronousResult)
        {


            //HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
            HttpWebRequest request = (HttpWebRequest)parameters["request"];
            
            string userName = parameters["userName"].ToString();
            string password = parameters["password"].ToString();


            // End the operation
            System.IO.Stream postStream = request.EndGetRequestStream(asynchronousResult);
            
            string body = @"username=" + userName + "&password=" + password;
            byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);


            // Write to the request stream.
            postStream.Write(postBytes, 0, postBytes.Length);
            postStream.Close();
            

            // Start the asynchronous operation to get the response

            parameters["request"] = request;

            request.BeginGetResponse(new AsyncCallback(LoginWayfarerResponseCallback),parameters);

        }

        private void LoginWayfarerResponseCallback(IAsyncResult asynchronousResult)
        {
            string wayFarer = "";

            try
            {
                //HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
                HttpWebRequest request = (HttpWebRequest)parameters["request"];

                // End the operation
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
                System.IO.Stream streamResponse = response.GetResponseStream();
                StreamReader streamRead = new StreamReader(streamResponse);

                if (response.Cookies != null)
                {
                    foreach (Cookie currentCookie in response.Cookies)
                    {
                        cookieCollection.Add(currentCookie);
                    }
                }


                string authString = streamRead.ReadToEnd().ToString().ToUpper().Trim();
                if (authString == "AUTHENTICATED")
                {
                    string decodedString = HttpUtility.UrlDecode(response.Headers["Set-Cookie"].ToString().Replace("Wayfarer=", ""));

                    if (decodedString.IndexOf(';') > 0)
                    {
                        wayFarer = decodedString.Substring(0, decodedString.IndexOf(';'));
                        parameters.Add("wayfarer", wayFarer);
                    }
                    else
                    {
                        wayFarer = decodedString;
                        parameters.Add("wayfarer", wayFarer);
                    }

                }


                // Close the stream object
                streamResponse.Close();
                streamRead.Close();

                // Release the HttpWebResponse
                response.Close();

                if (parameters["wayfarer"] != null)
                {
                    if (!string.IsNullOrEmpty(parameters["wayfarer"].ToString()))
                    {
                        ((TWCModel)parameters["myModel"]).Wayfarer = wayFarer;
                        GetVsGuid(((TWCModel)parameters["myModel"]));
                    }
                    else
                    {
                        OnLoginFinished(false);
                    }
                }

            }
            catch (Exception) 
            {
                OnLoginFinished(false);
            }



         
           
        }





        private void GetVsGuid(TWCModel myModel)
        {


            GetVsGuidRequest(myModel);
            
        }

        private void GetVsGuidRequest(TWCModel myModel)
        {

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://video2.timewarnercable.com/call?url=https://video-services.timewarnercable.com/api/v3/login");//&_=1376079759324");
                //request.CookieContainer = cookieContainer;
                request.Headers["X-Requested-With"] = @"XMLHttpRequest";
                request.Accept = "application/json, text/javascript, */*; q=0.01";
                request.Headers["Referer"] = "https://video2.timewarnercable.com/";
                request.Headers["Accept-Language"] = "en-us";
                request.Headers["Accept-Encoding"] = "gzip, deflate";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
                //request.Headers.Set(HttpRequestHeader.Cookie, @"device_id=5572-239ffcbd-e8b6d19d-7bb56ee1-b631; Wayfarer=xV%2FYFSxVUwpKfSwHktQ58zlo0ORQLbHO7VDd32rUnWqP9xbZGHH%2BFwqNPGPCdO0I6F6F6oD0HWLwTsp5OkSZM5b1pWGYZPCS5UuF68OCWkkr2WrKrxUOhyXEwlE2UTCTgBX5yaBmvCUC95TjP%2BDDMSLCm1Zi48Sffr1MPMBXPPYXwpZ6cGaO2f%2FplsG%2BVCnodDS9ZtP60rLdGS%2BrbtEvub%2BFR2I9zbqJers93D5R8EA0%2FDtkHkQReA%3D%3D; rememberme=eyJ1c2VybmFtZSI6IiIsImNjIjp0cnVlLCJsYXN0X2FjdGl2aXR5IjoiIiwibGFzdF9hY3Rpb24iOiIiLCJsYXN0X2NoYW5uZWwiOiIifQ%3D%3D; customer_info=eyJhY2NvdW50SG9sZGVyIjoiQnJpYW4gU2NvdHQifQ%3D%3D; header=%7B%22appStartup%22%3A%222013-08-09T20%3A21%3A21.468Z%22%2C%22sessionID%22%3A%226ab724f4-0274-44b5-bef2-8c0777b82dc9%22%2C%22previousSessionID%22%3A%22%22%2C%22UTCOffset%22%3A300%2C%22targetDataConsumers%22%3A%22PRODUCTION%22%2C%22logLevel%22%3A0%7D; clientDetails=%7B%22applicationName%22%3A%22OVP%22%2C%22applicationVersion%22%3A%222.3.0.1%22%2C%22apiVersion%22%3A%222.3.0.1%22%2C%22formFactor%22%3A%22PC%22%2C%22triggeredBy%22%3A%7B%22initiator%22%3A%22user%22%2C%22link%22%3A%22undefined%22%7D%2C%22deviceModel%22%3A%22Win32%22%2C%22deviceOS%22%3A%22Mozilla/5.0%20%28compatible%3B%20MSIE%209.0%3B%20Windows%20NT%206.1%3B%20WOW64%3B%20Trident/5.0%3B%20SLCC2%3B%20.NET%20CLR%202.0.50727%3B%20Media%20Center%20PC%206.0%3B%20MS-RTC%20LM%208%3B%20.NET%20CLR%203.5.30729%3B%20.NET%20CLR%203.0.30729%3B%20.NET4.0E%3B%20.NET%20CLR%201.1.4322%3B%20InfoPath.3%3B%20.NET4.0C%3B%20Zune%204.7%29%22%2C%22deviceID%22%3A%225572-239ffcbd-e8b6d19d-7bb56ee1-b631%22%7D; vs_client_platform_version=twc-tv-ovp_1.0");
                //string headerSetString = "" +
                //"rememberme=eyJ1c2VybmFtZSI6IkJpZ0JyaTQxIiwiY2MiOnRydWUsImxhc3RfYWN0aXZpdHkiOiIiLCJsYXN0X2FjdGlvbiI6IiIsImxhc3RfY2hhbm5lbCI6IiIsImZpbHRlcnN0YXRlIjoiYWxsIn0%3D; " +
                //"device_id=70a8-486a6692-b83ed2bb-a2c849ec-3984; " +
                //"Wayfarer=xV%2FYFSxVUwpKfSwHktQ58zlo0ORQLbHO7VDd32rUnWqP9xbZGHH%2BF9hOszgzWzL%2Fv9nJIK293fzwTsp5OkSZM5b1pWGYZPCS5UuF68OCWkkr2WrKrxUOhyXEwlE2UTCTgBX5yaBmvCUC95TjP%2BDDMR5I6pvGHukY%2Bv4y24xHe38G6qvZfkk9KXFs5fWy97M%2B8hmu1Y9CIAdHbrLDD8S9vq7vXQ3Md%2FdcqK1pr4OvnNuXIsQad61ckg%3D%3D; " +
                //"customer_info=eyJhY2NvdW50SG9sZGVyIjoiQnJpYW4gU2NvdHQifQ%3D%3D; " +
                //"vs_guid=C2GSYvgjlkFtTUGBT8pVjnN3ViafxjoaKTo3dE%2F%2FwEmOegi3RaAqcg%3D%3D; " +
                //"division=eyJkaXZpc2lvbiI6IkNUWC44MjYwIiwiemlwQ29kZSI6Ijc4NjM0LTUzNjEiLCJzdGF0ZSI6IlRYIiwic3Vic2NyaWJlZFR2UGFja2FnZVN0YW5kYXJkVHlwZXMiOlsiREdCU1QiLCJFUUNWSCIsIkJBU0lDIiwiQ1BTVFIiLCJTRFZSUyIsIlBTU1RaIl0sInJvYWRSdW5uZXJTdGFuZGFyZFR5cGUiOiJSUlNURCJ9; " +
                //"ovpsec=eyJjdXN0X2lkIjoiMjlmMDJmNmEyYzMwYzg2NDdhYjZlNDc5OGE5OWFmNTkyODQ0MmQzMSIsImR2ciI6MSwiZHZyX21zZyI6IiIsImxpdmV0diI6dHJ1ZSwib25kZW1hbmQiOnRydWUsImxpdmV0dl9tc2ciOiIiLCJwZXJtaXNzaW9ucyI6WyJ0dmV2ZXJ5d2hlcmUiLCJ0d2NhYmxldHYiLCJyZHZyIl19; " +
                //"wayfarer_ns=xV%2FYFSxVUwpKfSwHktQ58zlo0ORQLbHO7VDd32rUnWqP9xbZGHH%2BF9hOszgzWzL%2Fv9nJIK293fzwTsp5OkSZM5b1pWGYZPCS5UuF68OCWkkr2WrKrxUOhyXEwlE2UTCTgBX5yaBmvCUC95TjP%2BDDMR5I6pvGHukY%2Bv4y24xHe38G6qvZfkk9KXFs5fWy97M%2B8hmu1Y9CIAdHbrLDD8S9vq7vXQ3Md%2FdcqK1pr4OvnNuXIsQad61ckg%3D%3D; " +
                //"vs_guid_ns=C2GSYvgjlkFtTUGBT8pVjnN3ViafxjoaKTo3dE%2F%2FwEmOegi3RaAqcg%3D%3D; " +
                //"loc=eyJiZWhpbmRPd25Nb2RlbSI6dHJ1ZSwiaW5VUyI6dHJ1ZSwiaW5Vc09yVGVycml0b3J5Ijp0cnVlfQ%3D%3D; " +
                //"current_stb=eyJjbGllbnRUeXBlIjoiT0ROIiwiY2xpZW50VmVyc2lvbiI6IjYuMS4wLjUiLCJtYWNBZGRyZXNzIjoiMDA6MjE6QkU6REY6QTg6NkMiLCJuYW1lIjoiMDA6MjE6QkU6REY6QTg6NkMiLCJsaW5ldXBJZCI6IjI4NiIsImhlYWRlbmQiOiIxNjVfMjM3XzIxOF8yMjJfMjg2IiwiaXNEdnIiOnRydWUsImR2ciI6dHJ1ZX0%3D; " +
                //"ovp_stb=eyJzdGJJbmZvIjp7Im1hcyI6IjE2NS4yMzcuMjE4LjIyMiIsInNldFRvcEJveGVzIjpbeyJjbGllbnRUeXBlIjoiT0ROIiwiY2xpZW50VmVyc2lvbiI6IjYuMS4wLjUiLCJtYWNBZGRyZXNzIjoiMDA6MjE6QkU6REY6QTg6NkMiLCJuYW1lIjoiMDA6MjE6QkU6REY6QTg6NkMiLCJsaW5ldXBJZCI6IjI4NiIsImhlYWRlbmQiOiIxNjVfMjM3XzIxOF8yMjJfMjg2IiwiaXNEdnIiOnRydWUsImR2ciI6dHJ1ZX0seyJjbGllbnRUeXBlIjoiT0ROIiwiY2xpZW50VmVyc2lvbiI6IjYuMS4wLjUiLCJtYWNBZGRyZXNzIjoiMDA6MjE6QkU6MTk6OUI6OUMiLCJuYW1lIjoiMDA6MjE6QkU6MTk6OUI6OUMiLCJsaW5ldXBJZCI6IjI4NiIsImhlYWRlbmQiOiIxNjVfMjM3XzIxOF8yMjJfMjg2IiwiaXNEdnIiOmZhbHNlLCJkdnIiOmZhbHNlfV19LCJ0aW1lWm9uZSI6eyJ1dGNPZmZzZXQiOiItMzAwIiwidGltZVpvbmUiOiIyIn0sImhlYWRlbmQiOiIxNjVfMjM3XzIxOF8yMjJfMjg2In0%3D; " +
                //"eventgateway_stb_list=W3siY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibmFtZSI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjp0cnVlLCJkdnIiOnRydWV9LHsiY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibmFtZSI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjpmYWxzZSwiZHZyIjpmYWxzZX1d; " +
                //"vs_client_platform_version=twc-tv-ovp_1.0; " +
                //"parentalControls=true; " +
                //"header=%7B%22appStartup%22%3A%222013-08-09T20%3A00%3A30.345Z%22%2C%22sessionID%22%3A%2220ecb1be-1c6f-45d3-860a-84d74a098c75%22%2C%22previousSessionID%22%3A%22%22%2C%22UTCOffset%22%3A300%2C%22targetDataConsumers%22%3A%22PRODUCTION%22%2C%22logLevel%22%3A0%7D; " +
                //"clientDetails=%7B%22applicationName%22%3A%22OVP%22%2C%22applicationVersion%22%3A%222.3.0.1%22%2C%22apiVersion%22%3A%222.3.0.1%22%2C%22formFactor%22%3A%22PC%22%2C%22triggeredBy%22%3A%7B%22initiator%22%3A%22user%22%2C%22link%22%3A%22undefined%22%7D%2C%22deviceModel%22%3A%22Win32%22%2C%22deviceOS%22%3A%22Mozilla/5.0%20%28compatible%3B%20MSIE%209.0%3B%20Windows%20NT%206.1%3B%20WOW64%3B%20Trident/5.0%3B%20SLCC2%3B%20.NET%20CLR%202.0.50727%3B%20Media%20Center%20PC%206.0%3B%20MS-RTC%20LM%208%3B%20.NET%20CLR%203.5.30729%3B%20.NET%20CLR%203.0.30729%3B%20.NET4.0E%3B%20.NET%20CLR%201.1.4322%3B%20InfoPath.3%3B%20.NET4.0C%3B%20Zune%204.7%29%22%2C%22deviceID%22%3A%2270a8-486a6692-b83ed2bb-a2c849ec-3984%22%7D";

                string deviceIdHeaderString = HttpUtility.UrlEncode(myModel.DeviceId);
                string headerString = HttpUtility.UrlEncode(myModel.HeaderOptionsJson);
                string clientDetailsString = HttpUtility.UrlEncode(myModel.ClientDetailsJson);
                string wayFarer = HttpUtility.UrlEncode(myModel.Wayfarer);


                request.Headers["Cookie"] = "device_id=" + deviceIdHeaderString + "; header=" + headerString + "; clientDetails=" + clientDetailsString + "; Wayfarer=" + wayFarer + "; ";



                parameters["request"] = request;

                //response = (HttpWebResponse)request.GetResponse();
                request.BeginGetResponse(new AsyncCallback(GetVsGuidResponseCallback), parameters);
                
            }
            catch (Exception e) 
            { }


        }
        
        private void GetVsGuidResponseCallback(IAsyncResult asynchronousResult)
        {

            //HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
            HttpWebRequest request = (HttpWebRequest)parameters["request"];
            
            // End the operation
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            System.IO.Stream streamResponse = response.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse);

            string vsGuid = "";
            if (true)
            {
                string decodedString = HttpUtility.UrlDecode(response.Headers["Set-Cookie"].ToString().Replace("vs_guid=", ""));

                if (decodedString.IndexOf(';') > 0)
                {
                    vsGuid = decodedString.Substring(0, decodedString.IndexOf(';'));
                }
                else
                {
                    vsGuid = decodedString;
                }

            }

            parameters.Add("vs_guid", vsGuid);


            if (parameters["vs_guid"] != null)
            {
                if (!string.IsNullOrEmpty(parameters["vs_guid"].ToString()))
                {
                    ((TWCModel)parameters["myModel"]).Vs_Guid = vsGuid;
                }
            }


            if (response.Cookies != null)
            {
                foreach (Cookie currentCookie in response.Cookies)
                {
                    cookieCollection.Add(currentCookie);
                }
            }


            //string authString = streamRead.ReadToEnd();


            // Close the stream object
            streamResponse.Close();
            streamRead.Close();

            // Release the HttpWebResponse
            response.Close();
            OnLoginFinished(true);
        }




        public void ChangeChannel(string channel, string macAddress, TWCModel myModel)
        {

            ChangeChannelRequest(channel, macAddress, myModel);
            
        }

        private void ChangeChannelRequest(string channel, string macAddress, TWCModel myModel)
        {
            //living room
            //00:21:BE:19:9B:9C

            //dvr
            //00%3A21%3ABE%3ADF%3AA8%3A6C



            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://video2.timewarnercable.com/exec/proxy.cfm");
                //request.CookieContainer = cookieContainer;
                request.Headers["X-Requested-With"] = @"XMLHttpRequest";
                request.Accept = "application/json, text/javascript, */*; q=0.01";
                request.Headers["Referer"] = "https://video2.timewarnercable.com/js/libs/iframe/clientaccesspolicy.html";
                request.Headers["Accept-Language"] = "en-us";
                request.Headers["Accept-Encoding"] = "gzip, deflate";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
                request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                request.Headers["Cache-Control"] = "no-cache";

                //request.Accept = "application/json, text/javascript, */*; q=0.01";
                //request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                //request.Headers.Add("X-Requested-With", @"XMLHttpRequest");
                //request.Referer = "https://video2.timewarnercable.com/js/libs/iframe/clientaccesspolicy.html";
                //request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-us");
                //request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
                //request.Headers.Set(HttpRequestHeader.CacheControl, "no-cache");

                //request.Headers.Set(HttpRequestHeader.Cookie, @"rememberme=eyJ1c2VybmFtZSI6IkJpZ0JyaTQxIiwiY2MiOnRydWUsImxhc3RfYWN0aXZpdHkiOiIiLCJsYXN0X2FjdGlvbiI6IiIsImxhc3RfY2hhbm5lbCI6IiIsImZpbHRlcnN0YXRlIjoiYWxsIn0%3D; device_id=70a8-486a6692-b83ed2bb-a2c849ec-3984; Wayfarer=xV%2FYFSxVUwpKfSwHktQ58zlo0ORQLbHO7VDd32rUnWqP9xbZGHH%2BF9hOszgzWzL%2Fv9nJIK293fzwTsp5OkSZM5b1pWGYZPCS5UuF68OCWkkr2WrKrxUOhyXEwlE2UTCTgBX5yaBmvCUC95TjP%2BDDMR5I6pvGHukY%2Bv4y24xHe38G6qvZfkk9KXFs5fWy97M%2B8hmu1Y9CIAdHbrLDD8S9vq7vXQ3Md%2FdcqK1pr4OvnNuXIsQad61ckg%3D%3D; customer_info=eyJhY2NvdW50SG9sZGVyIjoiQnJpYW4gU2NvdHQifQ%3D%3D; vs_guid=C2GSYvgjlkFtTUGBT8pVjnN3ViafxjoaKTo3dE%2F%2FwEmOegi3RaAqcg%3D%3D; division=eyJkaXZpc2lvbiI6IkNUWC44MjYwIiwiemlwQ29kZSI6Ijc4NjM0LTUzNjEiLCJzdGF0ZSI6IlRYIiwic3Vic2NyaWJlZFR2UGFja2FnZVN0YW5kYXJkVHlwZXMiOlsiREdCU1QiLCJFUUNWSCIsIkJBU0lDIiwiQ1BTVFIiLCJTRFZSUyIsIlBTU1RaIl0sInJvYWRSdW5uZXJTdGFuZGFyZFR5cGUiOiJSUlNURCJ9; ovpsec=eyJjdXN0X2lkIjoiMjlmMDJmNmEyYzMwYzg2NDdhYjZlNDc5OGE5OWFmNTkyODQ0MmQzMSIsImR2ciI6MSwiZHZyX21zZyI6IiIsImxpdmV0diI6dHJ1ZSwib25kZW1hbmQiOnRydWUsImxpdmV0dl9tc2ciOiIiLCJwZXJtaXNzaW9ucyI6WyJ0dmV2ZXJ5d2hlcmUiLCJ0d2NhYmxldHYiLCJyZHZyIl19; wayfarer_ns=xV%2FYFSxVUwpKfSwHktQ58zlo0ORQLbHO7VDd32rUnWqP9xbZGHH%2BF9hOszgzWzL%2Fv9nJIK293fzwTsp5OkSZM5b1pWGYZPCS5UuF68OCWkkr2WrKrxUOhyXEwlE2UTCTgBX5yaBmvCUC95TjP%2BDDMR5I6pvGHukY%2Bv4y24xHe38G6qvZfkk9KXFs5fWy97M%2B8hmu1Y9CIAdHbrLDD8S9vq7vXQ3Md%2FdcqK1pr4OvnNuXIsQad61ckg%3D%3D; vs_guid_ns=C2GSYvgjlkFtTUGBT8pVjnN3ViafxjoaKTo3dE%2F%2FwEmOegi3RaAqcg%3D%3D; loc=eyJiZWhpbmRPd25Nb2RlbSI6dHJ1ZSwiaW5VUyI6dHJ1ZSwiaW5Vc09yVGVycml0b3J5Ijp0cnVlfQ%3D%3D; current_stb=eyJjbGllbnRUeXBlIjoiT0ROIiwiY2xpZW50VmVyc2lvbiI6IjYuMS4wLjUiLCJtYWNBZGRyZXNzIjoiMDA6MjE6QkU6REY6QTg6NkMiLCJuYW1lIjoiMDA6MjE6QkU6REY6QTg6NkMiLCJsaW5ldXBJZCI6IjI4NiIsImhlYWRlbmQiOiIxNjVfMjM3XzIxOF8yMjJfMjg2IiwiaXNEdnIiOnRydWUsImR2ciI6dHJ1ZX0%3D; ovp_stb=eyJzdGJJbmZvIjp7Im1hcyI6IjE2NS4yMzcuMjE4LjIyMiIsInNldFRvcEJveGVzIjpbeyJjbGllbnRUeXBlIjoiT0ROIiwiY2xpZW50VmVyc2lvbiI6IjYuMS4wLjUiLCJtYWNBZGRyZXNzIjoiMDA6MjE6QkU6REY6QTg6NkMiLCJuYW1lIjoiMDA6MjE6QkU6REY6QTg6NkMiLCJsaW5ldXBJZCI6IjI4NiIsImhlYWRlbmQiOiIxNjVfMjM3XzIxOF8yMjJfMjg2IiwiaXNEdnIiOnRydWUsImR2ciI6dHJ1ZX0seyJjbGllbnRUeXBlIjoiT0ROIiwiY2xpZW50VmVyc2lvbiI6IjYuMS4wLjUiLCJtYWNBZGRyZXNzIjoiMDA6MjE6QkU6MTk6OUI6OUMiLCJuYW1lIjoiMDA6MjE6QkU6MTk6OUI6OUMiLCJsaW5ldXBJZCI6IjI4NiIsImhlYWRlbmQiOiIxNjVfMjM3XzIxOF8yMjJfMjg2IiwiaXNEdnIiOmZhbHNlLCJkdnIiOmZhbHNlfV19LCJ0aW1lWm9uZSI6eyJ1dGNPZmZzZXQiOiItMzAwIiwidGltZVpvbmUiOiIyIn0sImhlYWRlbmQiOiIxNjVfMjM3XzIxOF8yMjJfMjg2In0%3D; eventgateway_stb_list=W3siY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibmFtZSI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjp0cnVlLCJkdnIiOnRydWV9LHsiY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibmFtZSI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjpmYWxzZSwiZHZyIjpmYWxzZX1d; vs_client_platform_version=twc-tv-ovp_1.0; parentalControls=true; header=%7B%22appStartup%22%3A%222013-08-09T20%3A00%3A30.345Z%22%2C%22sessionID%22%3A%2220ecb1be-1c6f-45d3-860a-84d74a098c75%22%2C%22previousSessionID%22%3A%22%22%2C%22UTCOffset%22%3A300%2C%22targetDataConsumers%22%3A%22PRODUCTION%22%2C%22logLevel%22%3A0%7D; clientDetails=%7B%22applicationName%22%3A%22OVP%22%2C%22applicationVersion%22%3A%222.3.0.1%22%2C%22apiVersion%22%3A%222.3.0.1%22%2C%22formFactor%22%3A%22PC%22%2C%22triggeredBy%22%3A%7B%22initiator%22%3A%22user%22%2C%22link%22%3A%22undefined%22%7D%2C%22deviceModel%22%3A%22Win32%22%2C%22deviceOS%22%3A%22Mozilla/5.0%20%28compatible%3B%20MSIE%209.0%3B%20Windows%20NT%206.1%3B%20WOW64%3B%20Trident/5.0%3B%20SLCC2%3B%20.NET%20CLR%202.0.50727%3B%20Media%20Center%20PC%206.0%3B%20MS-RTC%20LM%208%3B%20.NET%20CLR%203.5.30729%3B%20.NET%20CLR%203.0.30729%3B%20.NET4.0E%3B%20.NET%20CLR%201.1.4322%3B%20InfoPath.3%3B%20.NET4.0C%3B%20Zune%204.7%29%22%2C%22deviceID%22%3A%2270a8-486a6692-b83ed2bb-a2c849ec-3984%22%7D");
                //string headerSetString = "" +
                //"rememberme=eyJ1c2VybmFtZSI6IkJpZ0JyaTQxIiwiY2MiOnRydWUsImxhc3RfYWN0aXZpdHkiOiIiLCJsYXN0X2FjdGlvbiI6IiIsImxhc3RfY2hhbm5lbCI6IiIsImZpbHRlcnN0YXRlIjoiYWxsIn0%3D; " +
                //"device_id=70a8-486a6692-b83ed2bb-a2c849ec-3984; " +
                //"Wayfarer=xV%2FYFSxVUwpKfSwHktQ58zlo0ORQLbHO7VDd32rUnWqP9xbZGHH%2BF9hOszgzWzL%2Fv9nJIK293fzwTsp5OkSZM5b1pWGYZPCS5UuF68OCWkkr2WrKrxUOhyXEwlE2UTCTgBX5yaBmvCUC95TjP%2BDDMR5I6pvGHukY%2Bv4y24xHe38G6qvZfkk9KXFs5fWy97M%2B8hmu1Y9CIAdHbrLDD8S9vq7vXQ3Md%2FdcqK1pr4OvnNuXIsQad61ckg%3D%3D; " +
                //"customer_info=eyJhY2NvdW50SG9sZGVyIjoiQnJpYW4gU2NvdHQifQ%3D%3D; " +
                //"vs_guid=C2GSYvgjlkFtTUGBT8pVjnN3ViafxjoaKTo3dE%2F%2FwEmOegi3RaAqcg%3D%3D; " +
                //"division=eyJkaXZpc2lvbiI6IkNUWC44MjYwIiwiemlwQ29kZSI6Ijc4NjM0LTUzNjEiLCJzdGF0ZSI6IlRYIiwic3Vic2NyaWJlZFR2UGFja2FnZVN0YW5kYXJkVHlwZXMiOlsiREdCU1QiLCJFUUNWSCIsIkJBU0lDIiwiQ1BTVFIiLCJTRFZSUyIsIlBTU1RaIl0sInJvYWRSdW5uZXJTdGFuZGFyZFR5cGUiOiJSUlNURCJ9; " +
                //"ovpsec=eyJjdXN0X2lkIjoiMjlmMDJmNmEyYzMwYzg2NDdhYjZlNDc5OGE5OWFmNTkyODQ0MmQzMSIsImR2ciI6MSwiZHZyX21zZyI6IiIsImxpdmV0diI6dHJ1ZSwib25kZW1hbmQiOnRydWUsImxpdmV0dl9tc2ciOiIiLCJwZXJtaXNzaW9ucyI6WyJ0dmV2ZXJ5d2hlcmUiLCJ0d2NhYmxldHYiLCJyZHZyIl19; " +
                //"wayfarer_ns=xV%2FYFSxVUwpKfSwHktQ58zlo0ORQLbHO7VDd32rUnWqP9xbZGHH%2BF9hOszgzWzL%2Fv9nJIK293fzwTsp5OkSZM5b1pWGYZPCS5UuF68OCWkkr2WrKrxUOhyXEwlE2UTCTgBX5yaBmvCUC95TjP%2BDDMR5I6pvGHukY%2Bv4y24xHe38G6qvZfkk9KXFs5fWy97M%2B8hmu1Y9CIAdHbrLDD8S9vq7vXQ3Md%2FdcqK1pr4OvnNuXIsQad61ckg%3D%3D; " +
                //"vs_guid_ns=C2GSYvgjlkFtTUGBT8pVjnN3ViafxjoaKTo3dE%2F%2FwEmOegi3RaAqcg%3D%3D; " +
                //"loc=eyJiZWhpbmRPd25Nb2RlbSI6dHJ1ZSwiaW5VUyI6dHJ1ZSwiaW5Vc09yVGVycml0b3J5Ijp0cnVlfQ%3D%3D; " +
                //"current_stb=eyJjbGllbnRUeXBlIjoiT0ROIiwiY2xpZW50VmVyc2lvbiI6IjYuMS4wLjUiLCJtYWNBZGRyZXNzIjoiMDA6MjE6QkU6REY6QTg6NkMiLCJuYW1lIjoiMDA6MjE6QkU6REY6QTg6NkMiLCJsaW5ldXBJZCI6IjI4NiIsImhlYWRlbmQiOiIxNjVfMjM3XzIxOF8yMjJfMjg2IiwiaXNEdnIiOnRydWUsImR2ciI6dHJ1ZX0%3D; " +
                //"ovp_stb=eyJzdGJJbmZvIjp7Im1hcyI6IjE2NS4yMzcuMjE4LjIyMiIsInNldFRvcEJveGVzIjpbeyJjbGllbnRUeXBlIjoiT0ROIiwiY2xpZW50VmVyc2lvbiI6IjYuMS4wLjUiLCJtYWNBZGRyZXNzIjoiMDA6MjE6QkU6REY6QTg6NkMiLCJuYW1lIjoiMDA6MjE6QkU6REY6QTg6NkMiLCJsaW5ldXBJZCI6IjI4NiIsImhlYWRlbmQiOiIxNjVfMjM3XzIxOF8yMjJfMjg2IiwiaXNEdnIiOnRydWUsImR2ciI6dHJ1ZX0seyJjbGllbnRUeXBlIjoiT0ROIiwiY2xpZW50VmVyc2lvbiI6IjYuMS4wLjUiLCJtYWNBZGRyZXNzIjoiMDA6MjE6QkU6MTk6OUI6OUMiLCJuYW1lIjoiMDA6MjE6QkU6MTk6OUI6OUMiLCJsaW5ldXBJZCI6IjI4NiIsImhlYWRlbmQiOiIxNjVfMjM3XzIxOF8yMjJfMjg2IiwiaXNEdnIiOmZhbHNlLCJkdnIiOmZhbHNlfV19LCJ0aW1lWm9uZSI6eyJ1dGNPZmZzZXQiOiItMzAwIiwidGltZVpvbmUiOiIyIn0sImhlYWRlbmQiOiIxNjVfMjM3XzIxOF8yMjJfMjg2In0%3D; " +
                //"eventgateway_stb_list=W3siY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibmFtZSI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjp0cnVlLCJkdnIiOnRydWV9LHsiY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibmFtZSI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjpmYWxzZSwiZHZyIjpmYWxzZX1d; " +
                //"vs_client_platform_version=twc-tv-ovp_1.0; " +
                //"parentalControls=true; " +
                //"header=%7B%22appStartup%22%3A%222013-08-09T20%3A00%3A30.345Z%22%2C%22sessionID%22%3A%2220ecb1be-1c6f-45d3-860a-84d74a098c75%22%2C%22previousSessionID%22%3A%22%22%2C%22UTCOffset%22%3A300%2C%22targetDataConsumers%22%3A%22PRODUCTION%22%2C%22logLevel%22%3A0%7D; " +
                //"clientDetails=%7B%22applicationName%22%3A%22OVP%22%2C%22applicationVersion%22%3A%222.3.0.1%22%2C%22apiVersion%22%3A%222.3.0.1%22%2C%22formFactor%22%3A%22PC%22%2C%22triggeredBy%22%3A%7B%22initiator%22%3A%22user%22%2C%22link%22%3A%22undefined%22%7D%2C%22deviceModel%22%3A%22Win32%22%2C%22deviceOS%22%3A%22Mozilla/5.0%20%28compatible%3B%20MSIE%209.0%3B%20Windows%20NT%206.1%3B%20WOW64%3B%20Trident/5.0%3B%20SLCC2%3B%20.NET%20CLR%202.0.50727%3B%20Media%20Center%20PC%206.0%3B%20MS-RTC%20LM%208%3B%20.NET%20CLR%203.5.30729%3B%20.NET%20CLR%203.0.30729%3B%20.NET4.0E%3B%20.NET%20CLR%201.1.4322%3B%20InfoPath.3%3B%20.NET4.0C%3B%20Zune%204.7%29%22%2C%22deviceID%22%3A%2270a8-486a6692-b83ed2bb-a2c849ec-3984%22%7D";

                string deviceIdHeaderString = HttpUtility.UrlEncode(myModel.DeviceId);
                string headerString = HttpUtility.UrlEncode(myModel.HeaderOptionsJson);
                string clientDetailsString = HttpUtility.UrlEncode(myModel.ClientDetailsJson);
                string wayFarer = HttpUtility.UrlEncode(myModel.Wayfarer);
                string vs_guid = HttpUtility.UrlEncode(myModel.Vs_Guid);

                request.Headers["Cookie"]="device_id=" + deviceIdHeaderString + "; header=" + headerString + "; clientDetails=" + clientDetailsString + "; Wayfarer=" + wayFarer + "; vs_guid=" + vs_guid + ";";

                request.Method = "POST";
                //request.ServicePoint.Expect100Continue = false;

                parameters["request"] = request;
                parameters["myModel"] = myModel;
                parameters["macAddress"] = macAddress;
                parameters["channel"] = channel;

                request.BeginGetRequestStream(new AsyncCallback(ChangeChannelRequestStreamCallback), parameters);

            }
            catch (WebException e)
            {
               
            }

        }

        private void ChangeChannelRequestStreamCallback(IAsyncResult asynchronousResult)
        {


            //HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
            HttpWebRequest request = (HttpWebRequest)parameters["request"];
            //request.CookieContainer = cookieContainer;
            string macAddress = HttpUtility.UrlEncode(parameters["macAddress"].ToString());
            string channel = parameters["channel"].ToString();
            string body = @"apicall=tune-channel&mac=" + macAddress + "&channel=" + channel + "&method=POST";
            byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
            //request.ContentLength = postBytes.Length;


            System.IO.Stream stream = request.EndGetRequestStream(asynchronousResult);
            stream.Write(postBytes, 0, postBytes.Length);
            stream.Close();

            // Start the asynchronous operation to get the response

            parameters["request"] = request;

            request.BeginGetResponse(new AsyncCallback(ChangeChannelResponseCallback), parameters);

        }


        private void ChangeChannelResponseCallback(IAsyncResult asynchronousResult)
        {

            //HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
            HttpWebRequest request = (HttpWebRequest)parameters["request"];
            //request.CookieContainer = cookieContainer;
            // End the operation
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            System.IO.Stream streamResponse = response.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse);

            foreach (Cookie currentCookie in response.Cookies)
            {
                cookieCollection.Add(currentCookie);
            }

            // Close the stream object
            streamResponse.Close();
            streamRead.Close();

            // Release the HttpWebResponse
            response.Close();
            OnChangeChannelFinished();
        }



        public void GetDevices(TWCModel myModel)
        {

            GetDevicesRequest(myModel);

        }

        private void GetDevicesRequest(TWCModel myModel)
        {

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://video2.timewarnercable.com/call?url=https://video-services.timewarnercable.com/api/v3/secured/stb");//&_=1376244308832");
                //request.CookieContainer = cookieContainer;
                request.Headers["X-Requested-With"]= @"XMLHttpRequest";
                request.Accept = "application/json, text/javascript, */*; q=0.01";
                request.Headers["Referer"] = "https://video2.timewarnercable.com/js/libs/iframe/clientaccesspolicy.html";
                request.Headers["Accept-Language"]= "en-us";
                request.Headers["Accept-Encoding"]="gzip, deflate";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
                //request.Headers.Set(HttpRequestHeader.Cookie, @"rememberme=eyJ1c2VybmFtZSI6IkJpZ0JyaTQxIiwiY2MiOnRydWUsImxhc3RfYWN0aXZpdHkiOiIiLCJsYXN0X2FjdGlvbiI6IiIsImxhc3RfY2hhbm5lbCI6IiJ9; eventgateway_stb_list=W3siY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibmFtZSI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjp0cnVlLCJkdnIiOnRydWV9LHsiY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibmFtZSI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjpmYWxzZSwiZHZyIjpmYWxzZX1d; current_stb=eyJjbGllbnRUeXBlIjoiT0ROIiwiY2xpZW50VmVyc2lvbiI6IjYuMS4wLjUiLCJtYWNBZGRyZXNzIjoiMDA6MjE6QkU6MTk6OUI6OUMiLCJuYW1lIjoiMDA6MjE6QkU6MTk6OUI6OUMiLCJsaW5ldXBJZCI6IjI4NiIsImhlYWRlbmQiOiIxNjVfMjM3XzIxOF8yMjJfMjg2IiwiaXNEdnIiOmZhbHNlLCJkdnIiOmZhbHNlfQ%3D%3D; device_id=abe8-38058139-bc0c6ecc-2dd30c98-00c9; Wayfarer=xV%2FYFSxVUwpKfSwHktQ58zlo0ORQLbHO7VDd32rUnWqP9xbZGHH%2BF3NEUaDrOI3sEjTBEcbK0QjwTsp5OkSZM4SJ5ARIaI6nO7BU30KaH3sMVRry69aIBt%2B30%2Bt9TlDoHDjMXunzjXXd7ZjvA0miUn1slMfQN6kSPiz8lgCLr%2BfaeGUjhLq4TneuNVkckO6Jjs2z7G1aRRRjKTUd%2F%2BRgT5eChs3F4tOUdi3VaL2TV%2BmPrpY4tgMUFQ%3D%3D; customer_info=eyJhY2NvdW50SG9sZGVyIjoiQnJpYW4gU2NvdHQifQ%3D%3D; vs_guid=C2GSYvgjlkFtTUGBT8pVjnN3ViafxjoaKTo3dE%2F%2FwEmOegi3RaAqcg%3D%3D; division=eyJkaXZpc2lvbiI6IkNUWC44MjYwIiwiemlwQ29kZSI6Ijc4NjM0LTUzNjEiLCJzdGF0ZSI6IlRYIiwic3Vic2NyaWJlZFR2UGFja2FnZVN0YW5kYXJkVHlwZXMiOlsiREdCU1QiLCJFUUNWSCIsIkJBU0lDIiwiQ1BTVFIiLCJTRFZSUyIsIlBTU1RaIl0sInJvYWRSdW5uZXJTdGFuZGFyZFR5cGUiOiJSUlNURCJ9; ovpsec=eyJjdXN0X2lkIjoiMjlmMDJmNmEyYzMwYzg2NDdhYjZlNDc5OGE5OWFmNTkyODQ0MmQzMSIsImR2ciI6MSwiZHZyX21zZyI6IiIsImxpdmV0diI6dHJ1ZSwib25kZW1hbmQiOnRydWUsImxpdmV0dl9tc2ciOiIiLCJwZXJtaXNzaW9ucyI6WyJyZHZyIiwidHZldmVyeXdoZXJlIiwidHdjYWJsZXR2Il19; wayfarer_ns=xV%2FYFSxVUwpKfSwHktQ58zlo0ORQLbHO7VDd32rUnWqP9xbZGHH%2BF3NEUaDrOI3sEjTBEcbK0QjwTsp5OkSZM4SJ5ARIaI6nO7BU30KaH3sMVRry69aIBt%2B30%2Bt9TlDoHDjMXunzjXXd7ZjvA0miUn1slMfQN6kSPiz8lgCLr%2BfaeGUjhLq4TneuNVkckO6Jjs2z7G1aRRRjKTUd%2F%2BRgT5eChs3F4tOUdi3VaL2TV%2BmPrpY4tgMUFQ%3D%3D; vs_guid_ns=C2GSYvgjlkFtTUGBT8pVjnN3ViafxjoaKTo3dE%2F%2FwEmOegi3RaAqcg%3D%3D; loc=eyJiZWhpbmRPd25Nb2RlbSI6dHJ1ZSwiaW5VUyI6dHJ1ZSwiaW5Vc09yVGVycml0b3J5Ijp0cnVlfQ%3D%3D; vs_client_platform_version=twc-tv-ovp_1.0; parentalControls=true; header=%7B%22appStartup%22%3A%222013-08-11T18%3A03%3A57.383Z%22%2C%22sessionID%22%3A%22dddc1ea9-f29e-42bc-a360-9e95d9040883%22%2C%22previousSessionID%22%3A%22%22%2C%22UTCOffset%22%3A300%2C%22targetDataConsumers%22%3A%22PRODUCTION%22%2C%22logLevel%22%3A0%7D; clientDetails=%7B%22applicationName%22%3A%22OVP%22%2C%22applicationVersion%22%3A%222.3.0.1%22%2C%22apiVersion%22%3A%222.3.0.1%22%2C%22formFactor%22%3A%22PC%22%2C%22triggeredBy%22%3A%7B%22initiator%22%3A%22user%22%2C%22link%22%3A%22undefined%22%7D%2C%22deviceModel%22%3A%22Win32%22%2C%22deviceOS%22%3A%22Mozilla/5.0%20%28compatible%3B%20MSIE%209.0%3B%20Windows%20NT%206.1%3B%20WOW64%3B%20Trident/5.0%3B%20SLCC2%3B%20.NET%20CLR%202.0.50727%3B%20Media%20Center%20PC%206.0%3B%20MS-RTC%20LM%208%3B%20.NET%20CLR%203.5.30729%3B%20.NET%20CLR%203.0.30729%3B%20.NET4.0E%3B%20.NET%20CLR%201.1.4322%3B%20InfoPath.3%3B%20.NET4.0C%3B%20Zune%204.7%29%22%2C%22deviceID%22%3A%22abe8-38058139-bc0c6ecc-2dd30c98-00c9%22%7D");

                string deviceIdHeaderString = HttpUtility.UrlEncode(myModel.DeviceId);
                string headerString = HttpUtility.UrlEncode(myModel.HeaderOptionsJson);
                string clientDetailsString = HttpUtility.UrlEncode(myModel.ClientDetailsJson);
                string wayFarer = HttpUtility.UrlEncode(myModel.Wayfarer);
                string vs_guid = HttpUtility.UrlEncode(myModel.Vs_Guid);

                request.Headers["Cookie"] = "device_id=" + deviceIdHeaderString + "; header=" + headerString + "; clientDetails=" + clientDetailsString + "; Wayfarer=" + wayFarer + "; vs_guid=" + vs_guid + ";";

                parameters["request"] = request;

                //response = (HttpWebResponse)request.GetResponse();
                request.BeginGetResponse(new AsyncCallback(GetDevicesResponseCallback), parameters);
            }
            catch (WebException e)
            {
             
            }

        }

        private void GetDevicesResponseCallback(IAsyncResult asynchronousResult)
        {

            //HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
            HttpWebRequest request = (HttpWebRequest)parameters["request"];
            
            // End the operation
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            System.IO.Stream streamResponse = response.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse);
         

            string devicesReturn = streamRead.ReadToEnd();

            if (response.Cookies != null)
            {
                foreach (Cookie currentCookie in response.Cookies)
                {
                    cookieCollection.Add(currentCookie);
                }
            }
           
            // Close the stream object
            streamResponse.Close();
            streamRead.Close();

            // Release the HttpWebResponse
            response.Close();


            StbInfoObject returnSetTopBoxes = JsonConvert.DeserializeObject<StbInfoObject>(devicesReturn);

            if (returnSetTopBoxes != null)
            {
                myModel.Boxes = returnSetTopBoxes.stbInfo;
            }


            OnGetDevicesFinished();
        }


        public void GetGuide(string headEndId,TWCModel myModel)
        {
            parameters["headendId"] = headEndId;

            GetGuideRequest(myModel);

        }

        private void GetGuideRequest(TWCModel myModel)
        {

            try
            {
                string headEndId = parameters["headendId"].ToString();

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://video2.timewarnercable.com/call?url=https%3A%2F%2Fvideo-services.timewarnercable.com%2Fapi%2Fv3%2Fsecured%2Fchannels%3Fheadend%3D"+headEndId);//165_237_218_222_286");
                //request.CookieContainer = cookieContainer;
                request.Headers["X-Requested-With"] = @"XMLHttpRequest";
                request.Accept = "*/*";
                request.Headers["Referer"] = "https://video2.timewarnercable.com/js/libs/iframe/clientaccesspolicy.html";
                request.Headers["Accept-Language"] = "en-us";
                request.Headers["Accept-Encoding"] = "gzip, deflate";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
                //request.Headers.Set(HttpRequestHeader.Cookie, @"rememberme=eyJ1c2VybmFtZSI6IkJpZ0JyaTQxIiwiY2MiOnRydWUsImxhc3RfYWN0aXZpdHkiOiIiLCJsYXN0X2FjdGlvbiI6IiIsImxhc3RfY2hhbm5lbCI6IiJ9; eventgateway_stb_list=W3siY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibmFtZSI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjp0cnVlLCJkdnIiOnRydWV9LHsiY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibmFtZSI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjpmYWxzZSwiZHZyIjpmYWxzZX1d; current_stb=eyJjbGllbnRUeXBlIjoiT0ROIiwiY2xpZW50VmVyc2lvbiI6IjYuMS4wLjUiLCJtYWNBZGRyZXNzIjoiMDA6MjE6QkU6MTk6OUI6OUMiLCJuYW1lIjoiMDA6MjE6QkU6MTk6OUI6OUMiLCJsaW5ldXBJZCI6IjI4NiIsImhlYWRlbmQiOiIxNjVfMjM3XzIxOF8yMjJfMjg2IiwiaXNEdnIiOmZhbHNlLCJkdnIiOmZhbHNlfQ%3D%3D; device_id=abe8-38058139-bc0c6ecc-2dd30c98-00c9; Wayfarer=xV%2FYFSxVUwpKfSwHktQ58zlo0ORQLbHO7VDd32rUnWqP9xbZGHH%2BF3NEUaDrOI3sEjTBEcbK0QjwTsp5OkSZM4SJ5ARIaI6nO7BU30KaH3sMVRry69aIBt%2B30%2Bt9TlDoHDjMXunzjXXd7ZjvA0miUn1slMfQN6kSPiz8lgCLr%2BfaeGUjhLq4TneuNVkckO6Jjs2z7G1aRRRjKTUd%2F%2BRgT5eChs3F4tOUdi3VaL2TV%2BmPrpY4tgMUFQ%3D%3D; customer_info=eyJhY2NvdW50SG9sZGVyIjoiQnJpYW4gU2NvdHQifQ%3D%3D; vs_guid=C2GSYvgjlkFtTUGBT8pVjnN3ViafxjoaKTo3dE%2F%2FwEmOegi3RaAqcg%3D%3D; division=eyJkaXZpc2lvbiI6IkNUWC44MjYwIiwiemlwQ29kZSI6Ijc4NjM0LTUzNjEiLCJzdGF0ZSI6IlRYIiwic3Vic2NyaWJlZFR2UGFja2FnZVN0YW5kYXJkVHlwZXMiOlsiREdCU1QiLCJFUUNWSCIsIkJBU0lDIiwiQ1BTVFIiLCJTRFZSUyIsIlBTU1RaIl0sInJvYWRSdW5uZXJTdGFuZGFyZFR5cGUiOiJSUlNURCJ9; ovpsec=eyJjdXN0X2lkIjoiMjlmMDJmNmEyYzMwYzg2NDdhYjZlNDc5OGE5OWFmNTkyODQ0MmQzMSIsImR2ciI6MSwiZHZyX21zZyI6IiIsImxpdmV0diI6dHJ1ZSwib25kZW1hbmQiOnRydWUsImxpdmV0dl9tc2ciOiIiLCJwZXJtaXNzaW9ucyI6WyJyZHZyIiwidHZldmVyeXdoZXJlIiwidHdjYWJsZXR2Il19; wayfarer_ns=xV%2FYFSxVUwpKfSwHktQ58zlo0ORQLbHO7VDd32rUnWqP9xbZGHH%2BF3NEUaDrOI3sEjTBEcbK0QjwTsp5OkSZM4SJ5ARIaI6nO7BU30KaH3sMVRry69aIBt%2B30%2Bt9TlDoHDjMXunzjXXd7ZjvA0miUn1slMfQN6kSPiz8lgCLr%2BfaeGUjhLq4TneuNVkckO6Jjs2z7G1aRRRjKTUd%2F%2BRgT5eChs3F4tOUdi3VaL2TV%2BmPrpY4tgMUFQ%3D%3D; vs_guid_ns=C2GSYvgjlkFtTUGBT8pVjnN3ViafxjoaKTo3dE%2F%2FwEmOegi3RaAqcg%3D%3D; loc=eyJiZWhpbmRPd25Nb2RlbSI6dHJ1ZSwiaW5VUyI6dHJ1ZSwiaW5Vc09yVGVycml0b3J5Ijp0cnVlfQ%3D%3D; vs_client_platform_version=twc-tv-ovp_1.0; parentalControls=true; header=%7B%22appStartup%22%3A%222013-08-11T18%3A03%3A57.383Z%22%2C%22sessionID%22%3A%22dddc1ea9-f29e-42bc-a360-9e95d9040883%22%2C%22previousSessionID%22%3A%22%22%2C%22UTCOffset%22%3A300%2C%22targetDataConsumers%22%3A%22PRODUCTION%22%2C%22logLevel%22%3A0%7D; clientDetails=%7B%22applicationName%22%3A%22OVP%22%2C%22applicationVersion%22%3A%222.3.0.1%22%2C%22apiVersion%22%3A%222.3.0.1%22%2C%22formFactor%22%3A%22PC%22%2C%22triggeredBy%22%3A%7B%22initiator%22%3A%22user%22%2C%22link%22%3A%22undefined%22%7D%2C%22deviceModel%22%3A%22Win32%22%2C%22deviceOS%22%3A%22Mozilla/5.0%20%28compatible%3B%20MSIE%209.0%3B%20Windows%20NT%206.1%3B%20WOW64%3B%20Trident/5.0%3B%20SLCC2%3B%20.NET%20CLR%202.0.50727%3B%20Media%20Center%20PC%206.0%3B%20MS-RTC%20LM%208%3B%20.NET%20CLR%203.5.30729%3B%20.NET%20CLR%203.0.30729%3B%20.NET4.0E%3B%20.NET%20CLR%201.1.4322%3B%20InfoPath.3%3B%20.NET4.0C%3B%20Zune%204.7%29%22%2C%22deviceID%22%3A%22abe8-38058139-bc0c6ecc-2dd30c98-00c9%22%7D");


                string deviceIdHeaderString = HttpUtility.UrlEncode(myModel.DeviceId);
                string headerString = HttpUtility.UrlEncode(myModel.HeaderOptionsJson);
                string clientDetailsString = HttpUtility.UrlEncode(myModel.ClientDetailsJson);
                string wayFarer = HttpUtility.UrlEncode(myModel.Wayfarer);
                string vs_guid = HttpUtility.UrlEncode(myModel.Vs_Guid);

                request.Headers["Cookie"] = "device_id=" + deviceIdHeaderString + "; header=" + headerString + "; clientDetails=" + clientDetailsString + "; Wayfarer=" + wayFarer + "; vs_guid=" + vs_guid + ";";

                parameters["request"] = request;

                //response = (HttpWebResponse)request.GetResponse();
                request.BeginGetResponse(new AsyncCallback(GetGuideResponseCallback), parameters);
            }
            catch (WebException e)
            {

            }

        }

        private void GetGuideResponseCallback(IAsyncResult asynchronousResult)
        {

            //HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
            HttpWebRequest request = (HttpWebRequest)parameters["request"];
            
            // End the operation
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            System.IO.Stream streamResponse = response.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse);

            
            string devicesReturn = streamRead.ReadToEnd();

            if (response.Cookies != null)
            {
                foreach (Cookie currentCookie in response.Cookies)
                {
                    cookieCollection.Add(currentCookie);
                }
            }

            // Close the stream object
            streamResponse.Close();
            streamRead.Close();

            // Release the HttpWebResponse
            response.Close();


            ChanelObject returnChannels = JsonConvert.DeserializeObject<ChanelObject>(devicesReturn);

            if (returnChannels != null)
            {
               myModel.Channels = returnChannels.channels;
            }

            
            OnGetGuideFinished();
        }




        public void GetShows(string headEndId,string hourBegin,string hourEnd,string startChannel,string endChannel,  TWCModel myModel)
        {
            /*
            parameters["headendId"] = headEndId;
            parameters["hourBegin"] = hourBegin;
            parameters["hourEnd"] = hourEnd;
            parameters["startChannel"] = startChannel;
            parameters["endChannel"] = endChannel;
            */
            //GetShowsRequest(myModel);

            try
            {/*
                string headEndId = parameters["headendId"].ToString();
                string hourBegin = parameters["hourBegin"].ToString();
                string hourEnd = parameters["hourEnd"].ToString();
                string startChannel = parameters["startChannel"].ToString();
                string endChannel = parameters["endChannel"].ToString();
                */
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://video2.timewarnercable.com/exec/proxy.cfm?apicall=guide&headend=" + headEndId + "&hourBegin=" + hourBegin + "&hourEnd=" + hourEnd + "&channelIndexBegin=" + startChannel + "&channelIndexEnd=" + endChannel);//165_237_218_222_286");
                //request.CookieContainer = cookieContainer;
                request.Headers["X-Requested-With"] = @"XMLHttpRequest";
                request.Accept = "application/json, text/javascript, */*; q=0.01";
                request.Headers["Referer"] = "https://video2.timewarnercable.com/js/libs/iframe/clientaccesspolicy.html";
                request.Headers["Accept-Language"] = "en-us";
                request.Headers["Accept-Encoding"] = "gzip, deflate";
                request.Headers["Connection"] = "Keep-Alive";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
                //request.Headers.Set(HttpRequestHeader.Cookie, @"rememberme=eyJ1c2VybmFtZSI6IkJpZ0JyaTQxIiwiY2MiOnRydWUsImxhc3RfYWN0aXZpdHkiOiIiLCJsYXN0X2FjdGlvbiI6IiIsImxhc3RfY2hhbm5lbCI6IiJ9; eventgateway_stb_list=W3siY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibmFtZSI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjp0cnVlLCJkdnIiOnRydWV9LHsiY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibmFtZSI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjpmYWxzZSwiZHZyIjpmYWxzZX1d; current_stb=eyJjbGllbnRUeXBlIjoiT0ROIiwiY2xpZW50VmVyc2lvbiI6IjYuMS4wLjUiLCJtYWNBZGRyZXNzIjoiMDA6MjE6QkU6MTk6OUI6OUMiLCJuYW1lIjoiMDA6MjE6QkU6MTk6OUI6OUMiLCJsaW5ldXBJZCI6IjI4NiIsImhlYWRlbmQiOiIxNjVfMjM3XzIxOF8yMjJfMjg2IiwiaXNEdnIiOmZhbHNlLCJkdnIiOmZhbHNlfQ%3D%3D; device_id=abe8-38058139-bc0c6ecc-2dd30c98-00c9; Wayfarer=xV%2FYFSxVUwpKfSwHktQ58zlo0ORQLbHO7VDd32rUnWqP9xbZGHH%2BF3NEUaDrOI3sEjTBEcbK0QjwTsp5OkSZM4SJ5ARIaI6nO7BU30KaH3sMVRry69aIBt%2B30%2Bt9TlDoHDjMXunzjXXd7ZjvA0miUn1slMfQN6kSPiz8lgCLr%2BfaeGUjhLq4TneuNVkckO6Jjs2z7G1aRRRjKTUd%2F%2BRgT5eChs3F4tOUdi3VaL2TV%2BmPrpY4tgMUFQ%3D%3D; customer_info=eyJhY2NvdW50SG9sZGVyIjoiQnJpYW4gU2NvdHQifQ%3D%3D; vs_guid=C2GSYvgjlkFtTUGBT8pVjnN3ViafxjoaKTo3dE%2F%2FwEmOegi3RaAqcg%3D%3D; division=eyJkaXZpc2lvbiI6IkNUWC44MjYwIiwiemlwQ29kZSI6Ijc4NjM0LTUzNjEiLCJzdGF0ZSI6IlRYIiwic3Vic2NyaWJlZFR2UGFja2FnZVN0YW5kYXJkVHlwZXMiOlsiREdCU1QiLCJFUUNWSCIsIkJBU0lDIiwiQ1BTVFIiLCJTRFZSUyIsIlBTU1RaIl0sInJvYWRSdW5uZXJTdGFuZGFyZFR5cGUiOiJSUlNURCJ9; ovpsec=eyJjdXN0X2lkIjoiMjlmMDJmNmEyYzMwYzg2NDdhYjZlNDc5OGE5OWFmNTkyODQ0MmQzMSIsImR2ciI6MSwiZHZyX21zZyI6IiIsImxpdmV0diI6dHJ1ZSwib25kZW1hbmQiOnRydWUsImxpdmV0dl9tc2ciOiIiLCJwZXJtaXNzaW9ucyI6WyJyZHZyIiwidHZldmVyeXdoZXJlIiwidHdjYWJsZXR2Il19; wayfarer_ns=xV%2FYFSxVUwpKfSwHktQ58zlo0ORQLbHO7VDd32rUnWqP9xbZGHH%2BF3NEUaDrOI3sEjTBEcbK0QjwTsp5OkSZM4SJ5ARIaI6nO7BU30KaH3sMVRry69aIBt%2B30%2Bt9TlDoHDjMXunzjXXd7ZjvA0miUn1slMfQN6kSPiz8lgCLr%2BfaeGUjhLq4TneuNVkckO6Jjs2z7G1aRRRjKTUd%2F%2BRgT5eChs3F4tOUdi3VaL2TV%2BmPrpY4tgMUFQ%3D%3D; vs_guid_ns=C2GSYvgjlkFtTUGBT8pVjnN3ViafxjoaKTo3dE%2F%2FwEmOegi3RaAqcg%3D%3D; loc=eyJiZWhpbmRPd25Nb2RlbSI6dHJ1ZSwiaW5VUyI6dHJ1ZSwiaW5Vc09yVGVycml0b3J5Ijp0cnVlfQ%3D%3D; vs_client_platform_version=twc-tv-ovp_1.0; parentalControls=true; header=%7B%22appStartup%22%3A%222013-08-11T18%3A03%3A57.383Z%22%2C%22sessionID%22%3A%22dddc1ea9-f29e-42bc-a360-9e95d9040883%22%2C%22previousSessionID%22%3A%22%22%2C%22UTCOffset%22%3A300%2C%22targetDataConsumers%22%3A%22PRODUCTION%22%2C%22logLevel%22%3A0%7D; clientDetails=%7B%22applicationName%22%3A%22OVP%22%2C%22applicationVersion%22%3A%222.3.0.1%22%2C%22apiVersion%22%3A%222.3.0.1%22%2C%22formFactor%22%3A%22PC%22%2C%22triggeredBy%22%3A%7B%22initiator%22%3A%22user%22%2C%22link%22%3A%22undefined%22%7D%2C%22deviceModel%22%3A%22Win32%22%2C%22deviceOS%22%3A%22Mozilla/5.0%20%28compatible%3B%20MSIE%209.0%3B%20Windows%20NT%206.1%3B%20WOW64%3B%20Trident/5.0%3B%20SLCC2%3B%20.NET%20CLR%202.0.50727%3B%20Media%20Center%20PC%206.0%3B%20MS-RTC%20LM%208%3B%20.NET%20CLR%203.5.30729%3B%20.NET%20CLR%203.0.30729%3B%20.NET4.0E%3B%20.NET%20CLR%201.1.4322%3B%20InfoPath.3%3B%20.NET4.0C%3B%20Zune%204.7%29%22%2C%22deviceID%22%3A%22abe8-38058139-bc0c6ecc-2dd30c98-00c9%22%7D");


                string deviceIdHeaderString = HttpUtility.UrlEncode(myModel.DeviceId);
                string headerString = HttpUtility.UrlEncode(myModel.HeaderOptionsJson);
                string clientDetailsString = HttpUtility.UrlEncode(myModel.ClientDetailsJson);
                string wayFarer = HttpUtility.UrlEncode(myModel.Wayfarer);
                string vs_guid = HttpUtility.UrlEncode(myModel.Vs_Guid);

                request.Headers["Cookie"] = "device_id=" + deviceIdHeaderString + "; header=" + headerString + "; clientDetails=" + clientDetailsString + "; Wayfarer=" + wayFarer + "; vs_guid=" + vs_guid + ";";

                //parameters["request"] = request;

                //response = (HttpWebResponse)request.GetResponse();
                request.BeginGetResponse(new AsyncCallback(GetShowsResponseCallback), request);
            }
            catch (WebException e)
            {
                string hold = "";
            }

        }

        private void GetShowsRequest(TWCModel myModel)
        {



        }

        private void GetShowsResponseCallback(IAsyncResult asynchronousResult)
        {

            int minChannel = -1;
            int maxChannel = -1;

            try
            {

                HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
                //HttpWebRequest request = (HttpWebRequest)parameters["request"];
                //request.CookieContainer = cookieContainer;
                // End the operation
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
                System.IO.Stream streamResponse = response.GetResponseStream();
                StreamReader streamRead = new StreamReader(streamResponse);


                string showsReturn = streamRead.ReadToEnd();


                // Close the stream object
                streamResponse.Close();
                streamRead.Close();

                // Release the HttpWebResponse
                response.Close();


                ShowDataObject returnShows = JsonConvert.DeserializeObject<ShowDataObject>(showsReturn);

                if (myModel.Shows == null)
                {
                    myModel.Shows = new List<Show>();
                }

                if (returnShows != null)
                {
                    foreach (Show currentShow in returnShows.data.shows)
                    {
                        if (myModel.Shows.Where(x => x.episodeId == currentShow.episodeId && x.channelId == currentShow.channelId).Count() == 0)
                        {
                            myModel.Shows.Add(currentShow);
                        }
                    }

                }

                minChannel = returnShows.data.shows.Select(x => x.channelIdInt).Min();
                maxChannel = returnShows.data.shows.Select(x => x.channelIdInt).Max();
            }
            catch (Exception)
            {
                string hold = "";
            }
         
            OnGetShowsFinished(minChannel.ToString()+","+maxChannel.ToString());
        }


        public void GetRecordings(string macAddress, TWCModel myModel)
        {
            parameters["macAddress"] = macAddress;
            GetRecordingsRequest(myModel);

        }

        private void GetRecordingsRequest(TWCModel myModel)
        {

            try
            {
                string macAddress = HttpUtility.UrlEncode(parameters["macAddress"].ToString());
                
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://video2.timewarnercable.com/call?url=https://video-services.timewarnercable.com/api/v3/secured/dvr/recording/list?urlEncodedMacAddress="+macAddress);
                //request.CookieContainer = cookieContainer;
                request.Headers["X-Requested-With"] = @"XMLHttpRequest";
                request.Accept = "application/json, text/javascript, */*; q=0.01";
                request.Headers["Referer"] = "https://video2.timewarnercable.com/js/libs/iframe/clientaccesspolicy.html";
                request.Headers["Accept-Language"] = "en-us";
                request.Headers["Accept-Encoding"] = "gzip, deflate";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
                //request.Headers.Set(HttpRequestHeader.Cookie, @"rememberme=eyJ1c2VybmFtZSI6IkJpZ0JyaTQxIiwiY2MiOnRydWUsImxhc3RfYWN0aXZpdHkiOiIiLCJsYXN0X2FjdGlvbiI6IiIsImxhc3RfY2hhbm5lbCI6IiJ9; eventgateway_stb_list=W3siY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibmFtZSI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjp0cnVlLCJkdnIiOnRydWV9LHsiY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibmFtZSI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjpmYWxzZSwiZHZyIjpmYWxzZX1d; current_stb=eyJjbGllbnRUeXBlIjoiT0ROIiwiY2xpZW50VmVyc2lvbiI6IjYuMS4wLjUiLCJtYWNBZGRyZXNzIjoiMDA6MjE6QkU6MTk6OUI6OUMiLCJuYW1lIjoiMDA6MjE6QkU6MTk6OUI6OUMiLCJsaW5ldXBJZCI6IjI4NiIsImhlYWRlbmQiOiIxNjVfMjM3XzIxOF8yMjJfMjg2IiwiaXNEdnIiOmZhbHNlLCJkdnIiOmZhbHNlfQ%3D%3D; device_id=abe8-38058139-bc0c6ecc-2dd30c98-00c9; Wayfarer=xV%2FYFSxVUwpKfSwHktQ58zlo0ORQLbHO7VDd32rUnWqP9xbZGHH%2BF3NEUaDrOI3sEjTBEcbK0QjwTsp5OkSZM4SJ5ARIaI6nO7BU30KaH3sMVRry69aIBt%2B30%2Bt9TlDoHDjMXunzjXXd7ZjvA0miUn1slMfQN6kSPiz8lgCLr%2BfaeGUjhLq4TneuNVkckO6Jjs2z7G1aRRRjKTUd%2F%2BRgT5eChs3F4tOUdi3VaL2TV%2BmPrpY4tgMUFQ%3D%3D; customer_info=eyJhY2NvdW50SG9sZGVyIjoiQnJpYW4gU2NvdHQifQ%3D%3D; vs_guid=C2GSYvgjlkFtTUGBT8pVjnN3ViafxjoaKTo3dE%2F%2FwEmOegi3RaAqcg%3D%3D; division=eyJkaXZpc2lvbiI6IkNUWC44MjYwIiwiemlwQ29kZSI6Ijc4NjM0LTUzNjEiLCJzdGF0ZSI6IlRYIiwic3Vic2NyaWJlZFR2UGFja2FnZVN0YW5kYXJkVHlwZXMiOlsiREdCU1QiLCJFUUNWSCIsIkJBU0lDIiwiQ1BTVFIiLCJTRFZSUyIsIlBTU1RaIl0sInJvYWRSdW5uZXJTdGFuZGFyZFR5cGUiOiJSUlNURCJ9; ovpsec=eyJjdXN0X2lkIjoiMjlmMDJmNmEyYzMwYzg2NDdhYjZlNDc5OGE5OWFmNTkyODQ0MmQzMSIsImR2ciI6MSwiZHZyX21zZyI6IiIsImxpdmV0diI6dHJ1ZSwib25kZW1hbmQiOnRydWUsImxpdmV0dl9tc2ciOiIiLCJwZXJtaXNzaW9ucyI6WyJyZHZyIiwidHZldmVyeXdoZXJlIiwidHdjYWJsZXR2Il19; wayfarer_ns=xV%2FYFSxVUwpKfSwHktQ58zlo0ORQLbHO7VDd32rUnWqP9xbZGHH%2BF3NEUaDrOI3sEjTBEcbK0QjwTsp5OkSZM4SJ5ARIaI6nO7BU30KaH3sMVRry69aIBt%2B30%2Bt9TlDoHDjMXunzjXXd7ZjvA0miUn1slMfQN6kSPiz8lgCLr%2BfaeGUjhLq4TneuNVkckO6Jjs2z7G1aRRRjKTUd%2F%2BRgT5eChs3F4tOUdi3VaL2TV%2BmPrpY4tgMUFQ%3D%3D; vs_guid_ns=C2GSYvgjlkFtTUGBT8pVjnN3ViafxjoaKTo3dE%2F%2FwEmOegi3RaAqcg%3D%3D; loc=eyJiZWhpbmRPd25Nb2RlbSI6dHJ1ZSwiaW5VUyI6dHJ1ZSwiaW5Vc09yVGVycml0b3J5Ijp0cnVlfQ%3D%3D; vs_client_platform_version=twc-tv-ovp_1.0; parentalControls=true; header=%7B%22appStartup%22%3A%222013-08-11T18%3A03%3A57.383Z%22%2C%22sessionID%22%3A%22dddc1ea9-f29e-42bc-a360-9e95d9040883%22%2C%22previousSessionID%22%3A%22%22%2C%22UTCOffset%22%3A300%2C%22targetDataConsumers%22%3A%22PRODUCTION%22%2C%22logLevel%22%3A0%7D; clientDetails=%7B%22applicationName%22%3A%22OVP%22%2C%22applicationVersion%22%3A%222.3.0.1%22%2C%22apiVersion%22%3A%222.3.0.1%22%2C%22formFactor%22%3A%22PC%22%2C%22triggeredBy%22%3A%7B%22initiator%22%3A%22user%22%2C%22link%22%3A%22undefined%22%7D%2C%22deviceModel%22%3A%22Win32%22%2C%22deviceOS%22%3A%22Mozilla/5.0%20%28compatible%3B%20MSIE%209.0%3B%20Windows%20NT%206.1%3B%20WOW64%3B%20Trident/5.0%3B%20SLCC2%3B%20.NET%20CLR%202.0.50727%3B%20Media%20Center%20PC%206.0%3B%20MS-RTC%20LM%208%3B%20.NET%20CLR%203.5.30729%3B%20.NET%20CLR%203.0.30729%3B%20.NET4.0E%3B%20.NET%20CLR%201.1.4322%3B%20InfoPath.3%3B%20.NET4.0C%3B%20Zune%204.7%29%22%2C%22deviceID%22%3A%22abe8-38058139-bc0c6ecc-2dd30c98-00c9%22%7D");


                string deviceIdHeaderString = HttpUtility.UrlEncode(myModel.DeviceId);
                string headerString = HttpUtility.UrlEncode(myModel.HeaderOptionsJson);
                string clientDetailsString = HttpUtility.UrlEncode(myModel.ClientDetailsJson);
                string wayFarer = HttpUtility.UrlEncode(myModel.Wayfarer);
                string vs_guid = HttpUtility.UrlEncode(myModel.Vs_Guid);

                request.Headers["Cookie"] = "device_id=" + deviceIdHeaderString + "; header=" + headerString + "; clientDetails=" + clientDetailsString + "; Wayfarer=" + wayFarer + "; vs_guid=" + vs_guid + ";";

                parameters["request"] = request;

                //response = (HttpWebResponse)request.GetResponse();
                request.BeginGetResponse(new AsyncCallback(GetRecordingsResponseCallback), parameters);
            }
            catch (WebException e)
            {

            }

        }

        private void GetRecordingsResponseCallback(IAsyncResult asynchronousResult)
        {

            //HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
            HttpWebRequest request = (HttpWebRequest)parameters["request"];
            //request.CookieContainer = cookieContainer;
            // End the operation
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            System.IO.Stream streamResponse = response.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse);


            string recordingsReturn = streamRead.ReadToEnd();

            // Close the stream object
            streamResponse.Close();
            streamRead.Close();

            // Release the HttpWebResponse
            response.Close();


            RecordingObject returnRecordings = JsonConvert.DeserializeObject<RecordingObject>(recordingsReturn);

            if (returnRecordings != null)
            {
                myModel.Recordings = returnRecordings.recordings;
            }


            OnGetRecordingsFinished();
        }



        public void GetShowImage(string episodeId,int width,int height, TWCModel myModel)
        {
            parameters["episodeId"] = episodeId;
            parameters["width"] = width.ToString();
            parameters["height"] = height.ToString();

            GetShowImageRequest(myModel);

        }

        private void GetShowImageRequest(TWCModel myModel)
        {

            string episodeId = parameters["episodeId"].ToString();
            string width = parameters["width"].ToString();
            string height = parameters["height"].ToString();


            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://services.timewarnercable.com/imageserver/program/"+episodeId+"?width="+width+"&height="+height);
                //request.CookieContainer = cookieContainer;
              
                request.Accept = "image/png, image/svg+xml, image/*;q=0.8, */*;q=0.5";
                request.Headers["Referer"] = "http://video2.timewarnercable.com/DVR";
                request.Headers["Accept-Language"] = "en-US";
                //request.Headers["Accept-Encoding"] = "gzip, deflate, peerdist";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
                request.Headers["Connection"] = "Keep-Alive";
                //request.Headers.Set(HttpRequestHeader.Cookie, @"rememberme=eyJ1c2VybmFtZSI6IkJpZ0JyaTQxIiwiY2MiOnRydWUsImxhc3RfYWN0aXZpdHkiOiIiLCJsYXN0X2FjdGlvbiI6IiIsImxhc3RfY2hhbm5lbCI6IiJ9; eventgateway_stb_list=W3siY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibmFtZSI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjp0cnVlLCJkdnIiOnRydWV9LHsiY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibmFtZSI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjpmYWxzZSwiZHZyIjpmYWxzZX1d; current_stb=eyJjbGllbnRUeXBlIjoiT0ROIiwiY2xpZW50VmVyc2lvbiI6IjYuMS4wLjUiLCJtYWNBZGRyZXNzIjoiMDA6MjE6QkU6MTk6OUI6OUMiLCJuYW1lIjoiMDA6MjE6QkU6MTk6OUI6OUMiLCJsaW5ldXBJZCI6IjI4NiIsImhlYWRlbmQiOiIxNjVfMjM3XzIxOF8yMjJfMjg2IiwiaXNEdnIiOmZhbHNlLCJkdnIiOmZhbHNlfQ%3D%3D; device_id=abe8-38058139-bc0c6ecc-2dd30c98-00c9; Wayfarer=xV%2FYFSxVUwpKfSwHktQ58zlo0ORQLbHO7VDd32rUnWqP9xbZGHH%2BF3NEUaDrOI3sEjTBEcbK0QjwTsp5OkSZM4SJ5ARIaI6nO7BU30KaH3sMVRry69aIBt%2B30%2Bt9TlDoHDjMXunzjXXd7ZjvA0miUn1slMfQN6kSPiz8lgCLr%2BfaeGUjhLq4TneuNVkckO6Jjs2z7G1aRRRjKTUd%2F%2BRgT5eChs3F4tOUdi3VaL2TV%2BmPrpY4tgMUFQ%3D%3D; customer_info=eyJhY2NvdW50SG9sZGVyIjoiQnJpYW4gU2NvdHQifQ%3D%3D; vs_guid=C2GSYvgjlkFtTUGBT8pVjnN3ViafxjoaKTo3dE%2F%2FwEmOegi3RaAqcg%3D%3D; division=eyJkaXZpc2lvbiI6IkNUWC44MjYwIiwiemlwQ29kZSI6Ijc4NjM0LTUzNjEiLCJzdGF0ZSI6IlRYIiwic3Vic2NyaWJlZFR2UGFja2FnZVN0YW5kYXJkVHlwZXMiOlsiREdCU1QiLCJFUUNWSCIsIkJBU0lDIiwiQ1BTVFIiLCJTRFZSUyIsIlBTU1RaIl0sInJvYWRSdW5uZXJTdGFuZGFyZFR5cGUiOiJSUlNURCJ9; ovpsec=eyJjdXN0X2lkIjoiMjlmMDJmNmEyYzMwYzg2NDdhYjZlNDc5OGE5OWFmNTkyODQ0MmQzMSIsImR2ciI6MSwiZHZyX21zZyI6IiIsImxpdmV0diI6dHJ1ZSwib25kZW1hbmQiOnRydWUsImxpdmV0dl9tc2ciOiIiLCJwZXJtaXNzaW9ucyI6WyJyZHZyIiwidHZldmVyeXdoZXJlIiwidHdjYWJsZXR2Il19; wayfarer_ns=xV%2FYFSxVUwpKfSwHktQ58zlo0ORQLbHO7VDd32rUnWqP9xbZGHH%2BF3NEUaDrOI3sEjTBEcbK0QjwTsp5OkSZM4SJ5ARIaI6nO7BU30KaH3sMVRry69aIBt%2B30%2Bt9TlDoHDjMXunzjXXd7ZjvA0miUn1slMfQN6kSPiz8lgCLr%2BfaeGUjhLq4TneuNVkckO6Jjs2z7G1aRRRjKTUd%2F%2BRgT5eChs3F4tOUdi3VaL2TV%2BmPrpY4tgMUFQ%3D%3D; vs_guid_ns=C2GSYvgjlkFtTUGBT8pVjnN3ViafxjoaKTo3dE%2F%2FwEmOegi3RaAqcg%3D%3D; loc=eyJiZWhpbmRPd25Nb2RlbSI6dHJ1ZSwiaW5VUyI6dHJ1ZSwiaW5Vc09yVGVycml0b3J5Ijp0cnVlfQ%3D%3D; vs_client_platform_version=twc-tv-ovp_1.0; parentalControls=true; header=%7B%22appStartup%22%3A%222013-08-11T18%3A03%3A57.383Z%22%2C%22sessionID%22%3A%22dddc1ea9-f29e-42bc-a360-9e95d9040883%22%2C%22previousSessionID%22%3A%22%22%2C%22UTCOffset%22%3A300%2C%22targetDataConsumers%22%3A%22PRODUCTION%22%2C%22logLevel%22%3A0%7D; clientDetails=%7B%22applicationName%22%3A%22OVP%22%2C%22applicationVersion%22%3A%222.3.0.1%22%2C%22apiVersion%22%3A%222.3.0.1%22%2C%22formFactor%22%3A%22PC%22%2C%22triggeredBy%22%3A%7B%22initiator%22%3A%22user%22%2C%22link%22%3A%22undefined%22%7D%2C%22deviceModel%22%3A%22Win32%22%2C%22deviceOS%22%3A%22Mozilla/5.0%20%28compatible%3B%20MSIE%209.0%3B%20Windows%20NT%206.1%3B%20WOW64%3B%20Trident/5.0%3B%20SLCC2%3B%20.NET%20CLR%202.0.50727%3B%20Media%20Center%20PC%206.0%3B%20MS-RTC%20LM%208%3B%20.NET%20CLR%203.5.30729%3B%20.NET%20CLR%203.0.30729%3B%20.NET4.0E%3B%20.NET%20CLR%201.1.4322%3B%20InfoPath.3%3B%20.NET4.0C%3B%20Zune%204.7%29%22%2C%22deviceID%22%3A%22abe8-38058139-bc0c6ecc-2dd30c98-00c9%22%7D");


                string deviceIdHeaderString = HttpUtility.UrlEncode(myModel.DeviceId);
                string headerString = HttpUtility.UrlEncode(myModel.HeaderOptionsJson);
                string clientDetailsString = HttpUtility.UrlEncode(myModel.ClientDetailsJson);
                string wayFarer = HttpUtility.UrlEncode(myModel.Wayfarer);
                string vs_guid = HttpUtility.UrlEncode(myModel.Vs_Guid);

                request.Headers["Cookie"] = "device_id=" + deviceIdHeaderString + "; header=" + headerString + "; clientDetails=" + clientDetailsString + "; Wayfarer=" + wayFarer + "; vs_guid=" + vs_guid + ";";

                parameters["request"] = request;

                //response = (HttpWebResponse)request.GetResponse();
                request.BeginGetResponse(new AsyncCallback(GetShowImageResponseCallback), parameters);
            }
            catch (WebException e)
            {

            }

        }

        private void GetShowImageResponseCallback(IAsyncResult asynchronousResult)
        {
            if (myModel.ShowImages == null)
            {
                myModel.ShowImages = new List<ShowImage>();
            }


            //HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
            HttpWebRequest request = (HttpWebRequest)parameters["request"];
            //request.CookieContainer = cookieContainer;
            // End the operation
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);

            System.IO.Stream streamResponse = response.GetResponseStream();
            //StreamReader streamRead = new StreamReader(streamResponse);
            //BinaryReader myReader = new BinaryReader(streamResponse);

            
             byte[] buffer = new byte[(int)streamResponse.Length];


            streamResponse.Read(buffer, 0, (int)streamResponse.Length);

            //UnZipper myUnzipper = new UnZipper(streamResponse);

            //myUnzipper.GetFileNamesInZip();

            //string readString = streamRead.ReadToEnd();

           
            ShowImage myImage = new ShowImage();
            myImage.EpisodeId = parameters["episodeId"].ToString();
            myImage.EpisodeImageBytes = buffer;
            myImage.ImageHeight = Convert.ToInt32(parameters["width"].ToString());
            myImage.ImageWidth = Convert.ToInt32(parameters["height"].ToString());


            if (myModel.ShowImages.Where(x => x.EpisodeId == myImage.EpisodeId).Count() == 0)
            {
                myModel.ShowImages.Add(myImage);
            }



                // Close the stream object
                streamResponse.Close();

                //myReader.Close();

                // Release the HttpWebResponse
                response.Close();




                OnGetShowImageFinished(myImage.EpisodeId);

            


            //string lsResponse = streamRead.ReadToEnd();

            //byte[] buffer = System.Text.UTF8Encoding.UTF8.GetBytes(lsResponse);
                        
            


            //BitmapImage myBitmapp = new BitmapImage();
            //myBitmap.SetSource(streamResponse);

           





            //RecordingObject returnRecordings = JsonConvert.DeserializeObject<RecordingObject>(recordingsReturn);

            //if (returnRecordings != null)
            //{
            //    myModel.Recordings = returnRecordings.recordings;
            //}



        }
    }
}
