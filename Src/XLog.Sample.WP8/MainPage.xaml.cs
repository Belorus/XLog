using System;
using System.IO;
using System.Windows;
using Windows.Storage;
using Microsoft.Phone.Controls;
using XLog.Formatters;
using XLog.NET.Targets;

namespace XLog.Sample.WP8
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            InitLogger();
            Button.Click += OnButtonClick;
        }

        private void InitLogger()
        {
            var formatter = new LineFormatter();
            var logConfig = new LogConfig(formatter);

            logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal,
                new SyncFileTarget(Path.Combine(ApplicationData.Current.LocalFolder.Path, "Log")));

            LogManager.Init(logConfig);
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            var logger = LogManager.Default.GetLogger("TestLogger");

            logger.Debug(string.Format("Hello {0}", "World"));
            LogManager.Default.Flush();
        }
    }
}