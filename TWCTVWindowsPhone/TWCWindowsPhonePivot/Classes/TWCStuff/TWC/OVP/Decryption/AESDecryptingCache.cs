namespace TWC.OVP.Decryption
{
    using Microsoft.Web.Media.SmoothStreaming;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Browser;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Text;

    public class AESDecryptingCache : ISmoothStreamingCache
    {
        private static Dictionary<Uri, CacheItem> _cache = new Dictionary<Uri, CacheItem>();
        private AESDecryptionInfo _decryptionInfo;

        public AESDecryptingCache(AESDecryptionInfo decryptionInfo)
        {
            this._decryptionInfo = decryptionInfo;
        }

        public IAsyncResult BeginPersist(CacheRequest request, CacheResponse response, AsyncCallback callback, object state)
        {
            if (!request.CanonicalUri.ToString().Contains("FragmentInfo") && !request.CanonicalUri.ToString().Contains("Manifest"))
            {
                try
                {
                    using (CryptoStream stream = new CryptoStream(response.Response, this._decryptionInfo.Decryptor, CryptoStreamMode.Read))
                    {
                        MemoryStream stream2 = new MemoryStream();
                        int count = 0x1000;
                        byte[] buffer = new byte[count];
                        while (stream.CanRead)
                        {
                            int num = stream.Read(buffer, 0, count);
                            if (num == 0)
                            {
                                break;
                            }
                            stream2.Write(buffer, 0, num);
                        }
                        stream2.Position = 0L;
                        response.Response = stream2;
                    }
                }
                catch
                {
                    response.Response = null;
                }
            }
            return null;
        }

        public IAsyncResult BeginRetrieve(CacheRequest request, AsyncCallback callback, object state)
        {
            if (this.IsStopped)
            {
                CacheAsyncResult result = new CacheAsyncResult();
                result.Complete(new CacheResponse(0L, null, null, null, HttpStatusCode.NotFound, "Not Found", DateTime.Now, false), true);
                return result;
            }
            if (!request.CanonicalUri.ToString().Contains("FragmentInfo") && !request.CanonicalUri.ToString().Contains("Manifest"))
            {
                return null;
            }
            CacheAsyncResult ar = new CacheAsyncResult {
                strUrl = request.CanonicalUri.ToString()
            };
            HttpWebRequest webRequest = WebRequestCreator.BrowserHttp.Create(request.CanonicalUri) as HttpWebRequest;
            webRequest.BeginGetResponse(delegate (IAsyncResult result) {
                try
                {
                    HttpWebResponse response = webRequest.EndGetResponse(result) as HttpWebResponse;
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        ar.Complete(null, true);
                    }
                    else
                    {
                        using (CryptoStream stream = new CryptoStream(response.GetResponseStream(), this._decryptionInfo.Decryptor, CryptoStreamMode.Read))
                        {
                            MemoryStream stream2 = new MemoryStream();
                            long contentLength = 0L;
                            int count = 0x1000;
                            byte[] buffer = new byte[count];
                            while (stream.CanRead)
                            {
                                int num2 = stream.Read(buffer, 0, count);
                                contentLength += num2;
                                if (num2 == 0)
                                {
                                    break;
                                }
                                stream2.Write(buffer, 0, num2);
                            }
                            stream2.Position = 0L;
                            if (request.CanonicalUri.ToString().Contains("Manifest"))
                            {
                                StreamReader reader = new StreamReader(stream2);
                                string s = reader.ReadToEnd().Replace("{start time})", "{start time})?" + this._decryptionInfo.SessionID).Replace("300000000", "550000000");
                                stream2 = new MemoryStream(Encoding.Unicode.GetBytes(s)) {
                                    Position = 0L
                                };
                            }
                            CacheResponse response2 = new CacheResponse(contentLength, response.ContentType, null, stream2, response.StatusCode, response.StatusDescription, DateTime.Now, true);
                            stream2.Position = 0L;
                            ar.Complete(response2, true);
                        }
                    }
                }
                catch (Exception)
                {
                    ar.Complete(null, true);
                }
            }, null);
            return ar;
        }

        public void CloseMedia(Uri manifestUri)
        {
        }

        public bool EndPersist(IAsyncResult ar)
        {
            return true;
        }

        public CacheResponse EndRetrieve(IAsyncResult ar)
        {
            CacheAsyncResult result = ar as CacheAsyncResult;
            if (result == null)
            {
                return null;
            }
            for (int i = 0; i < 0x18; i++)
            {
                if ((this.IsStopping || this.IsStopped) || result.AsyncWaitHandle.WaitOne(250))
                {
                    break;
                }
            }
            return (result.Result as CacheResponse);
        }

        public void OpenMedia(Uri manifestUri)
        {
        }

        public bool IsStopped { get; set; }

        public bool IsStopping { get; set; }
    }
}

