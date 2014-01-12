namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class EGUserMessageDisplayed
    {
        public EGUserMessageDisplayed(string message, string displayType)
        {
            this.Message = message;
            this.DisplayType = displayType;
        }

        public string DisplayType { get; set; }

        public string Message { get; set; }
    }
}

