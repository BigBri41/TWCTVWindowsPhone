namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class RSNStreamStartedEventMessage
    {
        public RSNStreamStartedEventMessage(params object[] args)
        {
            this.Args = args;
        }

        public object[] Args { get; set; }
    }
}

