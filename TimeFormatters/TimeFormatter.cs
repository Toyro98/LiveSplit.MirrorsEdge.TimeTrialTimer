using System;

namespace LiveSplit.TimeFormatters
{
    public class TimeFormatter : ITimeFormatter
    {
        public TimeAccuracy Accuracy { get; set; }

        public TimeFormatter(TimeAccuracy accuracy)
        {
            Accuracy = accuracy;
        }

        public string Format(TimeSpan? time)
        {
            if (time == null)
            {
                return TimeFormatConstants.DASH;
            }

            var shortTime = new ShortTimeFormatter().Format(time);

            if (Accuracy == TimeAccuracy.Hundredths)
            {
                return shortTime;
            }
            else if (Accuracy == TimeAccuracy.Tenths)
            {
                return shortTime.Substring(0, shortTime.IndexOf('.') + 2);
            }

            return shortTime.Substring(0, shortTime.IndexOf('.'));
        }
    }
}