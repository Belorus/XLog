using System;
using System.Text;

namespace XLog.Formatters
{
    public class LineFormatter : IFormatter
    {
        public string Format(Entry entry)
        {
            var builder = new StringBuilder();
            builder.Append(entry.TimeStamp.ToLocalTime().ToString("HH:mm:ss:fff"));
            builder.Append("|");
            builder.Append(entry.LevelStr);
            builder.Append("|");
            builder.Append(Environment.CurrentManagedThreadId);
            builder.Append("|");
            builder.Append(entry.Tag);
            builder.Append("|");
            builder.Append(entry.Message);
            if (entry.Exception != null)
            {
                builder.Append(" --> ");
                builder.Append(entry.Exception);
            }
            return builder.ToString();
        }
    }
}
