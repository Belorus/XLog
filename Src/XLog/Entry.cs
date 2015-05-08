using System;

namespace XLog
{
    public class Entry
    {
        private static readonly string[] Levels = {
            "TRACE",
            "DEBUG",
            "INFO",
            "WARN",
            "ERROR",
            "FATAL"
        };

        public int Level;
        public string LevelStr;
        public string Tag;
        public string Message;
        public DateTimeOffset TimeStamp;
        public Exception Exception;

        public Entry(int level, string tag, string message, Exception ex)
        {
            Level = level;
            LevelStr = Levels[level];
            Tag = tag;
            Message = message;
            Exception = ex;
            TimeStamp = DateTimeOffset.UtcNow;
        }
    }
}
