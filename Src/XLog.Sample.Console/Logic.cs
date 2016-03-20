using System.Diagnostics;

namespace XLog.Sample.Console
{
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
    }
}