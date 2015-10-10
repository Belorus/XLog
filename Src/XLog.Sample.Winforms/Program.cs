using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using XLog.Formatters;
using XLog.NET.Targets;

namespace XLog.Sample.Winforms
{
    static class Program
    {
        public static Logger Logger;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            InitLogger();
           
            Logger = LogManager.Default.GetLogger("Program");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            Application.ThreadException += ApplicationOnThreadException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            LogManager.Default.Flush();
        }

        private static void ApplicationOnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Logger.Error("ApplicationOnThreadException", e.Exception);
            LogManager.Default.Flush();

            Application.Exit();
        }

        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Logger.Error("TaskSchedulerOnUnobservedTaskException", e.Exception);
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error("CurrentDomainOnUnhandledException", (Exception)e.ExceptionObject);
            LogManager.Default.Flush();

            Application.Exit();
        }

        private static void InitLogger()
        {
            var formatter = new LineFormatter();
            var logConfig = new LogConfig(formatter);

            logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal, new SyncFileTarget(@"I:\Log.log"));

            LogManager.Init(logConfig);
        }
    }
}
