using System;
using XLog.Categories;
using XLog.Formatters;

namespace XLog
{
    public class LogManager
    {
        private static LogManager _instance;

        private static readonly LogManager NullLogManager = new LogManager(new LogConfig(new NullFormatter(), new LogCategoryRegistrar()) { IsEnabled = false });

        public static LogManager Default
        {
            get { return _instance ?? NullLogManager; }
        }

        public readonly LogConfig Config;

        private LogManager(LogConfig config)
        {
            Config = config;
        }

        public static void Init(LogConfig config)
        {
            _instance = new LogManager(config);

            string initMessage = $@">>> LogManager initialized successfully. UTC time: {DateTime.UtcNow.ToString("R")}";

            _instance.GetLogger("XLog.LogManager").Info(initMessage);
        }

        public Logger GetLogger(string tag, LogConfig config = null)
        {
            if (string.IsNullOrEmpty(tag))
            {
                throw new ArgumentNullException(nameof(tag));
            }

            return new Logger(tag, config ?? Config);
        }

        public void Flush()
        {
            foreach (var target in Config.TargetConfigs)
            {
                target.Target.Flush();
            }
        }
    }
}
