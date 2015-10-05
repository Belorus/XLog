using System;

namespace XLog
{
    public class LogManager
    {
        private static LogManager _instance;

        public static LogManager Default
        {
            get { return _instance; }
        }

        public readonly LogConfig Config;
        
        private LogManager(LogConfig config)
        {
            Config = config;
        }

        public static void Init(LogConfig config)
        {
            _instance = new LogManager(config);
        }

        public Logger GetLogger(string tag, LogConfig config = null)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentNullException("tag");
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
