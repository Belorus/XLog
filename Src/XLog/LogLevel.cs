namespace XLog
{
    public static class LogLevels
    {
        public static readonly string[] Levels =
        {
            "TRACE",
            "DEBUG",
            "INFO ",
            "WARN ",
            "ERROR",
            "FATAL"
        };

        public static int Count
        {
            get { return Levels.Length; }
        }
    }

    public enum LogLevel
    {
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warn = 3,
        Error = 4,
        Fatal = 5,
    }
}
