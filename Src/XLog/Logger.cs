using System;

namespace XLog
{
    public class Logger
    {
        private readonly LogConfig _config;
        public readonly string Tag;

        internal Logger(string tag, LogConfig config)
        {
            Tag = tag;
            _config = config;
        }

        public bool IsTraceEnabled
        {
            get { return _config.IsLevelEnabled((int)LogLevel.Trace); }
        }

        public bool IsDebugEnabled
        {
            get { return _config.IsLevelEnabled((int)LogLevel.Debug); }
        }

        public bool IsInfoEnabled
        {
            get { return _config.IsLevelEnabled((int)LogLevel.Info); }
        }

        public bool IsErrorEnabled
        {
            get { return _config.IsLevelEnabled((int)LogLevel.Error); }
        }

        public bool IsFatalEnabled
        {
            get { return _config.IsLevelEnabled((int)LogLevel.Fatal); }
        }

        public bool IsLevelEnabled(LogLevel level)
        {
            return _config.IsLevelEnabled((int) level);
        }

        public void Trace(string message, Exception ex = null)
        {
            Log(LogLevel.Trace, 0L, message, ex);
        }

        public void Trace(long category, string message, Exception ex = null)
        {
            Log(LogLevel.Trace, category, message, ex);
        }

        public void Debug(string message, Exception ex = null)
        {
            Log(LogLevel.Debug, 0L, message, ex);
        }

        public void Debug(long category, string message, Exception ex = null)
        {
            Log(LogLevel.Debug, category, message, ex);
        }

        public void Info(string message, Exception ex = null)
        {
            Log(LogLevel.Info, 0L, message, ex);
        }

        public void Info(long category, string message, Exception ex = null)
        {
            Log(LogLevel.Info, category, message, ex);
        }

        public void Warn(string message, Exception ex = null)
        {
            Log(LogLevel.Warn, 0L, message, ex);
        }

        public void Warn(long category, string message, Exception ex = null)
        {
            Log(LogLevel.Warn, category, message, ex);
        }

        public void Error(string message, Exception ex = null)
        {
            Log(LogLevel.Error, 0L, message, ex);
        }

        public void Error(long category, string message, Exception ex = null)
        {
            Log(LogLevel.Error, category, message, ex);
        }

        public void Fatal(string message, Exception ex = null)
        {
            Log(LogLevel.Fatal, 0L, message, ex);
        }

        public void Fatal(long category, string message, Exception ex = null)
        {
            Log(LogLevel.Fatal, category, message, ex);
        }

        public void Log(LogLevel logLevel, string message, Exception ex = null)
        {
            Log(logLevel, 0L, message, ex);
        }

        public void Log(LogLevel logLevel, long category, string message, Exception ex = null)
        {
            LogInternal(logLevel, message, category, ex);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _config.Levels[(int)logLevel];
        }

        private void LogInternal(LogLevel logLevel, string message, long category, Exception ex)
        {
            if (!_config.IsEnabled || !_config.Levels[(int)logLevel] || !_config.CategoryRegistrar.IsEnabled(category))
            {
                return;
            }

            var entry = new Entry(logLevel, Tag, message, category, ex);

            for (int index = 0; index < _config.TargetConfigs.Count; index++)
            {
                var c = _config.TargetConfigs[index];
                if (c.SupportsLevel(logLevel))
                {
                    try
                    {
                        c.Target.Write(entry, _config.Formatter);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("Target write failed. --> {0}", e));
                    }
                }
            }
        }
    }
}
