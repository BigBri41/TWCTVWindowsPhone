namespace TWC.OVP.Messages
{
    using Microsoft.SilverlightMediaFramework.Plugins.Primitives;
    using System;
    using System.Runtime.CompilerServices;

    public class PlayStateChangedMessage
    {
        public PlayStateChangedMessage(MediaPluginState playState, string changeCause)
        {
            this.PlayState = playState;
            if (changeCause == null)
            {
                changeCause = "";
            }
            this.StateChangeCause = changeCause;
        }

        public MediaPluginState PlayState { get; set; }

        public string StateChangeCause { get; set; }
    }
}

