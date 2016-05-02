using System;
using Android.Util;
using XLog.Formatters;

namespace XLog.Android.Targets
{
    public class LogcatTarget : Target
    {
        private readonly string _tag;

        public LogcatTarget(string tag)
            : this(null, tag)
        {
        }

        public LogcatTarget(IFormatter formatter, string tag)
            : base(formatter)
        {
            _tag = tag;
        }

        public override void Write(Entry entry, IFormatter formatter)
        {
            var contents = (Formatter ?? formatter).Format(entry);
            switch (entry.Level)
            {
                case LogLevel.Trace:
                    Log.Verbose(_tag, contents);
                    break;
                case LogLevel.Debug:
                    Log.Debug(_tag, contents);
                    break;
                case LogLevel.Info:
                    Log.Info(_tag, contents);
                    break;
                case LogLevel.Warn:
                    Log.Warn(_tag, contents);
                    break;
                case LogLevel.Error:
                    Log.Error(_tag, contents);
                    break;
                case LogLevel.Fatal:
                    Log.WriteLine(LogPriority.Assert, _tag, contents);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Write(string content)
        {
        }
    }
}