namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class RSNPlayerBeaconEventMessage
    {
        public RSNPlayerBeaconEventMessage(params object[] agrs)
        {
            this.Args = agrs;
        }

        public object[] Args { get; set; }
    }
}

