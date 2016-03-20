using System.Diagnostics;
using NLog.Config;
using NLog.Targets;

namespace NLog.Sample.Console
{
    class Program
    {
        const int Iterations = 1000000;

        static void Main(string[] args)
        {
            InitializeNLog();
            Test();
        }

        private static void Test()
        {
            Logger logger = LogManager.GetLogger("Logic");

            logger.Info("Hello world!");

            var sw = Stopwatch.StartNew();

            for (int i = 0; i < Iterations; i++)
                logger.Info("Hello world!");

            sw.Stop();
            System.Console.WriteLine("Done {0:N0} records in {1:N0} ms", Iterations, + sw.ElapsedMilliseconds);
        }

        private static void InitializeNLog()
        {
            var config = new LoggingConfiguration();
            var fileTarget = new FileTarget
            {
                Name = "fileTarget",
                FileName = @"NLog.log",
                AutoFlush = false,
                KeepFileOpen = true
            };

            config.AddTarget(fileTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, fileTarget));
            
            LogManager.Configuration = config;


        }
    }
}
