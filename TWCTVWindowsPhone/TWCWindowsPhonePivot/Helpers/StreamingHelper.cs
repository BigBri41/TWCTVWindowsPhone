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
using TWCWindowsPhonePivot.Models;
using TWCWindowsPhonePivot.Classes;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Cryptography;
using System.Globalization;

namespace TWCWindowsPhonePivot.Helpers
{
   
    public class StreamingHelper
    {
        AesManaged cryptoManager = new AesManaged();
        ICryptoTransform decriptor;

        private byte[] HexStringToBytes(string hexString)
        {
            if (hexString == null)
            {
                throw new ArgumentNullException("hexString");
            }
            if ((hexString.Length & 1) != 0)
            {
                throw new ArgumentOutOfRangeException(hexString, "hexString must contain an even number of characters.");
            }
            if (hexString.Length > 0x20)
            {
                hexString = hexString.Substring(2, 0x20);
            }
            byte[] buffer = new byte[hexString.Length / 2];
            for (int i = 0; i < hexString.Length; i += 2)
            {
                buffer[i / 2] = byte.Parse(hexString.Substring(i, 2), NumberStyles.HexNumber);
            }
            return buffer;
        }


        static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public Dictionary<string, object> parameters { get; set; }
        private TWCModel workingModel = new TWCModel();
        public HttpWebResponse StreamResponse;

        public event EventHandler GetStreamFinished;
        public event EventHandler GetStreamingChannelsFinished;

        public void SetParameters(Dictionary<string,object> setParams)
        {
            parameters = setParams;
        }

        public void OnGetStreamingChannelsFinished(List<Channel> returnChannels)
        {
            EventHandler handler = GetStreamingChannelsFinished;
            if (null != handler) handler(returnChannels, EventArgs.Empty);
        }

        public void OnGetStreamFinished()
        {
            EventHandler handler = GetStreamFinished;
            if (null != handler) handler(true, EventArgs.Empty);
        }

        public StreamingHelper(Dictionary<string, object> setParameters)
        {
            parameters = setParameters;
        }
        public StreamingHelper()
        {
            parameters = new Dictionary<string,object>();
        }


        public void GetStreamingChannels(TWCModel myModel)
        {
            GetStreamingChannelsRequest(myModel);

        }

