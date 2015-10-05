using System;

namespace XLog
{
    public class Entry
    {
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
            TimeStamp = DateTime.UtcNow;
        }
    }
}
