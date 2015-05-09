using System;

namespace XLog
{
    internal class Logger : ILogger
    {
        private readonly LogConfig _config;
        public readonly string Tag;

        internal Logger(string tag, LogConfig config)
        {
            Tag = tag;
            _config = config;
        }

        string ILogger.Tag
        {
            get { return Tag; }
        }

        public void Trace(string message, Exception ex = null)
        {
            Log(LogLevel.Trace, message, ex);
        }

        public void Trace(string message, params object[] ps)
        {
            Log(LogLevel.Trace, message, ps);
        }

        public void Debug(string message, Exception ex = null)
        {
            Log(LogLevel.Debug, message, ex);
        }

        public void Debug(string message, params object[] ps)
        {
            Log(LogLevel.Debug, message, ps);
        }

        public void Info(string message, Exception ex = null)
        {
            Log(LogLevel.Info, message, ex);
        }

        public void Info(string message, params object[] ps)
        {
            Log(LogLevel.Info, message, ps);
        }

        public void Warn(string message, Exception ex = null)
        {
            Log(LogLevel.Warn, message, ex);
        }

        public void Warn(string message, params object[] ps)
        {
            Log(LogLevel.Warn, message, ps);
        }

        public void Error(string message, Exception ex = null)
        {
            Log(LogLevel.Error, message, ex);
        }

        public void Error(string message, params object[] ps)
        {
            Log(LogLevel.Error, message, ps);
        }

        public void Fatal(string message, Exception ex = null)
        {
            Log(LogLevel.Fatal, message, ex);
        }

        public void Fatal(string message, params object[] ps)
        {
            Log(LogLevel.Fatal, message, ps);
        }

        public void Log(int logLevel, string message, Exception ex)
        {
            LogInternal(logLevel, message, null, ex, false);
        }

        public void Log(int logLevel, string message, params object[] ps)
        {
            LogInternal(logLevel, message, ps, null, ps.Length > 0);
        }

        public bool IsEnabled(int logLevel)
        {
            return _config.IsLevelEnabled(logLevel);
        }

        private void LogInternal(int logLevel, string message, object[] ps, Exception ex, bool doFormat)
        {
            if (!_config.IsEnabled || !_config.IsLevelEnabled(logLevel))
            {
                return;
            }

            if (doFormat)
            {
                message = string.Format(message, ps);
            }

            var entry = new Entry(logLevel, Tag, message, ex);
            for (int index = 0; index < _config.TargetConfigs.Length; index++)
            {
                var c = _config.TargetConfigs[index];
                if (c.SupportsLevel(logLevel))
                {
                    try
                    {
                        c.Target.Write(entry);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Target write failed. --> {0}", e);
                    }
                }
            }
        }
    }
}
