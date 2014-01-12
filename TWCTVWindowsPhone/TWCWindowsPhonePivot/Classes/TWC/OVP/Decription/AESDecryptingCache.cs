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
using System.Security.Cryptography;
using Microsoft.Web.Media.SmoothStreaming;
using System.Collections.Generic;
using System.Threading;

namespace TWC.OVP.Decryption
{
    public class AESDecryptingCache : ISmoothStreamingCache
    {
        // Fields
        private static Dictionary<Uri, CacheItem> _cache;
        private AESDecryptionInfo _decryptionInfo;

        // Methods
        public AESDecryptingCache(AESDecryptionInfo decryptionInfo);
        public IAsyncResult BeginPersist(CacheRequest request, CacheResponse response, AsyncCallback callback, object state);
        public IAsyncResult BeginRetrieve(CacheRequest request, AsyncCallback callback, object state);
        public void CloseMedia(Uri manifestUri);
        public bool EndPersist(IAsyncResult ar);
        public CacheResponse EndRetrieve(IAsyncResult ar);
        public void OpenMedia(Uri manifestUri);

        // Properties
        public bool IsStopped { get; set; }
        public bool IsStopping { get; set; }
    }

    public class AESDecryptingCacheFullRequest : ISmoothStreamingCache
    {
        // Fields
        private static Dictionary<Uri, CacheItem> _cache;
        private AESDecryptionInfo _decryptionInfo;

        // Methods
        public AESDecryptingCacheFullRequest(AESDecryptionInfo decryptionInfo);
        public IAsyncResult BeginPersist(CacheRequest request, CacheResponse response, AsyncCallback callback, object state);
        public IAsyncResult BeginRetrieve(CacheRequest request, AsyncCallback callback, object state);
        public void CloseMedia(Uri manifestUri);
        public bool EndPersist(IAsyncResult ar);
        public CacheResponse EndRetrieve(IAsyncResult ar);
        public void OpenMedia(Uri manifestUri);

        // Properties
        public bool IsStopped { get; set; }
        public bool IsStopping { get; set; }
    }

    public class AESDecryptionInfo
    {
        // Fields
        private static AesManaged _aes;

        // Methods
        static AESDecryptionInfo();
        public AESDecryptionInfo(string iv, byte[] key);
        private byte[] HexStringToBytes(string hexString);

        // Properties
        public ICryptoTransform Decryptor { get; set; }
        public SmoothStreamingMediaElement MediaElement { get; set; }
        public string SessionID { get; set; }
    }

    public class AESOVPRequestDecryptingCache : ISmoothStreamingCache
    {
        // Fields
        private static Dictionary<Uri, CacheItem> _cache;
        private AESDecryptionInfo _decryptionInfo;

        // Methods
        public AESOVPRequestDecryptingCache(AESDecryptionInfo decryptionInfo);
        public IAsyncResult BeginPersist(CacheRequest request, CacheResponse response, AsyncCallback callback, object state);
        public IAsyncResult BeginRetrieve(CacheRequest request, AsyncCallback callback, object state);
        public void CloseMedia(Uri manifestUri);
        public bool EndPersist(IAsyncResult ar);
        public CacheResponse EndRetrieve(IAsyncResult ar);
        public void OpenMedia(Uri manifestUri);

        // Properties
        public bool IsStopped { get; set; }
        public bool IsStopping { get; set; }
    }

    public class CacheAsyncResult : IAsyncResult
    {
        // Fields
        private ManualResetEvent _completeEvent;

        // Methods
        public CacheAsyncResult();
        public void Complete(object result, bool completedSynchronously);

        // Properties
        public object AsyncState { get; internal set; }
        public WaitHandle AsyncWaitHandle { get; }
        public bool CompletedSynchronously { get; private set; }
        public bool IsCompleted { get; private set; }
        public object Result { get; private set; }
        public string strUrl { get; set; }
        internal TimeSpan Timestamp { get; private set; }
    }

    public class CacheItem
    {
        // Methods
        public CacheItem();

        // Properties
        public long ContentLength { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
        public DateTime DownloadTime { get; set; }
        public WebHeaderCollection Headers { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }
    }
}


 

