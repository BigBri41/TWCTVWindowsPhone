namespace TWC.OVP.Controls
{
    using Microsoft.SilverlightMediaFramework.Core;
    using Microsoft.Web.Media.SmoothStreaming;
    using System;
    using System.Linq;

    public class TextTimeline
    {
        private static TextTimeline _instance;

        private TextTimeline(VideoPlayer player)
        {
            player.PlaybackPositionChanged += new EventHandler<CustomEventArgs<TimeSpan>>(this.player_PlaybackPositionChanged);
        }

        private string GetTimelineString(SmoothStreamingMediaElement ssme)
        {
            string str = "";
            TimeSpan span = ssme.StartPosition.Add(TimeSpan.FromSeconds(-30.0));
            for (int i = 0; i < 90; i++)
            {
                long totalSeconds = (long) span.TotalSeconds;
                if ((totalSeconds - ((long) ssme.StartPosition.TotalSeconds)) == 0L)
                {
                    str = str + "S";
                }
                if ((totalSeconds - ((long) ssme.Position.TotalSeconds)) == 0L)
                {
                    str = str + "P";
                }
                if ((totalSeconds - ((long) ssme.EndPosition.TotalSeconds)) == 0L)
                {
                    str = str + "E";
                }
                if ((totalSeconds - ((long) ssme.LivePosition)) == 0L)
                {
                    str = str + "L";
                }
                else
                {
                    str = str + "-";
                }
                span = span.Add(TimeSpan.FromSeconds(1.0));
            }
            if (((ssme.ManifestInfo != null) && (ssme.ManifestInfo.Segments != null)) && ssme.ManifestInfo.Segments.Any<SegmentInfo>())
            {
                foreach (StreamInfo info in ssme.ManifestInfo.Segments.First<SegmentInfo>().SelectedStreams)
                {
                    if (info.Attributes["type"] == "video")
                    {
                        str = str + " | " + info.Attributes["type"] + " ";
                        foreach (TrackInfo info2 in info.AvailableTracks)
                        {
                            bool flag = ssme.VideoPlaybackTrack == info2;
                            if (flag)
                            {
                                str = str + "[";
                            }
                            str = str + info2.Bitrate;
                            if (flag)
                            {
                                str = str + "]";
                            }
                            str = str + " ";
                        }
                    }
                }
            }
            return str;
        }

        public static void Initialize(VideoPlayer player)
        {
            _instance = new TextTimeline(player);
        }

        private void player_PlaybackPositionChanged(object sender, CustomEventArgs<TimeSpan> e)
        {
            SmoothStreamingMediaElement sSME = ((VideoPlayer) sender).SSME;
        }
    }
}

