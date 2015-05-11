using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using XLog.Formatters;

namespace XLog.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var formatter = new LineFormatter();
            var logConfig = new LogConfig(formatter) { IsEnabled = true };
            //var fastFileTarget = new FastFileTarget("F:\\Logs", "Log");
            //logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal, fastFileTarget);
            //logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal, new SyncFileTarget("Logs", "Log"));
            //logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal, new ConsoleTarget());
            //logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal, new DebugTarget());
            logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal, new NullTarget());

            LogManager.Init(logConfig);
            //CancelTestTest(fastFileTarget);
            Console.WriteLine("Press any key to start test");
            Console.ReadKey();
            Test2();
            //fastFileTarget.Dispose();
            Console.ReadKey();
        }

        private static async void CancelTestTest(FastFileTarget target)
        {
            await Task.Delay(5000);
            target.Dispose();
        }

        private static void Test()
        {
            var now = DateTime.Now;
            Task.WhenAll(Enumerable.Range(0, 100).Select(_ => new Foo()).Select(f => f.DoWorkAsync()).Concat(Enumerable.Range(0, 100).Select(_ => new Bar()).Select(f => f.DoWorkAsync()))).Wait();
            var elapsed = DateTime.Now - now;
            Console.WriteLine("Took {0}", elapsed);
        }

        private static void Test2()
        {
            var now = DateTime.Now;
            Task.WhenAll(Task.Factory.StartNew(new Foo().DoWork), Task.Factory.StartNew(new Bar().DoWork)).Wait();
            var elapsed = DateTime.Now - now;
            Console.WriteLine("Took {0}", elapsed);
        }
    }

    public class Foo
    {
        private static readonly Logger Log = LogManager.Default.GetLogger("Foo");

        public static int Id;
        private readonly int _id;

        public Foo()
        {
            _id = Id++;
        }

        public async Task DoWorkAsync()
        {
            int i = 0;
            while (i++ < 1000)
            {
                await Task.Delay(5);
                Log.Debug("id = {0}, i = {1} very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very long string", _id, i);
            }
        }

        public void DoWork()
        {
            int i = 0;
            while (i++ < 100000)
            {
                Log.Debug("id = {0}, i = {1} very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very long string", _id, i);
            }
        }
    }

    public class Bar
    {
        private static readonly Logger Log = LogManager.Default.GetLogger("Bar");

        public static int Id;
        private readonly int _id;

        public Bar()
        {
            _id = Id++;
        }

        public async Task DoWorkAsync()
        {
            int i = 0;
            while (i++ < 1000)
            {
                await Task.Delay(5);
                Log.Debug("id = {0}, i = {1} very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very long string", _id, i);
            }
        }

        public void DoWork()
        {
            int i = 0;
            while (i++ < 100000)
            {
                Log.Debug("id = {0}, i = {1} very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very long string", _id, i);
            }
        }
    }
}
