using XLog.Formatters;
using XLog.NET.Targets;

namespace XLog.Sample.Console
{
    class Program
    {
        static void Main()
        {
            InitLogger();

            Logic.TestWrite();

            LogManager.Default.Flush();
        }

        private static void InitLogger()
        {
            var formatter = new LineFormatter();
            var logConfig = new LogConfig(formatter);

            logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal, new NullTarget());

            LogManager.Init(logConfig);
        }
    }
}
