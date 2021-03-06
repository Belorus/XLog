﻿using System;

namespace XLog
{
    public class Logger
    {
        private readonly LogConfig _config;
        private readonly long? _defaultCategory;

        public readonly string Tag;

        internal Logger(string tag, long? defaultCategory, LogConfig config)
        {
            Tag = tag;
            _defaultCategory = defaultCategory;
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
            LogInternal(LogLevel.Trace, null, message, ex);
        }

        public void Trace(long category, string message, Exception ex = null)
        {
            LogInternal(LogLevel.Trace, category, message, ex);
        }

        public void TraceFormat(string format, params object[] args)
        {
            LogInternalFormat(LogLevel.Trace, null, null, format, args);
        }

        public void TraceFormat(long category, string format, params object[] args)
        {
            LogInternalFormat(LogLevel.Trace, category, null, format, args);
        }

        public void Debug(string message, Exception ex = null)
        {
            LogInternal(LogLevel.Debug, null, message, ex);
        }

        public void Debug(long category, string message, Exception ex = null)
        {
            LogInternal(LogLevel.Debug, category, message, ex);
        }

        public void DebugFormat(string format, params object[] args)
        {
            LogInternalFormat(LogLevel.Debug, null, null, format, args);
        }

        public void DebugFormat(long category, string format, params object[] args)
        {
            LogInternalFormat(LogLevel.Debug, category, null, format, args);
        }

        public void Info(string message, Exception ex = null)
        {
            LogInternal(LogLevel.Info, null, message, ex);
        }

        public void Info(long category, string message, Exception ex = null)
        {
            LogInternal(LogLevel.Info, category, message, ex);
        }

        public void InfoFormat(string format, params object[] args)
        {
            LogInternalFormat(LogLevel.Info, null, null, format, args);
        }

        public void InfoFormat(long category, string format, params object[] args)
        {
            LogInternalFormat(LogLevel.Info, category, null, format, args);
        }

        public void Warn(string message, Exception ex = null)
        {
            LogInternal(LogLevel.Warn, null, message, ex);
        }

        public void Warn(long category, string message, Exception ex = null)
        {
            LogInternal(LogLevel.Warn, category, message, ex);
        }

        public void WarnFormat(string format, params object[] args)
        {
            LogInternalFormat(LogLevel.Warn, null, null, format, args);
        }

        public void WarnFormat(long category, string format, params object[] args)
        {
            LogInternalFormat(LogLevel.Warn, category, null, format, args);
        }

        public void Error(string message, Exception ex = null)
        {
            LogInternal(LogLevel.Error, null, message, ex);
        }

        public void Error(long category, string message, Exception ex = null)
        {
            LogInternal(LogLevel.Error, category, message, ex);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            LogInternalFormat(LogLevel.Error, null, null, format, args);
        }

        public void ErrorFormat(long category, string format, params object[] args)
        {
            LogInternalFormat(LogLevel.Error, category, null, format, args);
        }

        public void Fatal(string message, Exception ex = null)
        {
            LogInternal(LogLevel.Fatal, null, message, ex);
        }

        public void Fatal(long category, string message, Exception ex = null)
        {
            LogInternal(LogLevel.Fatal, category, message, ex);
        }

        public void FatalFormat(string format, params object[] args)
        {
            LogInternalFormat(LogLevel.Fatal, null, null, format, args);
        }

        public void FatalFormat(long category, string format, params object[] args)
        {
            LogInternalFormat(LogLevel.Fatal, category, null, format, args);
        }

        public void Log(LogLevel logLevel, string message, Exception ex = null)
        {
            LogInternal(logLevel, null, message, ex);
        }

        public void Log(LogLevel logLevel, long category, string message, Exception ex = null)
        {
            LogInternal(logLevel, category, message, ex);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _config.Levels[(int)logLevel];
        }

        private void LogInternal(LogLevel logLevel, long? category, string message, Exception ex)
        {
            long coercedCategory = category ?? _defaultCategory ?? 0L;

            if (!_config.IsEnabled || !_config.Levels[(int)logLevel] || !_config.CategoryRegistrar.IsEnabled(coercedCategory))
            {
                return;
            }

            PerformWrite(logLevel, message, coercedCategory, ex);
        }

        private void LogInternalFormat(LogLevel logLevel, long? category, Exception ex, string format, params object[] args)
        {
            long coercedCategory = category ?? _defaultCategory ?? 0L;

            if (!_config.IsEnabled || !_config.Levels[(int)logLevel] || !_config.CategoryRegistrar.IsEnabled(coercedCategory))
            {
                return;
            }

            PerformWrite(logLevel, string.Format(format, args), coercedCategory, ex);
        }

        private void PerformWrite(LogLevel logLevel, string message, long category, Exception ex)
        {
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
                        System.Diagnostics.Debug.WriteLine(string.Format("Target write failed. --> {0}", e.ToString()));
                    }
                }
            }
        }
    }
}
