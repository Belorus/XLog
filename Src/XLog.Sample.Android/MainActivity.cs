using System.IO;
using Android.App;
using Android.OS;
using XLog.Formatters;
using XLog.NET.Targets;

namespace XLog.Sample.Android
{
    [Activity(Label = "XLog.Sample.Android", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            InitLogging();

            var logger = LogManager.Default.GetLogger("TestLogger");
            logger.Debug(string.Format("Hello {0}", "World"));

        }

        private void InitLogging()
        {
            var formatter = new LineFormatter();
            var logConfig = new LogConfig(formatter);

            logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal, new SyncFileTarget(Path.Combine(global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "Log.log")));
            LogManager.Init(logConfig);
        }
    }
}

