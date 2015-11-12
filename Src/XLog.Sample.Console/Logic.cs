using System.Diagnostics;

namespace XLog.Sample.Console
{
    internal static class Logic
    {
        private static readonly Logger Log = LogManager.Default.GetLogger("Logic");

        public static void TestWrite()
        {
            var sw = Stopwatch.StartNew();

            for (int i = 0; i < 1000000; i++)
                Log.Info("Hello world!");

            sw.Stop();
            System.Console.WriteLine("Done in " + sw.ElapsedMilliseconds);

        }
    }
}