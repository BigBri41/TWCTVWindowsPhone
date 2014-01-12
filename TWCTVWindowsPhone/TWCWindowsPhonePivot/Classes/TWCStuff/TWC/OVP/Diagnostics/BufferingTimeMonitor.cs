namespace TWC.OVP.Diagnostics
{
    using Microsoft.SilverlightMediaFramework.Core;
    using Microsoft.SilverlightMediaFramework.Plugins.Primitives;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using TWC.OVP.Controls;

    public class BufferingTimeMonitor
    {
        private List<BufferEvent> _bufferEvents;
        private DateTime _playbackStartTime;

        public BufferingTimeMonitor(VideoPlayer player)
        {
            EventHandler<CustomEventArgs<PlaylistItem>> handler = null;
            EventHandler<CustomEventArgs<MediaPluginState>> handler2 = null;
            this._bufferEvents = new List<BufferEvent>();
            if (handler == null)
            {
                handler = (s, e) => this.Reset();
            }
            player.PlaylistItemChanged += handler;
            if (handler2 == null)
            {
                handler2 = delegate (object s, CustomEventArgs<MediaPluginState> e) {
                    if (((MediaPluginState) e.Value) == MediaPluginState.Buffering)
                    {
                        this.BufferingStarting();
                    }
                    else
                    {
                        this.BufferingEnding();
                    }
                };
            }
            player.PlayStateChanged += handler2;
        }

        private void BufferingEnding()
        {
            BufferEvent event2 = (from e in this._bufferEvents
                where e.IsOpen
                select e).LastOrDefault<BufferEvent>();
            if (event2 != null)
            {
                event2.End = DateTime.Now;
            }
        }

        private void BufferingStarting()
        {
            BufferEvent item = new BufferEvent {
                Start = DateTime.Now
            };
            this._bufferEvents.Add(item);
        }

        public int GetBufferingEventCount(double seconds = 0.0)
        {
            DateTime startTime = this.GetStartTime(seconds);
            DateTime now = DateTime.Now;
            return this.GetEventsWithinWindow(startTime, now).Count<BufferEvent>();
        }

        public double GetBufferPercentage(double seconds = 0.0)
        {
            DateTime start = this.GetStartTime(seconds);
            DateTime end = DateTime.Now;
            double num = Enumerable.Sum<BufferEvent>(from e in this.GetEventsWithinWindow(start, end) select new BufferEvent { Start = (e.Start >= start) ? e.Start : start, End = (e.End <= end) ? e.End : end }, e => ((TimeSpan) (e.End - e.Start)).TotalSeconds);
            TimeSpan span = (TimeSpan) (end - start);
            double totalSeconds = span.TotalSeconds;
            return ((100.0 * num) / totalSeconds);
        }

        private IEnumerable<BufferEvent> GetEventsWithinWindow(DateTime start, DateTime end)
        {
            return (from e in this._bufferEvents
                where ((e.Start >= start) && (e.Start <= end)) || ((e.End >= start) && (e.End <= end))
                select e);
        }

        private DateTime GetStartTime(double seconds)
        {
            if (seconds <= 0.0)
            {
                return this._playbackStartTime;
            }
            DateTime time = DateTime.Now.AddSeconds(-seconds);
            if (time < this._playbackStartTime)
            {
                return this._playbackStartTime;
            }
            return time;
        }

        private void Reset()
        {
            this._bufferEvents.Clear();
            this._playbackStartTime = DateTime.Now;
        }

        private class BufferEvent
        {
            private DateTime _end;

            public DateTime End
            {
                get
                {
                    if (this._end == DateTime.MinValue)
                    {
                        return DateTime.Now;
                    }
                    return this._end;
                }
                set
                {
                    this._end = value;
                }
            }

            public bool IsOpen
            {
                get
                {
                    return (this._end == DateTime.MinValue);
                }
            }

            public DateTime Start { get; set; }
        }
    }
}

