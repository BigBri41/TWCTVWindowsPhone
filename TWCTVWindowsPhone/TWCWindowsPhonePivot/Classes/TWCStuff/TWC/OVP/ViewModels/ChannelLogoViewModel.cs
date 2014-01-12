namespace TWC.OVP.ViewModels
{
    using System;
    using System.Runtime.CompilerServices;
    using TWC.OVP.Models;

    public class ChannelLogoViewModel
    {
        public ChannelLogoViewModel(ChannelLogo model)
        {
            this.Dimensions = model.Dimensions;
            this.Url = model.Url;
        }

        public string Dimensions { get; private set; }

        public string Url { get; private set; }
    }
}

