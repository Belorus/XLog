using System.Diagnostics;

namespace XLog.Sample.Console
{
    internal static class LogCategory
    {
        static LogCategory()
        {
            FirstCategory = LogManager.Default.Config.CategoryRegistrar.Register("FirstCategory");
            SecondCategory = LogManager.Default.Config.CategoryRegistrar.Register("SecondCategory");
        }

        public static readonly long FirstCategory;
        public static readonly long SecondCategory;
    }

    internal static class Logic
    {
        const int Iterations = 1000000;
        private static readonly Logger Log = LogManager.Default.GetLogger("Logic");

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
            Log.Info(LogCategory.FirstCategory | LogCategory.SecondCategory, "Test category message");
            Log.Info(LogCategory.SecondCategory, "Test category message");
        }
    }
}