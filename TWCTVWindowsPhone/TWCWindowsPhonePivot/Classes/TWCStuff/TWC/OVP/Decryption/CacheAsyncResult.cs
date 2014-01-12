namespace TWC.OVP.Decryption
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class CacheAsyncResult : IAsyncResult
    {
        private ManualResetEvent _completeEvent = new ManualResetEvent(false);

        public void Complete(object result, bool completedSynchronously)
        {
            this.Result = result;
            this.CompletedSynchronously = completedSynchronously;
            this.IsCompleted = true;
            this._completeEvent.Set();
        }

        public object AsyncState { get; internal set; }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                return this._completeEvent;
            }
        }

        public bool CompletedSynchronously { get; private set; }

        public bool IsCompleted { get; private set; }

        public object Result { get; private set; }

        public string strUrl { get; set; }

        internal TimeSpan Timestamp { get; private set; }
    }
}

