using System;
using System.Linq;
using System.Threading.Tasks;
using XLog.Formatters;

namespace XLog.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var logConfig = new LogConfig { IsEnabled = true };
            var formatter = new LineFormatter();
            logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal, new SyncFileTarget(formatter, "Logs", "Log"));
            logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal, new ConsoleTarget(formatter));
            logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal, new DebugTarget(formatter));

            LogManager.Init(logConfig);
            Test();
            Console.ReadKey();
        }

        private static void Test()
        {
            var now = DateTime.Now;
            Task.WhenAll(Enumerable.Range(0, 100).Select(_ => new Foo()).Select(f => f.DoWork()).Concat(Enumerable.Range(0, 100).Select(_ => new Bar()).Select(f => f.DoWork()))).Wait();
            var elapsed = DateTime.Now - now;
            Console.WriteLine("Took {0}", elapsed);
        }
    }

    public class Foo
    {
        private static readonly ILogger Log = LogManager.Default.GetLogger("Foo");

        public static int Id;
        private readonly int _id;

        public Foo()
        {
            _id = Id++;
        }

        public async Task DoWork()
        {
            int i = 0;
            while (i++ < 1000)
            {
                await Task.Delay(50);
                Log.Debug("id = {0}, i = {0}", _id, i);
            }
        }
    }

    public class Bar
    {
        private static readonly ILogger Log = LogManager.Default.GetLogger("Bar");

        public static int Id;
        private readonly int _id;

        public Bar()
        {
            _id = Id++;
        }

        public async Task DoWork()
        {
            int i = 0;
            while (i++ < 1000)
            {
                await Task.Delay(50);
                Log.Debug("id = {0}, i = {0}", _id, i);
            }
        }
    }
}
