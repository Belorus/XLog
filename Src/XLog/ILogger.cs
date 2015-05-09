using System;

namespace XLog
{
    public interface ILogger
    {
        string Tag { get; }
        bool IsEnabled(int logLevel);
        void Trace(string message, Exception ex = null);
        void Trace(string message, params object[] ps);
        void Debug(string message, Exception ex = null);
        void Debug(string message, params object[] ps);
        void Info(string message, Exception ex = null);
        void Info(string message, params object[] ps);
        void Warn(string message, Exception ex = null);
        void Warn(string message, params object[] ps);
        void Error(string message, Exception ex = null);
        void Error(string message, params object[] ps);
        void Fatal(string message, Exception ex = null);
        void Fatal(string message, params object[] ps);
        void Log(int logLevel, string message, Exception ex);
        void Log(int logLevel, string message, params object[] ps);
    }
}
