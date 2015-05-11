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

        public LogLevel Level;
        public string LevelStr;
        public string Tag;
        public string Message;
        public DateTime TimeStamp;
        public Exception Exception;

        public Entry(LogLevel level, string tag, string message, Exception ex)
        {
            Level = level;
            LevelStr = Levels[(int)level];
            Tag = tag;
            Message = message;
            Exception = ex;
            TimeStamp = DateTime.UtcNow;
        }
    }
}
