namespace TWC.OVP.Controls
{
    using System;
    using System.Runtime.CompilerServices;

    public class VideoPlayerErrorEventArgs : EventArgs
    {
        public VideoPlayerErrorEventArgs(string errorMessage, string errorDetail)
        {
            this.ErrorMessage = errorMessage;
            this.ErrorDetail = errorDetail;
        }

        public string ErrorDetail { get; set; }

        public string ErrorMessage { get; set; }
    }
}

