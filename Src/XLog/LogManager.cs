using System;
using System.Collections.Generic;

namespace XLog
{
    public class LogManager
    {
        private static LogManager _instance;

        public static LogManager Default
        {
            get { return _instance; }
        }

        private readonly Dictionary<string, Logger> _loggers;
        private readonly object _loggersLock = new object();

        public readonly LogConfig Config;
        
        public static void Init(LogConfig config)
        {
            _instance = new LogManager(config);
        }

        private LogManager(LogConfig config)
        {
            Config = config;
            _loggers = new Dictionary<string, Logger>(StringComparer.OrdinalIgnoreCase);
        }

        public Logger GetLogger(string tag, LogConfig config = null)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentNullException("tag");
            }

            Logger result;
            lock (_loggersLock)
            {
                if (!_loggers.TryGetValue(tag, out result))
                {
                    result = new Logger(tag, config ?? Config);
                    System.Diagnostics.Debug.WriteLine("Created Logger '{0}'", tag);
                    _loggers[tag] = result;
                }
            }

            return result;
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
