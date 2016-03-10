using System;
using System.Diagnostics;

namespace XLog
{
    public class Entry
    {
        // This stuff is an optimization. DateTime.Now is a very expensive operation.
        // It makes 2 calls to OS API. First to get time, seconds to calculate timezone offset
        private static readonly long BaseTimeTicks;
        private static readonly Stopwatch Delta;

        static Entry()
        {
            BaseTimeTicks = DateTime.Now.Ticks;
            Delta = Stopwatch.StartNew();
        }

        public LogLevel Level;
        public string LevelStr;
        public string Tag;
        public string Message;
        public long Category;
        public DateTime TimeStamp;
        public Exception Exception;

        public Entry(LogLevel level, string tag, string message, long category, Exception ex)
        {
            Level = level;
            LevelStr = LogLevels.Levels[(int)level];
            Tag = tag;
            Message = message;
            Category = category;
            Exception = ex;
            TimeStamp = new DateTime(BaseTimeTicks + Delta.ElapsedTicks);
        }
    }
}
