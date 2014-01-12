namespace TWC.OVP.Decryption
{
    using Microsoft.Web.Media.SmoothStreaming;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Text;
    using TWC.OVP.Services;

    public class AESOVPRequestDecryptingCache : ISmoothStreamingCache
    {
        private static Dictionary<Uri, CacheItem> _cache = new Dictionary<Uri, CacheItem>();
        private AESDecryptionInfo _decryptionInfo;

        public AESOVPRequestDecryptingCache(AESDecryptionInfo decryptionInfo)
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
            if (!request.CanonicalUri.AbsoluteUri.Contains("FragmentInfo") && !request.CanonicalUri.AbsoluteUri.Contains("Manifest"))
            {
                return null;
            }
            CacheResponse response = null;
            CacheAsyncResult result2 = new CacheAsyncResult {
                strUrl = request.CanonicalUri.ToString()
            };
            result2.Complete(response, true);
            return result2;
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
            if (this.IsStopping || this.IsStopped)
            {
                return null;
            }
            WebRequestResult result2 = OVPClientHttpRequest.BrowserHttpRequest(result.strUrl, 0x1770);
            if (result2.HttpStatusCode != 200)
            {
                return null;
            }
            using (CryptoStream stream2 = new CryptoStream(result2.Response.GetResponseStream(), this._decryptionInfo.Decryptor, CryptoStreamMode.Read))
            {
                MemoryStream stream = new MemoryStream();
                long contentLength = 0L;
                int count = 0x1000;
                byte[] buffer = new byte[count];
                while (stream2.CanRead)
                {
                    int num2 = stream2.Read(buffer, 0, count);
                    contentLength += num2;
                    if (num2 == 0)
                    {
                        break;
                    }
                    stream.Write(buffer, 0, num2);
                }
                stream.Position = 0L;
                if (result.strUrl.Contains("Manifest"))
                {
                    StreamReader reader = new StreamReader(stream);
                    string s = reader.ReadToEnd().Replace("{start time})", "{start time})?" + this._decryptionInfo.SessionID).Replace("300000000", "550000000");
                    stream = new MemoryStream(Encoding.Unicode.GetBytes(s)) {
                        Position = 0L
                    };
                }
                CacheResponse response = new CacheResponse(contentLength, result2.Response.ContentType, null, stream, result2.Response.StatusCode, result2.Response.StatusDescription, DateTime.Now, true);
                stream.Position = 0L;
                return response;
            }
        }

        public void OpenMedia(Uri manifestUri)
        {
        }

        public bool IsStopped { get; set; }

        public bool IsStopping { get; set; }
    }
}

