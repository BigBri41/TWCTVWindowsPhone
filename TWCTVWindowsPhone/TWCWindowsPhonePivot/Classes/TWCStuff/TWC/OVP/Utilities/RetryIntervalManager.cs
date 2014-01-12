namespace TWC.OVP.Utilities
{
    using System;

    public class RetryIntervalManager
    {
        private int _IndefinitInterval;
        private int[] _InitialRetryIntervals;
        private int _RetryCount;

        public RetryIntervalManager()
        {
            this._InitialRetryIntervals = new int[] { 
                10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 30, 30, 30, 30, 
                30, 30, 60, 60, 60, 60, 60
             };
            this._IndefinitInterval = 600;
        }

        public RetryIntervalManager(int[] initialIntervals, int indefinitInterval)
        {
            this._InitialRetryIntervals = new int[] { 
                10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 30, 30, 30, 30, 
                30, 30, 60, 60, 60, 60, 60
             };
            this._IndefinitInterval = 600;
            this._InitialRetryIntervals = initialIntervals;
            this._IndefinitInterval = indefinitInterval;
        }

        public TimeSpan GetNextInterval()
        {
            int num = this._IndefinitInterval;
            if (this._RetryCount < this._InitialRetryIntervals.Length)
            {
                num = this._InitialRetryIntervals[this._RetryCount];
            }
            this._RetryCount++;
            return TimeSpan.FromSeconds((double) num);
        }

        public void Reset()
        {
            this._RetryCount = 0;
        }
    }
}