        private void GetStreamingChannelsRequest(TWCModel myModel)
        {

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://services.timewarnercable.com/api/smarttv/live/v1/channels");
                //request.CookieContainer = cookieContainer;
                //request.Headers["X-Requested-With"] = @"XMLHttpRequest";
                request.Accept = "*/*";
                request.Headers["Referer"] = "http://video2.timewarnercable.com/bin/TWC.OVP.Player.xap?commit=0a628ce9facd988e23fbbbe89aee4fd79152bfd4";
                request.Headers["Accept-Language"] = "en-us";
                //request.Headers["Accept-Encoding"] = "gzip, deflate";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
                //request.Headers["Connection"] = "Keep-Alive";
                
                //request.Headers.Set(HttpRequestHeader.Cookie, @"rememberme=eyJ1c2VybmFtZSI6IkJpZ0JyaTQxIiwiY2MiOnRydWUsImxhc3RfYWN0aXZpdHkiOiIiLCJsYXN0X2FjdGlvbiI6IiIsImxhc3RfY2hhbm5lbCI6IiJ9; eventgateway_stb_list=W3siY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibmFtZSI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjp0cnVlLCJkdnIiOnRydWV9LHsiY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibmFtZSI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjpmYWxzZSwiZHZyIjpmYWxzZX1d; current_stb=eyJjbGllbnRUeXBlIjoiT0ROIiwiY2xpZW50VmVyc2lvbiI6IjYuMS4wLjUiLCJtYWNBZGRyZXNzIjoiMDA6MjE6QkU6MTk6OUI6OUMiLCJuYW1lIjoiMDA6MjE6QkU6MTk6OUI6OUMiLCJsaW5ldXBJZCI6IjI4NiIsImhlYWRlbmQiOiIxNjVfMjM3XzIxOF8yMjJfMjg2IiwiaXNEdnIiOmZhbHNlLCJkdnIiOmZhbHNlfQ%3D%3D; device_id=abe8-38058139-bc0c6ecc-2dd30c98-00c9; Wayfarer=xV%2FYFSxVUwpKfSwHktQ58zlo0ORQLbHO7VDd32rUnWqP9xbZGHH%2BF3NEUaDrOI3sEjTBEcbK0QjwTsp5OkSZM4SJ5ARIaI6nO7BU30KaH3sMVRry69aIBt%2B30%2Bt9TlDoHDjMXunzjXXd7ZjvA0miUn1slMfQN6kSPiz8lgCLr%2BfaeGUjhLq4TneuNVkckO6Jjs2z7G1aRRRjKTUd%2F%2BRgT5eChs3F4tOUdi3VaL2TV%2BmPrpY4tgMUFQ%3D%3D; customer_info=eyJhY2NvdW50SG9sZGVyIjoiQnJpYW4gU2NvdHQifQ%3D%3D; vs_guid=C2GSYvgjlkFtTUGBT8pVjnN3ViafxjoaKTo3dE%2F%2FwEmOegi3RaAqcg%3D%3D; division=eyJkaXZpc2lvbiI6IkNUWC44MjYwIiwiemlwQ29kZSI6Ijc4NjM0LTUzNjEiLCJzdGF0ZSI6IlRYIiwic3Vic2NyaWJlZFR2UGFja2FnZVN0YW5kYXJkVHlwZXMiOlsiREdCU1QiLCJFUUNWSCIsIkJBU0lDIiwiQ1BTVFIiLCJTRFZSUyIsIlBTU1RaIl0sInJvYWRSdW5uZXJTdGFuZGFyZFR5cGUiOiJSUlNURCJ9; ovpsec=eyJjdXN0X2lkIjoiMjlmMDJmNmEyYzMwYzg2NDdhYjZlNDc5OGE5OWFmNTkyODQ0MmQzMSIsImR2ciI6MSwiZHZyX21zZyI6IiIsImxpdmV0diI6dHJ1ZSwib25kZW1hbmQiOnRydWUsImxpdmV0dl9tc2ciOiIiLCJwZXJtaXNzaW9ucyI6WyJyZHZyIiwidHZldmVyeXdoZXJlIiwidHdjYWJsZXR2Il19; wayfarer_ns=xV%2FYFSxVUwpKfSwHktQ58zlo0ORQLbHO7VDd32rUnWqP9xbZGHH%2BF3NEUaDrOI3sEjTBEcbK0QjwTsp5OkSZM4SJ5ARIaI6nO7BU30KaH3sMVRry69aIBt%2B30%2Bt9TlDoHDjMXunzjXXd7ZjvA0miUn1slMfQN6kSPiz8lgCLr%2BfaeGUjhLq4TneuNVkckO6Jjs2z7G1aRRRjKTUd%2F%2BRgT5eChs3F4tOUdi3VaL2TV%2BmPrpY4tgMUFQ%3D%3D; vs_guid_ns=C2GSYvgjlkFtTUGBT8pVjnN3ViafxjoaKTo3dE%2F%2FwEmOegi3RaAqcg%3D%3D; loc=eyJiZWhpbmRPd25Nb2RlbSI6dHJ1ZSwiaW5VUyI6dHJ1ZSwiaW5Vc09yVGVycml0b3J5Ijp0cnVlfQ%3D%3D; vs_client_platform_version=twc-tv-ovp_1.0; parentalControls=true; header=%7B%22appStartup%22%3A%222013-08-11T18%3A03%3A57.383Z%22%2C%22sessionID%22%3A%22dddc1ea9-f29e-42bc-a360-9e95d9040883%22%2C%22previousSessionID%22%3A%22%22%2C%22UTCOffset%22%3A300%2C%22targetDataConsumers%22%3A%22PRODUCTION%22%2C%22logLevel%22%3A0%7D; clientDetails=%7B%22applicationName%22%3A%22OVP%22%2C%22applicationVersion%22%3A%222.3.0.1%22%2C%22apiVersion%22%3A%222.3.0.1%22%2C%22formFactor%22%3A%22PC%22%2C%22triggeredBy%22%3A%7B%22initiator%22%3A%22user%22%2C%22link%22%3A%22undefined%22%7D%2C%22deviceModel%22%3A%22Win32%22%2C%22deviceOS%22%3A%22Mozilla/5.0%20%28compatible%3B%20MSIE%209.0%3B%20Windows%20NT%206.1%3B%20WOW64%3B%20Trident/5.0%3B%20SLCC2%3B%20.NET%20CLR%202.0.50727%3B%20Media%20Center%20PC%206.0%3B%20MS-RTC%20LM%208%3B%20.NET%20CLR%203.5.30729%3B%20.NET%20CLR%203.0.30729%3B%20.NET4.0E%3B%20.NET%20CLR%201.1.4322%3B%20InfoPath.3%3B%20.NET4.0C%3B%20Zune%204.7%29%22%2C%22deviceID%22%3A%22abe8-38058139-bc0c6ecc-2dd30c98-00c9%22%7D");
               

                string deviceIdHeaderString = HttpUtility.UrlEncode(myModel.DeviceId);
                string headerString = HttpUtility.UrlEncode(myModel.HeaderOptionsJson);
                string clientDetailsString = HttpUtility.UrlEncode(myModel.ClientDetailsJson);
                string wayFarer = HttpUtility.UrlEncode(myModel.Wayfarer);
                string vs_guid = HttpUtility.UrlEncode(myModel.Vs_Guid);
                /*
                string cookieText = "";
                cookieText = cookieText + "eventgateway_stb_list=W3siY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibmFtZSI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjp0cnVlLCJkdnIiOnRydWV9LHsiY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibmFtZSI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjpmYWxzZSwiZHZyIjpmYWxzZX1d;"; 

cookieText = cookieText + "division=eyJkaXZpc2lvbiI6IkNUWC44MjYwIiwiemlwQ29kZSI6Ijc4NjM0LTUzNjEiLCJzdGF0ZSI6IlRYIiwic3Vic2NyaWJlZFR2UGFja2FnZVN0YW5kYXJkVHlwZXMiOlsiREdCU1QiLCJFUUNWSCIsIkJBU0lDIiwiQ1BTVFIiLCJTRFZSUyIsIlBTU1RaIl0sInJvYWRSdW5uZXJTdGFuZGFyZFR5cGUiOiJSUlNURCJ9;"; 
cookieText = cookieText + "ovpsec=eyJjdXN0X2lkIjoiMjlmMDJmNmEyYzMwYzg2NDdhYjZlNDc5OGE5OWFmNTkyODQ0MmQzMSIsImR2ciI6MSwiZHZyX21zZyI6IiIsImxpdmV0diI6dHJ1ZSwib25kZW1hbmQiOnRydWUsImxpdmV0dl9tc2ciOiIiLCJwZXJtaXNzaW9ucyI6WyJyZHZyIiwidHdjYWJsZXR2IiwidHZldmVyeXdoZXJlIl19;";

cookieText = cookieText + "loc=eyJiZWhpbmRPd25Nb2RlbSI6dHJ1ZSwiaW5VUyI6dHJ1ZSwiaW5Vc09yVGVycml0b3J5Ijp0cnVlfQ%3D%3D;";
                */
                request.Headers["Cookie"] = "device_id=" + deviceIdHeaderString + "; header=" + headerString + "; clientDetails=" + clientDetailsString + "; Wayfarer=" + wayFarer + "; vs_guid=" + vs_guid + "; wayfarer_ns=" + wayFarer + "; vs_guid_ns=" + vs_guid + ";";

                parameters["request"] = request;

                //response = (HttpWebResponse)request.GetResponse();
                request.BeginGetResponse(new AsyncCallback(GetStreamingChannelsResponseCallback), parameters);
            }
            catch (WebException e)
            {

            }

        }

