namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class UserInteractionMessage
    {
        public UserInteractionMessage(string action)
        {
            this.UserAction = action;
        }

        public string UserAction { get; set; }
    }
}

