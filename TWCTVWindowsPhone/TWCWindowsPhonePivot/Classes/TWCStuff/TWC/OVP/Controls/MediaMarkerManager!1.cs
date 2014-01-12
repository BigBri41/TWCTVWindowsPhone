namespace TWC.OVP.Controls
{
    using Microsoft.SilverlightMediaFramework.Core.Media;
    using Microsoft.SilverlightMediaFramework.Utilities.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    public class MediaMarkerManager<TMediaMarker> where TMediaMarker: MediaMarker
    {
        private readonly object _syncObject;
        private const long LargestNormalStepSizeMillis = 0x3e8L;

        public event System.Action<MediaMarkerManager<TMediaMarker>, TMediaMarker> MarkerLeft;

        public event System.Action<MediaMarkerManager<TMediaMarker>, TMediaMarker, bool> MarkerReached;

        public MediaMarkerManager()
        {
            this._syncObject = new object();
            this.PreviouslyActiveMarkers = new List<TMediaMarker>();
            this.LargestNormalStepSize = TimeSpan.FromMilliseconds(1000.0);
        }

        public void CheckMarkerPositions(TimeSpan mediaPosition, MediaMarkerCollection<TMediaMarker> markers, bool seeking = false, bool ignoreSearchWindow = false)
        {
            System.Func<TMediaMarker, bool> func = null;
            System.Func<TMediaMarker, bool> func2 = null;
            System.Func<TMediaMarker, bool> func3 = null;
            System.Func<TMediaMarker, bool> func4 = null;
            Action<TMediaMarker> action = null;
            object obj2;
            bool lockTaken = false;
            try
            {
                IList<TMediaMarker> activeMarkers;
                Monitor.Enter(obj2 = this._syncObject, ref lockTaken);
                if (func == null)
                {
                    func = i => !markers.Contains(i);
                }
                if (func2 == null)
                {
                    func2 = i => base.PreviouslyActiveMarkers.Remove(i);
                }
                Enumerable.Where<TMediaMarker>(this.PreviouslyActiveMarkers.ToList<TMediaMarker>(), func).ForEach<TMediaMarker>(new Action<TMediaMarker>(this.OnMarkerLeft)).ForEach<TMediaMarker, bool>(func2);
                if (!seeking && this.PreviousPosition.HasValue)
                {
                    TimeSpan ts = (this.PreviousPosition.Value < mediaPosition) ? this.PreviousPosition.Value : mediaPosition;
                    TimeSpan span2 = (this.PreviousPosition.Value > mediaPosition) ? this.PreviousPosition.Value : mediaPosition;
                    seeking = span2.Subtract(ts) > this.LargestNormalStepSize;
                }
                TimeSpan? searchAfter = (this.SeekingSearchWindow.HasValue && !ignoreSearchWindow) ? new TimeSpan?(mediaPosition.Subtract(this.SeekingSearchWindow.Value)) : null;
                if (!seeking && this.PreviousPosition.HasValue)
                {
                    TimeSpan rangeStart = (this.PreviousPosition.Value < mediaPosition) ? this.PreviousPosition.Value : mediaPosition;
                    TimeSpan rangeEnd = (this.PreviousPosition.Value > mediaPosition) ? this.PreviousPosition.Value : mediaPosition;
                    activeMarkers = markers.WhereActiveInRange(rangeStart, rangeEnd, searchAfter).ToList<TMediaMarker>();
                }
                else
                {
                    activeMarkers = markers.WhereActiveAtPosition(mediaPosition, searchAfter).ToList<TMediaMarker>();
                }
                if (func3 == null)
                {
                    func3 = i => !i.IsActiveAtPosition(mediaPosition);
                }
                Enumerable.Where<TMediaMarker>(this.PreviouslyActiveMarkers, func3).ForEach<TMediaMarker>(new Action<TMediaMarker>(this.OnMarkerLeft)).ForEach<TMediaMarker, bool>(i => activeMarkers.Remove(i));
                if (func4 == null)
                {
                    func4 = delegate (TMediaMarker i) {
                        if (base.PreviousPosition.HasValue)
                        {
                            return !base.PreviouslyActiveMarkers.Contains(i);
                        }
                        return true;
                    };
                }
                if (action == null)
                {
                    action = i => ((MediaMarkerManager<TMediaMarker>) this).OnMarkerReached(i, seeking);
                }
                Enumerable.Where<TMediaMarker>(activeMarkers, func4).ForEach<TMediaMarker>(action);
                this.PreviouslyActiveMarkers = (from i in this.PreviouslyActiveMarkers
                    where i.IsActiveAtPosition(mediaPosition) && !activeMarkers.Contains(i)
                    select i).Concat<TMediaMarker>(activeMarkers).ToList<TMediaMarker>();
                this.PreviousPosition = new TimeSpan?(mediaPosition);
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(obj2);
                }
            }
        }

        protected virtual void OnMarkerLeft(TMediaMarker mediaMarker)
        {
            this.MarkerLeft.IfNotNull<System.Action<MediaMarkerManager<TMediaMarker>, TMediaMarker>>(i => i((MediaMarkerManager<TMediaMarker>) this, mediaMarker));
        }

        protected virtual void OnMarkerReached(TMediaMarker mediaMarker, bool skippedInto)
        {
            this.MarkerReached.IfNotNull<System.Action<MediaMarkerManager<TMediaMarker>, TMediaMarker, bool>>(i => i((MediaMarkerManager<TMediaMarker>) this, mediaMarker, skippedInto));
        }

        public void Reset()
        {
            this.PreviousPosition = null;
            this.PreviouslyActiveMarkers.Clear();
        }

        public TimeSpan LargestNormalStepSize { get; set; }

        protected IList<TMediaMarker> PreviouslyActiveMarkers { get; private set; }

        protected TimeSpan? PreviousPosition { get; private set; }

        public TimeSpan? SeekingSearchWindow { get; set; }
    }
}

