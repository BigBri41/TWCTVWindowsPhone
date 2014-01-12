namespace TWC.OVP.Behaviors
{
    using System;
    using System.Windows;
    using TWC.OVP.Framework.Controls;

    public static class TimelineBehaviors
    {
        public static readonly DependencyProperty PlaybackResponseProperty = DependencyProperty.RegisterAttached("PlaybackResponse", typeof(TimeSpan), typeof(TimelineBehaviors), new PropertyMetadata(new PropertyChangedCallback(TimelineBehaviors.OnPlaybackResponseChanged)));

        public static TimeSpan GetPlaybackResponse(DependencyObject o)
        {
            return (TimeSpan) o.GetValue(PlaybackResponseProperty);
        }

        private static void OnPlaybackResponseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ConstrainedTimeline timeline = (ConstrainedTimeline) d;
            TimeSpan newValue = (TimeSpan) e.NewValue;
            timeline.PlaybackPosition = newValue;
            timeline.UpdateTimeline();
        }

        public static void SetPlaybackResponse(DependencyObject o, TimeSpan value)
        {
            o.SetValue(PlaybackResponseProperty, value);
        }
    }
}

