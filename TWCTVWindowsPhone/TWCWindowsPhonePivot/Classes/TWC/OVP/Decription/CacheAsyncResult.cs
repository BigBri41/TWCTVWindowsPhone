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
using System.Threading;

namespace TWC.OVP.Decription
{
public class CacheAsyncResult : IAsyncResult
{
    // Fields
    private ManualResetEvent _completeEvent = new ManualResetEvent(false);

    // Methods
    public void Complete(object result, bool completedSynchronously)
    {
        this.Result = result;
        this.CompletedSynchronously = completedSynchronously;
        this.IsCompleted = true;
        this._completeEvent.Set();
    }

    // Properties
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
