using System.Diagnostics;
using System.Windows;
using Windows.Storage;
using Microsoft.Phone.Controls;
using XLog.Formatters;
using XLog.NET.Targets;

namespace XLog.Sample.WP8
{
    public partial class MainPage : PhoneApplicationPage
    {
        private readonly LogConfig logConfig;

        public MainPage()
        {
            InitializeComponent();

            var formatter = new LineFormatter();
            logConfig = new LogConfig(formatter);
            
            logConfig.AddTarget(LogLevel.Trace, LogLevel.Fatal, new SyncFileTarget(ApplicationData.Current.LocalFolder.Path, "Log"));
            
            Button.Click += OnButtonClick;
        }
                
        private void OnButtonClick(object sender, RoutedEventArgs e)
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
            MessageBox.Show(sw.Elapsed.ToString());
        }

        
    }
}