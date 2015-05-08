using System;
using System.Collections.Generic;

namespace XLog
{
    public class LogManager
    {
        private static LogManager _default;

        private readonly Dictionary<string, Logger> _loggers;
        private readonly object _loggersLock = new object();

        public readonly LogConfig DefaultConfig;

        public static LogManager Default
        {
            get { return _default; }
        }

        public static void Init(LogConfig config)
        {
            if (_default != null)
            {
                throw new InvalidOperationException();
            }

            _default = new LogManager(config);
        }

        private LogManager(LogConfig config)
        {
            DefaultConfig = config;
            _loggers = new Dictionary<string, Logger>(StringComparer.OrdinalIgnoreCase);
        }

        public ILogger GetLogger(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                throw new ArgumentNullException("tag");
            }

            ILogger result;
            lock (_loggersLock)
            {
                if (!_loggers.ContainsKey(tag))
                {
                    var logger = new Logger(tag, DefaultConfig);
                    System.Diagnostics.Debug.WriteLine("Created Logger '{0}'", tag);
                    _loggers[tag] = logger;
                }

                result = _loggers[tag];
            }

            return result;
        }
    }
}
