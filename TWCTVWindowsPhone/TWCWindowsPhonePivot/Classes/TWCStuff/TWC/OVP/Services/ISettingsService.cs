namespace TWC.OVP.Services
{
    using System;
    using System.Collections.Generic;
    using TWC.OVP;
    using TWC.OVP.Framework.Models;

    public interface ISettingsService
    {
        TWC.OVP.AppMode AppMode { get; set; }

        string CachedGenreData { get; set; }

        string CachedGenreServiceData { get; set; }

        TWC.OVP.Framework.Models.CaptionsOverrideSettings CaptionsOverrideSettings { get; set; }

        bool IsClosedCaptioningEnabled { get; set; }

        bool IsMuted { get; set; }

        string PrevCaptionStreamLanguage { get; set; }

        string PrevCaptionStreamName { get; set; }

        List<string> RecentChannels { get; set; }

        string SelectedChannelFilter { get; set; }

        double VolumeLevel { get; set; }
    }
}

