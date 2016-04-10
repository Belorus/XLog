using System;

namespace XLog
{
    public struct Entry
    {
        // This stuff is an optimization. DateTime.Now is a very expensive operation.
        // It makes 2 calls to OS API. First to get time, seconds to calculate timezone offset
        private static readonly long TimezoneOffsetTicks;

        static Entry()
        {
            TimezoneOffsetTicks = (DateTime.Now - DateTime.UtcNow).Ticks;
        }

        public LogLevel Level;
        public string Tag;
        public string Message;
        public long Category;
        public DateTime TimeStamp;
        public Exception Exception;

        public Entry(LogLevel level, string tag, string message, long category, Exception ex)
        {
            Level = level;
            Tag = tag;
            Message = message;
            Category = category;
            Exception = ex;
            TimeStamp = DateTime.UtcNow.AddTicks(TimezoneOffsetTicks);
        }
    }
}
