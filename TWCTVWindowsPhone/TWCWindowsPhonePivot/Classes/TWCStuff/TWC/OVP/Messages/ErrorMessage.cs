namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    public class ErrorMessage
    {
        public event Action MessageDismissed;

        public ErrorMessage(string title, string message, string detail = "", ErrorMessageType messageType = 0, int priority = 0x3e8, Action dismissedEvent = null, bool logError = true)
        {
            this.Title = title;
            this.Message = message;
            this.Detail = detail;
            this.LogError = logError;
            this.MessageType = messageType;
            this.MessageDismissed += dismissedEvent;
            this.Priority = priority;
        }

        public void Dismissed()
        {
            if (this.MessageDismissed != null)
            {
                this.MessageDismissed();
            }
        }

        public string Detail { get; set; }

        public bool LogError { get; set; }

        public string Message { get; set; }

        public ErrorMessageType MessageType { get; set; }

        public int Priority { get; set; }

        public string Title { get; set; }
    }
}

