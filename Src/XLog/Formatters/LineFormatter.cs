using System;
using System.Text;

namespace XLog.Formatters
{
    public class LineFormatter : IFormatter
    {
        public string Format(Entry entry)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(entry.TimeStamp.ToLocalTime().ToString("HH:mm:ss:fff"));
            stringBuilder.Append("|");
            stringBuilder.Append(entry.LevelStr);
            stringBuilder.Append("|");
            stringBuilder.Append(Environment.CurrentManagedThreadId);
            stringBuilder.Append("|");
            stringBuilder.Append(entry.Tag);
            stringBuilder.Append("|");
            stringBuilder.Append(entry.Message);
            if (entry.Exception != null)
            {
                stringBuilder.Append(" --> ");
                stringBuilder.Append(entry.Exception);
            }
            return stringBuilder.ToString();
        }
    }
}
