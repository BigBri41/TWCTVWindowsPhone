namespace TWC.OVP.Messages
{
    using System;
    using System.Runtime.CompilerServices;

    public class EGStreamScrubbedMessage
    {
        public EGStreamScrubbedMessage(double scrubEnd, bool blocked, string interactionType)
        {
            this.ScrubEnd = scrubEnd;
            this.Blocked = blocked;
            this.InteractionType = interactionType;
        }

        public bool Blocked { get; set; }

        public string InteractionType { get; set; }

        public double ScrubEnd { get; set; }
    }
}

