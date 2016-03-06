using System;
using System.Diagnostics;
using System.IO;
using Android.App;
using Android.OS;
using Android.Widget;
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

            var btn = FindViewById<Button>(Resource.Id.MyButton);
            btn.Click += BtnOnClick;
        }

        private void BtnOnClick(object sender, EventArgs eventArgs)
        {
            var log = LogManager.Default.GetLogger("PerfLogger");
            log.Info("Hello world!");

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++)
            {
                log.Info("Hello world!");
            }
            sw.Stop();
            Toast.MakeText(this, sw.ElapsedMilliseconds.ToString(), ToastLength.Long).Show();
        }

        private void InitLogging()
        {
            var formatter = new LineFormatter();
            var logConfig = new LogConfig(formatter);
            var target = new SyncFileTarget(Path.Combine(global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "Log.log"));
            logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal, target);
            LogManager.Init(logConfig);
        }
    }
}

