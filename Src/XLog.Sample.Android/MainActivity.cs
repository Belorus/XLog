using System;
using System.Diagnostics;
using Android.App;
using Android.Widget;
using Android.OS;
using XLog.Formatters;
using XLog.NET;

namespace XLog.Sample.Android
{
    [Activity(Label = "XLog.Sample.Android", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;
        private LogConfig logConfig;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            var formatter = new LineFormatter();
            logConfig = new LogConfig(formatter);

            
            logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal, new SyncFileTarget(global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "Log"));

            button.Click += OnButtonClick;
       }

        private void OnButtonClick(object sender, EventArgs e)
        {
            LogManager.Init(logConfig);

            var logger = LogManager.Default.GetLogger("TestLogger");

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 100000; i++)
            {
                logger.Debug(string.Format("Hello {0}", i));
            }
            LogManager.Default.Flush();
            sw.Stop();
            Toast.MakeText(this, sw.Elapsed.ToString(), ToastLength.Long).Show();
        }
    }
}

