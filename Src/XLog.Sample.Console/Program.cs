using System;
using System.Threading.Tasks;
using XLog.Formatters;
using XLog.NET.Targets;

namespace XLog.Sample.Console
{
    internal class Program
    {
        private static void Main()
        {
            InitLogger();

            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            try
            {
                Logic.TestWrite();
            }
            catch (Exception ex)
            {
                LogManager.Default.GetLogger("CrashLogger").Error("oops", ex);
                throw;
            }
            finally
            {
                LogManager.Default.Flush();
            }

            System.Console.ReadLine();
        }

        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            LogManager.Default.GetLogger("UnobservedCrashLogger").Error("oops", e.Exception);

        }

        private static void InitLogger()
        {
            var formatter = new LineFormatter();
            var logConfig = new LogConfig(formatter);

            logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal, new SyncFileTarget(@"D:\Log.log"));

            LogManager.Init(logConfig);
        }
    }
}