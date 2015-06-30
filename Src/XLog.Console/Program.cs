using System;
using System.Diagnostics;
using XLog.Formatters;

namespace XLog.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var formatter = new LineFormatter();
            var logConfig = new LogConfig(formatter);
            
            //logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal, new FastFileTarget("F:\\Logs", "Log"));
            logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal, new SyncFileTarget("Logs", "Log"));
            //logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal, new ConsoleTarget());
            //logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal, new DebugTarget());
            //logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal, new NullTarget());

            try
            {
                LogManager.Init(logConfig);
                Measure();
            }
            finally
            {
                LogManager.Default.Flush();
            }
            Console.ReadKey();
        }

        private static void Measure()
        {
            var sw = Stopwatch.StartNew();
            new LogUser().DoWork();
            var elapsed = sw.Elapsed;
            Console.WriteLine("Took {0}", elapsed);
        }
    }

    public class LogUser
    {
        private static readonly Logger Log = LogManager.Default.GetLogger("Foo");

        public static int Id;
        private readonly int _id;

        public LogUser()
        {
            _id = Id++;
        }

        public void DoWork()
        {
            int i = 0;
            while (i++ < 200000)
            {
                Log.Debug(string.Format("id = {0}, i = {1} very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very long string", _id, i));
            }
        }
    }
}
