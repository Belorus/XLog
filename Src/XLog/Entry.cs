using System;

namespace XLog
{
    public class Entry
    {
        public int Level;
        public string LevelStr;
        public string Tag;
        public string Message;
        public DateTimeOffset TimeStamp;
        public Exception Exception;

        public Entry(int level, string tag, string message, Exception ex)
        {
            Level = level;
            LevelStr = FormatLevel(level);
            Tag = tag;
            Message = message;
            Exception = ex;
            TimeStamp = DateTimeOffset.UtcNow;
        }

        public static string FormatLevel(int logLevel)
        {
            switch (logLevel)
            {
                case 0: return "TRACE";
                case 1: return "DEBUG";
                case 2: return "INFO";
                case 3: return "WARN";
                case 4: return "ERROR";
                case 5: return "FATAL";
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}
