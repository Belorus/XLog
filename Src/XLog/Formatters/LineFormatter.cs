using System;
using System.Text;

namespace XLog.Formatters
{
    public class LineFormatter : IFormatter
    {
        public string Format(Entry entry)
        {
            int length = 12 + 1 + 5 + 1 + 4 + 1 + entry.Tag.Length + 1 + entry.Message.Length;
            string exceptionStr = null;
            if (entry.Exception != null)
            {
                exceptionStr = entry.Exception.ToString();
                length += 5 + exceptionStr.Length;
            }

            var builder = new StringBuilder(length);
            builder.Append(entry.TimeStamp.ToLocalTime().ToString("HH:mm:ss:fff"));
            builder.Append("|");
            builder.Append(entry.LevelStr);
            builder.Append("|");
            builder.Append(Environment.CurrentManagedThreadId);
            builder.Append("|");
            builder.Append(entry.Tag);
            builder.Append("|");
            builder.Append(entry.Message);
            if (exceptionStr != null)
            {
                builder.Append(" --> ");
                builder.Append(exceptionStr);
            }
            System.Diagnostics.Debug.Assert(builder.Capacity == length);
            return builder.ToString();
        }
    }
}
