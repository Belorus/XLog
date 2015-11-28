using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XLog.Formatters;
using XLog.NET.Targets;

namespace XLog.Sample.W10
{
    sealed partial class App
    {
        public static Logger Logger;

        public App()
        {
            this.InitializeComponent();
            InitLogger();
            Logger = LogManager.Default.GetLogger("Program");


            Application.Current.UnhandledException += CurrentOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        }

        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Logger.Error("TaskSchedulerOnUnobservedTaskException", e.Exception);
            LogManager.Default.Flush();
        }

        private void CurrentOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Fatal("CurrentOnUnhandledException", e.Exception);
            LogManager.Default.Flush();
        }

        private static void InitLogger()
        {
            var formatter = new LineFormatter();
            var logConfig = new LogConfig(formatter);

            logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal, new SyncFileTarget(Path.Combine(ApplicationData.Current.LocalFolder.Path, @"Log.log")));

            LogManager.Init(logConfig);
        }







        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }

            Window.Current.Activate();
        }
    }
}
