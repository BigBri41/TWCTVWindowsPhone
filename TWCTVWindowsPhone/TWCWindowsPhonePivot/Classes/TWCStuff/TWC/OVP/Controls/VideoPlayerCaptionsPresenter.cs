namespace TWC.OVP.Controls
{
    using Microsoft.SilverlightMediaFramework.Core.Accessibility.Controls;
    using Microsoft.SilverlightMediaFramework.Utilities.Extensions;
    using System;

    public class VideoPlayerCaptionsPresenter : CaptionsPresenter
    {
        public void RedrawActiveCaptions()
        {
            this.GetVisualChildren<VideoPlayerCaptionBlockRegion>().ForEach<VideoPlayerCaptionBlockRegion>(i => i.RedrawActiveCaptions());
        }

        public void UpdateCaptionsTWC(TimeSpan mediaPosition, bool isSeeking)
        {
            this.GetVisualChildren<VideoPlayerCaptionBlockRegion>().ForEach<VideoPlayerCaptionBlockRegion>(i => i.UpdateCaptions(mediaPosition, isSeeking));
        }
    }
}

