using System.Diagnostics;

namespace XLog.Sample.Console
{
    internal static class LogCategory
    {
        static LogCategory()
        {
            FirstCategory = LogManager.Default.Config.CategoryRegistrar.Register("FirstCategory");
            SecondCategory = LogManager.Default.Config.CategoryRegistrar.Register("SecondCategory");
            ThirdCategory = LogManager.Default.Config.CategoryRegistrar.Register("ThirdCategory");
        }

        public static readonly long FirstCategory;
        public static readonly long SecondCategory;
        public static readonly long ThirdCategory;
    }

    internal static class Logic
    {
        const int Iterations = 1000000;
        private static readonly Logger Log = LogManager.Default.GetLogger("Logic", defaultCategory: LogCategory.SecondCategory);

        public static void TestWrite()
        {
            //Warmup
            Log.Info("Hello world!");

            var sw = Stopwatch.StartNew();

            for (int i = 0; i < Iterations; i++)
                Log.Info("Hello world!");

            sw.Stop();

            System.Console.WriteLine("Done {0:N0} records in {1:N0} ms", Iterations, +sw.ElapsedMilliseconds);
        }

        public static void TestCategory()
        {
            Log.InfoFormat(LogCategory.FirstCategory | LogCategory.SecondCategory, "Test category message {0}", 100500);
            Log.Info(LogCategory.SecondCategory, "Test category message");
            Log.Info(LogCategory.FirstCategory | LogCategory.ThirdCategory, "Test category message");
            Log.Info("Default category");
        }
    }
}