namespace XLog.Sample.Console
{
    internal static class Logic
    {
        private static readonly Logger Log = LogManager.Default.GetLogger("Logic");

        public static void TestWrite()
        {
            Log.Info("Hello world!");
        }
    }
}