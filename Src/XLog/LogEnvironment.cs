namespace XLog
{
    public static class LogEnvironment
    {
        public static int CurrentManagedThreadId
        {
            get
            {
#if UNITY
                return System.Threading.Thread.CurrentThread.ManagedThreadId;
#else
                return System.Environment.CurrentManagedThreadId;
#endif
            }
        }
    }
}