        private void GetStreamingChannelsResponseCallback(IAsyncResult asynchronousResult)
        {

            //HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
            HttpWebRequest request = (HttpWebRequest)parameters["request"];

            // End the operation
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            System.IO.Stream streamResponse = response.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse);


            string devicesReturn = streamRead.ReadToEnd();


            // Close the stream object
            streamResponse.Close();
            streamRead.Close();

            // Release the HttpWebResponse
            response.Close();


            StreamChannels returnChannels = JsonConvert.DeserializeObject<StreamChannels>(devicesReturn);


            OnGetStreamingChannelsFinished(returnChannels.channels);
        }




        public void GetStream(string streamLink, TWCModel myModel)
        {           
            parameters["streamLink"] = streamLink;
            //GetStreamRequest(myModel);
            GetCurrentStreamInfo(myModel);
        }

        private void GetCurrentStreamInfo(TWCModel myModel)
        {
            try
            {
                workingModel = myModel;
                string streamLink = parameters["streamLink"].ToString();

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://services.timewarnercable.com"+streamLink);
                //request.CookieContainer = cookieContainer;
                //request.Headers["X-Requested-With"] = @"XMLHttpRequest";
                request.Accept = "*/*";
                request.Headers["Referer"] = "http://video2.timewarnercable.com/bin/TWC.OVP.Player.xap?commit=0a628ce9facd988e23fbbbe89aee4fd79152bfd4";
                request.Headers["Accept-Language"] = "en-us";
                //request.Headers["Accept-Encoding"] = "gzip, deflate";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
                request.Headers["Connection"] = "Keep-Alive";
                //request.Headers.Set(HttpRequestHeader.Cookie, @"rememberme=eyJ1c2VybmFtZSI6IkJpZ0JyaTQxIiwiY2MiOnRydWUsImxhc3RfYWN0aXZpdHkiOiIiLCJsYXN0X2FjdGlvbiI6IiIsImxhc3RfY2hhbm5lbCI6IiJ9; eventgateway_stb_list=W3siY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibmFtZSI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjp0cnVlLCJkdnIiOnRydWV9LHsiY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibmFtZSI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjpmYWxzZSwiZHZyIjpmYWxzZX1d; current_stb=eyJjbGllbnRUeXBlIjoiT0ROIiwiY2xpZW50VmVyc2lvbiI6IjYuMS4wLjUiLCJtYWNBZGRyZXNzIjoiMDA6MjE6QkU6MTk6OUI6OUMiLCJuYW1lIjoiMDA6MjE6QkU6MTk6OUI6OUMiLCJsaW5ldXBJZCI6IjI4NiIsImhlYWRlbmQiOiIxNjVfMjM3XzIxOF8yMjJfMjg2IiwiaXNEdnIiOmZhbHNlLCJkdnIiOmZhbHNlfQ%3D%3D; device_id=abe8-38058139-bc0c6ecc-2dd30c98-00c9; Wayfarer=xV%2FYFSxVUwpKfSwHktQ58zlo0ORQLbHO7VDd32rUnWqP9xbZGHH%2BF3NEUaDrOI3sEjTBEcbK0QjwTsp5OkSZM4SJ5ARIaI6nO7BU30KaH3sMVRry69aIBt%2B30%2Bt9TlDoHDjMXunzjXXd7ZjvA0miUn1slMfQN6kSPiz8lgCLr%2BfaeGUjhLq4TneuNVkckO6Jjs2z7G1aRRRjKTUd%2F%2BRgT5eChs3F4tOUdi3VaL2TV%2BmPrpY4tgMUFQ%3D%3D; customer_info=eyJhY2NvdW50SG9sZGVyIjoiQnJpYW4gU2NvdHQifQ%3D%3D; vs_guid=C2GSYvgjlkFtTUGBT8pVjnN3ViafxjoaKTo3dE%2F%2FwEmOegi3RaAqcg%3D%3D; division=eyJkaXZpc2lvbiI6IkNUWC44MjYwIiwiemlwQ29kZSI6Ijc4NjM0LTUzNjEiLCJzdGF0ZSI6IlRYIiwic3Vic2NyaWJlZFR2UGFja2FnZVN0YW5kYXJkVHlwZXMiOlsiREdCU1QiLCJFUUNWSCIsIkJBU0lDIiwiQ1BTVFIiLCJTRFZSUyIsIlBTU1RaIl0sInJvYWRSdW5uZXJTdGFuZGFyZFR5cGUiOiJSUlNURCJ9; ovpsec=eyJjdXN0X2lkIjoiMjlmMDJmNmEyYzMwYzg2NDdhYjZlNDc5OGE5OWFmNTkyODQ0MmQzMSIsImR2ciI6MSwiZHZyX21zZyI6IiIsImxpdmV0diI6dHJ1ZSwib25kZW1hbmQiOnRydWUsImxpdmV0dl9tc2ciOiIiLCJwZXJtaXNzaW9ucyI6WyJyZHZyIiwidHZldmVyeXdoZXJlIiwidHdjYWJsZXR2Il19; wayfarer_ns=xV%2FYFSxVUwpKfSwHktQ58zlo0ORQLbHO7VDd32rUnWqP9xbZGHH%2BF3NEUaDrOI3sEjTBEcbK0QjwTsp5OkSZM4SJ5ARIaI6nO7BU30KaH3sMVRry69aIBt%2B30%2Bt9TlDoHDjMXunzjXXd7ZjvA0miUn1slMfQN6kSPiz8lgCLr%2BfaeGUjhLq4TneuNVkckO6Jjs2z7G1aRRRjKTUd%2F%2BRgT5eChs3F4tOUdi3VaL2TV%2BmPrpY4tgMUFQ%3D%3D; vs_guid_ns=C2GSYvgjlkFtTUGBT8pVjnN3ViafxjoaKTo3dE%2F%2FwEmOegi3RaAqcg%3D%3D; loc=eyJiZWhpbmRPd25Nb2RlbSI6dHJ1ZSwiaW5VUyI6dHJ1ZSwiaW5Vc09yVGVycml0b3J5Ijp0cnVlfQ%3D%3D; vs_client_platform_version=twc-tv-ovp_1.0; parentalControls=true; header=%7B%22appStartup%22%3A%222013-08-11T18%3A03%3A57.383Z%22%2C%22sessionID%22%3A%22dddc1ea9-f29e-42bc-a360-9e95d9040883%22%2C%22previousSessionID%22%3A%22%22%2C%22UTCOffset%22%3A300%2C%22targetDataConsumers%22%3A%22PRODUCTION%22%2C%22logLevel%22%3A0%7D; clientDetails=%7B%22applicationName%22%3A%22OVP%22%2C%22applicationVersion%22%3A%222.3.0.1%22%2C%22apiVersion%22%3A%222.3.0.1%22%2C%22formFactor%22%3A%22PC%22%2C%22triggeredBy%22%3A%7B%22initiator%22%3A%22user%22%2C%22link%22%3A%22undefined%22%7D%2C%22deviceModel%22%3A%22Win32%22%2C%22deviceOS%22%3A%22Mozilla/5.0%20%28compatible%3B%20MSIE%209.0%3B%20Windows%20NT%206.1%3B%20WOW64%3B%20Trident/5.0%3B%20SLCC2%3B%20.NET%20CLR%202.0.50727%3B%20Media%20Center%20PC%206.0%3B%20MS-RTC%20LM%208%3B%20.NET%20CLR%203.5.30729%3B%20.NET%20CLR%203.0.30729%3B%20.NET4.0E%3B%20.NET%20CLR%201.1.4322%3B%20InfoPath.3%3B%20.NET4.0C%3B%20Zune%204.7%29%22%2C%22deviceID%22%3A%22abe8-38058139-bc0c6ecc-2dd30c98-00c9%22%7D");


                string deviceIdHeaderString = HttpUtility.UrlEncode(myModel.DeviceId);
                string headerString = HttpUtility.UrlEncode(myModel.HeaderOptionsJson);
                string clientDetailsString = HttpUtility.UrlEncode(myModel.ClientDetailsJson);
                string wayFarer = HttpUtility.UrlEncode(myModel.Wayfarer);
                string vs_guid = HttpUtility.UrlEncode(myModel.Vs_Guid);
                string vs_guid_ns = HttpUtility.UrlEncode(myModel.Vs_Guid);
                request.Headers["Cookie"] = "device_id=" + deviceIdHeaderString + "; header=" + headerString + "; clientDetails=" + clientDetailsString + "; Wayfarer=" + wayFarer + "; vs_guid=" + vs_guid + "; vs_guid_ns=" + vs_guid_ns + ";";

                parameters["request"] = request;

                //response = (HttpWebResponse)request.GetResponse();
                request.BeginGetResponse(new AsyncCallback(GetCurrentStreamInfoResponseCallback), parameters);
            }
            catch (WebException e)
            {

            }            
        }

        private void GetCurrentStreamInfoResponseCallback(IAsyncResult asynchronousResult)
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


            ChannelStreamInfo returnRecordings = JsonConvert.DeserializeObject<ChannelStreamInfo>(recordingsReturn);

            parameters.Add("streamChannelInfo", returnRecordings);

            GetToken(workingModel);
            
        }


        private void GetStreamRequest(TWCModel myModel)
        {

            try
            {
                ChannelStreamInfo currentChannelStreamInfo = (ChannelStreamInfo)parameters["streamChannelInfo"];

                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://video2.timewarnercable.com/call?url=https://video-services.timewarnercable.com/api/v3/secured/dvr/recording/list?urlEncodedMacAddress=" + macAddress);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(currentChannelStreamInfo.stream_url);
                //request.CookieContainer = cookieContainer;
                //request.Headers["X-Requested-With"] = @"XMLHttpRequest";
                request.Accept = "*/*";
                request.Headers["Referer"] = "http://video2.timewarnercable.com/bin/TWC.OVP.Player.xap?commit=0a628ce9facd988e23fbbbe89aee4fd79152bfd4";
                request.Headers["Accept-Language"] = "en-us";
                request.Headers["Accept-Encoding"] = "gzip, deflate";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
                request.Headers["Connection"] = "Keep-Alive";
                //request.Headers.Set(HttpRequestHeader.Cookie, @"rememberme=eyJ1c2VybmFtZSI6IkJpZ0JyaTQxIiwiY2MiOnRydWUsImxhc3RfYWN0aXZpdHkiOiIiLCJsYXN0X2FjdGlvbiI6IiIsImxhc3RfY2hhbm5lbCI6IiJ9; eventgateway_stb_list=W3siY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibmFtZSI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjp0cnVlLCJkdnIiOnRydWV9LHsiY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibmFtZSI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjpmYWxzZSwiZHZyIjpmYWxzZX1d; current_stb=eyJjbGllbnRUeXBlIjoiT0ROIiwiY2xpZW50VmVyc2lvbiI6IjYuMS4wLjUiLCJtYWNBZGRyZXNzIjoiMDA6MjE6QkU6MTk6OUI6OUMiLCJuYW1lIjoiMDA6MjE6QkU6MTk6OUI6OUMiLCJsaW5ldXBJZCI6IjI4NiIsImhlYWRlbmQiOiIxNjVfMjM3XzIxOF8yMjJfMjg2IiwiaXNEdnIiOmZhbHNlLCJkdnIiOmZhbHNlfQ%3D%3D; device_id=abe8-38058139-bc0c6ecc-2dd30c98-00c9; Wayfarer=xV%2FYFSxVUwpKfSwHktQ58zlo0ORQLbHO7VDd32rUnWqP9xbZGHH%2BF3NEUaDrOI3sEjTBEcbK0QjwTsp5OkSZM4SJ5ARIaI6nO7BU30KaH3sMVRry69aIBt%2B30%2Bt9TlDoHDjMXunzjXXd7ZjvA0miUn1slMfQN6kSPiz8lgCLr%2BfaeGUjhLq4TneuNVkckO6Jjs2z7G1aRRRjKTUd%2F%2BRgT5eChs3F4tOUdi3VaL2TV%2BmPrpY4tgMUFQ%3D%3D; customer_info=eyJhY2NvdW50SG9sZGVyIjoiQnJpYW4gU2NvdHQifQ%3D%3D; vs_guid=C2GSYvgjlkFtTUGBT8pVjnN3ViafxjoaKTo3dE%2F%2FwEmOegi3RaAqcg%3D%3D; division=eyJkaXZpc2lvbiI6IkNUWC44MjYwIiwiemlwQ29kZSI6Ijc4NjM0LTUzNjEiLCJzdGF0ZSI6IlRYIiwic3Vic2NyaWJlZFR2UGFja2FnZVN0YW5kYXJkVHlwZXMiOlsiREdCU1QiLCJFUUNWSCIsIkJBU0lDIiwiQ1BTVFIiLCJTRFZSUyIsIlBTU1RaIl0sInJvYWRSdW5uZXJTdGFuZGFyZFR5cGUiOiJSUlNURCJ9; ovpsec=eyJjdXN0X2lkIjoiMjlmMDJmNmEyYzMwYzg2NDdhYjZlNDc5OGE5OWFmNTkyODQ0MmQzMSIsImR2ciI6MSwiZHZyX21zZyI6IiIsImxpdmV0diI6dHJ1ZSwib25kZW1hbmQiOnRydWUsImxpdmV0dl9tc2ciOiIiLCJwZXJtaXNzaW9ucyI6WyJyZHZyIiwidHZldmVyeXdoZXJlIiwidHdjYWJsZXR2Il19; wayfarer_ns=xV%2FYFSxVUwpKfSwHktQ58zlo0ORQLbHO7VDd32rUnWqP9xbZGHH%2BF3NEUaDrOI3sEjTBEcbK0QjwTsp5OkSZM4SJ5ARIaI6nO7BU30KaH3sMVRry69aIBt%2B30%2Bt9TlDoHDjMXunzjXXd7ZjvA0miUn1slMfQN6kSPiz8lgCLr%2BfaeGUjhLq4TneuNVkckO6Jjs2z7G1aRRRjKTUd%2F%2BRgT5eChs3F4tOUdi3VaL2TV%2BmPrpY4tgMUFQ%3D%3D; vs_guid_ns=C2GSYvgjlkFtTUGBT8pVjnN3ViafxjoaKTo3dE%2F%2FwEmOegi3RaAqcg%3D%3D; loc=eyJiZWhpbmRPd25Nb2RlbSI6dHJ1ZSwiaW5VUyI6dHJ1ZSwiaW5Vc09yVGVycml0b3J5Ijp0cnVlfQ%3D%3D; vs_client_platform_version=twc-tv-ovp_1.0; parentalControls=true; header=%7B%22appStartup%22%3A%222013-08-11T18%3A03%3A57.383Z%22%2C%22sessionID%22%3A%22dddc1ea9-f29e-42bc-a360-9e95d9040883%22%2C%22previousSessionID%22%3A%22%22%2C%22UTCOffset%22%3A300%2C%22targetDataConsumers%22%3A%22PRODUCTION%22%2C%22logLevel%22%3A0%7D; clientDetails=%7B%22applicationName%22%3A%22OVP%22%2C%22applicationVersion%22%3A%222.3.0.1%22%2C%22apiVersion%22%3A%222.3.0.1%22%2C%22formFactor%22%3A%22PC%22%2C%22triggeredBy%22%3A%7B%22initiator%22%3A%22user%22%2C%22link%22%3A%22undefined%22%7D%2C%22deviceModel%22%3A%22Win32%22%2C%22deviceOS%22%3A%22Mozilla/5.0%20%28compatible%3B%20MSIE%209.0%3B%20Windows%20NT%206.1%3B%20WOW64%3B%20Trident/5.0%3B%20SLCC2%3B%20.NET%20CLR%202.0.50727%3B%20Media%20Center%20PC%206.0%3B%20MS-RTC%20LM%208%3B%20.NET%20CLR%203.5.30729%3B%20.NET%20CLR%203.0.30729%3B%20.NET4.0E%3B%20.NET%20CLR%201.1.4322%3B%20InfoPath.3%3B%20.NET4.0C%3B%20Zune%204.7%29%22%2C%22deviceID%22%3A%22abe8-38058139-bc0c6ecc-2dd30c98-00c9%22%7D");


                string deviceIdHeaderString = HttpUtility.UrlEncode(myModel.DeviceId);
                string headerString = HttpUtility.UrlEncode(myModel.HeaderOptionsJson);
                string clientDetailsString = HttpUtility.UrlEncode(myModel.ClientDetailsJson);
                string wayFarer = HttpUtility.UrlEncode(myModel.Wayfarer);
                string vs_guid = HttpUtility.UrlEncode(myModel.Vs_Guid);
                string vs_guid_ns = HttpUtility.UrlEncode(myModel.Vs_Guid);
                string vxtoken = parameters["vxtoken"].ToString();
                request.Headers["Cookie"] = "device_id=" + deviceIdHeaderString + "; header=" + headerString + "; clientDetails=" + clientDetailsString + "; Wayfarer=" + wayFarer + "; vs_guid=" + vs_guid + "; vs_guid_ns="+vs_guid_ns+"; vxtoken="+vxtoken+";";

                parameters["request"] = request;

                //response = (HttpWebResponse)request.GetResponse();
                request.BeginGetResponse(new AsyncCallback(GetStreamResponseCallback), parameters);
            }
            catch (WebException e)
            {

            }

        }

        private void GetStreamResponseCallback(IAsyncResult asynchronousResult)
        {

            //HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
            HttpWebRequest request = (HttpWebRequest)parameters["request"];
            //request.CookieContainer = cookieContainer;
            // End the operation
            //HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);

            StreamResponse = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            //StreamResponse.Close();
            //System.IO.Stream streamResponse = response.GetResponseStream();
            BinaryReader streamRead = new BinaryReader(StreamResponse.GetResponseStream());

            byte[] stream = streamRead.ReadBytes((int)StreamResponse.ContentLength);
            //string recordingsReturn = streamRead.ReadToEnd();

            byte[] outputBuffer = new byte[StreamResponse.ContentLength];
           
            decriptor.TransformBlock(stream, 0, stream.Length, outputBuffer, 0);
            string hold = "";
            /*
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
            */

            OnGetStreamFinished();
        }



        public void GetToken(TWCModel myModel)
        {
            GetTokenRequest(myModel);

        }

        private void GetTokenRequest(TWCModel myModel)
        {

            try
            {
                //string streamLink = parameters["streamLink"].ToString();
                ChannelStreamInfo currentStreamInfo = (ChannelStreamInfo)parameters["streamChannelInfo"];
                
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://video2.timewarnercable.com/call?url=https://video-services.timewarnercable.com/api/v3/secured/dvr/recording/list?urlEncodedMacAddress=" + macAddress);
                
                
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(currentStreamInfo.key_url);
                //request.CookieContainer = cookieContainer;
                //request.Headers["X-Requested-With"] = @"XMLHttpRequest";
                request.Accept = "*/*";
                request.Headers["Referer"] = "http://video2.timewarnercable.com/bin/TWC.OVP.Player.xap?commit=0a628ce9facd988e23fbbbe89aee4fd79152bfd4";
                request.Headers["Accept-Language"] = "en-us";
                //request.Headers["Accept-Encoding"] = "gzip, deflate";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
                request.Headers["Connection"] = "Keep-Alive";
                //request.Headers.Set(HttpRequestHeader.Cookie, @"rememberme=eyJ1c2VybmFtZSI6IkJpZ0JyaTQxIiwiY2MiOnRydWUsImxhc3RfYWN0aXZpdHkiOiIiLCJsYXN0X2FjdGlvbiI6IiIsImxhc3RfY2hhbm5lbCI6IiJ9; eventgateway_stb_list=W3siY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibmFtZSI6IjAwOjIxOkJFOkRGOkE4OjZDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjp0cnVlLCJkdnIiOnRydWV9LHsiY2xpZW50VHlwZSI6Ik9ETiIsImNsaWVudFZlcnNpb24iOiI2LjEuMC41IiwibWFjQWRkcmVzcyI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibmFtZSI6IjAwOjIxOkJFOjE5OjlCOjlDIiwibGluZXVwSWQiOiIyODYiLCJoZWFkZW5kIjoiMTY1XzIzN18yMThfMjIyXzI4NiIsImlzRHZyIjpmYWxzZSwiZHZyIjpmYWxzZX1d; current_stb=eyJjbGllbnRUeXBlIjoiT0ROIiwiY2xpZW50VmVyc2lvbiI6IjYuMS4wLjUiLCJtYWNBZGRyZXNzIjoiMDA6MjE6QkU6MTk6OUI6OUMiLCJuYW1lIjoiMDA6MjE6QkU6MTk6OUI6OUMiLCJsaW5ldXBJZCI6IjI4NiIsImhlYWRlbmQiOiIxNjVfMjM3XzIxOF8yMjJfMjg2IiwiaXNEdnIiOmZhbHNlLCJkdnIiOmZhbHNlfQ%3D%3D; device_id=abe8-38058139-bc0c6ecc-2dd30c98-00c9; Wayfarer=xV%2FYFSxVUwpKfSwHktQ58zlo0ORQLbHO7VDd32rUnWqP9xbZGHH%2BF3NEUaDrOI3sEjTBEcbK0QjwTsp5OkSZM4SJ5ARIaI6nO7BU30KaH3sMVRry69aIBt%2B30%2Bt9TlDoHDjMXunzjXXd7ZjvA0miUn1slMfQN6kSPiz8lgCLr%2BfaeGUjhLq4TneuNVkckO6Jjs2z7G1aRRRjKTUd%2F%2BRgT5eChs3F4tOUdi3VaL2TV%2BmPrpY4tgMUFQ%3D%3D; customer_info=eyJhY2NvdW50SG9sZGVyIjoiQnJpYW4gU2NvdHQifQ%3D%3D; vs_guid=C2GSYvgjlkFtTUGBT8pVjnN3ViafxjoaKTo3dE%2F%2FwEmOegi3RaAqcg%3D%3D; division=eyJkaXZpc2lvbiI6IkNUWC44MjYwIiwiemlwQ29kZSI6Ijc4NjM0LTUzNjEiLCJzdGF0ZSI6IlRYIiwic3Vic2NyaWJlZFR2UGFja2FnZVN0YW5kYXJkVHlwZXMiOlsiREdCU1QiLCJFUUNWSCIsIkJBU0lDIiwiQ1BTVFIiLCJTRFZSUyIsIlBTU1RaIl0sInJvYWRSdW5uZXJTdGFuZGFyZFR5cGUiOiJSUlNURCJ9; ovpsec=eyJjdXN0X2lkIjoiMjlmMDJmNmEyYzMwYzg2NDdhYjZlNDc5OGE5OWFmNTkyODQ0MmQzMSIsImR2ciI6MSwiZHZyX21zZyI6IiIsImxpdmV0diI6dHJ1ZSwib25kZW1hbmQiOnRydWUsImxpdmV0dl9tc2ciOiIiLCJwZXJtaXNzaW9ucyI6WyJyZHZyIiwidHZldmVyeXdoZXJlIiwidHdjYWJsZXR2Il19; wayfarer_ns=xV%2FYFSxVUwpKfSwHktQ58zlo0ORQLbHO7VDd32rUnWqP9xbZGHH%2BF3NEUaDrOI3sEjTBEcbK0QjwTsp5OkSZM4SJ5ARIaI6nO7BU30KaH3sMVRry69aIBt%2B30%2Bt9TlDoHDjMXunzjXXd7ZjvA0miUn1slMfQN6kSPiz8lgCLr%2BfaeGUjhLq4TneuNVkckO6Jjs2z7G1aRRRjKTUd%2F%2BRgT5eChs3F4tOUdi3VaL2TV%2BmPrpY4tgMUFQ%3D%3D; vs_guid_ns=C2GSYvgjlkFtTUGBT8pVjnN3ViafxjoaKTo3dE%2F%2FwEmOegi3RaAqcg%3D%3D; loc=eyJiZWhpbmRPd25Nb2RlbSI6dHJ1ZSwiaW5VUyI6dHJ1ZSwiaW5Vc09yVGVycml0b3J5Ijp0cnVlfQ%3D%3D; vs_client_platform_version=twc-tv-ovp_1.0; parentalControls=true; header=%7B%22appStartup%22%3A%222013-08-11T18%3A03%3A57.383Z%22%2C%22sessionID%22%3A%22dddc1ea9-f29e-42bc-a360-9e95d9040883%22%2C%22previousSessionID%22%3A%22%22%2C%22UTCOffset%22%3A300%2C%22targetDataConsumers%22%3A%22PRODUCTION%22%2C%22logLevel%22%3A0%7D; clientDetails=%7B%22applicationName%22%3A%22OVP%22%2C%22applicationVersion%22%3A%222.3.0.1%22%2C%22apiVersion%22%3A%222.3.0.1%22%2C%22formFactor%22%3A%22PC%22%2C%22triggeredBy%22%3A%7B%22initiator%22%3A%22user%22%2C%22link%22%3A%22undefined%22%7D%2C%22deviceModel%22%3A%22Win32%22%2C%22deviceOS%22%3A%22Mozilla/5.0%20%28compatible%3B%20MSIE%209.0%3B%20Windows%20NT%206.1%3B%20WOW64%3B%20Trident/5.0%3B%20SLCC2%3B%20.NET%20CLR%202.0.50727%3B%20Media%20Center%20PC%206.0%3B%20MS-RTC%20LM%208%3B%20.NET%20CLR%203.5.30729%3B%20.NET%20CLR%203.0.30729%3B%20.NET4.0E%3B%20.NET%20CLR%201.1.4322%3B%20InfoPath.3%3B%20.NET4.0C%3B%20Zune%204.7%29%22%2C%22deviceID%22%3A%22abe8-38058139-bc0c6ecc-2dd30c98-00c9%22%7D");


                string deviceIdHeaderString = HttpUtility.UrlEncode(myModel.DeviceId);
                string headerString = HttpUtility.UrlEncode(myModel.HeaderOptionsJson);
                string clientDetailsString = HttpUtility.UrlEncode(myModel.ClientDetailsJson);
                string wayFarer = HttpUtility.UrlEncode(myModel.Wayfarer);
                string vs_guid = HttpUtility.UrlEncode(myModel.Vs_Guid);
                string vs_guid_ns = HttpUtility.UrlEncode(myModel.Vs_Guid);
                request.Headers["Cookie"] = "device_id=" + deviceIdHeaderString + "; header=" + headerString + "; clientDetails=" + clientDetailsString + "; Wayfarer=" + wayFarer + "; vs_guid=" + vs_guid + "; vs_guid_ns=" + vs_guid_ns + ";";

                parameters["request"] = request;

                //response = (HttpWebResponse)request.GetResponse();
                request.BeginGetResponse(new AsyncCallback(GetTokenResponseCallback), parameters);
            }
            catch (WebException e)
            {
                string hold = "";
            }

        }

        private void GetTokenResponseCallback(IAsyncResult asynchronousResult)
        {

            //HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
            HttpWebRequest request = (HttpWebRequest)parameters["request"];
            //request.CookieContainer = cookieContainer;
            // End the operation
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            System.IO.Stream streamResponse = response.GetResponseStream();
            //StreamReader streamRead = new StreamReader(streamResponse);
            CookieContainer blah = new CookieContainer();
   
            if(!string.IsNullOrEmpty(response.Headers["Set-Cookie"]))
            {
                string vxToken = response.Headers["Set-Cookie"].Substring(0, response.Headers["Set-Cookie"].IndexOf(";"));
                vxToken = vxToken.Replace("vxtoken=", "");
                parameters.Add("vxtoken", vxToken);
            }
           
            BinaryReader streamRead = new BinaryReader(streamResponse);

            //char[] blahblah = new char[streamResponse.Length];

            //streamRead.Read(blahblah, 0, (int)streamResponse.Length);
            //string returnStuff = streamRead.ReadToEnd();


            byte[] encryptedBytes = new byte[streamResponse.Length];

            //streamResponse.Read(blahblah, 0, (int)streamResponse.Length);
            encryptedBytes = streamRead.ReadBytes((int)streamResponse.Length);
            byte[] iv = HexStringToBytes(((ChannelStreamInfo)parameters["streamChannelInfo"]).iv);

            

             cryptoManager = new AesManaged();
             decriptor = cryptoManager.CreateDecryptor(encryptedBytes, iv);

                   
           
            //string recordingsReturn = streamRead.ReadToEnd();
            //byte[] key = new byte[streamResponse.Length];
            //streamResponse.Read(key, 0, (int)streamResponse.Length);

            //byte[] key = Convert.FromBase64String(recordingsReturn);
           

            streamResponse.Close();
            streamRead.Close();

            GetStreamRequest(workingModel);

            /*
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
            */

          }

    }
}
